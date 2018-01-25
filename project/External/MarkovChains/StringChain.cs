namespace ZRLib.External.MarkovChains
{
    public sealed class StringChain : MarkovChain<string>
    {
        public StringChain(int order)
            : base(order, "")
        {
        }
    }
}
