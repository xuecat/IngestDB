using System;
using System.Collections.Generic;
using System.Text;

namespace IngestTaskPlugin.Dto
{
    //时间段
    public class TimePeriod
    {
        public TimePeriod()
        {
            Id = 0;
            StartTime = DateTime.MinValue;
            EndTime = DateTime.MinValue;
        }

        public TimePeriod(int id, DateTime start, DateTime end)
        {
            Id = id;
            StartTime = start;
            EndTime = end;
        }

        //获得两个时间段的交集
        //算法，首先判断没有交集的情况，如果有交集，那么取大的开始时间，小的结束时间
        public static TimePeriod GetIntersect(TimePeriod tp1, TimePeriod tp2)
        {
            if (tp1.EndTime <= tp2.StartTime || tp2.EndTime <= tp1.StartTime)
            {
                return null;
            }

            TimePeriod tp3 = new TimePeriod();

            tp3.StartTime = tp1.StartTime > tp2.StartTime ? tp1.StartTime : tp2.StartTime;
            tp3.EndTime = tp1.EndTime < tp2.EndTime ? tp1.EndTime : tp2.EndTime;

            return tp3;
        }

        public static int CompareDescByStartTime(TimePeriod tp1, TimePeriod tp2)
        {
            if (tp1 == null)
            {
                if (tp2 == null)
                {
                    return 0;
                }
                else
                {
                    return 1;
                }
            }
            else
            {
                if (tp2 == null)
                {
                    return -1;
                }
                else
                {
                    if (tp1.StartTime > tp2.StartTime)
                    {
                        return -1;
                    }

                    if (tp1.StartTime < tp2.StartTime)
                    {
                        return 1;
                    }

                    return 0;
                }
            }
        }

        public static int CompareAscByStartTime(TimePeriod tp1, TimePeriod tp2)
        {
            if (tp1 == null)
            {
                if (tp2 == null)
                {
                    return 0;
                }
                else
                {
                    return -1;
                }
            }
            else
            {
                if (tp2 == null)
                {
                    return 1;
                }
                else
                {
                    if (tp1.StartTime > tp2.StartTime)
                    {
                        return 1;
                    }

                    if (tp1.StartTime < tp2.StartTime)
                    {
                        return -1;
                    }

                    return 0;
                }
            }
        }

        public static int CompareDescByDuration(TimePeriod tp1, TimePeriod tp2)
        {
            if (tp1 == null)
            {
                if (tp2 == null)
                {
                    return 0;
                }
                else
                {
                    return 1;
                }
            }
            else
            {
                if (tp2 == null)
                {
                    return -1;
                }
                else
                {
                    if (tp1.Duration > tp2.Duration)
                    {
                        return -1;
                    }

                    if (tp1.Duration < tp2.Duration)
                    {
                        return 1;
                    }

                    return 0;
                }
            }
        }

        public int Id { get; set; }
        public DateTime StartTime { set; get; }
        public DateTime EndTime { set; get; }

        public TimeSpan Duration
        {
            get
            {
                return (EndTime - StartTime);
            }
        }
    }

    public class ChannelTimePeriods
    {
        public ChannelTimePeriods()
        {
            Periods = new List<TimePeriod>();
        }
        public ChannelTimePeriods(int channelId)
        {
            ChannelId = channelId;
            Periods = new List<TimePeriod>();
        }

        public int ChannelId { get; set; }
        public List<TimePeriod> Periods { get; set; }
    }

    public class VTRTimePeriods
    {
        public VTRTimePeriods()
        {
            Periods = new List<TimePeriod>();
        }
        public VTRTimePeriods(int vtrId)
        {
            VTRId = vtrId;
            Periods = new List<TimePeriod>();
        }

        public int VTRId { get; set; }
        public List<TimePeriod> Periods { get; set; }
    }
}
