using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Restaurant.Application.DTOS.Admin;
using Restaurant.Application.DTOS.Common;
using Restaurant.Application.Interfaces;
using Restaurant.Domain.Entities;

namespace Restaurant.Application.Services
{
    public class UserManagementService : IUserManagementService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UserManagementService(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<ApiResponseDto<PagedResultDto<AdminUserDto>>> GetAllUsersAsync(PaginationDto pagination)
        {
            var query = _userManager.Users.AsQueryable();
            var total = await query.CountAsync();

            var users = await query
                .OrderByDescending(u => u.CreatedAt)
                .Skip(pagination.Skip)
                .Take(pagination.PageSize)
                .ToListAsync();

            var userDtos = new List<AdminUserDto>();
            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                userDtos.Add(new AdminUserDto
                {
                    Id = user.Id,
                    Email = user.Email ?? string.Empty,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    PhoneNumber = user.PhoneNumber ?? string.Empty,
                    Address = user.Address,
                    PreferredLanguage = user.PreferredLanguage,
                    Roles = roles.ToList(),
                    IsDeleted = user.IsDeleted,
                    CreatedAt = user.CreatedAt,
                    UpdatedAt = user.UpdatedAt,
                    TotalOrders = user.Orders?.Count ?? 0,
                    TotalSpent = user.Orders?.Sum(o => o.Total) ?? 0
                });
            }

            var result = new PagedResultDto<AdminUserDto>
            {
                Items = userDtos,
                TotalCount = total,
                PageNumber = pagination.PageNumber,
                PageSize = pagination.PageSize
            };

            return ApiResponseDto<PagedResultDto<AdminUserDto>>.SuccessResponse(result);
        }

        public async Task<ApiResponseDto<AdminUserDto>> GetUserByIdAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return ApiResponseDto<AdminUserDto>.ErrorResponse("User not found");

            var roles = await _userManager.GetRolesAsync(user);

            var dto = new AdminUserDto
            {
                Id = user.Id,
                Email = user.Email ?? string.Empty,
                FirstName = user.FirstName,
                LastName = user.LastName,
                PhoneNumber = user.PhoneNumber ?? string.Empty,
                Address = user.Address,
                PreferredLanguage = user.PreferredLanguage,
                Roles = roles.ToList(),
                IsDeleted = user.IsDeleted,
                CreatedAt = user.CreatedAt,
                UpdatedAt = user.UpdatedAt,
                TotalOrders = user.Orders?.Count ?? 0,
                TotalSpent = user.Orders?.Sum(o => o.Total) ?? 0
            };

            return ApiResponseDto<AdminUserDto>.SuccessResponse(dto);
        }

        public async Task<ApiResponseDto<AdminUserDto>> CreateUserAsync(AdminCreateUserDto dto)
        {
            var existingUser = await _userManager.FindByEmailAsync(dto.Email);
            if (existingUser != null)
                return ApiResponseDto<AdminUserDto>.ErrorResponse("Email is already registered");

            var user = new ApplicationUser
            {
                UserName = dto.Email,
                Email = dto.Email,
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                PhoneNumber = dto.PhoneNumber,
                Address = dto.Address,
                CreatedAt = DateTime.UtcNow
            };

            var result = await _userManager.CreateAsync(user, dto.Password);
            if (!result.Succeeded)
                return ApiResponseDto<AdminUserDto>.ErrorResponse(
                    "Failed to create user",
                    result.Errors.Select(e => e.Description).ToList());

            // Validate and assign roles
            foreach (var role in dto.Roles)
            {
                if (await _roleManager.RoleExistsAsync(role))
                {
                    await _userManager.AddToRoleAsync(user, role);
                }
            }

            var roles = await _userManager.GetRolesAsync(user);
            var userDto = new AdminUserDto
            {
                Id = user.Id,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                PhoneNumber = user.PhoneNumber ?? string.Empty,
                Address = user.Address,
                PreferredLanguage = user.PreferredLanguage,
                Roles = roles.ToList(),
                IsDeleted = false,
                CreatedAt = user.CreatedAt
            };

            return ApiResponseDto<AdminUserDto>.SuccessResponse(userDto, "User created successfully");
        }

        public async Task<ApiResponseDto<bool>> AssignRolesToUserAsync(AssignRolesDto dto)
        {
            var user = await _userManager.FindByIdAsync(dto.UserId);
            if (user == null)
                return ApiResponseDto<bool>.ErrorResponse("User not found");

            var currentRoles = await _userManager.GetRolesAsync(user);
            await _userManager.RemoveFromRolesAsync(user, currentRoles);

            foreach (var role in dto.Roles)
            {
                if (await _roleManager.RoleExistsAsync(role))
                {
                    await _userManager.AddToRoleAsync(user, role);
                }
            }

            return ApiResponseDto<bool>.SuccessResponse(true, "Roles updated successfully");
        }

        public async Task<ApiResponseDto<bool>> ToggleUserStatusAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return ApiResponseDto<bool>.ErrorResponse("User not found");

            user.IsDeleted = !user.IsDeleted;
            user.UpdatedAt = DateTime.UtcNow;
            await _userManager.UpdateAsync(user);

            var status = user.IsDeleted ? "deactivated" : "activated";
            return ApiResponseDto<bool>.SuccessResponse(true, $"User {status} successfully");
        }

        public async Task<ApiResponseDto<bool>> DeleteUserAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return ApiResponseDto<bool>.ErrorResponse("User not found");

            user.IsDeleted = true;
            user.UpdatedAt = DateTime.UtcNow;
            await _userManager.UpdateAsync(user);

            return ApiResponseDto<bool>.SuccessResponse(true, "User deleted successfully");
        }

        public async Task<ApiResponseDto<List<RoleDto>>> GetAllRolesAsync()
        {
            var roles = await _roleManager.Roles.ToListAsync();
            var roleDtos = new List<RoleDto>();

            foreach (var role in roles)
            {
                var usersInRole = await _userManager.GetUsersInRoleAsync(role.Name!);
                roleDtos.Add(new RoleDto
                {
                    Name = role.Name!,
                    Description = role.Name!,
                    UsersCount = usersInRole.Count
                });
            }

            return ApiResponseDto<List<RoleDto>>.SuccessResponse(roleDtos);
        }
    }
}

