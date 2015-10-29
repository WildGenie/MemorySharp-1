using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using MemorySharp.Tools.Logger;
using MemorySharp.Tools.Scripts;

namespace MemorySharp.Tools.Extensions
{
    /// <summary>
    ///     A class containing extension methods for time spans.
    /// </summary
    public static class TimeSpanExtensions
    {
        #region Methods
        public static DateTime ToDateTime(this TimeSpan self)
        {
            return new DateTime(1, 1, 1).Add(self);
        }

        public static TimeSpan TrimMilliseconds(this TimeSpan self)
        {
            return self.Add(TimeSpan.FromMilliseconds(-self.Milliseconds));
        }

        internal static void AddNamespacesToScriptManager(params string[] param)
        {
            var field =
                typeof (ScriptManager).GetFields(BindingFlags.Static | BindingFlags.NonPublic)
                    .FirstOrDefault(f => f.FieldType == typeof (List<string>));

            if (field == null)
            {
                Log.Error(
                    "RebornBuddy update has moved or changed the type we are modifying, try updating ExBuddy or contact the author ExMatt.");
                return;
            }

            try
            {
                var list = field.GetValue(null) as List<string>;
                if (list == null)
                {
                    return;
                }

                foreach (var ns in param.Where(ns => !list.Contains(ns)))
                {
                    list.Add(ns);
                    Log.Info($"Added namespace '{ns}' to ScriptManager");
                }
            }
            catch
            {
                Log.Error("Failed to add namespaces to ScriptManager, this can cause issues with some profiles.");
            }
        }
        #endregion
    }
}