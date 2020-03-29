using Xunit;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;

namespace NodaMoney.Tests.PerformanceSpec
{

    public class GivenIWantToUseALotOfCurrencies
    {
        [Fact]
        public void StartBenchmark()
        {
            var summary1 = BenchmarkRunner.Run<DefaultBenchmarks>();

            var summary2 = BenchmarkRunner.Run<MemoryBenchmarks>();
        }
    }

    [MemoryDiagnoser]
    public class DefaultBenchmarks
    {
        [Benchmark]        
        public void CreateCurrency()
        {
            Currency currency = Currency.FromCode("EUR");
        }

        [Benchmark]
        public void CreateMoney()
        {
            Money money = new Money(10M, "EUR");
        }
    }

    [MemoryDiagnoser]
    public class MemoryBenchmarks
    {
        [Benchmark]
        public void CreatingOneMillionCurrency()
        {
            int max = 1_000_000;
            Currency[] currencies = new Currency[max];

            for (int i = 0; i < max; i++)
            {
                if (i % 3 == 0)
                    currencies[i] = Currency.FromCode("EUR");
                else if (i % 2 == 0)
                    currencies[i] = Currency.FromCode("USD");
                else
                    currencies[i] = Currency.FromCode("JPY");
            }
        }

        [Benchmark]
        public void CreatingOneMillionMoney()
        {
            int max = 1_000_000;
            Money[] money = new Money[max];

            for (int i = 0; i < max; i++)
            {
                if (i % 3 == 0)
                    money[i] = new Money(10M, "EUR");
                else if (i % 2 == 0)
                    money[i] = new Money(10M, "USD");
                else
                    money[i] = new Money(10M, "JPY");
            }
        }
    }
}
