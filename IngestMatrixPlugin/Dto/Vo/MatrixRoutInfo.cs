using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IngestMatrixPlugin.Dto.Vo
{
	public class MatrixRoutInfo
	{
		public long lMatrixID { get; set; }
		public long lInPort { get; set; }
        public long lOutPort { get; set; }
        public long lVirtualOutPort { get; set; }
        public long lVirtualInPort { get; set; }
        public long lState { get; set; }
    }
}
