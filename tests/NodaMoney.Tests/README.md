# Testing

## Test Frameworks and libraries

As our test framework, we use [xUnit](https://xunit.net/). For mocking we use [NSubstitute](https://nsubstitute.github.io/) and [Fake Xrm Easy](https://github.com/DynamicsValue/fake-xrm-easy).

We use [AutoBogus](https://github.com/nickdodd79/AutoBogus), which uses [Bogus](https://github.com/bchavez/Bogus)
to generate fake data. To use AutoBogus, we only need to reference [AutoBogus.NSubstitute](https://www.nuget.org/packages/AutoBogus.NSubstitute),
the NSubtitute binding for AutoBogus.

## Test Targets

Our test targets are:
1. `net10.0` - Tests the most recent .NET version
2. `net9.0` - Tests against .NET 9
3. `net8.0` - Tests against .NET 8
4. `net6.0` - To test functionality for netstandard2.1 compatibility
5. `net48`  - To test functionality for netstandard2.0 compatibility

This approach provides coverage for both the modern .NET versions and the compatibility scenarios:
- .NET Framework 4.8 implements netstandard2.0
- .NET 6.0 implements netstandard2.1

## Test structure

Try the use the general test guidelines from [Microsoft](https://docs.microsoft.com/en-us/dotnet/core/testing/unit-testing-best-practices).

We test the behaviour of the System Under Test (SUT), not the implementation. We don't care how the SUT does it, only
that it does it.

So we don't test private methods, because they are an implementation detail. We test the public methods, because they
are the interface of the SUT. Don't test all the details or in-between steps. Test the expected outcome to prevent
brittle tests.

### Test naming

We follow the [Given-When-Then](https://www.agilealliance.org/glossary/given-when-then/) style of writing acceptance
criteria for tests.

Where generally:
- **Given** part in the class names
- **When** and **Then** in the method names

#### Methods names

Method names should capture the **When** and **Then**, like **When/With[PreCondition(s)]_Then[ExpectedBehvavior]**.
We often remove the _Then_ prefix, because it's redundant, or replace it with _Should_ if it makes more sense.

Optionally you can include the method name under test at the beginning at the method name to group the tests (try not to
because when the method changes, you need to change the tests). The test method name should be a sentence that reads.

This will result in a test methods name like below:.

    (MethodNameUnderTest_)_(When)[PreCondition(s)]_ReturnsCorrectResult

    examples:
    WhenAgeLessThan18_ThrowException
    WhenContactIdExists_ReturnContact
    WhenContactIdIsNullOrWhitespace_ThrowArgumentException
    WhenContactIsCreated_PublishContactCreatedEvent
    WhenContactIsCreated_SendCreateContactCommand
    WhenNumbersArePositive_ShouldSumTwoNumbers
    ShouldSumTwoNumbers // omit the When_ part if there are no preconditions or only one precondition

### Class names
Class names have the **Given** part in BDD style, the context, which would result in a class name as **Given[Context]**.
Often we remove the _Given_ prefix, because it's redundant.

Often the context is the SUT (System-Under-Test), often the class under test (but it can differ if we want to test an
explicit behavior like an extension method). The method name under test can also be added to the class name, if the
number of tests methods is to many.

The group even more we can add the main preconditions to the class name, instead of the method name. Don`t add the
suffix _Test_ to the class name.

    (Given)[Context/ClassNameUnderTest](_[MethodNameUnderTest])(_When_[Main PreCondition(s)])

    examples:
    OpenTelemetryServiceClientDecorator
    OpenTelemetryServiceClientDecorator_Create
    OpenTelemetryServiceClientDecorator_Create_When_ServiceClientIsNull

Optional we can group test classes in a folder with the same name as the class under test. For folder names we use the
class name under test with the Suffix _Test_.

    [ClassNameUnderTest]Tests

    examples:
    OpenTelemetryServiceClientDecoratorTests

### Arrange, Act, Assert (AAA)

We follow the Arrange, Act, Assert (AAA) pattern for structuring our tests. This pattern is inspired by the
[AAA](https://en.wikipedia.org/wiki/Arrange-Act-Assert) (Arrange, Act, Assert) pattern.

    Arrange: setup the preconditions for the test
    Act: execute the method under test
    Assert: verify the expected outcome

```c#
    [Fact]
    public void WhenContactIdExists_ReturnsContact()
    {
        // Arrange
        var contactId = Guid.NewGuid();
        var contact = new Contact { Id = contactId };
        var contactRepository = Substitute.For<IContactRepository>();
        contactRepository.GetById(contactId).Returns(contact);
        var sut = new ContactService(contactRepository);

        // Act
        var result = sut.GetContact(contactId);

        // Assert
        Assert.Equal(contact, result);
    }
```

### Stub vs Mock

**Stubs** are used for querying (=reading) external dependencies that we don't control. We don't care about verifying
the calls, because it's input for the System Under Test (SUT). We are only interested in the outcome of the test.

Using [NSubstitue](https://nsubstitute.github.io/) (as our mocking framework) we can create a stub like so:

```c#
    // Arrange
    var stubSomeThing = Substitute.For<ISomeThing>();
    stubSomeThing.Execute(Arg.Any<string>).Returns("Hello world!");

    // Act

    // Assert
    // We don't assert a stub!
```

If possible try to not use stubs, but just call the real dependency, so that we know early when the dependency is
changed.

**Mocks** are used for commanding (changing) external dependencies that we don't control (unmanaged). We want to verify
that our command happened, because this is the output of our System Under Test (SUT).

```c#
    // Arrange
    var mockSomeThing = Substitute.For<ISomeThing>();
    mockSomeThing.Execute(Arg.Any<string>).Returns("Hello world!");

    // Act

    // Assert
    mockSomeThing.Recieved().Execute(); // this will verify that the mock has be called
```

White paper testing with mocks: https://www.jamesshore.com/v2/projects/testing-without-mocks/testing-without-mocks

For more info see:
https://enterprisecraftsmanship.com/posts/stubs-vs-mocks/
https://enterprisecraftsmanship.com/posts/when-to-mock/
