namespace Zaharuddin.Models;

// Partial class
public partial class cityfmcodetests {
    /// <summary>
    /// Autofac Module
    /// </summary>
    public class AutofacModule : Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            // Connections
            var dbs = Configuration.GetSection("Databases");
            var dbIds = dbs.GetChildren().Select(db => db.Key);
            foreach (string dbId in dbIds) {
                var rb = builder.RegisterGeneric(typeof(DatabaseConnection<,,,>)).UsingConstructor(typeof(String));
                rb.Named(dbId, typeof(DatabaseConnection<,,,>)).InstancePerLifetimeScope(); // Primary
                var rb2 = builder.RegisterGeneric(typeof(DatabaseConnection<,,,>)).UsingConstructor(typeof(String));
                rb2.Named(dbId + Config.SecondaryConnectionName, typeof(DatabaseConnection<,,,>)).InstancePerLifetimeScope(); // Secondary
            }

            // Language
            builder.RegisterType<Lang>().InstancePerLifetimeScope();

            // Security
            builder.RegisterType<AdvancedSecurity>().InstancePerLifetimeScope();

            // Profile
            builder.RegisterType<UserProfile>().InstancePerLifetimeScope();

            // Form
            builder.RegisterType<HttpForm>().InstancePerLifetimeScope();

            // HttpDataDictionary
            builder.RegisterType<HttpDataDictionary>().InstancePerLifetimeScope();

            // List options
            builder.RegisterType<ListOptions>().InstancePerLifetimeScope();

            // List options dictionary
            builder.RegisterType<ListOptionsDictionary>().InstancePerLifetimeScope();

            // List actions
            builder.RegisterType<ListActions>().InstancePerLifetimeScope();

            // Register named types
            foreach (var (name, t) in Config.NamedTypes)
                builder.RegisterType(t).Named(name, t).InstancePerLifetimeScope();

            // Register types
            var typeNames = Config.NamedTypes.Select(kvp => kvp.Value.Name);
            builder.RegisterAssemblyTypes(Assembly.GetExecutingAssembly())
                .Where(t => !t.Name.Contains("<") && !typeNames.Contains(t.Name) && typeNames.Any(typeName => t.Name.StartsWith(typeName)) && t.MemberType == MemberTypes.NestedType)
                .InstancePerLifetimeScope();

            // Container Build event
            ContainerBuild(builder);
        }
    }
} // End Partial class
