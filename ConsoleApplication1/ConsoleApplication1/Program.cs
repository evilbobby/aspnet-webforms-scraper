using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace ConsoleApplication1
{
    class Program
    {
        static void Main(string[] args)
        {
            ScrapeValue();

        }

        public static void ScrapeValue()
        {

            var url = "http://onlinemvd.dor.ga.gov/Tap/vinnumber.aspx";

            string currentState = GetCurrentState(url);


            string data = GetControlValues(currentState);
            byte[] buffer = Encoding.UTF8.GetBytes(data);
            string proxy = null;

            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
            req.Method = "POST";
            req.ContentType = "application/x-www-form-urlencoded";
            req.ContentLength = buffer.Length;
            req.Proxy = new WebProxy(proxy, true); // ignore for local addresses
            req.CookieContainer = new CookieContainer(); // enable cookies


            Stream reqst = req.GetRequestStream(); // add form data to request stream
            reqst.Write(buffer, 0, buffer.Length);
            reqst.Flush();
            reqst.Close();



            Console.WriteLine("\nPosting to " + url); 
            HttpWebResponse res = (HttpWebResponse)req.GetResponse(); // send request, get response

            Console.WriteLine("\nResponse stream is: \n");
            Stream resst = res.GetResponseStream(); // display HTTP response
            StreamReader sr = new StreamReader(resst);
            Console.WriteLine(sr.ReadToEnd());

        }


        public static string GetControlValues(string currentState)
        {

            return string.Format("__LASTFOCUS=&__EVENTTARGET=&__EVENTARGUMENT=&__PREVIOUSPAGE=cnIHjB0i4w1lh2DKzBPxFjG0IR3UbUTE1HRFd2aOUxylFqzZtEvszpQFGhmj4B7UyM6qrRl6MURd273wQYJKs8eHaQA1&ctl00%24txtSearch=Search&ctl00%24ContentPlaceHolderBody%24vin=WBAWB7C5XAP049376&ctl00%24ContentPlaceHolderBody%24Enter=Enter&ctl00%24ContentPlaceHolderBody%24radOwner=N&ctl00%24ctl22=", currentState);

        }

        public static string GetCurrentState(string url)
        {

            var currentState = string.Empty;

            WebClient wc = new WebClient();

            Stream st = wc.OpenRead(url);
            StreamReader sr = new StreamReader(st);

            var line = sr.ReadToEnd();

            if (line.IndexOf("__VIEWSTATE") != -1) // found line
            {
                sr.Close();
                st.Close();
                int startIndex = line.IndexOf("value=",line.IndexOf("__VIEWSTATE")) + 7;
                int endIndex = line.IndexOf("\"", startIndex);
                int count = endIndex - startIndex;
                currentState =string.Format("&__VIEWSTATE={0}", HttpUtility.UrlEncode(line.Substring(startIndex, count)));
            }

                 if (line.IndexOf("__EVENTVALIDATION") != -1) // found line
            {
               
                int startIndex = line.IndexOf("value=",line.IndexOf("__EVENTVALIDATION") ) + 7;
                int endIndex = line.IndexOf("\"", startIndex);
                int count = endIndex - startIndex;
                currentState += string.Format("&__EVENTVALIDATION={0}", HttpUtility.UrlEncode(line.Substring(startIndex, count)));
            }
             

            return currentState;
        }

    }



}
