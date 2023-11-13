# Using an Init Container

In this lab we will build an Init Container to run before our OnCallApi runs to ensure that the database is migrated.

This is a complex issue, too - and this approach is only reasonable if:

- The Migration is only going to be used by that container.
- You aren't sharing databases. 