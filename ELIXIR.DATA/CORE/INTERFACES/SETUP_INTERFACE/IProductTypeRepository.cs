using ELIXIR.DATA.DATA_ACCESS_LAYER.HELPERS;
using ELIXIR.DATA.DATA_ACCESS_LAYER.MODELS.SETUP_MODEL;
using ELIXIR.DATA.DTOs.SETUP_DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELIXIR.DATA.CORE.INTERFACES.SETUP_INTERFACE
{
    public interface IProductTypeRepository
    {
        Task<bool> AddNewProductType(ProductType productType);
        Task<bool> UpdateProductType(ProductType productType);
        Task<bool> UpdateProductTypeStatus(ProductType productType);
        Task<ProductType> GetProductTypeById(int id);
        Task<ProductType> GetProductTypeByName(string productTypeName);
        Task<IEnumerable<ProductType>> GetProductTypeByStatus(bool status);
        Task<IEnumerable<ProductType>> GetProductTypesAsync();
        Task<PagedList<ProductTypeDto>> GetAllProductTypePagination(bool status, UserParams userParams);
        Task<PagedList<ProductTypeDto>> GetAllProductTypePaginationOrig(string search, bool status, UserParams userParams);

    }
}
