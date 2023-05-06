namespace Zaharuddin.Models;

// Partial class
public partial class cityfmcodetests {
    /// <summary>
    /// Data class
    /// </summary>
    public class DotAccessData
    {
        /// <summary>
        /// Internal representation of data
        /// </summary>
        protected IDictionary<string, dynamic?> _data;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="data">Dictionary</param>
        public DotAccessData(IDictionary<string, dynamic?>? data = null) =>
            _data = data ?? new Dictionary<string, dynamic?>();

        /// <summary>
        /// Append
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="value">Value</param>
        public void Append(string key, dynamic? value = null)
        {
            if (Empty(key))
                throw new Exception("Key cannot be an empty.");
            dynamic currentValue = _data;
            string[] keyPath = key.Split('.');
            if (keyPath.Length == 1) {
                if (!currentValue.ContainsKey(key))
                    currentValue[key] = new List<dynamic>();
                if (!IsList(currentValue[key]))
                    currentValue[key] = new List<dynamic> { currentValue[key] };
                ((List<dynamic>)currentValue[key]).Add(value);
                return;
            }
            string endKey = keyPath.Last();
            for (int i = 0; i < keyPath.Length - 1; i++) {
                string currentKey = keyPath[i];
                if (!currentValue.ContainsKey(currentKey))
                    currentValue[currentKey] = new Dictionary<string, dynamic>();
                currentValue = currentValue[currentKey];
            }
            if (!currentValue.ContainsKey(endKey))
                currentValue[endKey] = new List<dynamic>();
            if (!IsList(currentValue[endKey])) // Promote this key to a List
                currentValue[endKey] = new List<dynamic> { currentValue[endKey] };
            ((List<dynamic>)currentValue[endKey]).Add(value);
        }

        /// <summary>
        /// Set
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="value">Value</param>
        public void Set(string key, dynamic? value = null)
        {
            if (Empty(key))
                throw new Exception("Key cannot be an empty.");
            dynamic currentValue = _data;
            string[] keyPath = key.Split('.');
            if (keyPath.Length == 1) {
                currentValue[key] = value;
                return;
            }
            string endKey = keyPath.Last();
            for (int i = 0; i < keyPath.Length - 1; i++) {
                string currentKey = keyPath[i];
                if (!currentValue.ContainsKey(currentKey))
                    currentValue[currentKey] = new Dictionary<string, dynamic>();
                if (!IsDictionary(currentValue[currentKey]))
                    throw new Exception($"Key path at {currentKey} of {key} cannot be set (not a dictionary).");
                currentValue = currentValue[currentKey];
            }
            currentValue[endKey] = value;
        }

        /// <summary>
        /// Remove
        /// </summary>
        /// <param name="key">Key</param>
        /// <returns>Data successfully found and removed</returns>
        public bool Remove(string key)
        {
            if (Empty(key))
                throw new Exception("Key cannot be an empty.");
            dynamic currentValue = _data;
            string[] keyPath = key.Split('.');
            if (keyPath.Length == 1)
                return currentValue.Remove(key);
            string endKey = keyPath.Last();
            for (int i = 0; i < keyPath.Length - 1; i++) {
                string currentKey = keyPath[i];
                if (!currentValue.ContainsKey(currentKey))
                    return false;
                currentValue = currentValue[currentKey];
            }
            return currentValue.Remove(endKey);
        }

        /// <summary>
        /// Get a value
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="def">Default value</param>
        /// <returns>Value</returns>
        public dynamic? Get(string key, dynamic? def = null)
        {
            dynamic currentValue = _data;
            string[] keyPath = key.Split('.');
            for (int i = 0; i < keyPath.Length; i++) {
                string currentKey = keyPath[i];
                if (!currentValue.ContainsKey(currentKey))
                    return def;
                if (!IsDictionary(currentValue))
                    return def;
                currentValue = currentValue[currentKey];
            }
            return currentValue ?? def;
        }

        /// <summary>
        /// Contains key
        /// </summary>
        /// <param name="key">Key</param>
        /// <returns>Key found or not</returns>
        public bool Has(string key)
        {
            dynamic currentValue = _data;
            string[] keyPath = key.Split('.');
            for (int i = 0; i < keyPath.Length; i++) {
                string currentKey = keyPath[i];
                if (!IsDictionary(currentValue) || !currentValue.ContainsKey(currentKey))
                    return false;
                currentValue = currentValue[currentKey];
            }
            return true;
        }

        /// <summary>
        /// Get Data dynamic
        /// </summary>
        /// <param name="key">Key</param>
        /// <returns>Data</returns>
        public DotAccessData GetData(string key)
        {
            if (Get(key) is IDictionary<string, dynamic?> d)
                return new DotAccessData(d);
            throw new Exception("Value at '" + key + "' could not be represented as a dictionary");
        }

        /// <summary>
        /// Import dictionary
        /// </summary>
        /// <param name="data">Dictionary to be imported</param>
        public IDictionary<string, dynamic?> Import(IDictionary<string, dynamic?> data) => Merge(_data, data);

        /// <summary>
        /// Import dictionary
        /// </summary>
        /// <param name="data">Object to be imported</param>
        public IDictionary<string, dynamic?> Import(object data) => Merge(_data, data);

        /// <summary>
        /// Import data
        /// </summary>
        /// <param name="data">Dictionary to be imported</param>
        public IDictionary<string, dynamic?> ImportData(DotAccessData data) => Import(data.ToDictionary());

        /// <summary>
        /// To dictionary
        /// </summary>
        /// <returns>Return as dictionary</returns>
        public Dictionary<string, dynamic?> ToDictionary() => new (_data);

        /// <summary>
        /// To JSON string
        /// </summary>
        /// <returns>Return JSON string of data</returns>
        public string ToJson() => ConvertToJson(_data);
    }
} // End Partial class
