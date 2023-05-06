namespace Zaharuddin.Models;

// Partial class
public partial class cityfmcodetests {

    public class CustomMySqlCompiler : MySqlCompiler, ICustomCompiler
    {
        public static bool QuoteIdentifier = true;

        public string Output { get; set; } = "";

        /// <summary>
        /// Constructor
        /// </summary>
        public CustomMySqlCompiler()
        {
            LastId = $"SELECT last_insert_id() as {WrapValue("Id")}";
        }

        /// <summary>
        /// Wrap a single string in a column identifier.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public override string Wrap(string value)
        {
            if (value.ToLowerInvariant().Contains(" as "))
            {
                var (before, after) = SplitAlias(value);
                return Wrap(before) + $" {ColumnAsKeyword}" + WrapValue(after);
            }
            if (!QuoteIdentifier)
                return value;
            if (value.Contains("."))
            {
                bool open = false;
                return string.Join(".", value.Split('.').Select((x, index) =>
                {
                    if (x.StartsWith(OpeningIdentifier) && x.EndsWith(ClosingIdentifier)) // Already quoted
                    {
                        return x;
                    }
                    else if (x.StartsWith(OpeningIdentifier) && !x.EndsWith(ClosingIdentifier)) // With opening identifier but without closing identifier
                    {
                        open = true;
                        return x;
                    }
                    else if (!x.StartsWith(OpeningIdentifier) && x.EndsWith(ClosingIdentifier)) // Without opening identifier but with closing identifier
                    {
                        open = false;
                        return x;
                    } else if (open) {
                        return x;
                    }
                    return WrapValue(x);
                }));
            }
            if (value.StartsWith(OpeningIdentifier) && value.EndsWith(ClosingIdentifier)) // Already quoted
                return value;

            // If we reach here then the value does not contain an "AS" alias
            // nor dot "." expression, so wrap it as regular value.
            return WrapValue(value);
        }
    }
} // End Partial class
