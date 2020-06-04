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

namespace ZRLib.Core.Body
{
    public sealed class Bodypart
    {
        public const int STATE_NORMAL = 0;
        public const int STATE_DAMAGED = 1;
        public const int STATE_DESTROYED = 2;

        public int Type;
        public int State;
        public object Item;


        public Bodypart(int type)
        {
            Type = type;
            State = STATE_NORMAL;
            Item = null;
        }

        public Bodypart(int type, int state)
        {
            Type = type;
            State = state;
            Item = null;
        }
    }
}
