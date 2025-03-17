using System.Collections.Concurrent;

namespace PortfolioManager.Services.FxRates
{
    public class ExchangeDirectQuotation : IExchangeRates
    {
        protected readonly ConcurrentDictionary<string, double> _usdRates = new();

        public void AddOrUpdateRate(string currency, double rate)
        {
            if (string.IsNullOrWhiteSpace(currency))
                throw new ArgumentException("Currency cannot be null or empty.", nameof(currency));

            _usdRates.AddOrUpdate(currency, rate, (key, oldValue) => rate);
        }


        public double GetRate(string fromCurrency, string toCurrency)
        {
            if (fromCurrency == toCurrency)
                return 1.0;

            if (!_usdRates.TryGetValue(fromCurrency, out double fromUsdRate))
                throw new Exception($"Exchange rate for {fromCurrency} to USD not found.");

            if (!_usdRates.TryGetValue(toCurrency, out double toUsdRate))
                throw new Exception($"Exchange rate for {toCurrency} to USD not found.");


            return fromUsdRate / toUsdRate;
        }
    }
}