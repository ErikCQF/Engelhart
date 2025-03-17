
namespace PortfolioManager.Assets
{
    public class Stock : IShare
    {
        public Stock(string symbol, double shares, double price)
        {
            Symbol = symbol;
            Shares = shares;
            Price = price;
        }

        public Stock(string symbol, double shares, double price, string currency)
        {
            Symbol = symbol;
            Shares = shares;
            Price = price;
            Currency = currency;
        }

        public string Symbol { get; set; } = string.Empty;
        public double Shares { get; set; }
        public double Price { get; set; }
        public string Currency { get; set; } = string.Empty;

        public double Value()
        {
            return Shares * Price;
        }
    }
}