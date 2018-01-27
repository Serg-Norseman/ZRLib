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
using SDL2;
using ZRLib.Core;
using ZRLib.Engine;

namespace ZRLib.Engine.sdl2
{
    public sealed class SDL2System : BaseSystem
    {
        private readonly IntPtr fWinPtr;
        private SDL.SDL_Event fEvent;

        public SDL2System(IMainWindow mainWindow, int width, int height, bool fullScreen)
            : base(mainWindow, width, height, fullScreen)
        {
            fFrameDuration = 50;

            SDL.SDL_Init(SDL.SDL_INIT_VIDEO /*| SDL.SDL_INIT_EVENTS*/);
            fWinPtr = SDL.SDL_CreateWindow("Win", SDL.SDL_WINDOWPOS_CENTERED, SDL.SDL_WINDOWPOS_CENTERED, width, height, 0);

            SDL.SDL_StartTextInput();

            fScreen = new SDL2Screen(fWinPtr, fullScreen);
            fScreen.SetSize(width, height);
        }

        public override bool SysCursorVisible
        {
            set {
                base.SysCursorVisible = value;
                SDL.SDL_ShowCursor(!value ? 0 : 1);
            }
        }

        public override string Caption
        {
            set { SDL.SDL_SetWindowTitle(fWinPtr, value); }
        }

        private static ShiftStates KeyboardStateToShiftState(SDL.SDL_KeyboardEvent keyEvent)
        {
            ShiftStates result = new ShiftStates();

            var modifier = keyEvent.keysym.mod;
            if (modifier.HasFlag(SDL.SDL_Keymod.KMOD_CTRL)) {
                result.Include(ShiftStates.SsCtrl);
            }
            if (modifier.HasFlag(SDL.SDL_Keymod.KMOD_SHIFT)) {
                result.Include(ShiftStates.SsShift);
            }
            if (modifier.HasFlag(SDL.SDL_Keymod.KMOD_ALT)) {
                result.Include(ShiftStates.SsAlt);
            }

            return result;
        }

        private static ShiftStates MouseStateToShiftState(SDL.SDL_MouseMotionEvent mouseEvent)
        {
            ShiftStates result = new ShiftStates();

            if (mouseEvent.state == 1) {
                result.Include(ShiftStates.SsLeft);
            }
            if (mouseEvent.state == 2) {
                result.Include(ShiftStates.SsMiddle);
            }
            if (mouseEvent.state == 3) {
                result.Include(ShiftStates.SsRight);
            }

            return result;
        }

        protected override unsafe void ProcessEvents()
        {
            try {
                IMainWindow mainWindow = fMainWindow;

                while (SDL.SDL_PollEvent(out fEvent) == 1 && !fTerminate) {
                    SDL.SDL_UserEvent temp = fEvent.user;

                    switch ((SDL.SDL_EventType)temp.type) {
                        case SDL.SDL_EventType.SDL_WINDOWEVENT:
                            SDL.SDL_WindowEvent winEvent = fEvent.window;
                            switch (winEvent.windowEvent) {
                                case SDL.SDL_WindowEventID.SDL_WINDOWEVENT_SHOWN:
                                    mainWindow.DoActive(true);
                                    break;
                                case SDL.SDL_WindowEventID.SDL_WINDOWEVENT_HIDDEN:
                                    mainWindow.DoActive(false);
                                    break;
                            }
                            break;

                        case SDL.SDL_EventType.SDL_KEYDOWN:
                            {
                                SDL.SDL_KeyboardEvent keyEvent = fEvent.key;
                                Keys key = (Keys)keyEvent.keysym.sym;

                                KeyEventArgs eventArgs = new KeyEventArgs();
                                eventArgs.Key = key;
                                eventArgs.Shift = KeyboardStateToShiftState(keyEvent);
                                mainWindow.ProcessKeyDown(eventArgs);
                                break;
                            }

                        case SDL.SDL_EventType.SDL_KEYUP:
                            {
                                SDL.SDL_KeyboardEvent keyEvent = fEvent.key;
                                Keys key = (Keys)keyEvent.keysym.sym;

                                KeyEventArgs eventArgs = new KeyEventArgs();
                                eventArgs.Key = key;
                                eventArgs.Shift = KeyboardStateToShiftState(keyEvent);
                                mainWindow.ProcessKeyUp(eventArgs);
                                break;
                            }

                        case SDL.SDL_EventType.SDL_TEXTINPUT:
                            SDL.SDL_TextInputEvent textInputEvent = fEvent.text;
                            string txt = SDLBufferToString((byte*)textInputEvent.text);
                            if (txt != null) {
                                KeyPressEventArgs eventArg = new KeyPressEventArgs();
                                eventArg.Key = txt[0];
                                mainWindow.ProcessKeyPress(eventArg);
                            }
                            break;

                        case SDL.SDL_EventType.SDL_MOUSEMOTION:
                            {
                                SDL.SDL_MouseMotionEvent motionEvent = fEvent.motion;
                                MouseMoveEventArgs eventArgs = new MouseMoveEventArgs(motionEvent.x, motionEvent.y);
                                eventArgs.Shift = MouseStateToShiftState(motionEvent);
                                mainWindow.ProcessMouseMove(eventArgs);
                                break;
                            }

                        case SDL.SDL_EventType.SDL_MOUSEBUTTONDOWN:
                            {
                                SDL.SDL_MouseButtonEvent buttonEvent = fEvent.button;
                                int dx = buttonEvent.x;
                                int dy = buttonEvent.y;
                                ShiftStates shift = new ShiftStates();

                                if (buttonEvent.button == 4 || buttonEvent.button == 5) {
                                } else {
                                    MouseEventArgs eventArgs = new MouseEventArgs(dx, dy);
                                    eventArgs.Shift = shift;
                                    switch (buttonEvent.button) {
                                        case 1:
                                            eventArgs.Button = MouseButton.mbLeft;
                                            break;
                                        case 2:
                                            eventArgs.Button = MouseButton.mbMiddle;
                                            break;
                                        case 3:
                                            eventArgs.Button = MouseButton.mbRight;
                                            break;
                                        default:
                                            eventArgs.Button = MouseButton.mbLeft;
                                            break;
                                    }
                                    mainWindow.ProcessMouseDown(eventArgs);
                                }
                                break;
                            }

                        case SDL.SDL_EventType.SDL_MOUSEBUTTONUP:
                            {
                                SDL.SDL_MouseButtonEvent buttonEvent = fEvent.button;
                                int ux = buttonEvent.x;
                                int uy = buttonEvent.y;
                                ShiftStates shift = new ShiftStates();

                                if (buttonEvent.button == 4 || buttonEvent.button == 5) {
                                } else {
                                    MouseEventArgs eventArgs = new MouseEventArgs(ux, uy);
                                    eventArgs.Shift = shift;
                                    switch (buttonEvent.button) {
                                        case 1:
                                            eventArgs.Button = MouseButton.mbLeft;
                                            break;
                                        case 2:
                                            eventArgs.Button = MouseButton.mbMiddle;
                                            break;
                                        case 3:
                                            eventArgs.Button = MouseButton.mbRight;
                                            break;
                                        default:
                                            eventArgs.Button = MouseButton.mbLeft;
                                            break;
                                    }
                                    mainWindow.ProcessMouseUp(eventArgs);
                                }
                                break;
                            }

                        case SDL.SDL_EventType.SDL_MOUSEWHEEL:
                            SDL.SDL_MouseWheelEvent wheelEvent = fEvent.wheel;
                            int wx = wheelEvent.x;
                            int wy = wheelEvent.y;
                            MouseWheelEventArgs mwArgs = new MouseWheelEventArgs(wx, wy);
                            mwArgs.Shift = new ShiftStates();
                            mwArgs.WheelDelta = wy * -1;
                            mainWindow.ProcessMouseWheel(mwArgs);
                            break;

                        case SDL.SDL_EventType.SDL_QUIT:
                            fTerminate = true;
                            break;
                    }
                }
            } catch (Exception ex) {
                Logger.Write("SDLSystem.processEvents(): " + ex.Message);
            }
        }

        private unsafe string SDLBufferToString(byte* text, int size = 32)
        {
            byte[] sourceBytes = new byte[size];
            int length = 0;

            for (int i = 0; i < size; i++) {
                if (text[i] == 0)
                    break;

                sourceBytes[i] = text[i];
                length++;
            }

            return Encoding.UTF8.GetString(sourceBytes, 0, length);
        }
    }
}
