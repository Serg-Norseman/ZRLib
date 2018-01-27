/*
 *  "ZRLib", Roguelike games development Library.
 *  Copyright (C) 2015 by Serg V. Zhdanovskih.
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

using System.Text;
using BSLib;

namespace ZRLib.Core
{
    // TODO: structure!!!
    public abstract class FlagSet
    {
        private const byte BITS_SIZE = 32;

        private int fValue;

        protected FlagSet()
        {
            fValue = 0;
        }

        protected FlagSet(FlagSet set)
        {
            fValue = set.fValue;
        }

        protected FlagSet(params int[] args)
        {
            fValue = 0;
            Include(args);
        }

        public void Clear()
        {
            fValue = 0;
        }

        public bool Empty
        {
            get {
                return (fValue == 0);
            }
        }

        public int Value
        {
            get {
                return fValue;
            }
            set {
                fValue = value;
            }
        }


        public void Include(params int[] e)
        {
            for (int i = 0; i < e.Length; i++) {
                Include(e[i]);
            }
        }

        public void Include(int elem)
        {
            fValue = BitHelper.SetBit(fValue, elem);
        }

        public void Exclude(int elem)
        {
            fValue = BitHelper.UnsetBit(fValue, elem);
        }

        public bool Contains(int elem)
        {
            return BitHelper.IsSetBit(fValue, elem);
        }

        public bool ContainsAll(params int[] e)
        {
            for (int i = 0; i < e.Length; i++) {
                if (!Contains(e[i])) {
                    return false;
                }
            }
            return true;
        }

        public bool HasIntersect(FlagSet es)
        {
            return ((fValue & es.fValue) > 0);
        }

        public bool HasIntersect(params int[] e)
        {
            for (int i = 0; i < e.Length; i++) {
                if (Contains(e[i])) {
                    return true;
                }
            }
            return false;
        }

        public void Add(FlagSet right)
        {
            fValue |= right.fValue; // result.data[i] |= Right.data[i];
        }

        public void Sub(FlagSet right)
        {
            fValue &= (~right.fValue); // result.data[i] = (byte)(result.data[i] & (~Right.data[i]));
        }

        public void Mul(FlagSet right)
        {
            fValue &= right.fValue; // result.data[i] &= Right.data[i];
        }

        public override bool Equals(object obj)
        {
            if (!(obj is FlagSet)) {
                return false;
            }
            if (this == obj) {
                return true;
            }

            FlagSet set = (FlagSet)obj;
            return (fValue == set.fValue);
        }

        public override int GetHashCode()
        {
            int hash = 5;
            hash = 59 * hash + fValue.GetHashCode();
            return hash;
        }

        public override string ToString()
        {
            StringBuilder str = new StringBuilder(32);
            str.Append('{');
            for (int i = 31; i >= 0; i--) {
                str.Append(Contains(i) ? "1" : "0");
            }
            str.Append('}');

            string signs = Signature;
            if (signs.Length > 0) {
                signs = ", " + signs;
            }

            return "[Value=" + str.ToString() + signs + "]";
        }

        /*@Override
        public FlagSet Clone()
        {
            FlagSet result = new FlagSet();
            result.fValue = fValue;
            return result;
        }*/

        protected FlagSet(string signature)
            : this()
        {
            Signature = signature;
        }

        protected virtual string[] SignaturesOrder
        {
            get {
                return new string[0];
            }
        }

        public string Signature
        {
            get {
                string[] signatures = SignaturesOrder;
    
                if (signatures == null) {
                    return "";
                }
    
                StringBuilder res = new StringBuilder();
    
                for (int i = 0; i < BITS_SIZE; i++) {
                    if (Contains(i)) {
                        if (res.Length != 0) {
                            res.Append(",");
                        }
    
                        if (i < signatures.Length) {
                            res.Append(signatures[i]);
                        }
                    }
                }
    
                return res.ToString();
            }
            set {
                Clear();
    
                string[] signatures = SignaturesOrder;
    
                if (signatures == null) {
                    return;
                }
    
                for (int i = 0; i < BITS_SIZE; i++) {
                    if (i < signatures.Length) {
                        string sign = signatures[i];
                        if (value.Contains(sign)) {
                            Include(i);
                        }
                    } else {
                        break;
                    }
                }
            }
        }


        protected static int ValueOf(string sign, string[] signatures)
        {
            if (signatures != null) {
                for (int i = 0; i < signatures.Length; i++) {
                    if (signatures[i].Equals(sign)) {
                        return i;
                    }
                }
            }

            return -1;
        }

        public int[] Array
        {
            get {
                int size = 0;
                for (int i = 0; i < BITS_SIZE; i++) {
                    if (Contains(i)) {
                        size++;
                    }
                }
    
                int[] result = new int[size];
    
                int idx = 0;
                for (int i = 0; i < BITS_SIZE; i++) {
                    if (Contains(i)) {
                        result[idx] = i;
                        idx++;
                    }
                }
    
                return result;
            }
        }
    }
}
