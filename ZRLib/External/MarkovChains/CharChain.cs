using System;
using System.Collections.Generic;

namespace ZRLib.External.MarkovChains
{
    public sealed class CharChain : MarkovChain<Char>
    {
        public CharChain(int order)
            : base(order, ' ')
        {
        }

        public void Init(string data)
        {
            IList<Char> chars = new List<Char>();
            foreach (char c in data) {
                chars.Add(c);
            }

            AddItems(chars);
        }
    }
}
