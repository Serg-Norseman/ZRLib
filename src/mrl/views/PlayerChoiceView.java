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
import java.util.ArrayList;
import java.util.List;
import jzrlib.core.LocatedEntityList;
import jzrlib.jterm.JTerminal;
import jzrlib.utils.AuxUtils;
import mrl.core.GlobalData;
import mrl.core.Locale;
import mrl.core.RS;
import mrl.creatures.Human;
import mrl.creatures.NPCStats;
import mrl.game.Game;
import mrl.maps.Layer;
import mrl.maps.buildings.Building;
import mrl.maps.buildings.HouseStatus;
import mrl.views.controls.ChoicesArea;

/**
 *
 * @author Serg V. Zhdanovskih
 */
public final class PlayerChoiceView extends SubView
{
    private final List<Human> fCandidates;
    private final ChoicesArea fChoicesArea;
    
    public PlayerChoiceView(BaseView ownerView, JTerminal terminal)
    {
        super(ownerView, terminal);
        this.fCandidates = new ArrayList<>();
        this.fChoicesArea = new ChoicesArea(terminal, 10, Color.green.darker());
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
        
        this.fTerminal.setTextForeground(Color.lightGray);
        UIPainter.writeCenter(fTerminal, 1, 158, 2, Locale.getStr(RS.rs_PlayerChoices));
        
        this.fChoicesArea.draw();
        
        UIPainter.writeCenter(fTerminal, 1, 158, 65, Locale.getStr(RS.rs_PressNumberOfChoice));
    }
    
    @Override
    public final void show()
    {
        LocatedEntityList population = ((Layer) this.getGameSpace().getPlainMap()).getCreatures();
        
        for (int i = 0; i < population.getCount(); i++) {
            Human human = (Human) population.getItem(i);
            int age = human.getAge();
            
            Building bld = human.getApartment();
            
            if (age >= 20 && age <= 35 && bld != null && bld.getStatus() == HouseStatus.hsMansion) {
                this.fCandidates.add(human);
                if (this.fCandidates.size() == 10) {
                    break;
                }
            }
        }
        
        int maxw = 0;
        for (Human human : this.fCandidates) {
            if (maxw < human.getName().length()) {
                maxw = human.getName().length();
            }
        }

        int idx = 0;
        for (Human human : this.fCandidates) {
            String name = human.getName();
            String desc = getHumanDesc(human);

            this.fChoicesArea.addChoice(String.valueOf(idx).charAt(0), AuxUtils.adjustString(name, maxw) + "  " + desc);
            idx++;
        }
    }
    
    private String getHumanDesc(Human human)
    {
        StringBuilder result = new StringBuilder();
        
        String str = String.valueOf(human.getAge());
        result.append("age ");
        result.append(str);
        
        NPCStats stats = human.getStats();
        str = String.format(", str %d, per %d, end %d, chr %d, int %d, agi %d, luck %d", stats.Str, stats.Per, stats.End, stats.Chr, stats.Int, stats.Agi, stats.Luk);
        result.append(str);

        return result.toString();
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
                this.getMainView().setView(ViewType.vtStartup);
                break;

            default:
                break;
        }
    }

    @Override
    public void keyTyped(KeyEvent e)
    {
        char keyChar = e.getKeyChar();
        if (keyChar >= '0' && keyChar <= '9') {
            int num = Integer.valueOf("0" + keyChar);
            if (num >= 0 && num < this.fCandidates.size()) {
                this.getGameSpace().getPlayerController().attach(this.fCandidates.get(num));
                this.getMainView().setView(ViewType.vtGame);
            }
        }
    }

    @Override
    public void mouseClicked(MouseEvent e)
    {
    }

    @Override
    public void mouseMoved(MouseEvent e)
    {
    }
}
