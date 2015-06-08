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

/**
 *
 * @author Serg V. Zhdanovskih
 */
public class ExtList<T> extends BaseObject
{
    public enum ListNotification
    {
        lnAdded,
        lnExtracted,
        lnDeleted;
    }

    private ArrayList<T> fList;

    public boolean OwnsObjects;

    public ExtList()
    {
        this.fList = new ArrayList<>();
        this.OwnsObjects = false;
    }

    public ExtList(boolean ownsObjects)
    {
        this.fList = new ArrayList<>();
        this.OwnsObjects = ownsObjects;
    }

    @Override
    protected void dispose(boolean disposing)
    {
        if (disposing) {
            this.clear();
            this.fList = null;
        }
        super.dispose(disposing);
    }

    public int getCount()
    {
        return this.fList.size();
    }

    public T get(int index)
    {
        return this.fList.get(index);
    }

    public void set(int index, T value)
    {
        if (index < 0 || index >= this.getCount()) {
            throw new ListException("List index out of bounds ({0})", index);
        }

        T temp = this.fList.get(index);
        if (value != temp) {
            this.fList.set(index, value);

            if (temp != null) {
                this.notify(temp, ListNotification.lnDeleted);
            }

            if (value != null) {
                this.notify(value, ListNotification.lnAdded);
            }
        }
    }

    private void notify(Object instance, ListNotification action)
    {
        if (this.OwnsObjects && action == ListNotification.lnDeleted) {
            if (instance != null && instance instanceof BaseObject) {
                ((BaseObject) instance).dispose();
            }
        }
    }

    public int add(T item)
    {
        int result = this.fList.size();
        this.fList.add(item);
        if (item != null) {
            this.notify(item, ListNotification.lnAdded);
        }
        return result;
    }

    public void clear()
    {
        for (int i = this.fList.size() - 1; i >= 0; i--) {
            this.notify(fList.get(i), ListNotification.lnDeleted);
        }
        this.fList.clear();
    }

    public void delete(int index)
    {
        Object temp = this.fList.get(index);

        this.fList.remove(index);

        if (temp != null) {
            this.notify(temp, ListNotification.lnDeleted);
        }
    }

    public void exchange(int index1, int index2)
    {
        T item = this.fList.get(index1);
        this.fList.set(index1, this.fList.get(index2));
        this.fList.set(index2, item);
    }

    public Object extract(T item)
    {
        Object result = null;
        int I = this.indexOf(item);
        if (I >= 0) {
            result = item;
            this.fList.remove(I);
            this.notify(result, ListNotification.lnExtracted);
        }
        return result;
    }

    public int indexOf(T item)
    {
        return this.fList.indexOf(item);
    }

    public void insert(int index, T item)
    {
        this.fList.add(index, item);
        if (item != null) {
            this.notify(item, ListNotification.lnAdded);
        }
    }

    public int remove(T item)
    {
        int result = this.indexOf(item);
        if (result >= 0) {
            this.delete(result);
        }
        return result;
    }

    public void pack()
    {
        for (int I = this.getCount() - 1; I >= 0; I--) {
            if (this.get(I) == null) {
                this.delete(I);
            }
        }
    }

    /*private void QuickSort(TListSortCompare comparer, int L, int R)
    {
            int I;
            do
            {
                    I = L;
                    int J = R;
                    object P = fList[(int)((uint)(L + R) >> 1)];
                    while (true)
                    {
        if (comparer(fList[I], P) >= 0)
                            {
            while (comparer(fList[J], P) > 0) J--;

                                    if (I <= J)
                                    {
                                            T tmp = fList[I];
                                            fList[I] = fList[J];
                                            fList[J] = tmp;

                                            I++;
                                            J--;
                                    }

                                    if (I > J)
                                    {
                                            break;
                                    }
                            }
                            else
                            {
                                    I++;
                            }
                    }
                    if (L < J) QuickSort(comparer, L, J);
                    L = I;
            }
            while (I < R);
    }

    public void QuickSort(TListSortCompare comparer)
    {
        if (this.getCount() > 0) {
            QuickSort(comparer, 0, this.getCount() - 1);
        }
    }

    public void MergeSort(TListSortCompare comparer)
    {
            MergeSort(new T[fList.Count], 0, fList.Count - 1, comparer);
    }

    private void MergeSort(T[] tmp, int left, int right, TListSortCompare comparer)
    {
            if (left >= right) return;

            int mid = (left + right) / 2;
            MergeSort(tmp, left, mid, comparer);
            MergeSort(tmp, mid + 1, right, comparer);

            int i = left, j = mid + 1, k = left;

            while (i <= mid && j <= right)
            {
                    if (comparer(fList[i], fList[j]) < 0)
                    {
                            tmp[k++] = fList[i++];
                    }
                    else
                    {
                            tmp[k++] = fList[j++];
                    }
            }
            while (i <= mid) tmp[k++] = fList[i++];
            while (j <= right) tmp[k++] = fList[j++];
            for (i = left; i <= right; ++i) fList[i] = tmp[i];
    }*/
}
