using MediatR;
using NHibernate;
using NHibernate.Linq;
using System;
using System.Threading;
using System.Threading.Tasks;
using Turnit.GenericStore.Api.Commands;
using Turnit.GenericStore.Api.Entities;

namespace Turnit.GenericStore.Api.CommandHandlers
{
    public class RemoveProductFromCategoryCommandHandler : IRequestHandler<RemoveProductFromCategoryCommand, string>
    {
        private readonly ISession _session;

        public RemoveProductFromCategoryCommandHandler(ISession session)
        {
            _session = session;
        }

        public async Task<string> Handle(RemoveProductFromCategoryCommand request, CancellationToken cancellationToken)
        {
            var productCategory = await _session.Query<ProductCategory>()
                .FirstOrDefaultAsync(x => x.Category.Id == request.CategoryId && x.Product.Id == request.ProductId, cancellationToken: cancellationToken);

            if (productCategory == null)
            {
                throw new Exception("Product doesn't exists in specified category");
            }

            await _session.DeleteAsync(productCategory, cancellationToken);

            await _session.FlushAsync(cancellationToken);

            return "Product removed successfully from category";
        }
    }
}
