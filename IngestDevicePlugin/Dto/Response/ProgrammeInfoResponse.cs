﻿using System;
using System.Collections.Generic;
using System.Text;

namespace IngestDevicePlugin.Dto
{
    public class ProgrammeInfoResponse
    {
        public int ProgrammeId { set; get; }
        public string ProgrammeName { set; get; }
        public string ProgrammeDesc { set; get; }

        //高标清
        public int TypeId { set; get; }
        public ProgrammeType PgmType { set; get; }
        public ImageType ImageType { set; get; }
        public emSignalSource SignalSourceType { set; get; }
        public int PureAudio { set; get; }
        public int CarrierID { set; get; }//运营商的ID
        public int GroupID { set; get; } // Add by chenzhi 2013-07-08 分组ID
    }
}
