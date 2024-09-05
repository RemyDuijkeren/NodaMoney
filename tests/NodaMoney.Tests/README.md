Test are structured in a BDD way (Spec). Here’s the way we write tests:

```C#
namespace NodaMoney.Tests.JoinSpec
{
   public class GivenAJoinWithTwoPredecessorsAndOneSuccessor
   {
     // ctor builds up the context

     [Fact]
     public void WhenAPredecessorIsAvailable_ThenJoinIsBlocked()
     {
       // set predecesor state,       // verify the join has the given state
     }

     [Fact]
     public void WhenBothPredecessorsAreFinished_ThenJoinIsFinished()
     {
       // set predecesor state       // verify the join changed state to Finished
     }
   }
}
```

How it works:
- The last part of the namespace becomes the logical grouping of the tests. This typically is the name of the class under test
  plus the “Spec” suffix.
- The test class starts with “Given” and the phrase that follows describes what’s instantiated in the constructor and typically
  stored in fields for use by tests. The Given is the Arrange in AAA.
- Test methods have two parts: “When” and “Then”, separated by an underscore.
    - When: describes the action or state change that is caused in the context to perform the test. This is the Act in AAA.
	  This is typically just one operation, but it could be more if changing the state/acting requires so.
    - Then: the Assert in AAA. Typically just one Assert or mock Verify, but there could be more than one if verifying the
	  state/interactions require so. But in either case, the Then should describe what you’re asserting.

Key benefits of this approach:
- This is plain unit test code. You could as well use MSTest, xUnit, NUnit, etc. No new paradigms to learn, just some naming
  conventions.
- The only additional “overhead” is having a separate context (Given) class to group related tests (those tests that use the
  same setup).
- Having a convention in place for how to write tests has proven immensely valuable on its own. I can navigate our tests and
  not tell the difference on who wrote which tests.
- It triggers good practices about test complexity almost automatically: because context + tests have to make sense as an
  english phrase, sometimes you realize that a given test is testing too much (the test method becomes TOOOO long to write).
- It’s trivial to write code that uses reflection to render this as a document

We use this as a guideline. There’s no requirement that we have a context class. Sometimes, it’s just not worth it because
you’re testing a very small unit. In this case, the *Spec becomes the class, such as below. This is typically more the
exception than the rule, though.
```C#
namespace NodaMoney.Tests
{
   public class FinalSpec   {   }
}
```
