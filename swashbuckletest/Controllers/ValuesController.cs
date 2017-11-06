using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace swashbuckletest.Controllers
{
    using Models;
    using X.PagedList;

    [Route("api/[controller]")]
    public class ValuesController : Controller
    {
        // GET api/values
        [HttpGet]
        [ProducesResponseType(typeof(IPagedList<Customer>), 200)]
        public IActionResult Get()
        {
            var customers = new List<Customer>
            {
                new Customer {Id = 1, Name = "dummy", Number = 1000}
            };

            return Ok(customers.ToPagedList());
        }
    }
}
