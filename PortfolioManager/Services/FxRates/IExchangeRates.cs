namespace PortfolioManager.Services.FxRates
{
    public interface IExchangeRates
    {
        double GetRate(string fromCurrency, string toCurrency);
    }
}