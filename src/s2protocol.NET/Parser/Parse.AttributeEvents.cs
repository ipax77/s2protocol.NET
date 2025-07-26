using s2protocol.NET.Models;

namespace s2protocol.NET.Parser; internal static partial class Parse
{
    internal static AttributeEvents GetAttributeEvents(Dictionary<string, object> attrDic)
    {
        int source = GetInt(attrDic, "source");
        int mapNameSpace = GetInt(attrDic, "mapNamespace");
        List<AttributeEventScope> scopes = GetAttributeScopes(attrDic);
        return new AttributeEvents(source, mapNameSpace, scopes);
    }

    private static List<AttributeEventScope> GetAttributeScopes(Dictionary<string, object> attrDic)
    {
        List<AttributeEventScope> scopesList = new List<AttributeEventScope>();

        if (attrDic.TryGetValue("scopes", out object? scopes))
        {
            if (scopes is Dictionary<object, object> scopesDic)
            {
                foreach (var scopeEnt in scopesDic)
                {
                    int? scope = scopeEnt.Key as int?;
                    if (scope != null && scopeEnt.Value is Dictionary<string, object> scopeDic)
                    {
                        scopesList.AddRange(GetAttributeScopes(scope.Value, scopeDic));
                    }
                }
            }
        }

        return scopesList;
    }

    private static List<AttributeEventScope> GetAttributeScopes(int scope, Dictionary<string, object> scopeDic)
    {
        List<AttributeEventScope> scopes = new();
        foreach (var ent in scopeDic)
        {
            if (!int.TryParse(ent.Key, out int scopeId) && scopeId == 0)
            {
                continue;
            }
            if (ent.Value is List<object> scopeList)
            {
                foreach (var listEnt in scopeList)
                {
                    if (listEnt is Dictionary<string, object> entDic)
                    {
                        int @namespace = GetInt(entDic, "namespace");
                        int attrid = GetInt(entDic, "attrid");
                        string value = GetString(entDic, "value");
                        scopes.Add(new AttributeEventScope(scope, scopeId, @namespace, attrid, value));
                    }
                }
            }
        }
        return scopes;
    }
}
