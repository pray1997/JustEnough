using System;
using System.Collections.Generic;
using System.Text;

namespace JustEnough.TemporalCore
{
    public class TemporalQuery
    {
        protected string clauseTemplate;
        protected DateTime? DatetimeStart { get; set; }
        protected DateTime? DatetimeEnd { get; set; }
        public virtual string GetClause()
        {
            return string.Format(clauseTemplate, DatetimeStart, DatetimeEnd);
        }

    }

    public class NonVersionedQuery : TemporalQuery
    {
        public override string GetClause()
        {
            return null;
        }
    }

    public class AsOfTemporalQuery : TemporalQuery
    {
        public AsOfTemporalQuery(DateTime datetime)
        {
            DatetimeStart = datetime;
            clauseTemplate = " FOR SYSTEM_TIME AS OF '{0:yyyy-MM-dd HH:mm:ss.fffffff}' ";
        }
    }
}
