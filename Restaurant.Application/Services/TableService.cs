using Microsoft.EntityFrameworkCore;

using Restaurant.Application.Contract;
using Restaurant.Application.DTOS.Admin;
using Restaurant.Application.DTOS.Common;
using Restaurant.Application.DTOS.Customer;
using Restaurant.Application.DTOS.Staff;
using Restaurant.Application.Interfaces;
using Restaurant.Domain.Entities;
using Restaurant.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restaurant.Application.Services
{
    public class TableService : ITableService
    {
        private readonly IUnitOfWork _unitOfWork;

        public TableService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // Admin - Get All Tables
        public async Task<ApiResponseDto<PagedResultDto<AdminTableDto>>> GetAllTablesAsync(PaginationDto pagination)
        {
            var query = _unitOfWork.Table.GetAll();
            var total = await query.CountAsync();

            var tables = await query
                .Skip(pagination.Skip)
                .Take(pagination.PageSize)
                .ToListAsync();

            var tableDtos = tables.Select(t => new AdminTableDto
            {
                Id = t.Id,
                TableNumber = t.TableNumber,
                Capacity = t.Capacity,
                IsAvailable = t.IsAvailable,
                Location = t.Location,
                CreatedAt = t.CreatedAt
            }).ToList();

            var result = new PagedResultDto<AdminTableDto>
            {
                Items = tableDtos,
                TotalCount = total,
                PageNumber = pagination.PageNumber,
                PageSize = pagination.PageSize
            };

            return ApiResponseDto<PagedResultDto<AdminTableDto>>.SuccessResponse(result);
        }

        // Admin - Create Table
        public async Task<ApiResponseDto<AdminTableDto>> CreateTableAsync(AdminCreateTableDto dto)
        {
            var existing = await _unitOfWork.Table.GetTableByNumberAsync(dto.TableNumber);
            if (existing != null)
            {
                return ApiResponseDto<AdminTableDto>.ErrorResponse("Table number already exists");
            }

            var table = new Table
            {
                TableNumber = dto.TableNumber,
                Capacity = dto.Capacity,
                Location = dto.Location,
                IsAvailable = dto.IsAvailable
            };

            await _unitOfWork.Table.Create(table);
            await _unitOfWork.SaveChangesAsync();

            var result = new AdminTableDto
            {
                Id = table.Id,
                TableNumber = table.TableNumber,
                Capacity = table.Capacity,
                IsAvailable = table.IsAvailable,
                Location = table.Location,
                CreatedAt = table.CreatedAt
            };

            return ApiResponseDto<AdminTableDto>.SuccessResponse(result, "Table created successfully");
        }

        // Admin - Update Table
        public async Task<ApiResponseDto<AdminTableDto>> UpdateTableAsync(AdminUpdateTableDto dto)
        {
            var table = await _unitOfWork.Table.GetById(dto.Id);
            if (table == null)
            {
                return ApiResponseDto<AdminTableDto>.ErrorResponse("Table not found");
            }

            table.TableNumber = dto.TableNumber;
            table.Capacity = dto.Capacity;
            table.Location = dto.Location;
            table.IsAvailable = dto.IsAvailable;

            _unitOfWork.Table.Update(table);
            await _unitOfWork.SaveChangesAsync();

            var result = new AdminTableDto
            {
                Id = table.Id,
                TableNumber = table.TableNumber,
                Capacity = table.Capacity,
                IsAvailable = table.IsAvailable,
                Location = table.Location,
                CreatedAt = table.CreatedAt
            };

            return ApiResponseDto<AdminTableDto>.SuccessResponse(result, "Table updated successfully");
        }

        // Admin - Delete Table
        public async Task<ApiResponseDto<bool>> DeleteTableAsync(int id)
        {
            var table = await _unitOfWork.Table.GetById(id);
            if (table == null)
            {
                return ApiResponseDto<bool>.ErrorResponse("Table not found");
            }

            await _unitOfWork.Table.Delete(id);
            await _unitOfWork.SaveChangesAsync();

            return ApiResponseDto<bool>.SuccessResponse(true, "Table deleted successfully");
        }

        // Customer - Get Available Tables
        public async Task<ApiResponseDto<List<AvailableTableDto>>> GetAvailableTablesForCustomerAsync()
        {
            var tables = await _unitOfWork.Table.GetAvailableTablesAsync();

            var result = tables.Select(t => new AvailableTableDto
            {
                Id = t.Id,
                TableNumber = t.TableNumber,
                Capacity = t.Capacity,
                Location = t.Location,
                IsAvailable = t.IsAvailable
            }).ToList();

            return ApiResponseDto<List<AvailableTableDto>>.SuccessResponse(result);
        }

        // Staff - Get Tables
        public async Task<ApiResponseDto<List<StaffTableDto>>> GetTablesForStaffAsync()
        {
            var tables = await _unitOfWork.Table.GetAll()
                .Include(t => t.Orders.Where(o => o.Status != OrderStatus.Delivered && o.Status != OrderStatus.Cancelled))
                .ToListAsync();

            var result = tables.Select(t =>
            {
                var currentOrder = t.Orders.FirstOrDefault();
                return new StaffTableDto
                {
                    Id = t.Id,
                    TableNumber = t.TableNumber,
                    Capacity = t.Capacity,
                    IsAvailable = t.IsAvailable,
                    Location = t.Location,
                    CurrentOrderId = currentOrder?.Id,
                    CurrentOrderNumber = currentOrder?.OrderNumber,
                    CurrentOrderStatus = currentOrder?.Status.ToString()
                };
            }).ToList();

            return ApiResponseDto<List<StaffTableDto>>.SuccessResponse(result);
        }

        // Staff - Update Table Availability
        public async Task<ApiResponseDto<bool>> UpdateTableAvailabilityAsync(UpdateTableAvailabilityDto dto)
        {
            var table = await _unitOfWork.Table.GetById(dto.TableId);
            if (table == null)
            {
                return ApiResponseDto<bool>.ErrorResponse("Table not found");
            }

            table.IsAvailable = dto.IsAvailable;
            _unitOfWork.Table.Update(table);
            await _unitOfWork.SaveChangesAsync();

            return ApiResponseDto<bool>.SuccessResponse(true, "Table availability updated");
        }
    }
}
