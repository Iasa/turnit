using MediatR;
using System.Threading;
using System.Threading.Tasks;
using Turnit.GenericStore.Api.Commands;
using NHibernate;
using Turnit.GenericStore.Api.Entities;
using System.Linq;
using NHibernate.Linq;
using System;

namespace Turnit.GenericStore.Api.CommandHandlers
{
    public class AddProductToCategoryCommandHandler : IRequestHandler<AddProductToCategoryCommand, string>
    {
        private readonly ISession _session;

        public AddProductToCategoryCommandHandler(ISession session)
        {
            _session = session;
        }

        public async Task<string> Handle(AddProductToCategoryCommand request, CancellationToken cancellationToken)
        {
            var productCategory = await _session.Query<ProductCategory>()
                .FirstOrDefaultAsync(x => x.Category.Id == request.CategoryId && x.Product.Id == request.ProductId, cancellationToken: cancellationToken);

            if(productCategory != null)
            {
                throw new Exception("Product already exists in specified category");
            }

            var product = await _session.Query<Product>()
                .FirstOrDefaultAsync(x => x.Id == request.ProductId, cancellationToken: cancellationToken);

            if (product == null)
            {
                throw new Exception("Specified Product Id doesn't exists");
            }

            var category = await _session.Query<Category>()
                .FirstOrDefaultAsync(x => x.Id == request.CategoryId, cancellationToken: cancellationToken);

            if (category == null)
            {
                throw new Exception("Specified Category Id doesn't exists");
            }

            var newProductCategory = new ProductCategory
            {
                Product = product,
                Category = category
            };

            await _session.SaveAsync(newProductCategory, cancellationToken);

            await _session.FlushAsync(cancellationToken);

            return "Product added successfully to category";
        }
    }
}
