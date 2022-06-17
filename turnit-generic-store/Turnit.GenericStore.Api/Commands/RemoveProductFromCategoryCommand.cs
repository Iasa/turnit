using MediatR;
using System;

namespace Turnit.GenericStore.Api.Commands
{
    public class RemoveProductFromCategoryCommand : IRequest<string>
    {
        public Guid ProductId { get; set; }
        public Guid CategoryId { get; set; }
    }
}
