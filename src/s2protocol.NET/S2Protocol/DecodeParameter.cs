namespace s2protocol.NET.S2Protocol;

internal interface IDecodeParameter;

internal sealed record BoundsParameter(long Min, long Max) : IDecodeParameter;
internal sealed record FieldListParameter(List<DecodeField> Fields) : IDecodeParameter;
internal sealed record ChoiceParameter(Dictionary<long, DecodeChoice> Choices) : IDecodeParameter;
internal sealed record TypeIdParameter(int TypeId) : IDecodeParameter;
internal sealed record DecodeField(string Name, int TypeId, long Tag);
internal sealed record DecodeChoice(string Name, int TypeId);