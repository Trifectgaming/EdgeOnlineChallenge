using System;

namespace Assets.Scripts
{
    public class Disposable : IDisposable
    {
        private readonly Action _onDispose;

        public Disposable(Action onDispose)
        {
            _onDispose = onDispose;
        }

        public void Dispose()
        {
            if (_onDispose != null)
                _onDispose();
        }
    }
}