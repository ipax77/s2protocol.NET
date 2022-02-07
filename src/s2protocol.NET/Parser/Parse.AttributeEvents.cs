using IronPython.Runtime;
using s2protocol.NET.Models;

namespace s2protocol.NET.Parser;

internal partial class Parse
{
    internal static AttributeEvents GetAttributeEvents(dynamic pydic)
    {
        if (pydic is PythonDictionary attrDic)
        {
            int source = GetInt(attrDic, "source");
            int mapNameSpace = GetInt(attrDic, "mapNamespace");
            List<AttributeEventScope> scopes = GetAttributeScopes(attrDic);
            return new AttributeEvents(source, mapNameSpace, scopes);
        }
        return new AttributeEvents(0, 0, new List<AttributeEventScope>());
    }

    private static List<AttributeEventScope> GetAttributeScopes(PythonDictionary attrDic)
    {
        List<AttributeEventScope> scopesList = new List<AttributeEventScope>();

        if (attrDic.TryGetValue("scopes", out object? scopes))
        {
            if (scopes is PythonDictionary scopesDic)
            {
                foreach (var scopeEnt in scopesDic)
                {
                    int? scope = scopeEnt.Key as int?;
                    if (scope != null && scopeEnt.Value is PythonDictionary scopeDic)
                    {
                        scopesList.AddRange(GetAttributeScopes(scope.Value, scopeDic));
                    }
                }
            }
        }

        return scopesList;
    }

    private static List<AttributeEventScope> GetAttributeScopes(int scope, PythonDictionary scopeDic)
    {
        List<AttributeEventScope> scopes = new();
        foreach (var ent in scopeDic)
        {
            int? scopeId = ent.Key as int?;
            if (scopeId != null && ent.Value is List scopeList)
            {
                foreach (var listEnt in scopeList)
                {
                    if (listEnt is PythonDictionary entDic)
                    {
                        int @namespace = GetInt(entDic, "namespace");
                        int attrid = GetInt(entDic, "attrid");
                        string value = GetString(entDic, "value");
                        scopes.Add(new AttributeEventScope(scope, scopeId.Value, @namespace, attrid, value));
                    }
                }
            }
        }
        return scopes;
    }
}
