namespace s2protocol.NET.S2Protocol;

internal sealed record S2TypeInfo(string TypeName, int Number)
{
    public List<S2TypeInfoElement> Elements { get; set; } = [];
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