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
package jzrlib.core;

import java.util.ArrayList;
import jzrlib.utils.AuxUtils;

/**
 *
 * @author Serg V. Zhdanovskih
 */
public final class ProbabilityTable<T>
{
    private static final class Pair<T>
    {
        public T value;
        public int weight;
        
        public Pair(T value, int weight)
        {
            this.value = value;
            this.weight = weight;
        }
    }

    private final ArrayList<Pair<T>> fTable;
    private int fTotal;

    public ProbabilityTable()
    {
        this.fTable = new ArrayList();
        this.fTotal = 0;
    }

    public final int size()
    {
        return this.fTable.size();
    }

    public final void add(T item, int weight)
    {
        this.fTable.add(new Pair(item, weight));
        this.fTotal += weight;
    }

    public final T getRandomItem()
    {
        if (this.fTable.isEmpty()) {
            return null;
        }

        int index = AuxUtils.getRandom(this.fTotal);

        for (Pair<T> pair : this.fTable) {
            index -= (pair.weight);
            if (index < 0) {
                return pair.value;
            }
        }

        return null;
    }
}
