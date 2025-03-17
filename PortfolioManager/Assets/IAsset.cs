namespace PortfolioManager.Assets
{
    public interface IAsset
    {
        string Currency { get; }
        double Value();
    }
}