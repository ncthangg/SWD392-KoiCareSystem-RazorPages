﻿using KoiCareSystem.Common;
using KoiCareSystem.Data.Models;
using KoiCareSystem.Data;
using KoiCareSystem.Service.Base;

namespace KoiCareSystem.Service
{
    public interface ICategoryService
    {
        Task<ServiceResult> GetAll();
        Task<ServiceResult> GetById(int id);
        Task<ServiceResult> Save(Category category);
        Task<ServiceResult> DeleteById(int id);
    }
    public class CategoryService : ICategoryService
    {
        private readonly UnitOfWork _unitOfWork;
        public CategoryService()
        {
            _unitOfWork ??= new UnitOfWork();
        }
        //Get All
        public async Task<ServiceResult> GetAll()
        {
            #region Business Rule

            #endregion Business Rule

            var Categorys = await _unitOfWork.CategoryRepository.GetAllAsync();
            if (Categorys == null)
            {
                return new ServiceResult(Const.WARNING_NO_DATA_CODE, Const.WARNING_NO_DATA_MSG, new List<Category>());
            }
            else
            {
                return new ServiceResult(Const.SUCCESS_READ_CODE, Const.SUCCESS_READ_MSG, Categorys);
            }
        }
        //Get By Id
        public async Task<ServiceResult> GetById(int id)
        {
            #region Business Rule

            #endregion Business Rule

            var Category = await _unitOfWork.CategoryRepository.GetByIdAsync(id);
            if (Category == null)
            {
                return new ServiceResult(Const.WARNING_NO_DATA_CODE, Const.WARNING_NO_DATA_MSG);
            }
            else
            {
                return new ServiceResult(Const.SUCCESS_READ_CODE, Const.SUCCESS_READ_MSG, Category);
            }
        }
        //Get By Name
        public async Task<ServiceResult> GetCategoryByName(string name)
        {
            #region Business Rule

            #endregion Business Rule

            var Category = await _unitOfWork.CategoryRepository.GetByNameAsync(name);
            if (Category == null)
            {
                return new ServiceResult(Const.WARNING_NO_DATA_CODE, Const.WARNING_NO_DATA_MSG);
            }
            else
            {
                return new ServiceResult(Const.SUCCESS_READ_CODE, Const.SUCCESS_READ_MSG, Category);
            }
        }

        //Create/Update
        public async Task<ServiceResult> Save(Category category)
        {
            try
            {
                #region Business Rule

                #endregion Business Rule

                int result = -1;

                var item = this.GetCategoryByName(category.Name);

                if (item.Result.Status == Const.SUCCESS_READ_CODE)
                {
                    result = await _unitOfWork.CategoryRepository.UpdateAsync(category);
                    if (result > 0)
                    {
                        return new ServiceResult(Const.SUCCESS_UPDATE_CODE, Const.SUCCESS_UPDATE_MSG);
                    }
                    else
                    {
                        return new ServiceResult(Const.FAIL_UPDATE_CODE, Const.FAIL_UPDATE_MSG);
                    }
                }
                else
                {
                    result = await _unitOfWork.CategoryRepository.CreateAsync(category);
                    if (result > 0)
                    {
                        return new ServiceResult(Const.SUCCESS_CREATE_CODE, Const.SUCCESS_CREATE_MSG);
                    }
                    else
                    {
                        return new ServiceResult(Const.FAIL_CREATE_CODE, Const.FAIL_CREATE_MSG);
                    }
                }
            }
            catch (Exception ex)
            {
                return new ServiceResult(Const.ERROR_EXCEPTION, ex.ToString());
            }
        }
        //Delete by Id
        public async Task<ServiceResult> DeleteById(int id)
        {
            try
            {
                var result = false;

                var removeCategory = this.GetById(id);

                if (removeCategory != null && removeCategory.Result.Status == Const.SUCCESS_READ_CODE)
                {
                    result = await _unitOfWork.CategoryRepository.RemoveAsync((Category)removeCategory.Result.Data);

                    if (result)
                    {
                        return new ServiceResult(Const.SUCCESS_DELETE_CODE, Const.SUCCESS_DELETE_MSG, result);
                    }
                    else
                    {
                        return new ServiceResult(Const.FAIL_DELETE_CODE, Const.FAIL_DELETE_MSG, removeCategory.Result.Data);
                    }
                }
                else
                {
                    return new ServiceResult(Const.WARNING_NO_DATA_CODE, Const.WARNING_NO_DATA_MSG);
                }
            }
            catch (Exception ex)
            {
                return new ServiceResult(Const.ERROR_EXCEPTION, ex.ToString());
            }
        }
    }
}
