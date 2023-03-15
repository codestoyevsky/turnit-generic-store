using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Turnit.GenericStore.Api.Data.Repositories;

namespace Turnit.GenericStore.Api.Features.Sales;

[Route("products")]
public class ProductsController : ApiControllerBase
{
    private readonly IProductRepository _productRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly IProductCategoryRepository _productCategoryRepository;
    private readonly IProductAvailabilityRepository _productAvailabilityRepository;

    public ProductsController(IProductRepository productRepository, ICategoryRepository categoryRepository, IProductCategoryRepository productCategoryRepository, IProductAvailabilityRepository productAvailabilityRepository)
    {
        _productRepository = productRepository;
        _categoryRepository = categoryRepository;
        _productCategoryRepository = productCategoryRepository;
        _productAvailabilityRepository = productAvailabilityRepository;
    }

    [HttpGet, Route("by-category/{categoryId:guid}")]
    public async Task<ProductModel[]> ProductsByCategory(Guid categoryId)
    {
        var products = await _productRepository.GetAllByCategoryId(categoryId);
        var productAvailabilities = await _productAvailabilityRepository.GetAll(products.Select(p => p.Id).ToList());

        var result = productAvailabilities
            .GroupBy(pa => pa.Product.Id)
            .Select(g => new ProductModel
            {
                Id = g.Key,
                Name = g.First().Product.Name,
                Availability = g.Select(pa => new ProductModel.AvailabilityModel
                {
                    StoreId = pa.Store.Id,
                    Availability = pa.Availability
                }).ToArray()
            }).ToArray();

        return result;
    }

    [HttpGet, Route("")]
    public async Task<ProductCategoryModel[]> AllProducts()
    {
        var products = await _productRepository.GetAll();
        var productAvailibilities = await _productAvailabilityRepository.GetAll(products.Select(x => x.Id).ToList());
        var productCategories = await _productCategoryRepository.GetAll(products.Select(x => x.Id));

        var result = new List<ProductCategoryModel>();

        foreach (var categoryGroup in productCategories.GroupBy(x => x.Category.Id))
        {
            var category = categoryGroup.FirstOrDefault().Category;
            var productModels = categoryGroup.Select(x => new ProductModel
            {
                Id = x.Product.Id,
                Name = x.Product.Name,
                Availability = productAvailibilities.Where(z => z.Product.Id == x.Product.Id).Select(y => new ProductModel.AvailabilityModel
                {
                    StoreId = y.Store.Id,
                    Availability = y.Availability
                }).ToArray()
            }).ToArray();

            result.Add(new ProductCategoryModel
            {
                CategoryId = category.Id,
                Products = productModels
            });
        }

        var unCategorizedProducts = new List<ProductModel>();

        foreach (var product in products.Where(x => productCategories.Select(x => x.Product.Id).Contains(x.Id) == false))
        {
            unCategorizedProducts.Add(new ProductModel
            {
                Id = product.Id,
                Name = product.Name,
                Availability = productAvailibilities.Where(x => x.Product.Id == product.Id).Select(y => new ProductModel.AvailabilityModel
                {
                    StoreId = y.Store.Id,
                    Availability = y.Availability
                }).ToArray()
            });
        }

        if (unCategorizedProducts.Any())
        {
            result.Add(new ProductCategoryModel
            {
                CategoryId = null,
                Products = unCategorizedProducts.ToArray()
            });
        }

        return result.ToArray();
    }

    [HttpPost, Route("")]
    public async Task<IActionResult> Save(string name, string description)
    {
        await _productRepository.Save(name, description);
        return Ok();
    }

    [HttpPut, Route("{productId:guid}/category/{categoryId:guid}")]
    public async Task<IActionResult> AddProductToCategory(Guid productId, Guid categoryId)
    {
        var product = await _productRepository.GetById(productId);
        if (product == null)
        {
            return NotFound($"Prodcut not found, {productId}!");
        }

        var category = await _categoryRepository.GetById(categoryId);
        if (category == null)
        {
            return NotFound($"Category not found, {categoryId}!");
        }

        await _productCategoryRepository.AddProduct(productId, categoryId);

        return Ok();
    }

    [HttpDelete, Route("{productId:guid}/categories/{categoryId:guid}")]
    public async Task<IActionResult> RemoveProductFromCategory(Guid productId, Guid categoryId)
    {
        var product = await _productRepository.GetById(productId);
        if (product == null)
        {
            return NotFound($"Product not found, {productId}");
        }

        var category = await _categoryRepository.GetById(categoryId);
        if (category == null)
        {
            return NotFound($"Category not found, {categoryId}");
        }

        await _productCategoryRepository.RemoveProduct(productId, categoryId);

        return Ok();
    }

    [HttpPost, Route("{productId:guid}/book")]
    public async Task<IActionResult> BookProduct(Guid productId, List<BookProductRequestModel> request)
    {
        _productAvailabilityRepository.Book(productId, request.ToDictionary(x => x.StoreId, x => x.Quantity));
        return Ok();
    }
}