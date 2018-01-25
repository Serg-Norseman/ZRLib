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

using BSLib;
using ZRLib.Map;

namespace ZRLib.Engine
{
    public delegate void IBeforePaintEvent(object sender, BaseScreen screen);
    public delegate void IAfterPaintEvent(object sender, BaseScreen screen);
    public delegate void ITilePaintEvent(object sender, int x, int y, BaseTile tile, ExtRect r, BaseScreen screen);

    public abstract class MapViewer
    {
        protected ExtRect fClientRect;
        protected int fHeight;
        protected int fTileHeight;
        protected int fTileWidth;
        protected int fWidth;

        public ExtPoint CurrentTile;
        public AbstractMap Map;
        public int OffsetX;
        public int OffsetY;
        public bool ShowCursor;
        public bool ShowGrid;

        public IAfterPaintEvent OnAfterPaint;
        public IBeforePaintEvent OnBeforePaint;
        public ITilePaintEvent OnTilePaint;

        public ExtRect ClientRect
        {
            get { return fClientRect; }
        }

        public int Height
        {
            get { return fHeight; }
        }

        public virtual int TileHeight
        {
            get { return fTileHeight; }
            set { fTileHeight = value; }
        }

        public virtual int TileWidth
        {
            get { return fTileWidth; }
            set { fTileWidth = value; }
        }

        public int Width
        {
            get { return fWidth; }
        }


        protected bool RectIsVisible(ExtRect rect)
        {
            bool result = false;
            if (fClientRect.Contains(rect.Left, rect.Top) || fClientRect.Contains(rect.Left, rect.Bottom) || fClientRect.Contains(rect.Right, rect.Top) || fClientRect.Contains(rect.Right, rect.Bottom)) {
                result = true;
            }
            return result;
        }

        protected MapViewer(AbstractMap map)
        {
            Map = map;
            OffsetX = 0;
            OffsetY = 0;
        }

        public abstract void BufferPaint(BaseScreen screen, int destX, int destY);

        public abstract void CenterByTile(int tileX, int tileY);

        public abstract ExtPoint TileByMouse(int mX, int mY);

        public abstract ExtPoint TileCoords(int x, int y);

        public void BufferResize(ExtRect clientRect)
        {
            fClientRect = clientRect;
            fWidth = clientRect.Right - clientRect.Left;
            fHeight = clientRect.Bottom - clientRect.Top;
        }
    }
}
