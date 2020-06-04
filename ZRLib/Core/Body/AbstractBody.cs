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

using System.Collections.Generic;

namespace ZRLib.Core.Body
{
    public abstract class AbstractBody : BaseEntity
    {
        protected List<Bodypart> fParts;


        public IList<Bodypart> Parts
        {
            get { return fParts; }
        }


        protected AbstractBody(object owner)
            : base(owner)
        {
            fParts = new List<Bodypart>();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) {
                fParts = null;
            }
            base.Dispose(disposing);
        }

        public Bodypart AddPart(int type)
        {
            return AddPart(new Bodypart(type));
        }

        public Bodypart AddPart(int type, int state)
        {
            return AddPart(new Bodypart(type, state));
        }

        public Bodypart AddPart(Bodypart bodypart)
        {
            fParts.Add(bodypart);
            return bodypart;
        }

        public Bodypart GetOccupiedPart(object item)
        {
            if (item == null) {
                return null;
            }

            int num = fParts.Count;
            for (int i = 0; i < num; i++) {
                Bodypart entry = fParts[i];
                if (entry.Item == item) {
                    return entry;
                }
            }

            return null;
        }

        public Bodypart GetUnoccupiedPart(int part)
        {
            int num = fParts.Count;
            for (int i = 0; i < num; i++) {
                Bodypart entry = fParts[i];
                if (entry.Type == part && entry.Item == null) {
                    return entry;
                }
            }

            return null;
        }

        public abstract void Update();
    }
}
