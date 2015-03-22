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
package mrl.core;

import jzrlib.core.Rect;
import java.awt.event.KeyEvent;

/**
 *
 * @author Serg V. Zhdanovskih
 */
public class GlobalData
{
    public static final String MRL_NAME = "The MysteriesRL";
    public static final String MRL_VER = "v0.0.4";
    public static final String MRL_DEV_TIME = "2015";
    public static final String MRL_COPYRIGHT = "Copyright (c) " + MRL_DEV_TIME + " by Serg V. Zhdanovskih";
    
    public static final String MRL_DEFLANG = "en";

    // Game viewport
    public static final Rect GV_BOUNDS = new Rect(1, 1, 101, 69); // 80 - (4 + 7) = 80 - 11 = 69
    public static final int GV_XRad = 50;
    public static final int GV_YRad = 34;
    
    public static final boolean DEBUG_DISTRICTS = false;
    public static final boolean DEBUG_CITYGEN = false;
    public static final boolean DEBUG_BODY = true;
    
    public static final int KEY_TAB = KeyEvent.VK_TAB;
    public static final int KEY_ESC = KeyEvent.VK_ESCAPE;
    
    public static final int KEY_LEFT = KeyEvent.VK_LEFT;
    public static final int KEY_UP = KeyEvent.VK_UP;
    public static final int KEY_RIGHT = KeyEvent.VK_RIGHT;
    public static final int KEY_DOWN = KeyEvent.VK_DOWN;
}
