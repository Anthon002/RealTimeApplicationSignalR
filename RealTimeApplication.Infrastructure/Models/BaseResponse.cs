using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RealTimeApplication.Infrastructure.Models
{
    public sealed record BaseResponse(bool Status ,string Message);
    public sealed record BaseResponse<T>(bool Status, string Message, T? Value = default);
}