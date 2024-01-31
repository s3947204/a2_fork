using Data;
using Data.Models;

namespace AdminAPI.Repository;
public class CustomerManager
{
    private readonly MCBAContext _context;
    public CustomerManager(MCBAContext context)
    {
        _context = context;
    }   


    public IEnumerable<Customer> Get()
    {
        var customer = _context.Customer.ToList();
        return customer;
    }

    public Customer Get(int id)
    {
        return _context.Customer.Find(id);
    }


    public void Update(Customer customer)
    {
        _context.Customer.Update(customer);
        _context.SaveChanges();
    }



    public void Lock(int customerID)
    {
        var customer = _context.Customer.Find(customerID);
        if (customer != null) { 
            customer.Locked = true;
            _context.SaveChanges();
        }
    }

    public void Unlock(int customerID)
    {
        var customer = _context.Customer.Find(customerID);
        if (customer != null)
        {
            customer.Locked = false;
            _context.SaveChanges();
        }
    }
}

