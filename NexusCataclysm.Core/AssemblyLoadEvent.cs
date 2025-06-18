using System.Reflection;

namespace NexusCataclysm.Client;

public class AssemblyLoadEvent : EventArgs
{
    required public string Path { get; set; }

    public Assembly? LoadedPlugin { get; set; }

    public bool Skip { get; set; } = false;
}
