namespace Zaharuddin.Models;

// Partial class
public partial class cityfmcodetests {
    /// <summary>
    /// Export Handler class
    /// </summary>
    public class ExportHandler
    {
        // Constructor
        public ExportHandler(Controller controller) { // DN
            Controller = controller;
        }

        // Constructor
        public ExportHandler() { // DN
        }

        // Export data
        public async Task<IActionResult> ExportData(string exportType, string tableName, string[]? recordKey = null)
        {
            bool save = Get<bool>(Config.ApiExportSave) && !Empty(Config.ExportLogTableVar);
            bool output = (Query.TryGetValue(Config.ApiExportOutput, out StringValues sv) ? ConvertToBool(sv) : true) || !save; // Output by default unless output=0 and not save

            // Set up id for temp folder
            ExportId = Random(8);

            // Get table/page class
            dynamic? tbl = Resolve(tableName);
            if (tbl == null) // Check if valid table
                return Controller.Json(new { success = false, error = Language.Phrase("InvalidParameter") + ": table=" + tableName, version = Config.ProductVersion });

            // Get record key from query string or form data
            recordKey = !Empty(recordKey) ? recordKey : Get(Config.ApiKeyName).Split(Config.CompositeKeySeparator);
            bool isList = Empty(recordKey);
            if (isList && tbl.Type != "REPORT") { // List/View page
                var recordKeys = tbl.GetRecordKeys();
                recordKey = recordKeys.Count > 0
                    ? (recordKeys[0] is string[]) ? recordKeys[0] : new string[] { recordKeys[0] }
                    : new string[0];
            }

            // Validate export type
            string? exportClassName = null;
            if (tbl.Type == "REPORT") {
                if (Config.ExportReport.TryGetValue(exportType, out string? reportClassName))
                    exportClassName = reportClassName;
            } else if (Config.Export.TryGetValue(exportType, out string? tableClassName))
                exportClassName = tableClassName;
            if (exportClassName == null)
                return Controller.Json(new { success = false, error = Language.Phrase("InvalidParameter") + ": type=" + exportType, version = Config.ProductVersion });

            // Export data
            string keyValue = isList ? String.Empty : String.Join('_', recordKey!);
            string fileName = Query.TryGetValue(Config.ApiExportFileName, out StringValues fn) ? fn.ToString() : tbl.TableVar + (isList ? String.Empty : "_" + keyValue);
            string pageName = tbl.GetApiPageName(isList ? Config.ApiListAction : Config.ApiViewAction);
            bool isReport = tbl.Type == "REPORT";
            string reportType = tbl.TableReportType;
            string pageClassName = isReport && (new [] { "summary", "crosstab" }).Contains(reportType) ? pageName + TitleCase(reportType) : pageName; // Add Report type to page class name // DN
            bool custom = isReport;
            dynamic? page = Resolve(pageClassName);
            object result;
            if (page != null) {
                CurrentPage = page; // Set up current page
                page.Export = exportType;

                // Create export object
                dynamic doc = CreateInstance(exportClassName, new object?[] { tbl })!;
                if (!isReport)
                    doc.SetHorizontal(isList);

                // File ID
                string fileId = "";
                fileName = doc.FixFileName(fileName);

                // Make sure export folder exists
                if (save)
                    CreateFolder(ExportPath(true));

                // Export charts
                object chartResult = await ExportCharts(tbl, exportType);
                Dictionary<string, string> files = new();
                if (chartResult is string error)
                    return Controller.Json(new { success = false, error = error, version = Config.ProductVersion });
                else if (chartResult is Dictionary<string, string> dict)
                    files = dict;

                // Handle custom template (post back)
                if (Post("data", out StringValues ct)) {
                    string content = HtmlDecode(ct);
                    string html = await ReplaceCharts(content, files, exportType);
                    doc.LoadHtml(html);
                } else {
                    await page.Run();
                    if (custom) { // Custom export / Report
                        if (!page.IsTerminated) {
                            string html = await GetViewOutput(pageName, page);
                            html = await ReplaceCharts(html, files, exportType);
                            doc.LoadHtml(html);
                            page.Terminate();
                        }
                    } else { // Table export
                        if (isList) { // List page
                            // Add top/left charts
                            foreach (var file in files) {
                                var chart = tbl.ChartByID(file.Key);
                                if (chart != null && chart?.Position <= 2)
                                    doc.AddImage(file.Value, "after");
                            }
                            // Export
                            await page.ExportData(doc);
                            // Add right/bottom charts
                            foreach (var file in files) {
                                var chart = tbl.ChartByID(file.Key);
                                if (chart != null && chart?.Position > 2)
                                    doc.AddImage(file.Value, "before");
                            }
                        } else { // View page
                            await page.ExportData(doc, recordKey);
                        }
                    }
                }

                // Export
                result = await doc.Export(fileName, output, save);
                if (exportType == "email") { // Return email result
                    //result = result;
                } else if (save) {
                    // Get file ID
                    fileId = doc.GetFileId();
                    // Write export log for saved file
                    await WriteExportLog(fileId, DbCurrentDateTime(), CurrentUser(), exportType, tableName, keyValue, fileName, Request?.QueryString.ToString() ?? "");
                    // Return file ID if export file not returned
                    if (!output)
                        result = new { success = true, fileId = fileId, version = Config.ProductVersion };
                }
            } else {
                result = new { success = false, error = Language.Phrase("InvalidParameter") + ": table = " + tableName + ", export type = " + exportType };
            }

            // Clean up export files
            if (Config.ExportFilesExpiryTime > 0) {
                CleanPath(ExportPath(true), false, file => {
                    if (File.Exists(file)) {
                        var lastmdtime = new FileInfo(file).LastWriteTime;
                        if (((TimeSpan)(DateTime.Now - lastmdtime)).TotalMinutes > Config.ExportFilesExpiryTime)
                        {
                            FileDelete(file);
                            Log("export file '" + file + "' deleted");
                        }
                    }
                });
            }

            // Delete temp images
            CleanPath(UploadTempPath(), true);
            if (result is IActionResult actionResult) // DN
                return actionResult;
            else
                return Controller.Json(result);
        }

        // Export charts
        public async Task<object> ExportCharts(dynamic tbl, string exportType)
        {
            string json = Post<string>("charts") ?? "[]";
            var charts = JsonConvert.DeserializeObject<List<Dictionary<string, string>>>(json);
            Dictionary<string, string> files = new ();
            if (charts != null) {
                foreach (var chart in charts) {
                    byte[]? img = null;
                    string streamType = chart["streamType"];
                    string streamData = chart["stream"];
                    string chartEngine = chart["chartEngine"];

                    // Google Charts base64
                    if (streamType == "base64") {
                        if (!Empty(streamData)) {
                            streamData = Regex.Replace(streamData, @"^data:image/\w+;base64,", "");
                            img = Convert.FromBase64String(streamData);
                        }
                    } else { // SVG
                        img ??= GetImageFromImagick(chart); // Get from Imagick
                    }
                    if (img == null)
                        return Language.Phrase("ChartExportError1").Replace("%t", streamType).Replace("%e", chartEngine);

                    // Save the file
                    string filename = chart["fileName"] ?? "";
                    if (Empty(filename))
                        return Language.Phrase("ChartExportError2");
                    var path = UploadTempPath();
                    if (!DirectoryExists(path) && !CreateFolder(path))
                        return Language.Phrase("ChartExportError3");
                    string filePath = IncludeTrailingDelimiter(path, true) + filename;
                    string id = Regex.Replace(Path.GetFileNameWithoutExtension(filename), @"^chart_", "");
                    await ResizeAndSaveChart(tbl, img, exportType, path, filename);
                    files.Add(id, filePath);
                }
            }
            return files;
        }

        // Resize and save chart image
        protected async Task<bool> ResizeAndSaveChart(dynamic tbl, byte[] img, string exportType, string path, string fileName)
        {
            bool exportPdf = exportType == "pdf";
            bool exportWord = exportType == "word" && Config.UseWordExtension;
            bool exportExcel = exportType == "excel" && Config.UseExcelExtension;
            Dictionary<string, int> dimension = ChartDimension(tbl, img, exportType);
            int width = dimension["width"];
            int height = dimension["height"];
            if ((exportPdf || exportWord || exportExcel) && dimension["width"] > 0 && dimension["height"] > 0)
                ResizeBinary(ref img, ref width, ref height, null, false); // Keep aspect ratio for chart
            await SaveFile(path, fileName, img);
            return true;
        }

        // Get chart export width and height
        protected Dictionary<string, int> ChartDimension(dynamic tbl, byte[] data, string exportType)
        {
            bool portrait = SameText(tbl?.ExportPageOrientation, "portrait");
            bool exportPdf = exportType == "pdf";
            bool exportWord = exportType == "word" && Config.UseWordExtension;
            bool exportExcel = exportType == "excel" && Config.UseExcelExtension;
            int width = 0;
            int height = 0;
            int maxWidth = 0;
            int maxHeight = 0;
            if (exportPdf) {
                maxWidth = portrait ? Config.PdfMaxImageWidth : Config.PdfMaxImageHeight;
                maxHeight = portrait ? Config.PdfMaxImageHeight : Config.PdfMaxImageWidth;
            } else if (exportWord) {
                maxWidth = portrait ? Config.WordMaxImageWidth : Config.WordMaxImageHeight;
                maxHeight = portrait ? Config.WordMaxImageHeight : Config.WordMaxImageWidth;
            } else if (exportExcel) {
                maxWidth = portrait ? Config.ExcelMaxImageWidth : Config.ExcelMaxImageHeight;
                maxHeight = portrait ? Config.ExcelMaxImageHeight : Config.ExcelMaxImageWidth;
            }
            if (exportPdf || exportWord || exportExcel) {
                try {
                    MagickImage img = new(data);
                    width = img.Width > 0 ? Math.Min(img.Width, maxWidth) : maxWidth;
                    height = img.Height > 0 ? Math.Min(img.Height, maxHeight) : maxHeight;
                } catch { }
            }
            return new Dictionary<string, int> {
                { "width", width },
                { "height", height }
            };
        }

        // Get image from Imagick
        protected byte[]? GetImageFromImagick(Dictionary<string, string> chart)
        {
            string svgdata = chart["stream"];
            if (Empty(svgdata))
                return null;
            svgdata = svgdata.Replace("+", " "); // Replace + to ' '
            // IMPORTANT NOTE: Magick.NET does not support SVG syntax: fill="url('#id')". Need to replace the attributes:
            // - fill="url('#id')" style="fill-opacity: n1; ..."
            // to:
            // - fill="color" style="fill-opacity: n2; ..."
            // from xml below:
            // <linearGradient ... id="id">
            // <stop stop-opacity="0.5" stop-color="#ff0000" offset="0%"></stop>
            // ...</linearGradient>
            XmlDocument doc = new ();
            doc.LoadXml(svgdata);
            var nodes = doc.SelectNodes("//*[@fill]");
            if (nodes != null) {
                foreach (XmlElement node in nodes) {
                    string fill = node.GetAttribute("fill");
                    string style = node.GetAttribute("style");
                    if (fill.StartsWith("url(") && fill.Substring(5, 1) == "#") {
                        var id = fill.Substring(6, fill.Length - 8);
                        var nsmgr = new XmlNamespaceManager(doc.NameTable);
                        nsmgr.AddNamespace("ns", "http://www.w3.org/2000/svg");
                        if (doc.SelectSingleNode("//*/ns:linearGradient[@id='" + id + "']", nsmgr) is XmlElement gnode &&
                            gnode.SelectSingleNode("ns:stop[@offset='0%']", nsmgr) is XmlElement snode) {
                            var fillcolor = snode.GetAttribute("stop-color");
                            var fillopacity = snode.GetAttribute("stop-opacity");
                            if (!Empty(fillcolor))
                                node.SetAttribute("fill", fillcolor);
                            if (!Empty(fillopacity) && !Empty(style)) {
                                style = Regex.Replace(style, @"fill-opacity:\s*\S*;", "fill-opacity: " + fillopacity + ";");
                                node.SetAttribute("style", style);
                            }
                        }
                    }
                }
            }
            svgdata = doc.DocumentElement?.OuterXml ?? "";
            MagickNET.SetLogEvents(LogEvents.All);
            MagickReadSettings settings = new();
            settings.ColorSpace = ColorSpace.RGB;
            settings.Format = MagickFormat.Svg;
            using var image = new MagickImage(Encoding.UTF8.GetBytes(svgdata), settings);
            image.Format = MagickFormat.Png;
            return image.ToByteArray();
        }

        // Replace charts in custom template
        protected async Task<string> ReplaceCharts(string text, Dictionary<string, string> files, string exportType)
        {
            var doc = new AngleSharp.Html.Parser.HtmlParser().ParseDocument(text);
            foreach (var chart in doc.QuerySelectorAll(".ew-chart")) {
                string id = Regex.Replace(chart.GetAttribute("id") ?? "", @"^div_cht_", "");
                if (files.TryGetValue(id, out string? filePath)) {
                    var div = doc.CreateElement("div");
                    div.SetAttribute("class", chart.GetAttribute("class") ?? ""); // Copy classes, e.g. "ew-chart break-before-page"
                    var img = doc.CreateElement("img");
                    byte[] data = await FileReadAllBytes(filePath);
                    img.SetAttribute("src", ImageToBase64Url(data));
                    try {
                        MagickImage image = new(data);
                        if (image.Width > 0)
                            img.SetAttribute("width", Convert.ToString(image.Width));
                        if (image.Height > 0)
                            img.SetAttribute("height", Convert.ToString(image.Height));
                    } catch { }
                    div.AppendChild(img);
                    chart.Replace(div);
                }
            }
            return doc.DocumentElement.OuterHtml;
        }

        // Search export log
        public async Task<IActionResult> Search(string search)
        {
            if (search == Config.ExportLogSearch) {
                bool output = Query.ContainsKey(Config.ApiExportOutput) ? Get<bool>(Config.ApiExportOutput) : true; // Output by default unless output=0
                return await SearchExportLog(output);
            } else {
                string? fileName = Query.ContainsKey(Config.ApiExportFileName) ? Get(Config.ApiExportFileName) : null;
                return await WriteExportFile(search, fileName);
            }
        }

        // Search export log
        protected async Task<IActionResult> SearchExportLog(bool output)
        {
            List<string> zipNames = new () { Config.ExportLogArchivePrefix };
            string exportLogTable = Config.ExportLogTableVar;
            if (Empty(exportLogTable))
                return new EmptyResult();
            dynamic? tbl = Resolve(exportLogTable);
            if (tbl == null)
                return new EmptyResult();
            string filter = tbl.ApplyUserIDFilters("");
            // Handle export type
            DbField fld = tbl.Fields[Config.ExportLogFieldNameExportType];
            fld.AdvancedSearch.ParseSearchValue(Get(Config.ExportLogFieldNameExportTypeAlias));
            string exportType = fld.AdvancedSearch.SearchValue;
            if (!Empty(exportType)) {
                zipNames.Add(exportType);
                string opr = !Empty(fld.AdvancedSearch.SearchOperator) ? fld.AdvancedSearch.SearchOperator : "=";
                string wrk = GetSearchSql(fld, exportType, opr, fld.AdvancedSearch.SearchCondition, fld.AdvancedSearch.SearchValue2, fld.AdvancedSearch.SearchOperator2, Config.ExportLogDbId);
                AddFilter(ref filter, wrk);
            }
            // Handle tablename
            fld = tbl.Fields[Config.ExportLogFieldNameTable];
            fld.AdvancedSearch.ParseSearchValue(Get(Config.ExportLogFieldNameTableAlias));
            string tableName = fld.AdvancedSearch.SearchValue;
            if (!Empty(tableName)) {
                zipNames.Add(tableName);
                string opr = !Empty(fld.AdvancedSearch.SearchOperator) ? fld.AdvancedSearch.SearchOperator : "LIKE";
                string wrk = GetSearchSql(fld, tableName, opr, fld.AdvancedSearch.SearchCondition, fld.AdvancedSearch.SearchValue2, fld.AdvancedSearch.SearchOperator2, Config.ExportLogDbId);
                AddFilter(ref filter, wrk);
            }
            // Handle filename
            fld = tbl.Fields[Config.ExportLogFieldNameFileName];
            fld.AdvancedSearch.ParseSearchValue(Get(Config.ExportLogFieldNameFileNameAlias));
            string fileName = fld.AdvancedSearch.SearchValue;
            if (!Empty(fileName)) {
                zipNames.Add(fileName);
                string opr = !Empty(fld.AdvancedSearch.SearchOperator) ? fld.AdvancedSearch.SearchOperator : "LIKE";
                string wrk = GetSearchSql(fld, fileName, opr, fld.AdvancedSearch.SearchCondition, fld.AdvancedSearch.SearchValue2, fld.AdvancedSearch.SearchOperator2, Config.ExportLogDbId);
                AddFilter(ref filter, wrk);
            }
            // Handle datetime
            fld = tbl.Fields[Config.ExportLogFieldNameDateTime];
            if (!fld.AdvancedSearch.Get())
                fld.AdvancedSearch.ParseSearchValue(Get(Config.ExportLogFieldNameDateTimeAlias));
            string dt = fld.AdvancedSearch.SearchValue;
            if (!CheckDate(dt))
                return Controller.Json(new { success = false, error = Language.Phrase("IncorrectDate").Replace("%s", dt) + ": " + Config.ExportLogFieldNameDateTimeAlias, version = Config.ProductVersion });
            if (!Empty(dt)) {
                dt = UnformatDateTime(dt, "1");
                zipNames.Add(dt);
                string opr = !Empty(fld.AdvancedSearch.SearchOperator) ? fld.AdvancedSearch.SearchOperator : "=";
                string wrk = opr == "="
                    ? GetDateFilterSql(fld.Expression, opr, dt, fld.DataType, Config.ExportLogDbId)
                    : GetSearchSql(fld, dt, opr, fld.AdvancedSearch.SearchCondition, fld.AdvancedSearch.SearchValue2, fld.AdvancedSearch.SearchOperator2, Config.ExportLogDbId);
                AddFilter(ref filter, wrk);
            }
            // Validate limit
            string limitValue = Get(Config.ExportLogLimit);
            if (!string.IsNullOrEmpty(limitValue) && !IsNumeric(limitValue))
                return Controller.Json(new { success = false, error = Language.Phrase("IncorrectInteger") + ": " + Config.ExportLogLimit, version = Config.ProductVersion });
            List<Dictionary<string, object>> rows = new();
            // Handle limit
            if (Int32.TryParse(limitValue, out int limit) && limit > 0) {
                zipNames.Add(ConvertToString(limit));
                DbField dateTimeField = tbl.Fields[Config.ExportLogFieldNameDateTime];
                string sql = tbl.GetSql(filter, dateTimeField.Expression + " DESC");
                rows = await tbl.Connection.GetRowsAsync(await tbl.Connection.SelectLimit(sql, limit, -1, true));
            } else if (!Empty(filter)) {
                rows = await tbl.Connection.GetRowsAsync(tbl.GetSql(filter));
            } else {
                return new EmptyResult();
            }
            List<string> fileIds = rows.Select(row => ConvertToString(row[Config.ExportLogFieldNameFileId])).ToList();
            if (output && fileIds.Count >= 1) {
                if (fileIds.Count == 1) { // Single file, just output
                    return await WriteExportFile(fileIds[0]);
                } else { // More than one file, zip for output
                    try {
                        string zipFile = ExportPath(true) + String.Join('_', zipNames) + ".zip";
                        using (var archive = ZipFile.Open(zipFile, ZipArchiveMode.Create)) {
                            foreach (string fileId in fileIds) {
                                Dictionary<string, string>? file = await GetExportFileByGuid(fileId);
                                if (file != null) {
                                    string filePath = file["filePath"];
                                    if (File.Exists(filePath))
                                        archive.CreateEntryFromFile(filePath, Path.GetFileName(filePath), System.IO.Compression.CompressionLevel.Optimal);
                                }
                            }
                        }
                        string contentType = "application/zip";
                        AddHeader(HeaderNames.ContentType, contentType);
                        AddHeader(HeaderNames.ContentDisposition, "attachment; filename=\"" + Path.GetFileName(zipFile) + "\"");
                        byte[] data = await FileReadAllBytes(zipFile);
                        File.Delete(zipFile);
                        return Controller.File(data, contentType);
                    } catch (Exception) {
                        if (Config.Debug)
                            throw;
                    }
                }
            }
            return Controller.Json(new Dictionary<string, object>()
            {
                { "success", true },
                { Config.ExportLogFieldNameFileId, fileIds },
                { "version", Config.ProductVersion }
            });
        }

        // Write export file
        protected async Task<IActionResult> WriteExportFile(string? guid, string? fileName = null)
        {
            Dictionary<string, string>? file = await GetExportFileByGuid(guid, fileName);
            if (file != null) {
                fileName = file["fileName"];
                string filePath = file["filePath"];
                if (File.Exists(filePath)) {
                    string contentType = ContentType(filePath);
                    byte[] data = await FileReadAllBytes(filePath);
                    string ext = Path.GetExtension(filePath).Replace(".", "").ToLower();
                    return Controller.File(data, contentType, fileName);
                }
            }
            return new EmptyResult();
        }

        // Get export file
        protected async Task<Dictionary<string, string>?> GetExportFileByGuid(string? guid, string? fileName = null)
        {
            string exportLogTable = Config.ExportLogTableVar;
            if (Empty(exportLogTable) || !CheckGuid(guid))
                return null;
            dynamic? tbl = Resolve(exportLogTable);
                if (tbl == null)
                    return null;
            DbField fileIdField = tbl.Fields[Config.ExportLogFieldNameFileId];
            string filter = fileIdField.Expression + " = " + QuotedValue(guid, DataType.Guid, Config.ExportLogDbId);
            var row = await tbl.Connection.GetRowAsync(tbl.GetSql(filter));
            if (row != null) {
                fileName ??= row[Config.ExportLogFieldNameFileName]; // Get file name
                return new Dictionary<string, string> {
                    { "fileName", fileName },
                    { "filePath", ExportPath(true) + guid + Path.GetExtension(fileName) }
                };
            }
            return null;
        }
    }
} // End Partial class
