using PortfolioManager.Assets;

namespace PortfolioManager.Services.Consolidators
{
    public class ConsolidatorService
    {
        private readonly Dictionary<Type, ConsolidatorAbstract> _consolidators;

        public ConsolidatorService(List<ConsolidatorAbstract> consolidators)
        {
            _consolidators = consolidators.ToDictionary(a => a.AssetType);
        }

        public IEnumerable<IAsset> Consolidate(IEnumerable<IAsset> assets)
        {
            if(assets.Count()==0) return assets;

            var groupedPortfolio = assets
               .GroupBy(asset => asset.GetType()) // Group by type
               .ToDictionary(group => group.Key, group => group.ToList());

            var result =  new List<IAsset>();

            foreach (var group in groupedPortfolio.Values ?? Enumerable.Empty<IEnumerable<IAsset>>())
            {
                if(group.Count()==0) continue;  

                Type assetType = group.First().GetType();

                if (!(_consolidators.TryGetValue(assetType, out var consolidator)))
                {
                    throw new Exception($"{assetType} not found for factory");
                }
                var assetsConsiladet = consolidator.Consolidate(group);

                result.AddRange(assetsConsiladet);
            }

            return  result;
        }
    }
}
