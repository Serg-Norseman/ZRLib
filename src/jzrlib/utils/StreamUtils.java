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
package jzrlib.utils;

import java.io.IOException;
import jzrlib.core.FileVersion;
import jzrlib.core.Point;
import jzrlib.core.Rect;
import jzrlib.external.BinaryInputStream;
import jzrlib.external.BinaryOutputStream;

/**
 *
 * @author Serg V. Zhdanovskih
 */
public final class StreamUtils
{
    public static boolean readBoolean(BinaryInputStream S) throws IOException
    {
        return StreamUtils.readByte(S) > 0;
    }

    public static void writeBoolean(BinaryOutputStream S, boolean val) throws IOException
    {
        StreamUtils.writeByte(S, (byte) (val ? 1 : 0));
    }


    public static int readByte(BinaryInputStream S) throws IOException
    {
        return S.readUnsigned8();
    }

    public static void writeByte(BinaryOutputStream S, byte val) throws IOException
    {
        S.writeUnsigned8(val);
    }


    public static int readWord(BinaryInputStream S) throws IOException
    {
        return S.readUnsigned16();
    }

    public static void writeWord(BinaryOutputStream S, short val) throws IOException
    {
        S.writeUnsigned16(val);
    }


    public static int readInt(BinaryInputStream S) throws IOException
    {
        return S.readSigned32();
    }

    public static void writeInt(BinaryOutputStream S, int val) throws IOException
    {
        S.writeSigned32(val);
    }


    public static float readFloat(BinaryInputStream S) throws IOException
    {
        return S.readFloat();
    }

    public static void writeFloat(BinaryOutputStream S, float val) throws IOException
    {
        S.writeFloat(val);
    }


    public static double readDouble(BinaryInputStream S) throws IOException
    {
        return S.readDouble();
    }

    public static void writeDouble(BinaryOutputStream S, double val) throws IOException
    {
        S.writeDouble(val);
    }



    public static FileVersion readFileVersion(BinaryInputStream S) throws IOException
    {
        FileVersion result = new FileVersion();
        result.Release = (short) StreamUtils.readWord(S);
        result.Revision = (short) StreamUtils.readWord(S);
        return result;
    }

    public static void writeFileVersion(BinaryOutputStream S, FileVersion FV) throws IOException
    {
        StreamUtils.writeWord(S, FV.Release);
        StreamUtils.writeWord(S, FV.Revision);
    }


    public static String readString(BinaryInputStream S) throws IOException
    {
        int sLen = StreamUtils.readInt(S);
        byte[] buffer = new byte[sLen];
        for (int i = 0; i < sLen; i++) {
            buffer[i] = (byte) StreamUtils.readByte(S);
        }

        return new String(buffer, 0, sLen, "Cp1251");
    }

    public static void writeString(BinaryOutputStream S, String val) throws IOException
    {
        byte[] data;
        if (TextUtils.isNullOrEmpty(val)) {
            data = new byte[0];
        } else {
            data = val.getBytes("cp1251");
        }

        int sLen = data.length;
        StreamUtils.writeInt(S, sLen);

        for (int i = 0; i < sLen; i++) {
            StreamUtils.writeByte(S, data[i]);
        }
    }

    ///

    public static String readText(BinaryInputStream S) throws IOException
    {
        int sLen = (int) S.available();
        byte[] buffer = new byte[sLen];
        for (int i = 0; i < sLen; i++) {
            buffer[i] = (byte) StreamUtils.readByte(S);
        }

        return new String(buffer, 0, sLen, "Cp1251");
    }


    public static Point readPoint(BinaryInputStream S) throws IOException
    {
        Point point = new Point();
        point.X = StreamUtils.readInt(S);
        point.Y = StreamUtils.readInt(S);
        return point;
    }

    public static void writePoint(BinaryOutputStream S, Point point) throws IOException
    {
        StreamUtils.writeInt(S, point.X);
        StreamUtils.writeInt(S, point.Y);
    }


    public static Rect readRect(BinaryInputStream S) throws IOException
    {
        Rect rect = new Rect();
        rect.Left = StreamUtils.readInt(S);
        rect.Top = StreamUtils.readInt(S);
        rect.Right = StreamUtils.readInt(S);
        rect.Bottom = StreamUtils.readInt(S);
        return rect;
    }

    public static void writeRect(BinaryOutputStream S, Rect rect) throws IOException
    {
        StreamUtils.writeInt(S, rect.Left);
        StreamUtils.writeInt(S, rect.Top);
        StreamUtils.writeInt(S, rect.Right);
        StreamUtils.writeInt(S, rect.Bottom);
    }
}
