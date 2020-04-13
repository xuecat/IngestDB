using System;
using System.Collections.Generic;
using System.Text;

namespace IngestDBCore.Tool
{
    /// <summary>
    /// 时码处理类
    /// </summary>
    public class SB_TimeCode
    {
        public SB_TimeCode()
        {
            Hour = 0;
            Minute = 0;
            Second = 0;
            Frame = 0;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ui">无符号的整数，每8位分别存储时，分，秒，帧</param>
        public SB_TimeCode(uint ui)
        {
            Hour = (sbyte)((ui & 0xFF000000) >> 24);
            Minute = (sbyte)((ui & 0x00FF0000) >> 16);
            Second = (sbyte)((ui & 0x0000FF00) >> 8);
            Frame = (sbyte)((ui & 0x000000FF));
        }

        /// <summary>
        /// 通过时，分，秒，帧造成一个对象
        /// </summary>
        /// <param name="hour"></param>
        /// <param name="minute"></param>
        /// <param name="second"></param>
        /// <param name="frame"></param>
        public SB_TimeCode(int hour, int minute, int second, int frame)
        {
            Hour = hour;
            Minute = minute;
            Second = second;
            Frame = frame;
        }

        public uint GetUIntValue()
        {
            uint ui = (uint)((Hour << 24) + ((Minute << 16) & 0x00ff0000) + ((Second << 8) & 0x0000ff00) + (Frame & 0x000000ff));
            return ui;
        }

        public string ToTCString()
        {
            return string.Format("{0:d2}:{1:d2}:{2:d2}:{3:d2}", this.Hour, this.Minute, this.Second, this.Frame);
        }

        /// <summary>
        /// 可以用来入出点相减，得到两点之间的时间差
        /// </summary>
        /// <param name="tc1"></param>
        /// <param name="tc2"></param>
        /// <returns></returns>
        public static TimeSpan operator -(SB_TimeCode tc1, SB_TimeCode tc2)
        {
            //算法不是很精确的计算，尽量的扩大时间范围，确保时间段足够大
            int days = 0, hours = 0, minutes = 0, seconds = 0, milliseconds = 0;

            //如果tc1.Frame < tc2.Frame，不做处理的话，已经是在扩大时间范围了
            if (tc1.Frame > tc2.Frame)
            {
                tc1.Second++;
            }

            if (tc1.Second < tc2.Second)
            {
                tc1.Minute--;
                seconds = tc1.Second - tc2.Second + 60;
            }
            else
            {
                seconds = tc1.Second - tc2.Second;
            }

            if (tc1.Minute < tc2.Minute)
            {
                tc1.Hour--;
                minutes = tc1.Minute - tc2.Minute + 60;
            }
            else
            {
                minutes = tc1.Minute - tc2.Minute;
            }

            if (tc1.Hour < tc2.Hour)
            {
                throw new Exception(string.Format("Time code can not subtract,{0} < {1}", tc1.ToTCString(), tc2.ToTCString()));
            }
            else
            {
                hours = (tc1.Hour - tc2.Hour) % 24;
                days = (tc1.Hour - tc2.Hour) / 24;
            }

            TimeSpan ts = new TimeSpan(days, hours, minutes, seconds, milliseconds);
            return ts;
        }

        public int Hour { set; get; }
        public int Minute { set; get; }
        public int Second { set; get; }
        public int Frame { set; get; }
    }
}
