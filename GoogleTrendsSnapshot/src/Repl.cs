using System;
using System.Text;
using System.Linq;
using System.Collections.Generic;

namespace GoogleTrendsSnapshot
{
    /// <summary>
    /// Read, print, eval, loop.
    /// Command interface for the user.
    /// </summary>
    public class Repl
    {
        private const string PROMPT_LINE = "> ";

        /// <summary>
        /// Commands that the user can use.
        /// Note: Commands are uppercase
        /// for easier string matching.
        /// </summary>
        private enum Command
        {
            LITERAL,
            EXIT,
            SNAP,
            DELETE,
            CLEAR
        }

        /// <summary>
        /// Start the repl, interpret user input.
        /// </summary>
        public void Run(Options options)
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
                
                var prompt = "ENTER A NEW SEARCH TERM:";
                var (nextTerm, termCommand) = ReadUserInput(prompt, options.AutoCapitalize);
                switch (termCommand)
                {
                    case Command.EXIT: return;
                    
                    case Command.LITERAL:
                        AddTerm(searchTerms, nextTerm);
                        break;
                    
                    case Command.DELETE:
                        DeleteTerm(searchTerms, nextTerm);
                        break;

                    case Command.CLEAR:
                        ClearTerms(searchTerms);
                        break;
                    
                    case Command.SNAP:
                        TakeSnapshot(searchTerms, options);
                        break;
                }
            }
        }

        /// <summary>
        /// Read the user's input.  Check for 'exit' command.
        /// </summary>
        /// <param name="prompt">Prompt message.</param>
        /// <param name="autoCap">Auto capitalize.</param>
        /// <returns>(User input, check for "exit" command)</returns>
        private (string, Command) ReadUserInput(string prompt = "", bool autoCap = true)
        {
            Console.WriteLine(prompt);
            Console.Write(PROMPT_LINE);
            
            var user_input = Console.ReadLine();
            if (autoCap) user_input = user_input.ToUpper();

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
            Command result = Command.LITERAL;
            var firstToken = input.Split(" ")[0];
            Enum.TryParse(firstToken.ToUpper(), out result);
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
        /// Add new term to list. Limit to 5 max.
        /// </summary>
        /// <param name="terms">List of terms.</param>
        /// <param name="newTerm">New term.</param>
        private void AddTerm(List<string> terms, string newTerm)
        {
            int maxTerms = 5; // @Hardcoded, Google Trends only allows 5 terms max.
            if (terms.Count < maxTerms)
                terms.Add(newTerm);
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

        /// <summary>
        /// Clear all terms.
        /// </summary>
        /// <param name="terms">List of terms.</param>
        private void ClearTerms(List<string> terms)
        {
            terms.Clear();
        }

        /// <summary>
        /// Take a snapshot and report it's success and filename.
        /// </summary>
        /// <param name="terms">Search terms.</param>
        /// <param name="options">Options.</param>
        private void TakeSnapshot(List<string> terms, Options options)
        {
            Console.WriteLine("\nTAKING SNAPSHOT - PLEASE WAIT...\n");

            var (filename, success) = Snapshot.TakeSnapshot(terms, options);
            if (!success)   Console.WriteLine("\nERROR: Snapshot failed.");
            else            Console.WriteLine("\nSNAPSHOT COMPLETE");

            Console.WriteLine("\nPRESS ANY KEY TO CONTINUE");
            Console.ReadKey();
        }
    }
}