using System;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using SQLite;
using System.IO;
using System.Linq;
using DAL;
using System.Collections.Generic;
using System.Net;

namespace Lab7Sqlite
{
    [Activity(Label = "Lab7Sqlite", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);


            /* ------ copy and open the dB file using the SQLite-Net ORM ------ */

            string dbPath = "";
            SQLiteConnection db = null;

            // Get the path to the database that was deployed in Assets
            dbPath = Path.Combine(
                System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "tidesDB.db3");

            // I used this code to create the initial file, then commented it out because I was no longer using the file from my computer. This basically just ensures that the file exists on the Android device the first time you run it.
            
            //using (Stream inStream = Assets.Open("tidesDB.db3"))
            //using (Stream outStream = File.Create(dbPath))
                //inStream.CopyTo(outStream);

            // Open the database
            db = new SQLiteConnection(dbPath);

            //This code was useful for testing. I left it in because It may be needed later.
            //if (db.CreateTable<TideDataObject>() == 0)
            //{
            //    // A table already exixts, delete any data it contains
            //    db.DeleteAll<TideDataObject>();
            //}

            /* ------ Spinner initialization ------ */

            // I hardcoded the names of the stations. There is no DB data the first time you initialize this, so the values need to be hardcoded
            List<string> stationNames = new List<string> { "Florence", "Charleston", "Southbeach" };
            var adapter = new ArrayAdapter(this, Android.Resource.Layout.SimpleSpinnerItem, stationNames);

            var stationSpinner = FindViewById<Spinner>(Resource.Id.stationSpinner);
            stationSpinner.Adapter = adapter;

            // Event handler for selected spinner item
            string selectedStation = "";
            stationSpinner.ItemSelected += delegate (object sender, AdapterView.ItemSelectedEventArgs e) {
                Spinner spinner = (Spinner)sender;
                selectedStation = (string)spinner.GetItemAtPosition(e.Position);
            };

            /* ------- DatePicker initialization ------- */

            var tideDatePicker = FindViewById<DatePicker>(Resource.Id.tideDatePicker);
            //Hardcoded the beginning date. I imagined it was only necessary to go out 1 full year
            string firstDate = "1/1/2017";
            tideDatePicker.DateTime = Convert.ToDateTime(firstDate);

            /* ------- Query for selected tide date -------- */

            Button listViewButton = FindViewById<Button>(Resource.Id.listViewButton);
            
            listViewButton.Click += delegate
            {
                DateTime endDate = tideDatePicker.DateTime; //No widget for picking an end time at this point, so we are just accessing data for 1 day
                DateTime startDate = tideDatePicker.DateTime; 
                
                Helper.DB = db; //I created a static helper class to store my DB in order to pass it between activities
                var listView = new Intent(this, typeof(ListViewActivity));
                listView.PutExtra("StartDate", startDate.ToString());
                listView.PutExtra("EndDate", endDate.ToString());
                listView.PutExtra("Station", selectedStation);
                StartActivity(listView);
            };
        }

        
    }
}

