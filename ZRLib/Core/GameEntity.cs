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

using System.IO;

namespace ZRLib.Core
{
    public abstract class GameEntity : BaseEntity, ISerializable
    {
        private static int LastUID;

        protected readonly GameSpace fSpace;
        private int fCLSID;
        private int fUID;


        public virtual GameSpace Space
        {
            get {
                return fSpace;
            }
        }

        public int UID
        {
            get {
                return fUID;
            }
            set {
                if (fUID != value) {
                    GameSpace space = GameSpace.Instance;
    
                    if (space != null && fUID != -1) {
                        space.DeleteEntity(this);
                    }
    
                    fUID = value;
    
                    if (space != null && value != -1) {
                        space.AddEntity(this);
                    }
                }
            }
        }

        public virtual int CLSID
        {
            get {
                return fCLSID;
            }
            set {
                fCLSID = value;
            }
        }

        public virtual string Desc
        {
            get {
                return "";
            }
            set {
            }
        }

        public virtual string Name
        {
            get {
                return "";
            }
            set {
            }
        }

        public virtual byte SerializeKind
        {
            get {
                return 0;
            }
        }


        protected GameEntity(GameSpace space, object owner)
            : base(owner)
        {
            fSpace = space;
            fUID = -1;
            NewUID();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) {
                if (GameSpace.Instance != null) {
                    GameSpace.Instance.DeleteEntity(this);
                }
            }
            base.Dispose(disposing);
        }

        public static void ResetUID(int newID)
        {
            LastUID = newID;
        }

        public static int NextUID()
        {
            int result = LastUID;
            LastUID++;
            return result;
        }

        public void NewUID()
        {
            UID = NextUID();
        }

        public virtual bool Assign(GameEntity entity)
        {
            return true;
        }

        public virtual void LoadFromStream(BinaryReader stream, FileVersion version)
        {
            fCLSID = StreamUtils.ReadInt(stream);
        }

        public virtual void SaveToStream(BinaryWriter stream, FileVersion version)
        {
            StreamUtils.WriteInt(stream, fCLSID);
        }
    }
}
