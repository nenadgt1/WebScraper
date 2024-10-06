using HtmlAgilityPack;
using MySql.Data.MySqlClient;
using System;

class Program
{
    static void Main(string[] args)
    {
        string url = "https://reklama5.mk/Search?city=&cat=24&q=";

        var web = new HtmlWeb();
        var doc = web.Load(url);
        Console.WriteLine("Web page loaded successfully.");

        var adNodes = doc.DocumentNode.SelectNodes("//div[@class='row ad-top-div']");

        if (adNodes == null || adNodes.Count == 0)
        {
            Console.WriteLine("No ads found on the page.");
            return;
        }

        foreach (var adNode in adNodes)
        {
            try
            {
                var adTitleNode = adNode.SelectSingleNode(".//h3/a[@class='SearchAdTitle']");
                var adTitle = adTitleNode?.InnerText.Trim();
                var adUrl = "https://reklama5.mk" + adTitleNode?.GetAttributeValue("href", string.Empty);

                var adPriceNode = adNode.SelectSingleNode(".//span[@class='search-ad-price']");
                var adPrice = adPriceNode?.InnerText.Trim();

                Console.WriteLine($"Scraping ad: {adTitle}");

                var adDoc = web.Load(adUrl);
                var carMake = adDoc.DocumentNode.SelectSingleNode("//div[1]/p")?.InnerText.Trim();
                var carModel = adDoc.DocumentNode.SelectSingleNode("//div[2]/p")?.InnerText.Trim();
                var carYear = adDoc.DocumentNode.SelectSingleNode("//div[3]/p")?.InnerText.Trim();
                var carMileage = adDoc.DocumentNode.SelectSingleNode("//div[4]/p")?.InnerText.Trim();

                Console.WriteLine($"Extracted Data - Make: {carMake}, Model: {carModel}, Year: {carYear}, Mileage: {carMileage}, Price: {adPrice}");

                string connectionString = "Server=34.107.109.154;Database=car_ads;Uid=scraper_user;Pwd=password;";
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "INSERT INTO cars (make, title, model, year, mileage, price, ad_number) VALUES (@make, @title, @model, @year, @mileage, @price, @adNumber)";
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@make", string.IsNullOrEmpty(carMake) ? DBNull.Value : carMake);
                    cmd.Parameters.AddWithValue("@title", string.IsNullOrEmpty(adTitle) ? DBNull.Value : adTitle);
                    cmd.Parameters.AddWithValue("@model", string.IsNullOrEmpty(carModel) ? DBNull.Value : carModel);
                    cmd.Parameters.AddWithValue("@year", string.IsNullOrEmpty(carYear) ? DBNull.Value : carYear);
                    cmd.Parameters.AddWithValue("@mileage", string.IsNullOrEmpty(carMileage) ? DBNull.Value : carMileage);
                    cmd.Parameters.AddWithValue("@price", string.IsNullOrEmpty(adPrice) ? DBNull.Value : adPrice);
                    cmd.Parameters.AddWithValue("@adNumber", string.IsNullOrEmpty(adUrl) ? DBNull.Value : adUrl);


                    cmd.ExecuteNonQuery();
                    Console.WriteLine($"Ad saved to database: {adTitle}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error processing ad: {ex.Message}");
            }
        }
    }
}
