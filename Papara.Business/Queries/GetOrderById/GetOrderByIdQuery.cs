﻿using MediatR;
using Papara.Business.DTOs.Order;
using Papara.Business.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Papara.Business.Queries.GetOrderById
{
    public record GetOrderByIdQuery(Guid Id) : IRequest<ResponseHandler<OrderResponse>>;
}