/*
 *  "ZRLib", Roguelike games development Library.
 *  Copyright (C) 2015, 2020 by Serg V. Zhdanovskih.
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
using System.Collections.Generic;
using BSLib;
using ZRLib.Core;

namespace ZRLib.Map
{
    public delegate bool ITileTransformer(int x, int y, BaseTile tile);

    public abstract class AbstractMap : BaseObject, IMap
    {
        public static readonly int[] CoursesX = new int[] {
            0,
            1,
            1,
            1,
            0,
            -1,
            -1,
            -1
        };

        public static readonly int[] CoursesY = new int[] {
            -1,
            -1,
            0,
            1,
            1,
            1,
            0,
            -1
        };

        private readonly EntityList<GameEntity> fFeatures;
        private int fHeight;
        private int fWidth;

        public ExtRect AreaRect
        {
            get {
                return ExtRect.Create(0, 0, Width - 1, Height - 1);
            }
        }

        public int Height
        {
            get { return fHeight; }
        }

        public int Width
        {
            get { return fWidth; }
        }

        public EntityList<GameEntity> Features
        {
            get {
                return fFeatures;
            }
        }


        protected AbstractMap(int width, int height)
            : base()
        {
            fFeatures = new EntityList<GameEntity>(this);
            Resize(width, height);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) {
                DestroyTiles();
                fFeatures.Dispose();
            }
            base.Dispose(disposing);
        }

        protected virtual BaseTile CreateTile()
        {
            return new BaseTile();
        }

        protected virtual void CreateTiles()
        {
        }

        protected virtual void DestroyTiles()
        {
        }

        public virtual void Resize(int width, int height)
        {
            DestroyTiles();
            fWidth = width;
            fHeight = height;
            CreateTiles();
        }

        public abstract void SetMetaTile(int x, int y, TileType tile);

        public abstract void FillMetaBorder(int x1, int y1, int x2, int y2, TileType tile);

        public virtual bool IsValid(int x, int y)
        {
            // Attention: using of getters is mandatory!
            return x >= 0 && x < Width && y >= 0 && y < Height;
        }

        public abstract BaseTile GetTile(int x, int y);

        public virtual void SetSeen(int x, int y)
        {
            BaseTile tile = GetTile(x, y);
            if (tile != null) {
                tile.IncludeStates(BaseTile.TS_SEEN, BaseTile.TS_VISITED);
            }
        }

        public virtual bool IsBlockLOS(int x, int y)
        {
            return false;
        }

        public virtual bool IsBarrier(int x, int y)
        {
            return false;
        }

        public virtual void Normalize()
        {
        }

        public virtual bool CheckTile(int x, int y, int checkId, bool fg)
        {
            BaseTile tile = GetTile(x, y);
            if (tile == null) {
                return false;
            }

            int tid = (fg) ? tile.ForeBase : tile.BackBase;
            return (tid == checkId);
        }

        public virtual void SetTile(int x, int y, ushort tid, bool fg)
        {
            BaseTile tile = GetTile(x, y);
            if (tile != null) {
                if (fg) {
                    tile.Foreground = tid;
                } else {
                    tile.Background = tid;
                }
            }
        }

        public virtual void FillArea(ExtRect area, ushort tid, bool fg)
        {
            FillArea(area.Left, area.Top, area.Right, area.Bottom, tid, fg);
        }

        public virtual void FillArea(int x1, int y1, int x2, int y2, ushort tid, bool fg)
        {
            if (x1 > x2) {
                int t = x1;
                x1 = x2;
                x2 = t;
            }
            if (y1 > y2) {
                int t = y1;
                y1 = y2;
                y2 = t;
            }

            for (int yy = y1; yy <= y2; yy++) {
                for (int xx = x1; xx <= x2; xx++) {
                    SetTile(xx, yy, tid, fg);
                }
            }
        }

        public virtual void FillBackground(ushort tid)
        {
            FillArea(0, 0, Width - 1, Height - 1, tid, false);
        }

        public virtual void FillForeground(ushort tid)
        {
            FillArea(0, 0, Width - 1, Height - 1, tid, true);
        }

        public virtual void FillBorder(int x1, int y1, int x2, int y2, ushort tid, bool fg)
        {
            if (x1 > x2) {
                int t = x1;
                x1 = x2;
                x2 = t;
            }
            if (y1 > y2) {
                x1 = y2;
                y2 = y1;
            }

            for (int xx = x1; xx <= x2; xx++) {
                SetTile(xx, y1, tid, fg);
                SetTile(xx, y2, tid, fg);
            }

            for (int yy = y1; yy <= y2; yy++) {
                SetTile(x1, yy, tid, fg);
                SetTile(x2, yy, tid, fg);
            }
        }

        public virtual void DrawHLine(int x1, int x2, int y, ushort tid, bool fg)
        {
            if (x1 > x2) {
                int t = x1;
                x1 = x2;
                x2 = t;
            }
            for (int xx = x1; xx <= x2; xx++) {
                SetTile(xx, y, tid, fg);
            }
        }

        public virtual void DrawVLine(int y1, int y2, int x, ushort tid, bool fg)
        {
            if (y1 > y2) {
                int t = y1;
                y1 = y2;
                y2 = t;
            }
            for (int yy = y1; yy <= y2; yy++) {
                SetTile(x, yy, tid, fg);
            }
        }

        public virtual void FillRadial(int aX, int aY, ushort boundTile, ushort fillTile, bool fg)
        {
            BaseTile tile = GetTile(aX, aY);
            if (tile == null) return;
            int checkId = (fg) ? tile.Foreground : tile.Background;
            if (checkId == boundTile) return;

            if (fg) {
                tile.Foreground = fillTile;
            } else {
                tile.Background = fillTile;
            }

            for (int yy = aY - 1; yy <= aY + 1; yy++) {
                for (int xx = aX - 1; xx <= aX + 1; xx++) {
                    if (xx != aX || yy != aY) {
                        FillRadial(xx, yy, boundTile, fillTile, fg);
                    }
                }
            }
        }

        private void LineHandler(int aX, int aY, ref bool refContinue)
        {
            refContinue = !IsBlockLOS(aX, aY);
        }

        public bool LineOfSight(int x1, int y1, int x2, int y2)
        {
            return AuxUtils.DoLine(x1, y1, x2, y2, LineHandler, true);
        }

        public abstract Movements GetTileMovements(ushort tileID);

        public bool IsValidMove(int aX, int aY, Movements cms)
        {
            bool result = false;

            try {
                BaseTile tile = GetTile(aX, aY);
                if (tile != null) {
                    ushort bg = tile.BackBase;
                    ushort fg = tile.ForeBase;

                    result = GetTileMovements(bg).HasIntersect(cms);

                    if (result && fg != 0) {
                        result = (GetTileMovements(fg).HasIntersect(cms));
                    }
                }
            } catch (Exception ex) {
                Logger.Write("AbstractMap.isValidMove(): " + ex.Message);
            }

            return result;
        }

        public virtual ExtPoint SearchFreeLocation()
        {
            ExtRect area = AreaRect;
            area.Inflate(-1, -1);
            return SearchFreeLocation(area);
        }

        public virtual ExtPoint SearchFreeLocation(ExtRect area, int tries = 50)
        {
            Movements validMovements = new Movements(Movements.mkWalk);

            do {
                ExtPoint pt = RandomHelper.GetRandomPoint(area);
                if (IsValidMove(pt.X, pt.Y, validMovements)) {
                    return pt;
                }
                tries--;
            } while (tries != 0);

            return ExtPoint.Empty;
        }

        public ExtPoint GetNearestPlace(int aX, int aY, int radius, bool withoutLive, Movements validMovements)
        {
            ExtPoint result = ExtPoint.Empty;

            try {
                List<ExtPoint> valid = new List<ExtPoint>();

                for (int rx = aX - radius; rx <= aX + radius; rx++) {
                    for (int ry = aY - radius; ry <= aY + radius; ry++) {
                        if (rx != aX || ry != aY) {
                            bool res = IsValidMove(rx, ry, validMovements);
                            if (res) {
                                res = (!withoutLive || FindCreature(rx, ry) == null);
                                if (res) {
                                    valid.Add(new ExtPoint(rx, ry));
                                }
                            }
                        }
                    }
                }

                if (valid.Count > 0) {
                    int i = RandomHelper.GetRandom(valid.Count);
                    result = valid[i];
                }
            } catch (Exception ex) {
                Logger.Write("AbstractMap.getNearestPlace(): " + ex.Message);
            }

            return result;
        }

        public virtual CreatureEntity FindCreature(int aX, int aY)
        {
            return null;
        }

        public virtual LocatedEntity FindItem(int aX, int aY)
        {
            return null;
        }

        public bool CheckFore(int x, int y, TileType defTile)
        {
            BaseTile tile = GetTile(x, y);
            return (tile != null) && (tile.Foreground == TranslateTile(defTile));
        }

        public int CheckAdjacently(int x, int y, int tileId, bool fg)
        {
            int result = 0;

            for (int yy = y - 1; yy <= y + 1; yy++) {
                for (int xx = x - 1; xx <= x + 1; xx++) {
                    if ((xx != x) || (yy != y)) {
                        BaseTile mtile = GetTile(xx, yy);
                        if (mtile != null) {
                            int tid = (fg) ? mtile.Foreground : mtile.Background;

                            if (tid == tileId) {
                                result++;
                            }
                        }
                    }
                }
            }

            return result;
        }

        public virtual int CheckForeAdjacently(int x, int y, TileType defTile)
        {
            int result = 0;
            ushort tileId = TranslateTile(defTile);

            for (int yy = y - 1; yy <= y + 1; yy++) {
                for (int xx = x - 1; xx <= x + 1; xx++) {
                    if ((xx != x) || (yy != y)) {
                        BaseTile tile = GetTile(xx, yy);
                        if (tile != null && tile.Foreground == tileId) {
                            result++;
                        }
                    }
                }
            }

            return result;
        }

        public int GetBackTilesCount(int ax, int ay, short tid)
        {
            int result = 0;
            for (int yy = ay - 1; yy <= ay + 1; yy++) {
                for (int xx = ax - 1; xx <= ax + 1; xx++) {
                    BaseTile tile = GetTile(xx, yy);
                    if (tile != null && tile.Background == tid) {
                        result++;
                    }
                }
            }
            return result;
        }

        public virtual ushort TranslateTile(TileType defTile)
        {
            return (ushort)defTile;
        }

        private void SetPathTile(int xx, int yy, ushort tid, bool bg)
        {
            BaseTile tile = GetTile(xx, yy);
            if (tile != null) { // && tile.foreGround == 0
                if (bg) {
                    tile.Background = tid;
                } else {
                    tile.Foreground = tid;
                }
            }
        }

        public virtual void Gen_Path(int px1, int py1, int px2, int py2, ExtRect area, ushort tid, bool wide, bool bg, ITileChangeHandler tileHandler)
        {
            Gen_Path(px1, py1, px2, py2, area, tid, wide, 3, bg, tileHandler);
        }

        public virtual void Gen_Path(int px1, int py1, int px2, int py2, ExtRect area, ushort tid, bool wide, int wide2, bool bg, ITileChangeHandler tileHandler)
        {
            bool refContinue = true;

            int prevX = -1;
            int prevY = -1;

            while (px1 != px2 || py1 != py2) {
                if (prevX != px1 || prevY != py1) {
                    if (tileHandler != null) {
                        tileHandler(this, px1, py1, tid, ref refContinue);
                        if (!refContinue) {
                            break;
                        }
                    } else {
                        SetPathTile(px1, py1, tid, bg);
                    }

                    prevX = px1;
                    prevY = py1;
                }

                int dx;
                int dy;

                if (wide && RandomHelper.GetRandom(wide2) != 0) { // 3
                    dx = RandomHelper.GetRandom(3) - 1;
                    dy = RandomHelper.GetRandom(3) - 1;
                } else {
                    dx = Math.Sign(px2 - px1);
                    dy = Math.Sign(py2 - py1);
                }

                int num = RandomHelper.GetRandom(2);
                switch (num) {
                    case 0:
                        dx = 0;
                        break;
                    case 1:
                        dy = 0;
                        break;
                }

                px1 = AuxUtils.Middle(area.Left, px1 + dx, area.Right);
                py1 = AuxUtils.Middle(area.Top, py1 + dy, area.Bottom);
            }

            if (tileHandler != null) {
                tileHandler(this, px2, py2, tid, ref refContinue);
            } else {
                SetPathTile(px2, py2, tid, bg);
            }
        }

        public void Gen_River(AbstractMap map, int x1, int y1, int x2, int y2, ExtRect area)
        {
            Gen_Path(x1, y1, x2, y2, area, map.TranslateTile(TileType.ttWater), true, true, null);
        }

        /// <summary>
        /// Generating sparse spaces (for roguelike games).
        /// Newsgroups: rec.games.roguelike.development
        /// Subject: Cave-building algorithm
        /// Original: CavesByRndGrowth.dpr
        /// 
        /// @author Garth Dighton <gdighton@geocities.com>, 15.04.2002
        /// @author Stanislav Fateev <stasulka@list.ru>, 17.12.2005
        /// @author Serg V. Zhdanovskih
        /// </summary>
        public void Gen_RarefySpace(ExtRect area, ITileChangeHandler tileChanger, int MaxBorders, int Threshold)
        {
            if (tileChanger == null) {
                return;
            }

            bool refContinue = true;

            int h = area.GetHeight();
            int w = area.GetWidth();
            int xx = w / 2;
            int yy = h / 2;

            IList<ExtPoint> List = new List<ExtPoint>();

            int usedTiles = 0;

            do {
                int tx = area.Left + xx;
                int ty = area.Top + yy;
                tileChanger(this, tx, ty, null, ref refContinue);
                usedTiles++;

                for (int dr = 0; dr < 8; dr++) {
                    int x = xx + AbstractMap.CoursesX[dr];
                    int y = yy + AbstractMap.CoursesY[dr];

                    if (IsValid(area.Left + x, area.Top + y) && RandomHelper.GetRandom(MaxBorders) + 1 > List.Count) {
                        List.Add(new ExtPoint(x, y));
                    }
                }

                if (List.Count > 0) {
                    int i = RandomHelper.GetRandom(List.Count);
                    ExtPoint pt = List[i];
                    List.RemoveAt(i);
                    xx = pt.X;
                    yy = pt.Y;
                }
            } while (List.Count > 0 && usedTiles < Threshold);
        }

        public void Gen_Lake(ExtRect area, ITileChangeHandler tileHandler)
        {
            int thres = (int)Math.Round(area.GetWidth() * area.GetHeight() * 0.5f);
            Gen_RarefySpace(area, tileHandler, 7, thres);
        }

        public virtual float GetPathTileCost(CreatureEntity creature, int tx, int ty, BaseTile tile)
        {
            return (IsBarrier(tx, ty) ? PathSearch.BARRIER_COST : 1.0f);
        }

        public bool ForEachTile(int left, int top, int right, int bottom, ITileTransformer transformer)
        {
            if (transformer == null) {
                return false;
            }

            for (int y = top; y <= bottom; y++) {
                for (int x = left; x <= right; x++) {
                    if (x < 0 || y < 0 || x >= fWidth || y >= fHeight) {
                        continue;
                    }

                    BaseTile tile = GetTile(x, y);
                    bool res = transformer(x, y, tile);
                    if (!res) return res;
                }
            }

            return true;
        }
    }
}
