namespace SetDesktopResolution
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;
	using System.Threading.Tasks;
	using System.Windows;
	using System.Windows.Controls;
	using System.Windows.Data;
	using System.Windows.Documents;
	using System.Windows.Input;
	using System.Windows.Media;
	using System.Windows.Media.Imaging;
	using System.Windows.Navigation;
	using System.Windows.Shapes;
	using Microsoft.Win32;
	using SetDesktopResolution.Common;

	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	/// <inheritdoc cref="Window" />
	public partial class MainWindow : Window
	{

		public MainWindow()
		{
			InitializeComponent();
		}
		
		private void ButtonClick(object sender, RoutedEventArgs e)
		{
			var openDialog = new OpenFileDialog
			{
				Filter = "Executable (.exe)|*.exe"
			};

			if(!(openDialog.ShowDialog() ?? false))
				return;

			ExecutablePathTextBox.Text = openDialog.FileName;
		}
	}
}
