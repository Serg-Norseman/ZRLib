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
package mrl.game;

import java.awt.Color;
import java.io.File;
import java.io.IOException;
import java.util.ArrayList;
import java.util.List;
import jzrlib.common.DayTime;
import jzrlib.common.GameTime;
import jzrlib.core.EntityList;
import jzrlib.core.GameEntity;
import jzrlib.core.GameSpace;
import jzrlib.core.Point;
import jzrlib.core.Rect;
import jzrlib.core.event.Event;
import jzrlib.core.event.IEventListener;
import jzrlib.core.event.ScheduledEventManager;
import jzrlib.map.IMap;
import jzrlib.utils.AuxUtils;
import jzrlib.utils.Logger;
import mrl.core.IProgressController;
import mrl.core.Locale;
import mrl.core.RS;
import mrl.creatures.Creature;
import mrl.game.events.VandalismEvent;
import mrl.generators.CityGenerator;
import mrl.maps.Layer;
import mrl.maps.MapFeature;
import mrl.maps.Minimap;
import mrl.maps.buildings.Building;
import mrl.maps.buildings.HouseStatus;
import mrl.maps.buildings.Room;
import mrl.maps.buildings.features.Stairs;
import mrl.maps.city.City;
import mrl.maps.city.CityRegion;
import mrl.maps.city.District;
import mrl.maps.city.Street;

/**
 *
 * @author Serg V. Zhdanovskih
 */
public final class Game extends GameSpace implements IEventListener
{
    private final Layer fPlainMap;
    private final Layer fCellarsMap;
    private final Layer fDungeonsMap;
    
    private final List<Message> fMessages;
    private final PlayerController fPlayerController;
    private final GameTime fTime;
    
    private City fCity;
    
    public IMap fMinimap;

    public Game(Object owner)
    {
        super(owner);
        
        Logger.init(getAppPath() + "MysteriesRL.log");
        
        this.fPlainMap = new Layer(false, "plain");
        this.fCellarsMap = new Layer(true, "cellars");
        this.fDungeonsMap = new Layer(true, "dungeons");
        
        this.fMinimap = null;
        this.fMessages = new ArrayList<>();
        this.fPlayerController = new PlayerController();
        this.fTime = new GameTime();
        
        ScheduledEventManager.subscribe(this);
    }
    
    public static String getAppPath()
    {
        String applicationDir = Game.class.getProtectionDomain().getCodeSource().getLocation().getPath();

        String result = applicationDir;
        if (result.endsWith("classes/")) {
            result = result + "../../../";
        } else if (result.endsWith("/MysteriesRL.jar")) {
            result = result.substring(0, result.indexOf("/MysteriesRL.jar"));
        }

        File file = new File(result);
        try {
            result = file.getCanonicalPath() + "\\";
        } catch (IOException ex) {
        }

        return result;
    }

    public final IMap getPlainMap()
    {
        return this.fPlainMap;
    }
    
    public final IMap getCellarsMap()
    {
        return this.fCellarsMap;
    }
    
    public final IMap getDungeonsMap()
    {
        return this.fDungeonsMap;
    }
    
    public final City getCity()
    {
        return this.fCity;
    }
    
    public final PlayerController getPlayerController()
    {
        return this.fPlayerController;
    }
    
    public final void initNew(IProgressController progressController)
    {
        this.fTime.set(1890, 07, 01, 12, 00, 00);

        this.fPlainMap.initLayer(progressController);

        int mw = fPlainMap.getWidth();
        int mh = fPlainMap.getHeight();
        
        int ctH = AuxUtils.getBoundedRnd(mh / 2, (int) (mh * 0.66f));
        int ctW = AuxUtils.getBoundedRnd(mw / 2, (int) (mw * 0.66f));
        
        int ctX = AuxUtils.getRandom(mw - ctW);
        int ctY = AuxUtils.getRandom(mh - ctH);
        
        Rect cityArea = new Rect(ctX, ctY, ctX + ctW, ctY + ctH);
        
        progressController.setStage(Locale.getStr(RS.rs_DungeonsGeneration), 100);
        //this.fDungeonsMap.initDungeon(cityArea.clone(), null, false);
        
        this.fCity = new City(this, fPlainMap, cityArea);
        CityGenerator civFactory = new CityGenerator(fPlainMap, this.fCity, progressController);
        civFactory.buildCity();
        
        this.fMinimap = new Minimap(this.fPlainMap, this.fCity);

        List<Building> blds = this.fCity.getBuildings();
        progressController.setStage(Locale.getStr(RS.rs_BuildingsPopulate), blds.size());
        for (Building bld : blds) {
            if (!bld.Populated) {
                bld.fill();
            }

            progressController.complete(0);
        }

        progressController.setStage(Locale.getStr(RS.rs_CellarsGeneration), blds.size());
        for (Building bld : blds) {
            if (bld.getStatus() == HouseStatus.hsMansion) {
                this.genCellar(bld);
            }

            progressController.complete(0);
        }

        // flush features
        EntityList feats = this.fPlainMap.getFeatures();
        for (int i = 0; i < feats.getCount(); i++) {
            GameEntity entity = feats.getItem(i);
            
            if (entity instanceof MapFeature) {
                MapFeature mf = (MapFeature) entity;
                
                this.fPlainMap.getTile(mf.getPosX(), mf.getPosY()).Foreground = (short) mf.getTileID().Value;
            }
        }
    }
    
    private void genCellar(Building bld)
    {
        Rect privArea = bld.getPrivathandArea();
        if (privArea == null) {
            privArea = bld.getArea();
        }
        
        privArea = privArea.clone();
        this.fCellarsMap.initDungeon(privArea, null, true);

        this.genStairway(this.fPlainMap, this.fCellarsMap, privArea);
        //this.genStairway(this.fCellarsMap, this.fDungeonsMap, privArea);
    }

    private void genStairway(IMap map1, IMap map2, Rect area)
    {
        Point src = map1.searchFreeLocation(area);
        Point dst = map2.searchFreeLocation(area);
        
        if (src != null && dst != null) {
            Stairs stairs = new Stairs(this, map1, src, dst);
            stairs.setDescending(true);
            map1.getFeatures().add(stairs);
            stairs.render();

            stairs = new Stairs(this, map2, dst, src);
            stairs.setDescending(false);
            map2.getFeatures().add(stairs);
            stairs.render();
        } else {
            Logger.write("Game.genStairway(): null");
        }
    }
    
    public final GameTime getTime()
    {
        return this.fTime;
    }
    
    public final String getLocationName(int px, int py)
    {
        PlayerController playerController = this.getPlayerController();
        Creature player = playerController.getPlayer();
        
        if (px < 0 && py < 0) {
            px = player.getPosX();
            py = player.getPosY();
        }
        
        CityRegion region = this.fCity.findRegion(px, py);
        if (region != null) {
            if (region instanceof Street) {
                return String.valueOf(((Street) region).Num) + " street";
            } else if (region instanceof District) {
                String res;
                res = String.valueOf(((District) region).Num) + " district";
                
                Building bld = this.fCity.findBuilding(px, py);
                if (bld != null) {
                    res += ", " + bld.getAddress();
                    
                    HouseStatus status = bld.getStatus();
                    res += " (" + status.Name + ")";
                    
                    Room room = bld.findRoom(px, py);
                    if (room != null) {
                        res += "[" + room.getDesc() + "]";
                    }
                }
                
                return res;
            }
        }

        return "unknown";
    }

    public final List<Message> getMessages()
    {
        return this.fMessages;
    }
    
    public final void addMessage(String text)
    {
        this.addMessage(text, Color.lightGray);
    }

    public final void addMessage(String text, Color color)
    {
        Message msg = new Message(text, color);
        this.fMessages.add(msg);
    }
    
    public final boolean checkTarget(int mx, int my)
    {
        if (this.fPlainMap.isBarrier(mx, my)) {
            return false;
        }
        return true;
    }
    
    public final void look(int mx, int my)
    {
        boolean here = (mx == -1 && my == -1);

        String prefix;
        if (here) {
            prefix = "Current location: ";
        } else {
            prefix = "Location: ";
        }

        String location = prefix + this.getLocationName(mx, my);
        this.addMessage(location);

        if (!here) {
            Creature creature = (Creature) this.fPlainMap.findCreature(mx, my);
            if (creature != null) {
                this.addMessage("You see " + creature.getDesc());
            }
        }
    }
    
    public final float getLightFactor()
    {
        DayTime dt = this.fTime.getDayTime();
        return dt.RadMod;
    }
    
    public final float getLightBrightness()
    {
        DayTime dt = this.fTime.getDayTime();
        return dt.Brightness;
    }
    
    // every 100 ms
    public final void doTurn()
    {
        this.fTime.tick(60);

        this.getPlayerController().doPathStep();

        this.getPlayerController().getPlayer().doTurn();
    }

    public final void updateWater()
    {
        this.fPlainMap.updateWater(this.getPlayerController().getViewport());
    }
    
    @Override
    public void onEvent(Event event)
    {
        if (event instanceof VandalismEvent) {
            this.addMessage("vandalism!!!", Color.red);
            this.fPlayerController.getPlayer().getStats().addXP(50);
        }
    }
    
    public final List<GameEntity> getEntities(Point pt, int radius)
    {
        return null;
    }
}
