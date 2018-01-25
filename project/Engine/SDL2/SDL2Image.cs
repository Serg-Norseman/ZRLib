/*
 *  "NorseWorld: Ragnarok", a roguelike game for PCs.
 *  Copyright (C) 2002-2008, 2014 by Serg V. Zhdanovskih (aka Alchemist).
 *
 *  this file is part of "NorseWorld: Ragnarok".
 *
 *  this program is free software: you can redistribute it and/or modify
 *  it under the terms of the GNU General Public License as published by
 *  the Free Software Foundation, either version 3 of the License, or
 *  (at your option) any later version.
 *
 *  this program is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *  GNU General Public License for more details.
 *
 *  You should have received a copy of the GNU General Public License
 *  along with this program.  If not, see <http://www.gnu.org/licenses/>.
 */

using System;
using System.IO;
using System.Runtime.InteropServices;
using SDL2;
using ZRLib.Core;
using ZRLib.Engine;

namespace ZRLib.Engine.sdl2
{
    public sealed class SDL2Image : BaseImage
    {
        private IntPtr fSurfacePtr;
        internal IntPtr fTexturePtr;

        public SDL2Image(BaseScreen screen)
            : base(screen)
        {
        }

        public SDL2Image(BaseScreen screen, string fileName, int transColor)
            : base(screen, fileName, transColor)
        {
        }

        protected override void Done()
        {
            if (fSurfacePtr != null) {
                SDL.SDL_FreeSurface(fSurfacePtr);
                fSurfacePtr = IntPtr.Zero;
            }
            if (fTexturePtr != null) {
                SDL.SDL_DestroyTexture(fTexturePtr);
                fTexturePtr = IntPtr.Zero;
            }
            base.Done();
        }

        public override void LoadFromStream(Stream stream, int transColor)
        {
            try {
                int num = (int)(stream.Length - stream.Position);
                byte[] buffer;
                using (var rd = new BinaryReader(stream)) {
                    buffer = rd.ReadBytes(num);
                }

                GCHandle bufferHandle = GCHandle.Alloc(buffer, GCHandleType.Pinned);
                IntPtr rw = SDL.SDL_RWFromMem(bufferHandle.AddrOfPinnedObject(), num);
                fSurfacePtr = SDL_image.IMG_LoadTyped_RW(rw, 0, "TGA");
                bufferHandle.Free();

                if (fSurfacePtr != null) {
                    SDL.SDL_Surface srfStr = (SDL.SDL_Surface)Marshal.PtrToStructure(fSurfacePtr, typeof(SDL.SDL_Surface));
                    Height = (short)srfStr.h;
                    Width = (short)srfStr.w;
                    TransColor = transColor;

                    if (!PaletteMode) {
                        IntPtr fmt = ((SDL2Screen)fScreen).fFormatPtr;
                        IntPtr srf = SDL.SDL_ConvertSurface(fSurfacePtr, fmt, 0);
                        SDL.SDL_FreeSurface(fSurfacePtr);
                        fSurfacePtr = srf;
                    }

                    if (transColor != BaseScreen.clNone) {
                        var format = ((SDL.SDL_Surface)Marshal.PtrToStructure(fSurfacePtr, typeof(SDL.SDL_Surface))).format;
                        uint tc = SDL2Screen.ConvertColor(format, transColor);
                        SDL.SDL_SetColorKey(fSurfacePtr, 4096, tc);
                    }

                    fTexturePtr = SDL.SDL_CreateTextureFromSurface(((SDL2Screen)fScreen).fRenderPtr, fSurfacePtr);
                }
            } catch (Exception ex) {
                Logger.Write("SDLImage.loadFromStream(): " + ex.Message);
            }
        }

        public override void SetTransDefault()
        {
        }

        public override void ReplaceColor(int index, int replacement)
        {
            // used only for fonts rendering, don't change!
            if (fTexturePtr != null) {
                byte r = (byte)(replacement & 0xff);
                byte g = (byte)((replacement >> 8) & 0xff);
                byte b = (byte)((replacement >> 16) & 0xff);
                SDL.SDL_SetTextureColorMod(fTexturePtr, r, g, b);
            }
        }
    }
}
