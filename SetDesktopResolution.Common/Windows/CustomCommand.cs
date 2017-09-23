namespace SetDesktopResolution.Common.Windows
{
	using System;
	using System.Windows.Input;

	public class CustomCommand : CustomCommand<object>
	{
		public CustomCommand(Action command, bool canExecute = true)
			: base(_ => command(), canExecute)
		{
		}

		public void Execute() => CommandAction?.Invoke(null);
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
		public virtual bool CanExecute(object parameter) => this.CanExecuteValue;

		/// <inheritdoc />
		public void Execute(object parameter)
		{
			// Resharper likes to complain about this sometimes, but it compiles okay
			if (this is CustomCommand c)
			{
				c.Execute();
				return;
			}

			if (!(parameter is T o))
				throw new ArgumentException("Invalid parameter type");

			Execute(o);
		}

		public void Execute(T parameter) => CommandAction?.Invoke(parameter);

		public event EventHandler CanExecuteChanged;

		protected void OnCanExecuteChanged(object sender, EventArgs e) => CanExecuteChanged?.Invoke(sender, e);
	}
}
