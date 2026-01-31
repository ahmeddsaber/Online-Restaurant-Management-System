using Microsoft.EntityFrameworkCore;
using Restaurant.Application.Contract;

using Restaurant.Application.Interfaces;
using Restaurant.Domain.Entities;

namespace Restaurant.Application.Services
{
    public class MenuItemService : IMenuItemService
    {
        private readonly IUnitOfWork _unitOfWork;

        public MenuItemService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // دالة async
        public async Task<IEnumerable<MenuItem>> GetAllMenuItemsAsync()
        {
            var items = _unitOfWork.MenuItem.GetAll(); // افترض إن GetAll() بيرجع IQueryable<MenuItem>



            return await Task.FromResult(items); 
            
        }
    }
}
