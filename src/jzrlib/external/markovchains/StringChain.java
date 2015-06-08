package jzrlib.external.markovchains;

/**
 *
 * @author Serg V. Zhdanovskih
 */
public final class StringChain extends MarkovChain<String>
{
    public StringChain(int order)
    {
        super(order, "");
    }
}
