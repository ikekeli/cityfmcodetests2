namespace Zaharuddin.Controllers;

// Partial class
public partial class HomeController : Controller
{
    // list
    [Route("fxratelist/{id?}", Name = "fxratelist-fxrate-list")]
    [Route("home/fxratelist/{id?}", Name = "fxratelist-fxrate-list-2")]
    public async Task<IActionResult> FxrateList()
    {
        // Create page object
        fxrateList = new GLOBALS.FxrateList(this);
        fxrateList.Cache = _cache;

        // Run the page
        return await fxrateList.Run();
    }

    // add
    [Route("fxrateadd/{id?}", Name = "fxrateadd-fxrate-add")]
    [Route("home/fxrateadd/{id?}", Name = "fxrateadd-fxrate-add-2")]
    public async Task<IActionResult> FxrateAdd()
    {
        // Create page object
        fxrateAdd = new GLOBALS.FxrateAdd(this);

        // Run the page
        return await fxrateAdd.Run();
    }

    // edit
    [Route("fxrateedit/{id?}", Name = "fxrateedit-fxrate-edit")]
    [Route("home/fxrateedit/{id?}", Name = "fxrateedit-fxrate-edit-2")]
    public async Task<IActionResult> FxrateEdit()
    {
        // Create page object
        fxrateEdit = new GLOBALS.FxrateEdit(this);

        // Run the page
        return await fxrateEdit.Run();
    }

    // delete
    [Route("fxratedelete/{id?}", Name = "fxratedelete-fxrate-delete")]
    [Route("home/fxratedelete/{id?}", Name = "fxratedelete-fxrate-delete-2")]
    public async Task<IActionResult> FxrateDelete()
    {
        // Create page object
        fxrateDelete = new GLOBALS.FxrateDelete(this);

        // Run the page
        return await fxrateDelete.Run();
    }
}
