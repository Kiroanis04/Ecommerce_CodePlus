using ECommerce.Application.Products.Commands.Create;
using ECommerce.Application.Products.Commands.Delete;
using ECommerce.Application.Products.Commands.DeleteProduct;
using ECommerce.Application.Products.Commands.Update;
using ECommerce.Application.Products.DTOs;
using ECommerce.Application.Products.Queries.GetAll;
using ECommerce.Application.Products.Queries.GetById;
using ECommerce.Application.Services;
using ECommerce.DAL.Entities;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : BaseApiController
{

    [HttpGet]
    public async Task<ActionResult<List<Product>>> GetAll(CancellationToken cancellationToken)
    {
        var products = await Sender.Send(new GetAllProductsQuery(), cancellationToken);
        return Ok(products);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Product>> GetById(int id, CancellationToken cancellationToken)
    {
        var product = await Sender.Send(new GetByIdQuery(id), cancellationToken);
        if (product == null)
            return NotFound($"Product with ID {id} not found.");

        return Ok(product);
    }

    [HttpPost]
    public async Task<ActionResult<Product>> Create([FromBody] CreateProductDto dto)
    {
        var (success, error, product) = await Sender.Send(CreateCommand(dto));
        if (!success)
            return BadRequest(error);

        return CreatedAtAction(nameof(GetById), new { id = product!.Id }, product);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] Product product)
    {
        var (success, error) = await Sender.Send(UpdateProductCommand(id,product));
        if (!success)
        {
            return error!.Contains("not found") ? NotFound(error) : BadRequest(error);
        }

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var (success, error) = await Sender.Send(DeleteProductCommand(id));
        if (!success)
            return NotFound(error);

        return NoContent();
    }
}