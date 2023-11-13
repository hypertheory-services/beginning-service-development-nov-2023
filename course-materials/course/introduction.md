# Introduction

In this course we are emphasizing the developer skills and techniques necessary for us to realize the benefits of the CI/CD workflow. The gist of much of this is _shortening feedback loops_. This means a huge part of our practice and discipline as developers is using the human/machine interactions as a way to guide our work. The term "feedback loop" is a central philosophy in Cybernetics, a branch of engineering created by Norbert Weiner, beginning in the 1940's.

> "The core concept of cybernetics is circular causality or _feedback_-- where the observed outcomes of actions are taken as inputs for further action in ways that support the pursuit and maintenance of particular conditions, or their disruption" ... "Cybernetics is named after an example of circular causality, that of steering a shipt, where the helmsperson maintains a steady course in a changing environment by adjusting their steering in continual response to the effect it is observed as having"- [Wikipedia](https://en.wikipedia.org/wiki/Cybernetics)

In software development, we often talk about two specific _feedback loops_": The _inner_ feedback loop, and the _outer_ feedback loop. On its face, these are easy to understand:

- The inner feedback loop is everything that happens on the developers machine while writing, testing, and debugging code.
- The outer feedback loop is what happens when the code is pushed to source code control. This includes automated builds, testing, release management, automated code reviews, etc.

Depending on how long and where you've written code, for _many_ developers, the outer dev loop is **somebody else's job**. Part of our Devops transformation is for developers to take _some_ ownership of that process, while increasing their reliance on their inner feedback loop.

One part of the reason for this is that, historically, the outer feedback loop has a very slow response time, and even lower reliability. But I don't want to pretend like this is something we didn't already _know_. Entire sets of disciplines like eXtreme Programming (XP), Agile Software Development, and Lean Software Development (along with their attendant processes, including Scrum) have been pushing for _decades_ ways to minimize the feedback loop, or, more clearly, to shorten the amount of time between project or feature inception to having running code in front of the users.

We needed more than _process_, we needed tools and technology.

## The Marketing Pitch

Most of what you've heard about DevOps, about CI/CD, Microservices, all that jazz has (rightfully) centered around the term "Value Stream". This is, roughly, a set of principles and practices that make sure we are efficiently delivering "value" to the customers as a rapid clip. That sounds fun! That sounds like what we signed up for! It is definitely what our customers want.

We are now at the point where we can start to deliver on this promise. But it means we have to overcome a lot of inertia we have as developers. We've lived in an era of software development where, while we never intentionally _tried_ to slow the "value stream" (even if we didn't call that), we were keenly aware that writing software is a) hard, b) risky, and c) often doomed to fail. We've lost our confidence. We have made the easy things hard, and the hard things too expensive.

This course is designed to help you re-evaluate your values as a software developer. To look thoughtfully about the "way we do it" and evaluate if those practices make sense in the new landscape.

### Common "Problems" In Enterprise Software Development

- "Works on my Machine" Category of Errors
- Managing Complex Infrastructure
- Merge Conflicts in Long-Lived Branches
- Risky Deployments
  - Huge deployments contain huge risk
  - Without telemetry/instrumentation we had fear of a Titanic level failure
    - Over-Tested
    - Levels of bureaucracy to minimize risk
    - One "software development lifecycle" to rule them all.
    - Standardized tools
    - Over-reliance on home-grown frameworks and libraries

### Solutions Presented in this Course

- Dev/Prod Parity (Containers, Kubernetes, Etc.)
- Infrastructure as a Service (Ansible, Kubernetes, etc.)
- Continuous Integration - Trunk Based Development
- Continuos Delivery
  - Frequent (daily!) small releases contain lower risks.
  - Visibility into running applications
    - "Testing in Production"
    - Green/Blue Deployments
    - Canary Users
    - (Nearly) instantaneous rollback

All of this is _super cool_ but we have to learn how to do it. Devops is not just a thing we can _buy_ - it's a thing we have to do, and it starts with what we do every day - sitting down in our IDE and writing code.

We do have to work a little more. We have to learn a few things new, and, frankly, learn a few things we probably should have known already. But the pay off for us is immense. You `developer experience` will increase astronomically, but it will take a bit of time and patience and _attention_.
