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
package jzrlib.sandbox;

/**
 * The Dice class implements a set of polyhedral dice, as used in many RPG.
 * It accepts dice with any positive number of sides.
 *
 * @author	mdriesen
 * @author Serg V. Zhdanovskih
 */
public final class Dice
{
    /**
     * Returns the result of a dice roll. The parameters <code>number</code> and
     * <code>dice</code> should be positive, <code>mod</code> can be any
     * integer. If <code>number</code> or <code>dice</code> are not positive,
     * this method returns <code>mod</code>.
     *
     * @param number	the amount of dice to roll
     * @param dice	the type of dice to roll
     * @param mod	the modifier applied to the result of the roll
     * @return	the result of (number)d(dice)+(mod)
     */
    public static int roll(int number, int dice, int mod)
    {
        if (number < 1 || dice < 1) {
            return mod;
        }

        int result = 0;
        for (int i = 0; i < number; i++) {
            result += ((int) (dice * Math.random() + 1));
        }
        return result + mod;
    }

    /**
     * Returns the result of a dice roll. The input string has the form 'xdy',
     * 'xdy+z' or 'xdy-z', with x, y and z positive integers.
     *
     * @param roll	the string representation of the roll
     * @return	the result of the roll
     * @throws NumberFormatException	if the string contains an unparsable part
     */
    public static int roll(String roll)
    {
        int index1 = roll.indexOf('d');
        int index2 = roll.indexOf('+');
        int index3 = roll.indexOf('-');
        int number = Integer.parseInt(roll.substring(0, index1));
        int mod = 0;

        int dice;
        if (index2 > 0) {
            dice = Integer.parseInt(roll.substring(index1 + 1, index2));
            mod = Integer.parseInt(roll.substring(index2 + 1, roll.length()));
        } else if (index3 > 0) {
            dice = Integer.parseInt(roll.substring(index1 + 1, index3));
            mod = -Integer.parseInt(roll.substring(index3 + 1, roll.length()));
        } else {
            dice = Integer.parseInt(roll.substring(index1 + 1, roll.length()));
        }

        return roll(number, dice, mod);
    }
}
