namespace PortfolioManager.Assets
{
    public interface IShare : IAsset
    {
        double Price { get; set; }
        double Shares { get; set; }
        string Symbol { get; set; }
    }
}