/*
 * MemorySharp Library
 * http://www.binarysharp.com/
 *
 * Copyright (C) 2012-2014 Jämes Ménétrey (a.k.a. ZenLulz).
 * This library is released under the MIT License.
 * See the file LICENSE for more information.
*/

using System;

namespace Binarysharp.MemoryManagement.Native.Enums
{

    #region FlashWindowFlags
    /// <summary>
    ///     Flash window flags list.
    /// </summary>
    [Flags]
    public enum FlashWindowFlags
    {
        /// <summary>
        ///     Flash both the window caption and taskbar button. This is equivalent to setting the <see cref="Caption" /> |
        ///     <see cref="Tray" /> flags.
        /// </summary>
        All = 0x3,

        /// <summary>
        ///     Flash the window caption.
        /// </summary>
        Caption = 0x1,

        /// <summary>
        ///     Stop flashing. The system restores the window to its original state.
        /// </summary>
        Stop = 0x0,

        /// <summary>
        ///     Flash continuously, until the <see cref="Stop" /> flag is set.
        /// </summary>
        Timer = 0x4,

        /// <summary>
        ///     Flash continuously until the window comes to the foreground.
        /// </summary>
        TimerNoForeground = 0x0C,

        /// <summary>
        ///     Flash the taskbar button.
        /// </summary>
        Tray = 0x2
    }
    #endregion FlashWindowFlags

    #region InputTypes
    #endregion InputTypes

    #region KeyboardFlags
    #endregion KeyboardFlags

    #region Keys
    #endregion Keys

    #region MemoryAllocationFlags
    #endregion MemoryAllocationFlags

    #region MemoryProtectionFlags
    #endregion MemoryProtectionFlags

    #region MemoryReleaseFlags
    #endregion MemoryReleaseFlags

    #region MemoryStateFlags
    #endregion MemoryStateFlags

    #region MemoryTypeFlags
    #endregion MemoryTypeFlags

    #region MouseFlags
    #endregion MouseFlags

    #region PebStructure
    #endregion PebStructure

    #region ProcessAccessFlags
    #endregion ProcessAccessFlags

    #region ProcessInformationClass
    #endregion ProcessInformationClass

    #region SystemMetrics
    #endregion SystemMetrics

    #region TebStructure
    #endregion TebStructure

    #region ThreadAccessFlags
    #endregion ThreadAccessFlags

    #region ThreadContextFlags
    #endregion ThreadContextFlags

    #region ThreadCreationFlags
    #endregion ThreadCreationFlags

    #region TranslationTypes
    #endregion TranslationTypes

    #region WaitReturnValues
    #endregion WaitReturnValues

    #region WindowsMessages
    #endregion WindowsMessages

    #region WindowStates
    #endregion WindowStates
}