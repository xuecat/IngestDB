using System;
using System.Collections.Generic;

namespace IngestMatrixPlugin.Models.DB
{
    public partial class DbpMatrixinfo
    {
        public int Matrixid { get; set; }
        public int Matrixtypeid { get; set; }
        public string Matrixname { get; set; }
        public int Inportnum { get; set; }
        public int Outportnum { get; set; }
        public int Comport { get; set; }
        public string Otherparam { get; set; }
        public int Comportbaud { get; set; }
    }
}
