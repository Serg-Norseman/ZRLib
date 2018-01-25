/*
 *  "MysteriesRL", roguelike game.
 *  Copyright (C) 2015, 2017 by Serg V. Zhdanovskih (aka Alchemist, aka Norseman).
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

using MysteriesRL.Game;
using ZRLib.Core.Body;

namespace MysteriesRL.Creatures
{
    public abstract class CustomBody : AbstractBody
    {
        protected CustomBody(object owner)
            : base(owner)
        {
        }

        public new Creature Owner
        {
            get { return (Creature)base.Owner; }
        }

        public MRLGame Space
        {
            get {
                return (MRLGame)Owner.Space;
            }
        }
    }
}
