using NexusCataclysm.Client;

namespace NexusCataclysm.Core;
public interface ILauncher
{
    event EventHandler<AssemblyLoadEvent> BeforeAssemblyLoad;

    event EventHandler<AssemblyLoadEvent> AfterAssemblyLoad;

    event EventHandler<PluginLoadEvent> BeforePluginLoad;

    string HomeDirectory { get; set; }

    string PluginDirectory { get; set; }

    string ConfigurationDirectory { get; set; }
}
