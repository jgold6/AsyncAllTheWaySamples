using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using UIKit;

namespace AsyncAllTheWayiOS
{
	public partial class ViewController : UITableViewController
	{
		public ViewController() : base()
		{
			rand = new Random(DateTime.Now.Millisecond);
		}

		protected ViewController(IntPtr handle) : base(handle)
		{
			// Note: this .ctor should not contain any initialization logic.
		}

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();
			// Perform any additional setup after loading the view, typically from a nib.
		}

		public override void DidReceiveMemoryWarning()
		{
			base.DidReceiveMemoryWarning();
			// Release any cached data, images, etc that aren't in use.
		}

		HttpClient client;
		Random rand;
		string CellIdentifier = "TableCell";
		public override nint NumberOfSections(UITableView tableView)
		{
			return 1;
		}

		public override nint RowsInSection(UITableView tableView, nint section)
		{
			return 50;
		}

		public override UITableViewCell GetCell(UITableView tableView, Foundation.NSIndexPath indexPath)
		{
			// We will need a cancellationTokenSource so we can cancel the async call if the view moves back on screen 
			// while text is already being loaded. Without this, if a view is loading some text, but the view moves off
			// and back on screen, the new load may take less time than the old load and then the old load will 
			// overwrite the new text load and the wrong data will be displayed. So we will c ancel any async task on a 
			// recycled view before loading the new text. 
			CancellationTokenSource cts;

			TableViewCellWithCTS cell = tableView.DequeueReusableCell(CellIdentifier) as TableViewCellWithCTS;
			if (cell == null) { 
				cell = new TableViewCellWithCTS(UITableViewCellStyle.Default, CellIdentifier); 
			}
			else {
				// If view exists, cancel any pending async text loading for this view
				// by calling cts.Cancel();
				cts = cell.Cts;

				// If cancellation has not already been requested, cancel the async task
				if (!cts.IsCancellationRequested)
				{
					cts.Cancel();
				}
			}

			cell.TextLabel.Text = "Placeholder";

			// Create new CancellationTokenSource for this view's async call
			cts = new CancellationTokenSource();

			// Add to the Tag property of the view wrapped in a Java.Lang.Object
			cell.Cts = cts;

			// Get the cancellation token to pass into the async method
			var ct = cts.Token;

			Task.Run(async () =>
			{
				try
				{
					string text = await GetTextAsync(indexPath.Row, ct);
					InvokeOnMainThread(()=> {
						cell.TextLabel.Text = text;
					});

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

			return cell;
		}

		async Task<string> GetTextAsync(int position, CancellationToken ct)
		{
			// Check to see if task was cancelled, if so throw cancelled exception.
			// Good to check at several points, including just prior to returning the string. 
			ct.ThrowIfCancellationRequested();
			await Task.Delay(rand.Next(100, 750)); // to simulate a task that takes variable amount of time
			ct.ThrowIfCancellationRequested();
			if (client == null)
				client = new HttpClient();
			string response = await client.GetStringAsync("http://example.com");
			string stringToDisplayInList = response.Substring(41, 14) + " " + position.ToString();
			ct.ThrowIfCancellationRequested();
			return stringToDisplayInList;
		}
	}

	public class TableViewCellWithCTS : UITableViewCell
	{
		public CancellationTokenSource Cts { get; set; }

		public TableViewCellWithCTS(UITableViewCellStyle style, string cellIdentifier) : base(style, cellIdentifier)
		{
			
		}
	}
}
