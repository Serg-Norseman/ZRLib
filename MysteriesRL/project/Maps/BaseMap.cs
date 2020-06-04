/*
 *  "MysteriesRL", roguelike game.
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

using MysteriesRL.Game;
using ZRLib.Core;
using ZRLib.Map;

namespace MysteriesRL.Maps
{
    public class BaseMap : CustomMap
    {
        public BaseMap(int width, int height)
            : base(width, height)
        {
        }

        public override Movements GetTileMovements(ushort tileID)
        {
            Movements moves = new Movements();

            if (tileID != 0) {
                var tileRec = MRLData.Tiles[tileID];
                if (!tileRec.Flags.Contains(TileFlags.tfBarrier)) {
                    moves.Include(Movements.mkWalk);
                }
            }

            return moves;
        }

        public override ushort TranslateTile(TileType defTile)
        {
            switch (defTile) {
                case TileType.ttUndefined:
                    return (ushort)0;

                case TileType.ttGround:
                    return (ushort)'.';
                case TileType.ttWall:
                    return (ushort)'x';

                case TileType.ttCaveFloor:
                    return (ushort)TileID.tid_DungeonFloor;

                case TileType.ttRock:
                case TileType.ttDungeonWall:
                    return (ushort)TileID.tid_DungeonWall;

                case TileType.ttGrass:
                    return (ushort)TileID.tid_Grass;

                case TileType.ttMountain:
                    return (ushort)TileID.tid_Tree; // TODO

                default:
                    return (ushort)0;
            }
        }

        public override void SetMetaTile(int x, int y, TileType tile)
        {
            BaseTile baseTile = GetTile(x, y);
            if (baseTile != null) {
                baseTile.Background = TranslateTile(tile);
            }
        }

        public override void FillMetaBorder(int x1, int y1, int x2, int y2, TileType tile)
        {
            ushort defTile = TranslateTile(tile);
            FillBorder(x1, y1, x2, y2, defTile, false);
        }

        public virtual char GetMetaTile(int x, int y)
        {
            BaseTile baseTile = GetTile(x, y);
            if (baseTile != null) {
                return (char)baseTile.Background;
            } else {
                return ' ';
            }
        }
    }
}
