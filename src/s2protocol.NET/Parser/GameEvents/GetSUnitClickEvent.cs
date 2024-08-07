﻿using IronPython.Runtime;
using s2protocol.NET.Models;

namespace s2protocol.NET.Parser;
internal static partial class Parse
{
    private static SUnitClickEvent GetSUnitClickEvent(PythonDictionary pydic, GameEvent gameEvent)
    {
        int unitTag = GetInt(pydic, "m_unitTag");
        return new SUnitClickEvent(gameEvent, unitTag);
    }
}
