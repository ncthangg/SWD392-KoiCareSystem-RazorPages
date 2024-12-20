﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using KoiCareSystem.Data.DBContext;
using KoiCareSystem.Data.Models;
using KoiCareSystem.Service;
using KoiCareSystem.Common.DTOs;
using KoiCareSystem.RazorWebApp.PageBase;

namespace KoiCareSystem.RazorWebApp.Pages.Member.KoiFishPages
{
    public class DeleteModel : BasePageModel
    {

        private readonly KoiFishService _koiFishService;

        public DeleteModel(KoiFishService koiFishService)
        {
            _koiFishService = koiFishService;
        }
        //========================================================
        [BindProperty]
        public KoiFish KoiFish { get; set; } = default!;
        //========================================================
        public async Task<IActionResult> OnGetAsync(int id)
        {
            LoadUserIdFromSession();

            if (UserId == null)
            {
                return RedirectToPage("/Guest/Login"); // Điều hướng đến trang đăng nhập nếu không có UserId trong session
            }

            var koifish = await _koiFishService.GetById(id);

            var result = (KoiFish)koifish.Data;

            if (result == null)
            {
                return NotFound();
            }
            else
            {
                KoiFish = result;
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var koifish = await _koiFishService.GetById(id);
            var result = (KoiFish)koifish.Data;

            if (result != null)
            {
                KoiFish = result;
                _koiFishService.DeleteById(id);
                //_context.KoiFishes.Remove(KoiFish);
                //await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
