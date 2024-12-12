using Microsoft.EntityFrameworkCore;
using PharmaStockManager.Models;

namespace PharmaStockManager.Services
{
    public class WarehouseService
    {
        private readonly PharmaContext _context;

        public WarehouseService(PharmaContext context)
        {
            _context = context;
        }

        public async Task<Warehouse> CreateWarehouseAsync(string name, string address = null)
        {
            var warehouse = new Warehouse
            {
                Name = name,
                Address = address,
                RefCode = await GenerateUniqueRefCodeAsync(),
            };

            _context.Warehouses.Add(warehouse);
            await _context.SaveChangesAsync();
            return warehouse;
        }

        public async Task<string> GenerateUniqueRefCodeAsync()
        {
            string refCode;
            bool isUnique;
            do
            {
                // Generate a shortened GUID as RefCode
                refCode = Guid.NewGuid()
                              .ToString("N") // Remove dashes
                              .Substring(0, 8) // Take the first 8 characters
                              .ToUpper();

                // Check if RefCode already exists in the database
                isUnique = !await _context.Warehouses.AnyAsync(w => w.RefCode == refCode);
            }
            while (!isUnique); // Repeat if collision occurs

            return refCode;
        }
    }

}
