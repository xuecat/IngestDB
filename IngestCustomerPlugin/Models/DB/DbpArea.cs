using System;
using System.Collections.Generic;

namespace IngestCustomerPlugin.Models
{
    public partial class DbpArea
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Matrixtypeid { get; set; }
        public string Matrixname { get; set; }
        public int Comport { get; set; }
        public int Comportbaud { get; set; }
        public string Devicectrlip { get; set; }
        public int Devicectrlport { get; set; }
    }
}
