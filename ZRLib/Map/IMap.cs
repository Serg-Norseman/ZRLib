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
using ZRLib.Core;

namespace ZRLib.Map
{
    public delegate void ITileChangeHandler(IMap map, int x, int y,
                                            object extData, ref bool refContinue);

    public interface IMap
    {
        ExtRect AreaRect { get; }
        int Height { get; }
        int Width { get; }

        EntityList<GameEntity> Features { get; }
        BaseTile GetTile(int x, int y);

        bool IsValid(int x, int y);
        bool IsBlockLOS(int x, int y);
        bool IsBarrier(int x, int y);

        void Normalize();
        void Resize(int width, int height);
        void SetSeen(int x, int y);

        void SetTile(int x, int y, ushort tid, bool fg);
        bool CheckTile(int x, int y, int checkId, bool fg);

        void FillArea(ExtRect area, ushort tid, bool fg);
        void FillArea(int x1, int y1, int x2, int y2, ushort tid, bool fg);
        void FillBorder(int x1, int y1, int x2, int y2, ushort tid, bool fg);
        void FillRadial(int aX, int aY, ushort boundTile, ushort fillTile, bool fg);

        void FillBackground(ushort tid);
        void FillForeground(ushort tid);

        CreatureEntity FindCreature(int aX, int aY);
        LocatedEntity FindItem(int aX, int aY);

        ExtPoint SearchFreeLocation();
        ExtPoint SearchFreeLocation(ExtRect area, int tries = 50);

        int CheckForeAdjacently(int x, int y, TileType defTile);

        void FillMetaBorder(int x1, int y1, int x2, int y2, TileType tile);
        void SetMetaTile(int x, int y, TileType tile);
        ushort TranslateTile(TileType defTile);

        void Gen_Path(int px1, int py1, int px2, int py2, ExtRect area, ushort tid, bool wide, bool bg, ITileChangeHandler tileHandler);
        void Gen_Path(int px1, int py1, int px2, int py2, ExtRect area, ushort tid, bool wide, int wide2, bool bg, ITileChangeHandler tileHandler);

        /// <summary>
        /// Get the cost of moving through the given tile. This can be used to make
        /// certain areas more desirable. A simple and valid implementation of this
        /// method would be to return 1 in all cases.
        /// </summary>
        /// <param name="creature"> </param>
        /// <param name="tx"> The tile X coordinate </param>
        /// <param name="ty"> The tile Y coordinate </param>
        /// <param name="tile"> </param>
        /// <returns> The relative cost of moving across the given tile </returns>
        float GetPathTileCost(CreatureEntity creature, int tx, int ty, BaseTile tile);
    }
}
