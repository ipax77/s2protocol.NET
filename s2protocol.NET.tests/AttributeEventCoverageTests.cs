using s2protocol.NET.Models;
using s2protocol.NET.Mpq;
using s2protocol.NET.S2Protocol;
using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using Xunit;

namespace s2protocol.NET.tests;

public class AttributeEventCoverageTests
{
    [Fact]
    public void TypedDecoderPreservesStreamOrderAndBlizzardValueSemantics()
    {
        byte[] content = Encode(
            source: 7,
            mapNamespace: 0x12345678,
            declaredCount: 3,
            new TestAttribute(11, 101, 1, [0, (byte)'A', (byte)'B', 0]),
            new TestAttribute(12, 202, 2, [(byte)'A', 0, (byte)'B', 0]),
            new TestAttribute(13, 101, 1, [(byte)'T', (byte)'e', (byte)'s', (byte)'t']));

        AttributeEvents decoded = S2ProtocolVersion.DecodeReplayAttributeEvents(content);

        Assert.Equal(7, decoded.Source);
        Assert.Equal(0x12345678, decoded.MapNamespace);
        Assert.Collection(
            decoded.Attributes,
            value => AssertAttribute(value, 1, 11, 101, "AB"),
            value => AssertAttribute(value, 2, 12, 202, "A\0B"),
            value => AssertAttribute(value, 1, 13, 101, "Test"));
    }

    [Fact]
    public void RawDecoderPreservesRepeatedScopeAndAttributeEntries()
    {
        byte[] content = Encode(
            source: 3,
            mapNamespace: 99,
            declaredCount: 2,
            new TestAttribute(41, 501, 4, [(byte)'O', (byte)'n', (byte)'e', 0]),
            new TestAttribute(42, 501, 4, [(byte)'T', (byte)'w', (byte)'o', 0]));

        Dictionary<string, object> decoded = S2ProtocolVersion.DecodeReplayAttributeEventsRaw(content.AsMemory());
        var scopes = Assert.IsType<Dictionary<byte, Dictionary<uint, List<Dictionary<string, object>>>>>(decoded["scopes"]);
        List<Dictionary<string, object>> repeated = scopes[4][501];

        Assert.Equal(3L, decoded["source"]);
        Assert.Equal(99L, decoded["mapNamespace"]);
        Assert.Collection(
            repeated,
            value => AssertRawAttribute(value, 41, 501, "One"),
            value => AssertRawAttribute(value, 42, 501, "Two"));
    }

    [Theory]
    [InlineData(0u)]
    [InlineData(1u)]
    [InlineData(uint.MaxValue)]
    public void HeaderCountDoesNotLimitPayloadDecoding(uint declaredCount)
    {
        byte[] content = Encode(
            source: 1,
            mapNamespace: 2,
            declaredCount,
            new TestAttribute(10, 20, 1, [(byte)'A', 0, 0, 0]),
            new TestAttribute(11, 21, 2, [(byte)'B', 0, 0, 0]));

        AttributeEvents typed = S2ProtocolVersion.DecodeReplayAttributeEvents(content);
        Dictionary<string, object> raw = S2ProtocolVersion.DecodeReplayAttributeEventsRaw(content);
        var rawScopes = Assert.IsType<Dictionary<byte, Dictionary<uint, List<Dictionary<string, object>>>>>(raw["scopes"]);

        Assert.Equal(2, typed.Attributes.Count);
        Assert.Equal(2, rawScopes.Values.SelectMany(scope => scope.Values).Sum(values => values.Count));
    }

    [Fact]
    public void EmptyAndTruncatedPayloadsMatchDecoderContract()
    {
        AttributeEvents typed = S2ProtocolVersion.DecodeReplayAttributeEvents(ReadOnlyMemory<byte>.Empty);
        Dictionary<string, object> raw = S2ProtocolVersion.DecodeReplayAttributeEventsRaw(ReadOnlyMemory<byte>.Empty);

        Assert.Equal(0, typed.Source);
        Assert.Equal(0, typed.MapNamespace);
        Assert.Empty(typed.Attributes);
        Assert.Empty(raw);

        byte[] complete = Encode(
            source: 1,
            mapNamespace: 2,
            declaredCount: 1,
            new TestAttribute(3, 4, 5, [(byte)'T', (byte)'e', (byte)'s', (byte)'t']));
        byte[] truncated = complete[..^1];

        Assert.Throws<DecodeException>(() => S2ProtocolVersion.DecodeReplayAttributeEvents(truncated));
        Assert.Throws<DecodeException>(() => S2ProtocolVersion.DecodeReplayAttributeEventsRaw(truncated));
    }

    [Fact]
    public void CorrectedAliasesStaySynchronizedWithoutChangingJsonShape()
    {
        AttributeEventScope entry = new(2, 17, 3, 17, "Fast");
        AttributeEvents events = new(1, 99, [entry]);

#pragma warning disable CS0618 // Compatibility properties are intentionally verified.
        Assert.Equal(events.MapNamespace, events.MapNameSpace);
        Assert.Same(events.Attributes, events.Scopes);
        Assert.Equal(entry.AttributeId, entry.Attrid);
        Assert.Equal(entry.AttributeId, entry.SopeId);
#pragma warning restore CS0618

        string json = JsonSerializer.Serialize(events);
        Assert.Contains("\"MapNameSpace\":99", json, StringComparison.Ordinal);
        Assert.Contains("\"Scopes\":", json, StringComparison.Ordinal);
        Assert.Contains("\"Attrid\":17", json, StringComparison.Ordinal);
        Assert.DoesNotContain("\"MapNamespace\":", json, StringComparison.Ordinal);
        Assert.DoesNotContain("\"Attributes\":", json, StringComparison.Ordinal);
        Assert.DoesNotContain("\"AttributeId\":", json, StringComparison.Ordinal);

        AttributeEvents roundTripped = Assert.IsType<AttributeEvents>(JsonSerializer.Deserialize<AttributeEvents>(json));
        AttributeEventScope roundTrippedEntry = Assert.Single(roundTripped.Attributes);
        Assert.Equal(events.MapNamespace, roundTripped.MapNamespace);
        Assert.Equal(entry.AttributeId, roundTrippedEntry.AttributeId);
        Assert.Equal(entry.Value, roundTrippedEntry.Value);
    }

    [Theory]
    [InlineData("test1.SC2Replay")]
    [InlineData("test2.SC2Replay")]
    [InlineData("test3.SC2Replay")]
    [InlineData("test4.SC2Replay")]
    [InlineData("test5.SC2Replay")]
    [InlineData("test6.SC2Replay")]
    [InlineData("test7.SC2Replay")]
    [InlineData("test8.SC2Replay")]
    public void ReplayFixtureTypedAndRawDecodersAgree(string replayFile)
    {
        string replayPath = Path.Combine(AppContext.BaseDirectory, "replays", replayFile);
        using MPQArchive archive = new(replayPath);
        ReadOnlyMemory<byte> content = archive.ReadFile("replay.attributes.events")
            ?? throw new InvalidOperationException($"No attributes found in '{replayFile}'.");

        AttributeEvents typed = S2ProtocolVersion.DecodeReplayAttributeEvents(content);
        Dictionary<string, object> raw = S2ProtocolVersion.DecodeReplayAttributeEventsRaw(content);
        var rawScopes = Assert.IsType<Dictionary<byte, Dictionary<uint, List<Dictionary<string, object>>>>>(raw["scopes"]);

        Assert.Equal((long)typed.Source, raw["source"]);
        Assert.Equal((long)typed.MapNamespace, raw["mapNamespace"]);
        Assert.Equal(typed.Attributes.Count,
            rawScopes.Values.SelectMany(scope => scope.Values).Sum(values => values.Count));

        foreach (IGrouping<(int Scope, int AttributeId), AttributeEventScope> group in
                 typed.Attributes.GroupBy(value => (value.Scope, value.AttributeId)))
        {
            List<Dictionary<string, object>> rawValues = rawScopes[(byte)group.Key.Scope][(uint)group.Key.AttributeId];
            Assert.Equal(group.Select(value => value.Namespace), rawValues.Select(value => (int)(long)value["namespace"]));
            Assert.Equal(group.Select(value => value.Value), rawValues.Select(value => (string)value["value"]));
        }
    }

    private static void AssertAttribute(
        AttributeEventScope value,
        int scope,
        int @namespace,
        int attributeId,
        string text)
    {
        Assert.Equal(scope, value.Scope);
        Assert.Equal(@namespace, value.Namespace);
        Assert.Equal(attributeId, value.AttributeId);
        Assert.Equal(text, value.Value);
    }

    private static void AssertRawAttribute(
        Dictionary<string, object> value,
        long @namespace,
        long attributeId,
        string text)
    {
        Assert.Equal(@namespace, value["namespace"]);
        Assert.Equal(attributeId, value["attrid"]);
        Assert.Equal(text, value["value"]);
    }

    private static byte[] Encode(
        byte source,
        uint mapNamespace,
        uint declaredCount,
        params TestAttribute[] attributes)
    {
        byte[] content = new byte[9 + (attributes.Length * 13)];
        Span<byte> destination = content;
        destination[0] = source;
        BinaryPrimitives.WriteUInt32LittleEndian(destination[1..5], mapNamespace);
        BinaryPrimitives.WriteUInt32LittleEndian(destination[5..9], declaredCount);

        int offset = 9;
        foreach (TestAttribute attribute in attributes)
        {
            BinaryPrimitives.WriteUInt32LittleEndian(destination[offset..(offset + 4)], attribute.Namespace);
            BinaryPrimitives.WriteUInt32LittleEndian(destination[(offset + 4)..(offset + 8)], attribute.AttributeId);
            destination[offset + 8] = attribute.Scope;

            Span<byte> encodedValue = destination[(offset + 9)..(offset + 13)];
            attribute.DecodedValue.CopyTo(encodedValue);
            encodedValue.Reverse();
            offset += 13;
        }

        return content;
    }

    private readonly record struct TestAttribute(
        uint Namespace,
        uint AttributeId,
        byte Scope,
        byte[] DecodedValue);
}
