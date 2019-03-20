using System;
using System.Collections.Generic;

public class GeneratorExample {

    public static IEnumerable<int> Start() {
        var c = 1;
        while (c < 10000) {
            yield return c;
            c *= 2;
        }
    }

    public static void Main() {

        var e = Start().GetEnumerator();
        while (e.MoveNext()) {
            Console.WriteLine(e.Current);
        }

        foreach (var x in Start()) {
            Console.WriteLine(x);
        }
    }
}