package jzrlib.external.markovchains;

import java.util.ArrayList;
import java.util.Collections;
import java.util.HashMap;
import java.util.Iterator;
import java.util.List;
import java.util.Map;
import java.util.Random;

/**
 * A random text generation using Markov Chains.
 *
 * @author Serg V. Zhdanovskih
 */
public class MarkovChain<T>
{
    /**
     * Order of Markov Chain -- How many states does the next depend on?
     */
    private final int order;

    /**
     * Constant for empty items.
     */
    private final T emptyItem;

    /**
     * Tuple used for keys in table.
     */
    private class ChainKey
    {
        private List<T> items = new ArrayList<>(order);

        public ChainKey()
        {
            items = Collections.nCopies(order, emptyItem);
        }

        private ChainKey(ChainKey key, T item)
        {
            if (order > 0) {
                items.addAll(key.items);
                items.set(0, item);
                Collections.rotate(items, -1);
            }
        }

        public ChainKey shift(T item)
        {
            return new ChainKey(this, item);
        }

        @Override
        public boolean equals(Object obj)
        {
            try {
                @SuppressWarnings("unchecked")
                ChainKey other = (ChainKey) obj;
                return items.equals(other.items);
            } catch (ClassCastException ex) {
                return false;
            }
        }

        @Override
        public int hashCode()
        {
            return items.hashCode();
        }

        @Override
        public String toString()
        {
            return "ChainKey:" + items.toString();
        }
    }

    /**
     * Contains information about possible suffixes for given prefix.
     */
    private class SuffixInfo
    {
        private final List<T> suffixes = new ArrayList<>();

        /**
         * Add a suffix to the info.
         *
         * @param item The item to add.
         */
        public void add(T item)
        {
            suffixes.add(item);
        }

        /**
         * Pick a random suffix.
         *
         * @param rand RNG for random choice.
         * @return Chosen suffix.
         */
        public T pickRandomItem(Random rand)
        {
            int index = rand.nextInt(suffixes.size());
            return suffixes.get(index);
        }
    }

    /**
     * Table of prefixes to possible suffixes
     */
    private Map<ChainKey, SuffixInfo> chain = new HashMap<>();

    public MarkovChain(int order, T emptyItem)
    {
        this.order = order;
        this.emptyItem = emptyItem;
    }

    /**
     * Add prefix/suffix association to the map.
     *
     * @param key The prefix
     * @param item Associated suffix
     */
    private void addItem(ChainKey key, T item)
    {
        SuffixInfo nextItems = chain.get(key);
        if (nextItems == null) {
            nextItems = new SuffixInfo();
            chain.put(key, nextItems);
        }
        nextItems.add(item);
    }

    /**
     * Add a sequence of items to the chain.
     *
     * @param iter Iterator over sequence of items.
     */
    public void addItems(final Iterator<T> iter)
    {
        // Start with an empty key.
        ChainKey key = new ChainKey();
        while (iter.hasNext()) {
            T item = iter.next();
            addItem(key, item);

            // Create next key
            key = key.shift(item);
        }

        // Pad with empty items to loop back to beginning.
        for (int i = 0; i < order; ++i) {
            addItem(key, emptyItem);
            key = key.shift(emptyItem);
        }
    }

    /**
     * Add a sequence of items to the chain.
     *
     * @param it Iterable sequence of items.
     */
    public void addItems(Iterable<T> it)
    {
        addItems(it.iterator());
    }

    /**
     * Use Markov chain to generate sequence of items.
     *
     * @param n of items to generate. (May include empty)
     * @param rand Random number generator to pick next item.
     * @return List containing generated sequence.
     */
    public List<T> generate(int n, Random rand)
    {
        //System.out.println("chain = " + chain);
        List<T> result = new ArrayList<>(n);
        ChainKey key = new ChainKey();
        for (int i = 0; i < n; ++i) {
            SuffixInfo suffixInfo = chain.get(key);
            T item = suffixInfo.pickRandomItem(rand);
            result.add(item);
            key = key.shift(item);
        }
        return result;
    }
}
