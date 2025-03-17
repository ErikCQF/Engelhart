using PortfolioManager.Assets;

namespace PortfolioManager.Services.Consolidators
{
    public class StockConsolidator : ConsolidatorAbstract
    {
        public override Type AssetType => typeof(Stock);

        public override IEnumerable<IAsset> Consolidate(IEnumerable<IAsset> assets)
        {
            var stocks = assets.OfType<Stock>(); 
            return ConsolidateStocks(stocks);
        }

        private IEnumerable<IAsset> ConsolidateStocks(IEnumerable<Stock> stocks)
        {
            var assets = stocks.ToList();
            if (assets.Count() == 0) return new List<IAsset>();

            var groupedStocks = assets
                                .GroupBy(stock => stock.Symbol)
                                .ToDictionary(group => group.Key, group => group.ToList());

            var consolidated = new List<Stock>();

            foreach (var stock in groupedStocks)
            {
                //Weighted Avarage Price, as in the spec
                var avgPrice = stock.Value.Sum(a => Math.Abs(a.Value())) / stock.Value.Sum(a => Math.Abs(a.Shares));
                var totalShares = stock.Value.Sum(a => a.Shares);
                var ccy = stock.Value.First().Currency;

                consolidated.Add(new Stock(stock.Key, totalShares, avgPrice, ccy));
            }
            return consolidated;
        }
    }
}
