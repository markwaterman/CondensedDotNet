using System;
using Condensed;
using Condensed.Linq;

class Program
{
    static void Main()
    {
        // Add 100 million dates spanning December 2016:
        var cc = new CondensedCollection<DateTime>(capacity: 100000000);
        for (int i = 0; i < 100000000; i++)
            cc.Add(new DateTime(2016, 12, i % 30 + 1));

        // Count the Tuesdays in our collection:
        var tuesCount = cc.Count(d => d.DayOfWeek == DayOfWeek.Tuesday);
        // 13,333,333
    }
}

