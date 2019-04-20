using System;
using io = System.IO;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Net;
using System.ComponentModel;
using System.Windows;

namespace GoalReacher
{
	class ProgressDisplay : DependencyObject
	{
		public ProgressDisplay(string title, string background, double width, double height, int numberOfItems, decimal goalAmount, decimal actualAmount)
		{
			Title = title;
			LoadBackground(background);
			NumberOfItems = numberOfItems;
			GoalAmount = goalAmount;
			ActualAmount = actualAmount;
			Canvas = new Canvas();

			UpdateDisplay(width, height);
		}

		public string Background
		{
			get { return (string)GetValue(BackgroundProperty); }
			set { SetValue(BackgroundProperty, value); }
		}

		// Using a DependencyProperty as the backing store for Background.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty BackgroundProperty =
			 DependencyProperty.Register("Background", typeof(string), typeof(ProgressDisplay), new PropertyMetadata(""));



		public string Status
		{
			get { return (string)GetValue(StatusProperty); }
			set { SetValue(StatusProperty, value); }
		}

		// Using a DependencyProperty as the backing store for Status.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty StatusProperty =
			 DependencyProperty.Register("Status", typeof(string), typeof(ProgressDisplay), new PropertyMetadata(""));

		internal void UpdateDisplay(double width, double height)
		{
			Canvas.Children.Clear();

			width -= c_horizontalOffset * 2;
			height -= c_topOffset + c_bottomOffset;
			var border = new Rectangle { Width = width, Height = height, Stroke = Brushes.White, StrokeThickness = 1 };
			Canvas.Children.Add(border);
			Canvas.SetLeft(border, c_horizontalOffset);
			Canvas.SetTop(border, c_topOffset);

			CreateProgressItems(NumberOfItems, GoalAmount, ActualAmount, width, height);
			Canvas.ToolTip = $"You're {(ActualAmount / GoalAmount) * 100:f2}% of the way to your goal!";
		}

		private void CreateProgressItems(int numberOfItems, decimal goalAmount, decimal actualAmount, double width, double height)
		{
			int itemsPerRow = (int)Math.Ceiling(Math.Sqrt(numberOfItems));
			int itemsPerColumn = (int)Math.Ceiling((double)numberOfItems / itemsPerRow);
			double itemWidth = width / itemsPerRow;
			double itemHeight = height / itemsPerColumn;

			double currentX = c_horizontalOffset;
			double currentY = c_topOffset;
			double currentOpacity = c_completedItemOpacity;

			double progress = (double)actualAmount / (double)goalAmount;
			double progressIndexToChangeDisplay = numberOfItems * progress;

			for (int i = 0; i < NumberOfItems; i++)
			{
				if (i == Math.Floor(progressIndexToChangeDisplay))
					currentOpacity = (progressIndexToChangeDisplay - Math.Floor(progressIndexToChangeDisplay)) * c_completedItemOpacity;
				else if (i > progressIndexToChangeDisplay)
					currentOpacity = c_defaultOpacity;

				var progressBox = new Rectangle { Width = itemWidth, Height = itemHeight, Fill = Brushes.DarkGray, Opacity = currentOpacity, Stroke = Brushes.White, StrokeThickness = c_progressStrokeThickness };
				Canvas.Children.Add(progressBox);
				Canvas.SetLeft(progressBox, currentX);
				Canvas.SetTop(progressBox, currentY);

				currentX += itemWidth;
				if (currentX - c_horizontalOffset + itemWidth - 2 >= width)
				{
					currentX = c_horizontalOffset;
					currentY += itemHeight;
				}
			}
		}

		private void LoadBackground(string background)
		{
			if (background.ToLower().StartsWith("http"))
			{
				Background = $@"Resources\Loading.jpg";
				UpdateBackgroundImage(background);
			}
			else
			{
				Background = $@"Resources\{background}";
			}
		}

		private void UpdateBackgroundImage(string background)
		{
			// Check for cached image
			m_sourceBackground = background;
			string cachedFileName = background.GetHashCode() + (!string.IsNullOrEmpty(io.Path.GetExtension(background)) ? io.Path.GetExtension(background) : ".jpg"); // assume jpeg if no extension
			string cacheLocation = io.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), c_developerFolder, c_appFolder, "images");
			m_cachedFileLocation = io.Path.Combine(cacheLocation, cachedFileName);

			if (io.File.Exists(m_cachedFileLocation))
			{
				Background = m_cachedFileLocation;
				return;
			}

			// Download image if needed
			io.Directory.CreateDirectory(cacheLocation);
			m_client = new WebClient();
			m_client.DownloadFileCompleted += OnDownloadComplete;
			m_client.DownloadFileAsync(new Uri(background), m_cachedFileLocation);
		}

		private void OnDownloadComplete(object sender, AsyncCompletedEventArgs e)
		{
			if (e.Error == null && io.File.Exists(m_cachedFileLocation))
				Background = m_cachedFileLocation;
			else
				Status = $"Failed to load image: {e.Error?.Message ?? "Unknown Error"}";

			m_client.DownloadFileCompleted -= OnDownloadComplete;
		}

		public Canvas Canvas { get; private set; }
		public string Title { get; }
		public int NumberOfItems { get; }
		public decimal GoalAmount { get; set; }
		public decimal ActualAmount { get; set; }

		WebClient m_client;
		string m_cachedFileLocation;
		string m_sourceBackground;

		const double c_defaultOpacity = .85;
		const double c_completedItemOpacity = 0.0;
		const double c_progressStrokeThickness = .25;
		const double c_horizontalOffset = 8;
		const double c_topOffset = 8;
		const double c_bottomOffset = 40;
		const string c_developerFolder = "Ryan.Gano";
		const string c_appFolder = "GoalReacher";
	}
}
