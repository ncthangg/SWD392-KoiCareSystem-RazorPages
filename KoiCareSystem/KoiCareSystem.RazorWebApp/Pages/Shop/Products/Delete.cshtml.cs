﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using KoiCareSystem.Data.Models;
using KoiCareSystem.Service;
using AutoMapper;
using KoiCareSystem.RazorWebApp.PageBase;

namespace KoiCareSystem.RazorWebApp.Pages.Shop.Products
{
    public class DeleteModel : BasePageModel
    {
        private readonly ProductService _productService;
        private readonly CategoryService _categoryService;

        public DeleteModel(IMapper mapper)
        {
            _productService ??= new ProductService(mapper);
            _categoryService ??= new CategoryService();
        }
        //========================================================
        [BindProperty]
        public Product Product { get; set; } = default!;
        //========================================================

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var product = await _productService.GetById((int)id) ;
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

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _productService.GetById((int)id);
            if (product != null)
            {
                Product = (Product)product.Data;
                await _productService.DeleteById((int)id);
            }

            return RedirectToPage("./Index");
        }
    }
}
