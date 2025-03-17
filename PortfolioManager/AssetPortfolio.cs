using PortfolioManager.Assets;
using PortfolioManager.Services.Consolidators;

namespace PortfolioManager
{

    public class AssetPortfolio
    {
        
        protected List<IAsset> _portfolio { get; set; } = new List<IAsset>();

        public AssetPortfolio() { }
   
        public virtual void Add(Stock s)
        {
            _portfolio.Add(s);
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
 
        public virtual AssetPortfolio Consolidate()
        {
      
            AssetPortfolio consolidated =new();

            var assets = StockConsolidator.ConsolidateStocks(_portfolio.OfType<Stock>());

            //Populate the portfolio with consolidated asset values
            foreach (var asset in assets ?? Enumerable.Empty<IAsset>())
            {                
                consolidated.Add((Stock)asset);
            }

            return consolidated;
        }
    }
}