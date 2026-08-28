using ECommerce.Application.Customers.Commands.RegisterCustomer;
using ECommerce.Application.Customers.Commands.UpgradeToVip;
using ECommerce.Application.Customers.DTOs;
using ECommerce.Application.Customers.Queries.GetById;
using ECommerce.Application.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading;

namespace ECommerce.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CustomersController : BaseApiController
{

    [HttpGet("{id}")]
    public async Task<ActionResult> GetById(int id, CancellationToken cancellationToken)
    {
        var customer = await Sender.Send(new GetCustomerByIdQuery(id),cancellationToken);
        if (customer == null)
            return NotFound($"Customer with ID {id} not found.");

        return Ok(customer);
    }

    [HttpPost]
    public async Task<ActionResult> Create([FromBody] CreateCustomerDto dto, CancellationToken cancellationToken)
    {
        var (success, error, customer) = await Sender.Send(new RegisterCustomerCommand(dto), cancellationToken);
        if (!success)
            return BadRequest(error);

        return CreatedAtAction(nameof(GetById), new { id = customer!.Id }, customer);
    }

    [HttpPost("{id}/upgrade-vip")]
    public async Task<IActionResult> UpgradeToVip(int id, CancellationToken cancellationToken)
    {
        var (success, error) = await await Sender.Send(new UpgradeCustomerToVipCommand(id), cancellationToken);
        if (!success)
        {
            if (error == "NotFound")
                return NotFound();

            return BadRequest(error);
        }

        return Ok(new { message = "Customer upgraded to VIP successfully." });
    }
}