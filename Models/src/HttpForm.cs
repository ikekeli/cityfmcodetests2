namespace Zaharuddin.Models;

// Partial class
public partial class cityfmcodetests {
    /// <summary>
    /// Form class
    /// </summary>
    public class HttpForm
    {
        public int Index { get; set; } = -1;

        public string FormName = "";

        public IFormCollection Data = FormCollection.Empty;

        // Constructor
        public async Task Init() =>
            Data = Request != null && Request.HasJsonContentType()
                ? await Request.ReadFromJsonAsync<Dictionary<string, object>>() is var jsonData && jsonData != null
                    ? new FormCollection(jsonData.ToDictionary(
                            kvp => kvp.Key,
                            kvp => kvp.Value is JsonElement je && je.ValueKind == JsonValueKind.Array
                                ? new StringValues(je.EnumerateArray().ToList().Select(v => ConvertToString(v)).ToArray())
                                : new StringValues(ConvertToString(kvp.Value))
                        ))
                    : FormCollection.Empty
                : (IFormCollection)Form;

        // Set index // DN
        public void SetIndex(int index) => Index = index;

        // Reset index // DN
        public void ResetIndex() => Index = -1;

        // Get form element name based on index
        public string GetIndexedName(string name) => (Index < 0) ? name : Regex.Replace(name, "^([a-z]{1,2})_", "${1}" + Index + "_", RegexOptions.IgnoreCase);

        // Get form element name with form name prefix
        public string GetPrefixedName(string name) => Regex.IsMatch(name, @"^(fn_)?(x|o)\d*_") && FormName != "" ? FormName + "$" + name : name;

        // Has value for form element
        public bool HasValue(string name) => GetIndexedName(name) is var wrkname && (Data.ContainsKey(GetPrefixedName(wrkname)) || Data.ContainsKey(wrkname));

        // Try get value
        public bool TryGetValue(string name, [NotNull] out StringValues value) => GetIndexedName(name) is var wrkname &&
            Data.TryGetValue(GetPrefixedName(wrkname), out value) || Data.TryGetValue(wrkname, out value);

        // Get value for form element
        public string GetValue(string name) => TryGetValue(name, out StringValues sv) ? String.Join(Config.MultipleOptionSeparator, sv.ToArray()) : String.Empty;

        // Get int value for form element // DN
        public int GetInt(string name) => ConvertToInt(GetValue(name));

        // Get bool value for form element // DN
        public bool GetBool(string name) => ConvertToBool(GetValue(name));

        // Get file
        public IFormFile GetFile(string name, int index = 0) => Files.GetFiles(GetIndexedName(name))[index];

        // Get files
        public IReadOnlyList<IFormFile> GetFiles(string name) => Files.GetFiles(GetIndexedName(name));

        // Get upload file size
        public long GetUploadFileSize(string name, int index = 0) => GetFile(name, index)?.Length ?? -1;

        // Get upload file name
        public string GetUploadFileName(string name, int index = 0)
        {
            var file = GetFile(name, index);
            if (file != null) {
                string fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Value?.Trim('"') ?? "";
                return Path.GetFileName(fileName);
            }
            return "";
        }

        // Get file content type
        public string GetUploadFileContentType(string name, int index = 0)
        {
            var file = GetFile(name, index);
            if (file != null) {
                string ct = ContentType(GetUploadFileName(name));
                return (ct != "") ? ct : file.ContentType;
            }
            return "";
        }

        // Get upload file data
        public async Task<byte[]> GetUploadFileData(string name, int index = 0)
        {
            var file = GetFile(name, index);
            if (file != null && file.Length > 0) {
                using var stream = new MemoryStream();
                await file.CopyToAsync(stream);
                return stream.ToArray();
            }
            return new byte[] {};
        }

        // Save upload file
        public async Task<bool> SaveUploadFile(string name, string filePath, int index = 0)
        {
            var file = GetFile(name, index);
            if (file != null && file.Length > 0) {
                using var stream = new FileStream(filePath, FileMode.Create);
                await file.CopyToAsync(stream);
                return true;
            }
            return false;
        }

        // Get file image width and height
        public (int width, int height) GetUploadImageDimension(string name, int index = 0)
        {
            var file = GetFile(name, index);
            if (file != null && file.Length > 0) {
                try {
                    using var readStream = file.OpenReadStream();
                    var info = new MagickImageInfo(readStream);
                    return (width: info.Width, height: info.Height);
                } catch {}
            }
            return (width: -1, height: -1);
        }
    }
} // End Partial class
