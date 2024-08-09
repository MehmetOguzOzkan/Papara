using MediatR;
using Papara.Business.DTOs.Payment;
using Papara.Business.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Papara.Business.Commands.Payment
{
    public record PaymentCommand(PaymentRequest Request) : IRequest<ResponseHandler<PaymentResponse>>;
}
