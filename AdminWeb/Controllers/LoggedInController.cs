using AdminWeb.Filters;
using Data.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Mime;
using System.Text;

namespace AdminWeb.Controllers;

[AuthorizeLogin]
public class LoggedInController : Controller
{

    private readonly HttpClient _client;
    public LoggedInController(IHttpClientFactory clientFactory)
    {
        _client = clientFactory.CreateClient("api");
    }

    public async Task<IActionResult> Index()
    {
        // need to make a api call here to get a list of customers to pass to the view
        var response = await _client.GetAsync("api/Customer");
        response.EnsureSuccessStatusCode();

        // Storing the response details received from web api.
        var result = await response.Content.ReadAsStringAsync();

        // Deserializing the response received from web api and storing into a list.
        var customers = JsonConvert.DeserializeObject<List<Customer>>(result);
        return View(customers);
    }


    public async Task<IActionResult> Edit(int id)
    {    
        return View(await GetCustomer(id));
    }

    [HttpPost]
    public async Task<IActionResult> Edit(Customer customer)
    {
        if (ModelState.IsValid)
        {
            var content =
                new StringContent(JsonConvert.SerializeObject(customer), Encoding.UTF8, MediaTypeNames.Application.Json);

            var response = await _client.PutAsync("api/Customer/Update", content);

            if (response.IsSuccessStatusCode)
                return RedirectToAction("Index");
        }

        return View(customer);
    }

    public async Task<IActionResult> Lock(int id)
    {
        var response = await _client.PutAsync($"api/Customer/Lock/{id}", null);
        if(response.IsSuccessStatusCode)
          return RedirectToAction("Index");

        return BadRequest();
    }

    public async Task<IActionResult> Unlock(int id)
    {
        var response = await _client.PutAsync($"api/Customer/Unlock/{id}", null);
        if (response.IsSuccessStatusCode)
            return RedirectToAction("Index");

        return BadRequest();
    }



    private async Task<Customer> GetCustomer(int id)
    {
        var response = await _client.GetAsync($"api/Customer/{id}");
        response.EnsureSuccessStatusCode();

        // Storing the response details received from web api.
        var result = await response.Content.ReadAsStringAsync();
        var customer = JsonConvert.DeserializeObject<Customer>(result);
        return customer;
    }
}


