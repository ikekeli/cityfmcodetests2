namespace Zaharuddin.Models;

// Partial class
public partial class cityfmcodetests {
    /// <summary>
    /// File Viewer class
    /// </summary>
    public class FileViewer
    {
        // Constructor
        public FileViewer(Controller controller) { // DN
            Controller = controller;
        }

        // Constructor
        public FileViewer() { // DN
        }

        /// <summary>
        /// Output file by file path
        /// </summary>
        /// <returns>Action result</returns>
        public async Task<IActionResult> GetFile(string fn)
        {
            // Get parameters
            string sessionId = Get("session");
            sessionId = Decrypt(sessionId);
            bool resize = Get<bool>("resize");
            int width = Get<int>("width");
            int height = Get<int>("height");
            bool download = Get("download", out StringValues d) ? ConvertToBool(d) : true; // Download by default
            if (width == 0 && height == 0 && resize) {
                width = Config.ThumbnailDefaultWidth;
                height = Config.ThumbnailDefaultHeight;
            }

            // If using session (internal request), file path is always encrypted.
            // If not (external request), DO NOT support external request for file path.
            string key = Config.RandomKey + sessionId;
            fn = UseSession ? Decrypt(fn, key) : "";
            if (FileExists(fn)) {
                Response?.Clear();
                string ext = Path.GetExtension(fn).Replace(".", "").ToLower();
                string ct = ContentType(fn);
                if (Config.ImageAllowedFileExtensions.Contains(ext, StringComparer.OrdinalIgnoreCase)) {
                    if ((width > 0 || height > 0) && ResizeFileToBinary(fn, ref width, ref height) is byte[] b)
                        return Controller.File(b, ct, Path.GetFileName(fn));
                    else
                        return Controller.PhysicalFile(fn, ct, Path.GetFileName(fn));
                } else if (Config.DownloadAllowedFileExtensions.Contains(ext, StringComparer.OrdinalIgnoreCase)) {
                    if (ext == "pdf" && Config.EmbedPdf && FileExists(fn)) // Embed Pdf // DN
                        return Controller.File(await FileReadAllBytes(fn), ct); // Return File Content
                    else
                        return Controller.PhysicalFile(fn, ct, Path.GetFileName(fn));
                }
            }
            return JsonBoolResult.FalseResult;
        }

        /// <summary>
        /// Output file by table name and file name
        /// </summary>
        /// <returns>Action result</returns>
        public async Task<IActionResult> GetFile(string table, string fn)
        {
            // Get parameters
            //string sessionId = Get("session");
            bool resize = Get<bool>("resize");
            int width = Get<int>("width");
            int height = Get<int>("height");
            bool download = Get("download", out StringValues d) ? ConvertToBool(d) : true; // Download by default
            if (width == 0 && height == 0 && resize) {
                width = Config.ThumbnailDefaultWidth;
                height = Config.ThumbnailDefaultHeight;
            }

            // Get table object
            string tableName = "";
            dynamic? tbl = null;
            if (!Empty(table)) {
                tbl = Resolve(table);
                tableName = tbl?.Name ?? "";
            }

            // API request with table/fn
            if (tableName != "") {
                fn = Decrypt(fn, Config.RandomKey);
            } else {
                fn = "";
            }

            // Check file
            if (FileExists(fn)) {
                Response?.Clear();
                string ext = Path.GetExtension(fn).Replace(".", "").ToLower();
                string ct = ContentType(fn);
                if (Config.ImageAllowedFileExtensions.Contains(ext, StringComparer.OrdinalIgnoreCase)) {
                    if ((width > 0 || height > 0) && ResizeFileToBinary(fn, ref width, ref height) is byte[] b)
                        return Controller.File(b, ct, Path.GetFileName(fn));
                    else
                        return Controller.PhysicalFile(fn, ct, Path.GetFileName(fn));
                } else if (Config.DownloadAllowedFileExtensions.Contains(ext, StringComparer.OrdinalIgnoreCase)) {
                    if (ext == "pdf" && Config.EmbedPdf && FileExists(fn)) // Embed Pdf // DN
                        return Controller.File(await FileReadAllBytes(fn), ct); // Return File Content
                    else
                        return Controller.PhysicalFile(fn, ct, Path.GetFileName(fn));
                }
            }
            return JsonBoolResult.FalseResult;
        }

        /// <summary>
        /// Output file by table name, field name and primary key
        /// </summary>
        /// <returns>Action result</returns>
        public async Task<IActionResult> GetFile(string table, string field, string[] keys)
        {
            // Get parameters
            //string sessionId = Get("session");
            bool resize = Get<bool>("resize");
            int width = Get<int>("width");
            int height = Get<int>("height");
            bool download = Get("download", out StringValues d) ? ConvertToBool(d) : true; // Download by default
            if (width == 0 && height == 0 && resize) {
                width = Config.ThumbnailDefaultWidth;
                height = Config.ThumbnailDefaultHeight;
            }

            // Get table object
            string tableName = "";
            dynamic? tbl = null;
            if (!Empty(table)) {
                tbl = Resolve(table);
                tableName = tbl?.Name ?? "";
            }
            if (Empty(tableName) || Empty(field) || keys.Length == 0)
                return JsonBoolResult.FalseResult;
            return await tbl?.GetFileData(field, keys, resize, width, height) ?? new EmptyResult();
        }
    }
} // End Partial class
