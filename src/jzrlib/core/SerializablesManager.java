/*
 *  "JZRLib", Java Roguelike games development Library.
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
package jzrlib.core;

/**
 *
 * @author Serg V. Zhdanovskih
 */
public final class SerializablesManager
{
    private static final ISerializableCreateHandler[] fSerializables;

    static {
        fSerializables = new ISerializableCreateHandler[256];
        clearSerializables();
    }

    public static void clearSerializables()
    {
        for (int i = 0; i < 256; i++) {
            fSerializables[i] = null;
        }
    }

    public static ISerializable createSerializable(int kind, Object owner)
    {
        ISerializableCreateHandler createProc = fSerializables[kind];
        if (createProc == null) {
            throw new RuntimeException(String.format("Object kind unknown (%d)", new Object[]{kind}));
        }
        return createProc.invoke(owner);
    }

    public static void registerSerializable(int kind, ISerializableCreateHandler createProc)
    {
        fSerializables[kind] = createProc;
    }
}
