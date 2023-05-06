namespace Zaharuddin.Models;

// Partial class
public partial class cityfmcodetests {
    /// <summary>
    /// Page class
    /// </summary>
    public class UploadHandler
    {
        public string UploadTable = "";

        // Page terminated // DN
        private bool _terminated = false;

        // Page class constructor
        public UploadHandler(Controller controller) { // DN
            Controller = controller;
        }

        // Page class constructor
        public UploadHandler() { // DN
        }

        // Download file content
        public async Task<IActionResult> DownloadFileContent()
        {
            string name = Get("id");
            UploadTable = Get("table");
            string filename = Get(name);
            string folder = UploadTempPath(name, UploadTable);
            string version = Get("version");
            if (!Empty(version))
                folder = PathCombine(folder, version, true);

            // Show file content (Config.ImageAllowedFileExtensions and Config.DownloadAllowedFileExtensions only)
            var ar = Config.ImageAllowedFileExtensions.Concat(Config.DownloadAllowedFileExtensions).ToArray();
            string file = IncludeTrailingDelimiter(folder, true) + filename;
            if (Regex.IsMatch(filename.ToLower(), @"\.(" + String.Join("|", ar) + ")$")) {
                if (FileExists(file)) {
                    var value = await FileReadAllBytes(file);
                    string contentType = ContentType(value, filename);
                    if (Regex.IsMatch(filename.ToLower(), @"\.pdf$") && Config.EmbedPdf)
                        return Controller.File(value, contentType);
                    else
                        return Controller.File(value, contentType, filename);
                }
            }
            return new EmptyResult();
        }

        // Delete file
        public IActionResult Delete()
        {
            string name = Get("id");
            if (name != "") {
                UploadTable = Get("table");
                string filename = Get(name);
                var folder = UploadTempPath(name, UploadTable);
                DeleteFile(IncludeTrailingDelimiter(folder, true) + filename);
                var version = Config.UploadThumbnailFolder;
                folder = PathCombine(folder, version, true);
                DeleteFile(IncludeTrailingDelimiter(folder, true) + filename);
                return Controller.Json(new { success = true });
            }
            return new EmptyResult();
        }

        // Download file list
        public async Task<IActionResult> DownloadFileList()
        {
            string name = Get("id");
            string token = Get<string>(Config.TokenName) ?? Request?.Headers[Config.TokenName.HeaderCase()].ToString() ?? "";
            UploadTable = Get("table");
            List<object[]> files = new ();
            if (name != "") {
                var folder = UploadTempPath(name, UploadTable);
                if (DirectoryExists(folder)) {
                    var ar = GetFiles(folder);
                    foreach (var file in ar) {
                        var value = await FileReadAllBytes(file);
                        var filesize = value.Length;
                        string filetype = ContentType(value, file);
                        files.Add(new object[] { name, file, filetype, filesize, token });
                    }
                }
                return OutputJson(name, files);
            }
            return new EmptyResult();
        }

        // Upload file
        public async Task<IActionResult> Upload()
        {
            if (Files.Count > 0) { // DN
                Language = ResolveLanguage();
                HttpForm form = new ();
                await form.Init();
                string name = form.GetValue("id");
                UploadTable = form.GetValue("table");
                string folder = UploadTempPath(name, UploadTable);
                string token = Post<string>(Config.TokenName) ?? Request?.Headers[Config.TokenName.HeaderCase()].ToString() ?? "";
                string exts = form.GetValue("exts");
                List<string> extList = exts.Split(',').ToList();
                if (!Empty(Config.UploadAllowedFileExtensions)) {
                    var allowedExtList = Config.UploadAllowedFileExtensions.Split(',');
                    exts = String.Join(",", extList.Where(ext => allowedExtList.Contains(ext, StringComparer.OrdinalIgnoreCase))); // Make sure exts is a subset of Config.UploadAllowedFileExtensions
                    if (Empty(exts))
                        exts = Config.UploadAllowedFileExtensions;
                }
                if (Empty(exts))
                    exts = @"\w+";
                string filetypes = @"\.(" + exts.Replace(",", "|") + ")$";
                int maxsize = form.GetInt("maxsize");
                int maxfilecount = form.GetInt("maxfilecount");
                string filename = form.GetUploadFileName(name);

                // Skip if no file uploaded
                if (Empty(filename))
                    return Controller.BadRequest("Missing file name");
                if (Config.UploadConvertAccentedChars) {
                    filename = HtmlEncode(filename);
                    filename = Regex.Replace(filename, @"&([a-zA-Z])(uml|acute|grave|circ|tilde|cedil);", "$1");
                    filename = HtmlDecode(filename);
                }
                string filetype = form.GetUploadFileContentType(name);
                long filesize = form.GetUploadFileSize(name);
                byte[] value = await form.GetUploadFileData(name);

                // Check file types
                if (!Regex.IsMatch(filename, filetypes, RegexOptions.IgnoreCase)) {
                    string fileerror = Language.Phrase("UploadErrorAcceptFileTypes");
                    return OutputJson("files", new () { new object[] { name, filename, filetype, filesize, token, fileerror }});
                }

                // Check file size
                if (maxsize > 0 && maxsize < filesize) {
                    string fileerror = Language.Phrase("UploadErrorMaxFileSize");
                    return OutputJson("files", new () { new object[] { name, filename, filetype, filesize, token, fileerror }});
                }

                // Check max file count
                int filecount = FolderFileCount(folder);
                if (maxfilecount > 0 && maxfilecount <= filecount) {
                    string fileerror = Language.Phrase("UploadErrorMaxNumberOfFiles");
                    return OutputJson("files", new () { new object[] { name, filename, filetype, filesize, token, fileerror } });
                }

                // Delete all files in directory if replace
                string version = Config.UploadThumbnailFolder;
                if (form.GetBool("replace"))
                    CleanPath(folder, false);
                await SaveFile(folder, filename, value);
                folder = PathCombine(folder, version, true);
                int w = Config.UploadThumbnailWidth;
                int h = Config.UploadThumbnailHeight;
                ResizeBinary(ref value, ref w, ref h);
                await SaveFile(folder, filename, value);
                return OutputJson("files", new () { new object[] { name, Path.Join(folder, filename), filetype, filesize, token } });
            }
            return new EmptyResult();
        }

        // Output JSON
        public IActionResult OutputJson(string id, List<object[]> files)
        {
            string baseurl, table, url, thumbnail_url, delete_url = "";
            var list = new List<Dictionary<string, object>>();
            if (IsList(files)) {
                foreach (var file in files) {
                    if (file.Length >= 5) {
                        string name = ConvertToString(file[0]);
                        string filename = Path.GetFileName(ConvertToString(file[1])); // Full path file in file[1] // DN
                        string fileerror = (file.Length > 5) ? ConvertToString(file[5]) : "";
                        string version = Config.UploadThumbnailFolder;
                        string token = ConvertToString(file[4]);
                        if (Config.DownloadViaScript || Empty(Config.UploadTempPath) || Empty(Config.UploadTempHrefPath)) {
                            baseurl = FullUrl(CurrentPageName().ToLower(), "upload");
                            table = (UploadTable != "") ? "&table=" + UploadTable : "";
                            url = baseurl + "?id=" + name + table + "&" + name + "=" + RawUrlEncode(filename) + "&download=1";
                            thumbnail_url = baseurl + "?id=" + name + table + "&" + name + "=" + RawUrlEncode(filename) + "&version=" + version + "&download=1";
                            delete_url = baseurl + "?id=" + name + table + "&" + name + "=" + RawUrlEncode(filename) + "&delete=1";
                        } else {
                            baseurl = UploadTempPath("", "", false);
                            table = (UploadTable != "") ? UploadTable + "/" : "";
                            url = baseurl + table + name + "/" + RawUrlEncode(filename);
                            thumbnail_url = baseurl + table + name + "/" + version + "/" + RawUrlEncode(filename);
                        }
                        url += "&" + Config.TokenName + "=" + token;
                        thumbnail_url += "&" + Config.TokenName + "=" + token;
                        string ext = Path.GetExtension(filename).Replace(".", "").ToLower();
                        if (!Config.ImageAllowedFileExtensions.Contains(ext, StringComparer.OrdinalIgnoreCase)) // Non image files
                            thumbnail_url = "";
                        var obj = new Dictionary<string, object>();
                        obj.Add("name", filename);
                        obj.Add("extension", Path.GetExtension(filename).Replace(".", ""));
                        obj.Add("size", ConvertToInt(file[3]));
                        obj.Add("type", ConvertToString(file[2]));
                        obj.Add("url", url);
                        if (!Empty(fileerror)) {
                            obj.Add("error", fileerror);
                        } else {
                            obj.Add(version + "Url", thumbnail_url);
                        }
                        obj.Add("deleteUrl", delete_url);
                        obj.Add("deleteType", "GET"); // Use GET
                        if (Empty(fileerror)) {
                            if (Config.ImageAllowedFileExtensions.Contains(ext, StringComparer.OrdinalIgnoreCase)) { // Image files
                                try {
                                    IEnumerable<MetadataExtractor.Directory> directories = MetadataExtractor.ImageMetadataReader.ReadMetadata(ConvertToString(file[1]));
                                    obj.Add("exists", !directories.Any(directory => directory.Name == "IPTC" && directory.Tags.Any(tag => tag.Description == "FileNotFound")));
                                } catch {}
                            } else { // Non image files
                                obj.Add("exists", true);
                            }
                        }
                        list.Add(obj);
                    }
                }
            }

            // Set file header / content type
            AddHeader(HeaderNames.ContentDisposition, "inline; filename=files.json");

            // Output json
            var dict = new Dictionary<string, List<Dictionary<string, object>>> { {id, list} };
            return Controller.Json(dict); // Returns utf-8 data
        }

        /// <summary>
        /// Page run
        /// </summary>
        /// <returns>Page result</returns>
        [HttpCacheExpiration(CacheLocation = CacheLocation.Private, NoStore = true, MaxAge = 0)]
        public async Task<IActionResult> Run() { // DN
            // Handle download file content
            if (Get("download") != "") {
                return await DownloadFileContent();
            } else if (Get("delete") != "") { // Handle delete file
                return Delete();
            } else if (Get("id") != "") { // Handle download file list
                return await DownloadFileList();
            } else if (Files.Count > 0) { // Handle upload file (multi-part)
                return await Upload();
            }
            return new EmptyResult();
        }

        // Terminate page
        public IActionResult Terminate(string url = "") { // DN
            if (_terminated)
                return new EmptyResult();
            _terminated = true;
            return new EmptyResult();
        }
    }
} // End Partial class
