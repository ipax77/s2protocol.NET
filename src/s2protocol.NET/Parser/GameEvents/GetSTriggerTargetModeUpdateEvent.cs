using IronPython.Runtime;
using s2protocol.NET.Models;

namespace s2protocol.NET.Parser;
internal partial class Parse
{
    private static STriggerTargetModeUpdateEvent GetSTriggerTargetModeUpdateEvent(PythonDictionary pydic, GameEvent gameEvent)
    {
        int m_abilCmdIndex = GetInt(pydic, "m_abilCmdIndex");
        int m_abilLink = GetInt(pydic, "m_abilLink");
        int m_state = GetInt(pydic, "m_state");
        return new STriggerTargetModeUpdateEvent(gameEvent, m_abilCmdIndex, m_abilLink, m_state);
    }
}
