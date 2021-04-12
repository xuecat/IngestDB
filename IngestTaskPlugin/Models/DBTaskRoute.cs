using ShardingCore.VirtualRoutes.Months;
using System;
using System.Collections.Generic;
using System.Text;

namespace IngestTaskPlugin.Models
{
    public class DBTaskRoute : AbstractSimpleShardingMonthKeyDateTimeVirtualTableRoute<DbpTask>
    {
        public override DateTime GetBeginTime()
        {
            return new DateTime(2021, 1, 01);
        }
    }

    public class DBTaskMetadataRoute : AbstractSimpleShardingMonthKeyDateTimeVirtualTableRoute<DbpTaskMetadata>
    {
        public override DateTime GetBeginTime()
        {
            return new DateTime(2021, 1, 01);
        }
    }
}
