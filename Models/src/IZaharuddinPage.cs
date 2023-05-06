namespace Zaharuddin.Models;

// Partial class
public partial class cityfmcodetests {
    // IZaharuddinPage interface // DN
    public interface IZaharuddinPage
    {
        Task<IActionResult> Run();
        IActionResult Terminate(string url = "");
    }
} // End Partial class
