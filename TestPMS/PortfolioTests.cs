using PortfolioManager.Assets;
using PortfolioManager.Services.Consolidators;
using PortfolioManager.Services.FxRates;

namespace PortfolioManager.Tests
{
    public class PortfolioTests
    {
        /// <summary>
        /// Copied from Main
        /// </summary>
        [Fact]
        public void AssetPortfolio_Test()
        {
            var portfolio = new AssetPortfolio();
            portfolio.Add(new Stock("ABC", 200, 4));
            portfolio.Add(new Stock("DDW", 100, 10));

            Assert.True(AreEqual(portfolio.Value(), 1800));
        }
        /// <summary>
        /// Compatibility test with AssetPortfolio
        /// See AssetPortfolio_Test()
        /// </summary>
        [Fact]
        public void TestMultAssetPortfolio_Same_Currency()
        {
            //Arrange
            var fxRates = Create_ExchangeRates();
            var consolidator = Create_ConsolidatorService();
            var portfolio = new MultAssetPortfolio("USD", fxRates, consolidator);

            portfolio.Add(new Stock("ABC", 200, 4, "USD"));
            portfolio.Add(new Stock("DDW", 100, 10, "USD"));

            //Assert and Act
            Assert.True(AreEqual(portfolio.Value(), 1800));
        }
        /// <summary>
        /// From Spec: Modify the design to accommodate multiple asset types:
        ///  a. foreign currency assets (i.e. 1000 EUR at current exchange rate)
        ///  b. foreign currency denominated stocks (i.e. stock denominated in GBP)
        /// </summary>
        /// <param name="CCY"></param>
        [Theory]
        [MemberData(nameof(GetCurrencyPorfolioToTest))]
        public void Test_Portfolio_Mult_Currency_Mult_Asset_Portfolio(string CCY)
        {
            //Arrange
            var fxRates = Create_ExchangeRates();
            var consolidator = Create_ConsolidatorService();

            var usdToCcy = fxRates.GetRate("USD", CCY);
            var eurTocy = fxRates.GetRate("EUR", CCY);
            var gbpToCcy = fxRates.GetRate("GBP", CCY);

            var portfolio = new MultAssetPortfolio(CCY, fxRates, consolidator);
            //Add Asset in USD
            portfolio.Add(new Stock("ABC", 200, 4, "USD"));
            portfolio.Add(new Stock("ABC", 100, 5, "USD"));
            var abcTotal = ((200 * 4) + (100 * 5)) * usdToCcy;

            //Add Asset in EUR
            portfolio.Add(new Stock("DDW", 100, 10, "EUR"));
            var dowTotal = 100 * 10 * eurTocy;
            //Add Asset in GBP

            portfolio.Add(new Stock("APT", 100, 10, "GBP"));
            var aptTotal = 100 * 10 * gbpToCcy;

            //Add Currencies
            portfolio.Add(new CurrencyAsset("USD", 50));
            portfolio.Add(new CurrencyAsset("USD", 50));
            portfolio.Add(new CurrencyAsset("GBP", 100));
            portfolio.Add(new CurrencyAsset("EUR", 100));

            var usdTotal = 100 * usdToCcy;
            var gbpTotal = 100 * gbpToCcy;
            var eurTotal = 100 * eurTocy;
            var portFolioTotal = abcTotal + dowTotal + aptTotal + usdTotal + gbpTotal + eurTotal;

            //Act
            var portValue = portfolio.Value();

            var portValueCosolidated = (portfolio.Consolidate()).Value();

            //Assert 
            Assert.True(AreEqual(portValue, portFolioTotal));
            Assert.True(AreEqual(portValueCosolidated, portFolioTotal));

        }
        /// <summary>
        /// From Spec: Adjust the design to allow valuing the portfolio in any currency (i.e. given current exchange rates, 
        /// be able to find the value of the entire portfolio, for example, in USD, GBP, EUR, etc.)
        /// </summary>
        /// <param name="CCY_FROM"></param>
        /// <param name="CCY_TO"></param>
        [Theory]
        [MemberData(nameof(GetChangeCurrencyPorfolioToTest))]
        public void Test_Portfolio_Mult_Currency_Mult_Asset_Portfolio_Change_Curreny_Porfolio(string CCY_FROM, string CCY_TO)
        {
            //Arrange
            var fxRates = Create_ExchangeRates();
            var consolidator = Create_ConsolidatorService();

            var usdToCcy = fxRates.GetRate("USD", CCY_FROM);
            var eurTocy = fxRates.GetRate("EUR", CCY_FROM);
            var gbpToCcy = fxRates.GetRate("GBP", CCY_FROM);
            var fromToRate = fxRates.GetRate(CCY_FROM, CCY_TO);

            var portfolio = new MultAssetPortfolio(CCY_FROM, fxRates, consolidator);

            //Add Asset in USD
            portfolio.Add(new Stock("ABC", 200, 4, "USD"));
            portfolio.Add(new Stock("ABC", 100, 5, "USD"));
            var abcTotal = ((200 * 4) + (100 * 5)) * usdToCcy;

            //Add Asset in EUR
            portfolio.Add(new Stock("DDW", 100, 10, "EUR"));
            var dowTotal = 100 * 10 * eurTocy;
            //Add Asset in GBP

            portfolio.Add(new Stock("APT", 100, 10, "GBP"));
            var aptTotal = 100 * 10 * gbpToCcy;

            //Add Currencies
            portfolio.Add(new CurrencyAsset("USD", 50));
            portfolio.Add(new CurrencyAsset("USD", 50));
            portfolio.Add(new CurrencyAsset("GBP", 100));
            portfolio.Add(new CurrencyAsset("EUR", 100));

            var usdTotal = 100 * usdToCcy;
            var gbpTotal = 100 * gbpToCcy;
            var eurTotal = 100 * eurTocy;
            var portFolioTotal = abcTotal + dowTotal + aptTotal + usdTotal + gbpTotal + eurTotal;
            

            //Act
            var portValue = portfolio.Value();
            var portValueCosolidated = (portfolio.Consolidate()).Value();

            portfolio.SetPortfolioCcy(CCY_TO);


            var portValueChangeCurrency = portfolio.Value();
            var portValueCosolidatedChangeCurrency = (portfolio.Consolidate()).Value();

            //Assert 
            Assert.True(AreEqual(portValue, portFolioTotal));
            Assert.True(AreEqual(portValueCosolidated, portFolioTotal));

            Assert.True(AreEqual(portValueChangeCurrency, portFolioTotal* fromToRate));
            Assert.True(AreEqual(portValueCosolidatedChangeCurrency, portFolioTotal * fromToRate));

        }
  

        /// <summary>
        /// From Spect: Complete the function to consolidate the portfolio by unique security and average cost. 
        /// For example if the portfolio consists of 100 shares of ABC at $2 and 200 shares of ABC at $3.50, 
        /// the consolidated portfolio will have 300 shares of ABC at $3.
        /// </summary>
        [Fact]
        public void TestConsolidator_Service_For_Stocks()
        {
            // Arrange
            const string ABC = "ABC";
            const string DOW = "DOW";

            var portfolio = new List<Stock>
            {
                new Stock(ABC, 100, 2),
                new Stock(ABC, 200, 3.5),
                new Stock(DOW, 100, 10)
            };

            var consolidator = Create_ConsolidatorService();

            // Act
            var consolidated = (consolidator.Consolidate(portfolio).Select(a => a as Stock)).ToList();
            Stock? abcAsset = consolidated?.Where(a => a?.Symbol == ABC).FirstOrDefault();
            Stock? dowAsset = consolidated?.Where(a => a?.Symbol == DOW).FirstOrDefault();

            Assert.True(abcAsset is not null);
            Assert.True(dowAsset is not null);
            Assert.True(consolidated is not null);


            // Assert
            Assert.True(AreEqual(consolidated.Where(a => a?.Symbol == ABC).Count(), 1)); 
            Assert.True(AreEqual(consolidated.Where(a => a?.Symbol == DOW).Count(), 1));

            Assert.True(AreEqual(abcAsset.Shares, 300));
            Assert.True(AreEqual(abcAsset.Price, 3));

            Assert.True(AreEqual(dowAsset.Shares, 100));
            Assert.True(AreEqual(dowAsset.Price, 10));
        }

        /// <summary>        
        /// From Spec: Modify the design to accommodate multiple asset types:
        ///  a. foreign currency assets (i.e. 1000 EUR at current exchange rate) 
        /// <param name="CCY"></param>
        /// </summary>
        [Fact]
        public void Test_Consolidator_Service_For_Currency()
        {
            const string USD = "USD";
            const string EUR = "EUR";

            // Arrange
            var portfolio = new List<CurrencyAsset>
            {
                new CurrencyAsset(USD, 100),
                new CurrencyAsset(USD, -50),
                new CurrencyAsset(EUR, 200),
                new CurrencyAsset(EUR, 200)
            };

            var consolidator = Create_ConsolidatorService();

            // Act
            var consolidated = (consolidator.Consolidate(portfolio).Select(a => a as CurrencyAsset)).ToList();
            CurrencyAsset? usdAsset = consolidated?.Where(a => a?.Currency == USD).FirstOrDefault();
            CurrencyAsset? eurAsset = consolidated?.Where(a => a?.Currency == EUR).FirstOrDefault();

            // Assert
            Assert.True(consolidated is not null);
            Assert.True(usdAsset is not null);
            Assert.True(eurAsset is not null);

            Assert.True(AreEqual(consolidated.Where(a => a?.Currency == USD).Count(), 1));
            Assert.True(AreEqual(consolidated.Where(a => a?.Currency == EUR).Count(), 1));

            Assert.True(AreEqual(usdAsset.Amount, 50));
            Assert.True(AreEqual(eurAsset.Amount, 400));
        }

        private bool AreEqual(double d1, double d2)
        {
            return Math.Abs(d1 - d2) < 0.0001;
        }

        private ConsolidatorService Create_ConsolidatorService()
        {
            List<ConsolidatorAbstract> cons = new();
            cons.Add(new CurrencyAssetConsolidator());
            cons.Add(new StockConsolidator());

            return new ConsolidatorService(cons);
        }
        private ExchangeDirectQuotation Create_ExchangeRates()
        {
            var rates = new ExchangeDirectQuotation();
            rates.AddOrUpdateRate("USD", 1);
            rates.AddOrUpdateRate("GBP", 1.29);
            rates.AddOrUpdateRate("EUR", 1.09);

            return rates;
        }
        public static IEnumerable<object[]> GetCurrencyPorfolioToTest()
        {
            return new List<object[]>
                {
                    new object[] { "USD" },
                    new object[] { "GBP" },
                    new object[] { "EUR" }
                };
        }

        public static IEnumerable<object[]> GetChangeCurrencyPorfolioToTest()
        {
            var currencies = new[] { "USD", "GBP", "EUR" };

            return currencies
                .SelectMany(from => currencies, (from, to) => new object[] { from, to })
                .ToList();
        }


    }
}
