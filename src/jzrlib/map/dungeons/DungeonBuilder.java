/*
 *  "NorseWorld: Ragnarok", a roguelike game for PCs.
 *  Copyright (C) 2003 by Ruslan N. Garipov (aka Brigadir).
 *  Copyright (C) 2002-2008, 2014 by Serg V. Zhdanovskih (aka Alchemist).
 *
 *  This file is part of "NorseWorld: Ragnarok".
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
package jzrlib.map.dungeons;

import java.util.ArrayList;
import java.util.List;
import jzrlib.core.BaseObject;
import jzrlib.core.Directions;
import jzrlib.core.Point;
import jzrlib.core.Rect;
import jzrlib.map.AbstractMap;
import jzrlib.map.BaseTile;
import jzrlib.map.TileType;
import jzrlib.utils.AuxUtils;
import jzrlib.utils.Logger;

/**
 * Dungeons generation engine.
 * Original: rMapDungeon.pas.
 * Specifically and exclusively developed for Alchemist's "NorseWorld: Ragnarok" roguelike game.
 * @author Ruslan N. Garipov (aka Brigadir), 2003
 * @author Serg V. Zhdanovskih (aka Alchemist), 2003-2008, 2014-2015
 */
public final class DungeonBuilder extends BaseObject
{
    private static class WeightRow
    {
        public IDungeonAreaCreateProc areaCreateProc;
        public byte weight;

        public WeightRow(IDungeonAreaCreateProc areaCreateProc, int weight)
        {
            this.areaCreateProc = areaCreateProc;
            this.weight = (byte) weight;
        }
    }

    public static final int DefaultMarkRetriesLimit = 10;
    public static final boolean Debug_StepByStep = false;
    public static final boolean Debug_MultiColors = false;

    private List<DungeonArea> fAreasList;
    private boolean fBusy;
    private final AbstractMap fMap;
    private final Rect fDungeonArea;
    
    // <editor-fold defaultstate="collapsed" desc="Runtime settings">

    private WeightRow[] fAreaWeightTable;

    public byte AreaSizeLimit;
    public byte CorridorLengthBottomLimit;
    public byte CorridorWidthLimit;
    public byte CorridorWidthBottomLimit;
    public byte CylindricityRoomRadius;
    public byte DevouredAreaBottomLimit;
    public byte LinearCorridorMarksLimit;
    public byte MarkRetriesLimit;
    public byte QuadrantCorridorInRadius;
    public byte QuadrantCorridorExRadius;
    public byte RectRoomMarksLimit;
    public byte RightMarkSearchLimit;
    public byte ForcedStartupArea;

    // </editor-fold>

    public DungeonBuilder(AbstractMap map)
    {
        this(map, map.getAreaRect());
    }

    public DungeonBuilder(AbstractMap map, Rect dungeonArea)
    {
        this.fMap = map;
        
        if (dungeonArea != null) {
            this.fDungeonArea = dungeonArea;
        } else {
            this.fDungeonArea = map.getAreaRect();
        }

        this.fBusy = false;
        this.fAreasList = new ArrayList<>();
        
        this.initDungeonSettings();
    }

    @Override
    protected void dispose(boolean disposing)
    {
        if (disposing) {
            this.fAreasList.clear();
            this.fAreasList = null;

            this.fAreaWeightTable = null;
        }
        super.dispose(disposing);
    }

    public final boolean getBusy()
    {
        return this.fBusy;
    }

    public final List<DungeonArea> getAreasList()
    {
        return this.fAreasList;
    }

    public final Rect getDungeonArea()
    {
        return this.fDungeonArea;
    }

    public final int getDevouredArea()
    {
        int result = 0;

        try {
            for (DungeonArea area : this.fAreasList) {
                result += area.getDevourArea();
            }
        } catch (Exception ex) {
            Logger.write("DungeonBuilder.getDevouredArea(): " + ex.getMessage());
            result = -1;
        }

        return result;
    }

    private IDungeonAreaCreateProc getRandomAreaCreateProcByWeight()
    {
        int[] weightedIndex = new int[100];
        int stoppedAt = 0;

        for (int at = AreaType.atFirst; at <= AreaType.atLast; at++) {
            int num = stoppedAt + (int) this.fAreaWeightTable[at].weight - 1;
            for (int i = stoppedAt; i <= num; i++) {
                weightedIndex[i] = at;
            }
            stoppedAt += (int) this.fAreaWeightTable[at].weight;
        }

        int k = AuxUtils.getBoundedRnd(0, 99);
        return this.fAreaWeightTable[weightedIndex[k]].areaCreateProc;
    }

    private boolean isTimeToTerminateBuilding()
    {
        boolean result;
        try {
            result = false;
            int i = 0;
            while (i < this.fAreasList.size() && !result) {
                DungeonArea area = this.fAreasList.get(i);

                int mi = 0;
                while (mi < area.MarksList.size() && !result) {
                    DungeonMark mark = area.MarksList.get(mi);
                    result = (mark.getState() == DungeonMark.ms_Undefined);
                    mi++;
                }
                i++;
            }

            result = !result;
        } catch (Exception ex) {
            Logger.write("DungeonBuilder.isTimeToTerminateBuilding(): " + ex.getMessage());
            result = true;
        }
        return result;
    }

    private DungeonArea tryCreateArea(DungeonMark fromMark)
    {
        DungeonArea result;
        try {
            if (fromMark != null) {
                IDungeonAreaCreateProc areaCreateProc;

                if (fromMark.ForcedAreaCreateProc == null) {
                    areaCreateProc = this.getRandomAreaCreateProcByWeight();
                } else {
                    areaCreateProc = fromMark.ForcedAreaCreateProc;
                }

                result = areaCreateProc.invoke(this, fromMark);
            } else {
                result = null;
            }
        } catch (Exception ex) {
            Logger.write("DungeonBuilder.tryCreateArea(): " + ex.getMessage());
            result = null;
        }
        return result;
    }

    private void suspend()
    {
        if (DungeonBuilder.Debug_StepByStep) {
            /*javax.swing.JOptionPane.showMessageDialog(null, "Message text");
            ((TField) this.fMap).research(false, new TTileStateSet(TTileState.tsSeen, TTileState.tsVisited));
            rGlobals.nwrWin.repaint(50);*/
        }
    }

    protected final boolean isPointsToOtherArea(int ptX, int ptY, DungeonArea excludedArea)
    {
        try {
            for (DungeonArea area : this.fAreasList) {
                if (!area.equals(excludedArea) && area.isAllowedPointAsMark(ptX, ptY)) {
                    return true;
                }
            }
        } catch (Exception ex) {
            Logger.write("DungeonBuilder.isPointsToOtherArea(): " + ex.getMessage());
        }
        return false;
    }

    protected final List<DungeonArea> isAreaExistsAtPoint(int ptX, int ptY, DungeonArea excludedArea)
    {
        List<DungeonArea> result = new ArrayList<>();
        try {
            for (DungeonArea area : this.fAreasList) {
                if (!area.equals(excludedArea) && area.isOwnedPoint(ptX, ptY)) {
                    result.add(area);
                }
            }

            if (result.isEmpty()) {
                result = null;
            }
        } catch (Exception ex) {
            Logger.write("DungeonBuilder.isAreaExistsAtPoint(): " + ex.getMessage());
            result = null;
        }
        return result;
    }

    protected final boolean isFitAreaDimension(DungeonArea checkArea)
    {
        boolean result = this.fDungeonArea.isInside(checkArea.getDimensionRect());

        if (result) {
            int i = 0;
            while (i < this.fAreasList.size() & result) {
                DungeonArea area = this.fAreasList.get(i);
                if (!area.equals(checkArea)) {
                    result = (!checkArea.isIntersectWithArea(area));
                }
                i++;
            }
        }

        return result;
    }

    protected final void setTile(int ax, int ay, TileType defTile)
    {
        TileType resTile = defTile;
        switch (defTile) {
            case ttUndefinedMark:
                if (DungeonBuilder.Debug_MultiColors) {
                    resTile = TileType.ttMountain;
                } else {
                    resTile = TileType.ttDungeonWall;
                }
                break;

            case ttAreaGeneratorMark:
                if (DungeonBuilder.Debug_MultiColors) {
                    resTile = TileType.ttTree;
                } else {
                    resTile = TileType.ttDoor;
                }
                break;

            case ttRetriesExhaustMark:
                if (DungeonBuilder.Debug_MultiColors) {
                    resTile = TileType.ttMountain;
                } else {
                    resTile = TileType.ttDungeonWall;
                }
                break;

            case ttPointToOtherAreaMark:
                if (DungeonBuilder.Debug_MultiColors) {
                    resTile = TileType.ttTree;
                } else {
                    resTile = TileType.ttDoor;
                }
                break;


            case ttLinearCorridorWall:
                if (!DungeonBuilder.Debug_MultiColors) {
                    resTile = TileType.ttDungeonWall;
                }
                break;

            case ttRectRoomWall:
                if (!DungeonBuilder.Debug_MultiColors) {
                    resTile = TileType.ttDungeonWall;
                }
                break;

            case ttCylindricityRoomWall:
                if (!DungeonBuilder.Debug_MultiColors) {
                    resTile = TileType.ttDungeonWall;
                }
                break;

            case ttUndefined:
            case ttDungeonWall:
            default:
                break;
        }

        BaseTile tile = this.fMap.getTile(ax, ay);
        if (tile != null) {
            tile.Foreground = this.fMap.translateTile(resTile);
        } else {
            //Logger.write("DungeonBuilder.setTile(): tile is invalid");
        }
    }

    private void processArea(DungeonArea currentArea)
    {
        try {
            this.fAreasList.add(currentArea);
            currentArea.generateMarksList();
            currentArea.flushToMap();

            if (!this.isTimeToTerminateBuilding()) {
                for (DungeonMark mark : currentArea.MarksList) {
                    this.processMark(mark);
                }
            }

            currentArea.flushToMap();

            if (DungeonBuilder.Debug_StepByStep) {
                this.suspend();
            }
        } catch (Exception ex) {
            Logger.write("DungeonBuilder.processArea(): " + ex.getMessage());
            throw ex;
        }
    }

    private void processMark(DungeonMark mark)
    {
        try {
            DungeonArea newArea = this.tryCreateArea(mark);
            if (newArea != null) {
                if (newArea.tryApplyThisArea()) {
                    this.processArea(newArea);
                } else {
                    newArea.dispose();
                }
            }
        } catch (Exception ex) {
            Logger.write("DungeonBuilder.processMark(): " + ex.getMessage());
            throw ex;
        }
    }

    public final boolean build()
    {
        return this.build(null);
    }

    public final boolean build(Point startup)
    {
        try {
            try {
                if (this.fBusy) {
                    return false;
                }

                this.fBusy = true;
                if (!this.isWeightTableDefinedWell()) {
                    throw new RuntimeException("Areas Weight Table Defined Incorrect!\r\nPlease Check TDungeonSettings.areaWeight property.");
                }

                int mapArea = this.fDungeonArea.getSquare();
                if (startup == null) {
                    startup = this.fDungeonArea.getCenter();
                }

                boolean breakFlag;
                do {
                    // cleanup the map
                    this.fMap.fillArea(this.fDungeonArea, (int) this.fMap.translateTile(TileType.ttCaveFloor), false);
                    this.fMap.fillArea(this.fDungeonArea, (int) this.fMap.translateTile(TileType.ttRock), true);
                    this.fAreasList.clear();

                    // create the first mark
                    DungeonMark startupMark = new DungeonMark(null, startup.X, startup.Y, (AuxUtils.getBoundedRnd(Directions.dtNorth, Directions.dtEast)));
                    startupMark.ForcedAreaCreateProc = this.fAreaWeightTable[this.ForcedStartupArea].areaCreateProc;
                    this.processMark(startupMark);

                    // check devoured square
                    float devouredArea = this.getDevouredArea();
                    breakFlag = (Math.round((devouredArea / mapArea) * 100.0f) >= this.DevouredAreaBottomLimit);
                } while (!breakFlag);

                return true;
            } finally {
                this.fBusy = false;
            }
        } catch (Exception ex) {
            Logger.write("DungeonBuilder.build(): " + ex.getMessage());
            throw ex;
        }
    }

    public final void setAreaWeight(int areaType, int value)
    {
        this.fAreaWeightTable[areaType].weight = (byte) value;
    }

    private boolean isWeightTableDefinedWell()
    {
        int summary = 0;

        int index = 0;
        while (index < this.fAreaWeightTable.length && summary <= 100) {
            summary += this.fAreaWeightTable[index].weight;
            index += 1;
        }

        return summary == 100;
    }

    private void initDungeonSettings()
    {
        this.fAreaWeightTable = new WeightRow[19];
        this.fAreaWeightTable[0] = new WeightRow(RectangularRoom::createArea, 54);
        this.fAreaWeightTable[1] = new WeightRow(LinearCorridor::createArea, 10);
        this.fAreaWeightTable[2] = new WeightRow(CylindricityRoom::createArea, 6);
        this.fAreaWeightTable[3] = new WeightRow(QuadrantCorridor::createArea, 3);
        this.fAreaWeightTable[4] = new WeightRow(GenevaWheelRoom::createArea, 9);
        this.fAreaWeightTable[5] = new WeightRow(QuakeIIArena::createArea, 1);
        this.fAreaWeightTable[6] = new WeightRow(TempleRoom::createArea, 4);
        this.fAreaWeightTable[7] = new WeightRow(MonasticCellsRoom::createArea, 1);
        this.fAreaWeightTable[8] = new WeightRow(CryptRoom::createArea, 1);
        this.fAreaWeightTable[9] = new WeightRow(RoseRoom::createArea, 1);
        this.fAreaWeightTable[10] = new WeightRow(CrossroadRoom::createArea, 1);
        this.fAreaWeightTable[11] = new WeightRow(StarRoom::createArea, 1);
        this.fAreaWeightTable[12] = new WeightRow(FaithRoom::createArea, 1);
        this.fAreaWeightTable[13] = new WeightRow(SpiderRoom::createArea, 1);
        this.fAreaWeightTable[14] = new WeightRow(Alt1Room::createArea, 1);
        this.fAreaWeightTable[15] = new WeightRow(Alt2Room::createArea, 1);
        this.fAreaWeightTable[16] = new WeightRow(Alt3Room::createArea, 1);
        this.fAreaWeightTable[17] = new WeightRow(Alt4Room::createArea, 1);
        this.fAreaWeightTable[18] = new WeightRow(Alt5Room::createArea, 1);

        this.AreaSizeLimit = 10;
        this.CorridorWidthLimit = 5;
        this.CorridorWidthBottomLimit = 3;
        this.CorridorLengthBottomLimit = 3;
        this.CylindricityRoomRadius = 5;
        this.QuadrantCorridorInRadius = 5;
        this.QuadrantCorridorExRadius = (byte) (this.QuadrantCorridorInRadius + 3);
        this.MarkRetriesLimit = 10;
        this.RightMarkSearchLimit = 5;
        this.RectRoomMarksLimit = 5;
        this.LinearCorridorMarksLimit = 3;
        this.DevouredAreaBottomLimit = 70;
        this.ForcedStartupArea = AreaType.atCylindricityRoom;
    }
}
