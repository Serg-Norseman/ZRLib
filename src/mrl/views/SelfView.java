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
package mrl.views;

import java.awt.Color;
import java.awt.event.KeyEvent;
import java.awt.event.MouseEvent;
import jzrlib.jterm.JTerminal;
import mrl.core.GlobalData;
import mrl.creatures.Human;
import mrl.creatures.NPCStats;
import mrl.creatures.body.HumanBody;
import mrl.game.Game;
import mrl.maps.Layer;
import mrl.maps.city.City;

/**
 *
 * @author Serg V. Zhdanovskih
 */
public final class SelfView extends SubView
{
    
    public SelfView(BaseView ownerView, JTerminal terminal)
    {
        super(ownerView, terminal);
    }

    @Override
    public final Game getGameSpace()
    {
        return ((MainView) this.fOwnerView).getGameSpace();
    }

    @Override
    protected void updateView()
    {
        this.fTerminal.clear();
        this.fTerminal.setTextBackground(Color.black);
        this.fTerminal.setTextForeground(Color.white);
        UIPainter.drawBox(fTerminal, 0, 0, 159, 79, false);
        
        Game gameSpace = this.getGameSpace();
        Human player = gameSpace.getPlayerController().getPlayer();
        NPCStats stats = player.getStats();
        HumanBody body = (HumanBody) player.getBody();
        City city = gameSpace.getCity();
        Layer layer = (Layer) gameSpace.getPlainMap();
        
        this.fTerminal.setTextForeground(Color.lightGray);
        UIPainter.drawBox(fTerminal, 2, 2, 50, 50, false, "Player");
        this.fTerminal.write(4,  4, "Player name:  " + player.getName(), Color.white);
        this.fTerminal.write(4,  6, "Sex:          " + player.getSex().name(), Color.white);
        this.fTerminal.write(4,  8, "Age:          " + String.valueOf(player.getAge()), Color.white);
        //this.fTerminal.write(4, 10, "Personality:  " + player.getPersonality().getDesc(), Color.white);

        this.fTerminal.write(4, 14, "Strength:     " + String.valueOf(stats.Str), Color.white);
        this.fTerminal.write(4, 16, "Perception:   " + String.valueOf(stats.Per), Color.white);
        this.fTerminal.write(4, 18, "Endurance:    " + String.valueOf(stats.End), Color.white);
        this.fTerminal.write(4, 20, "Charisma:     " + String.valueOf(stats.Chr), Color.white);
        this.fTerminal.write(4, 22, "Intelligence: " + String.valueOf(stats.Int), Color.white);
        this.fTerminal.write(4, 24, "Agility:      " + String.valueOf(stats.Agi), Color.white);
        this.fTerminal.write(4, 26, "Luck:         " + String.valueOf(stats.Luk), Color.white);

        this.fTerminal.write(4, 30, "Level:        " + String.valueOf(stats.Level), Color.white);
        this.fTerminal.write(4, 32, "XP:           " + String.valueOf(stats.getCurXP()) + " / " + String.valueOf(stats.getNextLevelXP()), Color.white);
        this.fTerminal.write(4, 34, "Carry Weight: " + String.valueOf(0) + " / " + String.valueOf(stats.getCarryWeight()), Color.white);
        this.fTerminal.write(4, 36, "HP:           " + String.valueOf(player.getHP()) + " / " + String.valueOf(player.getHPMax()), Color.white);

        this.fTerminal.write(4, 40, "Stamina:      " + body.getAttribute("stamina"), Color.white);
        this.fTerminal.write(4, 42, "Hunger:       " + body.getAttribute("hunger"), Color.white);

        UIPainter.drawBox(fTerminal, 103, 2, 157, 14, false, "City");
        this.fTerminal.write(105,  4, "Name:        " + city.getName(), Color.white);
        this.fTerminal.write(105,  6, "Streets:     " + String.valueOf(city.getStreets().size()), Color.white);
        this.fTerminal.write(105,  8, "Districts:   " + String.valueOf(city.getDistricts().size()), Color.white);
        this.fTerminal.write(105, 10, "Buildings:   " + String.valueOf(city.getBuildings().size()), Color.white);
        this.fTerminal.write(105, 12, "Inhabitants: " + String.valueOf(layer.getCreatures().getCount()), Color.white);
        
        UIPainter.drawBox(fTerminal, 52, 2, 101, 50, false, "Crimes");
        
        UIPainter.drawBox(fTerminal, 2, 52, 157, 78, false, "Description");
        this.fTerminal.write(4, 54, player.getDesc(), Color.white);
    }
    
    @Override
    public void tick()
    {
    }

    @Override
    public void keyPressed(KeyEvent e)
    {
        switch (e.getKeyCode()) {
            case GlobalData.KEY_ESC:
                this.getMainView().setView(ViewType.vtGame);
                break;

            default:
                break;
        }
    }

    @Override
    public void keyTyped(KeyEvent e)
    {
    }

    @Override
    public void mouseClicked(MouseEvent e)
    {
    }

    @Override
    public void mouseMoved(MouseEvent e)
    {
    }
    
    @Override
    public final void show()
    {
    }
}
