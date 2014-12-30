using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;
using System.Xml.Linq;
using System.Web;

namespace FantasyBasketballStuff
{
    class InitialDataScraper
    {
        public static void initialScrape()
        {
            string baseLink = "http://www.basketball-reference.com/players/";
            char pageID = 'a';

            string rawPageHTML = null;

            //Search webpage for product code or any text
            List<Player> playerList = new List<Player>();

            while (pageID <= 'z')
            {
                HttpWebRequest myReq = (HttpWebRequest)WebRequest.Create(baseLink + pageID);
                HttpWebResponse myres = (HttpWebResponse)myReq.GetResponse();

                using (StreamReader sr = new StreamReader(myres.GetResponseStream()))
                {
                    rawPageHTML = sr.ReadToEnd();
                }
                if (rawPageHTML.Contains("<tbody"))
                {
                    rawPageHTML = rawPageHTML.Substring(rawPageHTML.IndexOf("<tbody"), (rawPageHTML.IndexOf("</tbody>") + 8 - rawPageHTML.IndexOf("<tbody")));

                    //Manual correction for Dennis Schroeder
                    rawPageHTML = rawPageHTML.Replace("&ouml;", "oe");

                    XDocument doc = XDocument.Parse(rawPageHTML);


                    playerList.AddRange((from tr in doc.Element("tbody").Elements("tr")
                                         where tr.Element("td").Element("strong") != null
                                         select new Player
                                         {
                                             playerUrl = tr.Element("td").Element("strong").Element("a").Attribute("href").Value.Substring(9),
                                             firstName = tr.Element("td").Element("strong").Element("a").Value.Split(' ')[0],
                                             lastName = tr.Element("td").Element("strong").Element("a").Value.Split(' ')[1],
                                             debutYear = Int32.Parse(tr.Elements("td").ElementAt(1).Value),
                                             position = tr.Elements("td").ElementAt(3).Value,
                                             height = tr.Elements("td").ElementAt(4).Value,
                                             weight = Int32.Parse(tr.Elements("td").ElementAt(5).Value),
                                             DOB = DateTime.Parse(tr.Elements("td").ElementAt(6).Element("a").Value)
                                         }).ToList());
                }
                pageID++;
            }
        }
        public class Player
        {
            public string playerUrl;
            public string firstName;
            public string lastName;
            public int debutYear;
            public string position;
            public string height;
            public int weight;
            public DateTime DOB;
        }
    }
}
    


