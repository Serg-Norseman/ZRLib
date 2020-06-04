/*
 *  "ZRLib", Roguelike games development Library.
 *  Copyright (C) 2015,2020 by Serg V. Zhdanovskih.
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
using BSLib;

namespace ZRLib.Map
{
    /// <summary>
    /// 
    /// </summary>
    public static class LightMgr
    {
        private static float[,] fLightMap;

        public static float[,] CalculateLight(IMap map, ExtRect viewport)
        {
            if (fLightMap == null) {
                fLightMap = new float[viewport.Height, viewport.Width];
            }

            var features = map.Features;
            var lights = new List<ILightSource>();
            for (int i = 0; i < features.Count; i++) {
                var feat = features[i] as ILightSource;
                if (feat != null && feat.InRect(viewport) && (feat.Power > 0.0f)) {
                    lights.Add(feat);
                }
            }

            for (int ay = viewport.Top; ay <= viewport.Bottom; ay++) {
                for (int ax = viewport.Left; ax <= viewport.Right; ax++) {
                    BaseTile tile = map.GetTile(ax, ay);
                    if (tile != null) {
                        float lightLevel = 0.0f;

                        for (int lid = 0; lid < lights.Count; lid++) {
                            var light = lights[lid];
                            float dist = MathHelper.Distance(light.PosX, light.PosY, ax, ay);
                            float lAmt = (1.0f - (dist / light.Power)).Clamp(0.0f, 1.0f);
                            lightLevel += lAmt;
                        }

                        fLightMap[ay - viewport.Top, ax - viewport.Left] = lightLevel;
                    }
                }
            }

            return fLightMap;
        }
    }
}
