using System;
using System.IO;
using System.Collections.Generic;

using System.Drawing;
using System.Drawing.Imaging;

using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using OpenQA.Selenium.Interactions;

namespace GoogleTrendsSnapshot
{
    /// <summary>
    /// Use Selenium to take snapshot of Google Trends.
    /// </summary>
    public abstract class Snapshot
    {
        /// <summary>
        /// Given a list of terms, take a snapshot of the
        /// Google Trends line graph.
        /// </summary>
        /// <returns>(Filename, Success)</returns>
        public static (string, bool) TakeSnapshot(List<string> searchTerms, Options options)
        {
            // Ensure chromedriver.exe can be found.
            var chromeDriverDir = Path.GetFullPath(options.ChromeDriverDirectory);
            if (!File.Exists(chromeDriverDir + "/chromedriver.exe"))
            {
                Console.WriteLine(">>> chromedriver.exe NOT FOUND");
                Console.WriteLine(">>> " + chromeDriverDir);
                return (null, false);
            }

            // Set ChromeDriver options.
            var chromeOptions = new ChromeOptions();
            if (options.Headless) chromeOptions.AddArgument("--headless");
            chromeOptions.AddArgument("--window-size="+options.WindowSize);
            chromeOptions.AddArgument("--log-level=3");
            chromeOptions.AddArgument("--disable-extensions");

            var service = ChromeDriverService.CreateDefaultService(chromeDriverDir);
            service.SuppressInitialDiagnosticInformation = true;
            service.EnableVerboseLogging = false;
            service.HideCommandPromptWindow = true;

            // Launch ChromeDriver
            string filename;
            using (var driver = new ChromeDriver(service, chromeOptions))
            {
                try
                {
                    // 0. (Implicit) Open ChromeDriver.
                    Console.WriteLine("> Opened ChromeDriver");

                    // 1. Navigate to Google Trends page.
                    driver.Navigate().GoToUrl(options.GoogleTrendsUrl);
                    Console.WriteLine("> Navigated to {0}", options.GoogleTrendsUrl);

                    // 2. Enter list of terms into search bar.
                    var searchBar = driver.FindElementByCssSelector("search input");
                    var searchInput = String.Join(", ", searchTerms);
                    searchBar.SendKeys(searchInput);
                    searchBar.SendKeys(Keys.Enter);
                    Console.WriteLine("> Entered terms: {0}", searchInput);
                    
                    // 3. Setup snapshot (hover over most recent results).
                    var header = driver.FindElementByClassName("explorepage-content-header");
                    var headerLocation = header.Location;
                    var graph = driver.FindElementByTagName("widget");
                    var graphLocation = graph.Location;
                    var graphSize = graph.Size;

                    var lineChart = driver.FindElementByCssSelector("line-chart-directive svg");
                    var hoverAction = new Actions(driver);
                    hoverAction.MoveToElement(
                        lineChart,
                        (int) Math.Round(lineChart.Size.Width * 0.98f),
                        (int) Math.Round(lineChart.Size.Height / 2f));
                    hoverAction.Perform();
                    Console.WriteLine("> Setup trends graph.");

                    // 4. Take snapshot.
                    var snapshot = (driver as ITakesScreenshot).GetScreenshot();
                    Console.WriteLine("> Took snapshot.");

                    // 5. Crop snapshot.
                    var rect = new Rectangle(
                        x: graphLocation.X,
                        y: headerLocation.Y,
                        width: graphSize.Width,
                        height: (graphLocation.Y + graphSize.Height) - headerLocation.Y
                    );
                    var originalSnapshot = Image.FromStream(new MemoryStream(snapshot.AsByteArray));
                    var croppedSnapshot = (originalSnapshot as Bitmap).Clone(rect, originalSnapshot.PixelFormat);
                    Console.WriteLine("> Cropped snapshot.");

                    // 5. Save file.
                    filename = GetSnapshotFileName(options.SnapshotDirectory, "png");
                    croppedSnapshot.Save(filename, ImageFormat.Png);
                    Console.WriteLine("> Saved snapshot - {0}", filename);
                }
                catch (Exception e)
                {
                    Console.WriteLine("\n>>> {0}\n", e.Message);
                    return (null, false);
                }
            }

            return (filename, true);
        }

        /// <summary>
        /// Determine filename for snapshot.
        /// Format:
        /// 000-snapshot.EXT
        /// Number is determined by total files
        /// already in directory.
        /// </summary>
        /// <param name="directory">Snapshot directory.</param>
        /// <param name="extension">File extension.</param>
        /// <returns>Filename for snapshot.</returns>
        public static string GetSnapshotFileName(string directory, string extension)
        {
            var files = Directory.GetFiles(directory, "*", SearchOption.TopDirectoryOnly);
            var fileCount = files.Length + 1;
            return string.Format("{0}/{1}_snapshot.{2}",
                    directory,
                    fileCount.ToString().PadLeft(3, '0'),
                    extension);
        }
    }
}