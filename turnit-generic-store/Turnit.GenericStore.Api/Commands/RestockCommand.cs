using MediatR;
using System;
using System.Collections.Generic;

namespace Turnit.GenericStore.Api.Commands
{
    public class RestockCommand : IRequest<string>
    {
        public Guid StoreId { get; set; }
        public List<RestockModel> ProductList { get; set; }
    }

    public class RestockModel
    {
        public Guid ProductId { get; set; }
        public int Amount { get; set; }
    }
}
