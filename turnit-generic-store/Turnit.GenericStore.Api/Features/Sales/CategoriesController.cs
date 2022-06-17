using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NHibernate;
using NHibernate.Linq;
using Turnit.GenericStore.Api.Entities;

namespace Turnit.GenericStore.Api.Features.Sales;

[Route("categories")]
public class CategoriesController : ApiControllerBase
{
    private readonly ISession _session;

    public CategoriesController(ISession session)
    {
        _session = session;
    }
    
    [HttpGet, Route("")]
    public async Task<List<CategoryModel>> AllCategories()
    {
        var result = await _session.Query<Category>()
            .Select(x => new CategoryModel
            {
                Id = x.Id,
                Name = x.Name
            })
            .ToListAsync();
        
        return result;
    }
}