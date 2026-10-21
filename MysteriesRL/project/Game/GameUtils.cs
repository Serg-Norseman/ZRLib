/*
 *  "MysteriesRL", roguelike game.
 *  Copyright (C) 2015, 2017, 2020 by Serg V. Zhdanovskih.
 *  
 *  Licensed under the GNU General Public License (GPL) v3.
 *  See LICENSE file in the project root for full license information.
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using ZRLib.Core;

namespace MysteriesRL.Game
{
    public static class GameUtils
    {
        public static List<string> ReadResText(string fileName, int codepage = 866)
        {
            var result = new List<string>();

            try {
                Assembly assembly = typeof(GameUtils).Assembly;
                using (Stream stm = assembly.GetManifestResourceStream("resources." + fileName)) {
                    using (StreamReader reader = new StreamReader(stm, Encoding.GetEncoding(codepage))) {
                        while (reader.Peek() != -1) {
                            result.Add(reader.ReadLine().TrimEnd());
                        }
                    }
                }
            } catch (Exception ex) {
                Logger.Write("GameUtils.ReadResText(): " + ex.Message);
            }

            return result;
        }
    }
}
