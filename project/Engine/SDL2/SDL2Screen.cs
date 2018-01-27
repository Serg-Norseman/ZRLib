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
using System.Runtime.InteropServices;
using BSLib;
using SDL2;
using ZRLib.Engine;

namespace ZRLib.Engine.sdl2
{
    public sealed class SDL2Screen : BaseScreen
    {
        private readonly IntPtr fWindowPtr;
        private readonly IntPtr fSurfacePtr;
        internal IntPtr fRenderPtr;
        internal IntPtr fFormatPtr;

        private SDL.SDL_Rect fSrcRect;
        private SDL.SDL_Rect fDstRect;
        private SDL.SDL_Rect fRect;

        public SDL2Screen(object wnd, bool fullScreen)
            : base(wnd, fullScreen)
        {
            fWindowPtr = (IntPtr)wnd;
            fRenderPtr = SDL.SDL_CreateRenderer(fWindowPtr, -1, 0);
            fSurfacePtr = SDL.SDL_GetWindowSurface(fWindowPtr);
            fFormatPtr = ((SDL.SDL_Surface)Marshal.PtrToStructure(fSurfacePtr, typeof(SDL.SDL_Surface))).format;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) {
                if (fRenderPtr != IntPtr.Zero) {
                    SDL.SDL_DestroyRenderer(fRenderPtr);
                }
                if (fSurfacePtr != IntPtr.Zero) {
                    SDL.SDL_FreeSurface(fSurfacePtr);
                }
            }
            base.Dispose(disposing);
        }

        public static SDL.SDL_Rect CreateRect(int x, int y, int w, int h)
        {
            var result = new SDL.SDL_Rect();
            result.x = x;
            result.y = y;
            result.w = w;
            result.h = h;
            return result;
        }

        public static uint ConvertColor(IntPtr surfaceFormat, int color)
        {
            byte a, r, g, b;
            GfxHelper.DecomposeARGB(color, out a, out r, out g, out b);
            return SDL.SDL_MapRGB(surfaceFormat, r, g, b);
        }

        public override BaseImage CreateImage()
        {
            return new SDL2Image(this);
        }

        public override BaseImage CreateImage(string fileName, int transColor)
        {
            return new SDL2Image(this, fileName, transColor);
        }

        public override void Clear(int color)
        {
            if (fRenderPtr != IntPtr.Zero) {
                fRect = CreateRect(0, 0, base.Width, base.Height);

                DrawColor = color;
                SDL.SDL_RenderFillRect(fRenderPtr, ref fRect);
            }
        }

        public override void BeginPaint()
        {
            if (fRenderPtr != IntPtr.Zero) {
                SDL.SDL_SetRenderDrawColor(fRenderPtr, (byte) 100, (byte) 0, (byte) 0, (byte) 0);
                SDL.SDL_RenderClear(fRenderPtr);
            }
        }

        public override void EndPaint()
        {
            if (fRenderPtr != IntPtr.Zero) {
                SDL.SDL_RenderPresent(fRenderPtr);
            }
        }

        private int DrawColor
        {
            set {
                byte r, g, b, a;
                GfxHelper.DecomposeARGB(value, out a, out r, out g, out b);
                SDL.SDL_SetRenderDrawColor(fRenderPtr, r, g, b, (byte)0);
            }
        }

        public override void DrawLine(int x1, int y1, int x2, int y2, int color)
        {
            if (fRenderPtr != IntPtr.Zero) {
                DrawColor = color;
                SDL.SDL_RenderDrawLine(fRenderPtr, fOffsetX + x1, fOffsetY + y1, fOffsetX + x2, fOffsetY + y2);
            }
        }

        public override void DrawRectangle(ExtRect rect, int fillColor, int borderColor)
        {
            if (fillColor != Colors.None) {
                FillRect(rect.Clone(), fillColor);
            }

            if (borderColor != Colors.None) {
                rect.Offset(fOffsetX, fOffsetY);
                fRect = CreateRect(rect.Left, rect.Top, rect.Width, rect.Height);

                DrawColor = borderColor;
                SDL.SDL_RenderDrawRect(fRenderPtr, ref fRect);
            }
        }

        public override void FillRect(ExtRect rect, int fillColor)
        {
            if (fillColor != Colors.None) {
                rect.Offset(fOffsetX, fOffsetY);
                fRect = CreateRect(rect.Left, rect.Top, rect.Width, rect.Height);

                DrawColor = fillColor;
                SDL.SDL_RenderFillRect(fRenderPtr, ref fRect);
            }
        }

        public override void DrawImage(int dX, int dY, int sX, int sY, int sW, int sH, BaseImage image, int opacity)
        {
            if (image != null) {
                dX += fOffsetX;
                dY += fOffsetY;

                if (dX >= fClipRect.Right || dY >= fClipRect.Bottom || dX + sW <= fClipRect.Left || dY + sH <= fClipRect.Top) {
                    return;
                }

                sX = Math.Abs(sX);

                if (dX < fClipRect.Left) {
                    int delta = fClipRect.Left - dX;
                    sW -= delta;
                    sX += delta;
                    dX = fClipRect.Left;
                }
                if (dY < fClipRect.Top) {
                    int delta = fClipRect.Top - dY;
                    sH -= delta;
                    sY += delta;
                    dY = fClipRect.Top;
                }

                if (dX + sW > fClipRect.Right) {
                    sW -= dX + sW - fClipRect.Right - 1;
                }
                if (dY + sH > fClipRect.Bottom) {
                    sH -= dY + sH - fClipRect.Bottom - 1;
                }

                if (sW + sX > image.Width) {
                    sW = image.Width - sX;
                }
                if (sH + sY > image.Height) {
                    sH = image.Height - sY;
                }

                if (sH > 0 && sW > 0 && dX <= Width && dY <= Height && dX + image.Width >= 0 && dY + image.Height >= 0) {
                    if (sW + dX > Width) {
                        sW = Width - dX;
                    }
                    if (sH + dY > Height) {
                        sH = Height - dY;
                    }
                }

                IntPtr imTexture = ((SDL2Image)image).fTexturePtr;
                if (imTexture != IntPtr.Zero) {
                    fSrcRect = CreateRect(sX, sY, sW, sH);
                    fDstRect = CreateRect(dX, dY, sW, sH);

                    if (opacity == 255) {
                        SDL.SDL_SetTextureAlphaMod(imTexture, (byte)opacity);
                    } else {
                        SDL.SDL_SetTextureAlphaMod(imTexture, (byte)opacity);
                    }

                    SDL.SDL_RenderCopy(fRenderPtr, imTexture, ref fSrcRect, ref fDstRect);
                }
            }
        }
    }
}
