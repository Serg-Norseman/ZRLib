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
using ZRLib.Core;

namespace ZRLib.Engine
{
    public sealed class Font : BaseObject
    {
        private sealed class TGEFontChar
        {
            public short Offset;
            public sbyte Width;
            public sbyte Adjust;
        }

        private int fColor;
        private BaseImage fImage;
        private BaseScreen fScreen;
        private TGEFontChar[] fChars;
        private int fCharColor;
        private string fImageFile;
        private Encoding fEncoding;

        public int Height;
        public string Name;

        public Font(BaseScreen screen, Encoding encoding)
        {
            fScreen = screen;
            fEncoding = encoding;

            fChars = new TGEFontChar[256];
            for (int i = 0; i < 256; i++) {
                fChars[i] = new TGEFontChar();
            }
        }

        public Font(BaseScreen screen, Encoding encoding, string fileName)
            : this(screen, encoding)
        {
            LoadFromFile(fileName);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) {
                if (fImage != null) {
                    fImage.Dispose();
                }
            }
            base.Dispose(disposing);
        }

        public int Color
        {
            get {
                return fColor;
            }
            set {
                if (fColor != value) {
                    fImage.ReplaceColor(fCharColor, value);
                    fColor = value;
                }
            }
        }

        public void LoadFromFile(string fileName)
        {
            try {
                Stream stm = ResourceManager.LoadStream(fileName);
                BinaryReader bis = new BinaryReader(stm);
                Parser parser = new Parser(bis, fEncoding);
                try {
                    fImageFile = "";
                    while (parser.Token != Parser.сptEOF) {
                        char token = parser.Token;
                        if (token != Parser.сptSymbol) {
                            if (token != '@') {
                                if (token == '{') {
                                    do {
                                        parser.NextToken();
                                    } while (parser.Token != '}');
                                }
                            } else {
                                parser.NextToken();
                                int c = parser.TokenInt();
                                parser.NextToken();
                                fChars[c].Width = (sbyte)parser.TokenInt();
                                parser.NextToken();
                                fChars[c].Adjust = (sbyte)parser.TokenInt();
                            }
                        } else {
                            string id = parser.TokenString();
                            parser.NextToken();
                            parser.CheckToken('=');
                            parser.NextToken();
                            string value = parser.TokenString();

                            if (id.CompareTo("Name") == 0) {
                                Name = value;
                            } else {
                                if (id.CompareTo("ImageFile") == 0) {
                                    fImageFile = value;
                                } else {
                                    if (id.CompareTo("Height") == 0) {
                                        Height = Convert.ToInt32(value);
                                    } else {
                                        if (id.CompareTo("CharDivider") == 0) {
                                            //fDivider = TAuxUtils.StrToInt(value);
                                        } else {
                                            if (id.CompareTo("CharColor") == 0) {
                                                fCharColor = Convert.ToInt32(value);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        parser.NextToken();
                    }

                    int offset = 1;
                    for (int b = 0; b <= 255; b++) {
                        if (fChars[b].Width > 0) {
                            fChars[b].Offset = (short)offset;
                            offset = offset + (int)fChars[b].Width + 1;
                        }
                    }

                    fImageFile = AuxUtils.ChangeExtension(fileName, ".tga");

                    fImage = fScreen.CreateImage();
                    fImage.PaletteMode = true;

                    Stream is1 = ResourceManager.LoadStream(fImageFile);
                    fImage.LoadFromStream(is1, Colors.Black);

                    fColor = Colors.White;
                } finally {
                    if (parser != null) {
                        parser.Dispose();
                    }
                }
            } catch (Exception ex) {
                Logger.Write("Font.loadFromFile(): " + ex.Message);
            }
        }

        public bool IsValidChar(char aChar)
        {
            try {
                string str = Convert.ToString(aChar);
                byte[] buf = fEncoding.GetBytes(str);
                int idx = (int)(buf[0] & 0xff);
                return (fChars[idx].Width > 0);
            } catch (Exception) {
                return false;
            }
        }

        public int GetTextWidth(string text)
        {
            int result = 0;
            if (string.IsNullOrEmpty(text)) {
                return result;
            }

            try {
                byte[] data = fEncoding.GetBytes(text);
                for (int i = 0; i < data.Length; i++) {
                    int idx = (data[i] & 0xff);
                    TGEFontChar fc = fChars[idx];

                    int symW = fc.Width;
                    int adj = fc.Adjust;
                    if (symW > 0) {
                        if (adj > 0 && i > 0) {
                            result -= adj;
                        }
                        result = result + symW + 1;
                        if (adj < 0) {
                            result += adj;
                        }
                    }
                }
                result--;
            } catch (Exception ex) {
                Logger.Write("Font.GetTextWidth(): " + ex.Message);
            }

            return result;
        }

        public void DrawText(BaseScreen screen, int aX, int aY, string text)
        {
            if (string.IsNullOrEmpty(text)) {
                return;
            }

            try {
                int symH = Height;
                int xx = aX;

                byte[] data = fEncoding.GetBytes(text);
                for (int i = 0; i < data.Length; i++) {
                    int idx = (data[i] & 0xff);
                    TGEFontChar chr = fChars[idx];

                    int symW = chr.Width;
                    if (symW > 0) {
                        int adj = chr.Adjust;
                        int offX = chr.Offset;
                        if (adj > 0 && i > 0) {
                            xx -= adj;
                        }
                        screen.DrawImage(xx, aY, offX, 0, symW, symH, fImage, 255);
                        xx = xx + symW + 1;
                        if (adj < 0) {
                            xx += adj;
                        }
                    }
                }
            } catch (Exception ex) {
                Logger.Write("Font.DrawText(): " + ex.Message);
            }
        }
    }
}
