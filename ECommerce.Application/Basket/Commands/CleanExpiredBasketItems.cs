using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.Basket.Commands
{
    public class CleanExpiredBasketItemsCommand : IRequest<CleanExpiredBasketItemsResult>
    {
        public int ExpiryDays { get; set; } = 7;
    }

    public class CleanExpiredBasketItemsResult
    {
        public int ItemsDeleted { get; set; }
        public List<string> DeletedItemNames { get; set; } = new();
        public List<string> Errors { get; set; } = new();
    }
}
