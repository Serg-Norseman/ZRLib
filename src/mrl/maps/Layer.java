/*
 *  "MysteriesRL", Java Roguelike game.
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
package mrl.maps;

import jzrlib.core.CreatureEntity;
import jzrlib.core.LocatedEntity;
import jzrlib.core.LocatedEntityList;
import jzrlib.core.Point;
import jzrlib.core.Rect;
import jzrlib.map.AbstractMap;
import jzrlib.map.BaseTile;
import jzrlib.map.IMap;
import jzrlib.map.PathSearch;
import jzrlib.map.TileStates;
import jzrlib.map.dungeons.AreaType;
import jzrlib.map.dungeons.DungeonBuilder;
import jzrlib.utils.AuxUtils;
import jzrlib.utils.Logger;
import jzrlib.utils.RefObject;
import mrl.core.IProgressController;
import mrl.core.Locale;
import mrl.core.RS;
import mrl.creatures.Creature;

/**
 *
 * @author Serg V. Zhdanovskih
 */
public class Layer extends BaseMap
{
    private final LocatedEntityList fCreatures;
    private String fId;
    
    public Layer(boolean underground, String id)
    {
        super(2000, 2000);
        this.fCreatures = new LocatedEntityList(this, true);
        this.fId = id;
        
        if (!underground) {
            this.fillBackground(TileID.tid_Grass.Value);
        } else {
            //this.fillBackground(TileID.tid_DungeonFloor.Value);
            //this.fillForeground(TileID.tid_DungeonWall.Value);
        }
    }
    
    public final void initLayer(IProgressController progressController)
    {
        int lakes = AuxUtils.getBoundedRnd(15, 25);

        progressController.setStage(Locale.getStr(RS.rs_LayerGeneration), 2 + lakes);
        
        progressController.complete(0);
        
        gen_BigRiver(this, 2);
        progressController.complete(0);
        
        for (int i = 1; i <= lakes; i++) {
            Point pt = AuxUtils.getRandomPoint(this.getAreaRect());
            int lw = AuxUtils.getBoundedRnd(55, 215);
            int lh = AuxUtils.getBoundedRnd(55, 215);
            Rect lakeRt = new Rect(pt.X, pt.Y, pt.X + lw, pt.Y + lh);
            this.gen_Lake(lakeRt, this::lakeTileHandler);

            progressController.complete(0);
        }
    }

    private void lakeTileHandler(IMap map, int x, int y, Object extData, RefObject<Boolean> refContinue)
    {
        BaseTile tile = map.getTile(x, y);
        if (tile != null) {
            tile.Background = (short) TileID.tid_Water.Value;
        }
    }

    public static void gen_BigRiver(AbstractMap map, int normalizeLoops)
    {
        int mapWidth = map.getWidth();
        int mapHeight = map.getHeight();

        int x = 0;
        int y = 0;
        int x2 = AuxUtils.getRandom(mapWidth / 2 - 2) + mapWidth / 2;
        int y2 = AuxUtils.getRandom(mapHeight / 2 - 2) + mapHeight / 2;

        int num = AuxUtils.getRandom(4);
        switch (num) {
            case 0:
                x = AuxUtils.getRandom(mapWidth - 2) + 1;
                y = 1;
                break;
            case 1:
                x = 1;
                y = AuxUtils.getRandom(mapHeight - 2) + 1;
                break;
            case 2:
                x = mapWidth - 1;
                y = AuxUtils.getRandom(mapHeight - 2) + 1;
                break;
            case 3:
                x = AuxUtils.getRandom(mapWidth - 2) + 1;
                y = mapHeight - 1;
                break;
        }
        
        int tidWater = TileID.tid_Water.Value;
        
        map.gen_Path(x, y, x2, y2, map.getAreaRect(), tidWater, true, 20, true, null);

        for (int i = 1; i <= normalizeLoops; i++) {
            for (int yy = 1; yy <= mapHeight - 2; yy++) {
                for (int xx = 1; xx <= mapWidth - 2; xx++) {
                    BaseTile tile = map.getTile(xx, yy);

                    if (tile.Background == tidWater) {
                        int adjacents = map.checkAdjacently(xx, yy, tidWater, false);

                        if (adjacents < 2) {
                            tile.Background = (short) TileID.tid_Grass.Value;
                        } else if (adjacents >= 4) {
                            tile.Background = (short) TileID.tid_Water.Value;
                        }
                    }
                }
            }
        }
    }

    @Override
    public boolean isBlockLOS(int x, int y)
    {
        BaseTile tile = this.getTile(x, y);
        if (tile == null) {
            return true;
        }

        TileID bg = TileID.forValue(tile.Background);
        if (bg != null && bg.Flags.contains(TileFlags.tfBlockLOS)) {
            return true;
        }

        TileID fg = TileID.forValue(tile.Foreground);
        if (fg != null && fg.Flags.contains(TileFlags.tfBlockLOS)) {
            return true;
        }

        return false;
    }

    @Override
    public boolean isBarrier(int x, int y)
    {
        BaseTile tile = this.getTile(x, y);
        if (tile == null) {
            return true;
        }

        TileID bg = TileID.forValue(tile.Background);
        if (bg != null && bg.Flags.contains(TileFlags.tfBarrier)) {
            return true;
        }
        
        TileID fg = TileID.forValue(tile.Foreground);
        if (fg != null && fg.Flags.contains(TileFlags.tfBarrier)) {
            return true;
        }

        return false;
    }
    
    @Override
    public float getPathTileCost(CreatureEntity creature, int tx, int ty, BaseTile tile)
    {
        if (tile == null) {
            return PathSearch.BARRIER_COST;
        }

        float cost = 1.0f;

        TileID bg = TileID.forValue(tile.Background);
        if (bg != null) {
            cost = bg.Cost;
        }
        
        TileID fg = TileID.forValue(tile.Foreground);
        if (fg != null) {
            if (cost < fg.Cost) {
                cost = fg.Cost;
            }
        }

        return cost;
    }
    
    public void updateWater(Rect viewport)
    {
        int tidWater = TileID.tid_Water.Value;
        
        for (int y = viewport.Top; y <= viewport.Bottom; y++) {
            for (int x = viewport.Left; x <= viewport.Right; x++) {
                BaseTile tile = this.getTile(x, y);

                if (tile != null && tile.hasState(TileStates.TS_SEEN)) {
                    int bg = tile.getBackBase();
                    if (bg == tidWater) {
                        int var;
                        if (AuxUtils.getRandom(100) < 10) {
                            var = AuxUtils.getRandom(3);
                        } else {
                            var = tile.getBackVar();
                            if (var == 2) {
                                var = 0;
                            } else {
                                var++;
                            }
                        }
                        tile.setBack(bg, var);
                    }
                }
            }
        }
    }

    public final LocatedEntityList getCreatures()
    {
        return this.fCreatures;
    }

    public void addCreature(Creature creature)
    {
        this.fCreatures.add(creature);
    }
    
    @Override
    public CreatureEntity findCreature(int aX, int aY)
    {
        /*int code = aY * this.getWidth() + aX;
        Creature creature = this.fCreatures.get(code);
        return creature;*/

        return (CreatureEntity) this.fCreatures.searchItemByPos(aX, aY);
    }

    @Override
    public LocatedEntity findItem(int aX, int aY)
    {
        return null;
    }

    public final void initDungeon(Rect area, Point startup, boolean cellar)
    {
        try {
            DungeonBuilder builder = new DungeonBuilder(this, area);
            try {
                /*TDungeonBuilder.Debug_StepByStep = debugByStep;*/

                if (cellar) {
                    builder.setAreaWeight(AreaType.atRectangularRoom, 60);
                    builder.setAreaWeight(AreaType.atLinearCorridor, 20);
                    builder.setAreaWeight(AreaType.atCylindricityRoom, 0);
                    builder.setAreaWeight(AreaType.atQuadrantCorridor, 0);
                    builder.setAreaWeight(AreaType.atGenevaWheel, 0);
                    builder.setAreaWeight(AreaType.atQuakeIIArena, 0);
                    builder.setAreaWeight(AreaType.atTemple, 0);
                    builder.setAreaWeight(AreaType.atMonasticCells, 4);
                    builder.setAreaWeight(AreaType.atCrypt, 0);
                    builder.setAreaWeight(AreaType.atRoseRoom, 0);
                    builder.setAreaWeight(AreaType.atCrossroad, 0);
                    builder.setAreaWeight(AreaType.atStarRoom, 0);
                    builder.setAreaWeight(AreaType.atFaithRoom, 0);
                    builder.setAreaWeight(AreaType.atSpiderRoom, 0);
                    builder.setAreaWeight(AreaType.atAlt1Room, 4);
                    builder.setAreaWeight(AreaType.atAlt2Room, 4);
                    builder.setAreaWeight(AreaType.atAlt3Room, 4);
                    builder.setAreaWeight(AreaType.atAlt4Room, 4);
                    builder.setAreaWeight(AreaType.atAlt5Room, 0);
                    builder.DevouredAreaBottomLimit = 80;
                } else {
                    builder.setAreaWeight(AreaType.atRectangularRoom, 46);
                    builder.setAreaWeight(AreaType.atLinearCorridor, 20);
                    builder.setAreaWeight(AreaType.atCylindricityRoom, 0);
                    builder.setAreaWeight(AreaType.atQuadrantCorridor, 0);
                    builder.setAreaWeight(AreaType.atGenevaWheel, 4);
                    builder.setAreaWeight(AreaType.atQuakeIIArena, 0);
                    builder.setAreaWeight(AreaType.atTemple, 4);
                    builder.setAreaWeight(AreaType.atMonasticCells, 4);
                    builder.setAreaWeight(AreaType.atCrypt, 4);
                    builder.setAreaWeight(AreaType.atRoseRoom, 4);
                    builder.setAreaWeight(AreaType.atCrossroad, 4);
                    builder.setAreaWeight(AreaType.atStarRoom, 0);
                    builder.setAreaWeight(AreaType.atFaithRoom, 0);
                    builder.setAreaWeight(AreaType.atSpiderRoom, 0);
                    builder.setAreaWeight(AreaType.atAlt1Room, 2);
                    builder.setAreaWeight(AreaType.atAlt2Room, 3);
                    builder.setAreaWeight(AreaType.atAlt3Room, 2);
                    builder.setAreaWeight(AreaType.atAlt4Room, 3);
                    builder.setAreaWeight(AreaType.atAlt5Room, 0);
                    builder.DevouredAreaBottomLimit = 60;
                }

                builder.build(startup);
            } finally {
                builder.dispose();
            }
        } catch (Exception ex) {
            Logger.write("Layer.initDungeon(): " + ex.getMessage());
            throw ex;
        }
    }
}
