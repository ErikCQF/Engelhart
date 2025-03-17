using PortfolioManager.Assets;
using System;

namespace PortfolioManager
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            //****  SEE README FOR INSTRUCTIONS ****//

            Test1();

            Console.WriteLine("Done... (Press a key to close)");
            Console.ReadKey();
        }

        private static void Test1()
        {
            var portfolio = new AssetPortfolio();
            portfolio.Add(new Stock("ABC", 200, 4));
            portfolio.Add(new Stock("DDW", 100, 10));
            Assert(AreEqual(portfolio.Value(), 1800),
                " Test1 Failed, Expected Value:" + "\t" + 1800 + ",\t" + "Actual Value: \t" + portfolio.Value() + "\n");
        }


        private static void Assert(bool condition, string failure)
        {
            if (!condition)
                Console.WriteLine(failure);
        }

        private static bool AreEqual(double d1, double d2)
        {
            return Math.Abs(d1 - d2) < .0001;
        }
    }
}