//==========================================================
// Solution to problem 1.
//==========================================================

using System;
using System.IO;
using System.Text.RegularExpressions;

namespace Exam1
{
    public class Problem1
    {
        public static void Main (String [] args)
        {
            String [] monthWord = {"January", "February", "March", "April", "May", "June", "July",
                "August", "September", "October", "November", "December"};

            if (args.Length != 1) {
                Console.Error.WriteLine ("Please specify the name of the input file.");
                Environment.Exit (1);
            }
            var inputPath = args [0];
            var input = "";
            try {
                input = File.ReadAllText (inputPath);
            } catch (FileNotFoundException e) {
                Console.Error.WriteLine (e.Message);
                Environment.Exit (1);
            }

            var regex = new Regex (@"((\d{4})-(\d{2})-(\d{2}))|(.)", RegexOptions.Singleline);
            foreach (Match m in regex.Matches (input)) {
                if (m.Groups [1].Success) {
                    var year = Convert.ToInt32 (m.Groups [2].Value);
                    var month = Convert.ToInt32 (m.Groups [3].Value) - 1;
                    var day = Convert.ToInt32 (m.Groups [4].Value);
                    Console.Write ($"{ monthWord[month] } { day }, { year }");
                } else {
                    Console.Write (m.Value);
                }
            }
        }
    }
}
