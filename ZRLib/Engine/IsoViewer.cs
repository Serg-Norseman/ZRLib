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
using BSLib;
using ZRLib.Map;

namespace ZRLib.Engine
{
    public sealed class IsoViewer : MapViewer
    {
        private int fTH;
        private int fTHd2;
        private int fTW;
        private int fTWd2;

        public IsoViewer(AbstractMap map)
            : base(map)
        {
        }

        public override int TileHeight
        {
            set {
                base.TileHeight = value;
                fTH = value;
    
                if (MathHelper.IsOdd(fTH)) {
                    fTH--;
                }
    
                fTHd2 = (int)(fTH >> 1);
            }
        }

        public override int TileWidth
        {
            set {
                base.TileWidth = value;
                fTW = value;
                fTWd2 = (int)(fTW >> 1);
            }
        }

        public override void BufferPaint(BaseScreen screen, int destX, int destY)
        {
            screen.InitClip(ExtRect.Create(destX, destY, destX + Width, destY + Height));

            if (OnBeforePaint != null) {
                OnBeforePaint(this, screen);
            }

            screen.FillRect(ExtRect.Create(destX, destY, destX + Width, destY + Height), Colors.Black);

            if (OnTilePaint != null) {
                int isoWidth = Map.Width * base.TileHeight + Map.Height * base.TileHeight;
                int isoHeight = Map.Width * fTHd2 + Map.Height * fTHd2;
                if (isoWidth < Width) {
                    OffsetX = (int)((Width - isoWidth) >> 1) + (Map.Height - 1) * base.TileHeight;
                }
                if (isoHeight < Height) {
                    OffsetY = (int)(Height - isoHeight) >> 1;
                }

                ExtPoint centerTile = TileByMouse(fWidth >> 1, fHeight >> 1);
                int dx = fWidth / fTileWidth;
                int dy = fHeight / fTileHeight;
                int d = Math.Max(dx, dy) + 1;

                for (int y = centerTile.Y - d; y <= centerTile.Y + d; y++) {
                    for (int x = centerTile.X - d; x <= centerTile.X + d; x++) {
                        BaseTile tile = Map.GetTile(x, y);
                        if (tile != null) {
                            ExtPoint scrPt = TileCoords(x, y);
                            int ax = scrPt.X;
                            int ay = scrPt.Y;

                            if (ax > -base.TileWidth && ax < Width && ay > -base.TileHeight && ay < Height) {
                                ExtRect rt = ExtRect.CreateBounds(ax + destX, ay + destY, base.TileWidth, base.TileHeight);
                                OnTilePaint(this, x, y, tile, rt, screen);
                            }
                        }
                    }
                }

                if (OnAfterPaint != null) {
                    OnAfterPaint(this, screen);
                }

                screen.DoneClip();
            }
        }

        public override void CenterByTile(int tileX, int tileY)
        {
            ExtPoint scrPt = TileCoords(tileX, tileY);
            OffsetX = (Width >> 1) - (scrPt.X - OffsetX) - fTH;
            OffsetY = (Height >> 1) - (scrPt.Y - OffsetY) - fTHd2;
        }

        public override ExtPoint TileByMouse(int mX, int mY)
        {
            int xo = mX - (OffsetX + fTH);
            int yo = mY - (OffsetY + fTHd2);
            int xx = (int)Math.Round((yo + xo / 2.0f) / fTH);
            int yy = (int)Math.Round((yo - xo / 2.0f) / fTH);
            return new ExtPoint(xx, yy);
        }

        public override ExtPoint TileCoords(int x, int y)
        {
            return new ExtPoint(OffsetX + (x - y) * fTH, OffsetY + (x + y) * fTHd2);
        }
    }
}
