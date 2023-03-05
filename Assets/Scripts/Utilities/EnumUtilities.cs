using KSC;

namespace Utilities
{
    public static class EnumUtilities
    {
        public static string StatusEnumToText(this AgentStatus agentStatus)
            => agentStatus switch
            {
                AgentStatus.OK => "В порядке",
                AgentStatus.Critical => "Критическое",
                AgentStatus.Warning => "Требует внимания",
                _ => agentStatus.ToString(),
            };
    }
}
