namespace SetDesktopResolution
{
	using System;
	using System.CodeDom;
	using System.Collections.Generic;
	using System.ComponentModel;
	using System.Linq;
	using System.Runtime.CompilerServices;
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

	using JetBrains.Annotations;

	using Microsoft.Win32;

	using Serilog;

	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	/// <inheritdoc cref="Window" />
	public partial class MainWindow : Window, INotifyPropertyChanged
	{
		public MainWindowViewModel ViewModel { get; }
		
		public MainWindow(App app)
		{
			InitializeComponent();
			ViewModel = new MainWindowViewModel(app.LogEvents);
			OnPropertyChanged(nameof(ViewModel));
		}

		private void BrowseButtonClick(object sender, RoutedEventArgs e)
		{
			var openDialog = new OpenFileDialog
			{
				Filter = "Executable (.exe)|*.exe"
			};

			if (!(openDialog.ShowDialog() ?? false))
				return;

			ViewModel.ExecutablePath = openDialog.FileName;
		}

		private void LogTextBoxScrollToEnd(object sender, DataTransferEventArgs e)
		{
			LogTextBox.ScrollToEnd();
		}

		public event PropertyChangedEventHandler PropertyChanged;

		[NotifyPropertyChangedInvocator]
		private void OnPropertyChanged([CallerMemberName] string propertyName = null) => 
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

		private void ButtonClick(object sender, RoutedEventArgs e)
		{
			new OptionsDialog().ShowDialog();

			ViewModel.Update();
		}
	}
}
