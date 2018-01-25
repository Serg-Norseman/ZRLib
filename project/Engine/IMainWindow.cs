/*
 *  "NorseWorld: Ragnarok", a roguelike game for PCs.
 *  Copyright (C) 2002-2008, 2014 by Serg V. Zhdanovskih (aka Alchemist).
 *
 *  this file is part of "NorseWorld: Ragnarok".
 *
 *  this program is free software: you can redistribute it and/or modify
 *  it under the terms of the GNU General Public License as published by
 *  the Free Software Foundation, either version 3 of the License, or
 *  (at your option) any later version.
 *
 *  this program is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *  GNU General Public License for more details.
 *
 *  You should have received a copy of the GNU General Public License
 *  along with this program.  If not, see <http://www.gnu.org/licenses/>.
 */

using System;

namespace ZRLib.Engine
{
    /// <summary>
    /// 
    /// </summary>
    public interface IMainWindow
    {
        void DoActive(bool active);
        void ProcessGameStep();
        void ProcessKeyDown(KeyEventArgs eventArgs);
        void ProcessKeyPress(KeyPressEventArgs eventArgs);
        void ProcessKeyUp(KeyEventArgs eventArgs);
        void ProcessMouseDown(MouseEventArgs eventArgs);
        void ProcessMouseMove(MouseMoveEventArgs eventArgs);
        void ProcessMouseUp(MouseEventArgs eventArgs);
        void ProcessMouseWheel(MouseWheelEventArgs eventArgs);
        void Update(long time);
    }
}
