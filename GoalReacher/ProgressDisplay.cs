using System;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace GoalReacher
{
	class ProgressDisplay
	{
		public ProgressDisplay(string title, string background, double width, double height, int numberOfItems, decimal goalAmount, decimal actualAmount)
		{
			Title = title;
			Background = background;
			NumberOfItems = numberOfItems;
			GoalAmount = goalAmount;
			ActualAmount = actualAmount;
			Canvas = new Canvas();

			UpdateDisplay(width, height);
		}

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

		public Canvas Canvas { get; private set; }
		public string Title { get; }
		public string Background { get; }
		public int NumberOfItems { get; }
		public decimal GoalAmount { get; set; }
		public decimal ActualAmount { get; set; }

		const double c_defaultOpacity = .85;
		const double c_completedItemOpacity = 0.0;
		const double c_progressStrokeThickness = .25;
		const double c_horizontalOffset = 8;
		const double c_topOffset = 8;
		const double c_bottomOffset = 40;
	}
}
