using System;

namespace KSC
{
    public enum AgentStatus
    {
        OK = 0,
        Critical,
        Warning
    }

    public class Agent : IDisposable
    {
        public event Action<Agent> OnDispose;
        public event Action<Agent> OnInfoUpdated;

        public string HostName { get; private set; }
        public string Name { get; private set; }
        public string Group { get; private set; }
        public string IP { get; private set; }
        public AgentStatus Status { get; private set; }

        public Agent(string hostName)
        {
            HostName = hostName;
        }

        public void UpdateAgentInfo(string name, string group, string ip, AgentStatus status)
        {
            Name = name;
            Group = group;
            IP = ip;
            Status = status;

            OnInfoUpdated?.Invoke(this);
        }

        public void Dispose()
        {
            OnDispose?.Invoke(this);
        }
    }
}
