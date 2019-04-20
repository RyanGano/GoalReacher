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
				yield return ProgressDisplay.Create(goal, (int)Width - c_widthOffset, (int)Height - c_heightOffset);
		}

		private void OnSizeChanged(object sender, SizeChangedEventArgs e)
		{
			UpdateDisplay();
		}

		private void UpdateProgress_Click(object sender, RoutedEventArgs e)
		{
			UpdateDisplay();
		}

		private void TextBox_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
		{
			if (e.Key == System.Windows.Input.Key.Enter)
			{
				UpdateDisplay();
				e.Handled = true;
			}
		}

		private void UpdateDisplay()
		{
			CurrentDisplay.UpdateDisplay(Display.ActualWidth, Display.ActualHeight);
		}

		private void UpdateProgressData(object sender, RoutedEventArgs e)
		{
			UpdateUIWithProgressData();
		}

		private void ProgressSelector_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
		{
			CurrentDisplay = ((ComboBox)sender).SelectedItem as ProgressDisplay;
			if (CurrentDisplay != null)
			{
				Display.Content = CurrentDisplay;
				Title = string.Format(c_windowTitle, CurrentDisplay.Title);
			}
		}

		const int c_widthOffset = 14;
		const int c_heightOffset = 38;
		const string c_windowTitle = "Goal Reacher: {0}";
	}
}
