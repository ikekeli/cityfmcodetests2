namespace Zaharuddin.Models;

// Partial class
public partial class cityfmcodetests {
    /// <summary>
    /// Session class
    /// </summary>
    public class HttpSession
    {
        private JsonSerializerSettings _settings = new () { TypeNameHandling = TypeNameHandling.All };

        // Keys
        public IEnumerable<string>? Keys => _session?.Keys;

        // Session ID
        public string SessionId => _session?.Id ?? "";

        // Get session
        private ISession? _session => UseSession ? HttpContext?.Session : null; // No session for external use of API

        // Remove
        public void Remove(string key) => _session?.Remove(key);

        // Clear
        public void Clear() => _session?.Clear();

        // Get value as bytes
        public byte[]? GetBytes(string key) => _session?.Get(key);

        // Set value as bytes
        public void SetBytes(string key, byte[] value) => _session?.Set(key, value);

        // Get value as string
        public string GetString(string key) => _session?.GetString(key) ?? "";

        // Set value as bytes
        public void SetString(string key, string value) => _session?.SetString(key, value);

        // Try get value as string
        public bool TryGetValue(string name, [NotNullWhen(true)] out string? value)
        {
            value = _session?.GetString(name);
            return value != null;
        }

        // Try get value as object
        public bool TryGetValue(string name, [NotNullWhen(true)] out object? value)
        {
            value = GetValue(name);
            return value != null;
        }

        // Get value as int32
        public int GetInt(string key) => _session?.GetInt32(key) ?? 0;

        // Set value as int32
        public void SetInt(string key, int value) => _session?.SetInt32(key, value);

        // Serialize and set
        public void SetValue(string key, object? value) => SetValue(key, value, _settings);

        // Serialize and set
        public void SetValue(string key, object? value, JsonSerializerSettings settings)
        {
            if (value == null)
                Remove(key);
            else
                SetString(key, JsonConvert.SerializeObject(value, settings));
        }

        // Get as deserialized object (TypeNameHandling.All)
        public object? GetValue(string key) => GetValue(key, _settings);

        // Get as deserialized object
        public object? GetValue(string key, JsonSerializerSettings settings)
        {
            try {
                var data = _session?.GetString(key);
                if (data != null)
                    return JsonConvert.DeserializeObject(data, settings);
            } catch {}
            return null;
        }

        // Get as deserialized type T (TypeNameHandling.All)
        public T? GetValue<T>(string key) => GetValue<T>(key, _settings);

        // Get as deserialized type T
        public T? GetValue<T>(string key, JsonSerializerSettings settings)
        {
            try {
                var data = _session?.GetString(key);
                if (data != null)
                    return JsonConvert.DeserializeObject<T>(data, settings);
            } catch {}
            return default(T);
        }

        // Get value as bool
        public bool GetBool(string key) => ConvertToBool(GetValue(key));

        // Set value as bool
        public void SetBool(string key, bool value) => SetValue(key, value);

        // Get/Set as object (string)
        public object? this[string name]
        {
            get => _session?.GetString(name);
            set => _session?.SetString(name, ConvertToString(value));
        }
    }
} // End Partial class
