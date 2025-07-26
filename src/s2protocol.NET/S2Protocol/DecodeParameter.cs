namespace s2protocol.NET.S2Protocol;

internal interface IDecodeParameter;

internal record BoundsParameter(long Min, long Max) : IDecodeParameter;
internal record FieldListParameter(List<DecodeField> Fields) : IDecodeParameter;
internal record ChoiceParameter(Dictionary<long, DecodeChoice> Choices) : IDecodeParameter;
internal record TypeIdParameter(int TypeId) : IDecodeParameter;
internal record Bounds(long Min, long Max);
internal sealed record DecodeField(string Name, int TypeId, long Tag);
internal sealed record DecodeChoice(string Name, int TypeId);