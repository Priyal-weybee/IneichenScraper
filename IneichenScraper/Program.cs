
using CsvHelper;
using HtmlAgilityPack;
using System;
using System.Configuration;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading.Tasks;
using static System.Collections.Specialized.BitVector32;
using System.Text.RegularExpressions;
using System.Security.Cryptography.X509Certificates;
using System.Data.SqlClient;
using System.Data;
using static IneichenScraper.Program;

namespace IneichenScraper
{
    internal class Program
    {


        public class IneichenProduct
        {
            public int Id { get; set; }

            public string Title { get; set; }

            public string Description { get; set; }

            public string ImageUrl { get; set; }

            public string Link { get; set; }

            public string LotCount { get; set; }

            public string StartDate { get; set; }

            public string StartMonth { get; set; }

            public string StartYear { get; set; }

            public string StartTime { get; set; }

            public string EndDate { get; set; }

            public string EndMonth { get; set; }

            public string EndYear { get; set; }

            public string EndTime { get; set; }

            public string Location { get; set; }
        }


        static void Main(string[] args)
        {
            var IPobj = new List<IneichenProduct>();
            var web = new HtmlWeb();
            var document = web.Load("https://ineichen.com/auctions/past/");

            var product = document.DocumentNode.SelectNodes(".//div/div[@class='auction-item']");


            foreach (var items in product)
            {


                var titleNode = items.SelectSingleNode(".//div[@class='auction-item__title']").InnerText.Trim();
                string title = titleNode != null ? titleNode : " ";

                var descriptionNode = items.SelectSingleNode(".//div[@class='auction-date-location']").InnerText.Trim();
                string description = descriptionNode != null ? descriptionNode : null;
           


                var imageAttribute = items.SelectSingleNode(".//a[@class='auction-item__image']/img");
                string imageUrl = imageAttribute.GetAttributeValue("src", "");
               

                var linkNode = items.SelectSingleNode(".//div[@class='auction-item__btns']/a");
                string linkUrl = linkNode != null ? linkNode.GetAttributeValue("href", "") : "";


            
                string lotCount = items.SelectSingleNode(".//div[@class='auction-item__btns']/a").InnerText;
                Match match_lotCount = Regex.Match(lotCount, @"(\d{2})");
                string matched_lotCount = (match_lotCount.Groups[1].Value);



                var StartDate = items.SelectSingleNode(".//div[@class='auction-date-location']/div[1]").InnerText.Trim();
                Match match_StartDate = Regex.Match(StartDate, @"^(\d{1,2})");
                string matched_StartDate = (match_StartDate.Groups[1].Value);


                var StartMonth = items.SelectSingleNode(".//div[@class='auction-date-location']/div[1]").InnerText.Trim();
                Match match_StartMonth = (Regex.Match(StartMonth, @"^\d{1,2}(.|-)?(\d{1,2})?(\w*)"));
                string matched_StartMonth = match_StartMonth.Groups[3].Value;


                var StartYear = items.SelectSingleNode(".//div[@class='auction-date-location']/div[1]").InnerText.Trim();
                Match match_StartYear = (Regex.Match(StartYear, @"\d{4}"));
                string matched_StartYear = match_StartYear.Value;
             


                var StartTime = items.SelectSingleNode(".//div[@class='auction-date-location']/div[1]").InnerText.Trim();
                Match match_StartTime = (Regex.Match(StartTime, @"(\d{2}:\d{2})"));
                string matched_StartTime = match_StartTime.Value;




                var EndDate = items.SelectSingleNode(".//div[@class='auction-date-location']/div[1]").InnerText.Trim();
               // Match match_EndDate = Regex.Match(EndDate, @"(\d{1,2})([a-zA-Z]*)((\d{4})?((\d{2}:\d{2})CET)?)$");
                Match match_EndDate = Regex.Match(EndDate, @"(-)\s?(\d{1,2})");
                 string matched_EndDate = match_EndDate.Groups[2].Value;



                var EndMonth = items.SelectSingleNode(".//div[@class='auction-date-location']/div[1]").InnerText.Trim();
               // Match match_EndMonth = Regex.Match(EndMonth, @"(-)\s?\d{1,2}\s?([a-zA-Z]*)\s?(\d{4})?");
                Match match_EndMonth = Regex.Match(EndMonth, @"(-)\s?(\d{1,2})\s([a-zA-Z]*)");
               // Match match_EndMonth = Regex.Match(EndMonth, @"((-)\s?(\d{1,2})\s([a-zA-Z]*))?(CET\s(\d{1,2})\s([a-zA-Z]*))?");

                string matched_EndMonth = match_EndMonth.Groups[3].Value;


                var EndYear = items.SelectSingleNode(".//div[@class='auction-date-location']/div[1]").InnerText.Trim();
                //Match match_EndYear = (Regex.Match(EndYear, @"(-)\s?\d{1,2}\s?[a-zA-Z]*\s?(\d{4})"));
                Match match_EndYear = (Regex.Match(EndYear, @"(-)\s?(\d{1,2})\s([a-zA-Z]*)\s(\d{4})"));
                string matched_EndYear = match_EndYear.Groups[4].Value;



                var EndTime = items.SelectSingleNode(".//div[@class='auction-date-location']/div[1]").InnerText.Trim();
                //Match match_EndTime = Regex.Match(EndTime, @"((\d{2}:\d{2})\s?\(?CET\)?)$");
                Match match_EndTime = Regex.Match(EndTime, @"[,]\s?(\d{2}:\d{2})\sCET\s?(\d{1,2})\s([a-zA-Z]*)\s?[,]\s?(\d{2}:\d{2})");
                string matched_EndTime = match_EndTime.Groups[4].Value;


                var LocationNode = items.SelectSingleNode(".//div[@class='auction-date-location__item'][2]/span").InnerText.Trim();
                string location = LocationNode != null ? LocationNode : null;


                var IneichenProduct = new IneichenProduct() { Title = title, Description = description, ImageUrl = imageUrl, Link = linkUrl, LotCount = matched_lotCount, StartDate = matched_StartDate, StartMonth = matched_StartMonth, StartYear = matched_StartYear,StartTime=matched_StartTime,EndDate= matched_EndDate,EndMonth=matched_EndMonth,EndYear=matched_EndYear,EndTime=matched_EndTime, Location = location };
          
                IPobj.Add(IneichenProduct);
               Console.WriteLine($" {title} {description} {imageUrl} {linkUrl}  {match_lotCount} {matched_StartDate} {match_StartMonth} {matched_StartYear} {matched_StartTime} {matched_EndDate} {matched_EndMonth} {matched_EndYear} {matched_EndTime} {location}");
              



            }
            //InsertAuctionDataIntoDatabase(IPobj);
            InsertOrUpdateAuctionDataIntoDatabase(IPobj);

            Console.ReadLine();


          
            void InsertOrUpdateAuctionDataIntoDatabase(List<IneichenProduct> auctionDataList)
            {
                string conn = ConfigurationManager.ConnectionStrings["CreateConnection"].ConnectionString;

                using (SqlConnection connection = new SqlConnection(conn))
                {
                    connection.Open();

                    foreach (var data in auctionDataList)
                    {
                        string checkQuery = "SELECT COUNT(*) FROM Auctions WHERE Title = @Title";
                        using (SqlCommand checkCommand = new SqlCommand(checkQuery, connection))
                        {
                            checkCommand.Parameters.AddWithValue("@Title", data.Title);
                            int existingCount = (int)checkCommand.ExecuteScalar();

                            if (existingCount > 0)
                            {

                                ;
                                string updateQuery = "SP_UpdateIneichenProductData";
                                using (SqlCommand updateCommand = new SqlCommand(updateQuery, connection))
                                {
                                    updateCommand.CommandType = CommandType.StoredProcedure;
                                    updateCommand.Parameters.AddWithValue("@Title", data.Title);
                                    updateCommand.Parameters.AddWithValue("@Description", data.Description);
                                    updateCommand.Parameters.AddWithValue("@ImageUrl", data.ImageUrl);
                                    updateCommand.Parameters.AddWithValue("@Link", data.Link);
                                    updateCommand.Parameters.AddWithValue("@LotCount", data.LotCount);
                                    updateCommand.Parameters.AddWithValue("@StartDate", data.StartDate);
                                    updateCommand.Parameters.AddWithValue("@StartMonth", data.StartMonth);
                                    updateCommand.Parameters.AddWithValue("@StartYear", data.StartYear);
                                    updateCommand.Parameters.AddWithValue("@StartTime", data.StartTime);
                                    updateCommand.Parameters.AddWithValue("@EndDate", data.EndDate);
                                    updateCommand.Parameters.AddWithValue("@EndMonth", data.EndMonth);
                                    updateCommand.Parameters.AddWithValue("@EndYear", data.EndYear);
                                    updateCommand.Parameters.AddWithValue("@EndTime", data.EndTime);
                                    updateCommand.Parameters.AddWithValue("@Location", data.Location);

                                    updateCommand.ExecuteNonQuery();
                                }
                                Console.WriteLine("Data Updated Successfully");
                            }
                            else
                            {
                                string insertQuery = "SP_InsertIneichenProductData";

                                using (SqlCommand insertCommand = new SqlCommand(insertQuery, connection))
                                {
                                    insertCommand.CommandType = CommandType.StoredProcedure;

                                    insertCommand.Parameters.AddWithValue("@Title", data.Title);
                                    insertCommand.Parameters.AddWithValue("@Description", data.Description);
                                    insertCommand.Parameters.AddWithValue("@ImageUrl", data.ImageUrl);
                                    insertCommand.Parameters.AddWithValue("@Link", data.Link);
                                    insertCommand.Parameters.AddWithValue("@LotCount", data.LotCount);
                                    insertCommand.Parameters.AddWithValue("@StartDate", data.StartDate);
                                    insertCommand.Parameters.AddWithValue("@StartMonth", data.StartMonth);
                                    insertCommand.Parameters.AddWithValue("@StartYear", data.StartYear);
                                    insertCommand.Parameters.AddWithValue("@StartTime", data.StartTime);
                                    insertCommand.Parameters.AddWithValue("@EndDate", data.EndDate);
                                    insertCommand.Parameters.AddWithValue("@EndMonth", data.EndMonth);
                                    insertCommand.Parameters.AddWithValue("@EndYear", data.EndYear);
                                    insertCommand.Parameters.AddWithValue("@EndTime", data.EndTime);
                                    insertCommand.Parameters.AddWithValue("@Location", data.Location);

                                    insertCommand.ExecuteNonQuery();
                                }

                                Console.WriteLine("Data Inserted Successfully");
                            }
                        }
                    }
                }

            }




        }
    }
}



//void InsertAuctionDataIntoDatabase(List<IneichenProduct> auctionDataList)
//{

//    string conn = ConfigurationManager.ConnectionStrings["CreateConnection"].ConnectionString;

//    using (SqlConnection connection = new SqlConnection(conn))
//    {
//        connection.Open();

//        try
//        {
//            foreach (var data in auctionDataList)
//            {
//                // stored procedure
//                string insertQuery = "SP_InsertIneichenProductData";

//                using (SqlCommand command = new SqlCommand(insertQuery, connection))
//                {
//                    command.CommandType = CommandType.StoredProcedure;

//                    command.Parameters.AddWithValue("@Title", data.Title);
//                    command.Parameters.AddWithValue("@Description", data.Description);
//                    command.Parameters.AddWithValue("@ImageUrl", data.ImageUrl);
//                    command.Parameters.AddWithValue("@Link", data.Link);
//                    command.Parameters.AddWithValue("@LotCount", data.LotCount);
//                    command.Parameters.AddWithValue("@StartDate", data.StartDate);
//                    command.Parameters.AddWithValue("@StartMonth", data.StartMonth);
//                    command.Parameters.AddWithValue("@StartYear", data.StartYear);
//                    command.Parameters.AddWithValue("@StartTime", data.StartTime);
//                    command.Parameters.AddWithValue("@EndDate", data.EndDate);
//                    command.Parameters.AddWithValue("@EndMonth", data.EndMonth);
//                    command.Parameters.AddWithValue("@EndYear", data.EndYear);
//                    command.Parameters.AddWithValue("@EndTime", data.EndTime);
//                    command.Parameters.AddWithValue("@Location", data.Location);

//                    command.ExecuteNonQuery();
//                }
//            }
//        }
//        catch (Exception ex)
//        {
//            Console.WriteLine(ex.Message.ToString());
//        }
//    }
//}