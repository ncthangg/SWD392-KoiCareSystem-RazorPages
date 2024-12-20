﻿using Microsoft.AspNetCore.Mvc;
using KoiCareSystem.Data.Models;
using KoiCareSystem.Service;
using AutoMapper;
using KoiCareSystem.RazorWebApp.PageBase;

namespace KoiCareSystem.RazorWebApp.Pages.Shop.Products
{
    public class DetailsModel : BasePageModel
    {
        private readonly ProductService _productService;
        private readonly CategoryService _categoryService;

        public DetailsModel(IMapper mapper)
        {
            _productService ??= new ProductService(mapper);
            _categoryService ??= new CategoryService();
        }
        //========================================================
        public Product Product { get; set; } = default!;

        //========================================================
        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _productService.GetById((int)id);
            if (product == null)
            {
                return NotFound();
            }
            else
            {
                Product = (Product)product.Data;
            }
            return Page();
        }
    }
}
