/*
MIT License

Copyright 2017 Jon Goldberger

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated 
documentation files (the "Software"), to deal in the Software without restriction, including without limitation 
the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and 
to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of 
the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO 
THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE 
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, 
TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
 */



using Android.App;
using Android.Widget;
using Android.OS;
using Android.Content;
using Android.Views;
using System.Threading.Tasks;
using Org.Apache.Http.Client.Params;
using System.Net.Http;
using Org.Apache.Http.Client;
using System;
using System.Threading;
using Java.Interop;

namespace AsyncAllTheWayAndroid
{
	[Activity(Label = "AsyncAllTheWayAndroid", MainLauncher = true, Icon = "@mipmap/icon")]
	public class MainActivity : Activity
	{
		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);

			// Set our view from the "main" layout resource
			SetContentView(Resource.Layout.Main);

			ListView listView = FindViewById<ListView>(Resource.Id.listView1);

			listView.Adapter = new AsyncArrayAdapter(this, Android.Resource.Layout.SimpleListItem1);


		}
	}

	public class AsyncArrayAdapter : ArrayAdapter
	{
		Activity context;
		HttpClient client;
		Random rand;

		public AsyncArrayAdapter(Activity context, int resource) : base(context, resource)
		{
			this.context = context;
			rand = new Random(DateTime.Now.Millisecond);
		}

		public override long GetItemId(int position)
		{
			return position;
		}

		public override int Count
		{
			get { return 50; }
		}

		public override View GetView(int position, View convertView, ViewGroup parent)
		{
			// re-use an existing view, if one is available
			View view = convertView; 
			// Otherwise create a new one
			if (view == null) {
				view = context.LayoutInflater.Inflate(Android.Resource.Layout.SimpleListItem1, null);
			}
			TextView textView = view.FindViewById<TextView>(Android.Resource.Id.Text1);

			textView.Text = GetTextAsync(position).Result;


			return view;
		}

		async Task<string> GetTextAsync(int position)
		{
			await Task.Delay(rand.Next(100, 500)).ConfigureAwait(false);
			if (client == null)
				client = new HttpClient();
			string response = await client.GetStringAsync("http://example.com").ConfigureAwait(false);
			string stringToDisplayInList = response.Substring(41, 14) + " " + position.ToString();
			return stringToDisplayInList;
		}

	}

	public class Wrapper<T> : Java.Lang.Object
	{
		public T Data;
	}
}

