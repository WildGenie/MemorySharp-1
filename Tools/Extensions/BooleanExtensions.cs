/**************************************************************************************
 *	File:		 BooleanExtensions.cs
 *	Description: Extension methods for the System.Boolean data type.
 *
 * 
 *	Author:		 infloper@gmail.com
 *	Created:	 6/14/2014 5:52:30 PM
 *	CLR ver:	 4.0.30319.18444
 * 
 **************************************************************************************
 * Changes history.
 **************************************************************************************
 * Date:		Author:				  Description:
 * --------		--------------------
 **************************************************************************************/

using System.Collections.Generic;

namespace MemorySharp.Tools.Extensions
{
    /// <summary>
    ///     Extension methods for the <see cref="System.Boolean" /> data type.
    /// </summary>
    public static class BooleanExtensions
    {
        #region  Fields
        /// <summary>
        /// </summary>
        /// <example>
        ///     <code>
        /// 	{ "1",		true  },
        /// 	{ "T",		true  },
        /// 	{ "TAK",	true  },
        /// 	{ "TRUE",	true  },
        /// 	{ "Y",		true  },
        /// 	{ "YES",	true  },
        /// 	{ "PRAWDA",	true  },
        /// 	{ "P",		true  },
        /// 	{ "0",		false },
        /// 	{ "N",		false },
        /// 	{ "NIE",	false },
        /// 	{ "FALSE",	false },
        /// 	{ "NO",		false },
        /// 	{ "FAŁSZ",	false },
        /// 	{ "FALSZ",	false },
        ///  { "F",		false }																};
        ///  </code>
        /// </example>
        public static readonly Dictionary<string, bool> BooleanMapping = new Dictionary<string, bool>
        {
            {"1", true},
            {"T", true},
            {"TAK", true},
            {"TRUE", true},
            {"Y", true},
            {"YES", true},
            {"PRAWDA", true},
            {"P", true},
            {"0", false},
            {"N", false},
            {"NIE", false},
            {"FALSE", false},
            {"NO", false},
            {"FAŁSZ", false},
            {"FALSZ", false},
            {"F", false}
        };
        #endregion
    }
}