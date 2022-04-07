using ConsoleApp1;
using System.Net;

namespace NooNapp
{
    static class Program
    {
        static void Main()
        {
            Console.WriteLine("Hello, World!");

            bool result = true;

            try
            {
                for (int i = 1; i < 20; i++)
                {
                    var ddd = GetIhernItemDetails(i);
                }
            }
            catch
            {
                result = false;
            }

            if (result)
                Console.WriteLine("Success!");
            else Console.WriteLine("Fail!");

            Console.ReadLine();
        }




        public static string getIherbItem(long Id)
        {
            string url = "https://iherb.com/pr/s/" + Id;
            var pageContent = GetIherbContent(url);
            return pageContent;
        }
        public static string GetIherbContent(string url, string website = "ae", int count = 0)
        {
            // using System.Net;
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            try
            {
                var client = new WebClient();

                if (website == "us")
                {
                    client.Headers.Add("Cookie", "ih-preference=store=0&country=US&language=en-US&currency=USD;iher-pref1=storeid=0&sccode=US&lan=en-US&scurcode=USD&wp=2&lchg=1&ifv=1&bi=0");
                }
                else
                    client.Headers.Add("Cookie", "ih-preference=store=0&country=AE&language=en-US&currency=AED;iher-pref1=storeid=0&sccode=AE&lan=en-US&scurcode=AED&wp=2&lchg=1&ifv=1&bi=0");
                //client.Headers.Add("Cookie", "ih-preference=store=0&country=US&language=en-US&currency=USD;iher-pref1=storeid=0&sccode=US&lan=en-US&scurcode=USD&wp=2&lchg=1&ifv=1&bi=0");

                //    client.Headers.Add("Cookie", cookiefull);
                //}

                var content = client.DownloadString(url);
                var ss = client.ResponseHeaders["set-cookie"];
                return content;
            }
            catch (System.Net.WebException e)
            {

                //Console.WriteLine("TRY " + count + " Error Open URL (" + url + ")");
                //if (count < 3)
                //{
                //    Thread.Sleep(1000 * 31);
                //    count++;
                //    return GetContent(url, count);
                //}

                throw e;


            }
            return "";

        }
        public static IherbItem GetIhernItemDetails(long Id)
        {
            try
            {
                Console.Write(".");

                var html = getIherbItem(Id);
                int indexer = 0;
                var title = Setting.GetValue(html, indexer, "og:title", "content=\"", "\"", ref indexer);

                var brand = Setting.GetValue(html, indexer, "og:brand", "content=\"", "\"", ref indexer);
                var availability = Setting.GetValue(html, indexer, "og:availability", "content=", "/", ref indexer, " ");
                var price = Setting.GetValue(html, indexer, "og:standard_price", "content=\"", "\"", ref indexer, " ");

                if (title.Contains("Discontinued Item"))
                    availability = "Discontinued";
                //var img3 = Downloader.GetValue(html, indexer, "og:images", "content=\"", "\"", ref indexer);
                var prmryPrntCtgry = Setting.GetValue(html, indexer, "prmryPrntCtgry", "\"", "\"", ref indexer);
                var prtNum = Setting.GetValue(html, indexer, "prtNum", "\"", "\"", ref indexer);
                var upcCd = Setting.GetValue(html, indexer, "upcCd: ", "", ",", ref indexer);
                var weight = Setting.GetValue(html, indexer, "data-shipping-weight-kg", "\"", "\"", ref indexer, " kg");

                var img1 = Setting.GetValue(html, indexer, "class=\"lazy img-responsive\" data-lazyload=\"", "", "\"", ref indexer);
                var img2 = Setting.GetValue(html, indexer, "class=\"lazy img-responsive\" data-lazyload=\"", "", "\"", ref indexer);
                var img3 = Setting.GetValue(html, indexer, "class=\"lazy img-responsive\" data-lazyload=\"", "", "\"", ref indexer);
                title = title.Replace(brand + ", ", "");
                int indextLastsemi = title.LastIndexOf(',');
                if (indextLastsemi != -1)
                {
                    title = title.Remove(indextLastsemi, 1);
                    title = title.Insert(indextLastsemi, " -");
                }



                var currency = Setting.GetValue(html, 0, "itemprop=\"priceCurrency", "content=\"", "\"", ref indexer, "");
                if (img1 == "N/A")
                {
                    img1 = Setting.GetValue(html, indexer, "iherb-product-image", "src=\"", "\"", ref indexer);
                    //https://s3.images-iherb.com/sns/sns00155/v/4.jpg
                }



                IherbItem iherbItem = new IherbItem()
                {
                    availability = availability,
                    brand = brand,
                    Id = Id,
                    Img1 = img1,
                    Img2 = img2,
                    Img3 = img3,
                    price = price,
                    prmryPrntCtgry = prmryPrntCtgry,
                    prtNum = prtNum,
                    upcCd = upcCd,
                    barcode = upcCd,
                    weight = weight,
                    title = title,
                    currency = currency
                };
                //InsertIherbItem(iherbItem);
                Console.WriteLine(">> Found Item " + Id);
                return iherbItem;
            }
            catch (System.Net.WebException ex)
            {
                var status = ((HttpWebResponse)ex.Response).StatusCode;

                if (status == HttpStatusCode.Forbidden)
                {
                    throw ex;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("<< Error item " + Id);
            }
            finally
            {

            }
            return null;
        }
    }

    public class IherbItem
    {
        public long Id { get; set; }
        public string title { get; set; }
        public string price { get; set; }
        public string brand { get; set; }
        public string availability { get; set; }
        public ItemAvailabilty MainAvailability
        {
            get
            {
                if (availability == "instock")
                {
                    return ItemAvailabilty.Available;
                }
                else if (availability == "out of stock")
                {
                    return ItemAvailabilty.OutOfStock;
                }
                else if (availability == "Discontinued")
                {
                    return ItemAvailabilty.OutOfStock;
                }
                else return ItemAvailabilty.OutOfStock;
            }
        }
        public string Img1 { get; set; }
        public string Img2 { get; set; }
        public string Img3 { get; set; }
        public string prmryPrntCtgry { get; set; }
        public string prtNum { get; set; }
        public string upcCd { get; set; }
        public string barcode { get; set; }
        public string weight { get; set; }
        public string currency { get; set; }

        public enum ItemAvailabilty : int
        {
            OutOfStock = 0,
            Available = 1,
            UnKnown = 2
        }
    }


}
