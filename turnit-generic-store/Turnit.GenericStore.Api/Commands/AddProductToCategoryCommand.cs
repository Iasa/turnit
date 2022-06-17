using MediatR;
using System;

namespace Turnit.GenericStore.Api.Commands
{
    public class AddProductToCategoryCommand : IRequest<string>
    {
        public Guid ProductId { get; set; }
        public Guid CategoryId { get; set; }
    }
}
