using ELIXIR.DATA.CORE.ICONFIGURATION;
using ELIXIR.DATA.DATA_ACCESS_LAYER.EXTENSIONS;
using ELIXIR.DATA.DATA_ACCESS_LAYER.HELPERS;
using ELIXIR.DATA.DATA_ACCESS_LAYER.MODELS.SETUP_MODEL;
using ELIXIR.DATA.DTOs.SETUP_DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ELIXIR.API.Controllers.SETUP_CONTROLLER
{
    [ApiController]

    public class ProductTypeController : BaseApiController
    {

        private readonly IUnitOfWork _unitOfWork;

        public ProductTypeController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpPost("AddNewProductType")]
        public async Task<IActionResult> AddNewProductType(ProductType productType)
        {
            var exisitingProductType = await _unitOfWork.ProductType.GetProductTypeByName(productType.ProductTypeName);
            if (exisitingProductType != null)
                return BadRequest($"{productType.ProductTypeName} is already exist");
            await _unitOfWork.ProductType.AddNewProductType(productType);
            await _unitOfWork.CompleteAsync();
            return Ok($"Product type {productType.ProductTypeName} successfully added");
        }

        [HttpPut("UpdateProductType")]
        public async Task<IActionResult> UpdateProductType([FromBody] ProductType productType)
        {
            var exisitingProductType = await _unitOfWork.ProductType.GetProductTypeById(productType.Id);
            if (exisitingProductType == null)
                return BadRequest($"Product type is not exist");

            await _unitOfWork.ProductType.UpdateProductType(productType);
            await _unitOfWork.CompleteAsync();
            return Ok("Prodcut Type is sucessfully updated");

        }

        [HttpPut("UpdateProductTypeStatus")]
        public async Task<IActionResult> UpdateProductTypeStatus([FromBody] ProductType productType)
        {
            string status;
            var exisitingProducType = await _unitOfWork.ProductType.GetProductTypeById(productType.Id);
            if (exisitingProducType == null)
                return BadRequest("Product Type is not exist");
            await _unitOfWork.ProductType.UpdateProductTypeStatus(productType);

            status = productType.IsActive == true ? "Activated" : "Inactivated";
            await _unitOfWork.CompleteAsync();
            return Ok($"{productType.ProductTypeName} is successfully {status}");
        }

        [HttpGet("GetProductTypeById/{id}")]
        public async Task<IActionResult> GetProductTypeById([FromRoute]int id)
        {
            var productType = await _unitOfWork.ProductType.GetProductTypeById(id);
            if (productType == null) 
                return BadRequest("No Product Type found");
            return Ok(productType);
        }
        [HttpGet("GetAllProductTypePagination")]
        public async Task<ActionResult<IEnumerable<ProductTypeDto>>> GetAllProductTypePagination([FromQuery] bool status, [FromQuery] UserParams userParams)
        {
            var product = await _unitOfWork.ProductType.GetAllProductTypePagination(status, userParams);

            Response.AddPaginationHeader(product.CurrentPage, product.PageSize, product.TotalCount, product.TotalPages, product.HasNextPage, product.HasPreviousPage);

            var productTypeResult = new
            {
                product,
                product.CurrentPage,
                product.PageSize,
                product.TotalCount,
                product.TotalPages,
                product.HasNextPage,
                product.HasPreviousPage
            };

            return Ok(productTypeResult);
        }

        [HttpGet("GetAllPaginationByStatus/{status}")]
        public async Task<ActionResult<IEnumerable<ProductTypeDto>>> GetAllProductTypeByStatusPaginationOrig([FromRoute] bool status, [FromQuery] string search, [FromQuery] UserParams userParams)
        {
            if (search == null)
                return await _unitOfWork.ProductType.GetAllProductTypePagination(status, userParams);

            var productType = await _unitOfWork.ProductType.GetAllProductTypePaginationOrig(search, status, userParams);
            Response.AddPaginationHeader(productType.CurrentPage, productType.PageSize, productType.TotalCount, productType.TotalPages, productType.HasNextPage, productType.HasPreviousPage);

            var productTypeResult = new
            {
                productType,
                productType.CurrentPage,
                productType.PageSize,
                productType.TotalCount,
                productType.TotalPages,
                productType.HasNextPage,
                productType.HasPreviousPage
            };

            return Ok(productTypeResult);
        }
    
    }
}
