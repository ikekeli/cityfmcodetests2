namespace Zaharuddin.Models;

// Partial class
public partial class cityfmcodetests {
    /// <summary>
    /// Breadcrumb class
    /// </summary>
    public class Breadcrumb
    {
        public List<BreadcrumbLink> Links = new ();

        public List<BreadcrumbLink> SessionLinks = new ();

        public bool Visible = true;

        public static string CssClass = "breadcrumb float-sm-end ew-breadcrumbs";

        // Constructor
        public Breadcrumb() => Links.Add(new ("home", "HomePage", "index", "ew-home")); // Home

        // Check if an item exists
        protected bool Exists(string pageid, string table, string pageurl) => Links.Any(link => pageid == link.Id && table == link.TableVar && pageurl == link.Url);

        // Add breadcrumb
        public void Add(string pageid, string pagetitle, string pageurl, string pageurlclass = "", string table = "", bool current = false)
        {
            // Load session links
            LoadSession();

            // Get list of master tables
            var mastertable = new List<string>();
            if (!Empty(table)) {
                var tablevar = table;
                while (!Empty(Session[Config.ProjectName + "_" + tablevar + "_" + Config.TableMasterTable])) {
                    tablevar = Session.GetString(Config.ProjectName + "_" + tablevar + "_" + Config.TableMasterTable);
                    if (mastertable.Contains(tablevar))
                        break;
                    mastertable.Add(tablevar);
                }
            }

            // Add master links first
            foreach (var link in SessionLinks) {
                if (mastertable.Contains(link.TableVar) && link.Id == "list") {
                    if (link.Url == pageurl)
                        break;
                    if (!Exists(link.Id, link.TableVar, link.Url)) // DN
                        Links.Add(new (link.Id, link.Title, link.Url, link.ClassName, link.TableVar, false));
                }
            }

            // Add this link
            if (!Exists(pageid, table, pageurl))
                Links.Add(new (pageid, pagetitle, pageurl, pageurlclass, table, current));

            // Save session links
            SaveSession();
        }

        // Save links to Session
        public void SaveSession() => Session.SetValue(Config.SessionBreadcrumb, Links); // DN

        // Load links from Session
        public void LoadSession() { // DN
            if (Session[Config.SessionBreadcrumb] != null && Session.GetValue<List<BreadcrumbLink>>(Config.SessionBreadcrumb) is List<BreadcrumbLink> list)
                SessionLinks = list;
        }

        // Load language phrase
        protected string LanguagePhrase(string title, string table, bool current) {
            string wrktitle = (title == table) ? Language.TablePhrase(title, "TblCaption") : Language.Phrase(title);
            if (current)
                wrktitle = "<span id=\"ew-page-caption\">" + wrktitle + "</span>";
            return wrktitle;
        }

        // Render
        public void Render()
        {
            if (!Visible || Config.PageTitleStyle == "" || Config.PageTitleStyle == "None")
                return;
            var nav = "<ol class=\"" + CssClass + "\">";
            if (IsList(Links)) {
                var cnt = Links.Count;
                if (Config.PageTitleStyle == "Caption") {
                    //Write("<div class=\"ew-page-title\">" + LanguagePhrase(Links[cnt - 1].Title, Links[cnt - 1].TableVar, Links[cnt - 1].IsCurrent) + "</div>");
                    return;
                } else {
                    for (var i = 0; i < cnt; i++) {
                        var link = Links[i];
                        var url = link.Url;
                        if (i < cnt - 1) {
                            nav += "<li class=\"breadcrumb-item\" id=\"ew-breadcrumb" + (i + 1) + "\">";
                        } else {
                            nav += "<li class=\"breadcrumb-item active\" id=\"ew-breadcrumb" + (i + 1) + "\" class=\"active\">";
                            url = ""; // No need to show URL for current page
                        }
                        var text = LanguagePhrase(link.Title, link.TableVar, link.IsCurrent);
                        var title = LanguagePhrase(link.Title, link.TableVar, false);
                        if (!Empty(url)) {
                            nav += "<a href=\"" + GetUrl(url) + "\""; // Output the URL as is // DN
                            if (!Empty(title) && title != text)
                                nav += " title=\"" + HtmlEncode(title) + "\"";
                            if (link.ClassName != "")
                                nav += " class=\"" + link.ClassName + "\"";
                            nav += ">" + text + "</a>";
                        } else {
                            nav += text;
                        }
                        nav += "</li>";
                    }
                }
            }
            nav += "</ol>";
            Write(nav);
        }
    }
} // End Partial class
