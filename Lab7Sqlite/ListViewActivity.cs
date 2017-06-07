using System;
using System.Linq;

using Android.App;
using Android.Content;
using Android.OS;
using DAL;
using System.Net;
using System.IO;
using System.Globalization;
using SQLite;

namespace Lab7Sqlite
{
    [Activity(Label = "ListViewActivity")]
    public class ListViewActivity : ListActivity
    {
        string station = "";
        SQLiteConnection db;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            string selectedStation = (string)Intent.Extras.Get("Station");
            station = selectedStation;
            string endDateStr = (string)Intent.Extras.Get("EndDate");
            string startDateStr = (string)Intent.Extras.Get("StartDate");
            DateTime endDate = Convert.ToDateTime(endDateStr).AddDays(1);
            DateTime startDate = Convert.ToDateTime(startDateStr);
            
            db = Helper.DB;

            var tides = (from t in db.Table<TideDataObject>() //Check if data is in the database. If its not, this will be a count of 0
                         where (t.Station == selectedStation)
                             && (t.DateActual <= endDate)
                             && (t.DateActual >= startDate)
                         select t).ToList();

            if (tides.Count == 0) //If its 0, then we are initializing code to add it to the database
            {
                var id = "";
                if (selectedStation == "Florence") //Associate the station IDs with the station names based on what the user selected
                    id = "9434098";
                else if (selectedStation == "Charleston")
                    id = "9432780";
                else if (selectedStation == "Southbeach")
                    id = "9435380";

                string formattedSD = DateFactory(startDate); //Required to put the date from the datepicker into a format the API will accept

                // Makes a call to the API and puts it into a massive comma separated string, then I parse that into a format I can create TideDataObjects with
                ParseToTideDataObject(GetAPIData(id, formattedSD));

                tides = (from t in db.Table<TideDataObject>() //Check if data is in the database. This shouldn't be 0 anymore
                        where (t.Station == selectedStation)
                            && (t.DateActual <= endDate)
                            && (t.DateActual >= startDate)
                        select t).ToList();
            }

            ListAdapter = new TideDataAdapter(this, tides);

        }

        public static string GetAPIData(string stationid, string beginDT)
        {
            string baseURI = "https://tidesandcurrents.noaa.gov/api/datagetter?";
            string requiredParams = "product=predictions&datum=stnd&units=metric&time_zone=lst&application=web_services&interval=hilo&format=xml";
            //As per instructions for this lab, we grab not only the data for the selected date, but also for the rest of the year, hence the hardcoded end_date parameter
            string requestURI = baseURI + "begin_date=" + beginDT + "&" + "end_date=20171231" + "&" + "station=" + stationid + "&" + requiredParams;

            // Send a request to the service and get a response
            var request = (HttpWebRequest)WebRequest.Create(requestURI);
            var response = (HttpWebResponse)request.GetResponse();

            // Read and parse the response
            var reader = new StreamReader(response.GetResponseStream());
            string content = reader.ReadToEnd();

            string tideDataParsed = XMLParser.ParseResponseXml(content);
            tideDataParsed = tideDataParsed.Substring(0, tideDataParsed.Count() -1);

            return tideDataParsed;
        }

        //This method accepts a DateTime object and parses it into a string the NOAA website will accept as a parameter
        public static string DateFactory(DateTime date)
        {
            string[] step1 = date.ToString().Split(' '); //Divide the string into 3 pieces, the date, the time, and the letters AM or PM
            string step2 = step1[0]; // Step 2 now equals the date
            string[] step3 = step2.Split('/'); //Divide the date into 3 separate parts, the day, the month, and the year
            if(step3[0].Count() < 2) //Check to see if month is single or double digit. If its single, make it double (i.e. 1 vs 01)
            {
                step3[0] = "0" + step3[0];
            }
            if (step3[1].Count() < 2)
            {
                step3[1] = "0" + step3[1];
            }
            string step4 = step3[2] + step3[0] + step3[1]; //Step4 is now the rearrangement of the date string with year first, then month, then day
            //string[] step5 = step1[1].Split(':'); //Split the time string into 3 pieces to remove the seconds portion
            string step6 = step4; //Recombine all pieces leaving out the AM/PM string to get a string the resembles "20170201"
            // + " " + step5[0] + ":" + step5[1]

            return step6;
        }

        public void ParseToTideDataObject(string stringtoparse)
        {
            string[] tideObjects = stringtoparse.Split('/'); // "2017-01-01 02:35,8.341,H"

            foreach(var s in tideObjects)
            {
                string[] data = s.Split(','); // Separate out each value
                string[] dateTime = data[0].Split(' '); //separate the date and the time
                string[] reformatDate = dateTime[0].Split('-'); //separate the pieces of the date into their own parts
                string reformattedDate = reformatDate[1] + "/" + reformatDate[2] + "/" + reformatDate[0] + " " + dateTime[1]; //Rearrange the pieces of the date how I want them
                
                string prediction = ((int)(decimal.Parse(data[1]) * 100)).ToString() + "cm"; //The value I got back from the API was in meters, 
                                                                                             //so I'm converting it to CM as per instructions for the app in previous assignments

                DateTime date = DateTime.ParseExact(reformattedDate, "MM/dd/yyyy HH:mm", CultureInfo.InvariantCulture); //specifies exactly how I want the date formatted with what I have
                string dayOfWeek = date.ToString("ddd"); //Get the day of the week
                var tide = new TideDataObject
                {
                    DateActual = date,
                    DateDisplay = reformatDate[1] + "/" + reformatDate[2] + "/" + reformatDate[0],
                    Level = data[2],
                    Day = dayOfWeek,
                    PredCm = prediction,
                    Station = station,
                    Time = dateTime[1]
                };

                var tides = (from t in Helper.DB.Table<TideDataObject>() //Check if data is in the database. If its not, this will be a count of 0
                             where (t.Station == tide.Station)
                                 && (t.DateActual == tide.DateActual)
                                 && (t.Time == tide.Time)
                                 && (t.PredCm == tide.PredCm)
                                 && (t.Level == tide.Level)
                                 && (t.Day == tide.Day)
                                 && (t.DateDisplay == tide.DateDisplay)
                             select t).ToList();
                if (tides.Count() == 0)
                    db.Insert(tide);
            }

        }
    }
}