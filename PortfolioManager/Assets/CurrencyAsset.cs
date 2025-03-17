namespace PortfolioManager.Assets
{
    public class CurrencyAsset : IAsset
    {
        public double Amount { get; }
        public string Currency { get; }
        public CurrencyAsset(string currency, double amount)
        {
            Currency = currency;
            Amount = amount;
        }
        public double Value()
        {
            return Amount;
        }
    }
}