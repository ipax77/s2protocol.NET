using s2protocol.NET.Models;

namespace s2protocol.NET.Parser;internal static partial class Parse
{
    private static SSelectionDeltaEvent GetSSelectionDeltaEvent(Dictionary<string, object> gameDic, GameEvent gameEvent)
    {
        var delta = GetSelectionDeltaEventDelta(gameDic);
        int controlGroupId = GetInt(gameDic, "m_controlGroupId");
        return new SSelectionDeltaEvent(gameEvent, delta, controlGroupId);
    }

    private static SelectionDeltaEventDelta GetSelectionDeltaEventDelta(Dictionary<string, object> pydic)
    {
        if (pydic.ContainsKey("m_delta"))
        {
            if (pydic["m_delta"] is Dictionary<string, object> deltaDic)
            {

                List<int> addUnitTags = GetIntList(deltaDic, "m_addUnitTags");
                List<SelectionDeltaEventDeltaSubGroup> subgroups = new();
                List<int> zeroIndices = new();

                if (deltaDic.TryGetValue("m_addSubgroups", out object? subGroups))
                {
                    if (subGroups != null)
                    {
                        if (subGroups is List<object> subGroupList)
                        {


                            foreach (var ent in subGroupList)
                            {
                                if (ent is Dictionary<string, object> subDic)
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
                    if (removeMask is Dictionary<string, object> removeDic)
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
