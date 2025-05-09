using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CanteenManage.Repo.Contexts;
using CanteenManage.Repo.Models;

namespace CanteenManage.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FoodOrdersAPIController : ControllerBase
    {
        private readonly CanteenManageDBContext _context;

        public FoodOrdersAPIController(CanteenManageDBContext context)
        {
            _context = context;
        }

        // GET: api/FoodOrdersAPI
        [HttpGet]
        public async Task<ActionResult<IEnumerable<FoodOrder>>> GetFoodOrders()
        {
            return await _context.FoodOrders.ToListAsync();
        }

        // POST: api/FoodOrdersAPI
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<FoodOrder>> AddFoodOrder(FoodOrder foodOrder)
        {
            //_context.FoodOrders.Add(foodOrder);
            //await _context.SaveChangesAsync();

            return Ok(foodOrder);
        }

    }
}
