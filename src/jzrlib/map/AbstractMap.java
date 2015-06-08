/*
 *  "JZRLib", Java Roguelike games development Library.
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
package jzrlib.map;

import java.util.ArrayList;
import java.util.List;
import jzrlib.core.BaseObject;
import jzrlib.core.CreatureEntity;
import jzrlib.core.EntityList;
import jzrlib.core.LocatedEntity;
import jzrlib.core.Point;
import jzrlib.core.Rect;
import jzrlib.utils.AuxUtils;
import jzrlib.utils.Logger;
import jzrlib.utils.RefObject;

/**
 *
 * @author Serg V. Zhdanovskih
 */
public abstract class AbstractMap extends BaseObject implements IMap
{
    public static final int[] CoursesX = new int[] {  0,  1,  1,  1,  0, -1, -1, -1}; 
    public static final int[] CoursesY = new int[] { -1, -1,  0,  1,  1,  1,  0, -1};

    private final EntityList fFeatures;
    private int fHeight;
    private int fWidth;

    public AbstractMap(int width, int height)
    {
        super();
        this.fFeatures = new EntityList(this, true);
        this.resize(width, height);
    }

    @Override
    protected void dispose(boolean disposing)
    {
        if (disposing) {
            this.destroyTiles();
            this.fFeatures.dispose();
        }
        super.dispose(disposing);
    }

    protected BaseTile createTile()
    {
        return new BaseTile();
    }

    protected void createTiles()
    {
    }

    protected void destroyTiles()
    {
    }

    // <editor-fold defaultstate="collapsed" desc="IMap implementation">

    @Override
    public final void resize(int width, int height)
    {
        this.destroyTiles();
        this.fWidth = width;
        this.fHeight = height;
        this.createTiles();
    }

    @Override
    public final Rect getAreaRect()
    {
        return new Rect(0, 0, this.getWidth() - 1, this.getHeight() - 1);
    }

    @Override
    public int getHeight()
    {
        return this.fHeight;
    }

    @Override
    public int getWidth()
    {
        return this.fWidth;
    }

    @Override
    public final boolean isValid(int x, int y)
    {
        // Notice: using of getters is mandatory
        return x >= 0 && x < this.getWidth() && y >= 0 && y < this.getHeight();
    }

    @Override
    public final EntityList getFeatures()
    {
        return this.fFeatures;
    }

    @Override
    public abstract BaseTile getTile(int x, int y);

    @Override
    public final void setSeen(int x, int y)
    {
        BaseTile tile = this.getTile(x, y);
        if (tile != null) {
            tile.includeStates(TileStates.TS_SEEN, TileStates.TS_VISITED);
        }
    }

    @Override
    public boolean isBlockLOS(int x, int y)
    {
        return false;
    }

    @Override
    public boolean isBarrier(int x, int y)
    {
        return false;
    }

    @Override
    public void normalize()
    {
    }

    @Override
    public final boolean checkTile(int x, int y, int checkId, boolean fg)
    {
        BaseTile tile = this.getTile(x, y);
        if (tile == null) {
            return false;
        }

        int tid = (fg) ? tile.getForeBase() : tile.getBackBase();
        return (tid == checkId);
    }

    @Override
    public final void setTile(int x, int y, int tid, boolean fg)
    {
        BaseTile tile = this.getTile(x, y);
        if (tile != null) {
            if (fg) {
                tile.Foreground = (short) tid;
            } else {
                tile.Background = (short) tid;
            }
        }
    }

    @Override
    public final void fillArea(Rect area, int tid, boolean fg)
    {
        this.fillArea(area.Left, area.Top, area.Right, area.Bottom, tid, fg);
    }

    @Override
    public final void fillArea(int x1, int y1, int x2, int y2, int tid, boolean fg)
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
                this.setTile(xx, yy, tid, fg);
            }
        }
    }

    @Override
    public final void fillBackground(int tid)
    {
        this.fillArea(0, 0, this.getWidth() - 1, this.getHeight() - 1, tid, false);
    }

    @Override
    public final void fillForeground(int tid)
    {
        this.fillArea(0, 0, this.getWidth() - 1, this.getHeight() - 1, tid, true);
    }

    @Override
    public final void fillBorder(int x1, int y1, int x2, int y2, int tid, boolean fg)
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
            this.setTile(xx, y1, tid, fg);
            this.setTile(xx, y2, tid, fg);
        }

        for (int yy = y1; yy <= y2; yy++) {
            this.setTile(x1, yy, tid, fg);
            this.setTile(x2, yy, tid, fg);
        }
    }

    @Override
    public final void fillRadial(int aX, int aY, short boundTile, short fillTile, boolean fg)
    {
        this.setTile(aX, aY, fillTile, fg);

        for (int yy = aY - 1; yy <= aY + 1; yy++) {
            for (int xx = aX - 1; xx <= aX + 1; xx++) {
                if (xx != aX && yy != aY) {
                    BaseTile tile = this.getTile(xx, yy);
                    if (tile != null) {
                        int checkId = (fg) ? tile.Foreground : tile.Background;
                        if (checkId != fillTile && checkId != boundTile) {
                            this.fillRadial(xx, yy, boundTile, fillTile, fg);
                        }
                    }
                }
            }
        }
    }

    // </editor-fold>

    private void lineHandler(int aX, int aY, RefObject<Boolean> refContinue)
    {
        refContinue.argValue = !this.isBlockLOS(aX, aY);
    }

    public final boolean lineOfSight(int x1, int y1, int x2, int y2)
    {
        return AuxUtils.doLine(x1, y1, x2, y2, this::lineHandler, true);
    }

    public abstract Movements getTileMovements(short tileID);

    public final boolean isValidMove(int aX, int aY, Movements cms)
    {
        boolean result = false;

        try {
            BaseTile tile = this.getTile(aX, aY);
            if (tile != null) {
                int bg = tile.getBackBase();
                int fg = tile.getForeBase();

                result = this.getTileMovements((short)bg).hasIntersect(cms);

                if (result && fg != 0) {
                    result = (this.getTileMovements((short)fg).hasIntersect(cms));
                }
            }
        } catch (Exception ex) {
            Logger.write("AbstractMap.isValidMove(): " + ex.getMessage());
        }

        return result;
    }

    @Override
    public final Point searchFreeLocation()
    {
        Rect area = this.getAreaRect();
        area.inflate(-1, -1);
        return this.searchFreeLocation(area);
    }

    // FIXME: checkit & tests
    @Override
    public final Point searchFreeLocation(Rect area)
    {
        //AuxUtils.randomize();
        Movements validMovements = new Movements(Movements.mkWalk);

        int tries = 50;
        do {
            Point pt = AuxUtils.getRandomPoint(area);
            //int tx = AuxUtils.getBoundedRnd(1, this.getWidth() - 2);
            //int ty = AuxUtils.getBoundedRnd(1, this.getHeight() - 2);

            if (this.isValidMove(pt.X, pt.Y, validMovements)) {
                return pt;
            }
            tries--;
        } while (tries != 0);

        return null;
    }

    public final Point getNearestPlace(int aX, int aY, int radius, boolean withoutLive, Movements validMovements)
    {
        Point result = null;

        try {
            ArrayList<Point> valid = new ArrayList<>();

            for (int rx = aX - radius; rx <= aX + radius; rx++) {
                for (int ry = aY - radius; ry <= aY + radius; ry++) {
                    if (rx != aX || ry != aY) {
                        boolean res = this.isValidMove(rx, ry, validMovements);
                        if (res) {
                            res = (!withoutLive || this.findCreature(rx, ry) == null);
                            if (res) {
                                valid.add(new Point(rx, ry));
                            }
                        }
                    }
                }
            }

            if (valid.size() > 0) {
                int i = AuxUtils.getRandom(valid.size());
                result = valid.get(i);
            }
        } catch (Exception ex) {
            Logger.write("AbstractMap.getNearestPlace(): " + ex.getMessage());
        }

        return result;
    }

    @Override
    public CreatureEntity findCreature(int aX, int aY)
    {
        return null;
    }

    @Override
    public LocatedEntity findItem(int aX, int aY)
    {
        return null;
    }

    public final boolean checkFore(int x, int y, TileType defTile)
    {
        BaseTile tile = this.getTile(x, y);
        return (tile != null) && (tile.Foreground == this.translateTile(defTile));
    }

    public final int checkAdjacently(int x, int y, int tileId, boolean fg)
    {
        int result = 0;

        for (int yy = y - 1; yy <= y + 1; yy++) {
            for (int xx = x - 1; xx <= x + 1; xx++) {
                if ((xx != x) || (yy != y)) {
                    BaseTile mtile = this.getTile(xx, yy);
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

    @Override
    public final int checkForeAdjacently(int x, int y, TileType defTile)
    {
        int result = 0;
        short tileId = this.translateTile(defTile);

        for (int yy = y - 1; yy <= y + 1; yy++) {
            for (int xx = x - 1; xx <= x + 1; xx++) {
                if ((xx != x) || (yy != y)) {
                    BaseTile tile = this.getTile(xx, yy);
                    if (tile != null && tile.Foreground == tileId) {
                        result++;
                    }
                }
            }
        }

        return result;
    }

    public final int getBackTilesCount(int ax, int ay, short tid)
    {
        int result = 0;
        for (int yy = ay - 1; yy <= ay + 1; yy++) {
            for (int xx = ax - 1; xx <= ax + 1; xx++) {
                BaseTile tile = this.getTile(xx, yy);
                if (tile != null && tile.Background == tid) {
                    result++;
                }
            }
        }
        return result;
    }

    @Override
    public short translateTile(TileType defTile)
    {
        return (short) defTile.getValue();
    }

    private void setPathTile(int xx, int yy, int tid, boolean bg)
    {
        BaseTile tile = this.getTile(xx, yy);
        if (tile != null/* && tile.foreGround == 0*/) {
            if (bg) {
                tile.Background = (short) tid;
            } else {
                tile.Foreground = (short) tid;
            }
        }
    }

    @Override
    public final void gen_Path(int px1, int py1, int px2, int py2, 
            Rect area, int tid, boolean wide, boolean bg, ITileChangeHandler tileHandler)
    {
        gen_Path(px1, py1, px2, py2, area, tid, wide, 3, bg, tileHandler);
    }

    @Override
    public final void gen_Path(int px1, int py1, int px2, int py2, 
            Rect area, int tid, boolean wide, int wide2, boolean bg, ITileChangeHandler tileHandler)
    {
        RefObject<Boolean> refContinue = new RefObject<>(true);
        
        int prevX = -1;
        int prevY = -1;

        while (px1 != px2 || py1 != py2) {
            if (prevX != px1 || prevY != py1) {
                if (tileHandler != null) {
                    tileHandler.invoke(this, px1, py1, tid, refContinue);
                    if (!refContinue.argValue) {
                        break;
                    }
                } else {
                    setPathTile(px1, py1, tid, bg);
                }

                prevX = px1;
                prevY = py1;
            }

            int dx;
            int dy;

            if (wide && AuxUtils.getRandom(wide2) != 0) { // 3
                dx = AuxUtils.getRandom(3) - 1;
                dy = AuxUtils.getRandom(3) - 1;
            } else {
                dx = Integer.signum(px2 - px1);
                dy = Integer.signum(py2 - py1);
            }

            int num = AuxUtils.getRandom(2);
            switch (num) {
                case 0:
                    dx = 0;
                    break;
                case 1:
                    dy = 0;
                    break;
            }

            px1 = AuxUtils.middle(area.Left, px1 + dx, area.Right);
            py1 = AuxUtils.middle(area.Top, py1 + dy, area.Bottom);
        }

        if (tileHandler != null) {
            tileHandler.invoke(this, px2, py2, tid, refContinue);
        } else {
            setPathTile(px2, py2, tid, bg);
        }
    }

    public final void gen_River(AbstractMap map, int x1, int y1, int x2, int y2, Rect area)
    {
        gen_Path(x1, y1, x2, y2, area, map.translateTile(TileType.ttWater), true, true, null);
    }

    /**
     * Generating sparse spaces (for roguelike games).
     * Newsgroups: rec.games.roguelike.development
     * Subject: Cave-building algorithm
     * Original: CavesByRndGrowth.dpr
     *
     * @author Garth Dighton <gdighton@geocities.com>, 15.04.2002
     * @author Stanislav Fateev <stasulka@list.ru>, 17.12.2005
     * @author Serg V. Zhdanovskih
     */
    public final void gen_RarefySpace(Rect area, ITileChangeHandler tileChanger, int MaxBorders, int Threshold)
    {
        if (tileChanger == null) {
            return;
        }

        RefObject<Boolean> refContinue = new RefObject<>(true);

        int h = area.getHeight();
        int w = area.getWidth();
        int xx = w / 2;
        int yy = h / 2;

        List<Point> List = new ArrayList<>();

        int usedTiles = 0;

        do {
            int tx = area.Left + xx;
            int ty = area.Top + yy;
            tileChanger.invoke(this, tx, ty, null, refContinue);
            usedTiles++;

            for (int dr = 0; dr < 8; dr++) {
                int x = xx + AbstractMap.CoursesX[dr];
                int y = yy + AbstractMap.CoursesY[dr];

                if (this.isValid(area.Left + x, area.Top + y) && AuxUtils.getRandom(MaxBorders) + 1 > List.size()) {
                    List.add(new Point(x, y));
                }
            }

            if (List.size() > 0) {
                int i = AuxUtils.getRandom(List.size());
                Point pt = List.get(i);
                List.remove(i);
                xx = pt.X;
                yy = pt.Y;
            }
        } while (List.size() > 0 && usedTiles < Threshold);
    }

    public final void gen_Lake(Rect area, ITileChangeHandler tileHandler)
    {
        int thres = Math.round(area.getWidth() * area.getHeight() * 0.5f);
        gen_RarefySpace(area, tileHandler, 7, thres);
    }
    
    @Override
    public float getPathTileCost(CreatureEntity creature, int tx, int ty, BaseTile tile)
    {
        return (this.isBarrier(tx, ty) ? PathSearch.BARRIER_COST : 1.0f);
    }
}
