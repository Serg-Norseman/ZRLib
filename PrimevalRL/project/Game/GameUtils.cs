/*
 *  "Inferior", roguelike game.
 *  Copyright (C) 2020 by Serg V. Zhdanovskih.
 *  This program is licensed under the GNU General Public License.
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using BSLib;
using ZRLib.Core;

namespace PrimevalRL.Game
{
    public static class GameUtils
    {
        private const float DEG2RAD = 3.14159F / 180;

        public static ExtPoint[] GetCirclePoints(ExtPoint center, int count, float radius)
        {
            ExtPoint[] result = new ExtPoint[count];
            if (count > 0) {
                float degSection = 360.0f / count;
                for (int i = 0; i < count; i++) {
                    float degInRad = (i * degSection) * DEG2RAD;
                    float dx = (float)Math.Cos(degInRad) * radius;
                    float dy = (float)Math.Sin(degInRad) * radius;
                    result[i] = new ExtPoint((int)Math.Round(center.X + dx), (int)Math.Round(center.Y + dy));
                }
            }
            return result;
        }

        public static ExtPoint MidPt(ExtPoint pt1, ExtPoint pt2)
        {
            int dx = (int)Math.Round((pt2.X + pt1.X) / 2.0f);
            int dy = (int)Math.Round((pt2.Y + pt1.Y) / 2.0f);
            return new ExtPoint(dx, dy);
        }

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
