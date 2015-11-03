using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Binarysharp.MemoryManagement.Helpers;
using Binarysharp.MemoryManagement.Logging;
using Binarysharp.MemoryManagement.Plugins;

namespace Binarysharp.MemoryManagement.Managment
{
    /// <summary>
    ///     Class PluginManager.
    /// </summary>
    public static class PluginManager
    {
        #region  Properties

        /// <summary>
        ///     Gets the plugins.
        /// </summary>
        /// <value>The plugins.</value>
        public static Dictionary<string, PluginContainer> Plugins { get; internal set; } =
            new Dictionary<string, PluginContainer>();

        /// <summary>
        ///     Gets the value stating if the plugin should pulse.
        /// </summary>
        /// <value>If the plugin should be pulsed.</value>
        public static bool ShouldPulse { get; internal set; } = true;

        /// <summary>
        ///     Gets the plugin thread.
        /// </summary>
        /// <value>The plugin thread.</value>
        public static Task PluginThread { get; internal set; }

        /// <summary>
        ///     The log instance.
        /// </summary>
        private static ILog Log => FileLog.Instance;

        #endregion

        #region Methods

        /// <summary>
        ///     Starts the plugins.
        /// </summary>
        public static void StartPlugins()
        {
            ShouldPulse = true;

            PluginThread = Task.Factory.StartNew(PluginPulse, TaskCreationOptions.LongRunning);
        }

        /// <summary>
        ///     Stops the plugins.
        /// </summary>
        public static void StopPlugins()
        {
            ShouldPulse = false; // stop pulsing plugins
            if (PluginThread == null) return;
            PluginThread.Wait(5000);
            PluginThread = null;
        }

        /// <summary>
        ///     Shuts down plugins.
        /// </summary>
        public static void ShutDownPlugins()
        {
            foreach (var value in Plugins.Values)
            {
                try
                {
                    value.Plugin.OnShutdown(); // call all plugin shutdown procedures
                }
                catch (Exception ex)
                {
                    Log.LogError("Exception shutting down plugin: " + value.Plugin.Name + Environment.NewLine + ex);
                }
            }
        }

        /// <summary>
        ///     Gets the enabled plugins.
        /// </summary>
        /// <returns>System.Collections.Generic.IEnumerable&lt;WowSharp.Interfaces.IPlugin&gt;.</returns>
        public static IEnumerable<IPlugin> GetEnabledPlugins()
        {
            return
                Plugins.Where(plugin => plugin.Value != null && plugin.Value.Enabled).Select(
                    container => container.Value.Plugin);
        }

        /// <summary>
        ///     Pulses all plugins.
        /// </summary>
        public static void PulseAllPlugins()
        {
            foreach (var enabledPlugin in GetEnabledPlugins())
            {
                try
                {
                    if (ShouldPulse)
                    {
                        enabledPlugin.OnPulse();
                    }
                    else
                    {
                        return;
                    }
                }
                catch (Exception ex)
                {
                    Log.LogWarning(enabledPlugin.Name + Environment.NewLine +
                                   ex);
                    Plugins[enabledPlugin.Name].Enabled = false;
                }
            }
        }

        /// <summary>
        ///     Plugin pulse.
        /// </summary>
        public static void PluginPulse()
        {
            while (ShouldPulse)
            {
                PulseAllPlugins();

                Thread.Sleep(1000/60); // About 60 FPS pulses
            }
        }

        /// <summary>
        ///     Loads the plugins.
        /// </summary>
        public static void LoadPlugins()
        {
            if (!Directory.Exists(ApplicationFinder.ApplicationPath + "\\Plugins"))
            {
                Directory.CreateDirectory(ApplicationFinder.ApplicationPath + "\\Plugins");
            }
            Plugins.Clear();
            var plugins = Directory.GetFiles(ApplicationFinder.ApplicationPath + "\\Plugins", "*.dll");

            foreach (var plugin in plugins)
            {
                try
                {
                    var asm = System.Reflection.Assembly.LoadFrom(plugin);
                    foreach (var instance in (from bin in asm.GetTypes()
                        where bin.IsClass && !bin.IsAbstract && typeof (IPlugin).IsAssignableFrom(bin)
                        select Activator.CreateInstance(bin)).OfType<IPlugin>())
                    {
                        instance.OnLoad();
                        Plugins[instance.Name] = new PluginContainer(instance);
                    }
                }
                catch (Exception ex)
                {
                    Log.LogWarning("Failed to load Plugin: " + plugin + Environment.NewLine + ex);
                }
            }
        }

        #endregion
    }
}