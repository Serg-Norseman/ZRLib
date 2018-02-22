using System;
using System.Collections.Generic;
using ZRLib.Core;

namespace ZRLib.External.MarkovChains
{
    /// <summary>
    /// A random text generation using Markov Chains.
    /// </summary>
    public class MarkovChain<T>
    {
        /// <summary>
        /// fOrder of Markov Chain -- How many states does the next depend on?
        /// </summary>
        private readonly int fOrder;

        /// <summary>
        /// Constant for empty items.
        /// </summary>
        private readonly T fEmptyItem;

        /// <summary>
        /// Tuple used for keys in table.
        /// </summary>
        private class ChainKey
        {
            private readonly MarkovChain<T> OuterInstance;
            private readonly IList<T> fItems;

            public ChainKey(MarkovChain<T> outerInstance)
            {
                OuterInstance = outerInstance;

                fItems = new List<T>(OuterInstance.fOrder);
                for (int i = 0; i < OuterInstance.fOrder; i++) {
                    fItems.Add(outerInstance.fEmptyItem);
                }
            }

            internal ChainKey(MarkovChain<T> outerInstance, ChainKey key, T item)
            {
                OuterInstance = outerInstance;

                fItems = new List<T>(OuterInstance.fOrder);
                if (outerInstance.fOrder > 0) {
                    fItems.AddRange(key.fItems);
                    fItems[0] = item;
                    fItems.Rotate(-1);
                }
            }

            public ChainKey Shift(T item)
            {
                return new ChainKey(OuterInstance, this, item);
            }

            public override bool Equals(object obj)
            {
                try {
                    ChainKey other = (ChainKey)obj;
                    return fItems.Equals(other.fItems);
                } catch (InvalidCastException) {
                    return false;
                }
            }

            public override int GetHashCode()
            {
                return fItems.GetHashCode();
            }

            public override string ToString()
            {
                return "ChainKey:" + fItems.ToString();
            }
        }

        /// <summary>
        /// Contains information about possible suffixes for given prefix.
        /// </summary>
        private class SuffixInfo
        {
            private readonly IList<T> fSuffixes = new List<T>();

            /// <summary>
            /// Add a suffix to the info.
            /// </summary>
            /// <param name="item"> The item to add. </param>
            public void Add(T item)
            {
                fSuffixes.Add(item);
            }

            /// <summary>
            /// Pick a random suffix.
            /// </summary>
            /// <param name="rand"> RNG for random choice. </param>
            /// <returns> Chosen suffix. </returns>
            public T PickRandomItem(Random rand)
            {
                int index = rand.Next(fSuffixes.Count);
                return fSuffixes[index];
            }
        }

        /// <summary>
        /// Table of prefixes to possible suffixes
        /// </summary>
        private IDictionary<ChainKey, SuffixInfo> Chain = new Dictionary<ChainKey, SuffixInfo>();

        public MarkovChain(int order, T emptyItem)
        {
            fOrder = order;
            fEmptyItem = emptyItem;
        }

        /// <summary>
        /// Add prefix/suffix association to the map.
        /// </summary>
        /// <param name="key"> The prefix </param>
        /// <param name="item"> Associated suffix </param>
        private void AddItem(ChainKey key, T item)
        {
            SuffixInfo nextItems = Chain.GetValueOrNull(key);
            if (nextItems == null) {
                nextItems = new SuffixInfo();
                Chain[key] = nextItems;
            }
            nextItems.Add(item);
        }

        /// <summary>
        /// Add a sequence of items to the chain.
        /// </summary>
        /// <param name="iter"> Iterator over sequence of items. </param>
        public virtual void AddItems(IEnumerator<T> iter)
        {
            // Start with an empty key.
            ChainKey key = new ChainKey(this);
            while (iter.MoveNext()) {
                T item = iter.Current;
                AddItem(key, item);

                // Create next key
                key = key.Shift(item);
            }

            // Pad with empty items to loop back to beginning.
            for (int i = 0; i < fOrder; ++i) {
                AddItem(key, fEmptyItem);
                key = key.Shift(fEmptyItem);
            }
        }

        /// <summary>
        /// Add a sequence of items to the chain.
        /// </summary>
        /// <param name="it"> Iterable sequence of items. </param>
        public virtual void AddItems(IEnumerable<T> it)
        {
            AddItems(it.GetEnumerator());
        }

        /// <summary>
        /// Use Markov chain to generate sequence of items.
        /// </summary>
        /// <param name="n"> of items to generate. (May include empty) </param>
        /// <param name="rand"> Random number generator to pick next item. </param>
        /// <returns> List containing generated sequence. </returns>
        public virtual IList<T> Generate(int n, Random rand)
        {
            //System.out.println("chain = " + chain);
            IList<T> result = new List<T>(n);
            ChainKey key = new ChainKey(this);
            for (int i = 0; i < n; ++i) {
                SuffixInfo suffixInfo = Chain.GetValueOrNull(key);
                T item = suffixInfo.PickRandomItem(rand);
                result.Add(item);
                key = key.Shift(item);
            }
            return result;
        }
    }
}
