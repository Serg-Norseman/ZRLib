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

namespace ZRLib.Engine
{
    public enum MouseButton
    {
        mbLeft,
        mbRight,
        mbMiddle
    }

    /// <summary>
    /// 
    /// </summary>
    public class MouseEventArgs : EventArgs
    {
        public MouseButton Button;
        public ShiftStates Shift;
        public int X;
        public int Y;

        public MouseEventArgs(int x, int y)
        {
            Shift = new ShiftStates();
            X = x;
            Y = y;
        }
    }
}
