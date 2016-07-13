using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace NodaMoney.Tests.MoneyCalculationsSpec
{
    public class GivenIWantToDoCalculationsWithoutRoundingInBetween
    {
        [Fact]
        public void When_Then()
        {
            bool executed = false;
            Func<decimal, decimal> fx = amount =>
            { 
                executed = true;
                return amount / 3m;
            };

            var subject= new Money(5m);
            // Money result = subject.Perform(fx);
        }
    }
}
