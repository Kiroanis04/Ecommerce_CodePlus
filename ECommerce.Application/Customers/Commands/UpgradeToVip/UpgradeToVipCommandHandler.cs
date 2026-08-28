using ECommerce.Application.Customers.Commands.UpgradeToVip;
using ECommerce.Application.Customers.DTOs;
using ECommerce.Application.Repository_Interfaces;
using ECommerce.DAL.Entities;
using MediatR;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ECommerce.Application.Customers.Commands.UpgradeCustomerToVip
{
    public sealed class UpgradeCustomerToVipCommandHandler
        : IRequestHandler<UpgradeCustomerToVipCommand, UpgradeToVipResponse>
    {
        private const decimal VipThreshold = 5000m;

        private readonly ICustomerRepository _customerRepository;

        public UpgradeCustomerToVipCommandHandler(ICustomerRepository customerRepository)
        {
            _customerRepository = customerRepository;
        }

        public async Task<UpgradeToVipResponse> Handle(UpgradeCustomerToVipCommand request, CancellationToken cancellationToken)
        {
            var customer = await _customerRepository.GetByIdWithOrdersAsync(request.CustomerId);

            if (customer == null)
                return new UpgradeToVipResponse(false, "NotFound");

            var totalSpent = customer.Orders
                .Where(o => o.Status == OrderStatus.Paid)
                .Sum(o => o.TotalAmount);

            if (totalSpent < VipThreshold)
                return new UpgradeToVipResponse(
                    false,
                    $"Customer does not qualify for VIP. Total spend {totalSpent:C} is less than required {VipThreshold:C}.");

            customer.IsVip = true;
            await _customerRepository.SaveChangesAsync();

            return new UpgradeToVipResponse(true, null);
        }
    }
}