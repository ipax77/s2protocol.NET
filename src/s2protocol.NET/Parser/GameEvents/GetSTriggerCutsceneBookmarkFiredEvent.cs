using IronPython.Runtime;
using s2protocol.NET.Models;

namespace s2protocol.NET.Parser;
internal partial class Parse
{
    private static STriggerCutsceneBookmarkFiredEvent GetSTriggerCutsceneBookmarkFiredEvent(PythonDictionary pydic, GameEvent gameEvent)
    {
        long m_cutsceneId = GetBigInt(pydic, "m_cutsceneId");
        string m_bookmarkName = GetString(pydic, "m_bookmarkName");
        return new STriggerCutsceneBookmarkFiredEvent(gameEvent, m_cutsceneId, m_bookmarkName);
    }
}
