using MediatR;
using System;
using System.Collections.Generic;

namespace Turnit.GenericStore.Api.Commands
{
    public class BookProductCommand : IRequest<string>
    {
        public List<BookProductModel> ProductsToBook { get; set; }
    }

    public class BookProductModel
    {
        public Guid ProductId { get; set; }
        public Guid StoreId { get; set; }
        public int Amount { get; set; }
    }
}
