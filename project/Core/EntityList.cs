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
    public class EntityList : BaseEntity
    {
        private readonly ExtList<GameEntity> fList;

        public EntityList(object owner, bool ownsObjects)
            : base(owner)
        {
            fList = new ExtList<GameEntity>(ownsObjects);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) {
                fList.Dispose();
            }
            base.Dispose(disposing);
        }

        public virtual int Count
        {
            get {
                return fList.Count;
            }
        }

        public virtual bool OwnsObjects
        {
            get {
                return fList.OwnsObjects;
            }
            set {
                fList.OwnsObjects = value;
            }
        }


        public virtual GameEntity GetItem(int index)
        {
            return fList[index];
        }

        public virtual int Add(GameEntity entity)
        {
            return fList.Add(entity);
        }

        public virtual void Assign(EntityList list)
        {
            fList.Clear();

            int num = list.Count;
            for (int i = 0; i < num; i++) {
                Add(list.GetItem(i));
            }
        }

        public virtual void Clear()
        {
            fList.Clear();
        }

        public virtual void Delete(int index)
        {
            fList.Delete(index);
        }

        public GameEntity FindByCLSID(int id)
        {
            int num = fList.Count;
            for (int i = 0; i < num; i++) {
                GameEntity e = GetItem(i);
                if (e.CLSID_Renamed == id) {
                    return e;
                }
            }

            return null;
        }

        public GameEntity FindByGUID(int id)
        {
            int num = fList.Count;
            for (int i = 0; i < num; i++) {
                GameEntity e = GetItem(i);
                if (e.UID_Renamed == id) {
                    return e;
                }
            }

            return null;
        }

        public virtual void Exchange(int index1, int index2)
        {
            fList.Exchange(index1, index2);
        }

        public virtual GameEntity Extract(GameEntity item)
        {
            return (GameEntity)fList.Extract(item);
        }

        public virtual int IndexOf(GameEntity entity)
        {
            return fList.IndexOf(entity);
        }

        public virtual int Remove(GameEntity entity)
        {
            return fList.Remove(entity);
        }

        public virtual void LoadFromStream(BinaryReader stream, FileVersion version)
        {
            try {
                fList.Clear();

                int count = StreamUtils.ReadInt(stream);
                for (int i = 0; i < count; i++) {
                    sbyte kind = (sbyte)StreamUtils.ReadByte(stream);
                    try {
                        GameEntity item = (GameEntity)SerializablesManager.CreateSerializable(kind, base.Owner);
                        item.LoadFromStream(stream, version);
                        fList.Add(item);
                    } catch (Exception ex) {
                        Logger.Write("EntityList.loadFromStream(" + Convert.ToString(kind) + "): " + ex.Message);
                        throw ex;
                    }
                }
            } catch (Exception ex) {
                Logger.Write("EntityList.loadFromStream(): " + ex.Message);
                throw ex;
            }
        }

        public virtual void SaveToStream(BinaryWriter stream, FileVersion version)
        {
            try {
                int count = fList.Count;

                int num = fList.Count;
                for (int i = 0; i < num; i++) {
                    if (GetItem(i).SerializeKind <= 0) {
                        count--;
                    }
                }

                StreamUtils.WriteInt(stream, count);

                for (int i = 0; i < num; i++) {
                    GameEntity item = GetItem(i);
                    byte kind = item.SerializeKind;
                    if (kind > 0) {
                        StreamUtils.WriteByte(stream, kind);
                        item.SaveToStream(stream, version);
                    }
                }
            } catch (Exception ex) {
                Logger.Write("EntityList.saveToStream(): " + ex.Message);
                throw ex;
            }
        }
    }
}
