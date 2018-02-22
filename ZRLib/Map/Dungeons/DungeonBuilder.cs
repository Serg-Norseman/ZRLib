/*
 *  "NorseWorld: Ragnarok", a roguelike game for PCs.
 *  Copyright (C) 2003 by Ruslan N. Garipov (aka Brigadir).
 *  Copyright (C) 2002-2008, 2014 by Serg V. Zhdanovskih.
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

using System;
using System.Collections.Generic;
using BSLib;
using ZRLib.Core;

namespace ZRLib.Map.Dungeons
{
    /// <summary>
    /// Dungeons generation engine.
    /// Original: rMapDungeon.pas.
    /// Specifically and exclusively developed for Alchemist's "NorseWorld: Ragnarok" roguelike game.
    /// @author Ruslan N. Garipov (aka Brigadir), 2003
    /// @author Serg V. Zhdanovskih, 2003-2008, 2014-2015
    /// </summary>
    public sealed class DungeonBuilder : BaseObject
    {
        private class WeightRow
        {
            public IDungeonAreaCreateProc AreaCreateProc;
            public byte Weight;

            public WeightRow(IDungeonAreaCreateProc areaCreateProc, int weight)
            {
                AreaCreateProc = areaCreateProc;
                Weight = (byte)weight;
            }
        }

        public const int DefaultMarkRetriesLimit = 10;
        public const bool Debug_StepByStep = false;
        public const bool Debug_MultiColors = false;

        private IList<DungeonArea> fAreasList;
        private bool fBusy;
        private readonly AbstractMap fMap;
        private readonly ExtRect fDungeonArea;

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



        public DungeonBuilder(AbstractMap map)
            : this(map, map.AreaRect)
        {
        }

        public DungeonBuilder(AbstractMap map, ExtRect dungeonArea)
        {
            fMap = map;
            fDungeonArea = !dungeonArea.IsEmpty() ? dungeonArea : map.AreaRect;
            fBusy = false;
            fAreasList = new List<DungeonArea>();

            InitDungeonSettings();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) {
                fAreasList.Clear();
                fAreasList = null;

                fAreaWeightTable = null;
            }
            base.Dispose(disposing);
        }

        public bool Busy
        {
            get {
                return fBusy;
            }
        }

        public IList<DungeonArea> AreasList
        {
            get {
                return fAreasList;
            }
        }

        public ExtRect DungeonArea
        {
            get {
                return fDungeonArea;
            }
        }

        public int DevouredArea
        {
            get {
                int result = 0;
    
                try {
                    foreach (DungeonArea area in fAreasList) {
                        result += area.DevourArea;
                    }
                } catch (Exception ex) {
                    Logger.Write("DungeonBuilder.getDevouredArea(): " + ex.Message);
                    result = -1;
                }
    
                return result;
            }
        }

        private IDungeonAreaCreateProc RandomAreaCreateProcByWeight
        {
            get {
                int[] weightedIndex = new int[100];
                int stoppedAt = 0;
    
                for (int at = AreaType.AtFirst; at <= AreaType.AtLast; at++) {
                    int num = stoppedAt + (int)fAreaWeightTable[at].Weight - 1;
                    for (int i = stoppedAt; i <= num; i++) {
                        weightedIndex[i] = at;
                    }
                    stoppedAt += (int)fAreaWeightTable[at].Weight;
                }
    
                int k = RandomHelper.GetBoundedRnd(0, 99);
                return fAreaWeightTable[weightedIndex[k]].AreaCreateProc;
            }
        }

        private bool TimeToTerminateBuilding
        {
            get {
                bool result;
                try {
                    result = false;
                    int i = 0;
                    while (i < fAreasList.Count && !result) {
                        DungeonArea area = fAreasList[i];
    
                        int mi = 0;
                        while (mi < area.MarksList.Count && !result) {
                            DungeonMark mark = area.MarksList[mi];
                            result = (mark.State == DungeonMark.Ms_Undefined);
                            mi++;
                        }
                        i++;
                    }
    
                    result = !result;
                } catch (Exception ex) {
                    Logger.Write("DungeonBuilder.isTimeToTerminateBuilding(): " + ex.Message);
                    result = true;
                }
                return result;
            }
        }

        private DungeonArea TryCreateArea(DungeonMark fromMark)
        {
            DungeonArea result;
            try {
                if (fromMark != null) {
                    IDungeonAreaCreateProc areaCreateProc;

                    if (fromMark.ForcedAreaCreateProc == null) {
                        areaCreateProc = RandomAreaCreateProcByWeight;
                    } else {
                        areaCreateProc = fromMark.ForcedAreaCreateProc;
                    }

                    result = areaCreateProc(this, fromMark);
                } else {
                    result = null;
                }
            } catch (Exception ex) {
                Logger.Write("DungeonBuilder.tryCreateArea(): " + ex.Message);
                result = null;
            }
            return result;
        }

        private void Suspend()
        {
            if (DungeonBuilder.Debug_StepByStep) {
                /*ShowMessage(null, "Message text");
                ((TField) fMap).research(false, new TTileStateSet(TTileState.tsSeen, TTileState.tsVisited));
                rGlobals.nwrWin.repaint(50);*/
            }
        }

        internal bool IsPointsToOtherArea(int ptX, int ptY, DungeonArea excludedArea)
        {
            try {
                foreach (DungeonArea area in fAreasList) {
                    if (!area.Equals(excludedArea) && area.IsAllowedPointAsMark(ptX, ptY)) {
                        return true;
                    }
                }
            } catch (Exception ex) {
                Logger.Write("DungeonBuilder.isPointsToOtherArea(): " + ex.Message);
            }
            return false;
        }

        internal IList<DungeonArea> IsAreaExistsAtPoint(int ptX, int ptY, DungeonArea excludedArea)
        {
            IList<DungeonArea> result = new List<DungeonArea>();
            try {
                foreach (DungeonArea area in fAreasList) {
                    if (!area.Equals(excludedArea) && area.IsOwnedPoint(ptX, ptY)) {
                        result.Add(area);
                    }
                }

                if (result.Count == 0) {
                    result = null;
                }
            } catch (Exception ex) {
                Logger.Write("DungeonBuilder.isAreaExistsAtPoint(): " + ex.Message);
                result = null;
            }
            return result;
        }

        internal bool IsFitAreaDimension(DungeonArea checkArea)
        {
            bool result = fDungeonArea.IsInside(checkArea.DimensionRect);

            if (result) {
                int i = 0;
                while (i < fAreasList.Count & result) {
                    DungeonArea area = fAreasList[i];
                    if (!area.Equals(checkArea)) {
                        result = (!checkArea.IsIntersectWithArea(area));
                    }
                    i++;
                }
            }

            return result;
        }

        internal void SetTile(int ax, int ay, TileType defTile)
        {
            TileType resTile = defTile;
            switch (defTile) {
                case TileType.ttUndefinedMark:
                    if (DungeonBuilder.Debug_MultiColors) {
                        resTile = TileType.ttMountain;
                    } else {
                        resTile = TileType.ttDungeonWall;
                    }
                    break;

                case TileType.ttAreaGeneratorMark:
                    if (DungeonBuilder.Debug_MultiColors) {
                        resTile = TileType.ttTree;
                    } else {
                        resTile = TileType.ttDoor;
                    }
                    break;

                case TileType.ttRetriesExhaustMark:
                    if (DungeonBuilder.Debug_MultiColors) {
                        resTile = TileType.ttMountain;
                    } else {
                        resTile = TileType.ttDungeonWall;
                    }
                    break;

                case TileType.ttPointToOtherAreaMark:
                    if (DungeonBuilder.Debug_MultiColors) {
                        resTile = TileType.ttTree;
                    } else {
                        resTile = TileType.ttDoor;
                    }
                    break;


                case TileType.ttLinearCorridorWall:
                    if (!DungeonBuilder.Debug_MultiColors) {
                        resTile = TileType.ttDungeonWall;
                    }
                    break;

                case TileType.ttRectRoomWall:
                    if (!DungeonBuilder.Debug_MultiColors) {
                        resTile = TileType.ttDungeonWall;
                    }
                    break;

                case TileType.ttCylindricityRoomWall:
                    if (!DungeonBuilder.Debug_MultiColors) {
                        resTile = TileType.ttDungeonWall;
                    }
                    break;

                case TileType.ttUndefined:
                case TileType.ttDungeonWall:
                default:
                    break;
            }

            BaseTile tile = fMap.GetTile(ax, ay);
            if (tile != null) {
                tile.Foreground = fMap.TranslateTile(resTile);
            } else {
                //Logger.write("DungeonBuilder.setTile(): tile is invalid");
            }
        }

        private void ProcessArea(DungeonArea currentArea)
        {
            try {
                fAreasList.Add(currentArea);
                currentArea.GenerateMarksList();
                currentArea.FlushToMap();

                if (!TimeToTerminateBuilding) {
                    foreach (DungeonMark mark in currentArea.MarksList) {
                        ProcessMark(mark);
                    }
                }

                currentArea.FlushToMap();

                if (DungeonBuilder.Debug_StepByStep) {
                    Suspend();
                }
            } catch (Exception ex) {
                Logger.Write("DungeonBuilder.processArea(): " + ex.Message);
                throw ex;
            }
        }

        private void ProcessMark(DungeonMark mark)
        {
            try {
                DungeonArea newArea = TryCreateArea(mark);
                if (newArea != null) {
                    if (newArea.TryApplyThisArea()) {
                        ProcessArea(newArea);
                    } else {
                        newArea.Dispose();
                    }
                }
            } catch (Exception ex) {
                Logger.Write("DungeonBuilder.processMark(): " + ex.Message);
                throw ex;
            }
        }

        public bool Build()
        {
            return Build(ExtPoint.Empty);
        }

        public bool Build(ExtPoint startup)
        {
            try {
                try {
                    if (fBusy) {
                        return false;
                    }

                    fBusy = true;
                    if (!WeightTableDefinedWell) {
                        throw new Exception("Areas Weight Table Defined Incorrect!\r\nPlease Check TDungeonSettings.areaWeight property.");
                    }

                    int mapArea = fDungeonArea.Square;
                    if (startup.IsEmpty) {
                        startup = fDungeonArea.GetCenter();
                    }

                    bool breakFlag;
                    do {
                        // cleanup the map
                        fMap.FillArea(fDungeonArea, fMap.TranslateTile(TileType.ttCaveFloor), false);
                        fMap.FillArea(fDungeonArea, fMap.TranslateTile(TileType.ttRock), true);
                        fAreasList.Clear();

                        // create the first mark
                        DungeonMark startupMark = new DungeonMark(null, startup.X, startup.Y, (RandomHelper.GetBoundedRnd(Directions.DtNorth, Directions.DtEast)));
                        startupMark.ForcedAreaCreateProc = fAreaWeightTable[ForcedStartupArea].AreaCreateProc;
                        ProcessMark(startupMark);

                        // check devoured square
                        float devouredArea = DevouredArea;
                        breakFlag = ((devouredArea / mapArea) * 100.0f) >= DevouredAreaBottomLimit;
                    } while (!breakFlag);

                    return true;
                } finally {
                    fBusy = false;
                }
            } catch (Exception ex) {
                Logger.Write("DungeonBuilder.build(): " + ex.Message);
                throw ex;
            }
        }

        public void SetAreaWeight(int areaType, int value)
        {
            fAreaWeightTable[areaType].Weight = (byte)value;
        }

        private bool WeightTableDefinedWell
        {
            get {
                int summary = 0;
    
                int index = 0;
                while (index < fAreaWeightTable.Length && summary <= 100) {
                    summary += fAreaWeightTable[index].Weight;
                    index += 1;
                }
    
                return summary == 100;
            }
        }

        private void InitDungeonSettings()
        {
            fAreaWeightTable = new WeightRow[19];
            fAreaWeightTable[0] = new WeightRow(RectangularRoom.CreateArea, 54);
            fAreaWeightTable[1] = new WeightRow(LinearCorridor.CreateArea, 10);
            fAreaWeightTable[2] = new WeightRow(CylindricityRoom.CreateArea, 6);
            fAreaWeightTable[3] = new WeightRow(QuadrantCorridor.CreateArea, 3);
            fAreaWeightTable[4] = new WeightRow(GenevaWheelRoom.CreateArea, 9);
            fAreaWeightTable[5] = new WeightRow(QuakeIIArena.CreateArea, 1);
            fAreaWeightTable[6] = new WeightRow(TempleRoom.CreateArea, 4);
            fAreaWeightTable[7] = new WeightRow(MonasticCellsRoom.CreateArea, 1);
            fAreaWeightTable[8] = new WeightRow(CryptRoom.CreateArea, 1);
            fAreaWeightTable[9] = new WeightRow(RoseRoom.CreateArea, 1);
            fAreaWeightTable[10] = new WeightRow(CrossroadRoom.CreateArea, 1);
            fAreaWeightTable[11] = new WeightRow(StarRoom.CreateArea, 1);
            fAreaWeightTable[12] = new WeightRow(FaithRoom.CreateArea, 1);
            fAreaWeightTable[13] = new WeightRow(SpiderRoom.CreateArea, 1);
            fAreaWeightTable[14] = new WeightRow(Alt1Room.CreateArea, 1);
            fAreaWeightTable[15] = new WeightRow(Alt2Room.CreateArea, 1);
            fAreaWeightTable[16] = new WeightRow(Alt3Room.CreateArea, 1);
            fAreaWeightTable[17] = new WeightRow(Alt4Room.CreateArea, 1);
            fAreaWeightTable[18] = new WeightRow(Alt5Room.CreateArea, 1);

            AreaSizeLimit = 10;
            CorridorWidthLimit = 5;
            CorridorWidthBottomLimit = 3;
            CorridorLengthBottomLimit = 3;
            CylindricityRoomRadius = 5;
            QuadrantCorridorInRadius = 5;
            QuadrantCorridorExRadius = (byte)(QuadrantCorridorInRadius + 3);
            MarkRetriesLimit = 10;
            RightMarkSearchLimit = 5;
            RectRoomMarksLimit = 5;
            LinearCorridorMarksLimit = 3;
            DevouredAreaBottomLimit = 70;
            ForcedStartupArea = AreaType.AtCylindricityRoom;
        }
    }
}
