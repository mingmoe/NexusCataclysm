namespace NexusCataclysm.Core;
public class PluginLoadEvent : EventArgs
{
    /// <summary>
    /// Gets or sets the plugin, null to skip the plugin loading.
    /// </summary>
    required public Type? PluginType { get; set; }
}
