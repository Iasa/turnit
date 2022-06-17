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
    public class BookProductCommandHandler : IRequestHandler<BookProductCommand, string>
    {
        private readonly ISession _session;

        public BookProductCommandHandler(ISession session)
        {
            _session = session;
        }
        public async Task<string> Handle(BookProductCommand request, CancellationToken cancellationToken)
        {
            foreach (var product in request.ProductsToBook)
            {
                var productAvailability = await _session.Query<ProductAvailability>()
                .FirstOrDefaultAsync(x => x.Product.Id == product.ProductId && x.Store.Id == product.StoreId, cancellationToken: cancellationToken);

                if (productAvailability == null)
                {
                    Console.WriteLine("Specified product/store doesn't exists or product is not available in specified store");
                    continue;
                }


                if (productAvailability.Availability < product.Amount)
                {
                    Console.WriteLine($"Product {productAvailability.Product.Name} doesn't have enough items available in store {productAvailability.Store.Name}");
                    continue;
                }

                productAvailability.Availability -= product.Amount;

                await _session.UpdateAsync(productAvailability, cancellationToken);
                await _session.FlushAsync(cancellationToken);
            }

            return "Product booked successfully";
        }
    }
}
