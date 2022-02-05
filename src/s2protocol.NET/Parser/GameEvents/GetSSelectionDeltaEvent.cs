using IronPython.Runtime;
using s2protocol.NET.Models;

namespace s2protocol.NET.Parser;

internal partial class Parse
{
    private static SSelectionDeltaEvent GetSSelectionDeltaEvent(PythonDictionary gameDic, GameEvent gameEvent)
    {
        var delta = GetSelectionDeltaEventDelta(gameDic);
        int controlGroupId = GetInt(gameDic, "m_controlGroupId");
        return new SSelectionDeltaEvent(gameEvent, delta, controlGroupId);
    }

    private static SelectionDeltaEventDelta GetSelectionDeltaEventDelta(PythonDictionary pydic)
    {
        if (pydic.ContainsKey("m_delta"))
        {
            PythonDictionary? deltaDic = pydic["m_delta"] as PythonDictionary;
            if (deltaDic != null)
            {

                List<int> addUnitTags = GetIntList(deltaDic, "m_addUnitTags");
                List<SelectionDeltaEventDeltaSubGroup> subgroups = new List<SelectionDeltaEventDeltaSubGroup>();
                List<int> zeroIndices = new List<int>();

                if (deltaDic.TryGetValue("m_addSubgroups", out object? subGroups))
                {
                    if (subGroups != null)
                    {
                        List? subGroupList = subGroups as List;
                        if (subGroupList != null)
                        {


                            List<int> zeroindices = new List<int>();
                            foreach (var ent in subGroupList)
                            {
                                PythonDictionary? subDic = ent as PythonDictionary;
                                if (subDic != null)
                                {
                                    subgroups.Add(new SelectionDeltaEventDeltaSubGroup(
                                        GetInt(subDic, "m_unitLink"),
                                        GetInt(subDic, "m_subgroupPriority"),
                                        GetInt(subDic, "m_count"),
                                        GetInt(subDic, "m_intraSubgroupPriority")
                                    ));
                                }
                            }
                        }
                    }
                }

                if (deltaDic.TryGetValue("m_removeMask", out object? removeMask))
                {
                    PythonDictionary? removeDic = deltaDic["m_removeMask"] as PythonDictionary;
                    if (removeDic != null)
                    {
                        zeroIndices = GetIntList(removeDic, "ZeroIndices");
                    }
                }

                int subgroupIndex = GetInt(deltaDic, "m_subgroupIndex");
                return new SelectionDeltaEventDelta(addUnitTags, subgroups, zeroIndices, subgroupIndex);
            }
        }
        return new SelectionDeltaEventDelta(new List<int>(), new List<SelectionDeltaEventDeltaSubGroup>(), new List<int>(), 0);
    }
}
