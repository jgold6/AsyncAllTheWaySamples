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
			TableViewCellWithCTS cell = tableView.DequeueReusableCell(CellIdentifier) as TableViewCellWithCTS;
			if (cell == null) { 
				cell = new TableViewCellWithCTS(UITableViewCellStyle.Default, CellIdentifier); 
			}

			string text = GetTextAsync(indexPath.Row).Result;
			cell.TextLabel.Text = text;

			return cell;
		}

		async Task<string> GetTextAsync(int position)
		{
			await Task.Delay(rand.Next(100, 500)).ConfigureAwait(false); // to simulate a task that takes variable amount of time
			if (client == null)
				client = new HttpClient();
			string response = await client.GetStringAsync("http://example.com").ConfigureAwait(false);
			string stringToDisplayInList = response.Substring(41, 14) + " " + position.ToString();
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
