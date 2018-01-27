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

using System.IO;
using System.Text;
using BSLib;

namespace ZRLib.Core
{
    public static class StreamUtils
    {
        public static bool ReadBoolean(BinaryReader S)
        {
            return ReadByte(S) > 0;
        }

        public static void WriteBoolean(BinaryWriter S, bool val)
        {
            WriteByte(S, (byte)(val ? 1 : 0));
        }

        public static byte ReadByte(BinaryReader S)
        {
            return S.ReadByte();
        }

        public static void WriteByte(BinaryWriter S, byte val)
        {
            S.Write(val);
        }

        public static ushort ReadWord(BinaryReader S)
        {
            return S.ReadUInt16();
        }

        public static void WriteWord(BinaryWriter S, ushort val)
        {
            S.Write(val);
        }

        public static int ReadInt(BinaryReader S)
        {
            return S.ReadInt32();
        }

        public static void WriteInt(BinaryWriter S, int val)
        {
            S.Write(val);
        }

        public static float ReadFloat(BinaryReader S)
        {
            return S.ReadSingle();
        }

        public static void WriteFloat(BinaryWriter S, float val)
        {
            S.Write(val);
        }

        public static double ReadDouble(BinaryReader S)
        {
            return S.ReadDouble();
        }

        public static void WriteDouble(BinaryWriter S, double val)
        {
            S.Write(val);
        }

        public static FileVersion ReadFileVersion(BinaryReader S)
        {
            FileVersion result = new FileVersion();
            result.Release = ReadWord(S);
            result.Revision = ReadWord(S);
            return result;
        }

        public static void WriteFileVersion(BinaryWriter S, FileVersion FV)
        {
            WriteWord(S, FV.Release);
            WriteWord(S, FV.Revision);
        }

        public static string ReadString(BinaryReader S, Encoding encoding)
        {
            int sLen = ReadInt(S);
            byte[] buffer = new byte[sLen];
            for (int i = 0; i < sLen; i++) {
                buffer[i] = (byte)ReadByte(S);
            }

            return encoding.GetString(buffer, 0, sLen);
        }

        public static void WriteString(BinaryWriter S, string val, Encoding encoding)
        {
            byte[] data;
            if (string.IsNullOrEmpty(val)) {
                data = new byte[0];
            } else {
                data = encoding.GetBytes(val);
            }

            int sLen = data.Length;
            WriteInt(S, sLen);

            for (int i = 0; i < sLen; i++) {
                WriteByte(S, data[i]);
            }
        }

        public static string ReadText(BinaryReader S, Encoding encoding)
        {
            int sLen = (int)(S.BaseStream.Length - S.BaseStream.Position);
            byte[] buffer = new byte[sLen];
            for (int i = 0; i < sLen; i++) {
                buffer[i] = (byte)ReadByte(S);
            }

            return encoding.GetString(buffer, 0, sLen);
        }

        public static ExtPoint ReadPoint(BinaryReader S)
        {
            ExtPoint point = new ExtPoint();
            point.X = ReadInt(S);
            point.Y = ReadInt(S);
            return point;
        }

        public static void WritePoint(BinaryWriter S, ExtPoint point)
        {
            WriteInt(S, point.X);
            WriteInt(S, point.Y);
        }

        public static ExtRect ReadRect(BinaryReader S)
        {
            ExtRect rect = new ExtRect();
            rect.Left = ReadInt(S);
            rect.Top = ReadInt(S);
            rect.Right = ReadInt(S);
            rect.Bottom = ReadInt(S);
            return rect;
        }

        public static void WriteRect(BinaryWriter S, ExtRect rect)
        {
            WriteInt(S, rect.Left);
            WriteInt(S, rect.Top);
            WriteInt(S, rect.Right);
            WriteInt(S, rect.Bottom);
        }
    }
}
