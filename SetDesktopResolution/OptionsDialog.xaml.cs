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
	using System.Windows.Shapes;

	using SetDesktopResolution.Common.Windows;

	/// <summary>
	/// Interaction logic for OptionsDialog.xaml
	/// </summary>
	public partial class OptionsDialog : Window
	{
		public OptionsDialog()
		{
			InitializeComponent();
		}

		private void CancelButtonOnClick(object sender, RoutedEventArgs e) => Close();

		private void OkButtonClick(object sender, RoutedEventArgs e) => Close();
	}
}
