using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Deltas;
using Microsoft.AspNetCore.OData.Formatter;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.Extensions.Logging;
using ODataWebApiApplication15.Models;

namespace ODataWebApiApplication15.Controllers;

public class CustomersController : ODataController
{
    private readonly ILogger<CustomersController> _logger;

    private static List<Customer> _customers = new List<Customer>
    {
        new Customer { Id = 1, Name = "Customer1", CustomerType = CustomerType.Regular, Orders = new List<Order> { new Order { Id = 1, Amount = 100 } } },
        new Customer { Id = 2, Name = "Customer2", CustomerType = CustomerType.Premium | CustomerType.VIP, Orders = new List<Order> { new Order { Id = 2, Amount = 4000 } } },
        new Customer { Id = 3, Name = "Customer3", CustomerType = CustomerType.VIP, Orders = new List<Order> { new Order { Id = 2, Amount = 4000 }, new Order { Id = 3, Amount = 1000 } }}
    };

    public CustomersController(ILogger<CustomersController> logger)
    {
        _logger = logger;
    }

    // GET: odata/Customers
    [EnableQuery(PageSize = 10)]
    public IActionResult Get()
    {
        return Ok(_customers);
    }

    // GET: odata/Customers(1)
    [EnableQuery]
    public IActionResult Get([FromODataUri] int key)
    {
        var customer = _customers.FirstOrDefault(c => c.Id == key);
        if (customer == null)
        {
            return NotFound();
        }

        return Ok(customer);
    }

    // GET: odata/Customers(1)/GetCustomerOrdersTotalAmount
    [EnableQuery]
    [HttpGet("odata/Customers({key})/GetCustomerOrdersTotalAmount")]
    public IActionResult GetCustomerOrdersTotalAmount([FromODataUri] int key)
    {
        var customer = _customers.FirstOrDefault(c => c.Id == key);
        if (customer == null)
        {
            return NotFound();
        }

        return Ok(customer.Orders.Sum(o => o.Amount));
    }

    // GET: odata/Customers/GetCustomerByName(name='Customer1')
    [HttpGet("odata/Customers/GetCustomerByName(name={name})")]
    public IActionResult GetCustomerByName([FromODataUri] string name)
    {
        var customer = _customers.FirstOrDefault(c => c.Name == name);
        if (customer == null)
        {
            return NotFound();
        }

        return Ok(customer);
    }

    // POST: odata/Customers
    public IActionResult Post([FromBody] Customer customer)
    {
        if (!ModelState.IsValid && _customers.Any(c => c.Id == customer.Id))
        {
            return BadRequest(ModelState);
        }

        if (customer.Id <= 0)
        {
            customer.Id = _customers.Max(c => c.Id) + 1;
        }

        if (customer.Orders == null || customer.Orders.Count() == 0)
        {
            customer.Orders = new List<Order>
            {
                new Order { Id = _customers.SelectMany(c => c.Orders).Max(o => o.Id) + 1, Amount = 300 },
                new Order { Id = _customers.SelectMany(c => c.Orders).Max(o => o.Id) + 2, Amount = 400 }
            };
        }

        _customers.Add(customer);

        return Created(customer);
    }

    // PATCH: odata/Customers(2)
    [HttpPatch("odata/Customers({key})")]
    public IActionResult Update([FromRoute]int key, [FromBody] Delta<Customer> delta)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var customer = _customers.FirstOrDefault(c => c.Id == key);

        if (customer == null)
        {
            return NotFound(ModelState);
        }

        delta.TryGetPropertyValue(nameof(Customer.CustomerType), out var customerTypeValue);

        delta.Patch(customer);

        return Ok(customer);
    }
}
