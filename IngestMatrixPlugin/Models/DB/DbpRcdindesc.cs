using System;
using System.Collections.Generic;

namespace IngestMatrixPlugin.Models.DB
{
    public partial class DbpRcdindesc
    {
        public int Signalsrcid { get; set; }
        public int Rcdeviceid { get; set; }
        public int Recinidx { get; set; }
        public int? Signalsource { get; set; }
    }
}
