namespace s2protocol.NET.S2Protocol;

internal sealed record S2TypeInfo(string TypeName, int Number)
{
    public List<S2TypeInfoElement> Elements { get; set; } = [];
    internal S2DecodeKind DecodeKind { get; private set; }
    internal IDecodeParameter[] Parameters { get; private set; } = [];

    internal void BuildDecoderMetadata()
    {
        DecodeKind = TypeName switch
        {
            "_array" => S2DecodeKind.Array,
            "_bitarray" => S2DecodeKind.BitArray,
            "_blob" => S2DecodeKind.Blob,
            "_bool" => S2DecodeKind.Bool,
            "_choice" => S2DecodeKind.Choice,
            "_fourcc" => S2DecodeKind.FourCc,
            "_int" => S2DecodeKind.Int,
            "_null" => S2DecodeKind.Null,
            "_optional" => S2DecodeKind.Optional,
            "_real32" => S2DecodeKind.Real32,
            "_real64" => S2DecodeKind.Real64,
            "_struct" => S2DecodeKind.Struct,
            _ => S2DecodeKind.Unknown,
        };

        List<IDecodeParameter> parameters = [];

        foreach (var element in Elements)
        {
            switch (element)
            {
                case S2TypeInfoTypeElement bounds:
                    if (bounds.Bounds.Max == -1)
                    {
                        parameters.Add(new TypeIdParameter((int)bounds.Bounds.Min));
                    }
                    else
                    {
                        parameters.Add(new BoundsParameter(bounds.Bounds.Min, bounds.Bounds.Max));
                    }
                    break;

                case S2TypeInfoMElement mElement:
                    parameters.Add(new FieldListParameter(
                        mElement.Elements.ConvertAll(e =>
                            new DecodeField(e.TypeName, (int)e.Bounds.Min, e.Bounds.Max)
                        )
                    ));
                    break;

                case DsTypeInfoChoiceElemet choiceElem:
                    var choices = new Dictionary<long, DecodeChoice>(choiceElem.Elements.Count);
                    foreach (var kv in choiceElem.Elements)
                    {
                        choices[kv.Key] = new DecodeChoice(kv.Value.TypeName, kv.Value.Number);
                    }
                    parameters.Add(new ChoiceParameter(choices));
                    break;

                default:
                    throw new NotSupportedException($"Unknown element type: {element.GetType().Name}");
            }
        }

        Parameters = [.. parameters];
    }
}

internal enum S2DecodeKind
{
    Unknown,
    Array,
    BitArray,
    Blob,
    Bool,
    Choice,
    FourCc,
    Int,
    Null,
    Optional,
    Real32,
    Real64,
    Struct,
}


internal abstract record S2TypeInfoElement;

internal sealed record S2TypeInfoTypeElement : S2TypeInfoElement
{
    public BoundsParameter Bounds { get; set; } = new(0, 0);
}

internal sealed record S2TypeInfoMElement : S2TypeInfoElement
{
    public List<S2MElement> Elements { get; set; } = [];
}

internal sealed record S2MElement
{
    public string TypeName { get; set; } = string.Empty;
    public BoundsParameter Bounds { get; set; } = new(0, 0);
}

internal sealed record DsTypeInfoChoiceElemet : S2TypeInfoElement
{
    public Dictionary<int, S2ChoiceElement> Elements { get; set; } = [];
}

internal sealed record S2ChoiceElement
{
    public string TypeName { get; set; } = string.Empty;
    public int Number { get; set; }
}

internal sealed record S2EventType(int TypeId, string Name);
