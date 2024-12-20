﻿using AutoMapper;
using KoiCareSystem.Common.DTOs.Request;
using KoiCareSystem.Common;
using KoiCareSystem.Data.Models;
using KoiCareSystem.Data;
using KoiCareSystem.Service.Base;
namespace KoiCareSystem.Service
{
    public interface IOrderItemService
    {
        Task<ServiceResult> GetAllItemInOrder(int orderId);
        Task<ServiceResult> GetById(int id);
        Task<ServiceResult> GetItemInOrderByProductId(int productId);
        Task<ServiceResult> AddItemToOrder(RequestItemToOrderDto requestItemToOrderDto);
        Task<ServiceResult> DeleteItem(int id);
    }
    public class OrderItemService : IOrderItemService
    {
        private readonly UnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public OrderItemService(IMapper mapper)
        {
            _unitOfWork ??= new UnitOfWork();
            _mapper = mapper;
        }

        //Get All Item in Order
        public async Task<ServiceResult> GetAllItemInOrder(int orderId)
        {
            var orderItems = await _unitOfWork.OrderItemRepository.GetByOrderIdAsync(orderId);
            if (orderItems == null)
            {
                return new ServiceResult(Const.WARNING_NO_DATA_CODE, Const.WARNING_NO_DATA_MSG, new List<OrderItem>());
            }
            else
            {
                return new ServiceResult(Const.SUCCESS_READ_CODE, Const.SUCCESS_READ_MSG, orderItems);
            }
        }
        //Get Item by Id
        public async Task<ServiceResult> GetById(int id)
        {
            var orderItems = await _unitOfWork.OrderItemRepository.GetByIdAsync(id);
            if (orderItems == null)
            {
                return new ServiceResult(Const.WARNING_NO_DATA_CODE, Const.WARNING_NO_DATA_MSG, new List<OrderItem>());
            }
            else
            {
                return new ServiceResult(Const.SUCCESS_READ_CODE, Const.SUCCESS_READ_MSG, orderItems);
            }
        }
        public async Task<ServiceResult> GetItemInOrderByProductId(int productId)
        {
            var orderItems = await _unitOfWork.OrderItemRepository.GetByProductIdAsync(productId);
            if (orderItems == null)
            {
                return new ServiceResult(Const.WARNING_NO_DATA_CODE, Const.WARNING_NO_DATA_MSG, new List<OrderItem>());
            }
            else
            {
                return new ServiceResult(Const.SUCCESS_READ_CODE, Const.SUCCESS_READ_MSG, orderItems);
            }
        }
        public async Task<ServiceResult> AddItemToOrder(RequestItemToOrderDto requestItemToOrderDto)
        {
            var order = await _unitOfWork.OrderRepository.GetByIdAsync((int)requestItemToOrderDto.OrderId);
            var product = await _unitOfWork.ProductRepository.GetByIdAsync((int)requestItemToOrderDto.ProductId);

            // Kiểm tra nếu order hoặc product không tồn tại
            if (order == null || product == null)
            {
                return new ServiceResult(Const.WARNING_NO_DATA_CODE, Const.WARNING_NO_DATA_MSG);
            }
            if(order.StatusId != 1)
            {
                return new ServiceResult(Const.WARNING_NO_DATA_CODE, Const.WARNING_NO_DATA_MSG);
            }
            try
            {
                // Kiểm tra xem sản phẩm đã tồn tại trong Order hay chưa
                var exist = this.ProductInOrderExists((int)requestItemToOrderDto.OrderId, (int)requestItemToOrderDto.ProductId);
               
                if (exist) //tồn tại
                {        
                    var orderItem = _mapper.Map<OrderItem>(requestItemToOrderDto);

                    var itemExist = await _unitOfWork.OrderItemRepository.GetItemByOrderIdAndProductIdAsync((int)requestItemToOrderDto.OrderId, (int)requestItemToOrderDto.ProductId);

                    itemExist.Quantity += orderItem.Quantity;
                    itemExist.Price += orderItem.Quantity * (int)product.Price;
                    // Cập nhật OrderItem trong cơ sở dữ liệu
                    await _unitOfWork.OrderItemRepository.UpdateAsync(itemExist);
                }
                else
                {
                    var orderItem = _mapper.Map<OrderItem>(requestItemToOrderDto);
                    // Tạo mới OrderItem
                    orderItem.Price = orderItem.Quantity * (int)product.Price;

                    // Thêm OrderItem vào cơ sở dữ liệu
                    await _unitOfWork.OrderItemRepository.CreateAsync(orderItem);
                }

                // Cập nhật thông tin Order
                return await UpdateOrder(order);
            }
            catch (Exception ex)
            {
                // Ghi lại lỗi để hỗ trợ gỡ lỗi
                return new ServiceResult(Const.ERROR_EXCEPTION, ex.ToString());
            }
        }

        //Delete Item in Order
        public async Task<ServiceResult> DeleteItem(int id)
        {
            try
            {
                var result = false;

                var itemRemove = await this.GetById(id);

                if (itemRemove != null && itemRemove.Status == Const.SUCCESS_READ_CODE)
                {
                    result = await _unitOfWork.OrderItemRepository.RemoveAsync((OrderItem)itemRemove.Data);

                    if (result)
                    {
                        var item = (OrderItem)itemRemove.Data;

                        // Lấy thông tin Order sau khi xóa Item
                        var order = await _unitOfWork.OrderRepository.GetByOrderIdAsync(item.OrderId);
                        if (order != null)
                        {
                            // Cập nhật lại Order bằng cách sử dụng hàm UpdateOrder
                            var updateResult = await UpdateOrder(order);
                            if (updateResult.Status == Const.SUCCESS_UPDATE_CODE)
                            {
                                return new ServiceResult(Const.SUCCESS_UPDATE_CODE, Const.SUCCESS_CREATE_MSG, order);
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
                        return new ServiceResult(Const.FAIL_DELETE_CODE, Const.FAIL_DELETE_MSG);
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

        //Update Order
        private async Task<ServiceResult> UpdateOrder(Order order)
        {
            // Lấy tất cả các OrderItem trong Order hiện tại
            var orderItems = await _unitOfWork.OrderItemRepository.GetByOrderIdAsync(order.OrderId);
            // Cập nhật tổng số lượng và tổng giá cho Order
            order.Quantity = orderItems.Sum(oi => oi.Quantity);
            order.TotalPrice = orderItems.Sum(oi => oi.Price);

            // Lưu thay đổi vào cơ sở dữ liệu
            var updateResult = await _unitOfWork.OrderRepository.UpdateAsync(order);
            if (updateResult > 0)
            {
                return new ServiceResult(Const.SUCCESS_UPDATE_CODE, Const.SUCCESS_UPDATE_MSG);
            }
            else
            {
                return new ServiceResult(Const.FAIL_UPDATE_CODE, Const.FAIL_UPDATE_MSG);
            }
        }
        //Helper
        public bool ProductInOrderExists(int orderId, int productId)
        {
            return _unitOfWork.OrderItemRepository.ProductExists(orderId, productId);
        }
    }
}
