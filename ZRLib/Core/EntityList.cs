/*
 *  "ZRLib", Roguelike games development Library.
 *  Copyright (C) 2015, 2020 by Serg V. Zhdanovskih.
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
    public class EntityList<T> : BaseEntity
        where T : GameEntity
    {
        private readonly ExtList<T> fList;


        public int Count
        {
            get {
                return fList.Count;
            }
        }

        public T this[int index]
        {
            get {
                return fList[index];
            }
        }


        public EntityList(object owner)
            : base(owner)
        {
            fList = new ExtList<T>(true);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) {
                fList.Dispose();
            }
            base.Dispose(disposing);
        }

        public virtual int Add(T entity)
        {
            return (entity == null) ? -1 : fList.Add(entity);
        }

        public virtual void Assign(EntityList<T> list)
        {
            fList.Clear();

            int num = list.Count;
            for (int i = 0; i < num; i++) {
                Add(list[i]);
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

        public T FindByCLSID(int id)
        {
            int num = fList.Count;
            for (int i = 0; i < num; i++) {
                T e = fList[i];
                if (e.CLSID == id) {
                    return e;
                }
            }

            return null;
        }

        public T FindByGUID(int id)
        {
            int num = fList.Count;
            for (int i = 0; i < num; i++) {
                T e = fList[i];
                if (e.UID == id) {
                    return e;
                }
            }

            return null;
        }

        public virtual void Exchange(int index1, int index2)
        {
            fList.Exchange(index1, index2);
        }

        public virtual T Extract(T item)
        {
            return (T)fList.Extract(item);
        }

        public virtual int IndexOf(T entity)
        {
            return fList.IndexOf(entity);
        }

        public virtual int Remove(T entity)
        {
            return fList.Remove(entity);
        }

        public virtual void LoadFromStream(BinaryReader stream, FileVersion version)
        {
            try {
                fList.Clear();

                int count = StreamUtils.ReadInt(stream);
                for (int i = 0; i < count; i++) {
                    byte kind = StreamUtils.ReadByte(stream);
                    try {
                        T item = (T)SerializablesManager.CreateSerializable(kind, base.Owner);
                        item.LoadFromStream(stream, version);
                        fList.Add(item);
                    } catch (Exception ex) {
                        Logger.Write("EntityList.LoadFromStream(" + Convert.ToString(kind) + "): " + ex.StackTrace.ToString());
                        throw ex;
                    }
                }
            } catch (Exception ex) {
                Logger.Write("EntityList.LoadFromStream(): " + ex.StackTrace.ToString());
                throw ex;
            }
        }

        public virtual void SaveToStream(BinaryWriter stream, FileVersion version)
        {
            try {
                int count = fList.Count;

                int num = fList.Count;
                for (int i = 0; i < num; i++) {
                    if (fList[i].SerializeKind <= 0) {
                        count--;
                    }
                }

                StreamUtils.WriteInt(stream, count);

                for (int i = 0; i < num; i++) {
                    var item = fList[i];
                    byte kind = item.SerializeKind;
                    if (kind > 0) {
                        StreamUtils.WriteByte(stream, kind);
                        item.SaveToStream(stream, version);
                    }
                }
            } catch (Exception ex) {
                Logger.Write("EntityList.SaveToStream(): " + ex.StackTrace.ToString());
                throw ex;
            }
        }
    }
}
