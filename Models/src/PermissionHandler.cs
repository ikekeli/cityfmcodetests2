namespace Zaharuddin.Models;

// Partial class
public partial class cityfmcodetests {
    /// <summary>
    /// User level permisson handler
    /// </summary>
    public class PermissionHandler : AuthorizationHandler<PermissionRequirement>
    {
        protected string LoginUrl = "";

        public PermissionHandler()
        {
            LoginUrl = GetPathByName("login") ?? "";
        }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
        {
            Language = ResolveLanguage();
            Profile = ResolveProfile();
            var security = ResolveSecurity();
            if (IsApi()) { // API
                if (!ValidApiRequest())
                    return Task.CompletedTask;
                var routeValues = Request?.RouteValues;
                if (routeValues?.TryGetValue("controller", out object? name) ?? false) { // Note: Check "controller", not "action"
                    string pageAction = ConvertToString(name).ToLower();
                    if (Config.ApiPageActions.Contains(pageAction, StringComparer.OrdinalIgnoreCase)) {
                        context.Succeed(requirement);
                    } else if (pageAction == Config.ApiLookupAction) { // Lookup
                        if (SameText(Request?.ContentType, "application/json")) { // Batch lookup, to be checked in controller
                            context.Succeed(requirement);
                        } else {
                            string pageName = Param(Config.ApiLookupPage); // Get lookup page
                            var t = Type.GetType(Config.ProjectClassName + "+" + pageName);
                            if (t != null) {
                                var page = Resolve(pageName);
                                if (page != null) {
                                    string fieldName = Post(Config.ApiFieldName); // Get field name
                                    var tbl = page.FieldByName(fieldName)?.Lookup?.GetTable();
                                    if (tbl != null) {
                                        context.Succeed(requirement);
                                    }
                                }
                            }
                        }
                    } else if (pageAction == Config.ApiPushNotificationAction) { // Push notification
                        string action = ConvertToString(routeValues["action"]);
                        if (SameText(action, Config.ApiPushNotificationSubscribe) || SameText(action, Config.ApiPushNotificationDelete)) {
                            if (Config.PushAnonymous || security.IsLoggedIn)
                                context.Succeed(requirement);
                        } else if (SameText(action, Config.ApiPushNotificationSend)) {
                            security.LoadTablePermissions(Config.SubscriptionTableVar);
                            if (security.CanPush)
                                context.Succeed(requirement);
                        }
                    } else if (pageAction == "_" + Config.Api2FaAction) { // Two factor authentication // controller is prefixed with "_" // DN
                        string action = ConvertToString(routeValues["name"]);
                        if (action == Config.Api2FaShow) {
                            context.Succeed(requirement);
                        } else if (action == Config.Api2FaVerify) {
                            if (Config.ForceTwoFactorAuthentication && security.IsLoggingIn2FA || security.IsLoggedIn)
                                context.Succeed(requirement);
                        } else if (action == Config.Api2FaReset) {
                            if (security.IsLoggedIn)
                                context.Succeed(requirement);
                        } else if (action == Config.Api2FaBackupCodes || action == Config.Api2FaNewBackupCodes) {
                            if (security.IsLoggedIn && !security.IsSysAdmin)
                                context.Succeed(requirement);
                        } else if (action == Config.Api2FaSendOtp) {
                            if ((security.IsLoggingIn2FA || security.IsLoggedIn) && !security.IsSysAdmin)
                                context.Succeed(requirement);
                        }
                    } else if (pageAction == Config.ApiUploadAction) { // Upload
                        context.Succeed(requirement);
                    } else if (pageAction == Config.ApiPermissionsAction) { // Permission
                        if (IsGet() || IsPost() && security.IsAdmin)
                            context.Succeed(requirement);
                    } else if (pageAction == Config.ApiJqueryUploadAction ||
                        pageAction == Config.ApiSessionAction ||
                        pageAction == Config.ApiExportChartAction) {
                        context.Succeed(requirement); // To be validated by [AutoValidateAntiforgeryToken]
                    }
                }
                return Task.CompletedTask;
            } else { // Non API
                string[] ar = new string[] {};
                if (!Empty(RouteName))
                    ar = RouteName.Split('-');
                string currentPageName = ar.Length >= 1 ? ar[0] : ""; // Get current page name
                string table = (ar.Length > 2) ? ar[1] : "";
                string pageAction = (ar.Length > 2) ? ar[2] : currentPageName;
                string redirectUrl = "";

                // Succeed
                if (Empty(redirectUrl)) {
                    context.Succeed(requirement);
                } else {
                    Response?.OnStarting(async () => {
                        if (Get<bool>("modal")) { // Modal page
                            if (redirectUrl == LoginUrl && Config.UseModalLogin) { // Redirect to login
                                Response.Redirect(redirectUrl);
                            } else {
                                Response.StatusCode = 200;
                                await Response.WriteAsJsonAsync(new { url = redirectUrl });
                            }
                        } else {
                            if (redirectUrl != LoginUrl)
                                Response.Redirect(redirectUrl);
                        }
                    });
                }

                // Return
                return Task.CompletedTask;
            }
        }
    }
} // End Partial class
