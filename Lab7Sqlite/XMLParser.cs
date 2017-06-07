using System.Xml.Linq;

namespace Lab7Sqlite
{
    public class XMLParser
    {
        /* returned data looks like this:
         * <data>
            <pr t="2017-06-07 06:04" v="1.290" type="L"/>
            <pr t="2017-06-07 12:34" v="3.312" type="H"/>
            <pr t="2017-06-07 17:43" v="2.188" type="L"/>
            <pr t="2017-06-07 23:52" v="3.883" type="H"/>
            <pr t="2017-06-08 06:39" v="1.195" type="L"/>
            <pr t="2017-06-08 13:15" v="3.370" type="H"/>
            <pr t="2017-06-08 18:22" v="2.243" type="L"/>
           </data> */

        //I adapted the example code to fit the type of data we were getting back
        public static string ParseResponseXml(string responseXml)
        {
            var tideDoc = XDocument.Parse(responseXml);
            var dataElements = tideDoc.Element("data").Elements();

            string predictionsString = "";
            foreach (XElement e in dataElements)
            {
                predictionsString += e.Attribute("t").Value + ",";
                predictionsString += e.Attribute("v").Value + ",";
                predictionsString += e.Attribute("type").Value + "/";
            }
            return predictionsString;
        }
    }
}
