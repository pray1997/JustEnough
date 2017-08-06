using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Microsoft.EntityFrameworkCore.Metadata;
using System.Collections.Concurrent;
using System.Reflection;
using System.Threading;
using Microsoft.Extensions.Logging;

namespace JustEnough.TemporalCore
{
    public class VersionedQuerySettings
    {
        public bool SystemVersionEnabled { get; set; }
        public string ForClause { get; set; }
        public Dictionary<string, bool> VersionedTables { get; set; }

        public static string SystemVersionPlaceHolder { get; } = "$VERIONED$::";

        public bool IsTableVersioned(string schema, string table)
        {
            var key = string.Join(".", new[] { schema, table });
            return VersionedTables.TryGetValue(key, out var enabled) && enabled;
        }

        public static string ConvertToVersionedIdentifier(string table)
        {
            return SystemVersionPlaceHolder + table;
        }

        public static bool ParseVersionedIdentifier(string identifier, out string table)
        {
            table = null;
            if (identifier == null || !identifier.StartsWith(SystemVersionPlaceHolder, StringComparison.OrdinalIgnoreCase)) return false;

            table = identifier.Substring(SystemVersionPlaceHolder.Length);
            return true;
        }


        private static AsyncLocal<VersionedQuerySettings> asyncLocalCurrent = new AsyncLocal<VersionedQuerySettings>();

        public static VersionedQuerySettings Current
        {
            get => asyncLocalCurrent.Value;
            set => asyncLocalCurrent.Value = value;
        }
    }


}
