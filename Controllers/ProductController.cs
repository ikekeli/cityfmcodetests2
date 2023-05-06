namespace Zaharuddin.Controllers;

// Partial class
public partial class HomeController : Controller
{
    // list
    [Route("productlist/{productId?}", Name = "productlist-product-list")]
    [Route("home/productlist/{productId?}", Name = "productlist-product-list-2")]
    public async Task<IActionResult> ProductList()
    {
        // Create page object
        productList = new GLOBALS.ProductList(this);
        productList.Cache = _cache;

        // Run the page
        return await productList.Run();
    }

    // add
    [Route("productadd/{productId?}", Name = "productadd-product-add")]
    [Route("home/productadd/{productId?}", Name = "productadd-product-add-2")]
    public async Task<IActionResult> ProductAdd()
    {
        // Create page object
        productAdd = new GLOBALS.ProductAdd(this);

        // Run the page
        return await productAdd.Run();
    }

    // edit
    [Route("productedit/{productId?}", Name = "productedit-product-edit")]
    [Route("home/productedit/{productId?}", Name = "productedit-product-edit-2")]
    public async Task<IActionResult> ProductEdit()
    {
        // Create page object
        productEdit = new GLOBALS.ProductEdit(this);

        // Run the page
        return await productEdit.Run();
    }

    // delete
    [Route("productdelete/{productId?}", Name = "productdelete-product-delete")]
    [Route("home/productdelete/{productId?}", Name = "productdelete-product-delete-2")]
    public async Task<IActionResult> ProductDelete()
    {
        // Create page object
        productDelete = new GLOBALS.ProductDelete(this);

        // Run the page
        return await productDelete.Run();
    }
}
