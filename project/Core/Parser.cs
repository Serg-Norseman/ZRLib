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

using System;
using System.IO;
using System.Text;
using BSLib;

namespace ZRLib.Core
{
    public class ParserException : Exception
    {
        public ParserException()
        {
        }

        public ParserException(string message)
            : base(message)
        {
        }
    }

    public sealed class Parser : BaseObject
    {
        public const char сptEOF = (char)0;
        public const char сptSymbol = (char)1;
        public const char сptString = (char)2;
        public const char сptInteger = (char)3;
        public const char сptFloat = (char)4;

        private readonly char[] fBuffer = new char[4096];
        private Encoding fEncoding;
        private readonly BinaryReader fStream;

        private int fBufPos;
        private int fBufSize;
        private string fTemp;

        public int SourceLine;
        public char Token;

        private void ErrorStr(string message)
        {
            throw new ParserException(string.Format("{0} on line {1:D}", new object[]{ message, SourceLine }));
        }

        private void NextChar()
        {
            if (fBufPos == fBufSize - 1) {
                byte[] buf = new byte[4096];
                buf = fStream.ReadBytes(4096);
                fBufSize = buf.Length;

                string str = fEncoding.GetString(buf);
                for (int i = 0; i < fBufSize; i++) {
                    fBuffer[i] = (char)str[i];
                }
                fBufPos = 0;
            } else {
                fBufPos++;
            }
        }

        public Parser(BinaryReader stream, Encoding encoding)
        {
            fEncoding = encoding;
            fStream = stream;
            fBufSize = 0;
            fBufPos = 0;
            NextToken();
        }

        public void CheckToken(char T)
        {
            if (Token != T) {
                switch (T) {
                    case сptSymbol:
                        ErrorStr("Identifier expected");
                        break;
                    case сptString:
                        ErrorStr("String expected");
                        break;
                    case сptInteger:
                    case сptFloat:
                        ErrorStr("Number expected");
                        break;
                    default:
                        ErrorStr(string.Format("\"{0}\" expected", new object[]{ T }));
                        break;
                }
            }
        }

        public char NextToken()
        {
            try {
                byte[] buf = new byte[4096];
                fTemp = "";

                char result;

                // skip blanks
                while (true) {
                    if (fBufPos >= fBufSize) {
                        buf = fStream.ReadBytes(4096);
                        fBufSize = buf.Length;

                        string str = fEncoding.GetString(buf);
                        for (int i = 0; i < fBufSize; i++) {
                            fBuffer[i] = (char)str[i];
                        }

                        if (fBufSize <= 0) {
                            result = сptEOF;
                            Token = result;
                            return result;
                        }

                        fBufPos = 0;
                    } else {
                        byte b = (byte)fBuffer[fBufPos];
                        if (b == 10) {
                            SourceLine++;
                        } else {
                            if (b >= 33 && b <= 255) {
                                break;
                            }
                        }

                        NextChar();
                    }
                }

                //
                char c = (char)fBuffer[fBufPos];
                if ((c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z') || (c == '_')) {
                    fTemp += (fBuffer[fBufPos]);
                    NextChar();
                    while (true) {
                        char c5 = (char)fBuffer[fBufPos];
                        if (c5 < '0' || (c5 >= ':' && (c5 < 'A' || (c5 >= '[' && c5 != '_' && (c5 < 'a' || c5 >= '{'))))) {
                            break;
                        }
                        fTemp += (fBuffer[fBufPos]);
                        NextChar();
                    }
                    result = сptSymbol;
                } else if (c == '{') {
                    while (true) {
                        char c6 = (char)fBuffer[fBufPos];
                        if (c6 != '{') {
                            break;
                        }

                        NextChar();

                        while (true) {
                            char c7 = (char)fBuffer[fBufPos];
                            if (c7 != '{') {
                                if (c7 == '}') {
                                    NextChar();
                                    if ((short)fBuffer[fBufPos] != 125) {
                                        break;
                                    }
                                }
                            } else {
                                ErrorStr("Invalid string constant");
                            }
                            fTemp += (fBuffer[fBufPos]);
                            NextChar();
                        }
                    }
                    result = сptString;
                } else if (c == '$') {
                    fTemp += (fBuffer[fBufPos]);
                    NextChar();
                    while (true) {
                        char c2 = (char)fBuffer[fBufPos];
                        if (c2 < '0' || (c2 >= ':' && (c2 < 'A' || (c2 >= 'G' && (c2 < 'a' || c2 >= 'g'))))) {
                            break;
                        }
                        fTemp += (fBuffer[fBufPos]);
                        NextChar();
                    }
                    result = сptInteger;
                } else if ((c == '-') || (c >= '0' && c <= '9')) {
                    fTemp += (fBuffer[fBufPos]);
                    NextChar();

                    while (true) {
                        char c3 = (char)fBuffer[fBufPos];
                        if (c3 < '0' || c3 >= ':') {
                            break;
                        }
                        fTemp += (fBuffer[fBufPos]);
                        NextChar();
                    }

                    result = сptInteger;

                    while (true) {
                        char c4 = fBuffer[fBufPos];
                        if (c4 != '+' && (c4 < '-' || (c4 >= '/' && (c4 < '0' || (c4 >= ':' && c4 != 'E' && c4 != 'e'))))) {
                            break;
                        }
                        fTemp += (fBuffer[fBufPos]);
                        NextChar();
                        result = сptFloat;
                    }
                } else {
                    result = (char)fBuffer[fBufPos];
                    NextChar();
                }

                Token = result;
                return result;
            } catch (Exception) {
                return сptEOF;
            }
        }

        public double TokenFloat()
        {
            return Convert.ToDouble(TokenString());
        }

        public int TokenInt()
        {
            return Convert.ToInt32(TokenString());
        }

        public string TokenString()
        {
            return fTemp;
        }
    }
}
