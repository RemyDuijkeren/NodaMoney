using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Raven.Database.Server;
using Xunit;
using Raven.Tests.Helpers;
using NodaMoney.Serialization.JsonNet;
using Raven.Client.Document;

namespace NodaMoney.Tests.Serialization.RavenDbSerializationSpec
{
    // http://ravendb.net/docs/article-page/2.5/csharp/samples/raven-tests/createraventests
    public class GivenIWantToStoreInRavenDb : RavenTestBase
    {
        [Fact]
        public void WhenMoneyAsRoot_ThenThisMustWork()
        {
            Money euros = new Money(123.56, "EUR");

            using (var store = NewDocumentStore(configureStore: s => s.Configuration.Storage.Voron.AllowOn32Bits = true))
            {
                //using (var store = new DocumentStore { Url = "http://obelix:8081/", DefaultDatabase = "Example" })
                //{
                //store.Conventions.CustomizeJsonSerializer += serializer =>
                //    serializer.Converters.Add(new MoneyRavenDbConverter());
                store.Initialize();

                // Store in RavenDb
                using (var session = store.OpenSession())
                {
                    session.Store(euros);

                    session.SaveChanges();
                }

                // Read from RavenDb
                using (var session = store.OpenSession())
                {
                    var result = session.Query<Money>()
                        .Customize(customization => customization.WaitForNonStaleResultsAsOfNow())
                        .FirstOrDefault();

                    result.Should().Be(euros);
                }
            }
        }

        [Fact]
        public void WhenObjectWithMoneyAttribute_ThenThisMustWork()
        {
            SampleData sample = new SampleData { Name = "Test", Price = new Money(123.56, "EUR"), BaseCurrency = Currency.FromCode("USD") };
            using (var store = NewDocumentStore(configureStore: s => s.Configuration.Storage.Voron.AllowOn32Bits = true))
            {
            //using (var store = new DocumentStore { Url = "http://127.0.0.1:8080/", DefaultDatabase = "Example" })
            //{
                //store.Conventions.CustomizeJsonSerializer += serializer =>
                //    serializer.Converters.Add(new MoneyRavenDbConverter());
                store.Initialize();

                // Store in RavenDb
                using (var session = store.OpenSession())
                {
                    session.Store(sample);

                    session.SaveChanges();
                }

                //WaitForUserToContinueTheTest(store, true);
                // Read from RavenDb
                using (var session = store.OpenSession())
                {
                    var result = session.Query<SampleData>()
                        .Customize(customization => customization.WaitForNonStaleResultsAsOfNow())
                        .FirstOrDefault();

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
