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
    public class Snapshot
    {
        /// <summary>
        /// Given a list of terms, take a snapshot of the
        /// Google Trends line graph.
        /// </summary>
        /// <returns>(Filename, Success)</returns>
        public static (string, bool) TakeSnapshot(List<string> searchTerms)
        {
            // TODO: Load options from config file.
            var options = new ChromeOptions();
            options.AddArgument("--headless");
            options.AddArgument("--window-size=1920,1080");

            using (var driver = new ChromeDriver(@"./driver", options))
            {
                try
                {
                    // 1. Navigate to Google Trends page.
                    driver.Navigate().GoToUrl("https://trends.google.com/trends");

                    // 2. Enter list of terms into search bar.
                    var searchBar = driver.FindElementByCssSelector("search input");
                    searchBar.SendKeys(String.Join(", ", searchTerms));
                    searchBar.SendKeys(Keys.Enter);
                    
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

                    // 4. Take snapshot.
                    var snapshot = (driver as ITakesScreenshot).GetScreenshot();
                    var rect = new Rectangle(
                        x: graphLocation.X,
                        y: headerLocation.Y,
                        width: graphSize.Width,
                        height: (graphLocation.Y + graphSize.Height) - headerLocation.Y
                    );
                    var originalSnapshot = Image.FromStream(new MemoryStream(snapshot.AsByteArray));
                    var croppedSnapshot = (originalSnapshot as Bitmap).Clone(rect, originalSnapshot.PixelFormat);

                    // TODO: Determine actual file name.
                    croppedSnapshot.Save("test_snapshot.png");
                }
                catch
                {
                    // TODO: Better handle error.
                    Console.WriteLine("SOMETHING WENT WRONG");
                    return ("", false);
                }
            }

            // TODO: Return actual filename.
            return ("result.png", true);
        }
    }
}