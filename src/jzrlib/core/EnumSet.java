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

import jzrlib.utils.TypeUtils;

/**
 *
 * @author Serg V. Zhdanovskih
 * @param <T>
 */
public abstract class EnumSet<T extends Enum>/* implements Cloneable*/
{
    private static final byte BITS_SIZE = 32;
    
    private int fValue;

    public EnumSet()
    {
        this.fValue = 0;
    }

    public EnumSet(EnumSet set)
    {
        this.fValue = set.fValue;
    }

    public EnumSet(T... args)
    {
        this.fValue = 0;
        this.include(args);
    }

    public EnumSet(int value)
    {
        this.setValue(value);
    }

    public EnumSet(String signature)
    {
        this.fValue = 0;
        this.setSignature(signature);
    }

    public final int getValue()
    {
        return this.fValue;
    }

    public final void setValue(int value)
    {
        this.fValue = value;
    }

    public final void clear()
    {
        this.fValue = 0;
    }

    public final boolean isEmpty()
    {
        return (this.fValue == 0);
    }

    public String getSignature()
    {
        return "";
    }

    public void setSignature(String value)
    {
    }

    private int getBitIndex(T elem)
    {
        int idx;
        if (elem instanceof IExtEnum) {
            idx = ((IExtEnum) elem).getValue();
        } else {
            idx = elem.ordinal();
        }
        return idx;
    }

    public final void include(T... e)
    {
        for (int i = 0; i < e.length; i++) {
            this.include(e[i]);
        }
    }

    public final void include(T elem)
    {
        int bit = this.getBitIndex(elem);
        this.fValue = TypeUtils.setBit(this.fValue, bit, 1);
    }

    public final void exclude(T elem)
    {
        int bit = this.getBitIndex(elem);
        this.fValue = TypeUtils.setBit(this.fValue, bit, 0);
    }

    public final boolean contains(T elem)
    {
        int bit = this.getBitIndex(elem);
        return TypeUtils.hasBit(this.fValue, bit);
    }

    public final boolean containsAll(T... e)
    {
        for (int i = 0; i < e.length; i++) {
            if (!this.contains(e[i])) {
                return false;
            }
        }
        return true;
    }

    public final boolean hasIntersect(EnumSet<T> es)
    {
        return ((this.fValue & es.fValue) > 0);
    }

    public final boolean hasIntersect(T... e)
    {
        for (int i = 0; i < e.length; i++) {
            if (this.contains(e[i])) {
                return true;
            }
        }
        return false;
    }

    @Override
    public boolean equals(Object obj)
    {
        if (!(obj instanceof EnumSet)) {
            return false;
        }
        if (this == obj) {
            return true;
        }

        EnumSet set = (EnumSet) obj;
        return (this.fValue == set.fValue);
    }

    @Override
    public int hashCode()
    {
        int hash = 5;
        hash = 59 * hash + Integer.hashCode(this.fValue);
        return hash;
    }

    public final void add(EnumSet<T> right)
    {
        this.fValue |= right.fValue; // result.data[i] |= Right.data[i];
    }

    public final void sub(EnumSet<T> right)
    {
        this.fValue &= (~right.fValue); // result.data[i] = (byte)(result.data[i] & (~Right.data[i]));
    }

    public final void mul(EnumSet<T> right)
    {
        this.fValue &= right.fValue; // result.data[i] &= Right.data[i];
    }

    /*@Override
    public EnumSet<T> clone()
    {
        EnumSet<T> result = new EnumSet<>();
        result.fValue = this.fValue;
        return result;
    }*/
}
