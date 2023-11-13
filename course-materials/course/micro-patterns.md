# Micro-Patterns For CI/CD

> Learn to understand and recognize the low-level decisions and approaches in software development and how they empower (or destroy) our ability to put code into production continually. We will explore hypothesis driven development as a way to prove the necessity of a new feature before investing heavily in its development. We will begin to change our mindset as developers from "What can we add in our next sprint to move things forward" to "what can we add today to make sure we are building the right thing?"

## Some of Our Software Development ("Inner") Feedback Loops

- Editor Setup and Configuration
- Compiler Messages
- Language Servers
- Linting
- Automated Testing
- Dev/Prod Parity

### IDE Setup and Configuration

Knowing how to use your Integrated Development Environment and respond to feedback provided by the tools is key to "staying in the zone". Knowing how to easily navigate a code base, search for patterns within files, and use the code refactoring tools is requisite knowledge for a developer.

### User and Workspace Settings in VSCode

While not directly related to linting, a handy feature in VS Code is the ability to store the baseline editor settings, code snippets, and list of useful extensions with the source code. These settings will override the users settings when working with that code base, and they will be prompted to install the extensions. [See this](https://code.visualstudio.com/docs/getstarted/settings) for more details.

### Compiler Messages

When working in a language that supports compilation and a reasonable type system, we get a very short feedback loop by both the compiler error messages and (configurable) warnings, as well as assistance from our editors (code completion, documentation, etc.). The compiler, of course, can only tell us if we are using the language properly, and are using the types we are interacting with in a way they expect to be interacted with.

> The choice of our programming languages and environments has a direct effect on our ability to receive feedback from our tools _as we write our code_. Dynamic programming languages and interpreted languages often do not provide the high level of feedback needed when writing code, so developers supplement the compiler with unit tests.

### Language Servers

Many modern languages and frameworks contain code analysis tools that run during development that can provide the developer feedback about the use of the language, and even the optimal use of the language within a particular framework. Some examples:

- Roslyn for C# Development
- TypeScript Language Service
- Angular Language Service
- The Go Language Service

These tools expose APIs that interact with your development tools to give you hints and suggestions, including suggesting _refactorings_ and even possible misconfiguration for use in frameworks (like the Angular Language Service)

> Learn how to properly configure and use language services and access the help and feedback provided by them.

### Linting

Many programming languages and environments support the concept of `linting`. Linting is a post-compilation step (usually) that run additional, deeper rules _above and beyond_ what the compiler offers. Sometimes the rules that are configured are to steer developers clear of deprecated language features that, while still technically supported, for various reasons should be avoided in new code. (for example, the `disallow-var` rule in ESLint).

- Common
  - [Editor Config](https://editorconfig.org/)
- For .NET -
  - [DotnetFormat](https://github.com/dotnet/format)
- For TypeScript / JavaScript
  - [ESLint](https://eslint.org/)
  - [Creating ESLint Rules](https://eslint.org/docs/developer-guide/working-with-rules)

> Linting of code bases should be based upon a set of rules agreed upon by the team. The rules should be kept in source code control and changes should be documented. As code bases and "best practices" by the team are discovered, they should, when possible, be reified into the linting rules.

A fantastic example of a code linter helping developers write better code is in the set of rules created for developers using the NGRX redux library in Angular applications (https://www.npmjs.com/package/eslint-plugin-ngrx).

### Automated Testing

Automated testing is covered [Here](./automated-testing.md), but a few comments on how they relate to our inner development loop.

Some obvious points, I hope, is that we write tests to gain _confidence_ in our code. As any software tester will tell you, it is impossible to write enough tests to prove a system of any complexity 100% "correct" and bug free. Even when we can come _close_ to "bug free code", so many other variables come in to play in the actual "real world" environment in which that code runs that to just focus on "bugs" won't do you much good when, say, your code is running on an outdated, off-brand Android device behind a company firewall with spotty internet and the user is jumping on an elevator while answering a call. Our code always exists in an _ecosystem_ of other code and dependencies (networks, device memory, failing hard drives, etc.).

Our inner loop tests have to be designed for the following things:

- How well they implement the requirements. ("External Code Quality")
- How quickly they give us feedback when we "stray".
- Detecting shallow regressions.
- Guiding us away from naive, obvious implementations that have proven to be problematic
- Documenting difficult to understand design decisions in code.

This list is by no means extensive, but the guiding principle is that tests have to be designed to create confidence in the code we deliver while simultaneously minimizing friction in delivering new value.

[^1]: https://www.systemsinnovation.io/post/cybernetics

### Dev/Prod Parity

Our code often runs in several environments, including our developer machines, QA environments, and Production environments. The more we gain parity between those environments, the more confidence we have in our code moving successfully from one environment to the next.

The differences between the environments have to be part of the application configuration, and that configuration is a key part of our code base.

Imagine a utopia where you could be doing your development work on a machine configured _identically_ to the machine your code will run in production, including memory, processor, hard drive space, access to the network, etc.

We can't give each developer huge multi-core servers to work on day to day. Instead, we shrink all running code to the size that can run on a small linux "machine", with a protected, bounded set of resources, and we ship that "machine" across environments. This is the role of [Containers](./containers.md).

This is a hugely _important_ feedback loop that we have to be diligent about. If you are writing code that, as an application developer, cannot run on your local developer laptop the problem certainly _may_ be you have an underpowered laptop, but really the problem is more likely that what you are writing is just too darned big. Decompose that action.

## The Outer Feedback Loops

TODO: Pipelines, Build Servers, Releases, Etc.

## Software Development Patterns

Continuous Integration

In order for a feedback loop to work, there must be some kind of "control". In other words, there must be some measure of what is considered _optimal_, and then feedback can be given when we stray from that measure.

An incrementalist approach has been the usual tactic. We might say "We want to slightly increase our velocity next sprint", or "we want to decrease the number of defects reported over X number of months".

Continuous Integration is such a control. It aims high. It says:

> The optimum developer and customer experience would be new code delivered each and every day, at a _minimum_.

For me, _this_ is the paradigm shift. It seemed insurmountable (and sometimes still is).

It is _super easy_ to start discounting this style of work. Perhaps you already are. "Well, we work on _very important_ things here!", and "regulations! testing!", all that.

But let's start from the point of view of agreeing on something:

> Wouldn't it be cool _if_ every day the code I write ends up in front of my users? I get instantaneous feedback. I end up wasting a lot less time building something that nobody uses. And in the off chance what I create sucks in some way, well, I can get rid of it just as quickly (or quicker). When there _are_ issues, or suggestions, or features, they are based on _reality_ - what they are actually seeing in front of them. I get that information while the code is still fresh in my mind - the next day often - and can continue to crank until everyone is happy.

Continuous Integration means you will practice the discipline of holding yourself to committing you code directly to the main branch of your source code repository. You have done all you can to be confident that this code _could_ be delivered to your users, your customers, immediately. Whether it does so or not might be above your pay grade. There are testers, there are compliance people, there are business people contemplating strategy, release planners, all that, but you did your part.

Let yourself bask in that dream for a moment or two before you dismiss it. We are looking for that "well, that would be nice" feeling, which the cynics would call "dreaming", but we, as DevOps developers, say "That is optimal. My feedback loop will control on that state. Variations from it will be addressed and taken seriously."

Other than no more "merge conflict hell", the value in the CI approach is in the way it changes how we think about writing and delivering software.

If you are committing to main every day, and you are assuming that the code you have committed _will_ be placed in front of our users, then your thought process must be:

1. What can I do today that will deliver immediate value? How do I break a big task up into small, incremental, steps?
2. If what I'm working on will take _longer_ than one day to accomplish, how can I write this code so that when it _is_ ready, and released, the new feature (or replacement for a feature) can be "toggled" on?

> Let me be perfectly clear: CI means you will _often_ commit code that will be delivered to users that isn't "ready" yet. We will explore various ways to do this safely. For many this sounds heretical. But I assure you, once you grok it, it provides many advantages.
