using System;
using System.IO;
using System.Reflection;
using System.Text;
using System.Text.Json;
using Xunit;

namespace s2protocol.NET.tests;

public sealed class CliBehaviorTests
{
    private static readonly string[] VersionsArguments = ["--versions"];

    [Fact]
    public void VersionsDoesNotRequireReplayAndReadsJsonManifest()
    {
        using StringWriter output = new();
        using StringWriter error = new();

        int exitCode = s2cli.Program.Run(VersionsArguments, output, error);

        Assert.Equal(0, exitCode);
        Assert.Equal(string.Empty, error.ToString());
        Assert.Contains("'15405'", output.ToString(), StringComparison.Ordinal);
        Assert.Contains("'97563'", output.ToString(), StringComparison.Ordinal);
        Assert.DoesNotContain(".py", output.ToString(), StringComparison.Ordinal);
    }

    [Fact]
    public void AllWritesReplayHeaderFirst()
    {
        string? assemblyPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        Assert.NotNull(assemblyPath);
        string replayPath = Path.Combine(assemblyPath, "replays", "test10.SC2Replay");
        using PrefixTextWriter output = new(4096);
        using StringWriter error = new();

        int exitCode = s2cli.Program.Run(
            new[] { "--replay", replayPath, "--all", "--ndjson" },
            output,
            error);

        Assert.Equal(0, exitCode);
        Assert.Equal(string.Empty, error.ToString());
        string firstLine = output.ToString().Split(Environment.NewLine, 2, StringSplitOptions.None)[0];
        using JsonDocument header = JsonDocument.Parse(firstLine);
        Assert.Equal(96883, header.RootElement.GetProperty("BaseBuild").GetInt32());
    }

    private sealed class PrefixTextWriter(int capacity) : TextWriter
    {
        private readonly StringBuilder _content = new(capacity);

        public override Encoding Encoding => Encoding.UTF8;

        public override void Write(char value)
        {
            if (_content.Length < capacity)
            {
                _content.Append(value);
            }
        }

        public override void Write(string? value)
        {
            if (value is null || _content.Length >= capacity)
            {
                return;
            }

            int available = capacity - _content.Length;
            _content.Append(value.AsSpan(0, Math.Min(value.Length, available)));
        }

        public override string ToString() => _content.ToString();
    }
}
