using ELIXIR.DATA.CORE.INTERFACES.SETUP_INTERFACE;
using ELIXIR.DATA.DATA_ACCESS_LAYER.MODELS.SETUP_MODEL;
using ELIXIR.DATA.DATA_ACCESS_LAYER.STORE_CONTEXT;
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



    }
}
