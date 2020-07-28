using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace FPTBooking.WebAdmin.Models
{
    public interface IErrorModel : IReturnViewModel
    {
        string RequestId { get; set; }
        bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
        string Message { get; set; }
    }
}
