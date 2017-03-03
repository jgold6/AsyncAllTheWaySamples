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
using Xamarin.Forms;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Net.Http;
using System.Collections.Generic;
using UIKit;

namespace AsyncAllTheWayXamForms
{
	public class App : Application
	{
		
		ObservableCollection<string> items = new ObservableCollection<string>();
		HttpClient client;
		Random rand;
		List<int> indexes = new List<int>();

		public App()
		{
			rand = new Random(DateTime.Now.Millisecond);

			// Populate items List with Placeholder text and populate indexes with numbers 1 - 50
			for (int i = 0; i < 50; i++)
			{
				items.Add("Placeholder");
				indexes.Add(i);
			}

			// shuffle the indexes (just so list loads rows in a random order later)
			for (int i = 0; i < 49; i++)
			{
				int swapindex = rand.Next(0, 49);
				int hold = indexes[i];
				indexes[i] = indexes[swapindex];
				indexes[swapindex] = hold;
			}

			// The root page of your application
			var content = new ContentPage
			{
				Title = "AsyncAllTheWayXamForms",
				Content = new ListView
				{
					VerticalOptions = LayoutOptions.FillAndExpand,
					HorizontalOptions = LayoutOptions.FillAndExpand,
					ItemsSource = items
				}
			};

			MainPage = new NavigationPage(content);

			Task.Run(async () => {
				if (client == null)
					client = new HttpClient();
				for (int i = 0; i < 50; i++)
				{
					string text = await GetItemAsync(i);
					items.RemoveAt(indexes[i]);
					items.Insert(indexes[i], text);
				}
			});
		}

		public async Task<string> GetItemAsync(int i)
		{
			string response = await client.GetStringAsync("http://example.com");
			string stringToDisplayInList = response.Substring(41, 14) + " " + indexes[i].ToString();
			return stringToDisplayInList;
		}

		protected override void OnStart()
		{
			// Handle when your app starts
		}

		protected override void OnSleep()
		{
			// Handle when your app sleeps
		}

		protected override void OnResume()
		{
			// Handle when your app resumes
		}
	}
}
