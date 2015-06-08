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
 */
public abstract class FlagSet
{
    private static final byte BITS_SIZE = 32;
    
    private int fValue;

    public FlagSet()
    {
        this.fValue = 0;
    }

    public FlagSet(FlagSet set)
    {
        this.fValue = set.fValue;
    }

    public FlagSet(int... args)
    {
        this.fValue = 0;
        this.include(args);
    }

    public final void clear()
    {
        this.fValue = 0;
    }

    public final boolean isEmpty()
    {
        return (this.fValue == 0);
    }

    public final int getValue()
    {
        return this.fValue;
    }

    public final void setValue(int value)
    {
        this.fValue = value;
    }

    public final void include(int... e)
    {
        for (int i = 0; i < e.length; i++) {
            this.include(e[i]);
        }
    }

    public final void include(int elem)
    {
        this.fValue = TypeUtils.setBit(this.fValue, elem, 1);
    }

    public final void exclude(int elem)
    {
        this.fValue = TypeUtils.setBit(this.fValue, elem, 0);
    }

    public final boolean contains(int elem)
    {
        return TypeUtils.hasBit(this.fValue, elem);
    }

    public final boolean containsAll(int... e)
    {
        for (int i = 0; i < e.length; i++) {
            if (!this.contains(e[i])) {
                return false;
            }
        }
        return true;
    }

    public final boolean hasIntersect(FlagSet es)
    {
        return ((this.fValue & es.fValue) > 0);
    }

    public final boolean hasIntersect(int... e)
    {
        for (int i = 0; i < e.length; i++) {
            if (this.contains(e[i])) {
                return true;
            }
        }
        return false;
    }

    public final void add(FlagSet right)
    {
        this.fValue |= right.fValue; // result.data[i] |= Right.data[i];
    }

    public final void sub(FlagSet right)
    {
        this.fValue &= (~right.fValue); // result.data[i] = (byte)(result.data[i] & (~Right.data[i]));
    }

    public final void mul(FlagSet right)
    {
        this.fValue &= right.fValue; // result.data[i] &= Right.data[i];
    }

    @Override
    public boolean equals(Object obj)
    {
        if (!(obj instanceof FlagSet)) {
            return false;
        }
        if (this == obj) {
            return true;
        }

        FlagSet set = (FlagSet) obj;
        return (this.fValue == set.fValue);
    }

    @Override
    public int hashCode()
    {
        int hash = 5;
        hash = 59 * hash + Integer.hashCode(this.fValue);
        return hash;
    }

    @Override
    public String toString()
    {
        StringBuilder str = new StringBuilder(32);
        str.append('{');
        for (int i = 31; i >= 0; i--) {
            str.append(this.contains(i) ? "1" : "0");
        }
        str.append('}');

        String signs = this.getSignature();
        if (signs.length() > 0) {
            signs = ", " + signs;
        }

        return "[Value=" + str.toString() + signs + "]";
    }

    /*@Override
    public FlagSet clone()
    {
        FlagSet result = new FlagSet();
        result.fValue = this.fValue;
        return result;
    }*/

    public FlagSet(String signature)
    {
        this();
        this.setSignature(signature);
    }

    protected String[] getSignaturesOrder()
    {
        return new String[0];
    }

    public final String getSignature()
    {
        String[] signatures = getSignaturesOrder();
        
        if (signatures == null) {
            return "";
        }
        
        StringBuilder res = new StringBuilder();

        for (int i = 0; i < BITS_SIZE; i++) {
            if (this.contains(i)) {
                if (res.length() != 0) {
                    res.append(",");
                }

                if (i < signatures.length) {
                    res.append(signatures[i]);
                }
            }
        }

        return res.toString();
    }

    public final void setSignature(String value)
    {
        this.clear();

        String[] signatures = getSignaturesOrder();
        
        if (signatures == null) {
            return;
        }

        for (int i = 0; i < BITS_SIZE; i++) {
            if (i < signatures.length) {
                String sign = signatures[i];
                if (value.contains(sign)) {
                    this.include(i);
                }
            } else {
                break;
            }
        }
    }

    protected static int valueOf(String sign, String[] signatures)
    {
        if (signatures != null) {
            for (int i = 0; i < signatures.length; i++) {
                if (signatures[i].equals(sign)) {
                    return i;
                }
            }
        }

        return -1;
    }

    public final int[] getArray()
    {
        int size = 0;
        for (int i = 0; i < BITS_SIZE; i++) {
            if (this.contains(i)) {
                size++;
            }
        }

        int[] result = new int[size];
        
        int idx = 0;
        for (int i = 0; i < BITS_SIZE; i++) {
            if (this.contains(i)) {
                result[idx] = i;
                idx++;
            }
        }

        return result;
    }
}
