using ELIXIR.DATA.CORE.INTERFACES.SETUP_INTERFACE;
using ELIXIR.DATA.DATA_ACCESS_LAYER.HELPERS;
using ELIXIR.DATA.DATA_ACCESS_LAYER.MODELS.SETUP_MODEL;
using ELIXIR.DATA.DATA_ACCESS_LAYER.STORE_CONTEXT;
using ELIXIR.DATA.DTOs.SETUP_DTOs;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELIXIR.DATA.DATA_ACCESS_LAYER.REPOSITORIES.SETUP_REPOSITORY
{
    public class ProductTypeRepository : IProductTypeRepository
    {
        private new readonly StoreContext _context;

        public ProductTypeRepository(StoreContext context)
        {
            _context = context;
        }

        public async Task<bool> AddNewProductType(ProductType productType)
        {
            await _context.ProductTypes.AddAsync(productType);
            return true;
        }
        public async Task<bool> UpdateProductType(ProductType productType)
        {
            var existingProductType = _context.ProductTypes.Where(x => x.Id == productType.Id).FirstOrDefault();

            if (existingProductType != null)
            {
                existingProductType.ProductTypeName = productType.ProductTypeName;
                existingProductType.ModifiedBy = productType.ModifiedBy;
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public async Task<ProductType> GetProductTypeById(int id)
        {
            return await _context.ProductTypes.FirstOrDefaultAsync(x => x.Id == id);
        }
        public async Task<bool> UpdateProductTypeStatus(ProductType productType)
        {
            var productTypes = await _context.ProductTypes.FirstOrDefaultAsync(x => x.Id == productType.Id);
            if (productTypes == null)
                return false;
            productTypes.IsActive = productType.IsActive;
            productType.ModifiedBy = productType.ModifiedBy;

            await _context.SaveChangesAsync();
            return true;
        }
        public async Task<ProductType> GetProductTypeByName(string productTypeName)
        {
            return await _context.ProductTypes.FirstOrDefaultAsync(x => x.ProductTypeName == productTypeName);
        }
        public async Task<IEnumerable<ProductType>> GetProductTypeByStatus(bool status)
        {
            return await _context.ProductTypes.Where(x => x.IsActive == status).ToListAsync();
        }
        public async Task<IEnumerable<ProductType>> GetProductTypesAsync()
        {
            return await _context.ProductTypes.ToListAsync();
        }
        public async Task<PagedList<ProductTypeDto>> GetAllProductTypePagination(bool status, UserParams userParams)
        {
            var productType = _context.ProductTypes.Where(x => x.IsActive == status).Select(x => new ProductTypeDto
            {
                Id = x.Id,
                ProductTypeName = x.ProductTypeName,
                IsActive = x.IsActive,
                DateAdded = x.DateAdded.ToString()

            });

            return await PagedList<ProductTypeDto>.CreateAsync(productType, userParams.PageNumber, userParams.PageSize);
        }

        public async Task<PagedList<ProductTypeDto>> GetAllProductTypePaginationOrig(string search, bool status, UserParams userParams)
        {
            var productType = _context.ProductTypes.Where(x => x.IsActive == true).Select(x => new ProductTypeDto
            {
                Id = x.Id,
                ProductTypeName = x.ProductTypeName,
                IsActive = x.IsActive,
                DateAdded = x.DateAdded.ToString()

            }).OrderBy(x => x.ProductTypeName)
              .Where(x => x.IsActive == status)
              .Where(x => x.ProductTypeName.ToLower()
              .Contains(search.Trim().ToLower()));

            return await PagedList<ProductTypeDto>.CreateAsync(productType, userParams.PageNumber, userParams.PageSize);
        }

    }
}
