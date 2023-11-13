# Developer Testing

It's all a confidence game. All of our work in the inner loop is to gain confidence in the code we push to the main branch.

_Sometimes_ as developers we write unit tests to guide the design of our code. Test-Driven Development is the name given to this style of testing, and it is good for designing algorithmically complex code that is not easily verified through the external (public) interface of the application.

> **About Interfaces** When I say "interface" or "public interface" here I mean the way the user of our application or service will interact with our code. The entire _purpose_ of user interface is to **limit the possible inputs** from the user. API interfaces do that to a lesser extent.

The kinds of code we deliver as software application developers could be categorized in the following way:

1. Applications.
   - Applications use _libraries_ and _frameworks_ to deliver value to customers. These are delivered as services and user interfaces of various kinds. Application is defined, in this use, as "the action of putting something into operation".
   - Applications tend to change at a higher rate than libraries or frameworks.
   - Libraries and Frameworks are best _extracted_ from applications.
   - Tend to have _less_ Unit Tests because most of the functionality can be tested through the public interfaces.
   - Unit Tests tend to tie you to a specific implementation in your application, and hurt velocity.
2. Libraries
   - Represent code that is useful across applications
   - Libraries are "code that our code uses"
   - Tend to be more stable than applications (not as many changes).
   - Are usually tied to a specific programming environment
   - Delivered and versioned _declaratively_ to the code using dependency managers.
   - Often need _more_ unit tests than applications because the context in which they are to be used is mysterious.
   - Rarely can be _integration_ tested because they are not useful on their own.
3. Frameworks
   - Represent code that is meant to be "built upon" to speed application development.
   - Examples include ASP.NET MVC, Angular
   - Contain their own tests
   - We _may_ need to test how our application interacts with the framework, but that is rare.
     - A counter-example might be writing tests on a .NET API to see if the route table is configured properly.
     - If you can verify the correct code is executed upon a specific request, the "how" of that is not needed to be verified.

> Warning: Heretical Opinion Ahead

The presence of Unit Tests in application code is often a _code smell_. The strength of the _smell_ will be apparent in a couple of ways:

1. Decreased automated test run velocity.
2. Decrease developer experience (e.g. "Half my time is spent updating tests!")
3. Feeling locked in to a particular implementation because of tests.

Unit tests written by developers as a design tool (TDD) that are present in application code often lose their value after the code is written.

If there are Unit Tests for code in an application that is shared across portions of the application, strongly consider moving that to a library.
