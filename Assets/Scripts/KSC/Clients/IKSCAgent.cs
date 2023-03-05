using System;

namespace KSC.Clients
{
    public interface IKSCAgent
    {
        event Action<Agent> OnAgentSet;
        event Action<Agent> OnAgentRemoved;
        event Action<IKSCAgent> OnHostNameChanged;
        event Action<IKSCAgent> OnIPAddressChanged;
        event Action<IKSCAgent> OnKSCNameUsingChanged;

        Agent Agent { get; }
        string HostName { get; }
        string IPAddress { get; }
        bool UseKSCAgentName { get; }

        void SetKSCAgent(Agent agent);
        void RemoveKSCAgent();
        void SetHostName(string hostName);
        void SetIP(string ip);
        void SetKSCNameUsing(bool use);
    }
}