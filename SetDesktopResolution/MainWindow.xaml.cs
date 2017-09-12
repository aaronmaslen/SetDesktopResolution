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

			ExecutablePathTextBox.Text = openDialog.FileName;
		}

		private void LogTextBoxScrollToEnd(object sender, DataTransferEventArgs e)
		{
			LogTextBox.ScrollToEnd();
		}

		private void RunButtonClick(object sender, RoutedEventArgs e)
		{
			DisableControls(new Control[]
				                {
					                DevicesComboBox,
					                ModesComboBox,
					                ExecutablePathTextBox,
					                BrowseButton,
					                RunButton,
				                });
		}

		private readonly ICollection<Control> _disabledControls = new List<Control>();
		
		private void DisableControls(IEnumerable<Control> controlsToDisable)
		{
			foreach (var c in controlsToDisable)
			{
				c.IsEnabled = false;

				if (_disabledControls.Contains(c))
					continue;

				_disabledControls.Add(c);
			}
		}

		private void EnableControls()
		{
			foreach (var c in _disabledControls)
			{
				c.IsEnabled = true;
				_disabledControls.Remove(c);
			}
		}

		public event PropertyChangedEventHandler PropertyChanged;

		[NotifyPropertyChangedInvocator]
		protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}
