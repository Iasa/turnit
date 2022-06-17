using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using NHibernate;
using NHibernate.Linq;
using Turnit.GenericStore.Api.Commands;
using Turnit.GenericStore.Api.Dtos;
using Turnit.GenericStore.Api.Entities;

namespace Turnit.GenericStore.Api.Features.Sales;

[Route("products")]
public class ProductsController : ApiControllerBase
{
    private readonly ISession _session;
    private readonly IMediator _mediator;

    public ProductsController(ISession session, IMediator mediator)
    {
        _session = session;
        _mediator = mediator;
    }
    
    [HttpGet, Route("by-category/{categoryId:guid}")]
    public async Task<IEnumerable<ProductModel>> ProductsByCategory(Guid categoryId)
    {
        var products = _session.Query<ProductCategory>()
            .Where(x => x.Category.Id == categoryId)
            .Select(x => x.Product);

        var groupedProductAvailability = await _session.Query<ProductAvailability>()
            .Where(x => products.Contains(x.Product))
            .GroupBy(x => x.Product)
            .ToListAsync();

        var result = groupedProductAvailability
            .Select(x => new ProductModel
            {
                Id = x.Key.Id,
                Name = x.Key.Name,
                Availability = x.Select(a => new ProductModel.AvailabilityModel
                {
                    StoreId = a.Store.Id,
                    Availability = a.Availability
                }).ToArray()
            });
        
        return result;
    }
    
    [HttpGet, Route("")]
    public async Task<ProductCategoryModel[]> AllProducts()
    {        
        var products = _session.Query<Product>();
        var productCategories = _session.Query<ProductCategory>();
        var ava = _session.Query<ProductAvailability>();

        var productCategory = await products.LeftJoin(productCategories,
            p => p.Id,
            pc => pc.Product.Id,
            (p, pc) => new ProductCategory
            {
                Product = p,
                Category = pc.Category
            })
            .ToListAsync();

        var groupedProductCategory = productCategory.GroupBy(x => x.Category);

        var groupedProductAvailability = await _session.Query<ProductAvailability>()
            .GroupBy(x => x.Product)
            .ToListAsync();

        var productModelList = groupedProductAvailability
            .Select(x => new ProductModel
            {
                Id = x.Key.Id,
                Name = x.Key.Name,
                Availability = x.Select(av => new ProductModel.AvailabilityModel
                {
                    StoreId = av.Store.Id,
                    Availability = av.Availability
                }).ToArray()
            });

        var result = groupedProductCategory.Select(x => new ProductCategoryModel
        {
            CategoryId = x.Key?.Id,
            Products = productModelList.Where(p => x.Any(z => z.Product.Id == p.Id)).ToArray()
        }).ToList();

        return result.ToArray();
    }

    [HttpPut, Route("{productId}/category/{categoryId}")]
    public async Task<ActionResult<string>> AddProductToCategory(Guid productId, Guid categoryId)
    {
        var command = new AddProductToCategoryCommand
        {
            ProductId = productId,
            CategoryId = categoryId
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

    [HttpDelete, Route("{productId}/category/{categoryId}")]
    public async Task<ActionResult<string>> RemoveProductFromCategory(Guid productId, Guid categoryId)
    {
        var command = new RemoveProductFromCategoryCommand
        {
            ProductId = productId,
            CategoryId = categoryId
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

    [HttpPost, Route("{productId}/book")]
    public async Task<ActionResult<string>> BookProduct(Guid productId, [FromBody] List<BookProductRequest> bookProductRequest)
    {
        var command = new BookProductCommand
        {
            ProductsToBook = bookProductRequest.Select(x => new Commands.BookProductModel
            {
                StoreId = x.StoreId,
                Amount = x.Amount,
                ProductId = productId
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