namespace Zaharuddin.Models;

// Partial class
public partial class cityfmcodetests {
    // Chart data comparer // DN
    public class ChartDataComparer : Comparer<Dictionary<string, object>>
    {
        public int Index = 0;

        public object Seq = ""; // Empty (Default)

        public string Order = "ASC"; // ASC/DESC

        public ChartDataComparer(int index, object seq, string order)
        {
            if (index > 0)
                Index = index;
            Seq = seq;
            Order = SameText(order, "ASC") ? "ASC" : "DESC";
        }

        public override int Compare(Dictionary<string, object>? d1, Dictionary<string, object>? d2)
        {
            object? x = d1?.ElementAt(Index).Value;
            object? y = d2?.ElementAt(Index).Value;
            if (Empty(Seq)) { // Default
                if (IsNumeric(x) && IsNumeric(y)) {
                    Seq = "_number";
                } else if (IsDate(x) && IsDate(y)) {
                    Seq = "_date";
                } else {
                    Seq = "_string";
                }
            }
            if (SameText(Seq, "_string") && Order == "ASC") { // String, ASC
                return String.Compare(ConvertToString(x), ConvertToString(y));
            } else if (SameText(Seq, "_string") && Order == "DESC") { // String, DESC
                return String.Compare(ConvertToString(y), ConvertToString(x));
            } else if (SameText(Seq, "_number") && Order == "ASC") { // Number, ASC
                if (IsNumeric(x) && IsNumeric(y))
                    return Convert.ToDouble(x).CompareTo(Convert.ToDouble(y));
            } else if (SameText(Seq, "_number") && Order == "DESC") { // Number, DESC
                if (IsNumeric(x) && IsNumeric(y))
                    return Convert.ToDouble(y).CompareTo(Convert.ToDouble(x));
            } else if (SameText(Seq, "_date") && Order == "ASC") { // Date, ASC
                if (IsDate(x) && IsDate(y))
                    return DateTime.Compare(Convert.ToDateTime(x), Convert.ToDateTime(y));
            } else if (SameText(Seq, "_date") && Order == "DESC") { // Date, DESC
                if (IsDate(x) && IsDate(y))
                    return DateTime.Compare(Convert.ToDateTime(y), Convert.ToDateTime(x));
            } else if (!Empty(Seq) && ConvertToString(Seq).Contains("|")) { // Custom sequence by delimited string
                string[] ar = ConvertToString(Seq).Split('|');
                string sx = ConvertToString(x).Trim(), sy = ConvertToString(y).Trim();
                if (ar.Contains(sx) && ar.Contains(sy))
                    if (Order == "ASC") // Custom, ASC
                        return Array.IndexOf(ar, sx) - Array.IndexOf(ar, sy);
                    else // Custom, DESC
                        return Array.IndexOf(ar, sy) - Array.IndexOf(ar, sx);
            }
            return 0;
        }
    }
} // End Partial class
