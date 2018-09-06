using System.Linq;
using FluentAssertions;
using Raven.TestDriver;
using Xunit;

namespace NodaMoney.Tests.Serialization.RavenDbSerializationSpec
{
    public class GivenIWantToStoreInRavenDb : RavenTestDriver
    {
        [Fact]
        public void WhenMoneyAsRoot_ThenThisMustWork()
        {
            Money euros = new Money(123.56, "EUR");

            using (var store = GetDocumentStore())
            {
                // Store in RavenDb
                using (var session = store.OpenSession())
                {
                    session.Store(euros);
                    session.SaveChanges();
                }

                WaitForIndexing(store);
                // WaitForUserToContinueTheTest(store); // Sometimes we want to debug the test itself, this redirect us to the studio

                // Read from RavenDb
                using (var session = store.OpenSession())
                {
                    var result = session.Query<Money>().FirstOrDefault();

                    result.Should().Be(euros);
                }
            }
        }

        [Fact]
        public void WhenObjectWithMoneyAttribute_ThenThisMustWork()
        {
            SampleData sample = new SampleData { Name = "Test", Price = new Money(123.56, "EUR"), BaseCurrency = Currency.FromCode("USD") };

            using (var store = GetDocumentStore())
            {
                // Store in RavenDb
                using (var session = store.OpenSession())
                {
                    session.Store(sample);
                    session.SaveChanges();
                }

                WaitForIndexing(store);
                // WaitForUserToContinueTheTest(store); // Sometimes we want to debug the test itself, this redirect us to the studio

                // Read from RavenDb
                using (var session = store.OpenSession())
                {
                    var result = session.Query<SampleData>().FirstOrDefault();

                    result.Name.Should().Be(sample.Name);
                    result.Price.Should().Be(sample.Price);
                }
            }
        }
    }

    public class SampleData
    {
        public string Name { get; set; }

        public Money Price { get; set; }

        public Currency BaseCurrency { get; set; }
    }
}
