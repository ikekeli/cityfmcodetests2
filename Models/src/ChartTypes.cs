namespace Zaharuddin.Models;

// Partial class
public partial class cityfmcodetests {
    /// <summary>
    /// ChartTypes class
    /// </summary>
    public static class ChartTypes
    {
        /// <summary>
        /// Supported chart types
        ///
        /// Format - [chart_id, [normal_chart_name, scroll_chart_name]]
        /// chart_id - abnn
        /// **id = chart_id in previous version
        /// - a: 1 = Single Series, 2 = Multi Series, 3 = Stacked, 4 = Combination, 5 = Financial, 6 = Other
        /// - b: 0 = 2D, 1 = 3D
        /// - nn: 01 = Column, 02 = Line, 03 = Area, 04 = Bar, 05 = Pie, 06 = Doughnut, 07 Pareto
        /// - nn: 91 = Marimekko, 92 = Zoom-line
        /// - nn: 99 = Candlestick, 98 = Gantt
        /// </summary>
        public static Dictionary<string, string[]> Types = new ()
        {
            // Single Series
            { "1001", new[] {"bar"} }, // Column 2D (**1)
            { "1101", new[] {"bar"} }, // Column 3D (**5) // NOT supported, revert to Column 2D
            { "1002", new[] {"line"} }, // Line 2D (**4) // fill=false
            { "1003", new[] {"line"} }, // Area 2D (**7)
            { "1004", new[] {"bar"} }, // Bar 2D (**3)
            { "1104", new[] {"bar"} }, // Bar 3D (**104) // NOT supported, revert to Bar 2D
            { "1005", new[] {"pie"} }, // Pie 2D (**2)
            { "1105", new[] {"pie"} }, // Pie 3D (**6) // NOT supported, revert to Pie 2D
            { "1006", new[] {"doughnut"} }, // Doughnut 2D (**8)
            { "1106", new[] {"doughnut"} }, // Doughnut 3D (**101) // NOT supported, revert to Dougnut 2D

            // Multi Series
            { "2001", new[] {"bar"} }, // Multi-series Column 2D (**9)
            { "2101", new[] {"bar"} }, // Multi-series Column 3D (**10) // NOT supported, revert to Column 2D
            { "2002", new[] {"line"} }, // Multi-series Line 2D (**11) // fill=false
            { "2003", new[] {"line"} }, // Multi-series Area 2D (**12)
            { "2004", new[] {"bar"} }, // Multi-series Bar 2D (**13)
            { "2104", new[] {"bar"} }, // Multi-series Bar 3D (**102) // NOT supported, revert to Bar 2D

            // Stacked
            { "3001", new[] {"bar"} }, // Stacked Column 2D (**14)
            { "3101", new[] {"bar"} }, // Stacked Column 3D (**15) // NOT supported, revert to Column 2D
            { "3003", new[] {"line"} }, // Stacked Area 2D (**16)
            { "3004", new[] {"bar"} }, // Stacked Bar 2D (**17)
            { "3104", new[] {"bar"} }, // Stacked Bar 3D (**103) // NOT supported, revert to Bar 2D

            // Combination
            { "4001", new[] {"bar"} }, // Multi-series 2D Single Y Combination Chart (Column + Line + Area)
            { "4101", new[] {"bar"} }, // Multi-series 3D Single Y Combination Chart (Column + Line + Area) // NOT supported, revert to 4001
            { "4111", new[] {"bar"} }, // Multi-series Column 3D + Line - Single Y Axis // NOT supported, revert to 4001
            { "4021", new[] {"bar"} }, // Stacked Column2D + Line single Y Axis // Mixed, type in dataset
            { "4121", new[] {"bar"} }, // Stacked Column3D + Line single Y Axis // NOT supported, revert to 4021
            { "4031", new[] {"bar"} }, // Multi-series 2D Dual Y Combination Chart (Column + Line + Area) (**18) // Mixed, type in dataset
            { "4131", new[] {"bar"} }, // Multi-series Column 3D + Line - Dual Y Axis (**19) // NOT supported, revert to 4031
            { "4141", new[] {"bar"} } // Stacked Column 3D + Line Dual Y Axis // NOT supported, revert to 2D (Stacked Column 2D + Line Dual Y Axis)
        };

        /// <summary>
        /// Default type ID
        /// </summary>
        public static string DefaultType = "1001"; // // Default

        /// <summary>
        /// Get chart type name
        /// </summary>
        /// <param name="id">Chart type ID</param>
        /// <param name="scroll">Whether chart is scrollable</param>
        /// <returns>Chart name</returns>
        public static string GetName(string id, bool scroll = false) => Types.TryGetValue(id, out string[]? names)
            ? (scroll && names.Length >= 2 ? names[1] : names[0])
            : "bar"; // Default

        /// <summary>
        /// Get renderer class
        /// </summary>
        /// <returns>Renderer class typee</returns>
        public static Type GetRendererClass() => !Empty(Config.DefaultChartRenderer)
            ? Type.GetType(Config.DefaultChartRenderer) ?? typeof(ChartJsRenderer)
            : typeof(ChartJsRenderer);
    }
} // End Partial class
