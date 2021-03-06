/*
 *  "MysteriesRL", roguelike game.
 *  Copyright (C) 2015, 2017 by Serg V. Zhdanovskih.
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
using MysteriesRL.Creatures;
using MysteriesRL.Game;
using ZRLib.Core;
using ZRLib.Map;
using ZRLib.Map.Dungeons;

namespace MysteriesRL.Maps
{
    public class Layer : BaseMap
    {
        private readonly LocatedEntityList<Creature> fCreatures;
        private string fId;

        public LocatedEntityList<Creature> Creatures
        {
            get {
                return fCreatures;
            }
        }


        public Layer(bool underground, string id)
            : base(2000, 2000)
        {
            fCreatures = new LocatedEntityList<Creature>(this);
            fId = id;

            if (!underground) {
                FillBackground((int)TileID.tid_Grass);
            } else {
                //fillBackground(TileID.tid_DungeonFloor.Value);
                //fillForeground(TileID.tid_DungeonWall.Value);
            }
        }

        public void InitPlainLayer(IProgressController progressController)
        {
            int lakes = RandomHelper.GetBoundedRnd(15, 25);

            progressController.SetStage(Locale.GetStr(RS.Rs_LayerGeneration), 2 + lakes);

            progressController.Complete(0);

            MMapUtils.Gen_BigRiver(this, 2);
            progressController.Complete(0);

            for (int i = 1; i <= lakes; i++) {
                ExtPoint pt = RandomHelper.GetRandomPoint(AreaRect);
                int lw = RandomHelper.GetBoundedRnd(55, 215);
                int lh = RandomHelper.GetBoundedRnd(55, 215);
                ExtRect lakeRt = ExtRect.Create(pt.X, pt.Y, pt.X + lw, pt.Y + lh);
                Gen_Lake(lakeRt, LakeTileHandler);

                progressController.Complete(0);
            }
        }

        private static void LakeTileHandler(IMap map, int x, int y, object extData, ref bool refContinue)
        {
            BaseTile tile = map.GetTile(x, y);
            if (tile != null) {
                tile.Background = (ushort)TileID.tid_Water;
            }
        }

        public override bool IsBlockLOS(int x, int y)
        {
            BaseTile tile = GetTile(x, y);
            if (tile == null) {
                return true;
            }

            if (tile.Background != 0) {
                var tileRec = MRLData.Tiles[tile.Background];
                if (tileRec.Flags.Contains(TileFlags.tfBlockLOS)) {
                    return true;
                }
            }

            if (tile.Foreground != 0) {
                var tileRec = MRLData.Tiles[tile.Foreground];
                if (tileRec.Flags.Contains(TileFlags.tfBlockLOS)) {
                    return true;
                }
            }

            return false;
        }

        public override bool IsBarrier(int x, int y)
        {
            BaseTile tile = GetTile(x, y);
            if (tile == null) {
                return true;
            }

            if (tile.Background != 0) {
                var tileRec = MRLData.Tiles[tile.Background];
                if (tileRec.Flags.Contains(TileFlags.tfBarrier)) {
                    return true;
                }
            }

            if (tile.Foreground != 0) {
                var tileRec = MRLData.Tiles[tile.Foreground];
                if (tileRec.Flags.Contains(TileFlags.tfBarrier)) {
                    return true;
                }
            }

            return false;
        }

        public override float GetPathTileCost(CreatureEntity creature, int tx, int ty, BaseTile tile)
        {
            if (tile == null) {
                return PathSearch.BARRIER_COST;
            }

            float cost = 1.0f;

            if (tile.Background != 0) {
                var tileRec = MRLData.Tiles[tile.Background];
                cost = tileRec.Cost;
            }

            if (tile.Foreground != 0) {
                var tileRec = MRLData.Tiles[tile.Foreground];
                if (cost < tileRec.Cost) {
                    cost = tileRec.Cost;
                }
            }

            return cost;
        }

        public virtual void UpdateWater(ExtRect viewport)
        {
            int tidWater = (int)TileID.tid_Water;

            for (int y = viewport.Top; y <= viewport.Bottom; y++) {
                for (int x = viewport.Left; x <= viewport.Right; x++) {
                    BaseTile tile = GetTile(x, y);

                    if (tile != null && tile.HasState(BaseTile.TS_SEEN)) {
                        int bg = tile.BackBase;
                        if (bg == tidWater) {
                            int @var;
                            if (RandomHelper.GetRandom(100) < 10) {
                                @var = RandomHelper.GetRandom(3);
                            } else {
                                @var = tile.BackVar;
                                if (@var == 2) {
                                    @var = 0;
                                } else {
                                    @var++;
                                }
                            }
                            tile.SetBack(bg, @var);
                        }
                    }
                }
            }
        }

        public virtual void AddCreature(Creature creature)
        {
            fCreatures.Add(creature);
        }

        public override CreatureEntity FindCreature(int aX, int aY)
        {
            /*int code = aY * getWidth() + aX;
            Creature creature = fCreatures.get(code);
            return creature;*/

            return (CreatureEntity)fCreatures.SearchItemByPos(aX, aY);
        }

        public override LocatedEntity FindItem(int aX, int aY)
        {
            return null;
        }

        public void InitDungeon(ExtRect area, ExtPoint startup, bool cellar)
        {
            try {
                DungeonBuilder builder = new DungeonBuilder(this, area);
                try {
                    /*DungeonBuilder.Debug_StepByStep = debugByStep;*/

                    if (cellar) {
                        builder.SetAreaWeight(AreaType.AtRectangularRoom, 60);
                        builder.SetAreaWeight(AreaType.AtLinearCorridor, 20);
                        builder.SetAreaWeight(AreaType.AtCylindricityRoom, 0);
                        builder.SetAreaWeight(AreaType.AtQuadrantCorridor, 0);
                        builder.SetAreaWeight(AreaType.AtGenevaWheel, 0);
                        builder.SetAreaWeight(AreaType.AtQuakeIIArena, 0);
                        builder.SetAreaWeight(AreaType.AtTemple, 0);
                        builder.SetAreaWeight(AreaType.AtMonasticCells, 4);
                        builder.SetAreaWeight(AreaType.AtCrypt, 0);
                        builder.SetAreaWeight(AreaType.AtRoseRoom, 0);
                        builder.SetAreaWeight(AreaType.AtCrossroad, 0);
                        builder.SetAreaWeight(AreaType.AtStarRoom, 0);
                        builder.SetAreaWeight(AreaType.AtFaithRoom, 0);
                        builder.SetAreaWeight(AreaType.AtSpiderRoom, 0);
                        builder.SetAreaWeight(AreaType.AtAlt1Room, 4);
                        builder.SetAreaWeight(AreaType.AtAlt2Room, 4);
                        builder.SetAreaWeight(AreaType.AtAlt3Room, 4);
                        builder.SetAreaWeight(AreaType.AtAlt4Room, 4);
                        builder.SetAreaWeight(AreaType.AtAlt5Room, 0);
                        builder.DevouredAreaBottomLimit = 80;
                    } else {
                        builder.SetAreaWeight(AreaType.AtRectangularRoom, 46);
                        builder.SetAreaWeight(AreaType.AtLinearCorridor, 20);
                        builder.SetAreaWeight(AreaType.AtCylindricityRoom, 0);
                        builder.SetAreaWeight(AreaType.AtQuadrantCorridor, 0);
                        builder.SetAreaWeight(AreaType.AtGenevaWheel, 4);
                        builder.SetAreaWeight(AreaType.AtQuakeIIArena, 0);
                        builder.SetAreaWeight(AreaType.AtTemple, 4);
                        builder.SetAreaWeight(AreaType.AtMonasticCells, 4);
                        builder.SetAreaWeight(AreaType.AtCrypt, 4);
                        builder.SetAreaWeight(AreaType.AtRoseRoom, 4);
                        builder.SetAreaWeight(AreaType.AtCrossroad, 4);
                        builder.SetAreaWeight(AreaType.AtStarRoom, 0);
                        builder.SetAreaWeight(AreaType.AtFaithRoom, 0);
                        builder.SetAreaWeight(AreaType.AtSpiderRoom, 0);
                        builder.SetAreaWeight(AreaType.AtAlt1Room, 2);
                        builder.SetAreaWeight(AreaType.AtAlt2Room, 3);
                        builder.SetAreaWeight(AreaType.AtAlt3Room, 2);
                        builder.SetAreaWeight(AreaType.AtAlt4Room, 3);
                        builder.SetAreaWeight(AreaType.AtAlt5Room, 0);
                        builder.DevouredAreaBottomLimit = 60;
                    }

                    builder.Build(startup);
                } finally {
                    builder.Dispose();
                }
            } catch (Exception ex) {
                Logger.Write("Layer.initDungeon(): " + ex.Message);
                throw ex;
            }
        }
    }
}
