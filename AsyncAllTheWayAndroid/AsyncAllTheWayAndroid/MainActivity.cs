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
			// We will need a cancellationTokenSource so we can cancel the async call if the view moves back on screen 
			// while text is already being loaded. Without this, if a view is loading some text, but the view moves off
			// and back on screen, the new load may take less time than the old load and then the old load will 
			// overwrite the new text load and the wrong data will be displayed. So we will c ancel any async task on a 
			// recycled view before loading the new text. 
			CancellationTokenSource cts; 

			View view = convertView; // re-use an existing view, if one is available
			// Otherwise create a new one
			if (view == null) {
				view = context.LayoutInflater.Inflate(Android.Resource.Layout.SimpleListItem1, null);
			}
			else
			{
				// If view exists, cancel any pending async text loading for this view
				// by calling cts.Cancel();
				var wrapper = view.Tag.JavaCast<Wrapper<CancellationTokenSource>>();
				cts = wrapper.Data;

				// If cancellation has not already been requested, cancel the async task
				if (!cts.IsCancellationRequested)
				{
					cts.Cancel();
				}
			}

			TextView textView = view.FindViewById<TextView>(Android.Resource.Id.Text1);
			textView.Text = "placeholder";

			// Create new CancellationTokenSource for this view's async call
			cts = new CancellationTokenSource();

			// Add to the Tag property of the view wrapped in a Java.Lang.Object
			view.Tag = new Wrapper<CancellationTokenSource> { Data = cts };

			// Get the cancellation token to pass into the async method
			var ct = cts.Token;

			Task.Run(async () => {
				try
				{
					textView.Text = await GetTextAsync(position, ct);
				}
				catch (System.OperationCanceledException ex)
				{
					Console.WriteLine($"Text load cancelled: {ex.Message}");
				}
				catch (Exception ex)
				{
					Console.WriteLine(ex.Message);
				}
			}, ct);

			return view;
		}

		async Task<string> GetTextAsync(int position, CancellationToken ct)
		{
			// Check to see if task was cancelled, if so throw cancelled exception.
			// Good to check at several points, including just prior to returning the string. 
			ct.ThrowIfCancellationRequested();
			await Task.Delay(rand.Next(100,750)); // to simulate a task that takes variable amount of time
			ct.ThrowIfCancellationRequested();
			if (client == null)
				client = new HttpClient();
			string response = await client.GetStringAsync("http://example.com");
			string stringToDisplayInList = response.Substring(41, 14) + " " + position.ToString();
			ct.ThrowIfCancellationRequested();
			return stringToDisplayInList;
		}

	}

	public class Wrapper<T> : Java.Lang.Object
	{
		public T Data;
	}
}

