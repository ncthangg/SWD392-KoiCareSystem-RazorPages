﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using KoiCareSystem.Data.DBContext;
using KoiCareSystem.Data.Models;
using KoiCareSystem.Common.DTOs;
using KoiCareSystem.Service;
using KoiCareSystem.Common;
using KoiCareSystem.RazorWebApp.PageBase;

namespace KoiCareSystem.RazorWebApp.Pages.Member.PondPages
{
    public class DeleteModel : BasePageModel
    {
        private readonly PondService _pondService;

        public DeleteModel(PondService pondService)
        {
            _pondService = pondService;
        }
        //========================================================
        [BindProperty]
        public Pond Pond { get; set; } = default!;
        public string ErrorMessage { get; set; }
        //========================================================
        public async Task<IActionResult> OnGetAsync(int id)
        {
            LoadUserIdFromSession();

            if (UserId == null)
            {
                return RedirectToPage("/Guest/Login"); // Điều hướng đến trang đăng nhập nếu không có UserId trong session
            }

            var pond = await _pondService.GetById(id);

            var result = (Pond)pond.Data;

            if (result == null)
            {
                return NotFound();
            }
            else
            {
                Pond = result;
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var pond = await _pondService.GetById(id);
            var pondData = (Pond)pond.Data;

            if (pondData != null)
            {
                Pond = pondData;
                var result = await _pondService.DeleteById(id);
                if (result.Status != Const.SUCCESS_DELETE_CODE)
                {
                    /// Xóa không thành công, gán thông báo lỗi để hiển thị ra UI
                    ErrorMessage = result.Message;
                    ModelState.AddModelError(string.Empty, result.Message);
                    return Page();
                }
                else
                {
                    return RedirectToPage("./Index");

                }
            }

            return RedirectToPage("./Delete");
        }
    }
}
