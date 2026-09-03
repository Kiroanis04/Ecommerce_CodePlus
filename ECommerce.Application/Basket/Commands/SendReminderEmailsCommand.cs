using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.Basket.Commands
{
    public class SendReminderEmailsCommand : IRequest<SendReminderEmailsResult>
    {
        public int DaysAfterAddition { get; set; } = 4;
        public int MaxEmailsPerBatch { get; set; } = 100;
    }

    public class SendReminderEmailsResult
    {
        public int EmailsSent { get; set; }
        public List<string> Recipients { get; set; } = new();
        public List<string> Errors { get; set; } = new();
    }
}
