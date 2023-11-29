using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Voltage.Core.Abstract;

namespace Voltage.Entities.Entity
{
    public class UsersFriendListResult : IEntity
    {
        public string? SenderName { get; set; }
        public string? SenderId { get; set; }
        public string? ReceiverName { get; set; }
        public string? ReceiverId { get; set; }
        public Status RequestStatus { get; set; }
        public DateTime? RequestedDate { get; set; }
        public DateTime? AcceptedDate { get; set; }
    }
}
