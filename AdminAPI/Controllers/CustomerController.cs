using AdminAPI.Repository;
using Data;
using Data.Models;
using Microsoft.AspNetCore.Mvc;

namespace AdminAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CustomerController : ControllerBase
{

    private readonly CustomerManager _customerManager;
    private readonly MCBAContext _context;

    public CustomerController(CustomerManager customerManager, MCBAContext context)     
    {
        _context = context;
        _customerManager = customerManager;
    }

    /// <summary>
    /// Returns the list of all the customers
    /// </summary>
    /// <returns>List of all customesr</returns>
    /// Endpoint: api/Customer
    [HttpGet]
    public IEnumerable<Customer> Get()
    {
        return _customerManager.Get();
    }



    /// <summary>
    /// Returns a customer with the given id
    /// </summary>
    /// <param name="id">CustomerID</param>
    /// <returns>The customer with the id</returns>
    /// Endpoint: api/Customer/{id}
    [HttpGet("{id}")]
    public Customer Get(int id)
    {
        return _customerManager.Get(id);
    }


    /// <summary>
    /// Given a changed customer, it updates that customer in the db
    /// </summary>
    /// <param name="customer">The updated customer</param>
    [HttpPut("Update")]
    public void Update([FromBody] Customer customer)
    {
        _customerManager.Update(customer);
    }


    /// <summary>
    /// Given a customer id, it changes the lock field of that customer to true in the db
    /// </summary>
    /// <param name="customerID">The customer id </param>
    [HttpPut("Lock/{id}")]
    public void Lock(int id)
    {
        _customerManager.Lock(id);
    }

    /// <summary>
    /// GIven a customer id, it changes the lock field of the customer to false in the db
    /// </summary>
    /// <param name="id">The customer id</param>
    [HttpPut("Unlock/{id}")]
    public void Unlock(int id)
    {
        _customerManager.Unlock(id);
    }
}

