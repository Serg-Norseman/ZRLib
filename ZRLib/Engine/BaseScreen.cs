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

using System.Text;
using BSLib;

namespace ZRLib.Engine
{
    public abstract class BaseScreen : BaseObject
    {
        public const sbyte FILL_HORZ = 0;
        public const sbyte FILL_VERT = 1;
        public const sbyte FILL_TILE = 2;

        private ExtRect fOldClipRect;

        protected bool fFull;
        protected int fOffsetX;
        protected int fOffsetY;
        protected ExtRect fClipRect;

        public Font Font;
        public int Height;
        public int Width;

        protected BaseScreen(object wnd, bool fullScreen)
        {
            //WndHandle = aWndHandle;
            fFull = fullScreen;
            Font = null;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) {
                FreeData();
            }
            base.Dispose(disposing);
        }

        protected virtual void CreateData()
        {
        }

        protected virtual void FreeData()
        {
        }

        public abstract void BeginPaint();

        public abstract void EndPaint();

        public void SetOffset(int dx, int dy)
        {
            fOffsetX = dx;
            fOffsetY = dy;
        }

        public abstract void Clear(int color);

        public abstract void DrawImage(int dX, int dY, int sX, int sY, int sW, int sH, BaseImage image, int opacity);

        public virtual void DrawLine(int x1, int y1, int x2, int y2, int color)
        {
        }

        public abstract void DrawRectangle(ExtRect rect, int fillColor, int borderColor);

        public abstract void FillRect(ExtRect rect, int color);

        public void DrawFilled(ExtRect rect, sbyte fillKind, int sx, int sy, int sw, int sh, int tx, int ty, BaseImage aTex)
        {
            if (aTex != null) {
                int L = rect.Left;
                int T = rect.Top;
                int R = rect.Right;
                int B = rect.Bottom;
                int W = R - L + 1;
                int H = B - T + 1;

                switch (fillKind) {
                    case FILL_HORZ:
                        {
                            int cnt_w = W / sw + 1;
                            for (int x = 0; x < cnt_w; x++) {
                                int sz_w = sw;
                                int tmx = L + x * sw;
                                if (tmx + sw > R) {
                                    sz_w = R - tmx;
                                }
                                DrawImage(tmx, ty, sx, sy, sz_w, sh, aTex, 255);
                            }
                        }
                        break;

                    case FILL_VERT:
                        {
                            int cnt_h = H / sh + 1;
                            for (int y = 0; y < cnt_h; y++) {
                                int sz_h = sh;
                                int tmy = T + y * sh;
                                if (tmy + sh > B) {
                                    sz_h = B - tmy;
                                }
                                DrawImage(tx, tmy, sx, sy, sw, sz_h, aTex, 255);
                            }
                        }
                        break;

                    case FILL_TILE:
                        {
                            int cnt_h = H / sh;
                            if (H % sh != 0) {
                                cnt_h++;
                            }
                            int cnt_w = W / sw;
                            if (W % sw != 0) {
                                cnt_w++;
                            }

                            for (int y = 0; y < cnt_h; y++) {
                                int sz_h = sh;
                                int tmy = T + y * sh;
                                if (tmy + sh > B) {
                                    sz_h = B - tmy;
                                }

                                for (int x = 0; x < cnt_w; x++) {
                                    int sz_w = sw;
                                    int tmx = L + x * sw;
                                    if (tmx + sw > R) {
                                        sz_w = R - tmx;
                                    }
                                    DrawImage(tmx, tmy, sx, sy, sz_w, sz_h, aTex, 255);
                                }
                            }
                        }
                        break;
                }
            }
        }

        public Font CreateFont(string fileName, Encoding encoding)
        {
            return new Font(this, encoding, fileName);
        }

        public virtual BaseImage CreateImage()
        {
            return new BaseImage(this);
        }

        public virtual BaseImage CreateImage(string fileName, int transColor)
        {
            return new BaseImage(this, fileName, transColor);
        }

        public static int RGB(int r, int g, int b)
        {
            return (int)(r | g << 8 | b << 16);
        }

        public void InitClip(ExtRect clipRect)
        {
            fOldClipRect = fClipRect;
            fClipRect = clipRect;

            if (fClipRect.Left > fClipRect.Right) {
                int temp = fClipRect.Left;
                fClipRect.Left = fClipRect.Right;
                fClipRect.Right = temp;
            }
            if (fClipRect.Top > fClipRect.Bottom) {
                int temp = fClipRect.Top;
                fClipRect.Top = fClipRect.Bottom;
                fClipRect.Bottom = temp;
            }
            if (fClipRect.Right >= Width) {
                fClipRect.Right = Width - 1;
            }
            if (fClipRect.Left < 0) {
                fClipRect.Left = 0;
            }
            if (fClipRect.Bottom >= Height) {
                fClipRect.Bottom = Height - 1;
            }
            if (fClipRect.Top < 0) {
                fClipRect.Top = 0;
            }
        }

        public void DoneClip()
        {
            fClipRect = fOldClipRect;
        }

        public void SetSize(int width, int height)
        {
            Width = width;
            Height = height;
            fClipRect = ExtRect.Create(0, 0, Width, Height);
            FreeData();
            CreateData();
        }

        public int GetTextColor(bool foreground)
        {
            return (Font != null) ? Font.Color : Colors.Black;
        }

        public void SetTextColor(int color, bool foreground)
        {
            if (Font != null && foreground) {
                Font.Color = color;
            }
        }

        public int GetTextHeight(string text)
        {
            return (Font != null) ? Font.Height : 0;
        }

        public int GetTextWidth(string text)
        {
            return (Font != null) ? Font.GetTextWidth(text) : 0;
        }

        public void DrawText(int x, int y, string text, int format)
        {
            if (Font != null && text != null) {
                int sy = y + fOffsetY;
                ExtRect clipRect = fClipRect;

                if (sy + Font.Height > clipRect.Top && sy < clipRect.Bottom) {
                    Font.DrawText(this, x, y, text);
                }
            }
        }
    }
}
