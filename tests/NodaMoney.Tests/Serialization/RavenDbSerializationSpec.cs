using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Raven.Database.Server;
using Xunit;
using Raven.Tests.Helpers;
using NodaMoney.Serialization.JsonNet;

namespace NodaMoney.Tests.Serialization.RavenDbSerializationSpec
{
    // http://ravendb.net/docs/article-page/2.5/csharp/samples/raven-tests/createraventests
    public class GivenIWantToStoreInRavenDb : RavenTestBase
    {
        //[Fact]
        //public void WhenMoneyAsRoot_ThenThisMustWork()
        //{
        //    Money euros = new Money(123.56, "EUR");

        //    using (var store = NewDocumentStore(configureStore: s => s.Configuration.Storage.Voron.AllowOn32Bits = true))
        //    {
        //        // Store in RavenDb
        //        using (var session = store.OpenSession())
        //        {
        //            session.Store(euros);

        //            session.SaveChanges();
        //        }

        //        // Read from RavenDb
        //        using (var session = store.OpenSession())
        //        {
        //            var result = session.Query<Money>()
        //                .Customize(customization => customization.WaitForNonStaleResultsAsOfNow())
        //                .FirstOrDefault();

        //            result.Should().Be(euros);
        //        }
        //    }
        //}

        //[Fact]
        //public void WhenObjectWithMoneyAttribute_ThenThisMustWork()
        //{
        //    SampleData sample = new SampleData { Name = "Test", Amount = new Money(123.56, "EUR") };
        //    NonAdminHttp.EnsureCanListenToWhenInNonAdminContext(8079);
        //    using (var store = NewDocumentStore(configureStore: s => s.Configuration.Storage.Voron.AllowOn32Bits = true))
        //    {
        //        // store.Conventions.CustomizeJsonSerializer += serializer =>
        //        // serializer.Converters.Add(new MoneyJsonConverter());
        //        // WaitForUserToContinueTheTest(store, true);
        //        // store.UseEmbeddedHttpServer = true;
        //        // store.Configuration.Port = 8080;
        //        // store.Initialize();

        //        // Store in RavenDb
        //        using (var session = store.OpenSession())
        //        {
        //            session.Store(sample);

        //            session.SaveChanges();
        //        }

        //        // Read from RavenDb
        //        using (var session = store.OpenSession())
        //        {
        //            var result = session.Query<SampleData>()
        //                .Customize(customization => customization.WaitForNonStaleResultsAsOfNow())
        //                .FirstOrDefault();

        //            result.Should().Be(sample);
        //        }
        //    }
        //}
    }

    public class SampleData
    {
        public string Name { get; set; }

        public Money Amount { get; set; }
    }
}