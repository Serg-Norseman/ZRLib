/*
 *  "ZRLib", Roguelike games development Library.
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

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using BSLib;

namespace ZRLib.Core
{
    public static class Extensions
    {
        public static void AddRange<T>(this IList<T> list, IList<T> otherRange)
        {
            foreach (T item in otherRange)
                list.Add(item);
        }

        public static int IndexOf<T>(this T[] array, T item)
        {
            for (int i = 0; i < array.Length; i++) {
                if ((array[i] != null && array[i].Equals(item)) || (item == null && array[i] == null)) {
                    return i;
                }
            }
            return -1;
        }

        public static IEnumerable<T> GetValues<T>()
        {
            return Enum.GetValues(typeof(T)).Cast<T>();
        }

        public static T ParseEnum<T>(string name)
        {
            return (T)Enum.Parse(typeof(T), name);
        }

        public static TValue GetValueOrNull<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key)
        {
            TValue ret;
            dictionary.TryGetValue(key, out ret);
            return ret;
        }

        public static void Rotate<T>(this IList<T> list, int distance)
        {
            int size = list.Count();
            if (size == 0)
                return;
            distance %= size;
            if (distance == 0)
                return;
            if (distance < 0)
                distance += size;

            // Determine the least common multiple of distance and size, as there
            // are (distance / LCM) loops to cycle through.
            int a = size;
            int lcm = distance;
            int b = a % lcm;
            while (b != 0)
            {
                a = lcm;
                lcm = b;
                b = a % lcm;
            }

            // Now, make the swaps. We must take the remainder every time through
            // the inner loop so that we don't overflow i to negative values.
            while (--lcm >= 0)
            {
                T o = list[lcm];
                for (int i = lcm + distance; i != lcm; i = (i + distance) % size) {
                    T prev = list[i];
                    list[i] = o;
                    o = prev;
                }
                list[lcm] = o;
            }
        }
    }

    public delegate void ILineProc(int aX, int aY, ref bool refContinue);

    public static class AuxUtils
    {
        public const int MaxInt = 2147483647;
        public const char CR = '\r';
        public const char LF = '\n';
        public const string CRLF = "\r\n";

        public static int Middle(int a, int b, int c)
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

        public static void ExStub(string val)
        {
        }

        public static bool DoLine(int x1, int y1, int x2, int y2, ILineProc lineProc, bool excludeFirst)
        {
            bool result = true;

            int dX = Math.Abs(x1 - x2);
            int dY = Math.Abs(y1 - y2);
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

            bool refContinue = result;
            if (!excludeFirst) {
                lineProc(X, Y, ref refContinue);
            }

            if (refContinue) {
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

                        lineProc(X, Y, ref refContinue);
                        if (!refContinue) {
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

                        lineProc(X, Y, ref refContinue);
                        if (!refContinue) {
                            break;
                        }
                    }
                }
            }

            return refContinue;
        }

        public static bool Chance(int percent)
        {
            return RandomHelper.GetRandom(101) < percent;
        }

        public static string ChangeExtension(string originalName, string newExtension)
        {
            int lastDot = originalName.LastIndexOf('.');
            if (lastDot != -1) {
                return originalName.Substring(0, lastDot) + newExtension;
            } else {
                return originalName + newExtension;
            }
        }

        public static int CalcDistanceToArea(ExtPoint p, ExtRect a)
        {
            int result;
            if (a.Contains(p.X, p.Y)) {
                result = 0;
            } else {
                int dist = AuxUtils.MaxInt;

                for (int x = a.Left; x <= a.Right; x++) {
                    int td = MathHelper.Distance(p.X, p.Y, x, a.Top);
                    if (td < dist) {
                        dist = td;
                    }
                    td = MathHelper.Distance(p.X, p.Y, x, a.Bottom);
                    if (td < dist) {
                        dist = td;
                    }
                }

                for (int y = a.Top; y <= a.Bottom; y++) {
                    int td = MathHelper.Distance(p.X, p.Y, a.Left, y);
                    if (td < dist) {
                        dist = td;
                    }
                    td = MathHelper.Distance(p.X, p.Y, a.Right, y);
                    if (td < dist) {
                        dist = td;
                    }
                }
                result = dist;
            }
            return result;
        }

        public static string GetToken(string str, string sepChars, int tokenNum)
        {
            if (string.IsNullOrEmpty(str) || string.IsNullOrEmpty(sepChars)) {
                return "";
            }

            if (sepChars.IndexOf(str[str.Length - 1]) < 0) {
                str += sepChars[0];
            }

            int tok_num = 0;
            string tok = "";

            for (int i = 0; i < str.Length; i++) {
                if (sepChars.IndexOf(str[i]) >= 0) {
                    tok_num++;
                    if (tok_num == tokenNum) {
                        return tok;
                    }
                    tok = "";
                } else {
                    tok += (str[i]);
                }

            }
            return "";
        }

        public static int GetTokensCount(string str, string sepChars)
        {
            if (string.IsNullOrEmpty(str) || string.IsNullOrEmpty(sepChars)) {
                return 0;
            }

            int result = 0;

            if (sepChars.IndexOf(str[str.Length - 1]) < 0) {
                str += sepChars[0];
            }
            int num = str.Length;
            for (int i = 0; i < num; i++) {
                if (sepChars.IndexOf(str[i]) >= 0) {
                    result += 1;
                }
            }

            return result;
        }

        /// <summary>
        /// Returns the result of a dice roll. The parameters <code>number</code> and
        /// <code>dice</code> should be positive, <code>mod</code> can be any
        /// integer. If <code>number</code> or <code>dice</code> are not positive,
        /// this method returns <code>mod</code>.
        /// </summary>
        /// <param name="number">    the amount of dice to roll </param>
        /// <param name="dice">    the type of dice to roll </param>
        /// <param name="mod">    the modifier applied to the result of the roll
        /// @return    the result of (number)d(dice)+(mod) </param>
        public static int Roll(int number, int dice, int mod)
        {
            if (number < 1 || dice < 1) {
                return mod;
            }

            int result = 0;
            for (int i = 0; i < number; i++) {
                result += ((int)(dice * new Random(1).NextDouble() + 1));
            }
            return result + mod;
        }

        /// <summary>
        /// Returns the result of a dice roll. The input string has the form 'xdy',
        /// 'xdy+z' or 'xdy-z', with x, y and z positive integers.
        /// </summary>
        /// <param name="roll">    the string representation of the roll
        /// @return    the result of the roll </param>
        /// <exception cref="NumberFormatException">    if the string contains an unparsable part </exception>
        public static int Roll(string roll)
        {
            int index1 = roll.IndexOf('d');
            int index2 = roll.IndexOf('+');
            int index3 = roll.IndexOf('-');
            int number = Convert.ToInt32(roll.Substring(0, index1));
            int mod = 0;

            int dice;
            if (index2 > 0) {
                dice = Convert.ToInt32(roll.Substring(index1 + 1, index2 - (index1 + 1)));
                mod = Convert.ToInt32(roll.Substring(index2 + 1, roll.Length - (index2 + 1)));
            } else if (index3 > 0) {
                dice = Convert.ToInt32(roll.Substring(index1 + 1, index3 - (index1 + 1)));
                mod = -Convert.ToInt32(roll.Substring(index3 + 1, roll.Length - (index3 + 1)));
            } else {
                dice = Convert.ToInt32(roll.Substring(index1 + 1, roll.Length - (index1 + 1)));
            }

            return Roll(number, dice, mod);
        }

        public static ExtPointF IntersectLine2Line(ExtPointF p1, ExtPointF p2, ExtPointF p3, ExtPointF p4)
        {
            float denom = ((p4.Y - p3.Y) * (p2.X - p1.X) - (p4.X - p3.X) * (p2.Y - p1.Y));
            if (denom == 0) {
                return ExtPointF.Empty; // lines are parallel
            }
            float ua = ((p4.X - p3.X) * (p1.Y - p3.Y) - (p4.Y - p3.Y) * (p1.X - p3.X)) / denom;
            float ub = ((p2.X - p1.X) * (p1.Y - p3.Y) - (p2.Y - p1.Y) * (p1.X - p3.X)) / denom;

            if (ua < 0 || ua > 1 || ub < 0 || ub > 1) {
                return ExtPointF.Empty;
            }

            return new ExtPointF(p1.X + ua * (p2.X - p1.X), p1.Y + ua * (p2.Y - p1.Y));
        }

        public static ExtPointF IntersectLine2Rect(ExtPointF p1, ExtPointF p2, ExtRect rt)
        {
            float bx1 = rt.Left;
            float by1 = rt.Top;
            float bx2 = rt.Right;
            float by2 = rt.Bottom;

            ExtPointF tl = new ExtPointF(bx1, by1);
            ExtPointF tr = new ExtPointF(bx2, by1);
            ExtPointF bl = new ExtPointF(bx1, by2);
            ExtPointF br = new ExtPointF(bx2, by2);

            ExtPointF pt;

            pt = IntersectLine2Line(p1, p2, tl, tr);
            if (!pt.IsEmpty) {
                return pt;
            }

            pt = IntersectLine2Line(p1, p2, tr, br);
            if (!pt.IsEmpty) {
                return pt;
            }

            pt = IntersectLine2Line(p1, p2, br, bl);
            if (!pt.IsEmpty) {
                return pt;
            }

            pt = IntersectLine2Line(p1, p2, bl, tl);
            if (!pt.IsEmpty) {
                return pt;
            }

            return ExtPointF.Empty;
        }

        public static short GetUByte(sbyte val)
        {
            short result = (short)(val & 0xff);
            return result;
        }

        public static ushort FitShort(byte lo, byte hi)
        {
            ushort result = (ushort)(((hi & 0xFF) << 8) | (lo & 0xFF));
            return result;
        }

        public static ushort FitShort(int lo, int hi)
        {
            ushort result = (ushort)(((hi & 0xFF) << 8) | (lo & 0xFF));
            return result;
        }

        public static byte GetShortLo(ushort val)
        {
            return (byte)(val & 0xff);
        }

        public static byte GetShortHi(ushort val)
        {
            return (byte)((val >> 8) & 0xff);
        }

        public static bool HasFlag(int testValue, int flag)
        {
            return (testValue & flag) > 0;
        }


        /// <summary>
        /// Make an even blend between two colors.
        /// </summary>
        /// <param name="c1"> First color to blend. </param>
        /// <param name="c2"> Second color to blend. </param>
        /// <returns> Blended color. </returns>
        public static Color Blend(Color color1, Color color2, float ratio = 0.5f)
        {
            return Color.FromArgb(GfxHelper.Blend(color1.ToArgb(), color2.ToArgb(), ratio));
        }

        /// <summary>
        /// Make a color darker.
        /// </summary>
        /// <param name="color"> Color to make darker. </param>
        /// <param name="fraction"> Darkness fraction. </param>
        /// <returns> Darker color. </returns>
        public static Color Darker(Color color, float fraction)
        {
            int rgb = color.ToArgb();
            return Color.FromArgb(GfxHelper.Darker(rgb, fraction));
        }

        /// <summary>
        /// Make a color lighter.
        /// </summary>
        /// <param name="color"> Color to make lighter. </param>
        /// <param name="fraction"> Darkness fraction. </param>
        /// <returns> Lighter color. </returns>
        public static Color Lighter(Color color, float fraction)
        {
            int rgb = color.ToArgb();
            return Color.FromArgb(GfxHelper.Lighter(rgb, fraction));
        }
    }
}
