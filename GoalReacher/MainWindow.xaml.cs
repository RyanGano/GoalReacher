using System.Windows;

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
			m_progressDisplay = new ProgressDisplay(@"Resources\House.jpg", (int)Width - c_widthOffset, (int)Height - c_heightOffset, 350, new decimal(48502.99), new decimal(26474.22));
			ProgressDisplay.Content = m_progressDisplay;
			this.SizeChanged += OnSizeChanged;
		}

		private void OnSizeChanged(object sender, SizeChangedEventArgs e)
		{
			m_progressDisplay.UpdateSize(e.NewSize.Width - c_widthOffset, e.NewSize.Height - c_heightOffset);
		}

		ProgressDisplay m_progressDisplay;

		private void UpdateProgress_Click(object sender, RoutedEventArgs e)
		{
			m_progressDisplay.UpdateSize((int)Width - c_widthOffset, (int)Height - c_heightOffset);
		}

		const int c_widthOffset = 14;
		const int c_heightOffset = 38;
	}
}
