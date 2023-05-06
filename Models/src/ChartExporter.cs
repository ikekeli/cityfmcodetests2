namespace Zaharuddin.Models;

// Partial class
public partial class cityfmcodetests {
    /// <summary>
    /// Chart exporter class
    /// </summary>
    public class ChartExporter
    {
        // Constructor
        public ChartExporter(Controller controller) { // DN
            Controller = controller;
        }

        // Constructor
        public ChartExporter() { // DN
        }

        // Run
        public async Task<IActionResult> Export()
        {
            string json = Post<string>("charts") ?? "[]";
            var charts = JsonConvert.DeserializeObject<List<Dictionary<string, string>>>(json);
            var files = new List<string>();
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
                        return ServerError(Language.Phrase("ChartExportError1").Replace("%t", streamType).Replace("%e", chartEngine));
                    string filename = chart["fileName"] ?? "";
                    if (Empty(filename))
                        return ServerError(Language.Phrase("ChartExportError2"));
                    var path = ServerMapPath(Config.UploadDestPath);
                    if (!DirectoryExists(path) && !CreateFolder(path))
                        return ServerError(Language.Phrase("ChartExportError3"));
                    if (await SaveFile(path, filename, img))
                        files.Add(filename);
                    else
                        return ServerError(Language.Phrase("ChartExportError4"));
                }
            }
            return Controller.Json(new { success = true, files = files });
        }

        // Send server error
        protected IActionResult ServerError(string msg) => Controller.Json(new { success = false, error = msg });

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
            MagickReadSettings settings = new ();
            settings.ColorSpace = ColorSpace.RGB;
            settings.Format = MagickFormat.Svg;
            using var image = new MagickImage(Encoding.UTF8.GetBytes(svgdata), settings);
            image.Format = MagickFormat.Png;
            return image.ToByteArray();
        }
    }
} // End Partial class
