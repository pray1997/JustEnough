using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace JustEnough.TemporalCore
{
    public class TemporalQueryContext : IDisposable
    {
        TemporalQuery queryClause;
        DbContext dbContext;
        ILogger logger;

        static ConcurrentDictionary<Type, Dictionary<string, bool>> verionedTables = new ConcurrentDictionary<Type, Dictionary<string, bool>>();


        private Dictionary<string, bool> GetVersionedTablesFromDbConetxt(DbContext dbContext, ILogger logger)
        {
            return verionedTables.GetOrAdd(dbContext.GetType(), t =>
            {
                var tables = new Dictionary<string, bool>(StringComparer.OrdinalIgnoreCase);
                dbContext.Model.GetEntityTypes().ToList().ForEach(et => ScanEntityType(tables, et, logger));
                return tables;
            });
        }

        private static void ScanEntityType(Dictionary<string, bool> tables, IEntityType entity, ILogger logger)
        {
            try
            {
                var table = entity.Relational();
                var id = string.Join(".", new[] { table.Schema, table.TableName });
                if (!tables.ContainsKey(id))
                {
                    tables[id] = entity.ClrType.GetTypeInfo().CustomAttributes.Any(a => a.AttributeType == typeof(TemporalTableAttribute));
                }
            }
            catch (Exception ex)
            {
                if (logger != null)
                {
                    logger.LogCritical("Failed to get temporal metadata from entities: " + ex);
                }
            }
        }

        VersionedQuerySettings previousSettings = null;
        public TemporalQueryContext(DbContext dbContext, TemporalQuery queryClause, ILogger logger = null)
        {
            this.dbContext = dbContext;
            this.queryClause = queryClause;
            this.logger = logger;

            previousSettings = VersionedQuerySettings.Current;

            var forClause = queryClause.GetClause();
            if (string.IsNullOrEmpty(forClause))
            {
                VersionedQuerySettings.Current = null;
            }
            else
            {
                VersionedQuerySettings.Current = new VersionedQuerySettings
                {
                    SystemVersionEnabled = true,
                    ForClause = forClause,
                    VersionedTables = GetVersionedTablesFromDbConetxt(dbContext, logger)
                };
            }
        }

        public void Dispose()
        {
            VersionedQuerySettings.Current = previousSettings;
        }

        #region Create contexts for different types
        public static TemporalQueryContext AsOf(DbContext dbContext, DateTime datetime)
        {
            return new TemporalQueryContext(dbContext, new AsOfTemporalQuery(datetime));
        }
        public static TemporalQueryContext NonVersionedQuery(DbContext dbContext)
        {
            return new TemporalQueryContext(dbContext, new NonVersionedQuery());
        }
        #endregion
    }
}
