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

using System;
using System.IO;
using BSLib;

namespace ZRLib.Core
{
    public abstract class LocatedEntity : GameEntity
    {
        private int fPosX;
        private int fPosY;

        protected LocatedEntity(GameSpace space, object owner)
            : base(space, owner)
        {
        }

        public int PosX
        {
            get {
                return fPosX;
            }
        }

        public int PosY
        {
            get {
                return fPosY;
            }
        }

        public ExtPoint Location
        {
            get {
                return new ExtPoint(fPosX, fPosY);
            }
        }

        public virtual void SetPos(int posX, int posY)
        {
            if (fPosX != posX || fPosY != posY) {
                fPosX = posX;
                fPosY = posY;
            }
        }

        public bool InRect(ExtRect rt)
        {
            return fPosX >= rt.Left && fPosX <= rt.Right && fPosY >= rt.Top && fPosY <= rt.Bottom;
        }

        public override void LoadFromStream(BinaryReader stream, FileVersion version)
        {
            try {
                base.LoadFromStream(stream, version);

                fPosX = StreamUtils.ReadInt(stream);
                fPosY = StreamUtils.ReadInt(stream);
            } catch (Exception ex) {
                Logger.Write("LocatedEntity.loadFromStream(): " + ex.Message);
                throw ex;
            }
        }

        public override void SaveToStream(BinaryWriter stream, FileVersion version)
        {
            base.SaveToStream(stream, version);

            StreamUtils.WriteInt(stream, fPosX);
            StreamUtils.WriteInt(stream, fPosY);
        }
    }
}
