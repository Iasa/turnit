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
    public class RestockCommandHandler : IRequestHandler<RestockCommand, string>
    {
        private readonly ISession _session;

        public RestockCommandHandler(ISession session)
        {
            _session = session;
        }
        public async Task<string> Handle(RestockCommand request, CancellationToken cancellationToken)
        {
            foreach (var product in request.ProductList)
            {
                var productAvailability = await _session.Query<ProductAvailability>()
                .FirstOrDefaultAsync(x => x.Product.Id == product.ProductId && x.Store.Id == request.StoreId, cancellationToken: cancellationToken);

                if (productAvailability == null)
                {
                    Console.WriteLine("Specified product/store doesn't exists or product is not available in specified store");
                    continue;
                }

                productAvailability.Availability += product.Amount;

                await _session.UpdateAsync(productAvailability, cancellationToken);
                await _session.FlushAsync(cancellationToken);
            }

            return "Store restocked successfully";
        }
    }
}
