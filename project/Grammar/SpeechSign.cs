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

namespace ZRLib.Grammar
{
    public enum SyllableKind
    {
        sk_Undefined,
        sk_Opened,
        sk_Closed
    }

    public sealed class SpeechSign
    {
        public string Sign;
        public string Sound;
        public SyllableKind SyllableKind;

        public SpeechSign(string sign, string sound, SyllableKind syllableKind)
        {
            Sign = sign;
            Sound = sound;
            SyllableKind = syllableKind;
        }
    }
}
