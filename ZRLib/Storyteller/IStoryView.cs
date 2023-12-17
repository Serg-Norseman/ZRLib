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

namespace ZRLib.Storyteller
{
    /// <summary>
    /// 
    /// </summary>
    public interface IStoryView
    {
        void Clear();
        void AddText(string text);
        void AddAnswerText(string text, string tag);
        void BeginAnswers();
    }
}
