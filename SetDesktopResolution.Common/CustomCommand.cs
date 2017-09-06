namespace SetDesktopResolution.Common
{
	using System;
	using System.Linq;
	using System.Windows.Input;

	public class CustomCommand : CustomCommand<object>
	{
		public CustomCommand(Action command, bool canExecute = true)
			: base(_ => command(), canExecute)
		{
		}

		public void Execute() => Execute(null);
	}

	public class CustomCommand<T> : ICommand
	{
		public CustomCommand(Action<T> command, bool canExecute = true)
		{
			CommandAction = command;
			CanExecuteValue = canExecute;
		}
		
		protected Action<T> CommandAction { get; set; }

		protected bool CanExecuteValue { get; set; }

		/// <inheritdoc />
		public virtual bool CanExecute(object parameter) => CanExecuteValue;

		/// <inheritdoc />
		public void Execute(object parameter)
		{
			if (!(parameter is T o)) throw new ArgumentException("Invalid parameter type");
				
			Execute(o);
		}

		public void Execute(T parameter) => CommandAction?.Invoke(parameter);

		public event EventHandler CanExecuteChanged;

		protected void OnCanExecuteChanged(object sender, EventArgs e) => CanExecuteChanged?.Invoke(sender, e);
	}
}
