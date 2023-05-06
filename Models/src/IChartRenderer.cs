namespace Zaharuddin.Models;

// Partial class
public partial class cityfmcodetests {
    /// <summary>
    /// Interface IChartRenderer
    /// </summary>
    public interface IChartRenderer
    {
        string GetContainer(int width, int height);
        string GetScript(int width, int height);
    }
} // End Partial class
