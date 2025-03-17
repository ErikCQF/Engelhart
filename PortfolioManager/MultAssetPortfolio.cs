using PortfolioManager.Assets;
using PortfolioManager.Services.Consolidators;
using PortfolioManager.Services.FxRates;

namespace PortfolioManager
{
    public class MultAssetPortfolio : AssetPortfolio
    {
        public string Currency { get; private set; }
        protected readonly IExchangeRates _exchangeRates;

        public MultAssetPortfolio(string currency, IExchangeRates exchangeRates, ConsolidatorService consolidatorService) : base(consolidatorService)
        {
            Currency = currency;
            _exchangeRates = exchangeRates;
        }

        public override void Add(Stock s)
        {
            if (string.IsNullOrEmpty(s.Currency)) throw new ArgumentException("Stock is missing currency");
            base.Add(s);
        }
        public void SetPortfolioCcy(string ccy)
        {
            Currency = ccy;
        }
        public override void Add(IAsset s)
        {
            _portfolio.Add(s);
        }
  
        /// <summary>
        /// Ovverrides the base method and apply FX for it
        /// </summary>
        /// <returns></returns>
        public override double Value()
        {
            double v = 0;
            foreach (var s in _portfolio)
            {
                var ccyFrom = s.Currency;
                var ccyTo = Currency;
                var fxRate = _exchangeRates.GetRate(ccyFrom, ccyTo);
                v += s.Value() * fxRate;
            }
            return v;
        }

        /// <summary>
        /// Clone only Services structure.
        /// </summary>
        /// <returns></returns>
        public override AssetPortfolio Clone()
        {
            if (_consolidatorService is null) throw new ArgumentNullException("consolidators service not found");

            return new MultAssetPortfolio(Currency, _exchangeRates, _consolidatorService); 
        }
    }
}