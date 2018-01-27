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
using ZRLib.Core;

namespace ZRLib.Map
{
    /// <summary>
    /// It is basic tile implementation.
    /// 
    /// In the simplest case, any tile should have an identifier which shows - what 
    /// is located in that the tile: only grass, tree, or a wall. In the case where 
    /// image of tiles do not allow us to see what is on the ground level under 
    /// a tree or under a wall - quite enough one identifier. However, even in this 
    /// case, it is better to divide identifier into two portions (ground level and 
    /// a higher level) for accurate processing and capabilities for the future 
    /// expansion.
    /// 
    /// To abstract from the specific implementation, these two levels are named: 
    /// <b>background</b> and <b>foreground</b>.
    /// </summary>
    public class BaseTile
    {
        // Tile states
        public const int TS_SEEN = 1;
        public const int TS_VISITED = 2;
        public const int TS_NFM = 4; // special "not free marker"

        public ushort Background;
        public ushort Foreground;
        public ushort BackgroundExt;
        public ushort ForegroundExt;
        public int States;

        // PathSearch runtime
        public byte Pf_status;
        // see TilePathStatus
        public ExtPoint Pf_prev;

        public BaseTile()
        {
            Background = 0;
            Foreground = 0;
            BackgroundExt = 0;
            ForegroundExt = 0;
            States = 0;

            Pf_status = 0;
            Pf_prev = ExtPoint.Empty;
        }

        public virtual void Assign(BaseTile source)
        {
            Background = source.Background;
            Foreground = source.Foreground;
            BackgroundExt = source.BackgroundExt;
            ForegroundExt = source.ForegroundExt;
            States = source.States;
        }

        public void AddStates(int states)
        {
            States |= states;
        }

        public bool EmptyStates
        {
            get { return (States == 0); }
        }

        public void IncludeState(int state)
        {
            States |= state;
        }

        public void ExcludeState(int state)
        {
            States &= (state ^ 255);
        }

        public void IncludeStates(params int[] states)
        {
            foreach (int st in states) {
                States |= st;
            }
        }

        public bool HasState(int state)
        {
            return ((States & state) > 0);
        }

        public static ushort GetVarID(byte _base, byte _var)
        {
            return (ushort)((_var & 0xFF) << 8 | (_base & 0xFF));
        }

        public byte BackBase
        {
            get { return AuxUtils.GetShortLo(Background); }
        }

        public byte BackVar
        {
            get { return AuxUtils.GetShortHi(Background); }
        }

        public int Back
        {
            set { Background = (ushort)value; }
        }

        public void SetBack(int _base, int _var)
        {
            Background = AuxUtils.FitShort(_base, _var);
        }

        public byte ForeBase
        {
            get { return AuxUtils.GetShortLo(Foreground); }
        }

        public byte ForeVar
        {
            get { return AuxUtils.GetShortHi(Foreground); }
        }

        public int Fore
        {
            set { Foreground = (ushort)value; }
        }

        public void SetFore(int _base, int _var)
        {
            Foreground = AuxUtils.FitShort(_base, _var);
        }
    }
}
