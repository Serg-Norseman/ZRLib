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
using System.Text;
using BSLib;
using ZRLib.Core;
using ZRLib.Engine;
using ZRLib.Engine.sdl2;

namespace ZRLib.Engine
{
    public delegate void ICharTransformer(int x, int y, TermChar data);

    public sealed class TermChar
    {
        public char Character;
        public int Foreground;
        public int Background;
    }

    /// <summary>
    /// This simulates a ASCII terminal display.
    /// </summary>
    public class Terminal : BaseObject, IMainWindow
    {
        private int fCurrentBackground;
        private int fCurrentForeground;
        private BaseImage fFontImage;
        private int fFontCols;
        private int fCharHeight;
        private int fCharWidth;
        private readonly TermChar[] fTermBuffer;
        private readonly int fTermHeight;
        private readonly int fTermWidth;
        private int fSize;
        private BaseSystem fSystem;
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

        public int TextBackground
        {
            get { return fCurrentBackground; }
            set { fCurrentBackground = value; }
        }

        public int TextForeground
        {
            get { return fCurrentForeground; }
            set { fCurrentForeground = value; }
        }

        public BaseSystem System
        {
            get { return fSystem; }
        }

        public BaseScreen Screen
        {
            get { return fSystem.Screen; }
        }


        public Terminal(int width, int height, string fontName, int fontCols, int charWidth, int charHeight)
        {
            if (width < 1) {
                throw new ArgumentException("width " + width + " must be greater than 0.");
            }

            if (height < 1) {
                throw new ArgumentException("height " + height + " must be greater than 0.");
            }

            fCurrentBackground = Colors.Black;
            fCurrentForeground = Colors.LightGray;
            fSize = height * width;
            fTermWidth = width;
            fTermHeight = height;

            fTermBuffer = new TermChar[fSize];
            for (int i = 0; i < fSize; i++) {
                fTermBuffer[i] = new TermChar();
            }

            LoadGlyphs(fontName, fontCols, charWidth, charHeight);
            Clear();
        }

        private void LoadGlyphs(string fontName, int fontCols, int charWidth, int charHeight)
        {
            try {
                fFontCols = fontCols;
                fCharWidth = charWidth;
                fCharHeight = charHeight;

                int winWidth = fCharWidth * fTermWidth;
                int winHeight = fCharHeight * fTermHeight;
                fSystem = new SDL2System(this, winWidth, winHeight, false);

                fFontImage = fSystem.Screen.CreateImage();
                fFontImage.LoadFromFile(fontName, Colors.None);
                /*Assembly assembly = typeof(Terminal).Assembly;
                using (Stream stmImage = assembly.GetManifestResourceStream("Resources." + fontName)) {
                    fFontImage = new SDL2Image(fSystem.Screen); //fsc Image.FromStream(stmImage);
                    //((SDL2Image)fFontImage).Type = "PNG";
                    fFontImage.LoadFromStream(stmImage, Colors.None);
                }*/
            } catch (Exception e) {
                Logger.Write("loadGlyphs(): " + e.Message);
            }
        }

        private void InternalPaint(BaseScreen gfx)
        {
            try {
                int dx, dy;
                int prevbg = 0, prevfg = 0;
                for (int y = 0; y < fTermHeight; y++) {
                    for (int x = 0; x < fTermWidth; x++) {
                        int idx = y * fTermWidth + x;
                        TermChar tmc = fTermBuffer[idx];
                        int bg = tmc.Background;
                        int fg = tmc.Foreground;

                        if ((prevbg != bg) || (prevfg != fg)) {
                            fFontImage.ReplaceColor(0, bg);
                            fFontImage.ReplaceColor(1, fg);

                            prevbg = bg;
                            prevfg = fg;
                        }

                        int sx = (tmc.Character % fFontCols) * fCharWidth;
                        int sy = (tmc.Character / fFontCols) * fCharHeight;
                        dx = x * fCharWidth;
                        dy = y * fCharHeight;
                        gfx.DrawImage(dx, dy, sx, sy, fCharWidth, fCharHeight, fFontImage, 255);
                    }
                }
            } catch (Exception ex) {
                Logger.Write("Terminal.InternalPaint(): " + ex.Message);
            }
        }

        public void Repaint(int delayInterval)
        {
            BaseSystem.Sleep(delayInterval);
            Repaint();
        }

        public void Repaint()
        {
            BaseScreen scr = fSystem.Screen;
            if (scr != null) {
                try {
                    scr.BeginPaint();
                    UpdateView();
                    InternalPaint(scr);
                    scr.EndPaint();

                    /*fFrameCount += 1;
                    long now = BaseSystem.TickCount;
                    if (now > fFrameStartTime + 1000) {
                        FPS = ((1000 * fFrameCount / (now - fFrameStartTime)));
                        fFrameStartTime = now;
                        fFrameCount = 0;
                    }*/
                } catch (Exception ex) {
                    Logger.Write("Terminal.Repaint(): " + ex.Message);
                }
            }
        }

        protected virtual void UpdateView()
        {
            // dummy
        }

        private int WriteChar(char chr, int pos, int fg, int bg)
        {
            switch (chr) {
                case '\n':
                    pos = ((pos + fTermWidth) / fTermWidth) * fTermWidth;
                    break;

                default:
                    TermChar tmc = fTermBuffer[pos];
                    tmc.Character = chr;
                    tmc.Foreground = fg;
                    tmc.Background = bg;
                    pos++;
                    break;
            }
            if (pos >= fSize) {
                pos = 0;
            }
            return pos;
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

        public void Clear(char character, int foreground, int background)
        {
            for (int i = 0; i < fSize; i++) {
                var tmc = fTermBuffer[i];
                tmc.Character = character;
                tmc.Background = background;
                tmc.Foreground = foreground;
            }
        }

        public void Fill(char character, int x, int y, int width, int height)
        {
            Fill(character, x, y, width, height, fCurrentForeground, fCurrentBackground);
        }

        public void Fill(char character, int x, int y, int width, int height, int foreground, int background)
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

        public void Write(char chr, int foreground)
        {
            Write(chr, foreground, fCurrentBackground);
        }

        public void Write(char chr, int foreground, int background)
        {
            fTermCursor = WriteChar(chr, fTermCursor, foreground, background);
        }

        public void Write(int x, int y, char chr)
        {
            SetCursorPos(x, y);
            Write(chr, fCurrentForeground, fCurrentBackground);
        }

        public void Write(int x, int y, char chr, int foreground)
        {
            SetCursorPos(x, y);
            Write(chr, foreground, fCurrentBackground);
        }

        public void Write(int x, int y, char chr, int foreground, int background)
        {
            SetCursorPos(x, y);
            Write(chr, foreground, background);
        }

        public void Write(string str)
        {
            Write(str, fCurrentForeground, fCurrentBackground);
        }

        public void Write(string str, int foreground)
        {
            Write(str, foreground, fCurrentBackground);
        }

        public void Write(string str, int foreground, int background)
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

        public void Write(int x, int y, string str, int foreground)
        {
            Write(x, y, str, foreground, fCurrentBackground);
        }

        public void Write(int x, int y, string str, int foreground, int background)
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

        public void Quit()
        {
            fSystem.Quit();
        }

        #region IMainWindow implementation

        protected override void Dispose(bool disposing)
        {
            if (disposing) {
                fSystem.Dispose();
            }
            base.Dispose(disposing);
        }

        public virtual void DoActive(bool active)
        {
            // dummy
        }

        public virtual void ProcessGameStep()
        {
            // dummy
        }

        public virtual void ProcessKeyDown(ZRLib.Engine.KeyEventArgs eventArgs)
        {
            // dummy
        }

        public virtual void ProcessKeyPress(ZRLib.Engine.KeyPressEventArgs eventArgs)
        {
            // dummy
        }

        public virtual void ProcessKeyUp(ZRLib.Engine.KeyEventArgs eventArgs)
        {
            // dummy
        }

        public virtual void ProcessMouseDown(ZRLib.Engine.MouseEventArgs eventArgs)
        {
            // dummy
        }

        public virtual void ProcessMouseMove(MouseMoveEventArgs eventArgs)
        {
            // dummy
        }

        public virtual void ProcessMouseUp(ZRLib.Engine.MouseEventArgs eventArgs)
        {
            // dummy
        }

        public virtual void ProcessMouseWheel(MouseWheelEventArgs eventArgs)
        {
            // dummy
        }

        public virtual void Update(long time)
        {
            Repaint();
        }

        #endregion
    }
}
