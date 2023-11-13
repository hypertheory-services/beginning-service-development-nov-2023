# Beginning .NET Service Development

## The Landscape / Overview

Some businesses can have a single app they deploy, and a team of developers that build that app and the app can be deployed as a unit. This isn't a statement on value of the app, or even really complexity of the app - it's just a reflection that many businesses are straight-forward, a single mission, and a relatively small team of developers.

For example, StackOverflow has less then 20 developers working on the app. They can coordinate their efforts with one another, plan releases, etc. In these situations there is usually a single, monolithic code base all developers work in. Changes in one part of the application will give feedback (in terms of failing unit tests, build errors, etc.) if it would introduce regressions.

Other companies have dozens or hundreds of teams working together. Each team "owns" a specific area of business need (a "bounded context") and provide apps as _services_ that are aggregated together to provide overreaching business value.

Each of these teams usually works in separate code bases, from different sets of requirements, and each has it's own world of challenges and complexity.

Various methodologies including SOA (Service-Oriented-Architecture), Universal API Gateway, and Microservice Architecture etc. have been created to help address this kind of work.

### The Fundamental Challenges

As the inter-connection between these apps (services) grows, the ability for **Team Autonomy** seems to decrease. It becomes harder and harder for teams to be able to accomplish their dual responsibility of being _responsive to the needs of the business_ as well as making sure any changes don't break their customers both downstream and upstream.

While monolithic applications have their own problems, SOA-type approaches increase the complexity in sometimes surprising ways.

Big, mission critical services tend to become the "rulers". They become the services each other app needs to _conform_ to. These tend, over time, to become big, gargantuan messes ("Big ball of mud") services, often supporting multiple versions of the same functionality for many different consumers. The cohesion of these services becomes diluted, and often quickly fall into the quaint category of "Legacy Software" (using the definition here of legacy software being whatever services we all live in fear of and keep at arm's length.)

This is the first course in a series of courses. It is important to start, in this course, with a discussion (and demonstration) of some things that have changed. This style of development requires new techniques, new ways of thinking, and it may look strange if you don't have some background in what has changed.

In other words, you might think "Well, why didn't we do it this way all along? Is this just some new _fad_?".

The biggest reason - the _enabling technologies_. In my career, the history of ways we've come up with to improve developer experience while also staying more responsive to the needs of the business (eXtreme Programming, Agile Software Development, Lean Software Development, etc.) have been missing a few _key_ ingredients (spoiler: containers, CI/CD, orchestration).

We also need to talk about the _kinds_ of services you might build. We'll learn how to be more intentional about service/app design.

Additionally, we'll begin our journey of _how to actually_ create these things. In this series of courses we will primary use .NET as our platform.

## Course Materials Table of Contents

- [Course Description](https://hypertheory.training/services-training/devops-services)
- [Introduction](./introduction.md)
- [Twelve Factor App](./twelve-factor.md)
- [Micro Patterns](./micro-patterns.md)
- [Microservices](./microservices.md)
- [Containers](./containers.md)
  - [About Hashing](./hashing.md)
- [Networking Essentials](https://hypertheory.training/topics/networking/introduction)
- [Orchestration with Kubernetes](./kubernetes.md)
- [Concept Reference](./concepts.md)
- [Resources](./resources.md) - Links to helpful stuff.
