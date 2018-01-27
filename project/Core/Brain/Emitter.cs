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

namespace ZRLib.Core.Brain
{
    public sealed class Emitter : GameEntity
    {
        public sbyte EmitterKind;
        public int ExpiryTime;
        public int ExpiryTimeLeft;
        public bool DynamicSourcePos;
        public ExtPoint Position;
        public float Radius;
        public int SourceID;

        public Emitter(GameSpace space, object owner)
            : base(space, owner)
        {
        }
    }
}
