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

using BSLib;

namespace ZRLib.Core.Brain
{
    public sealed class EmitterList : ExtList<Emitter>
    {
        public EmitterList() : base()
        {
        }

        public int AddEmitter(sbyte emitterKind, int sourceID, ExtPoint pos, float radius, int duration, bool dynSource)
        {
            Emitter emitter = new Emitter(GameSpace.Instance, this);

            emitter.EmitterKind = emitterKind;
            emitter.SourceID = (int)sourceID;
            emitter.Position = pos;
            emitter.Radius = radius;
            emitter.ExpiryTime = duration;
            emitter.ExpiryTimeLeft = duration;
            emitter.DynamicSourcePos = dynSource;

            Add(emitter);
            return (int)emitter.UID;
        }

        public void UpdateEmitters(int elapsedTime)
        {
            for (int i = Count - 1; i >= 0; i--) {
                Emitter emitter = this[i];

                if ((double)emitter.ExpiryTime > (double)0f) {
                    emitter.ExpiryTimeLeft -= elapsedTime;
                }

                if ((double)emitter.ExpiryTime > (double)0f && emitter.ExpiryTimeLeft <= 0) {
                    Delete(i);
                } else {
                    if (emitter.DynamicSourcePos) {
                        LocatedEntity source = (LocatedEntity)GameSpace.Instance.FindEntity(emitter.SourceID);

                        if (source == null) {
                            emitter.DynamicSourcePos = false;
                        } else {
                            emitter.Position = source.Location;
                        }
                    }
                }
            }
        }
    }
}
