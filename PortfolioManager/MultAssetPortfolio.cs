using PortfolioManager.Assets;
using PortfolioManager.Services.Consolidators;
using PortfolioManager.Services.FxRates;

namespace PortfolioManager
{
    public class MultAssetPortfolio : AssetPortfolio
    {
        public string Currency { get; private set; }
        protected readonly IExchangeRates _exchangeRates;
        protected readonly ConsolidatorService? _consolidatorService;

        public MultAssetPortfolio(string currency, IExchangeRates exchangeRates, ConsolidatorService consolidatorService) 
        {
            Currency = currency;
            _exchangeRates = exchangeRates;
            _consolidatorService = consolidatorService;
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
        public virtual void Add(IAsset s)
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

        
        public override AssetPortfolio Consolidate()
        {
            if (_consolidatorService is null)
            {
                throw new Exception($"Missing {nameof(ConsolidatorService)}");
            }

            MultAssetPortfolio consolidated = new(Currency,_exchangeRates,_consolidatorService);

            var assets = _consolidatorService.Consolidate(_portfolio.ToList());

            //Populate the portfolio with consolidated asset values
            foreach (var asset in assets ?? Enumerable.Empty<IAsset>())
            {
                consolidated.Add(asset);
            }

            return consolidated;
        }

        /// <summary>
        /// Clone only Services structure.
        /// </summary>
        /// <returns></returns>
        public virtual AssetPortfolio Clone()
        {
            if (_consolidatorService is null) throw new ArgumentNullException("consolidators service not found");

            return new MultAssetPortfolio(Currency, _exchangeRates, _consolidatorService); 
        }
    }
}