# A Bit About Hashing

A hash function is a one-way function that maps data of an arbitrary size to a bit array of a fixed size.

These functions must be *deterministic* - meaning that given the same input (*message*), they always produce the same result (*hash*, *hash value* or *message digest*).

They use complex mathematics to ideally:

- Compute the value quickly.
- Make them one way (e.g. You can't figure out the message from the hash).
- Not able to create the same hash from two different messages (collision)
- A small change to the message should create a new value that is extensively different from the original hash.

The ability to fulfill these expectations is dependent on the algorithm used.

And MD5 hash will have more collisions than a SHA256 hash, for example. But SHA256 is slower than an MD5.

There are always tradeoffs.

## Hashes are Valuable Outside of Cryptography

While hashing algorithms are used in various security and cryptography practices, they are also useful for common programming tasks. For example, if we need to see if two files have the exact same content, we can run each file through a hashing algorithm and compare the values.

## Try It

If you have access to a Linux command prompt, you probably can run the following experiment.

Run the following command. This says "echo (print) the results, without a newline (-n), but pushing (piping, '|') this string through the md5sum tool and display the value.

```bash
$ echo -n 'Hello World!' | md5sum
```

This should produce a result that looks like this. Actually, the has should be *exactly* this if you gave the *exact* same message (`Hello World!`).

```bash
ed076287532e86365e841e92bfc50d8c  -
```

If you modify the message slightly and run it again (notice the added comma), you should get a completely different has value.

```bash
$ echo -n 'Hello, World!' | md5sum
```

Will produce:

```bash
bea8252ff4e80f41719ea13cdf007273  -
```

Hashes are used in programming for a lot of different scenarios:
    
- Comparing commits in source control (each commit is identified by a hash)
- Comparing large files (like layers in container images)
- Creating "cache busting" file names for HTTP resources (JavaScript, CSS, etc.)
