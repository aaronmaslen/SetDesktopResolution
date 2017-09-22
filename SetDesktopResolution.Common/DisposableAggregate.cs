namespace SetDesktopResolution.Common
{
	using System;
	using System.Collections.Generic;
	using System.Text;

	using JetBrains.Annotations;

	using SetDesktopResolution.Common.Extensions;

	/// <summary>
	/// Aaggregates multiple disposables
	/// </summary>
	public class DisposableAggregate : IDisposable
	{
		protected ICollection<IDisposable> Disposables { get; } = new List<IDisposable>();
		
		public DisposableAggregate(IEnumerable<IDisposable> disposables)
		{
			Disposables.AddAll(disposables);
		}

		public void Add([NotNull] IDisposable d) => Disposables.Add(d);

		public void Remove([NotNull] IDisposable d) => Disposables.Remove(d);

		protected virtual void Dispose(bool disposing)
		{
			if (!disposing) return;
			
			foreach (var d in Disposables)
				d.Dispose();
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}
	}
}
