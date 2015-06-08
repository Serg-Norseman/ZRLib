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
import jzrlib.utils.RefObject;
import jzrlib.utils.TextUtils;

/**
 *
 * @author Serg V. Zhdanovskih
 */
public class StringList extends BaseObject
{
    private final static class StringItem
    {
        public String Value;
        public Object Data;

        public StringItem()
        {
        }

        public StringItem(String value, Object data)
        {
            this.Value = value;
            this.Data = data;
        }

        @Override
        public StringItem clone()
        {
            return new StringItem(this.Value, this.Data);
        }
    }

    public enum TDuplicates
    {
        dupIgnore,
        dupAccept,
        dupError;
    }

    private ArrayList<StringItem> fList;
    private boolean fCaseSensitive;
    private boolean fSorted;
    private int fUpdateCount;

    private static final String LineBreak = "\r\n";
    private static final String OutOfBounds = "List index out of bounds ({0})";

    public INotifyEvent OnChange;
    public INotifyEvent OnChanging;

    public final int getCount()
    {
        return this.fList.size();
    }

    public final String get(int index)
    {
        if (index < 0 || index >= this.fList.size()) {
            throw new StringListException(OutOfBounds, index);
        }

        return this.fList.get(index).Value;
    }

    public final void set(int index, String value)
    {
        if (this.fSorted) {
            throw new StringListException("Operation not allowed on sorted list", 0);
        }
        if (index < 0 || index >= this.fList.size()) {
            throw new StringListException(OutOfBounds, index);
        }

        this.changing();
        StringItem item = this.fList.get(index);
        item.Value = value;
        this.changed();
    }

    public StringList()
    {
        this.fList = new ArrayList<>();
    }

    public StringList(String str)
    {
        this();
        this.setTextStr(str);
    }

    public StringList(String[] list)
    {
        this();
        for (int i = 0; i < list.length; i++) {
            this.addObject(list[i], null);
        }
    }

    @Override
    protected void dispose(boolean disposing)
    {
        if (disposing) {
            this.fList = null;
        }
        super.dispose(disposing);
    }

    public TDuplicates Duplicates = TDuplicates.values()[0];

    public final boolean getSorted()
    {
        return this.fSorted;
    }

    public final void setSorted(boolean value)
    {
        if (this.fSorted != value) {
            if (value) {
                this.sort();
            }
            this.fSorted = value;
        }
    }

    public final boolean getCaseSensitive()
    {
        {
            return this.fCaseSensitive;
        }
    }

    public final void setCaseSensitive(boolean value)
    {
        if (value != this.fCaseSensitive) {
            this.fCaseSensitive = value;
            if (this.fSorted) {
                this.sort();
            }
        }
    }

    public final Object getObject(int index)
    {
        if (index < 0 || index >= this.fList.size()) {
            throw new StringListException(OutOfBounds, index);
        }
        return this.fList.get(index).Data;
    }

    public final void setObject(int index, Object obj)
    {
        if (index < 0 || index >= this.fList.size()) {
            throw new StringListException(OutOfBounds, index);
        }
        this.changing();
        StringItem item = this.fList.get(index).clone();
        item.Data = obj;
        this.fList.set(index, item.clone());
        this.changed();
    }

    public final String getTextStr()
    {
        StringBuilder buffer = new StringBuilder();

        int num = this.fList.size() - 1;
        for (int i = 0; i <= num; i++) {
            buffer.append(this.fList.get(i).Value);

            if (i < num) {
                buffer.append(StringList.LineBreak);
            }
        }

        return buffer.toString();
    }

    public final void setTextStr(String value)
    {
        this.beginUpdate();
        try {
            this.clear();

            if (!TextUtils.isNullOrEmpty(value)) {
                int Start = 0;
                int L = StringList.LineBreak.length();
                int P = value.indexOf(StringList.LineBreak);
                if (P >= 0) {
                    do {
                        String s = value.substring(Start, P);
                        this.add(s);
                        Start = P + L;
                        P = value.indexOf(StringList.LineBreak, Start);
                    } while (P >= 0);
                }

                if (Start <= value.length()) {
                    String s = value.substring(Start, (value.length()));
                    this.add(s);
                }
            }
        } finally {
            this.endUpdate();
        }
    }

    public final int add(String str)
    {
        return this.addObject(str, null);
    }

    public final int addObject(String str, Object obj)
    {
        int result = -1;

        if (!this.fSorted) {
            result = this.fList.size();
        } else {
            RefObject<Integer> tempRef_result = new RefObject<>(result);
            boolean exists = this.find(str, tempRef_result);
            result = tempRef_result.argValue;

            if (exists) {
                StringList.TDuplicates duplicates = this.Duplicates;
                if (duplicates == StringList.TDuplicates.dupIgnore) {
                    return result;
                }
                if (duplicates == StringList.TDuplicates.dupError) {
                    throw new StringListException("String list does not allow duplicates", 0);
                }
            }
        }
        this.insertItem(result, str, obj);

        return result;
    }

    public final void addStrings(StringList strList)
    {
        if (strList == null) {
            return;
        }

        this.beginUpdate();
        try {
            int num = strList.getCount();
            for (int i = 0; i < num; i++) {
                this.addObject(strList.get(i), strList.getObject(i));
            }
        } finally {
            this.endUpdate();
        }
    }

    public final void assign(StringList source)
    {
        if (source != null) {
            this.beginUpdate();
            try {
                this.clear();
                this.addStrings(source);
            } finally {
                this.endUpdate();
            }
        }
    }

    public final void clear()
    {
        if (!this.fList.isEmpty()) {
            this.changing();
            this.fList.clear();
            this.changed();
        }
    }

    public final void delete(int index)
    {
        if (index < 0 || index >= this.fList.size()) {
            throw new StringListException(OutOfBounds, index);
        }

        this.changing();
        if (index < this.fList.size()) {
            this.fList.remove(index);
        }
        this.changed();
    }

    public final void exchange(int index1, int index2)
    {
        if (index1 < 0 || index1 >= this.fList.size()) {
            throw new StringListException(OutOfBounds, index1);
        }
        if (index2 < 0 || index2 >= this.fList.size()) {
            throw new StringListException(OutOfBounds, index2);
        }

        this.changing();
        this.exchangeItems(index1, index2);
        this.changed();
    }

    public final void insert(int index, String str)
    {
        this.insertObject(index, str, null);
    }

    public final void insertObject(int index, String str, Object obj)
    {
        if (this.fSorted) {
            throw new StringListException("Operation not allowed on sorted list", 0);
        }
        if (index < 0 || index > this.getCount()) {
            throw new StringListException(OutOfBounds, index);
        }
        this.insertItem(index, str, obj);
    }

    private void insertItem(int index, String str, Object obj)
    {
        this.changing();
        this.fList.add(index, new StringItem(str, obj));
        this.changed();
    }

    public final void exchangeItems(int index1, int index2)
    {
        StringItem temp = this.fList.get(index1).clone();
        this.fList.set(index1, this.fList.get(index2).clone());
        this.fList.set(index2, temp.clone());
    }

    private void setUpdateState(boolean updating)
    {
        if (updating) {
            this.changing();
        } else {
            this.changed();
        }
    }

    public final void beginUpdate()
    {
        if (this.fUpdateCount == 0) {
            this.setUpdateState(true);
        }
        this.fUpdateCount++;
    }

    public final void endUpdate()
    {
        this.fUpdateCount--;
        if (this.fUpdateCount == 0) {
            this.setUpdateState(false);
        }
    }

    private void changed()
    {
        if (this.fUpdateCount == 0 && this.OnChange != null) {
            this.OnChange.invoke(this);
        }
    }

    private void changing()
    {
        if (this.fUpdateCount == 0 && this.OnChanging != null) {
            this.OnChanging.invoke(this);
        }
    }

    public final int xIndexOf(String str)
    {
        int num = this.fList.size();
        for (int i = 0; i < num; i++) {
            if (this.compareStrings(this.fList.get(i).Value, str) == 0) {
                return i;
            }
        }

        return -1;
    }

    public final int indexOf(String str)
    {
        int result = -1;

        if (!this.fSorted) {
            result = xIndexOf(str);
        } else {
            RefObject<Integer> tempRef_result = new RefObject<>(result);
            boolean res = !this.find(str, tempRef_result);
            result = tempRef_result.argValue;
            if (res) {
                result = -1;
            }
        }

        return result;
    }

    public final int indexOfObject(Object obj)
    {
        int num = this.fList.size();
        for (int i = 0; i < num; i++) {
            if (this.fList.get(i).Data == obj) {
                return i;
            }
        }

        return -1;
    }

    public final boolean find(String str, RefObject<Integer> index)
    {
        boolean result = false;

        int L = 0;
        int H = this.fList.size() - 1;
        if (L <= H) {
            do {
                int I = (int) ((L + H) >>> 1);
                int C = this.compareStrings(this.fList.get(I).Value, str);
                if (C < 0) {
                    L = I + 1;
                } else {
                    H = I - 1;
                    if (C == 0) {
                        result = true;
                        if (this.Duplicates != StringList.TDuplicates.dupAccept) {
                            L = I;
                        }
                    }
                }
            } while (L <= H);
        }
        index.argValue = L;

        return result;
    }

    private void quickSort(int L, int R)
    {
        int I;
        do {
            I = L;
            int J = R;
            int P = (int) ((L + R) >>> 1);
            while (true) {
                if (SCompare(I, P) >= 0) {
                    while (SCompare(J, P) > 0) {
                        J--;
                    }
                    if (I <= J) {
                        this.exchangeItems(I, J);
                        if (P == I) {
                            P = J;
                        } else {
                            if (P == J) {
                                P = I;
                            }
                        }
                        I++;
                        J--;
                    }
                    if (I > J) {
                        break;
                    }
                } else {
                    I++;
                }
            }
            if (L < J) {
                this.quickSort(L, J);
            }
            L = I;
        } while (I < R);
    }

    public final int compareStrings(String s1, String s2)
    {
        //return String.Compare(s1, s2, !this.fCaseSensitive);
        if (this.fCaseSensitive) {
            return s1.compareTo(s2);
        } else {
            return s1.compareToIgnoreCase(s2);
        }
    }

    public final int SCompare(int index1, int index2)
    {
        return compareStrings(fList.get(index1).Value, fList.get(index2).Value);
    }

    public final void sort()
    {
        if (!this.fSorted && this.fList.size() > 1) {
            this.changing();
            this.quickSort(0, this.fList.size() - 1);
            this.changed();
        }
    }

    public final Object[] toArray()
    {
        int num = this.getCount();
        Object[] result = new Object[num];
        for (int i = 0; i < num; i++) {
            result[i] = this.get(i);
        }
        return result;
    }

}
