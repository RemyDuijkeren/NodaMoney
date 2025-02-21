using System.Linq;
using Raven.TestDriver;

namespace NodaMoney.Tests.Serialization.RavenDbSerializationSpec;

public class StoreInRavenDb : RavenTestDriver
{
    static StoreInRavenDb()
    {
        // ConfigureServer() must be set before calling GetDocumentStore() and can only be set once per test run.
        ConfigureServer(new TestServerOptions { Licensing = { ThrowOnInvalidOrMissingLicense = false } });
    }

    [Fact]
    public void WhenObjectWithMoneyAttribute_ThenThisMustWork()
    {
        // Only run this test for .NET 9.0, because only .NET 9.0 is installed on the build server.
        if (!AppContext.TargetFrameworkName.Contains(".NETCoreApp,Version=v9.0"))
        {
            return; // Skip execution for other frameworks
        }

        SampleData sample = new SampleData { Name = "Test", Price = new Money(123.56, "EUR"), BaseCurrency = Currency.FromCode("USD") };

        using var store = GetDocumentStore();

        // Store in RavenDb
        using (var session = store.OpenSession())
        {
            session.Store(sample);
            session.SaveChanges();
        }

        WaitForIndexing(store);
        //WaitForUserToContinueTheTest(store); // Sometimes we want to debug the test itself, this redirect us to the studio

        // Read from RavenDb
        using (var session = store.OpenSession())
        {
            var result = session.Query<SampleData>().FirstOrDefault();

            result.Name.Should().Be(sample.Name);
            result.Price.Should().Be(sample.Price);
        }
    }
}
