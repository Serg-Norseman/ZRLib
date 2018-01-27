/*
 *  "PrimevalRL", roguelike game.
 *  Copyright (C) 2015, 2017 by Serg V. Zhdanovskih.
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
using ZRLib.Map;
using ZRLib.External.BSP;

namespace PrimevalRL.Maps
{
    public static class MMapUtils
    {
        public static Axis GetTilesLineAxis(IMap map, int x, int y, int tileId)
        {
            if (map.CheckTile(x - 1, y, tileId, true) && map.CheckTile(x + 1, y, tileId, true)) {
                return Axis.axHorz;
            } else if (map.CheckTile(x, y - 1, tileId, true) && map.CheckTile(x, y + 1, tileId, true)) {
                return Axis.axVert;
            }

            return Axis.axNone;
        }

        public static Axis GetSideAxis(SideType side)
        {
            switch (side) {
                case SideType.stTop:
                    return Axis.axHorz;
                case SideType.stRight:
                    return Axis.axVert;
                case SideType.stBottom:
                    return Axis.axHorz;
                case SideType.stLeft:
                    return Axis.axVert;
                default:
                    return Axis.axNone;
            }
        }

        public static ExtPoint GetCheckPoint(SideType side, ExtRect area, int delta)
        {
            ExtPoint result;
            switch (side) {
                case SideType.stTop:
                    result = new ExtPoint(RandomHelper.GetBoundedRnd(area.Left + 1, area.Right - 1), area.Top - delta);
                    break;
                case SideType.stRight:
                    result = new ExtPoint(area.Right + delta, RandomHelper.GetBoundedRnd(area.Top + 1, area.Bottom - 1));
                    break;
                case SideType.stBottom:
                    result = new ExtPoint(RandomHelper.GetBoundedRnd(area.Left + 1, area.Right - 1), area.Bottom + delta);
                    break;
                case SideType.stLeft:
                    result = new ExtPoint(area.Left - delta, RandomHelper.GetBoundedRnd(area.Top + 1, area.Bottom - 1));
                    break;
                default:
                    result = ExtPoint.Empty;
                    break;
            }

            return result;
        }
    }
}
