using ECommerce.API.DTOs;
using ECommerce.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CustomersController : ControllerBase
{
    private readonly CustomerService _customerService;

    public CustomersController(CustomerService customerService)
    {
        _customerService = customerService;
    }

    [HttpGet("{id}")]
    public async Task<ActionResult> GetById(int id)
    {
        var customer = await _customerService.GetByIdAsync(id);
        if (customer == null)
            return NotFound($"Customer with ID {id} not found.");

        return Ok(customer);
    }

    [HttpPost]
    public async Task<ActionResult> Create([FromBody] CreateCustomerDto dto)
    {
        var (success, error, customer) = await _customerService.CreateAsync(dto);
        if (!success)
            return BadRequest(error);

        return CreatedAtAction(nameof(GetById), new { id = customer!.Id }, customer);
    }

    [HttpPost("{id}/upgrade-vip")]
    public async Task<IActionResult> UpgradeToVip(int id)
    {
        var (success, error) = await _customerService.UpgradeToVipAsync(id);
        if (!success)
        {
            if (error == "NotFound")
                return NotFound();

            return BadRequest(error);
        }

        return Ok(new { message = "Customer upgraded to VIP successfully." });
    }
}