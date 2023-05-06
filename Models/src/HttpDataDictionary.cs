namespace Zaharuddin.Models;

// Partial class
public partial class cityfmcodetests {
    /// <summary>
    /// HttpData Dictionary
    /// </summary>
    public class HttpDataDictionary : Dictionary<string, object>, IDisposable
    {
        private bool _disposed = false;

        // Overrides Object.Finalize
        ~HttpDataDictionary() => Dispose(false);

        // Releases all resources used by this object
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        // Custom Dispose method to clean up unmanaged resources
        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;
            // if (disposing) {
            //     foreach (var (key, value) in this) {
            //         if (value is IDisposable d)
            //             d.Dispose();
            //     }
            // }
            _disposed = true;
        }

        // Indexer
        public new object this[string key]
        {
            get => TryGetValue(key, out object? value) ? value : null!;
            set => base[key] = value;
        }

        // Get
        public T Get<T>(string key) => this[key] is var value && value != null ? (T)value : default(T)!;

        // Resolve (Get or create)
        [return: NotNull]
        public T Resolve<T>(string key) where T : new() => this[key] is var value && value != null ? (T)value : (T)(base[key] = new T());
    }
} // End Partial class
