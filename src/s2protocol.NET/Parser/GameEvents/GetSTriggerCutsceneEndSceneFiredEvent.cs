using IronPython.Runtime;
using s2protocol.NET.Models;

namespace s2protocol.NET.Parser;
internal partial class Parse
{
    private static STriggerCutsceneEndSceneFiredEvent GetSTriggerCutsceneEndSceneFiredEvent(PythonDictionary pydic, GameEvent gameEvent)
    {
        long m_cutsceneId = GetBigInt(pydic, "m_cutsceneId");
        return new STriggerCutsceneEndSceneFiredEvent(gameEvent, m_cutsceneId);
    }
}
