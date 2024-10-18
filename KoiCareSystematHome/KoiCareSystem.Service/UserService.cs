﻿using AutoMapper;
using KoiCareSystem.Common;
using KoiCareSystem.Common.DTOs.Request;
using KoiCareSystem.Data;
using KoiCareSystem.Data.Models;
using KoiCareSystematHome.Service.Base;

namespace KoiCareSystem.Service
{
    public interface IUserService
    {
        Task<ServiceResult> GetAll();
        Task<ServiceResult> GetUserById(int id);
        Task<ServiceResult> GetUserByEmail(string email);
        Task<ServiceResult> Save(RequestRegisterDto registerDto);
        Task<ServiceResult> SaveByAdmin(RequestRegisterAdminDto registerAdminDto);
        Task<ServiceResult> UpdateVerifyCode(string email, string code);
        Task<ServiceResult> VerifyCode(RequestVerifyEmailDto requestVerifyEmailDto);
        Task<ServiceResult> DeleteUserByEmail(string email);
        Task<ServiceResult> DeleteUserById(int id);
    }
    public class UserService : IUserService
    {
        private readonly UnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public UserService(IMapper mapper)
        {
            _unitOfWork ??= new UnitOfWork();
            _mapper = mapper;
        }
        //Get All
        public async Task<ServiceResult> GetAll()
        {
            #region Business Rule

            #endregion Business Rule

            var Users = await _unitOfWork.UserRepository.GetAllAsync();
            if (Users == null)
            {
                return new ServiceResult(Const.WARNING_NO_DATA_CODE, Const.WARNING_NO_DATA_MSG, new List<User>());
            }
            else
            {
                return new ServiceResult(Const.SUCCESS_READ_CODE, Const.SUCCESS_READ_MSG, Users);
            }
        }
        //Get By Id
        public async Task<ServiceResult> GetUserById(int id)
        {
            #region Business Rule

            #endregion Business Rule

            var User = await _unitOfWork.UserRepository.GetByIdAsync(id);
            if (User == null)
            {
                return new ServiceResult(Const.WARNING_NO_DATA_CODE, Const.WARNING_NO_DATA_MSG);
            }
            else
            {
                return new ServiceResult(Const.SUCCESS_READ_CODE, Const.SUCCESS_READ_MSG, User);
            }
        }
        //Get By Email
        public async Task<ServiceResult> GetUserByEmail(string email)
        {
            #region Business Rule

            #endregion Business Rule

            var User = await _unitOfWork.UserRepository.GetByEmailAsync(email);
            if (User == null)
            {
                return new ServiceResult(Const.WARNING_NO_DATA_CODE, Const.WARNING_NO_DATA_MSG);
            }
            else
            {
                return new ServiceResult(Const.SUCCESS_READ_CODE, Const.SUCCESS_READ_MSG, User);
            }
        }
        //Create/Update //ROLE: Guest
        public async Task<ServiceResult> Save(RequestRegisterDto registerDto)
        {
            try
            {
                #region Business Rule

                #endregion Business Rule

                int result = -1;

                var userExisted = await this.GetUserByEmail(registerDto.Email);

                if (userExisted.Status == Const.SUCCESS_READ_CODE)
                {
                    var oldUser = (User)userExisted.Data;
                    var userToUpdate = _mapper.Map<User>(registerDto);
                    userToUpdate.Id = oldUser.Id;
                    userToUpdate.PasswordHash = registerDto.Password;
                    result = await _unitOfWork.UserRepository.UpdateAsync(userToUpdate);

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
                    var newUser = _mapper.Map<User>(registerDto);
                    newUser.PasswordHash = HashPassword(registerDto.Password);
                    newUser.EmailVerificationToken = Guid.NewGuid().ToString();

                    // Lấy role "Member" từ cơ sở dữ liệu
                    var memberRole = await _unitOfWork.RoleRepository.GetByNameAsync("Member");
                    if (memberRole == null)
                    {
                        return new ServiceResult(Const.FAIL_READ_CODE, "Role 'Member' không tồn tại.");
                    }

                    newUser.RoleId = memberRole.Id; 

                    result = await _unitOfWork.UserRepository.CreateAsync(newUser);

                    if (result > 0)
                    {
                        return new ServiceResult(Const.SUCCESS_CREATE_CODE, Const.SUCCESS_CREATE_MSG, newUser);
                    }
                    else
                    {
                        return new ServiceResult(Const.FAIL_CREATE_CODE, Const.FAIL_CREATE_MSG, newUser);
                    }
                }
            }
            catch (Exception ex)
            {
                return new ServiceResult(Const.ERROR_EXCEPTION, ex.ToString());
            }
        }
        //Create/Update //ROLE: ADMIN
        public async Task<ServiceResult> SaveByAdmin(RequestRegisterAdminDto registerAdminDto)
        {
            try
            {
                #region Business Rule

                #endregion Business Rule

                int result = -1;

                var userExisted = await this.GetUserByEmail(registerAdminDto.Email);

                if (userExisted.Status == Const.SUCCESS_READ_CODE)
                {
                    var userData = (User)userExisted.Data;
                    var userToUpdate = _mapper.Map<User>(registerAdminDto);
                    userToUpdate.Id = userData.Id;
                    userToUpdate.PasswordHash = registerAdminDto.Password;
                    result = await _unitOfWork.UserRepository.UpdateAsync(userToUpdate);

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
                    var newUser = _mapper.Map<User>(registerAdminDto);
                    newUser.PasswordHash = HashPassword(registerAdminDto.Password);

                    result = await _unitOfWork.UserRepository.CreateAsync(newUser);

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
        public async Task<ServiceResult> UpdateVerifyCode(string email,string code)
        {
            try
            {
                #region Business Rule

                #endregion Business Rule

                int result = -1;

                var userExisted = await this.GetUserByEmail(email);

                if (userExisted.Status == Const.SUCCESS_READ_CODE)
                {
                    var userData = (User)userExisted.Data;
                    userData.EmailVerificationToken = code;
                    result = await _unitOfWork.UserRepository.UpdateAsync(userData);

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
                    return new ServiceResult(Const.WARNING_NO_DATA_CODE, Const.WARNING_NO_DATA_MSG);
                }
            }
            catch (Exception ex)
            {
                return new ServiceResult(Const.ERROR_EXCEPTION, ex.ToString());
            }
        }
        
        //Delete by Id, Email
        public async Task<ServiceResult> DeleteUserById(int id)
        {
            try
            {
                var result = false;

                var removeUser = this.GetUserById(id);

                if (removeUser != null && removeUser.Result.Status == Const.SUCCESS_READ_CODE)
                {
                    result = await _unitOfWork.UserRepository.RemoveAsync((User)removeUser.Result.Data);

                    if (result)
                    {
                        return new ServiceResult(Const.SUCCESS_DELETE_CODE, Const.SUCCESS_DELETE_MSG, result);
                    }
                    else
                    {
                        return new ServiceResult(Const.FAIL_DELETE_CODE, Const.FAIL_DELETE_MSG, removeUser.Result.Data);
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
        public async Task<ServiceResult> DeleteUserByEmail(string email)
        {
            try
            {
                var result = false;

                var removeUser = this.GetUserByEmail(email);

                if (removeUser != null && removeUser.Result.Status == Const.SUCCESS_READ_CODE)
                {
                    result = await _unitOfWork.UserRepository.RemoveAsync((User)removeUser.Result.Data);

                    if (result)
                    {
                        return new ServiceResult(Const.SUCCESS_DELETE_CODE, Const.SUCCESS_DELETE_MSG, result);
                    }
                    else
                    {
                        return new ServiceResult(Const.FAIL_DELETE_CODE, Const.FAIL_DELETE_MSG, removeUser.Result.Data);
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
        public async Task<ServiceResult> VerifyCode(RequestVerifyEmailDto requestVerifyEmailDto)
        {
            int result = -1;

            var userExisted = await this.GetUserByEmail(requestVerifyEmailDto.Email);
            if (userExisted.Status == Const.SUCCESS_READ_CODE)
            {
                var user = (User)userExisted.Data;
                if (user.IsVerified == true)
                {
                    return new ServiceResult(Const.WARNING_NO_DATA_CODE, Const.WARNING_NO_DATA_MSG);
                }
                if (user.EmailVerificationToken.ToLower().Contains((requestVerifyEmailDto.Code).ToLower()))
                {
                    user.IsVerified = true;
                    result = await _unitOfWork.UserRepository.UpdateAsync(user);
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
                    return new ServiceResult(Const.WARNING_NO_DATA_CODE, Const.WARNING_NO_DATA_MSG);
                }
            }
            else
            {
                return new ServiceResult(Const.FAIL_READ_CODE, Const.FAIL_READ_MSG);
            }
        }

        //Kiểm tra User có tồn tại chưa
        public bool UserIdExists(int id)
        {
            return _unitOfWork.UserRepository.UserExists(id);
        }
        public bool UserEmailExists(string email)
        {
            return _unitOfWork.UserRepository.UserExists(email);
        }
        //Helper
        private string HashPassword(string password)
        {
            // Thực hiện băm mật khẩu (sử dụng phương pháp băm phù hợp)
            return password; // Thay đổi thành phương pháp băm thực tế
        }
    }
}
