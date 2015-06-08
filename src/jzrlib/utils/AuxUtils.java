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

import java.util.List;
import java.util.Random;
import jzrlib.core.ILineProc;
import jzrlib.core.Point;
import jzrlib.core.Rect;

/**
 *
 * @author Serg V. Zhdanovskih
 */
public final class AuxUtils
{
    private static final String Nums = "0123456789+-";

    public static final boolean binEndian = false;
    public static final int MaxInt = 2147483647;
    public static final char CR = '\r';
    public static final char LF = '\n';
    public static final String CRLF = "\r\n";

    public static int RandSeed;
    private static int LastRandSeed;
    private static Random RandomEngine;

    static {
        AuxUtils.randomize();
    }

    public static void randomize()
    {
        AuxUtils.RandSeed = 0;
        AuxUtils.LastRandSeed = -1;

        if (AuxUtils.LastRandSeed != AuxUtils.RandSeed) {
            if (AuxUtils.RandSeed == 0) {
                AuxUtils.RandomEngine = new Random();
            } else {
                AuxUtils.RandomEngine = new Random(AuxUtils.RandSeed);
            }
            AuxUtils.LastRandSeed = AuxUtils.RandSeed;
        }
    }

    public static int getRandom(int range)
    {
        return AuxUtils.RandomEngine.nextInt(range);
    }

    public static int getBoundedRnd(int low, int high)
    {
        if (low > high) {
            int temp = low;
            low = high;
            high = temp;            
        }

        return low + AuxUtils.getRandom(high - low + 1);
    }

    public static Point getRandomPoint(Rect area)
    {
        int x = AuxUtils.getBoundedRnd(area.Left, area.Right);
        int y = AuxUtils.getBoundedRnd(area.Top, area.Bottom);
        return new Point(x, y);
    }

    public static int getRandomArrayInt(int... array)
    {
        int idx = AuxUtils.getRandom(array.length);
        return array[idx];
    }

    public static <T> T getRandomItem(List<T> list)
    {
        int size = list.size();

        if (size < 1) {
            return null;
        } else if (size == 1) {
            return list.get(0);
        } else {
            return list.get(AuxUtils.getRandom(size));
        }
    }

    public static <T extends Enum<T>> T getRandomEnum(Class<T> enumType)
    {
        T[] vals = enumType.getEnumConstants();
        int idx = AuxUtils.getRandom(vals.length);
        return vals[idx];
    }

    public static int middle(int a, int b, int c)
    {
        int result;
        if (a > b) {
            if (b > c) {
                result = b;
            } else {
                if (a > c) {
                    result = c;
                } else {
                    result = a;
                }
            }
        } else {
            if (a > c) {
                result = a;
            } else {
                if (b > c) {
                    result = c;
                } else {
                    result = b;
                }
            }
        }
        return result;
    }

    public static void exStub(String val)
    {
    }

    public static boolean isValidInt(String value)
    {
        if (value == null || value.length() == 0) return false;
        
        for (int i = 0; i < value.length(); i++) {
            if (AuxUtils.Nums.indexOf(value.charAt(i)) < 0) {
                return false;
            }
        }

        return true;
    }
    
    public static boolean isValueBetween(int value, int lowLimit, int topLimit, boolean includeLimits)
    {
        if (lowLimit > topLimit) {
            int temp = lowLimit;
            lowLimit = topLimit;
            topLimit = temp;
        }

        if (!includeLimits) {
            lowLimit++;
            topLimit--;
        }

        return value >= lowLimit && value <= topLimit;
    }

    public static boolean doLine(int x1, int y1, int x2, int y2, ILineProc lineProc, boolean excludeFirst)
    {
        boolean result = true;

        int dX = Math.abs(x1 - x2);
        int dY = Math.abs(y1 - y2);
        int dX2 = dX << 1;
        int dY2 = dY << 1;

        int XInc;
        if (x1 < x2) {
            XInc = 1;
        } else {
            XInc = -1;
        }

        int YInc;
        if (y1 < y2) {
            YInc = 1;
        } else {
            YInc = -1;
        }

        int X = x1;
        int Y = y1;

        RefObject<Boolean> refContinue = new RefObject<>(result);
        if (!excludeFirst) {
            lineProc.invoke(X, Y, refContinue);
        }

        if (refContinue.argValue) {
            if (dX > dY) {
                int S = dY2 - dX;
                int dXY = dY2 - dX2;

                for (int i = 1; i <= dX; i++) {
                    if (S >= 0) {
                        Y += YInc;
                        S += dXY;
                    } else {
                        S += dY2;
                    }
                    X += XInc;

                    lineProc.invoke(X, Y, refContinue);
                    if (!refContinue.argValue) {
                        break;
                    }
                }
            } else {
                int S = dX2 - dY;
                int dXY = dX2 - dY2;

                for (int i = 1; i <= dY; i++) {
                    if (S >= 0) {
                        X += XInc;
                        S += dXY;
                    } else {
                        S += dX2;
                    }
                    Y += YInc;

                    lineProc.invoke(X, Y, refContinue);
                    if (!refContinue.argValue) {
                        break;
                    }
                }
            }
        }

        return refContinue.argValue;
    }

    public static int distance(int x1, int y1, int x2, int y2)
    {
        int dX = x2 - x1;
        int dY = y2 - y1;
        return (int) Math.round(Math.sqrt((double) (dX * dX + dY * dY)));
    }

    public static int distance(Point pt1, Point pt2)
    {
        return AuxUtils.distance(pt1.X, pt1.Y, pt2.X, pt2.Y);
    }

    public static String adjustNumber(int val, int up)
    {
        String result = Integer.toString(val);
        if (result.length() < up) {
            StringBuilder sb = new StringBuilder(result);
            while (sb.length() < up) {
                sb.insert(0, '0');
            }
            result = sb.toString();
        }
        return result;
    }

    public static String adjustString(String val, int up)
    {
        String result = val;
        if (result.length() < up) {
            StringBuilder sb = new StringBuilder(result);
            while (sb.length() < up) {
                sb.append(' ');
            }
            result = sb.toString();
        }
        return result;
    }

    public static String adjustNum(int val, int up)
    {
        String result = Integer.toString(val);
        if (result.length() < up) {
            StringBuilder sb = new StringBuilder(result);
            while (sb.length() < up) {
                sb.insert(0, ' ');
            }
            result = sb.toString();
        }
        return result;
    }

    public static boolean chance(int percent)
    {
        return AuxUtils.getRandom(101) < percent;
    }

    public static String changeExtension(String originalName, String newExtension)
    {
        int lastDot = originalName.lastIndexOf('.');
        if (lastDot != -1) {
            return originalName.substring(0, lastDot) + newExtension;
        } else {
            return originalName + newExtension;
        }
    }

    public static <T> int indexOf(T item, T[] array)
    {
        for (int i = 0; i < array.length; i++) {
            if ((array[i] != null && array[i].equals(item)) || (item == null && array[i] == null)) {
                return i;
            }
        }

        return -1;
    }

    public static int indexOfInt(int item, int[] array)
    {
        for (int i = 0; i < array.length; i++) {
            if (array[i] == item) {
                return i;
            }
        }

        return -1;
    }

    public static int calcDistanceToArea(Point p, Rect a)
    {
        int result;
        if (a.contains(p.X, p.Y)) {
            result = 0;
        } else {
            int dist = AuxUtils.MaxInt;

            for (int x = a.Left; x <= a.Right; x++) {
                int td = AuxUtils.distance(p.X, p.Y, x, a.Top);
                if (td < dist) {
                    dist = td;
                }
                td = AuxUtils.distance(p.X, p.Y, x, a.Bottom);
                if (td < dist) {
                    dist = td;
                }
            }

            for (int y = a.Top; y <= a.Bottom; y++) {
                int td = AuxUtils.distance(p.X, p.Y, a.Left, y);
                if (td < dist) {
                    dist = td;
                }
                td = AuxUtils.distance(p.X, p.Y, a.Right, y);
                if (td < dist) {
                    dist = td;
                }
            }
            result = dist;
        }
        return result;
    }
    
    public static final int getSystemModel()
    {
        String sys = System.getProperty("sun.arch.data.model");
        return Integer.parseInt(sys);
    }
}
