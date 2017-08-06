using Microsoft.EntityFrameworkCore.Query.Sql.Internal;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore.Query.Expressions;
using Microsoft.EntityFrameworkCore.Query.Sql;

namespace JustEnough.TemporalCore
{
    public class TemporalSqlGeneratorFactory : SqlServerQuerySqlGeneratorFactory
    {
        public TemporalSqlGeneratorFactory(IRelationalCommandBuilderFactory commandBuilderFactory, ISqlGenerationHelper sqlGenerationHelper, IParameterNameGeneratorFactory parameterNameGeneratorFactory, IRelationalTypeMapper relationalTypeMapper) : base(commandBuilderFactory, sqlGenerationHelper, parameterNameGeneratorFactory, relationalTypeMapper)
        {
        }

        public override IQuerySqlGenerator CreateDefault(SelectExpression selectExpression) => new TemporalSqlGenerator(CommandBuilderFactory, SqlGenerationHelper, ParameterNameGeneratorFactory, RelationalTypeMapper, selectExpression);
    }
}
