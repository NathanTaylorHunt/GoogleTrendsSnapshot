using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;

namespace GoogleTrendsSnapshot
{
    /// <summary>
    /// Load options from INI file.
    /// </summary>
    public class Options
    {
        public string GoogleTrendsUrl { get; set; }
        public string ChromeDriverDirectory { get; set; }
        public string SnapshotDirectory { get; set; }
        public string WindowSize { get; set; }
        public bool Headless { get; set; }
        public bool AutoCapitalize { get; set; }
        
        /// <summary>
        /// Disallow creating new, use factory methods.
        /// </summary>
        protected Options() {}

        /// <summary>
        /// Load options from INI file.
        /// </summary>
        /// <param name="path">Path to options INI file.</param>
        /// <returns>(Options, Success)</returns>
        public static (Options, bool) LoadOptions(string path)
        {
            // Search for INI file.
            if (!File.Exists(path)) return (GetDefaults(), false);

            // Parse INI file.
            var config = new Dictionary<string, string>();
            using (var reader = new StreamReader(path))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    // INI expected format:
                    // KEY=VALUE
                    var tokens = line.Split("=");
                    if (tokens.Length == 2)
                        config.Add(tokens[0], tokens[1]);
                }
            }

            // Attempt to parse individual options.
            var options = GetDefaults();
            options.GoogleTrendsUrl = GetOption(
                    config, "GoogleTrendsUrl", options.GoogleTrendsUrl);
            options.ChromeDriverDirectory = GetOption(
                    config, "ChromeDriverDirectory", options.ChromeDriverDirectory);
            options.SnapshotDirectory = GetOption(
                    config, "SnapshotDirectory", options.SnapshotDirectory);
            options.WindowSize = GetOption(
                    config, "WindowSize", options.WindowSize);
            options.Headless = GetOption(
                    config, "Headless", options.Headless);
            options.AutoCapitalize = GetOption(
                    config, "AutoCapitalize", options.AutoCapitalize);
            return (options, true);
        }

        /// <summary>
        /// Get default options.
        /// </summary>
        /// <returns>Options configured to defaults.</returns>
        public static Options GetDefaults()
        {
            var options = new Options();
            options.GoogleTrendsUrl = "https://trends.google.com/trends";
            options.SnapshotDirectory = "./snapshots";
            options.ChromeDriverDirectory = "./driver";
            options.WindowSize = "1920,1080";
            options.Headless = true;
            options.AutoCapitalize = true;
            return options;
        }

        /// <summary>
        /// Attempt to parse option.
        /// If key not found, or conversion
        /// invalid, return default value.
        /// </summary>
        /// <param name="options">Options dict.</param>
        /// <param name="key">Key to search by.</param>
        /// <param name="defaultValue">Default value.</param>
        /// <returns>Either parsed value or default.</returns>
        private static T GetOption<T>(
                Dictionary<string, string> options,
                string key,
                T defaultValue) where T : IConvertible
        {
            string value;
            if (!options.TryGetValue(key, out value)) return defaultValue;

            try     { return (T)Convert.ChangeType(value, typeof(T)); }
            catch   { return defaultValue; }
        }
    }
}
