namespace SetDesktopResolution
{
	using System;
	using System.Collections.Generic;
	using System.ComponentModel;
	using System.Linq;
	using System.Runtime.CompilerServices;
	using System.Text;
	using System.Threading.Tasks;
	using System.Windows.Input;

	using JetBrains.Annotations;

	using Serilog;
	using Serilog.Events;

	using SetDesktopResolution.Common.Windows;
	using SetDesktopResolution.Common.Wmi;

	public class SettingsViewModel : INotifyPropertyChanged
	{
		private static readonly Dictionary<string, object> DefaultSettings =
			new Dictionary<string, object>
				{
					{ nameof(MinimumLogDisplayLevel), LogEventLevel.Information },
					{ nameof(MinimumRefreshRate), string.Empty },
				    { nameof(MaximumRefreshRate), string.Empty },
					{ nameof(Only32BitColor), true },
					{ nameof(IncludeInterlacedModes), false }
				};
		
		public SettingsViewModel()
		{
			GetSettings();
		}

		private readonly Properties.Settings _settings = Properties.Settings.Default;
        
		private void GetSettings()
		{
			MinimumLogDisplayLevel = _settings.MinimumLogDisplayLevel;

			CustomFilterSettings = _settings.OverrideModeFilter;
			
			var minrr = _settings.MinimumRefreshRate;
			MinimumRefreshRate = minrr > 0 ? minrr.ToString() : string.Empty;
			OverrideMinimumRefreshRate = MinimumRefreshRate != string.Empty;
            
			var maxrr = _settings.MaximumRefreshRate;
			MaximumRefreshRate = maxrr > 0 ? maxrr.ToString() : string.Empty;
			OverrideMaximumRefreshRate = MaximumRefreshRate != string.Empty;

			Only32BitColor = _settings.Only32BitColor;

			IncludeInterlacedModes = _settings.IncludeInterlacedModes;

			SingleProcessMode = _settings.ProcessDetectionMode == ProcessDetectionMode.SingleProcess;
			ProcessAndChildrenMode = _settings.ProcessDetectionMode == ProcessDetectionMode.ProcessPlusChildren;
			RunningProcessDetectionMode = _settings.ProcessDetectionMode == ProcessDetectionMode.RunningProcess;
		}

		private void SaveSettings()
		{
			_settings.MinimumLogDisplayLevel = MinimumLogDisplayLevel;

			_settings.OverrideModeFilter = CustomFilterSettings;

			if (!CustomFilterSettings)
				ResetToDefault(new[]
					               {
						               nameof(Only32BitColor),
						               nameof(MinimumRefreshRate),
						               nameof(MaximumRefreshRate),
						               nameof(IncludeInterlacedModes)
					               });
			
			if (MinimumRefreshRate == string.Empty)
				_settings.MinimumRefreshRate = 0;
			else if (int.TryParse(MinimumRefreshRate, out var minrr))
				_settings.MinimumRefreshRate = minrr;

			if (MaximumRefreshRate == string.Empty)
				_settings.MaximumRefreshRate = -1;
			else if (int.TryParse(MaximumRefreshRate, out var maxrr))
				_settings.MaximumRefreshRate = maxrr;

			_settings.Only32BitColor = Only32BitColor;

			_settings.IncludeInterlacedModes = IncludeInterlacedModes;

			if (SingleProcessMode) _settings.ProcessDetectionMode = ProcessDetectionMode.SingleProcess;
			else if (ProcessAndChildrenMode) _settings.ProcessDetectionMode = ProcessDetectionMode.ProcessPlusChildren;
			else if (RunningProcessDetectionMode) _settings.ProcessDetectionMode = ProcessDetectionMode.RunningProcess;
			
			_settings.Save();
		}

		private void ResetToDefault(IEnumerable<string> propertiesToReset)
		{
			foreach (var s in propertiesToReset)
			{
				if (!DefaultSettings.ContainsKey(s))
					throw new ArgumentException($"No default value for {s}");

				switch (s)
				{
					case nameof(MinimumLogDisplayLevel):
						MinimumLogDisplayLevel = (LogEventLevel)DefaultSettings[s];
						break;
					case nameof(MinimumRefreshRate):
						MinimumRefreshRate = (string)DefaultSettings[s];
						break;
					case nameof(MaximumRefreshRate):
						MaximumRefreshRate = (string)DefaultSettings[s];
						break;
					case nameof(Only32BitColor):
						Only32BitColor = (bool)DefaultSettings[s];
						break;
					case nameof(IncludeInterlacedModes):
						IncludeInterlacedModes = (bool)DefaultSettings[s];
						break;
					default:
						throw new ArgumentException($"Unknown property {s}");
				}
			}
		}

		private bool _customFilterSettings;

		public bool CustomFilterSettings
		{
			get => _customFilterSettings;
			set
			{
				if (_customFilterSettings == value) return;
				
				_customFilterSettings = value;
				OnPropertyChanged();		
			}
		}

		private bool _includeInterlacedModes;

		public bool IncludeInterlacedModes
		{
			get => _includeInterlacedModes;
			set
			{
				if (_includeInterlacedModes == value) return;
				
				_includeInterlacedModes = value;
				OnPropertyChanged();
			}
		}

		private LogEventLevel _minimumEventLevel;
        
		public LogEventLevel MinimumLogDisplayLevel
		{
			get => _minimumEventLevel;
			set
			{
				if (_minimumEventLevel == value) return;

				_minimumEventLevel = value;
				OnPropertyChanged();
			}
		}

		private bool _overrideMinimumRefreshRate;

		public bool OverrideMinimumRefreshRate
		{
			get => _overrideMinimumRefreshRate;
			set
			{
				if (_overrideMinimumRefreshRate == value) return;
				
				_overrideMinimumRefreshRate = value;
				OnPropertyChanged();
			}
		}

		private string _minimumRefreshRate;

		public string MinimumRefreshRate
		{
			get => _minimumRefreshRate;
			set
			{
				if (_minimumRefreshRate == value) return;
                
				_minimumRefreshRate = value;
				OnPropertyChanged();
			}
		}

		private bool _overrideMaximumRefreshRate;

		public bool OverrideMaximumRefreshRate
		{
			get => _overrideMaximumRefreshRate;
			set
			{
				if (_overrideMaximumRefreshRate == value) return;
				
				_overrideMaximumRefreshRate = value;
				OnPropertyChanged();
			}
		}

		private string _maximumRefreshRate;

		public string MaximumRefreshRate
		{
			get => _maximumRefreshRate;
			set
			{
				if (_maximumRefreshRate == value) return;
                
				_maximumRefreshRate = value;
				OnPropertyChanged();
			}
		}

		private bool _only32BitColor;

		public bool Only32BitColor
		{
			get => _only32BitColor;
			set
			{
				if (_only32BitColor == value) return;
				
				_only32BitColor = value;
				OnPropertyChanged();
			}
		}

		private bool _singleProcessMode;

		public bool SingleProcessMode
		{
			get => _singleProcessMode;
			set
			{
				if (_singleProcessMode == value) return;
				
				_singleProcessMode = value;
				OnPropertyChanged();
			}
		}

		private bool _processAndChildrenMode;

		public bool ProcessAndChildrenMode
		{
			get => _processAndChildrenMode;
			set
			{
				if (_processAndChildrenMode == value) return;
				
				_processAndChildrenMode = value;
				OnPropertyChanged();
			}
		}

		private bool _runningProcessDetectionMode;

		public bool RunningProcessDetectionMode
		{
			get => _runningProcessDetectionMode;
			set
			{
				if (_runningProcessDetectionMode == value) return;
				
				_runningProcessDetectionMode = value;
				OnPropertyChanged();
			}
		}
		
		public ICommand ExitAndSaveCommand => new CustomCommand(() =>
			{
				try
				{
					SaveSettings();
				}
				catch (ArgumentException ae)
				{
					Log.Logger.Error(ae, "Error while saving settings");
				}
			});

		public event PropertyChangedEventHandler PropertyChanged;

		[NotifyPropertyChangedInvocator]
		protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) =>
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
	}
}
