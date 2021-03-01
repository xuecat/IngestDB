using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace IngestDevicePlugin.Models
{
    [Table("dbp_area")]
    public partial class DbpArea
    {
        [Key, Column("ID", TypeName = "int(11)")]
        public int Id { get; set; }
        [Column("NAME", TypeName = "varchar(255)")]
        public string Name { get; set; }
        [Column("MATRIXTYPEID", TypeName = "int(11)")]
        public int MatrixTypeId { get; set; }
        [Column("MATRIXNAME", TypeName = "varchar(512)")]
        public string MatrixName { get; set; }
        [Column("COMPORT", TypeName = "int(11)")]
        public int ComPort { get; set; }
        [Column("COMPORTBAUD", TypeName = "int(11)")]
        public int ComPortBaud { get; set; }
        [Column("DEVICECTRLIP", TypeName = "varchar(11)")]
        public string DeviceCtrlIp { get; set; }
        [Column("DEVICECTRLPORT", TypeName = "int(11)")]
        public int DeviceCtrlPort { get; set; }

    }
}
