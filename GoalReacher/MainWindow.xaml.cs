using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Xml.Linq;

namespace GoalReacher
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();
			ProgressSelector.DataContext = this;
			UpdateUIWithProgressData();
			this.SizeChanged += OnSizeChanged;
		}

		internal ProgressDisplay CurrentDisplay { get; set; }
		IReadOnlyList<ProgressDisplay> ProgressDisplays { get; set; }

		private void UpdateUIWithProgressData()
		{
			ProgressDisplays = LoadXmlData(@"Resources\Data.xml").ToList();
			ProgressSelector.ItemsSource = ProgressDisplays;
			ProgressSelector.SelectedItem = ProgressDisplays.First();
		}

		private IEnumerable<ProgressDisplay> LoadXmlData(string dataPath)
		{
			var data = XDocument.Load(dataPath);

			foreach (var goal in data.Root.Elements("goal"))
			{
				//	< goal title = "2nd Mortgage" photo = "House.jpg" goalAmount = "48502.99" currentAmount = "26474.22" steps = "350" />
				yield return new ProgressDisplay(
					goal.Attribute("title").Value,
					$@"Resources\{goal.Attribute("photo").Value}",
					(int)Width - c_widthOffset,
					(int)Height - c_heightOffset,
					int.Parse(goal.Attribute("steps").Value),
					decimal.Parse(goal.Attribute("goalAmount").Value),
					decimal.Parse(goal.Attribute("currentAmount").Value));
			}
		}

		private void OnSizeChanged(object sender, SizeChangedEventArgs e)
		{
			CurrentDisplay.UpdateDisplay(ProgressDisplay.ActualWidth, ProgressDisplay.ActualHeight);
		}

		private void UpdateProgress_Click(object sender, RoutedEventArgs e)
		{
			CurrentDisplay.UpdateDisplay(ProgressDisplay.ActualWidth, ProgressDisplay.ActualHeight);
		}

		private void TextBox_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
		{
			if (e.Key == System.Windows.Input.Key.Enter)
			{
				CurrentDisplay.UpdateDisplay(ProgressDisplay.ActualWidth, ProgressDisplay.ActualHeight);
				e.Handled = true;
			}
		}

		private void UpdateProgressData(object sender, RoutedEventArgs e)
		{
			UpdateUIWithProgressData();
		}

		const int c_widthOffset = 14;
		const int c_heightOffset = 38;
		const string c_windowTitle = "Goal Reacher: {0}";

		private void ProgressSelector_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
		{
			CurrentDisplay = ((ComboBox)sender).SelectedItem as ProgressDisplay;
			if (CurrentDisplay != null)
			{
				ProgressDisplay.Content = CurrentDisplay;
				Title = string.Format(c_windowTitle, CurrentDisplay.Title);
			}
		}
	}
}
