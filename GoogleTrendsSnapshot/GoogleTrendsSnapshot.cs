using System;
using System.IO;

namespace GoogleTrendsSnapshot
{
    public class GoogleTrendsSnapshot
    {
        public static void Main(string[] args)
        {
            // Hello message.
            Console.Clear();
            Console.WriteLine("\nWELCOME TO GOOGLE TRENDS SNAPSHOT\n");

            var (options, success) = Options.LoadOptions("config.ini");
            
            if (!success) Console.WriteLine("ERROR: Failed to load config.inig, using defaults.");
            Console.WriteLine("\nPRESS ANY KEY TO CONTINUE");
            Console.ReadKey();

            // REPL
            var repl = new Repl();
            repl.Run(options);

            // Goodbye message.
            Console.Clear();
            Console.WriteLine("\nGoodbye :)");
        }
    }
}
