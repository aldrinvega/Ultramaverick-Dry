using System.Collections.Generic;
using System.Security.Cryptography.Pkcs;
using System.Threading.Tasks;
using ELIXIR.DATA.CORE.ICONFIGURATION;
using ELIXIR.DATA.DATA_ACCESS_LAYER.EXTENSIONS;
using ELIXIR.DATA.DATA_ACCESS_LAYER.HELPERS;
using ELIXIR.DATA.DATA_ACCESS_LAYER.MODELS.SETUP_MODEL;
using ELIXIR.DATA.DTOs.SETUP_DTOs;
using Microsoft.AspNetCore.Mvc;

namespace ELIXIR.API.Controllers.SETUP_CONTROLLER
{
    public class LabTestMasterListController : BaseApiController
    {
        private readonly IUnitOfWork _unitOfWork;

        public LabTestMasterListController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        #region Sample Type
        [HttpGet]
        [Route("GetAllSampleTypes")]
        public async Task<IActionResult> GetAllSampleTypes()
        {
            var sampleTypes = await _unitOfWork.LabtestMasterlist.GetAllSampleType();
            if (sampleTypes == null)
                return BadRequest("No Sample Types found.");
            return Ok(sampleTypes);
        }

        [HttpPost]
        [Route("AddNewSampleType")]
        public async Task<IActionResult> AddNewSampleType(SampleType sampleType)
        {
            var validateSampleType = await _unitOfWork.LabtestMasterlist.GetSampleTypeByName(sampleType.SampleTypeName);
            if (validateSampleType != null)
                return BadRequest($"Sample type {sampleType.SampleTypeName} is already exist");
            await _unitOfWork.LabtestMasterlist.AddNewSampleType(sampleType);
            await _unitOfWork.CompleteAsync();
            return Ok("Sample Type successfully added");
        }

        [HttpGet]
        [Route("GetSampleTypeById{id:int}")]
        public async Task<IActionResult> GetAllSampleTypeById(int id)
        {
            var sampleTypeResult = await _unitOfWork.LabtestMasterlist.GetSampleTypeById(id);

            if (sampleTypeResult == null)
                return BadRequest("No records found");
            return Ok(sampleTypeResult);
        }

        [HttpPut]
        [Route("UpdateSampleType/{id}")]
        public async Task<IActionResult> UpdateSampleType(int id, [FromBody] SampleType sampleType)
        {
            var validateSampleType = await _unitOfWork.LabtestMasterlist.GetSampleTypeById(sampleType.Id);
            if (validateSampleType == null)
                return BadRequest($"Id {sampleType.Id} is not exists");
            await _unitOfWork.LabtestMasterlist.UpdateSampleType(sampleType);
            await _unitOfWork.CompleteAsync();
            return Ok("Sample Type successfully updated");
        }

        [HttpGet]
        [Route("GetAllSampleTypePagination")]
        public async Task<ActionResult<IEnumerable<SampleTypeDto>>> GetAllSampleTypePagination([FromQuery]UserParams userParams)
        {
            var sampleTypes = await _unitOfWork.LabtestMasterlist.GetAllSampleTypePagination(userParams);
            
            Response.AddPaginationHeader(sampleTypes.CurrentPage, sampleTypes.PageSize, sampleTypes.TotalCount, sampleTypes.TotalPages, sampleTypes.HasNextPage, sampleTypes.HasPreviousPage);

            var sampleTypeResult = new
            {
                sampleTypes,
                sampleTypes.CurrentPage,
                sampleTypes.PageSize,
                sampleTypes.TotalCount,
                sampleTypes.TotalPages,
                sampleTypes.HasNextPage,
                sampleTypes.HasPreviousPage
            };

            return Ok(sampleTypeResult);
        }

        [HttpPut]
        [Route("UpdateSampleTypeStatus/{id}")]
        public async Task<IActionResult> UpdateSampleTypeStatus(int id, [FromBody] SampleType sampleType)
        {
            var validateSampleType = await _unitOfWork.LabtestMasterlist.GetSampleTypeById(id);
            if (validateSampleType == null)
                return BadRequest($"Sample Type with id {sampleType.Id} is not exists");
            await _unitOfWork.LabtestMasterlist.UpdateSampleTypeStatus(sampleType);
            return Ok("Sample Type updated successfully.");
        }
        #endregion
        
        #region Type of Swab
        [HttpGet]
        [Route("GetAllTypeOfSwab")]
        public async Task<IActionResult> GetAllTypeOfSwab()
        {
            var typeOfSwab = await _unitOfWork.LabtestMasterlist.GetAllTypeOfSwab();
            if (typeOfSwab == null)
                return BadRequest("No Type of Swab found.");
            return Ok(typeOfSwab);
        }

        [HttpPost]
        [Route("AddNewTypeOfSwab")]
        public async Task<IActionResult> AddNewTypeOfSwab(TypeOfSwab typeOfSwab)
        {
            var validateSampleType = _unitOfWork.LabtestMasterlist.GetSampleTypeByName(typeOfSwab.TypeofSwabName);
            if (validateSampleType != null)
                return BadRequest($"Sample type {typeOfSwab.TypeofSwabName} is already exist");
            await _unitOfWork.LabtestMasterlist.AddNewTypeOfSwab(typeOfSwab);
            await _unitOfWork.CompleteAsync();
            return Ok("Type of Swab successfully added");
        }

        [HttpGet]
        [Route("GetTypeOfSwabById{id:int}")]
        public async Task<IActionResult> GetSampleTypeById(int id)
        {
            var typeOfSwabResult = await _unitOfWork.LabtestMasterlist.GetTypeOfSwabById(id);

            if (typeOfSwabResult == null)
                return BadRequest("No records found");
            return Ok(typeOfSwabResult);
        }

        [HttpPut]
        [Route("UpdateTypeOfSwab/{id}")]
        public async Task<IActionResult> UpdateSampleType(int id, [FromBody] TypeOfSwab typeOfSwab)
        {
            var validateSampleType = await _unitOfWork.LabtestMasterlist.GetSampleTypeById(typeOfSwab.Id);
            if (validateSampleType == null)
                return BadRequest($"Id {typeOfSwab.Id} is not exists");
            await _unitOfWork.LabtestMasterlist.UpdateTypeOfSwab(typeOfSwab);
            await _unitOfWork.CompleteAsync();
            return Ok("Type of Swab successfully updated");
        }

        [HttpGet]
        [Route("GetAllTypeOfSwabPagination")]
        public async Task<ActionResult<IEnumerable<TypeOfSwabDto>>> GetAllTypeOfSwabPagination(UserParams userParams)
        {
            var typeOfSwab = await _unitOfWork.LabtestMasterlist.GetAllTypeOfSwabPagination(userParams);
            
            Response.AddPaginationHeader(typeOfSwab.CurrentPage, typeOfSwab.PageSize, typeOfSwab.TotalCount, typeOfSwab.TotalPages, typeOfSwab.HasNextPage, typeOfSwab.HasPreviousPage);

            var typeOfSwabResult = new
            {
                typeOfSwab,
                typeOfSwab.CurrentPage,
                typeOfSwab.PageSize,
                typeOfSwab.TotalCount,
                typeOfSwab.TotalPages,
                typeOfSwab.HasNextPage,
                typeOfSwab.HasPreviousPage
            };

            return Ok(typeOfSwab);
        }

        [HttpPut]
        [Route("UpdateTypeOfSwabStatus/{id}")]
        public async Task<IActionResult> UpdateTypeofSwabStatus(int id, [FromBody] TypeOfSwab typeOfSwab)
        {
            var validateTypeofSwab = await _unitOfWork.LabtestMasterlist.GetTypeOfSwabById(id);
            if (validateTypeofSwab == null)
                return BadRequest($"Sample Type with id {typeOfSwab.Id} is not exists");
            await _unitOfWork.LabtestMasterlist.UpdateTypeOfSwab(typeOfSwab);
            return Ok("Type of Swab updated successfully.");
        }
        #endregion
        
        #region Analysis
        [HttpGet]
        [Route("GetAllAnalyses")]
        public async Task<IActionResult> GetAllAnalyses()
        {
            var analysis = await _unitOfWork.LabtestMasterlist.GetAllAnalysis();
            if (analysis == null)
                return BadRequest("No Analyses found.");
            return Ok(analysis);
        }

        [HttpPost]
        [Route("AddNewAnalysis")]
        public async Task<IActionResult> AddNewAnalysis(Analysis analysis)
        {
            var validateSampleType = _unitOfWork.LabtestMasterlist.GetSampleTypeByName(analysis.AnalysisName);
            if (validateSampleType != null)
                return BadRequest($"Sample type {analysis.AnalysisName} is already exist");
            await _unitOfWork.LabtestMasterlist.AddNewAnalysis(analysis);
            await _unitOfWork.CompleteAsync();
            return Ok("Analysis successfully added");
        }

        [HttpGet]
        [Route("GetTypeAnalysisById{id:int}")]
        public async Task<IActionResult> GetAllSampleTypeById(int id, Analysis analysis)
        {
            var analysisResult = await _unitOfWork.LabtestMasterlist.GetAnalysisById(id);

            if (analysisResult == null)
                return BadRequest("No records found");
            return Ok(analysisResult);
        }

        [HttpPut]
        [Route("UpdateAnalysis/{id}")]
        public async Task<IActionResult> UpdateAnalysis(int id, [FromBody] Analysis analysis)
        {
            var validateAnalysis = await _unitOfWork.LabtestMasterlist.GetSampleTypeById(analysis.Id);
            if (validateAnalysis == null)
                return BadRequest($"Id {analysis.Id} is not exists");
            await _unitOfWork.LabtestMasterlist.UpdateAnalysis(analysis);
            await _unitOfWork.CompleteAsync();
            return Ok("Analysis successfully updated");
        }

        [HttpGet]
        [Route("GetAllAnalysesSwabPagination")]
        public async Task<ActionResult<IEnumerable<AnalysesDto>>> GetAllAnalysesPagination(UserParams userParams)
        {
            var analyses = await _unitOfWork.LabtestMasterlist.GetAllTypeOfSwabPagination(userParams);
            
            Response.AddPaginationHeader(analyses.CurrentPage, analyses.PageSize, analyses.TotalCount, analyses.TotalPages, analyses.HasNextPage, analyses.HasPreviousPage);

            var analysesResult = new
            {
                analyses,
                analyses.CurrentPage,
                analyses.PageSize,
                analyses.TotalCount,
                analyses.TotalPages,
                analyses.HasNextPage,
                analyses.HasPreviousPage
            };

            return Ok(analysesResult);
        }

        [HttpPut]
        [Route("UpdateAnalysisStatus/{id}")]
        public async Task<IActionResult> UpdateAnalysisStatus(int id, [FromBody] Analysis analysis)
        {
            var validateAnalysis = await _unitOfWork.LabtestMasterlist.GetAnalysisById(id);
            if (validateAnalysis == null)
                return BadRequest($"Analysis with id {analysis.Id} is not exists");
            await _unitOfWork.LabtestMasterlist.UpdateAnalysis(analysis);
            return Ok("Analysis updated successfully.");
        }
        #endregion
        
        #region Product Condition
        [HttpGet]
        [Route("GetAllProductCondition")]
        public async Task<IActionResult> GetAllProductCondition()
        {
            var productCondition = await _unitOfWork.LabtestMasterlist.GetAllProductCondition();
            if (productCondition == null)
                return BadRequest("No Product Conditions found.");
            return Ok(productCondition);
        }

        [HttpPost]
        [Route("AddNewProductCondition")]
        public async Task<IActionResult> AddNewAnalysis(ProductCondition productCondition)
        {
            var validateProductCondition =
                _unitOfWork.LabtestMasterlist.GetProductConditionByName(productCondition.ProductConditionName);
            if (validateProductCondition != null)
                return BadRequest($"Sample type {productCondition.ProductConditionName} is already exist");
            await _unitOfWork.LabtestMasterlist.AddNewProductCondition(productCondition);
            await _unitOfWork.CompleteAsync();
            return Ok("Product Condition successfully added");
        }

        [HttpGet]
        [Route("GetProductConditionById{id:int}")]
        public async Task<IActionResult> GetProductConditionById(int id, ProductCondition productCondition)
        {
            var productConditionName =
                await _unitOfWork.LabtestMasterlist.GetProductConditionByName(productCondition.ProductConditionName);

            if (productConditionName == null)
                return BadRequest("No records found");
            return Ok(productConditionName);
        }

        [HttpPut]
        [Route("UpdateProdutCondition/{id}")]
        public async Task<IActionResult> UpdateProductCondition(int id, [FromBody] ProductCondition productCondition)
        {
            var validateProductCondition = await _unitOfWork.LabtestMasterlist.GetProductConditionById(productCondition.Id);
            if (validateProductCondition == null)
                return BadRequest($"Id {productCondition.Id} is not exists");
            await _unitOfWork.LabtestMasterlist.UpdateProductCondition(productCondition);
            await _unitOfWork.CompleteAsync();
            return Ok("Product condition successfully updated");
        }

        [HttpGet]
        [Route("GetAllProductConditionPagination")]
        public async Task<ActionResult<IEnumerable<ProductConditionDto>>> GetAllProductConditionPagination(UserParams userParams)
        {
            var productCondition = await _unitOfWork.LabtestMasterlist.GetAllTypeOfSwabPagination(userParams);
            
            Response.AddPaginationHeader(productCondition.CurrentPage, productCondition.PageSize, productCondition.TotalCount, productCondition.TotalPages, productCondition.HasNextPage, productCondition.HasPreviousPage);

            var productConditionResult = new
            {
                productCondition,
                productCondition.CurrentPage,
                productCondition.PageSize,
                productCondition.TotalCount,
                productCondition.TotalPages,
                productCondition.HasNextPage,
                productCondition.HasPreviousPage
            };

            return Ok(productConditionResult);
        }

        [HttpPut]
        [Route("UpdateProductConditionStatus/{id}")]
        public async Task<IActionResult> UpdateProductConditionStatus(int id, [FromBody] ProductCondition productCondition)
        {
            var validateProductCondition = await _unitOfWork.LabtestMasterlist.GetProductConditionById(id);
            if (validateProductCondition == null)
                return BadRequest($"Analysis with id {productCondition.Id} is not exists");
            await _unitOfWork.LabtestMasterlist.UpdateProductCondition(productCondition);
            return Ok("Product Condition updated successfully.");
        }
        #endregion

        #region Disposition

        [HttpGet("GetAllDisposition")]
        public async Task<IActionResult> GetAllDisposition()
        {
            var dispositionResult = await _unitOfWork.LabtestMasterlist.GetAllDisposition();
            if (dispositionResult == null)
                return BadRequest("No Disposition Found");
            return Ok(dispositionResult);
        }

        [HttpGet("GetDispositionById")]
        public async Task<IActionResult> GetDispositionById(int id)
        {
            var dispositionResult = await _unitOfWork.LabtestMasterlist.GetDispositionById(id);
            if (dispositionResult == null)
                return BadRequest($"No Disposition found with id {id}");

            return Ok(dispositionResult);
        }

        [HttpPut("UpdateDispositionStatus")]
        public async Task<IActionResult> UpdateDispositionStatus(Disposition status)
        {
            var dispositionResult = await _unitOfWork.LabtestMasterlist.UpdateDispositionStatus(status);
            if (!dispositionResult)
                return BadRequest("Something went wrong");
            return Ok("Update success!");
        }


        #endregion

    }
}