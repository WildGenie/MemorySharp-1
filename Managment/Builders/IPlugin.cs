using System;

namespace Binarysharp.MemoryManagement.Managment.Builders
{
    /// <summary>
    ///     Base abstract Plugin class (credit to the DemonBuddy team for implementation ideas for this plugin interface)
    /// </summary>
    public interface IPlugin : INamedElement, IPulsableElement
    {
        /// <summary>
        ///     Current version of this plugin
        /// </summary>
        Version Version { get; }

        /// <summary>
        ///     The creator of this plugin
        /// </summary>
        string Author { get; }

        /// <summary>
        ///     Description to display on the plugin interface
        /// </summary>
        string Description { get; }

        /// <summary>
        ///     Work to be done when the plugin is disabled by the user
        /// </summary>
        void Disabled();

        /// <summary>
        ///     Work to be done when the plugin is loaded by the bot on startup
        /// </summary>
        void Initialize();
    }
}