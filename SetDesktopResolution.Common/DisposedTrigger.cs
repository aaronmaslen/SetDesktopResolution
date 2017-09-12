namespace SetDesktopResolution.Common
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    using JetBrains.Annotations;

    public class DisposedTrigger : IDisposable
    {
        protected Action OnDisposed { get; }

        public DisposedTrigger([NotNull] Action onDisposed)
        {
            OnDisposed = onDisposed;
        }
        
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
                OnDisposed();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
