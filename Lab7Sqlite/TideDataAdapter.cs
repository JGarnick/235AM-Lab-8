using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using DAL;

namespace Lab7Sqlite
{
    public class TideDataAdapter : BaseAdapter<TideDataObject>
    {
        List<TideDataObject> tdObjects;
        Activity context;       // The activity we are running in

        public TideDataAdapter(Activity c, List<TideDataObject> tdo) : base()
        {
            tdObjects = tdo;
            context = c;
        }

        public override long GetItemId(int position)
        {
            return position;
        }


        public override int Count
        {
            get { return tdObjects.Count; }
        }

        public override TideDataObject this[int position]
        {
            get { return tdObjects[position]; }
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            View view = convertView;
            if (view == null)
            {
                view = context.LayoutInflater.Inflate(Android.Resource.Layout.SimpleListItem1, null);
            }

            view.FindViewById<TextView>(Android.Resource.Id.Text1).Text = 
                "Date: " + tdObjects[position].DateDisplay + ",\t\t\t\t" + 
                "Day: " + tdObjects[position].Day + ",\t\t\t\t" + 
                "Time: " + tdObjects[position].Time + ",\t\t\t\t" + 
                "Level: " + tdObjects[position].Level;

            return view;
        }
    }
}