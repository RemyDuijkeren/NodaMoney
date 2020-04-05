using Xunit;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using System;
using System.Globalization;

namespace NodaMoney.Tests.PerformanceSpec
{
    public class GivenIWantToBenchmark
    {
        [Fact]
        public void ExecuteBenchmark()
        {
            var initalizingCurrencyReport = BenchmarkRunner.Run<InitalizingCurrencyBenchmarks>();
            var initalizingMoneyReport = BenchmarkRunner.Run<InitalizingMoneyBenchmarks>();
            var moneyOperationsReport = BenchmarkRunner.Run<MoneyOperationsBenchmarks>();
            var moneyFormattingReport = BenchmarkRunner.Run<MoneyFormattingBenchmarks>();
            var moneyParsingReport = BenchmarkRunner.Run<MoneyParsingBenchmarks>();
            //var addingCustomCurrencyReport = BenchmarkRunner.Run<AddingCustomCurrencyBenchmarks>();
            var highLoadBenchReport = BenchmarkRunner.Run<HighLoadBenchmarks>();
        }

        [Fact]
        public void SizeOfString()
        {
            int size = System.Text.ASCIIEncoding.Unicode.GetByteCount("123");
            Console.WriteLine($"size is {size}");
        }
    }

    [MemoryDiagnoser]
    public class InitalizingCurrencyBenchmarks
    {
        [Benchmark]
        public void FromCode()
        {
            Currency currency = Currency.FromCode("EUR");
        }

        [Benchmark]
        public void FromCodeBeRef()
        {
            ref Currency currency = ref Currency.FromCode("EUR");
        }
    }

    [MemoryDiagnoser]
    public class InitalizingMoneyBenchmarks
    {
        private readonly Currency _euro = Currency.FromCode("EUR");
        private readonly Money _money = new Money(10m, "EUR");

        [Benchmark(Baseline = true)]
        public void ExplicitCurrencyAsString()
        {
            var euros = new Money(6.54m, "EUR");
        }

        [Benchmark]
        public void ExplicitCurrencyAsStringAndRounding()
        {
            var euro = new Money(765.425m, "EUR", MidpointRounding.AwayFromZero);
        }

        [Benchmark]
        public void ExplicitCurrencyFromCode()
        {
            var euros = new Money(6.54m, Currency.FromCode("EUR"));
        }

        [Benchmark]
        public void HelperMethod()
        {
            var money = Money.Euro(6.54m);
        }

        [Benchmark]
        public void ImplicitCurrencyByConstructor()
        {
            var money = new Money(6.54m);
        }

        [Benchmark]
        public void ImplicitCurrencyByCasting()
        {
            Money money = 6.54m;
        }

        [Benchmark]
        public void Deconstruct()
        {
            var (amount, currency) = _money;
        }
    }

    [MemoryDiagnoser]
    public class MoneyOperationsBenchmarks
    {
        private readonly Money _euro10 = Money.Euro(10);
        private readonly Money _euro20 = Money.Euro(20);
        private readonly Money _dollar10 = Money.USDollar(10);
        private Money _euro = new Money(765.43m, "EUR");

        [Benchmark]
        public void Addition()
        {
            var euro30 = _euro10 + _euro20;
        }

        [Benchmark]
        public void Subtraction()
        {
            var euro10 = _euro20 - _euro10;
        }

        [Benchmark]
        public void CompareSameCurrency()
        {
            bool isSame = _euro10 == _euro20; // false
        }

        [Benchmark]
        public void CompareDifferentCurrency()
        {
            bool isSame = _euro10 == _dollar10; // false
        }

        [Benchmark]
        public void CompareAmount()
        {
            bool isBigger = _euro20 > _euro10; // true
        }

        [Benchmark]
        public void Increment()
        {
            ++_euro;
        }

        [Benchmark]
        public void Decrement()
        {
            --_euro;
        }
    }

    [MemoryDiagnoser]
    public class MoneyFormattingBenchmarks
    {
        private readonly Money _euro = new Money(765.43m, "EUR");
        private readonly CultureInfo ci = new CultureInfo("nl-NL");

        [Benchmark]
        public void Implicit()
        {
            var s = _euro.ToString();
        }

        [Benchmark]
        public void ImplicitWithFormat()
        {
            var s = _euro.ToString("C2");
        }

        [Benchmark]
        public void Explicit()
        {
            var s = _euro.ToString(ci);
        }

        [Benchmark]
        public void ExplicitWithFormat()
        {
            var s = _euro.ToString("C2", ci);
        }
    }

    [MemoryDiagnoser]
    public class MoneyParsingBenchmarks
    {
        [Benchmark]
        public void Implicit()
        {
            var euro = Money.Parse("€ 765,43"); // or € 765.43
        }

        [Benchmark]
        public void ImplicitTry()
        {
            Money.TryParse("€ 765,43", out Money euro); // or € 765.43
        }

        [Benchmark]
        public void Explicit()
        {
            var euro = Money.Parse("€ 765,43", Currency.FromCode("EUR"));  // or € 765.43
        }

        [Benchmark]
        public void ExplicitTry()
        {
            Money.TryParse("€ 765,43", Currency.FromCode("EUR"), out Money euro);
        }
    }

    [MemoryDiagnoser]
    public class AddingCustomCurrencyBenchmarks
    {
        private readonly CurrencyBuilder _builder = new CurrencyBuilder("BTC", "virtual")
        {
            EnglishName = "Bitcoin",
            Symbol = "฿",
            IsoNumber = "123", // iso number
            DecimalDigits = 8
        };

        [Benchmark]
        public void CreateBuilder()
        {
            var builder = new CurrencyBuilder("BTC", "virtual")
            {
                EnglishName = "Bitcoin",
                Symbol = "฿",
                IsoNumber = "123", // iso number
                DecimalDigits = 8
            };
        }

        [Benchmark]
        public void Build()
        {
            _builder.Build();
        }

        // [Benchmark]
        public void Register()
        {
            _builder.Register();
        }

        // [Benchmark]
        public void Unregister()
        {
            var dollar = CurrencyBuilder.Unregister("USD", "ISO-4217");
        }

        // [Benchmark]
        public void Replace()
        {
            Currency oldEuro = CurrencyBuilder.Unregister("EUR", "ISO-4217");

            var builder = new CurrencyBuilder("EUR", "ISO-4217");
            builder.LoadDataFromCurrency(oldEuro);
            builder.EnglishName = "New Euro";
            builder.DecimalDigits = 1;
            builder.Register();
        }
    }

    [MemoryDiagnoser]
    public class HighLoadBenchmarks
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
