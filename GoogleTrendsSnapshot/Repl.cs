using System;
using System.Text;
using System.Linq;
using System.Globalization;
using System.Collections.Generic;

namespace GoogleTrendsSnapshot
{
    public class Repl
    {
        private const string PROMPT_LINE = "> ";
        private enum Command
        {
            Literal,
            Exit,
            Snap,
            Delete,
            Clear
        }

        public void Run()
        {
            var searchTerms = new List<string>();
            while (true) {
                Console.Clear();
                Console.WriteLine("");
                Console.WriteLine("SEARCH TERMS: {0}", FormatList(searchTerms));
                Console.WriteLine("");
                Console.WriteLine("Type \'snap\' to take a snapshot.");
                Console.WriteLine("Type \'delete\' and a number to delete a search term.");
                Console.WriteLine("Type \'clear\' to clear all search terms.");
                Console.WriteLine("Type \'exit\' to quit.");
                Console.WriteLine("");
                
                var (nextTerm, termCommand) = ReadUserInput("ENTER A NEW SEARCH TERM:");
                switch (termCommand)
                {
                    case Command.Exit: return;
                    
                    case Command.Literal:
                        if (searchTerms.Count < 5)
                            searchTerms.Add(nextTerm);
                        break;
                    
                    case Command.Delete:
                        DeleteTerm(searchTerms, nextTerm);
                        break;

                    case Command.Clear:
                        searchTerms.Clear();
                        break;
                    
                    case Command.Snap:
                        Snapshot.TakeSnapshot(searchTerms);
                        break;
                }
            }
        }

        /// <summary>
        /// Read the user's input.  Check for 'exit' command.
        /// </summary>
        /// <param name="prompt">Prompt message</param>
        /// <returns>(User input, check for "exit" command)</returns>
        private (string, Command) ReadUserInput(string prompt = "")
        {
            Console.WriteLine(prompt);
            Console.Write(PROMPT_LINE);
            
            var user_input = Console.ReadLine();

            return (user_input, ParseCommand(user_input));
        }

        /// <summary>
        /// Determine if input matches command keyword.
        /// Input is converted to title case (ex: exit -> Exit).
        /// If input is multiple tokens, only parse the first.
        /// If no match, return Command.Literal.
        /// </summary>
        /// <param name="input">Input to parse.</param>
        /// <returns>Matching Command enum.</returns>
        private Command ParseCommand(string input)
        {
            Command result = Command.Literal;
            var firstToken = input.Split(" ")[0];
            var correctedInput = CultureInfo.InvariantCulture.TextInfo.ToTitleCase(firstToken);
            Enum.TryParse(correctedInput, out result);
            return result;
        }

        /// <summary>
        /// Format list of strings.
        /// If list is empty, return "NONE".
        /// Otherwise, return a numbered list
        /// with the following format:
        /// 
        /// (1) ITEM_1
        /// (2) ITEM_2
        /// ...
        /// (N) ITEM_N
        /// </summary>
        /// <param name="items">Collection of strings.</param>
        /// <returns>Formatted list.</returns>
        private string FormatList(ICollection<string> items)
        {
            if (items.Count == 0) return "NONE";

            var result = new StringBuilder();
            result.Append("\n");
            foreach (var it in items.Select((x, i) => new { Value = x, Index = i }))
                result.AppendLine(String.Format("  ({0}) \'{1}\'", it.Index+1, it.Value));
            return result.ToString();
        }

        /// <summary>
        /// Delete term from list by index.
        /// Parse second token of input as index,
        /// index starts at 1.
        /// </summary>
        /// <param name="terms">List of terms.</param>
        /// <param name="input">Index as raw string input.</param>
        private void DeleteTerm(List<string> terms, string input)
        {
            var index = 0;
            var tokens = input.Split(" ").ToArray();
            if (tokens.Length != 2) return;

            if (int.TryParse(tokens[1], out index)) {
                Console.WriteLine(index);
                if (index > 0 && index <= terms.Count)
                    terms.RemoveAt(index-1);
            }
        }
    }
}