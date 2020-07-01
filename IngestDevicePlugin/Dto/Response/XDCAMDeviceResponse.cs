using System;
using System.Collections.Generic;
using System.Text;

namespace IngestDevicePlugin.Dto.Response
{
    public class XDCAMDeviceResponse
    {
        /// <summary> 设备ID </summary>
        public int DeviceId { get; set; }             //设备ID
        /// <summary> 设备类型 </summary>
        public int DeviceType { get; set; }           //设备类型
        /// <summary> 设备名称 </summary>
        public string DeviceName { get; set; }         //设备名称
        /// <summary> 设备描述 </summary>
        public string DeviceDesc { get; set; }         //设备描述
        /// <summary> 设备地址 </summary>
        public string DeviceAddr { get; set; }         //设备地址
        /// <summary> 设备登陆名 </summary>
        public string LoginName { get; set; }          //设备登陆名
        /// <summary> 设备登陆密码 </summary>
        public string LoginPwd { get; set; }           //设备登陆密码
        /// <summary> 存储路径 </summary>
        public string StoragePath { get; set; }        //存储路径
        /// <summary> 工作模式 </summary>
        public XDCAMWorkMode WorkMode { get; set; } = XDCAMWorkMode.XDCAM_MANUUPLOAD;
        /// <summary> 控制设备的xdcam server的ip </summary>
        public string ServerIp { get; set; }      //控制设备的xdcam server的ip
        /// <summary> 设备的当前disc的id，无disc即为-1 </summary>
        public int DiscId { get; set; }         //设备的当前disc的id，无disc即为-1
        /// <summary> 设备状态 </summary>
        public XDCAMDeviceState DeviceState { get; set; } = XDCAMDeviceState.DS_DISCONNECTED; //设备状态

    }

    public class XDCAMDeviceInfo
    {
        /// <summary> 设备ID </summary>
        public int nDeviceID;             //设备ID
        /// <summary> 设备类型 </summary>
        public int nDeviceType;           //设备类型
        /// <summary> 设备名称 </summary>
        public string strDeviceName;         //设备名称
        /// <summary> 设备描述 </summary>
        public string strDeviceDesc;         //设备描述
        /// <summary> 设备地址 </summary>
        public string strDeviceAddr;         //设备地址
        /// <summary> 设备登陆名 </summary>
        public string strLoginName;          //设备登陆名
        /// <summary> 设备登陆密码 </summary>
        public string strLoginPwd;           //设备登陆密码
        /// <summary> 存储路径 </summary>
        public string strStoragePath;        //存储路径
        /// <summary> 工作模式 </summary>
        public XDCAMWorkMode nWorkMode = XDCAMWorkMode.XDCAM_MANUUPLOAD;
        /// <summary> 控制设备的xdcam server的ip </summary>
        public string strServerIP;      //控制设备的xdcam server的ip
        /// <summary> 设备的当前disc的id，无disc即为-1 </summary>
        public int nDiscID;         //设备的当前disc的id，无disc即为-1
        /// <summary> 设备状态 </summary>
        public XDCAMDeviceState nDeviceState = XDCAMDeviceState.DS_DISCONNECTED; //设备状态
    }

    /// <summary>
    /// 
    /// </summary>
    public enum XDCAMWorkMode
    {
        /// <summary> XDCAM_AUTOUPLOAD </summary>
        XDCAM_AUTOUPLOAD,
        /// <summary> XDCAM_MANUUPLOAD </summary>
        XDCAM_MANUUPLOAD
    }

    public enum XDCAMDeviceState
    {
        /// <summary> DS_DISCONNECTED </summary>
        DS_DISCONNECTED,    //未与server连接
        /// <summary> DS_CONNECTED </summary>
        DS_CONNECTED,       //与server连接
        /// <summary> DS_NODISC </summary>
        DS_NODISC,
        /// <summary> DS_DISC_INSERT </summary>
        DS_DISC_INSERT,
        /// <summary> DS_DISC_READY </summary>
        DS_DISC_READY,
        /// <summary> DS_DISC_CHANGED </summary>
        DS_DISC_CHANGED
    }

}
