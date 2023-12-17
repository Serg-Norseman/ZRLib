/*
 *  "ZRLib", Roguelike games development Library.
 *  Copyright (C) 2015, 2020 by Serg V. Zhdanovskih.
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
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using ZRLib.Core;

namespace ZRLib.Storyteller
{
    /// <summary>
    /// 
    /// </summary>
    public static class AEUtils
    {
        public static string GetAppPath()
        {
            Assembly asm = Assembly.GetEntryAssembly();
            if (asm == null) {
                asm = Assembly.GetExecutingAssembly();
            }

            Module[] mods = asm.GetModules();
            string fn = mods[0].FullyQualifiedName;
            return Path.GetDirectoryName(fn) + Path.DirectorySeparatorChar;
        }

        public static List<string> ReadResText(string fileName, int codepage = 866)
        {
            var result = new List<string>();

            try {
                Assembly assembly = typeof(AEUtils).Assembly;
                using (Stream stm = assembly.GetManifestResourceStream("AlterEgo.Resources." + fileName)) {
                    using (StreamReader reader = new StreamReader(stm, Encoding.GetEncoding(codepage))) {
                        string line;
                        while ((line = reader.ReadLine()) != null) {
                            result.Add(line.TrimEnd());
                        }
                    }
                }
            } catch (Exception ex) {
                Logger.Write("GameUtils.ReadResText(): ", ex);
            }

            return result;
        }

        public static string ExtractString(string str, out string value, string defValue)
        {
            string result = str;

            if (!string.IsNullOrEmpty(result)) {
                int I = 0;
                while (I < result.Length && char.IsLetter(result[I])) {
                    I++;
                }

                if (I > 0) {
                    value = result.Substring(0, I);
                    result = result.Remove(0, I);
                } else {
                    value = defValue;
                }
            } else {
                value = defValue;
            }

            return result;
        }

        public static string ExtractNumber(string str, out int value, bool noException, int defValue)
        {
            string result = str;

            if (!string.IsNullOrEmpty(result)) {
                int I = 0;
                while (I < result.Length && char.IsDigit(result[I])) {
                    I++;
                }

                if (I > 0) {
                    value = int.Parse(result.Substring(0, I));
                    result = result.Remove(0, I);
                } else {
                    if (!noException) {
                        throw new Exception(string.Format("The string {0} doesn't start with a valid number", str));
                    }
                    value = defValue;
                }
            } else {
                value = defValue;
            }

            return result;
        }

        public static IEnumerable<string> SplitAndKeep(this string s, char[] delims)
        {
            int start = 0, index;

            while ((index = s.IndexOfAny(delims, start)) != -1) {
                if (index - start > 0)
                    yield return s.Substring(start, index - start);
                yield return s.Substring(index, 1);
                start = index + 1;
            }

            if (start < s.Length) {
                yield return s.Substring(start);
            }
        }

        public static double ParseFloat(string strVal, double defaultValue = 0.0d)
        {
            if (!string.IsNullOrEmpty(strVal)) {
                if (strVal[0] == '.') {
                    strVal = "0" + strVal;
                }
            }
            return ConvertHelper.ParseFloat(strVal, defaultValue, true);
        }

        public static List<string> WordWrap(string text, int maxLineLength)
        {
            var list = new List<string>();

            int currentIndex;
            var lastWrap = 0;
            var whitespace = new[] { ' ', '\r', '\n', '\t' };
            var splitChars = new[] { ' ', ',', '.', '?', '!', ':', ';', '-', '\n', '\r', '\t' };
            do {
                currentIndex = (lastWrap + maxLineLength > text.Length) ? text.Length : (text.LastIndexOfAny(splitChars, Math.Min(text.Length - 1, lastWrap + maxLineLength)) + 1);
                if (currentIndex <= lastWrap)
                    currentIndex = Math.Min(lastWrap + maxLineLength, text.Length);
                list.Add(text.Substring(lastWrap, currentIndex - lastWrap).Trim(whitespace));
                lastWrap = currentIndex;
            } while (currentIndex < text.Length);

            return list;
        }
    }
}
