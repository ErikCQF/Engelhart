using PortfolioManager.Assets;
using PortfolioManager.Services.Consolidators;
using PortfolioManager.Services.FxRates;
using System.Net.Http.Headers;

namespace PortfolioManager
{

    public class AssetPortfolio
    {
        protected readonly ConsolidatorService? _consolidatorService;

        public AssetPortfolio()
        {

        }
        public AssetPortfolio(ConsolidatorService consolidatorService)
        {
            _consolidatorService = consolidatorService;
        }
        protected List<IAsset> _portfolio { get; set; } = new List<IAsset>();

        public virtual void Add(Stock s)
        {
            _portfolio.Add(s);
        }
        public virtual void Add(IAsset s)
        {
            if (s is Stock)
            {
                _portfolio.Add(s);
            }

            throw new InvalidOperationException("This PMS version Only Accept stocks in the porfolio cuurency");
        }

        public virtual double Value()
        {
            double v = 0;
            foreach (var s in _portfolio)
            {
                v += s.Value();
            }
            return v;
        }

        /// <summary>
        /// Clone only Services structure.
        /// </summary>
        /// <returns></returns>
        public virtual AssetPortfolio Clone()
        {
            return _consolidatorService == null ? new AssetPortfolio() : new AssetPortfolio(_consolidatorService);
        }
        public AssetPortfolio Consolidate()
        {
            if (_consolidatorService is null)
            {
                throw new Exception($"Missing {nameof(ConsolidatorService)}");
            }

            AssetPortfolio consolidated = this.Clone();

            var assets = _consolidatorService.Consolidate(_portfolio.ToList());

            //Populate the portfolio with consolidated asset values
            foreach (var asset in assets ?? Enumerable.Empty<IAsset>())
            {
                consolidated.Add(asset);
            }

            return consolidated;
        }
    }
}