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
package jzrlib.java;

/**
 *
 * @author Serg V. Zhdanovskih
 */
public class SmartPriorityQueue<T extends Comparable>
{
    private final class Node
    {
        private final T value;
        private Node next;

        public Node(T value, Node next)
        {
            this.value = value;
            this.next = next;
        }
    }

    private final int maxCapacity;
    private Node head;
    private int size;

    public SmartPriorityQueue(int maxCapacity)
    {
        this.maxCapacity = maxCapacity;
        this.head = null;
        this.size = 0;
    }

    public final boolean add(T object)
    {
        if (this.isFull()) {
            return false;
        }

        if (this.head == null) {
            this.head = new Node(object, null);
        } else if (object.compareTo(this.head.value) < 0) {
            this.head = new Node(object, this.head);
        } else {
            Node p = this.head;
            while (p.next != null && object.compareTo(p.next.value) >= 0) {
                p = p.next; //or equal to preserve FIFO on equal items
            }
            p.next = new Node(object, p.next);
        }
        ++this.size;
        return true;
    }

    public final T peek()
    {
        if (this.isEmpty()) {
            return null;
        } else {
            return this.head.value;
        }
    }

    public final T poll()
    {
        if (this.isEmpty()) {
            return null;
        }

        T value = this.head.value;
        this.head = this.head.next;
        --this.size;
        return value;
    }

    public final int size()
    {
        return this.size;
    }

    public final boolean contains(T object)
    {
        Node p = this.head;
        while (p != null) {
            if (((Comparable<T>) object).compareTo(p.value) == 0) {
                return true;
            }
            p = p.next;
        }
        return false;
    }

    public final void clear()
    {
        this.head = null;
        this.size = 0;
    }

    public final boolean isEmpty()
    {
        return (this.size == 0);
    }

    public final boolean isFull()
    {
        return (this.size == this.maxCapacity);
    }
}
