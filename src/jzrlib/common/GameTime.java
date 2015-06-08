/*
 *  "JZRLib", Java Roguelike games development Library.
 *  Copyright (C) 2015 by Serg V. Zhdanovskih (aka Alchemist, aka Norseman).
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
package jzrlib.common;

import jzrlib.utils.AuxUtils;

/**
 *
 * @author Serg V. Zhdanovskih
 */
public class GameTime implements Cloneable
{
    private static final byte[][] MonthDays = new byte[][]{
        new byte[]{31, 28, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31}, 
        new byte[]{31, 29, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31}
    };

    public short Year;
    public byte Month;
    public byte Day;
    public byte Hour;
    public byte Minute;
    public byte Second;
    public byte dummy;

    public GameTime()
    {
        
    }

    public GameTime(int year, int month, int day, int hour, int minute, int second)
    {
        this.set(year, month, day, hour, minute, second);
    }

    public final void set(int year, int month, int day, int hour, int minute, int second)
    {
        this.Year = (short) year;
        this.Month = (byte) month;
        this.Day = (byte) day;
        this.Hour = (byte) hour;
        this.Minute = (byte) minute;
        this.Second = (byte) second;
        this.dummy = 0;
    }
    
    protected static boolean isLeapYear(short year)
    {
        return (year % 4 == 0) && (year % 100 != 0) || (year % 400 == 0);
    }

    public String toString(boolean time, boolean sec)
    {
        String res;
        res = AuxUtils.adjustNumber(this.Year, 4) + "/" + AuxUtils.adjustNumber(this.Month, 2) + "/" + AuxUtils.adjustNumber(this.Day, 2);
        if (time) {
            res = res + " " + AuxUtils.adjustNumber(this.Hour, 2) + ":" + AuxUtils.adjustNumber(this.Minute, 2);
            
            if (sec) {
                res = res + ":" + AuxUtils.adjustNumber(this.Second, 2);
            }
        }
        return res;
    }

    @Override
    public GameTime clone()
    {
        GameTime varCopy = new GameTime(this.Year, this.Month, this.Day, this.Hour, this.Minute, this.Second);
        return varCopy;
    }

    public final void tick(int secs)
    {
        this.Second = (byte) (this.Second + (byte) secs);
        while (this.Second >= 60) {
            this.Second = (byte) ((int) this.Second - 60);
            if (this.Minute == 59) {
                this.Minute = 0;
                if (this.Hour == 23) {
                    this.Hour = 0;
                    if (this.Day == MonthDays[isLeapYear(this.Year) ? 1 : 0][this.Month - 1]) {
                        this.Day = 1;
                        if (this.Month == 12) {
                            this.Month = 1;
                            this.Year += 1;
                        } else {
                            this.Month += 1;
                        }
                    } else {
                        this.Day += 1;
                    }
                } else {
                    this.Hour += 1;
                }
            } else {
                this.Minute += 1;
            }
        }
    }

    public final Season getSeason()
    {
        int month = this.Month;
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
    
    public final DayTime getDayTime()
    {
        float d = (this.Hour + this.Minute / 100.0f);

        for (int i = DayTime.dt_First; i <= DayTime.dt_Last; i++) {
            DayTime dtx = DayTime.forValue(i);
            if (d >= dtx.bRange && d <= dtx.eRange) {
                return dtx;
            }
        }

        return DayTime.dt_Undefined;
    }
}
