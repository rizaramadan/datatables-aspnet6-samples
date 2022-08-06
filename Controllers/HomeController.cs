using System.Diagnostics;
using DataTables.AspNet.AspNetCore;
using DataTables.AspNet.Core;
using Microsoft.AspNetCore.Mvc;
using datatables.aspnet5.samples.Models;

namespace datatables.aspnet5.samples.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        return View();
    }
    
    /// <summary>
    /// This is your data method.
    /// DataTables will query this (HTTP GET) to fetch data to display.
    /// </summary>
    /// <param name="request">
    /// This represents your DataTables request.
    /// It's automatically binded using the default binder and settings.
    /// 
    /// You should use IDataTablesRequest as your model, to avoid unexpected behavior and allow
    /// custom binders to be attached whenever necessary.
    /// </param>
    /// <returns>
    /// Return data here, with a json-compatible result.
    /// </returns>
    public IActionResult PageData(IDataTablesRequest request)
    {
        // Nothing important here. Just creates some mock data.
        var data = Models.SampleEntity.GetSampleData();

        // Global filtering.
        // Filter is being manually applied due to in-memmory (IEnumerable) data.
        // If you want something rather easier, check IEnumerableExtensions Sample.
        var filteredData = String.IsNullOrWhiteSpace(request.Search.Value)
            ? data
            : data.Where(_item => _item.Name.Contains(request.Search.Value));

        // Paging filtered data.
        // Paging is rather manual due to in-memmory (IEnumerable) data.
        var dataPage = filteredData.Skip(request.Start).Take(request.Length);

        // Response creation. To create your response you need to reference your request, to avoid
        // request/response tampering and to ensure response will be correctly created.
        var response = DataTablesResponse.Create(request, data.Count(), filteredData.Count(), dataPage);

        // Easier way is to return a new 'DataTablesJsonResult', which will automatically convert your
        // response to a json-compatible content, so DataTables can read it when received.
        return new DataTablesJsonResult(response, true);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
