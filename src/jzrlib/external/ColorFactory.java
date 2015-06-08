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
package jzrlib.external;

import java.awt.Color;

/**
 * ColorFactory implements all "X11 colors" from the CSS3 specification as
 * defined in the W3C standard. Also included are some extra Netscape colors.
 *
 * @author	Maarten Driesen
 */
public final class ColorFactory
{
    // red colors
    public static final Color indianRed = new Color(205, 92, 92);
    public static final Color lightCoral = new Color(240, 128, 128);
    public static final Color salmon = new Color(250, 128, 114);
    public static final Color darkSalmon = new Color(233, 150, 122);
    public static final Color lightSalmon = new Color(255, 160, 120);
    public static final Color crimson = new Color(220, 20, 60);
    public static final Color red = new Color(255, 0, 0);
    public static final Color fireBrick = new Color(178, 34, 34);
    public static final Color darkRed = new Color(139, 0, 0);
    // pink colors
    public static final Color pink = new Color(255, 192, 203);
    public static final Color lightPink = new Color(255, 182, 193);
    public static final Color hotPink = new Color(255, 105, 180);
    public static final Color deepPink = new Color(255, 20, 147);
    public static final Color mediumVioletRed = new Color(199, 21, 133);
    public static final Color paleVioletRed = new Color(219, 112, 147);
    // orange colors
    public static final Color coral = new Color(255, 127, 80);
    public static final Color tomato = new Color(255, 99, 71);
    public static final Color orangeRed = new Color(255, 69, 0);
    public static final Color darkOrange = new Color(255, 140, 0);
    public static final Color orange = new Color(255, 165, 0);
    // yellow colors
    public static final Color gold = new Color(255, 215, 0);
    public static final Color yellow = new Color(255, 255, 0);
    public static final Color lightYellow = new Color(255, 255, 224);
    public static final Color lemmonChiffon = new Color(255, 250, 205);
    public static final Color lightGoldenrodYellow = new Color(250, 250, 210);
    public static final Color papayaWhip = new Color(255, 239, 213);
    public static final Color moccassin = new Color(255, 228, 181);
    public static final Color peachPuff = new Color(255, 218, 185);
    public static final Color paleGoldenrod = new Color(238, 232, 170);
    public static final Color khaki = new Color(240, 230, 140);
    public static final Color darkKhaki = new Color(189, 183, 107);
    // purple colors
    public static final Color lavender = new Color(230, 230, 250);
    public static final Color thistle = new Color(216, 191, 216);
    public static final Color plum = new Color(221, 160, 221);
    public static final Color violet = new Color(238, 130, 238);
    public static final Color orchid = new Color(218, 112, 214);
    public static final Color fuchsia = new Color(255, 0, 255);
    public static final Color magenta = new Color(255, 0, 255);
    public static final Color mediumOrchid = new Color(186, 85, 211);
    public static final Color mediumPurple = new Color(147, 112, 219);
    public static final Color blueViolet = new Color(138, 43, 226);
    public static final Color darkViolet = new Color(148, 0, 211);
    public static final Color darkOrchid = new Color(153, 50, 204);
    public static final Color darkMagenta = new Color(139, 0, 139);
    public static final Color purple = new Color(128, 0, 128);
    public static final Color indigo = new Color(75, 0, 130);
    public static final Color slateBlue = new Color(106, 90, 205);
    public static final Color darkSlateBlue = new Color(72, 61, 139);
    // green colors
    public static final Color greenYellow = new Color(173, 255, 47);
    public static final Color chartreuse = new Color(127, 255, 0);
    public static final Color lawnGreen = new Color(124, 252, 0);
    public static final Color lime = new Color(0, 255, 0);
    public static final Color limeGreen = new Color(50, 205, 50);
    public static final Color paleGreen = new Color(152, 251, 152);
    public static final Color lightGreen = new Color(144, 238, 144);
    public static final Color mediumSpringGreen = new Color(0, 250, 154);
    public static final Color springGreen = new Color(0, 255, 127);
    public static final Color mediumSeaGreen = new Color(60, 179, 113);
    public static final Color seaGreen = new Color(46, 139, 87);
    public static final Color forestGreen = new Color(34, 139, 34);
    public static final Color green = new Color(0, 128, 0);
    public static final Color darkGreen = new Color(0, 100, 0);
    public static final Color yellowGreen = new Color(154, 205, 50);
    public static final Color oliveDrab = new Color(107, 142, 35);
    public static final Color olive = new Color(128, 128, 0);
    public static final Color darkOliveGreen = new Color(85, 107, 47);
    public static final Color mediumAquamarine = new Color(102, 205, 170);
    public static final Color darkSeaGreen = new Color(143, 188, 143);
    public static final Color lightSeaGreen = new Color(32, 178, 170);
    public static final Color darkGreenCopper = new Color(74, 118, 110);
    public static final Color darkCyan = new Color(0, 139, 139);
    public static final Color teal = new Color(0, 128, 128);
    // blue colors
    public static final Color aqua = new Color(0, 255, 255);
    public static final Color cyan = new Color(0, 255, 255);
    public static final Color lightCyan = new Color(224, 255, 255);
    public static final Color paleTurquoise = new Color(175, 238, 238);
    public static final Color aquamarine = new Color(127, 255, 212);
    public static final Color turquoise = new Color(64, 224, 208);
    public static final Color mediumTurquoise = new Color(72, 209, 204);
    public static final Color darkTurquoise = new Color(0, 206, 209);
    public static final Color cadetBlue = new Color(95, 158, 160);
    public static final Color richBlue = new Color(89, 89, 171);
    public static final Color steelBlue = new Color(70, 130, 180);
    public static final Color lightSteelBlue = new Color(176, 196, 222);
    public static final Color powderBlue = new Color(176, 224, 230);
    public static final Color lightBlue = new Color(173, 216, 230);
    public static final Color skyBlue = new Color(135, 206, 235);
    public static final Color lightSkyBlue = new Color(135, 206, 250);
    public static final Color deepSkyBlue = new Color(0, 191, 255);
    public static final Color dodgerBlue = new Color(30, 144, 255);
    public static final Color summerSky = new Color(56, 176, 222);
    public static final Color cornflowerBlue = new Color(100, 149, 237);
    public static final Color royalBlue = new Color(65, 105, 225);
    public static final Color mediumSlateBlue = new Color(123, 104, 238);
    public static final Color neonBlue = new Color(77, 77, 255);
    public static final Color blue = new Color(0, 0, 255);
    public static final Color mediumBlue = new Color(0, 0, 205);
    public static final Color newMidnightBlue = new Color(0, 0, 156);
    public static final Color navyBlue = new Color(35, 35, 142);
    public static final Color darkBlue = new Color(0, 0, 139);
    public static final Color navy = new Color(0, 0, 128);
    public static final Color midnightBlue = new Color(25, 25, 112);
    // brown colors
    public static final Color cornsilk = new Color(255, 248, 220);
    public static final Color blanchedAlmond = new Color(255, 235, 205);
    public static final Color bisque = new Color(255, 228, 196);
    public static final Color navajoWhite = new Color(255, 222, 173);
    public static final Color wheat = new Color(245, 222, 179);
    public static final Color newTan = new Color(235, 199, 158);
    public static final Color burlyWood = new Color(222, 184, 135);
    public static final Color mediumWood = new Color(166, 128, 100);
    public static final Color tan = new Color(210, 180, 140);
    public static final Color rosyBrown = new Color(188, 143, 143);
    public static final Color sandyBrown = new Color(244, 164, 96);
    public static final Color goldenrod = new Color(218, 165, 32);
    public static final Color darkGoldenrod = new Color(184, 134, 11);
    public static final Color peru = new Color(205, 133, 63);
    public static final Color chocolate = new Color(210, 105, 30);
    public static final Color darkTan = new Color(151, 105, 79);
    public static final Color saddleBrown = new Color(139, 69, 19);
    public static final Color lightWood = new Color(133, 99, 99);
    public static final Color darkWood = new Color(133, 94, 66);
    public static final Color sienna = new Color(160, 82, 45);
    public static final Color brown = new Color(165, 42, 42);
    public static final Color maroon = new Color(128, 0, 0);
    public static final Color darkBrown = new Color(92, 64, 51);
    // white colors
    public static final Color white = new Color(255, 255, 255);
    public static final Color snow = new Color(255, 250, 250);
    public static final Color honeyDew = new Color(240, 255, 240);
    public static final Color mintCream = new Color(245, 255, 250);
    public static final Color azure = new Color(240, 255, 255);
    public static final Color aliceBlue = new Color(240, 248, 255);
    public static final Color ghostWhite = new Color(248, 248, 255);
    public static final Color whiteSmoke = new Color(245, 245, 245);
    public static final Color seaShell = new Color(255, 245, 238);
    public static final Color beige = new Color(245, 245, 220);
    public static final Color oldLace = new Color(253, 245, 230);
    public static final Color floralWhite = new Color(255, 250, 240);
    public static final Color ivory = new Color(255, 255, 240);
    public static final Color antiqueWhite = new Color(250, 235, 215);
    public static final Color linen = new Color(250, 240, 230);
    public static final Color lavenderBlush = new Color(255, 240, 245);
    public static final Color mistyRose = new Color(255, 228, 225);
    // grey colors
    public static final Color gainsboro = new Color(220, 220, 220);
    public static final Color lightGrey = new Color(211, 211, 211);
    public static final Color lightGray = new Color(211, 211, 211);
    public static final Color silver = new Color(192, 192, 192);
    public static final Color darkGrey = new Color(169, 169, 169);
    public static final Color darkGray = new Color(169, 169, 169);
    public static final Color grey = new Color(128, 128, 128);
    public static final Color gray = new Color(128, 128, 128);
    public static final Color dimGrey = new Color(105, 105, 105);
    public static final Color dimGray = new Color(105, 105, 105);
    public static final Color lightSlateGrey = new Color(119, 136, 153);
    public static final Color lightSlateGray = new Color(119, 136, 153);
    public static final Color slateGrey = new Color(112, 128, 144);
    public static final Color slateGray = new Color(112, 128, 144);
    public static final Color darkSlateGrey = new Color(47, 79, 79);
    public static final Color darkSlateGray = new Color(47, 79, 79);
    public static final Color black = new Color(0, 0, 0);
}
