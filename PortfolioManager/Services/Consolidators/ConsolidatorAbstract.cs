using PortfolioManager.Assets;

namespace PortfolioManager.Services.Consolidators
{
    public abstract class ConsolidatorAbstract
    {
        public abstract Type AssetType { get; }
        public abstract IEnumerable<IAsset> Consolidate(IEnumerable<IAsset> assets);
    }
}
