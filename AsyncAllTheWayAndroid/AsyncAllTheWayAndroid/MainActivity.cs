using Android.App;
using Android.Widget;
using Android.OS;
using Android.Content;
using Android.Views;
using System.Threading.Tasks;
using Org.Apache.Http.Client.Params;
using System.Net.Http;
using Org.Apache.Http.Client;

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

		public AsyncArrayAdapter(Activity context, int resource) : base(context, resource)
		{
			this.context = context;
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
			View view = convertView; // re-use an existing view, if one is available
			if (view == null) // otherwise create a new one
				view = context.LayoutInflater.Inflate(Android.Resource.Layout.SimpleListItem1, null);

			TextView textView = view.FindViewById<TextView>(Android.Resource.Id.Text1);
			textView.Text = "placeholder";

			Task.Run(async () => {
				textView.Text = await GetTextAsync(position);
			});

			return view;
		}

		async Task<string> GetTextAsync(int position)
		{
			await Task.Delay(300); // to simulate a task that takes longer
			if (client == null)
				client = new HttpClient();
			string response = await client.GetStringAsync("http://example.com");
			string stringToDisplayInList = response.Substring(41, 14) + " " + position.ToString();
			return stringToDisplayInList;
		}

	}
}

