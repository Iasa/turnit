using MediatR;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Turnit.GenericStore.Api.Commands;
using Turnit.GenericStore.Api.Dtos;

namespace Turnit.GenericStore.Api.Features.Sales
{
    [Route("store")]
    public class StoreController : ApiControllerBase
    {
        private readonly IMediator _mediator;

        public StoreController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost, Route("{storeId}/restock")]
        public async Task<ActionResult<string>> Restock(Guid storeId, [FromBody] List<RestockRequest> restockRequest)
        {
            var command = new RestockCommand
            {
                StoreId = storeId,
                ProductList = restockRequest.Select(x => new RestockModel
                {
                    ProductId = x.ProductId,
                    Amount = x.Amount
                }).ToList()
            };

            try
            {
                var response = await _mediator.Send(command);
                return Ok(response);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}
