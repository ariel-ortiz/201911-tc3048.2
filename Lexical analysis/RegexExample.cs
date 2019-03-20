using System;
using System.IO;
using System.Text.RegularExpressions;

public class RegexExample {
    public static void Main(String[] args) {
        var regex = new Regex(@"([/][*].*?[*][/])|(.)",
                              RegexOptions.Singleline);
        var text = File.ReadAllText("hello.c");
        foreach (Match m in regex.Matches(text)) {
            if (m.Groups[2].Success) {
                Console.Write(m.Value);
            }
        }
    }
}
