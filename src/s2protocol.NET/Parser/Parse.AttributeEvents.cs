using IronPython.Runtime;
using s2protocol.NET.Models;

namespace s2protocol.NET.Parser;

internal partial class Parse
{
    internal static AttributeEvents GetAttributeEvents(dynamic pydic)
    {
        PythonDictionary? attrDic = pydic as PythonDictionary;
        if (attrDic != null)
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
            PythonDictionary? scopesDic = attrDic["scopes"] as PythonDictionary;
            if (scopesDic != null)
            {
                foreach (var scopeEnt in scopesDic)
                {
                    int? scope = scopeEnt.Key as int?;
                    PythonDictionary? scopeDic = scopeEnt.Value as PythonDictionary;
                    if (scope != null && scopeDic != null)
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
        List<AttributeEventScope> scopes = new List<AttributeEventScope>();
        foreach (var ent in scopeDic)
        {
            int? scopeId = ent.Key as int?;
            List? scopeList = ent.Value as List;
            if (scopeId != null && scopeList != null)
            {
                foreach (var listEnt in scopeList)
                {
                    PythonDictionary? entDic = listEnt as PythonDictionary;
                    if (entDic != null)
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
