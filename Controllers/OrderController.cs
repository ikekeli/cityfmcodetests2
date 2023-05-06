namespace Zaharuddin.Controllers;

// Partial class
public partial class HomeController : Controller
{
    // list
    [Route("orderlist/{customerID?}", Name = "orderlist-order-list")]
    [Route("home/orderlist/{customerID?}", Name = "orderlist-order-list-2")]
    public async Task<IActionResult> OrderList()
    {
        // Create page object
        orderList = new GLOBALS.OrderList(this);
        orderList.Cache = _cache;

        // Run the page
        return await orderList.Run();
    }

    // add
    [Route("orderadd/{customerID?}", Name = "orderadd-order-add")]
    [Route("home/orderadd/{customerID?}", Name = "orderadd-order-add-2")]
    public async Task<IActionResult> OrderAdd()
    {
        // Create page object
        orderAdd = new GLOBALS.OrderAdd(this);

        // Run the page
        return await orderAdd.Run();
    }

    // edit
    [Route("orderedit/{customerID?}", Name = "orderedit-order-edit")]
    [Route("home/orderedit/{customerID?}", Name = "orderedit-order-edit-2")]
    public async Task<IActionResult> OrderEdit()
    {
        // Create page object
        orderEdit = new GLOBALS.OrderEdit(this);

        // Run the page
        return await orderEdit.Run();
    }

    // delete
    [Route("orderdelete/{customerID?}", Name = "orderdelete-order-delete")]
    [Route("home/orderdelete/{customerID?}", Name = "orderdelete-order-delete-2")]
    public async Task<IActionResult> OrderDelete()
    {
        // Create page object
        orderDelete = new GLOBALS.OrderDelete(this);

        // Run the page
        return await orderDelete.Run();
    }
}
