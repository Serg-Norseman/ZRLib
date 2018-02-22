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

using ZRLib.Core;

namespace ZRLib.Engine
{
    public sealed class ShiftStates : FlagSet
    {
        public const int SsShift = 0;
        public const int SsAlt = 1;
        public const int SsCtrl = 2;
        public const int SsLeft = 3;
        public const int SsRight = 4;
        public const int SsMiddle = 5;
        public const int SsDouble = 6;

        public ShiftStates(params int[] args)
            : base(args)
        {
        }
    }

    public class KeyEventArgs : EventArgs
    {
        public Keys Key;
        public ShiftStates Shift;

        public KeyEventArgs()
        {
            Shift = new ShiftStates();
        }
    }
}
