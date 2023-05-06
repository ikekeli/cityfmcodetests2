namespace Zaharuddin.Models;

// Partial class
public partial class cityfmcodetests {
    /// <summary>
    /// Abstract base class for export
    /// </summary>
    public abstract class AbstractExportBase
    {
        protected string? FileId = null; // File ID for saving to folder

        public dynamic? Table = null; // Table/Page object

        public StringBuilder Text = new (); // Text or HTML to be exported

        public virtual string ContentType { get; set; } = ""; // Content type

        public virtual bool UseCharset { get; set; } = false; // Add charset to content type

        public virtual bool UseBom { get; set; } = false; // Output byte order mark

        public virtual string CacheControl { get; set; } = "no-store, no-cache"; // Cache control

        public virtual string FileName { get; set; } = ""; // User specified file name

        public virtual string FileExtension { get; set; } = ""; // File extension without "."

        public virtual string Disposition { get; set; } = "attachment"; // Disposition for Content-Disposition header or email attachment

        public virtual bool? Download { get; set; } = null;

        // Constructor
        public AbstractExportBase(object? tbl = null)
        {
            Table = tbl;
            UseCharset = Query.ContainsKey(Config.ApiExportUseCharset) ? Get<bool>(Config.ApiExportUseCharset): UseCharset;
            UseBom = Query.ContainsKey(Config.ApiExportUseBom) ? Get<bool>(Config.ApiExportUseBom): UseBom;
            CacheControl = Query.ContainsKey(Config.ApiExportCacheControl) ? Get(Config.ApiExportCacheControl): CacheControl;
            Disposition = Query.ContainsKey(Config.ApiExportDisposition) ? Get(Config.ApiExportDisposition) : Disposition;
            Download = Query.ContainsKey(Config.ApiExportDownload) ? Get<bool>(Config.ApiExportDownload): Download; // Override $this->Disposition if not null
            ContentType = Query.ContainsKey(Config.ApiExportContentType) ? Get(Config.ApiExportContentType) : ContentType;
            if (Empty(ContentType) && !Empty(FileExtension))
                ContentType = ContentType("export." + FileExtension);
        }

        /// <summary>
        /// Get table
        /// </summary>
        /// <returns>Table object</returns>
        public dynamic? GetTable()
        {
            return Table;
        }

        /// <summary>
        /// Set table
        /// </summary>
        /// <param name="value">Table object</param>
        public void SetTable(dynamic value)
        {
            Table = value;
        }

        /// <summary>
        /// Get file ID (GUID)
        /// </summary>
        /// <returns>File ID</returns>
        public string GetFileId()
        {
            FileId ??= Guid.NewGuid().ToString();
            return FileId;
        }

        /// <summary>
        /// Get save file name ({guid}.{ext})
        /// </summary>
        /// <returns>Save file name</returns>
        public string GetSaveFileName()
        {
            return FixFileName(GetFileId());
        }

        /// <summary>
        /// Get Content-Type
        /// </summary>
        /// <returns>Content type</returns>
        public System.Net.Mime.ContentType GetContentType() // DN
        {
            System.Net.Mime.ContentType header = new (ContentType);
            if (UseCharset && !Empty(Config.Charset))
                header.CharSet = Config.Charset;
            return header;
        }

        /// <summary>
        /// Get Content-Disposition
        /// </summary>
        /// <param name="fileName">File name</param>
        /// <returns>Return Content-Disposition</returns>
        public System.Net.Mime.ContentDisposition GetContentDisposition(string fileName = "") // DN
        {
            string disposition = GetDisposition();
            System.Net.Mime.ContentDisposition header = new (disposition);
            if (disposition == "attachment" && !Empty(fileName))
                header.FileName = fileName;
            return header;
        }

        /// <summary>
        /// Get Cache-Control
        /// </summary>
        /// <returns>Return Cache-Control</returns>
        public string GetCacheControl() // DN
        {
            return CacheControl;
        }

        /// <summary>
        /// BOM
        /// </summary>
        public string Bom()
        {
            return UseBom && SameText(Config.Charset, "utf-8") ? Encoding.UTF8.GetString(Encoding.UTF8.GetPreamble()) : ""; // DN
        }

        /// <summary>
        /// Clean (erase) the output buffer and turn off output buffering
        /// </summary>
        public void CleanBuffer()
        {
            Response?.Clear();
        }

        /// <summary>
        /// Get disposition
        /// </summary>
        /// <returns>Content-Disposition</returns>
        public string GetDisposition()
        {
            if (Download != null)
                return Download.Value ? "attachment" : "inline";
            string value = Disposition.ToLowerInvariant();
            if ((new[] {"inline", "attachment"}).Contains(value))
                return value;
            return "attachment";
        }

        /// <summary>
        /// Fix file name
        /// </summary>
        /// <param name="fileName">File name</param>
        /// <returns>Return file name (as yyyyMMddhhmmssff.ext if empty) with extension</returns>
        public string FixFileName(string fileName)
        {
            if (Empty(fileName))
                fileName = (Table != null ? Table.TableVar + "_" : "") + DateTime.Now.ToString("yyyyMMddhhmmssfff"); // Temporary file name
            fileName += SameText(Path.GetExtension(fileName), "." + FileExtension) ? "" : "." + FileExtension;
            return fileName;
        }

        /// <summary>
        /// Clean output buffer, write headers and BOM before export
        /// </summary>
        /// <param name="fileName">File name</param>
        public void WriteHeaders(string fileName = "")
        {
            CleanBuffer();
            AddHeader(HeaderNames.ContentType, GetContentType().ToString());
            AddHeader(HeaderNames.ContentDisposition, GetContentDisposition(!Empty(FileName) ? FileName : fileName).ToString()); // Use FileName specified by user first
            AddHeader(HeaderNames.CacheControl, GetCacheControl());
        }

        /// <summary>
        /// Import data from table/page object
        /// </summary>
        public void Import()
        {
            Table?.Invoke("ExportData", new object?[] { this });
        }

        /// <summary>
        /// Export
        /// </summary>
        /// <param name="fileName">Output file name</param>
        /// <param name="output">Whether output to browser</param>
        /// <param name="save">Whether save to folder</param>
        abstract public Task<IActionResult> Export(string fileName = "", bool output = true, bool save = false);
    }
} // End Partial class
