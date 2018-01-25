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

namespace ZRLib.Map
{
    public abstract class CustomMap : AbstractMap
    {
        private BaseTile[] fTiles;

        protected CustomMap(int width, int height)
            : base(width, height)
        {
        }

        public override BaseTile GetTile(int x, int y)
        {
            BaseTile result = null;
            if (IsValid(x, y)) {
                result = fTiles[y * Width + x];
            }
            return result;
        }

        protected override void CreateTiles()
        {
            int num = Height * Width;
            fTiles = new BaseTile[num];
            for (int i = 0; i < num; i++) {
                fTiles[i] = CreateTile();
            }
        }

        protected override void DestroyTiles()
        {
            if (fTiles != null) {
                int num = Height * Width;
                for (int i = 0; i < num; i++) {
                    fTiles[i] = null;
                }
                fTiles = null;
            }
        }
    }
}
