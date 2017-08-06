using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Storage.Internal;
using System;
using System.Collections.Generic;
using System.Text;

namespace JustEnough.TemporalCore
{
    public class TemporalSqlGenerationHelper : SqlServerSqlGenerationHelper
    {
        public override string DelimitIdentifier(string identifier)
        {
            string versionedTable = null;
            var versionedQuerySettings = VersionedQuerySettings.Current;
            if (versionedQuerySettings != null && versionedQuerySettings.SystemVersionEnabled && VersionedQuerySettings.ParseVersionedIdentifier(identifier, out versionedTable))
            {
                identifier = versionedTable;
            }

            var str = base.DelimitIdentifier(identifier);

            return versionedTable != null ? str + versionedQuerySettings.ForClause : str;
        }

        public override void DelimitIdentifier(StringBuilder builder, string identifier)
        {
            string versionedTable = null;
            var versionedQuerySettings = VersionedQuerySettings.Current;
            if (versionedQuerySettings != null && versionedQuerySettings.SystemVersionEnabled && VersionedQuerySettings.ParseVersionedIdentifier(identifier, out versionedTable))
            {
                identifier = versionedTable;
            }

            base.DelimitIdentifier(builder, identifier);

            if (versionedTable != null)
            {
                builder.Append(versionedQuerySettings.ForClause);
            }
        }
    }
}
