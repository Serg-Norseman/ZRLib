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

namespace ZRLib.Core
{
    public interface ISerializable
    {
        byte SerializeKind { get; }

        void LoadFromStream(BinaryReader stream, FileVersion version);

        void SaveToStream(BinaryWriter stream, FileVersion version);
    }

    public delegate ISerializable ISerializableCreateHandler(object owner);

    public static class SerializablesManager
    {
        private static readonly ISerializableCreateHandler[] fSerializables;

        static SerializablesManager()
        {
            fSerializables = new ISerializableCreateHandler[256];
            ClearSerializables();
        }

        public static void ClearSerializables()
        {
            for (int i = 0; i < 256; i++) {
                fSerializables[i] = null;
            }
        }

        public static ISerializable CreateSerializable(int kind, object owner)
        {
            ISerializableCreateHandler createProc = fSerializables[kind];
            if (createProc == null) {
                throw new Exception(string.Format("Object kind unknown ({0:D})", new object[]{ kind }));
            }
            return createProc(owner);
        }

        public static void RegisterSerializable(int kind, ISerializableCreateHandler createProc)
        {
            fSerializables[kind] = createProc;
        }
    }
}
