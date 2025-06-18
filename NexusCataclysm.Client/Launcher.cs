
using Autofac;
using Autofac.Extension;
using Microsoft.Extensions.Logging;
using NexusCataclysm.Core;
using NLog.Config;
using NLog.Extensions.Logging;
using Pillar;
using System.Diagnostics;
using System.Reflection;
using Utopia.Core;
using Environments = NexusCataclysm.Shared.Environments;

namespace NexusCataclysm.Client;

public partial class Launcher
{

    [STAThread]
    private static int Main(string[] args)
    {
        Thread.CurrentThread.Name = "main";
        ExtendedHost? host = null;
        try
        {
            host = CreateWithArguments(args).Launch();
            host.StartInCurrentThread();
        }
        catch (Exception ex)
        {
            Console.WriteLine("An error occurred while running the application.");
            Console.WriteLine(ex.ToColoredStringDemystified());
            return 1;
        }
        finally
        {
            host?.StopApplication();
        }
        return 0;
    }

    private Launcher()
    {
        HomeDirectory = Path.GetFullPath(
            Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)
            ?? ".");

        PluginDirectory = Path.Combine(HomeDirectory, "plugins");
        ConfigurationDirectory = Path.Combine(HomeDirectory, "configs");
        EnvironmentName = GetInitialEnvironmentName();
    }

    public string HomeDirectory { get; set; }

    public string PluginDirectory { get; set; }

    public string ConfigurationDirectory { get; set; }

    public string EnvironmentName { get; set; }

    [EmitEvent]
    private readonly WeakEvent<AssemblyLoadEvent> _beforeAssemblyLoad = new();

    [EmitEvent]
    private readonly WeakEvent<AssemblyLoadEvent> _afterAssemblyLoad = new();

    [EmitEvent]
    private readonly WeakEvent<PluginLoadEvent> _beforePluginLoad = new();

    protected virtual void CreateDirectories()
    {
        if (!Directory.Exists(PluginDirectory))
        {
            Directory.CreateDirectory(PluginDirectory);
        }
        if (!Directory.Exists(ConfigurationDirectory))
        {
            Directory.CreateDirectory(ConfigurationDirectory);
        }
    }

    protected virtual IEnumerable<string> GetPluginAssemblies()
    {
        return Directory.GetFiles(PluginDirectory, "*.dll", SearchOption.AllDirectories);
    }

    protected virtual string GetInitialEnvironmentName()
    {
        var envName =
#if DEBUG
            "Development"
#else
			"Production"
#endif
            ;
        return envName;
    }

    protected virtual ExtendedHostBuilder CreateDefaultBuilder()
    {
        var builder = new ExtendedHostBuilder(new ExtendedHostEnvironment(
            Environments.Client,
            HomeDirectory,
            EnvironmentName));

        builder.RegisterHost<Client>();

        builder.Builder.AddOptions();

        builder.Builder.Register(context =>
        {
            return Logging.CreateDefaultConfiguration();
        }).As<LoggingConfiguration>().SingleInstance();

        builder.Builder.Register(context =>
        {
            return Logging.CreateDefaultOptions();
        }).As<NLogProviderOptions>().SingleInstance();

        builder.Builder.AddLogging(context =>
        {
            var configuration = context.Resolve<LoggingConfiguration>();
            var options = context.Resolve<NLogProviderOptions>();
            return (loggingBuilder) =>
            {
                loggingBuilder.ClearProviders();
                loggingBuilder.AddNLog(configuration, options);
            };
        });

        builder.Builder.Configure<ClientOptions>(null, (options) =>
        {
            options.HomeDirectory = HomeDirectory;
            options.PluginDirectory = PluginDirectory;
            options.ConfigurationDirectory = ConfigurationDirectory;
        });

        return builder;
    }

    protected virtual void Initialize(ExtendedHostBuilder builder)
    {
        builder.Builder.RegisterType<FNAGame>().AsSelf().SingleInstance();
        return;
    }

    protected virtual void RegisterPlugin(ExtendedHostBuilder builder, Type plugin)
    {
        builder.Builder.RegisterType(plugin).As(typeof(IPlugin)).SingleInstance();
    }

    private List<Type> GetPlugins()
    {
        // load assemblies
        var pluginFiles = GetPluginAssemblies().ToArray();

        List<AssemblyLoadEvent> readyToLoad = new(pluginFiles.Length);
        foreach (var plugin in pluginFiles)
        {
            readyToLoad.Add(new AssemblyLoadEvent
            {
                Path = plugin
            });
        }

        foreach (var plugin in readyToLoad)
        {
            _beforeAssemblyLoad.Fire(this, plugin);
        }

        List<AssemblyLoadEvent> loadedAssembly = new(readyToLoad.Count);

        foreach (var plugin in readyToLoad)
        {
            try
            {
                if (plugin.Skip)
                {
                    continue;
                }
                if (plugin.LoadedPlugin == null)
                {
                    plugin.LoadedPlugin = Assembly.LoadFrom(plugin.Path);
                }
                loadedAssembly.Add(plugin);

                var prior = plugin.LoadedPlugin;
                _afterAssemblyLoad.Fire(this, plugin);

                Trace.Assert(plugin.LoadedPlugin == prior, "LoadedPlugin can not be modified in AfterAssemblyLoad event");
                Trace.Assert(plugin.Skip == true, "Skip can not modified in AfterAssemblyLoad event");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to load plugin {plugin.Path}: {ex.ToColoredStringDemystified()}");
            }
        }

        List<Type> pluginTypes = new(loadedAssembly.Count);

        foreach (var plugin in loadedAssembly)
        {
            try
            {
                var assembly = plugin.LoadedPlugin;

                Trace.Assert(assembly != null);

                foreach (var type in assembly.ExportedTypes)
                {
                    if (type.GetCustomAttribute<PluginAttribute>() is not null)
                    {
                        Trace.Assert(type.IsAssignableTo(typeof(IPlugin)),
                            "Type with attribute PluginAttribute should inhert IPlugin");
                        pluginTypes.Add(type);
                    }
                    else if (type.GetCustomAttribute<ClientPluginAttribute>() is not null)
                    {
                        Trace.Assert(type.IsAssignableTo(typeof(IPlugin)),
                            "Type with attribute ClientPluginAttribute should inhert IPlugin");
                        pluginTypes.Add(type);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to add plugin {plugin.Path}: {ex.ToColoredStringDemystified()}");
            }
        }

        // fire event
        List<Type> fillteredPluginTypes = new(loadedAssembly.Count);

        foreach (var plugin in pluginTypes)
        {
            var e = new PluginLoadEvent()
            {
                PluginType = plugin
            };
            _beforePluginLoad.Fire(this, e);
            if (e.PluginType != null)
            {
                fillteredPluginTypes.Add(plugin);
            }
        }

        return fillteredPluginTypes;
    }

    public ExtendedHost Launch()
    {
        CreateDirectories();
        var plugins = GetPlugins();

        foreach (var plugin in plugins)
        {
            try
            {
                var method =
                    plugin.GetMethod(nameof(IPlugin.Initialize),
                    BindingFlags.Static | BindingFlags.Public,
                    [typeof(ILauncher)]);

                Debug.Assert(method != null);

                method.Invoke(null, [this]);
            }
            catch (Exception ex)
            {
                Console.WriteLine(
                    $"Failed to run plugin {plugin.FullName}(from {plugin.Assembly.Location}): {ex.ToColoredStringDemystified()}");
            }
        }

        var builder = CreateDefaultBuilder();
        plugins.ForEach((t) => RegisterPlugin(builder, t));
        Initialize(builder);
        return (ExtendedHost)builder.Build();
    }

    public static Launcher CreateWithArguments(string[] args)
    {
        var launcher = Create();

        int index = 0;

        while (index != args.Length)
        {
            var arg = args[index];

            if (arg.StartsWith("--home="))
            {
                launcher.HomeDirectory = arg["--home=".Length..].Trim('"', '\'');
            }
            else if (arg.StartsWith("--plugin="))
            {
                launcher.PluginDirectory = arg["--plugin=".Length..].Trim('"', '\'');
            }
            else if (arg.StartsWith("--config="))
            {
                launcher.ConfigurationDirectory = arg["--config=".Length..].Trim('"', '\'');
            }
            else
            {
                throw new ArgumentException("unknown argument: " + arg);
            }

            index++;
        }

        return launcher;
    }

    public static Launcher Create()
    {
        var launcher = new Launcher();
        return launcher;
    }
}
