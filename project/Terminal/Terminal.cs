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
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using BSLib;
using ZRLib.Core;

namespace ZRLib.Terminal
{
    public delegate void ICharTransformer(int x, int y, TermChar data);

    public sealed class TermChar
    {
        public char Character;
        public int Foreground;
        public int Background;

        public char PrevCharacter;
        public int PrevForeground;
        public int PrevBackground;
    }

    /// <summary>
    /// This simulates a ASCII terminal display.
    /// </summary>
    public sealed class Terminal : UserControl
    {
        private Bitmap fBufferImage;
        private bool fBufferChanged;
        private Color fCurrentBackground;
        private Color fCurrentForeground;
        private Image fFontImage;
        private int fFontCols;
        private int fCharHeight;
        private int fCharWidth;
        private readonly TermChar[] fTermBuffer;
        private readonly int fTermHeight;
        private readonly int fTermWidth;
        private int fSize;
        private int fTermCursor = 0;


        public int CharHeight
        {
            get { return fCharHeight; }
        }

        public int CharWidth
        {
            get { return fCharWidth; }
        }

        public int TermHeight
        {
            get { return fTermHeight; }
        }

        public int TermWidth
        {
            get { return fTermWidth; }
        }

        public int CursorX
        {
            get { return fTermCursor % fTermWidth; }
        }

        public int CursorY
        {
            get { return fTermCursor / fTermWidth; }
        }

        public Color TextBackground
        {
            get { return fCurrentBackground; }
            set { fCurrentBackground = value; }
        }

        public Color TextForeground
        {
            get { return fCurrentForeground; }
            set { fCurrentForeground = value; }
        }


        public Terminal(int width, int height)
        {
            if (width < 1) {
                throw new ArgumentException("width " + width + " must be greater than 0.");
            }

            if (height < 1) {
                throw new ArgumentException("height " + height + " must be greater than 0.");
            }

            LoadGlyphs("cp866_8x8.png", 32, 8, 8);

            fCurrentBackground = Color.Black;
            fCurrentForeground = Color.LightGray;
            fSize = height * width;
            fTermWidth = width;
            fTermHeight = height;

            fTermBuffer = new TermChar[fSize];
            for (int i = 0; i < fSize; i++) {
                fTermBuffer[i] = new TermChar();
            }

            Size = new Size(fCharWidth * fTermWidth, fCharHeight * fTermHeight);
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer, true);

            Clear();
        }

        private void LoadGlyphs(string fontName, int fontCols, int charWidth, int charHeight)
        {
            try {
                fFontCols = fontCols;
                fCharWidth = charWidth;
                fCharHeight = charHeight;

                Assembly assembly = typeof(Terminal).Assembly;
                using (Stream stmImage = assembly.GetManifestResourceStream("Resources." + fontName)) {
                    fFontImage = Image.FromStream(stmImage);
                }
            } catch (Exception e) {
                Logger.Write("loadGlyphs(): " + e.Message);
            }
        }

        protected override void OnResize(EventArgs e)
        {
            RecreateBuffer();
            base.OnResize(e);
        }

        private void RecreateBuffer()
        {
            if (Width != 0 && Height != 0) {
                fBufferImage = new Bitmap(Width, Height, PixelFormat.Format32bppRgb);
                fBufferChanged = true;
            }
        }

        private void InternalPaint(Graphics maingfx)
        {
            try {
                Graphics gfx = Graphics.FromImage(fBufferImage);
                gfx.SmoothingMode = SmoothingMode.None;

                ColorMap[] colorMap = new ColorMap[2];
                colorMap[0] = new ColorMap();
                colorMap[0].OldColor = Color.White;
                colorMap[0].NewColor = fCurrentForeground;
                colorMap[1] = new ColorMap();
                colorMap[1].OldColor = Color.Black;
                colorMap[1].NewColor = fCurrentBackground;
                ImageAttributes attr = new ImageAttributes();
                attr.SetRemapTable(colorMap);

                Rectangle destRect = new Rectangle(0, 0, fCharWidth, fCharHeight);
                int prevbg = 0, prevfg = 0;
                for (int y = 0; y < fTermHeight; y++) {
                    for (int x = 0; x < fTermWidth; x++) {
                        int idx = y * fTermWidth + x;
                        TermChar tmc = fTermBuffer[idx];
                        int bg = tmc.Background;
                        int fg = tmc.Foreground;

                        if ((tmc.PrevCharacter != tmc.Character) ||
                            (tmc.PrevBackground != bg) || (tmc.PrevForeground != fg) ||
                            fBufferChanged) {
                            tmc.PrevBackground = bg;
                            tmc.PrevForeground = fg;
                            tmc.PrevCharacter = tmc.Character;
                        } else {
                            continue;
                        }

                        if ((prevbg != bg) || (prevfg != fg)) {
                            colorMap[0].NewColor = Color.FromArgb(fg);
                            colorMap[1].NewColor = Color.FromArgb(bg);
                            attr.SetRemapTable(colorMap);

                            prevbg = bg;
                            prevfg = fg;
                        }

                        int sx = (tmc.Character % fFontCols) * fCharWidth;
                        int sy = (tmc.Character / fFontCols) * fCharHeight;
                        destRect.X = x * fCharWidth;
                        destRect.Y = y * fCharHeight;
                        gfx.DrawImage(fFontImage, destRect, sx, sy, fCharWidth, fCharHeight, GraphicsUnit.Pixel, attr, null, IntPtr.Zero);
                    }
                }

                maingfx.DrawImage(fBufferImage, 0, 0);
            } catch (Exception ex) {
                Logger.Write("OnPaint(): " + ex.Message);
            }
        }

        private object gfxLock = new object();

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Graphics gfx = e.Graphics;

            lock (gfxLock) {
                InternalPaint(gfx);
            }
        }

        private int WriteChar(char chr, int pos, Color fg, Color bg)
        {
            lock (gfxLock) {
                switch (chr) {
                    case '\n':
                        pos = ((pos + fTermWidth) / fTermWidth) * fTermWidth;
                        break;

                    default:
                        TermChar tmc = fTermBuffer[pos];
                        tmc.Character = chr;
                        tmc.Foreground = fg.ToArgb();
                        tmc.Background = bg.ToArgb();
                        pos++;
                        break;
                }
                if (pos >= fSize) {
                    pos = 0;
                }
                return pos;
            }
        }

        public void SetCursorPos(int x, int y)
        {
            if (x < 0 || x >= fTermWidth) {
                throw new ArgumentException("x " + x + " must be within range [0," + fTermWidth + ")");
            }

            if (y < 0 || y >= fTermHeight) {
                throw new ArgumentException("y " + y + " must be within range [0," + fTermHeight + ")");
            }

            fTermCursor = y * fTermWidth + x;
        }

        public void Clear()
        {
            Clear(' ', fCurrentForeground, fCurrentBackground);
        }

        public void Clear(char character, Color foreground, Color background)
        {
            int bg = background.ToArgb();
            int fg = foreground.ToArgb();
            lock (gfxLock) {
                for (int i = 0; i < fSize; i++) {
                    var tmc = fTermBuffer[i];
                    tmc.Character = character;
                    tmc.Foreground = fg;
                    tmc.Background = bg;
                }
            }
        }

        public void Fill(char character, int x, int y, int width, int height)
        {
            Fill(character, x, y, width, height, fCurrentForeground, fCurrentBackground);
        }

        public void Fill(char character, int x, int y, int width, int height, Color foreground, Color background)
        {
            if (x < 0 || x >= fTermWidth) {
                throw new ArgumentException("x " + x + " must be within range [0," + fTermWidth + ")");
            }

            if (y < 0 || y >= fTermHeight) {
                throw new ArgumentException("y " + y + " must be within range [0," + fTermHeight + ")");
            }

            if (width < 1) {
                throw new ArgumentException("width " + width + " must be greater than 0.");
            }

            if (height < 1) {
                throw new ArgumentException("height " + height + " must be greater than 0.");
            }

            if (x + width > fTermWidth) {
                throw new ArgumentException("x + width " + (x + width) + " must be less than " + (fTermWidth + 1) + ".");
            }

            if (y + height > fTermHeight) {
                throw new ArgumentException("y + height " + (y + height) + " must be less than " + (fTermHeight + 1) + ".");
            }

            for (int xo = x; xo < x + width; xo++) {
                for (int yo = y; yo < y + height; yo++) {
                    Write(xo, yo, character, foreground, background);
                }
            }
        }

        public void Write(char chr)
        {
            Write(chr, fCurrentForeground, fCurrentBackground);
        }

        public void Write(char chr, Color foreground)
        {
            Write(chr, foreground, fCurrentBackground);
        }

        public void Write(char chr, Color foreground, Color background)
        {
            fTermCursor = WriteChar(chr, fTermCursor, foreground, background);
        }

        public void Write(int x, int y, char chr)
        {
            SetCursorPos(x, y);
            Write(chr, fCurrentForeground, fCurrentBackground);
        }

        public void Write(int x, int y, char chr, Color foreground)
        {
            SetCursorPos(x, y);
            Write(chr, foreground, fCurrentBackground);
        }

        public void Write(int x, int y, char chr, Color foreground, Color background)
        {
            SetCursorPos(x, y);
            Write(chr, foreground, background);
        }

        public void Write(string str)
        {
            Write(str, fCurrentForeground, fCurrentBackground);
        }

        public void Write(string str, Color foreground)
        {
            Write(str, foreground, fCurrentBackground);
        }

        public void Write(string str, Color foreground, Color background)
        {
            if (str == null) {
                throw new NullReferenceException("string must not be null.");
            }

            try {
                byte[] data = Encoding.GetEncoding("Cp866").GetBytes(str);
                for (int i = 0; i < data.Length; i++) {
                    char chr = (char)(data[i] & 0xFF);
                    Write(chr, foreground, background);
                }
            } catch (Exception ex) {
                Logger.Write("Write(): " + ex.Message);
            }
        }

        public void Write(int x, int y, string str)
        {
            Write(x, y, str, fCurrentForeground, fCurrentBackground);
        }

        public void Write(int x, int y, string str, Color foreground)
        {
            Write(x, y, str, foreground, fCurrentBackground);
        }

        public void Write(int x, int y, string str, Color foreground, Color background)
        {
            SetCursorPos(x, y);
            Write(str, foreground, background);
        }

        public ExtPoint GetTerminalPoint(int mx, int my)
        {
            int tx = mx / CharWidth;
            int ty = my / CharHeight;
            return new ExtPoint(tx, ty);
        }

        public void WithEachTile(ICharTransformer transformer)
        {
            WithEachTile(0, 0, fTermWidth, fTermHeight, transformer);
        }

        public void WithEachTile(int left, int top, int width, int height, ICharTransformer transformer)
        {
            if (transformer == null) {
                return;
            }

            for (int x0 = 0; x0 < width; x0++) {
                for (int y0 = 0; y0 < height; y0++) {
                    int x = left + x0;
                    int y = top + y0;

                    if (x < 0 || y < 0 || x >= fTermWidth || y >= fTermHeight) {
                        continue;
                    }

                    int idx = y * fTermWidth + x;
                    TermChar data = fTermBuffer[idx];
                    transformer(x, y, data);
                }
            }
        }

        private static readonly char[] SINGLE_BOX = new char[] {
            (char)218,
            (char)191,
            (char)192,
            (char)217,
            (char)196,
            (char)179
        };

        private static readonly char[] DOUBLE_BOX = new char[] {
            (char)201,
            (char)187,
            (char)200,
            (char)188,
            (char)205,
            (char)186
        };

        public void DrawBox(int x1, int y1, int x2, int y2, bool single, string title)
        {
            DrawBox(x1, y1, x2, y2, single);
            title = " " + title + " ";
            WriteCenter(x1, x2, y1, title);
        }

        public void DrawBox(int x1, int y1, int x2, int y2, bool single)
        {
            char[] charSet;
            if (single) {
                charSet = SINGLE_BOX;
            } else {
                charSet = DOUBLE_BOX;
            }

            for (int xx = x1 + 1; xx <= x2 - 1; xx++) {
                Write(xx, y1, charSet[4]);
                Write(xx, y2, charSet[4]);
            }

            for (int yy = y1 + 1; yy <= y2 - 1; yy++) {
                Write(x1, yy, charSet[5]);
                Write(x2, yy, charSet[5]);
            }

            Write(x1, y1, charSet[0]);
            Write(x2, y1, charSet[1]);
            Write(x1, y2, charSet[2]);
            Write(x2, y2, charSet[3]);
        }

        public void WriteCenter(int x1, int x2, int y, string text)
        {
            int xx = x1 + (x2 - x1 - text.Length) / 2;
            Write(xx, y, text);
        }

        public void DrawProgress(int x1, int x2, int y, float complete, float size)
        {
            int len = (x2 - x1) + 1;
            float percent = (size == 0.0f) ? 0 : (complete / size);
            int bound = x1 + (int)(len * percent) - 1;
            for (int x = x1; x <= x2; x++) {
                char chr = (x <= bound) ? (char)219 : (char)176;
                Write(x, y, chr);
            }
        }
    }
}
