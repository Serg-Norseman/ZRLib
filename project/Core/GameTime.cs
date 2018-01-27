/*
 *  "ZRLib", Roguelike games development Library.
 *  Copyright (C) 2015 by Serg V. Zhdanovskih.
 *
 *  This program is free software: you can redistribute it and/or modify
 *  it under the terms of the GNU General Public License as published by
 *  the Free Software Foundation, either version 3 of the License, or
 *  (at your option) any later version.
 *
 *  This program is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *  GNU General Public License for more details.
 *
 *  You should have received a copy of the GNU General Public License
 *  along with this program.  If not, see <http://www.gnu.org/licenses/>.
 */

using BSLib;

namespace ZRLib.Core
{
    public enum DayTime
    {
        dt_Undefined,

        dt_NightFH,
        dt_Midnight,
        dt_NightSH,
        dt_Dawn,
        dt_DayFH,
        dt_Noon,
        dt_DaySH,
        dt_Dusk
    }

    public enum Season
    {
        Winter,
        Spring,
        Summer,
        Autumn
    }

    public struct DayTimeRec
    {
        public const int dt_First = 1;
        public const int dt_Last = 8;

        public readonly int Value;
        public readonly float bRange;
        public readonly float eRange;
        public readonly float Brightness;
        public readonly float RadMod;

        public DayTimeRec(int value, float begRange, float endRange, float brightness, float radMod)
        {
            Value = value;
            bRange = begRange;
            eRange = endRange;
            Brightness = brightness;
            RadMod = radMod;
        }
    }

    public class GameTime : ICloneable<GameTime>
    {
        private static readonly byte[][] MonthDays = new byte[][] {
            new byte[] {
                31,
                28,
                31,
                30,
                31,
                30,
                31,
                31,
                30,
                31,
                30,
                31
            },
            new byte[] {
                31,
                29,
                31,
                30,
                31,
                30,
                31,
                31,
                30,
                31,
                30,
                31
            }
        };

        public short Year;
        public byte Month;
        public byte Day;
        public byte Hour;
        public byte Minute;
        public byte Second;
        public byte Dummy;

        public static readonly DayTimeRec[] DayTimes = new DayTimeRec[9];

        static GameTime()
        {
            DayTimes[0] = new DayTimeRec(0, 0.0f, 0.0f, 127, 1.0f); // 0.5
            DayTimes[1] = new DayTimeRec(1, 22.0f, 23.59f, 0.25f, 0.5f); // 0.125
            DayTimes[2] = new DayTimeRec(2, 0.0f, 0.0f, 0.125f, 0.5f); // 0.125
            DayTimes[3] = new DayTimeRec(3, 0.00f, 2.0f, 0.25f, 0.5f); // 0.125
            DayTimes[4] = new DayTimeRec(4, 2.0f, 6.0f, 0.5f, 0.75f); // 0.25
            DayTimes[5] = new DayTimeRec(5, 6.0f, 11.00f, 0.80f, 1.0f); // 0.5
            DayTimes[6] = new DayTimeRec(6, 11.00f, 13.00f, 1.0f, 1.0f); // 0.75
            DayTimes[7] = new DayTimeRec(7, 13.00f, 18.0f, 0.80f, 1.0f); // 0.5
            DayTimes[8] = new DayTimeRec(8, 18.0f, 22.0f, 0.5f, 0.75f); // 0.25
        }

        public GameTime()
        {

        }

        public GameTime(int year, int month, int day, int hour, int minute, int second)
        {
            Set(year, month, day, hour, minute, second);
        }

        public void Set(int year, int month, int day, int hour, int minute, int second)
        {
            Year = (short)year;
            Month = (byte)month;
            Day = (byte)day;
            Hour = (byte)hour;
            Minute = (byte)minute;
            Second = (byte)second;
            Dummy = 0;
        }

        public virtual string ToString(bool time, bool sec)
        {
            string res;
            res = ConvertHelper.AdjustNumber(Year, 4) + "/" + ConvertHelper.AdjustNumber(Month, 2) + "/" + ConvertHelper.AdjustNumber(Day, 2);
            if (time) {
                res = res + " " + ConvertHelper.AdjustNumber(Hour, 2) + ":" + ConvertHelper.AdjustNumber(Minute, 2);

                if (sec) {
                    res = res + ":" + ConvertHelper.AdjustNumber(Second, 2);
                }
            }
            return res;
        }

        public virtual GameTime Clone()
        {
            GameTime varCopy = new GameTime(Year, Month, Day, Hour, Minute, Second);
            return varCopy;
        }

        public void Tick(int secs)
        {
            Second = (byte)(Second + secs);
            while (Second >= 60) {
                Second = (byte)(Second - 60);
                if (Minute == 59) {
                    Minute = 0;
                    if (Hour == 23) {
                        Hour = 0;
                        if (Day == MonthDays[DateHelper.IsLeapYear(Year) ? 1 : 0][Month - 1]) {
                            Day = 1;
                            if (Month == 12) {
                                Month = 1;
                                Year += 1;
                            } else {
                                Month += 1;
                            }
                        } else {
                            Day += 1;
                        }
                    } else {
                        Hour += 1;
                    }
                } else {
                    Minute += 1;
                }
            }
        }

        public Season Season
        {
            get {
                int month = Month;
                if (month >= 12 || month <= 2) {
                    return Season.Winter;
                }
                if (month <= 5) {
                    return Season.Spring;
                }
                if (month <= 8) {
                    return Season.Summer;
                }
                if (month <= 11) {
                    return Season.Autumn;
                }
                return Season.Winter;
            }
        }

        public DayTime DayTime
        {
            get {
                float d = (Hour + Minute / 100.0f);
                
                for (int i = DayTimeRec.dt_First; i <= DayTimeRec.dt_Last; i++) {
                    DayTimeRec dtx = DayTimes[i];
                    if (d >= dtx.bRange && d <= dtx.eRange) {
                        return (DayTime)i;
                    }
                }
                
                return DayTime.dt_Undefined;
            }
        }
    }
}
