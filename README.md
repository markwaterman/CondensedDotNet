# Condensed Collection Library for .NET

The **Condensed Collection Library for .NET** provides the [CondensedCollection](http://www.markwaterman.net/docs/condenseddotnet/html/567ca794-b467-8736-0b20-caacc958e8a9.htm)  class, an IList<T> implementation that uses interning (deduplication) for efficient storage of large numbers of immutable elements.

* [NuGet package](https://www.nuget.org/packages/Condensed)
* [Documentation](http://www.markwaterman.net/docs/condenseddotnet/)

## Usage

The CondensedCollection class provides a generalized form of interning for any immutable .NET type. Use this collection like an ordinary list, keeping the following guidelines in mind:

1. The type stored in the collection **must be [immutable](http://www.markwaterman.net/docs/condenseddotnet/html/12336e43-b0f4-46fb-a17b-c640871bddab.htm)**.
2. Your type must provide a way to compare its instances for equality.
3. If the contents of the collection are subject to change after the initial load, you want to perform [memory reclamation](http://www.markwaterman.net/docs/condenseddotnet/html/606626e5-fb28-47c5-939f-a87c14d4f99a.htm) from the collection's intern pool.

**Basic Usage**
```csharp
using Condensed;
using Condensed.Linq;
//...

var cc = new CondensedCollection<DateTime>(capacity: 100000000);

// Add 100 million dates spanning December 2016:
for (int i = 0; i < 100000000; i++)
    cc.Add(new DateTime(2016, 12, i % 30 + 1));

// Memory usage: 100 MB (versus ~780 MB for an ordinary List<DateTime>)

// Count the Tuesdays in our collection:
var tuesCount = cc.Count(d => d.DayOfWeek == DayOfWeek.Tuesday);

// 13,333,333 (counted in 0.002 seconds, vs 5 seconds for List<DateTime>)
```

Internally, the CondensedCollection uses a variable-width index to reference the unique values in its intern pool. Given sufficient repetition in your collection, you can achieve significant memory savings even when storing small types. In the example above, references to the 31 unique days in a month can be held in an internal index that is just one byte wide.

Interning items can reduce memory consumption and can often improve performance for certain compute-intensive tasks. The trade-off is that modifications to the collection are slower. The Wikipedia article on [string interning](https://en.wikipedia.org/wiki/String_interning) offers good background on the performance/space tradeoffs involved.

## Features

The CondensedCollection provides the following additional features:
* Specialized LINQ operators in the namespace that are optimized to work on the collection. (Add a "`using Condensed.Linq;`" statement to source files that need to use these extension methods.)
* Optional automatic fallback to non-deduplicated list behavior if the items in the collection aren't sufficiently unique to make the interning overhead worthwhile.
* Support for nullable types.
* Flexible options for reclaiming memory.

## Thread Safety

CondensedCollection is _not_ thread-safe and does not perform any internal synchronization. Multiple simultaneous readers are allowed, provided there is no active writer. An exclusive lock or a ReaderWriterLockSlim should be used to synchronize multi-threaded access to each instance of the collection.

## Status

Alpha, suitable for internal testing and experimentation. Feedback is appreciated!

## License

Apache 2.0