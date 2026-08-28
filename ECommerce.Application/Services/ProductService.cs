using ECommerce.Application.Products.DTOs;
using ECommerce.Application.Repository_Interfaces;
using ECommerce.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.Services
{
    public class ProductService
    {
        private readonly IProductRepository _repository;

        public ProductService(IProductRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<Product>> GetAllAsync()
        {
            return await _repository.GetAllAsync();
        }

        public async Task<Product?> GetByIdAsync(int id)
        {
            return await _repository.GetByIdAsync(id);
        }

        public async Task<(bool Success, string? Error, Product? Product)> CreateAsync(CreateProductDto dto)
        {
            if (dto.Price <= 0)
                return (false, "Product price must be greater than zero.", null);

            if (dto.StockQuantity < 0)
                return (false, "Stock quantity cannot be negative.", null);

            if (await _repository.SkuExistsAsync(dto.SKU))
                return (false, $"Product with SKU '{dto.SKU}' already exists.", null);

            var product = new Product
            {
                Name = dto.Name,
                SKU = dto.SKU.ToUpper(),
                Price = dto.Price,
                StockQuantity = dto.StockQuantity
            };

            await _repository.AddAsync(product);
            await _repository.SaveChangesAsync();

            return (true, null, product);
        }

        public async Task<(bool Success, string? Error)> UpdateAsync(int id, Product product)
        {
            var existing = await _repository.GetByIdAsync(id);
            if (existing == null)
                return (false, $"Product with ID {id} not found.");

            if (product.Price <= 0)
                return (false, "Price must be positive.");

            existing.Name = product.Name;
            existing.SKU = product.SKU;
            existing.Price = product.Price;
            existing.StockQuantity = product.StockQuantity;

            _repository.Update(existing);
            await _repository.SaveChangesAsync();

            return (true, null);
        }

        public async Task<(bool Success, string? Error)> DeleteAsync(int id)
        {
            var product = await _repository.GetByIdAsync(id);
            if (product == null)
                return (false, $"Product with ID {id} not found.");

            _repository.Remove(product);
            await _repository.SaveChangesAsync();

            return (true, null);
        }
    }
}
