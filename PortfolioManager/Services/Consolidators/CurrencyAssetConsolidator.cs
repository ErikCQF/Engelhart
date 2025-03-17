using PortfolioManager.Assets;

namespace PortfolioManager.Services.Consolidators
{
    public class CurrencyAssetConsolidator : ConsolidatorAbstract
    {
        public override Type AssetType => typeof(CurrencyAsset);

        public override IEnumerable<IAsset> Consolidate(IEnumerable<IAsset> assets)
        {
            var stocks = assets.OfType<CurrencyAsset>(); 
            return ConsolidateStocks(stocks);
        }

        private IEnumerable<IAsset> ConsolidateStocks(IEnumerable<CurrencyAsset> currencyAssets)
        {
            var assets = currencyAssets.ToList();
            if (assets.Count() == 0) return new List<IAsset>();

            var groupedAssets = assets
                                .GroupBy(stock => stock.Currency)
                                .ToDictionary(group => group.Key, group => group.ToList());

            var consolidated = new List<CurrencyAsset>();

            foreach (var stock in groupedAssets)
            {
                var total = stock.Value.Sum(a => a.Amount);
                consolidated.Add(new CurrencyAsset(stock.Key, total));
            }
            return consolidated;
        }
    }
}
