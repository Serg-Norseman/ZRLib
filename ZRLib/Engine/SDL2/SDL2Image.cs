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
using System.Runtime.InteropServices;
using BSLib;
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
            fType = "TGA";
        }

        public SDL2Image(BaseScreen screen, string fileName, int transColor)
            : base(screen, fileName, transColor)
        {
            fType = "TGA";
        }

        protected override void Done()
        {
            if (fSurfacePtr != IntPtr.Zero) {
                SDL.SDL_FreeSurface(fSurfacePtr);
                fSurfacePtr = IntPtr.Zero;
            }
            if (fTexturePtr != IntPtr.Zero) {
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
                fSurfacePtr = SDL_image.IMG_LoadTyped_RW(rw, 0, fType);
                bufferHandle.Free();

                if (fSurfacePtr != IntPtr.Zero) {
                    if (!PaletteMode) {
                        IntPtr fmt = ((SDL2Screen)fScreen).fFormatPtr;
                        IntPtr srf = SDL.SDL_ConvertSurface(fSurfacePtr, fmt, 0);
                        SDL.SDL_FreeSurface(fSurfacePtr);
                        fSurfacePtr = srf;
                    }

                    SDL.SDL_Surface srfStr = (SDL.SDL_Surface)Marshal.PtrToStructure(fSurfacePtr, typeof(SDL.SDL_Surface));
                    Height = (short)srfStr.h;
                    Width = (short)srfStr.w;
                    TransColor = transColor;

                    if (transColor != Colors.None) {
                        uint tc = SDL2Screen.ConvertColor(srfStr.format, transColor);
                        SDL.SDL_SetColorKey(fSurfacePtr, 4096, tc);
                    }

                    fTexturePtr = SDL.SDL_CreateTextureFromSurface(((SDL2Screen)fScreen).fRenderPtr, fSurfacePtr);
                }
            } catch (Exception ex) {
                Logger.Write("SDLImage.loadFromStream(): " + ex.Message);
                fTexturePtr = IntPtr.Zero;
            }
        }

        public override void SetTransDefault()
        {
        }

        public override void ReplaceColor(int index, int replacement)
        {
            // used only for fonts rendering, don't change!
            if (fTexturePtr != IntPtr.Zero) {
                byte r, g, b, a;
                GfxHelper.DecomposeARGB(replacement, out a, out r, out g, out b);
                SDL.SDL_SetTextureColorMod(fTexturePtr, r, g, b);
            }
        }
    }
}
