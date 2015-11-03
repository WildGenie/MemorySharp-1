using System;
using Binarysharp.MemoryManagement.Logging;

namespace Binarysharp.MemoryManagement.Plugins
{
    /// <summary>
    ///     Class PluginContainer.
    /// </summary>
    public class PluginContainer
    {
        #region  Fields

        /// <summary>
        ///     The plugin.
        /// </summary>
        public IPlugin Plugin;

        #endregion

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="PluginContainer" /> class.
        /// </summary>
        /// <param name="plugin">The plugin.</param>
        public PluginContainer(IPlugin plugin)
        {
            Plugin = plugin;
            Enabled = false;
        }

        #endregion

        #region  Properties

        /// <summary>
        ///     The log
        /// </summary>
        private static ILog Log => FileLog.Instance;

        /// <summary>
        ///     Gets or sets the enabled.
        /// </summary>
        /// <value>The enabled.</value>
        internal bool Enabled { get; set; }

        /// <summary>
        ///     Gets or sets the value if the plugin is enabled or not.
        /// </summary>
        /// <value>If the plugin is enabled or not.</value>
        public bool IsEnabled
        {
            get { return Enabled; }
            internal set
            {
                if (Enabled == value) return;
                Enabled = value;

                if (Enabled)
                {
                    try
                    {
                        Plugin.OnEnabled();
                    }
                    catch (Exception ex)
                    {
                        Log.LogError("Exception Enabling plugin: " + Plugin.Name + Environment.NewLine + ex);
                    }
                }
                else
                {
                    try
                    {
                        Plugin.OnDisabled();
                    }
                    catch (Exception ex)
                    {
                        Log.LogError("Exception Enabling plugin: " + Plugin.Name + Environment.NewLine + ex);
                    }
                }
            }
        }

        #endregion
    }
}