//==========================================================
// Solution to problem 2.
//==========================================================

using System;
using System.IO;
using System.Text.RegularExpressions;

namespace Exam1
{
    public class Problem2
    {
        public static void Main (String [] args)
        {           

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

            var regex = new Regex (@"[0-7]+", RegexOptions.Singleline);
            var sum = 0;
            foreach (Match m in regex.Matches (input)) {
                sum += Convert.ToInt32(m.Value, 8);
            }
            Console.WriteLine (Convert.ToString (sum, 8));
        }
    }
}
