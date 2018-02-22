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

namespace ZRLib.Core
{
    public abstract class AreaEntity : GameEntity
    {
        private ExtRect fArea;

        protected AreaEntity(GameSpace space, object owner)
            : base(space, owner)
        {
            fArea = ExtRect.Empty;
        }

        public virtual ExtRect Area
        {
            get { return fArea; }
            set { fArea = value; }
        }

        public void SetArea(int left, int top, int right, int bottom)
        {
            fArea = ExtRect.Create(left, top, right, bottom);
        }

        public bool InArea(int x, int y)
        {
            return x >= fArea.Left && x <= fArea.Right && y >= fArea.Top && y <= fArea.Bottom;
        }
    }
}
