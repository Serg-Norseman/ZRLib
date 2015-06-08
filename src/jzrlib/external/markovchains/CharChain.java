package jzrlib.external.markovchains;

import java.util.ArrayList;
import java.util.List;

/**
 *
 * @author Serg V. Zhdanovskih
 */
public final class CharChain extends MarkovChain<Character>
{
    public CharChain(int order)
    {
        super(order, ' ');
    }

    public final void init(String data)
    {
        List<Character> chars = new ArrayList<>();
        for (char c : data.toCharArray()) {
            chars.add(c);
        }

        this.addItems(chars);
    }
}
