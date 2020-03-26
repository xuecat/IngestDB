using System;
using System.Collections.Generic;
using System.Text;

namespace IngestDevicePlugin.Dto.Response
{
    public class OldResponse
    {
    }
    public enum emSignalSource//信号来源
    {
        emSatlitlleSource = 0,//卫星
        emCtrlMatrixSource = 1,//总控矩阵
        emVideoServerSource = 2,//视频服务器 
        emVtrSource = 3,//VTR
        emMSVSource = 4,//MSV
        emXDCAM = 5,//蓝光
        emIPTS = 6,//IPTS流
        emStreamMedia = 7 //流媒体
    }
    public class RoterInportDesc
    {
        public int nRCDeviceID;
        public int nRCInportIdx;
        public int nSignalSrcID;
        public emSignalSource nSignalSource = emSignalSource.emSatlitlleSource;//信号来源， 0:卫星 1:总控矩阵 2 视频服务器 3: VTR 4: MSV  5 蓝光  其他以后再扩展
    }
    public class GetAllRouterInPortInfo_param
    {
        public List<RoterInportDesc> inportDescs;
        public int nVaildDataCount;
        public string errStr;
        public bool bRet;
    }
}
