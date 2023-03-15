using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Turnit.GenericStore.Api.Data.Repositories;

namespace Turnit.GenericStore.Api.Features.Sales;

[Route("categories")]
public class CategoriesController : ApiControllerBase
{
    private readonly ICategoryRepository _categoryRepository;

    public CategoriesController(ICategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }

    [HttpGet, Route("")]
    public async Task<CategoryModel[]> AllCategories()
    {
        var categories = await _categoryRepository.GetAll();

        var result = categories
            .Select(x => new CategoryModel
            {
                Id = x.Id,
                Name = x.Name
            })
            .ToArray();

        return result;
    }

    [HttpPost, Route("")]
    public async Task<IActionResult> Save(string name)
    {
        await _categoryRepository.Save(name);
        return Ok();
    }
}