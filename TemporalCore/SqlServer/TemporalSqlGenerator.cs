using Microsoft.EntityFrameworkCore.Query.Expressions;
using Microsoft.EntityFrameworkCore.Query.Sql.Internal;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Linq.Expressions;
using JetBrains.Annotations;
using Remotion.Linq.Clauses;
using System.Linq;

namespace JustEnough.TemporalCore
{
    public class TemporalSqlGenerator : SqlServerQuerySqlGenerator
    {
        public TemporalSqlGenerator(IRelationalCommandBuilderFactory relationalCommandBuilderFactory, ISqlGenerationHelper sqlGenerationHelper, IParameterNameGeneratorFactory parameterNameGeneratorFactory, IRelationalTypeMapper relationalTypeMapper, SelectExpression selectExpression) : base(relationalCommandBuilderFactory, sqlGenerationHelper, parameterNameGeneratorFactory, relationalTypeMapper, selectExpression)
        {
        }

        public override Expression VisitTable(TableExpression tableExpression)
        {
            var exp = tableExpression;
            var versionedQuerySettings = VersionedQuerySettings.Current;
            if (versionedQuerySettings != null && versionedQuerySettings.SystemVersionEnabled && versionedQuerySettings.IsTableVersioned(exp.Schema, exp.Table))
            {
                var identifier = VersionedQuerySettings.ConvertToVersionedIdentifier(exp.Table);
                tableExpression = new TableExpression(identifier, tableExpression.Schema, tableExpression.Alias, tableExpression.QuerySource);
            }
                
            return base.VisitTable(tableExpression);
        }
    }
}
