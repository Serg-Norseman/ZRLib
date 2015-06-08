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
package jzrlib.utils;

/**
 *
 * @author Serg V. Zhdanovskih
 */
public final class TypeUtils
{
    public static short getUByte(byte val)
    {
        short result = (short) (val & 0xff);
        return result;
    }

    public static short fitShort(byte lo, byte hi)
    {
        short result = (short) (((hi & 0xFF) << 8) | (lo & 0xFF));
        return result;
    }

    public static int fitShort(int lo, int hi)
    {
        int result = ((hi & 0xFF) << 8) | (lo & 0xFF);
        return result;
    }

    public static int getShortLo(short val)
    {
        return val & 0xff;
    }

    public static int getShortHi(short val)
    {
        return (val >> 8) & 0xff;
    }

    public static int setBit(int changeValue, int position, int value)
    {
        int result = changeValue;

        int bt = (1 << position);
        if (value == 1) {
            result |= bt;
        } else {
            result &= (bt ^ 255);
        }

        return result;
    }

    public static boolean hasBit(int testValue, int position)
    {
        int bt = (1 << position);
        return (bt & testValue) > 0;
    }

    public static boolean hasFlag(int testValue, int flag)
    {
        return (testValue & flag) > 0;
    }

}
