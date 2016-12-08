using System;
using System.Collections.Generic;
using System.Linq;

using FluentAssertions;
using Xunit;
using Raven.Tests.Helpers;

namespace NodaMoney.Tests.Serialization.RavenDbSerializationSpec
{
    public class GivenIWantToStoreInRavenDb : RavenTestBase
    {
        [Fact]
        public void WhenMoneyAsRoot_ThenThisMustWork()
        {
            Money euros = new Money(123.56, "EUR");

            using (var store = NewDocumentStore())
            {
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
            SampleData sample = new SampleData { Name = "Test", Amount = new Money(123.56, "EUR") };

            using (var store = NewDocumentStore())
            {
                // Store in RavenDb
                using (var session = store.OpenSession())
                {
                    session.Store(sample);

                    session.SaveChanges();
                }

                // Read from RavenDb
                using (var session = store.OpenSession())
                {
                    var result = session.Query<SampleData>()
                        .Customize(customization => customization.WaitForNonStaleResultsAsOfNow())
                        .FirstOrDefault();

                    result.Should().Be(sample);
                }
            }
        }
    }

    public class SampleData
    {        
        public string Name { get; set; }

        public Money Amount { get; set; }
    }
}