namespace Zaharuddin.Models;

// Partial class
public partial class cityfmcodetests {
    /// <summary>
    /// Global events
    /// </summary>

    // ContentType Mapping event
    public static void ContentTypeMapping(IDictionary<string, string> mappings) {
        // Example:
        //mappings[".image"] = "image/png"; // Add new mappings
        //mappings[".rtf"] = "application/x-msdownload"; // Replace an existing mapping
        //mappings.Remove(".mp4"); // Remove MP4 videos
    }

    // Class Init event
    public static void ClassInit() {
        // Enter your code here
    }

    // Page Loading event
    public static void PageLoading() {
        // Enter your code here
    }

    // Page Rendering event
    public static void PageRendering() {
        //Log("Page Rendering");
    }

    // Page Unloaded event
    public static void PageUnloaded() {
        // Enter your code here
    }

    // Personal Data Downloading event
    public static void PersonalDataDownloading(Dictionary<string, object> row) {
        //Log("PersonalData Downloading");
    }

    // Personal Data Deleted event
    public static void PersonalDataDeleted(Dictionary<string, object> row) {
        //Log("PersonalData Deleted");
    }

    // AuditTrail Inserting event
    public static bool AuditTrailInserting(Dictionary<string, object> rsnew) {
        return true;
    }

    // Chart Rendered event
    public static void ChartRendered(ChartJsRenderer renderer) {
        // Example:
        //var data = renderer.Data;
        //var options = renderer.Options;
        //DbChart chart = renderer.Chart;
        //if (chart.ID == "<Report>_<Chart>") { // Check chart ID
        //}
    }

    // One Time Password Sending event
    public static bool OtpSending(string user, dynamic client) {
        // Example:
        // Log(user, client); // View user and client (Email or Sms object)
        // if (SameText(Config.TwoFactorAuthenticationType, "email")) { // Possible values: "email" or "sms"
        //     client.Content = ...; // Change content
        //     client.Recipient = ...; // Change recipient
        //     // return false; // Return false to cancel
        // }
        return true;
    }

    // Routes Add event
    public static void RouteAction(IEndpointRouteBuilder app) {
        // Example:
        // app.MapGet("/", () => "Hello World!");
    }

    // Services Add event
    public static void ServiceAdd(IServiceCollection services) {
        // Example:
        // services.AddSignalR();
    }

    // Container Build event
    public static void ContainerBuild(ContainerBuilder builder) {
        // Enter your code here
    }

    // App Build event
    public static void AppBuild(WebApplicationBuilder builder) {
        // Example:
        // builder.Configuration.AddAzureKeyVault(...);
    }

    // Global user code
} // End Partial class
