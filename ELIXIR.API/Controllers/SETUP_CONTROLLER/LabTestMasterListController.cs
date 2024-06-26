﻿using System.Collections.Generic;
using System.Threading.Tasks;
using ELIXIR.DATA.CORE.ICONFIGURATION;
using ELIXIR.DATA.DATA_ACCESS_LAYER.EXTENSIONS;
using ELIXIR.DATA.DATA_ACCESS_LAYER.HELPERS;
using ELIXIR.DATA.DATA_ACCESS_LAYER.MODELS.SETUP_MODEL;
using ELIXIR.DATA.DTOs.SETUP_DTOs;
using Microsoft.AspNetCore.Authentication.JwtBearer;
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

        [HttpGet("GetAllSampleTypeByStatus/{status}")]
        public async Task<ActionResult<IEnumerable<SampleTypeDto>>> GetAllSampleTypeByStatus(bool status)
        {
            var sampleTypes = await _unitOfWork.LabtestMasterlist.GetAllSampleTypeByStatus(status);

            return Ok(sampleTypes);
        }

        [HttpPost]
        [Route("AddNewSampleType")]
        public async Task<IActionResult> AddNewSampleType(SampleType sampleType)
        {
            var validateSampleType = await _unitOfWork.LabtestMasterlist.GetSampleTypeByName(sampleType.SampleTypeName);
            if (validateSampleType != null)
                return BadRequest($"Sample type {sampleType.SampleTypeName} is already exist");
            sampleType.IsActive = true;
            await _unitOfWork.LabtestMasterlist.AddNewSampleType(sampleType);
            await _unitOfWork.CompleteAsync();
            return Ok("Sample Type successfully added");
        }

        [HttpGet]
        [Route("GetSampleTypeById{id}")]
        public async Task<IActionResult> GetAllSampleTypeById(int id)
        {
            var sampleTypeResult = await _unitOfWork.LabtestMasterlist.GetSampleTypeById(id);

            if (sampleTypeResult == null)
                return BadRequest("No records found");
            return Ok(sampleTypeResult);
        }

        [HttpPut]
        [Route("UpdateSampleType")]
        public async Task<IActionResult> UpdateSampleType([FromBody] SampleType sampleType)
        {
            var validateSampleType = await _unitOfWork.LabtestMasterlist.GetSampleTypeById(sampleType.Id);
            var exisitingSampleType = await _unitOfWork.LabtestMasterlist.GetSampleTypeByName(sampleType.SampleTypeName);
            if (exisitingSampleType != null) return BadRequest($"{sampleType.SampleTypeName} is already exist");
            if (validateSampleType == null)
                return BadRequest($"Id {sampleType.Id} is not exists");
            await _unitOfWork.LabtestMasterlist.UpdateSampleType(sampleType);
            await _unitOfWork.CompleteAsync();
            return Ok("Sample Type successfully updated");
        }

        [HttpGet]
        [Route("GetAllSampleTypePagination")]
        public async Task<ActionResult<IEnumerable<SampleTypeDto>>> GetAllSampleTypePagination([FromRoute] bool status, [FromQuery] UserParams userParams)
        {
            var sampleTypes = await _unitOfWork.LabtestMasterlist.GetAllSampleTypePagination(status, userParams);

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

        [HttpGet]
        [Route("GetAllSampleTypePaginationOrig/{status}")]
        public async Task<ActionResult<IEnumerable<SampleTypeDto>>> GetAllSampleTypePaginationOrig([FromRoute] bool status, [FromQuery] UserParams userParams, [FromQuery] string search)
        {

            if (search == null)

                return await GetAllSampleTypePagination(status, userParams);

            var sampleType = await _unitOfWork.LabtestMasterlist.GetAllSampleTypeByStatusPaginationOrig(search, status, userParams);


            Response.AddPaginationHeader(sampleType.CurrentPage, sampleType.PageSize, sampleType.TotalCount, sampleType.TotalPages, sampleType.HasNextPage, sampleType.HasPreviousPage);

            var sampleTypeResult = new
            {
                sampleType,
                sampleType.CurrentPage,
                sampleType.PageSize,
                sampleType.TotalCount,
                sampleType.TotalPages,
                sampleType.HasNextPage,
                sampleType.HasPreviousPage
            };

            return Ok(sampleTypeResult);
        }

        [HttpPut]
        [Route("UpdateSampleTypeStatus")]
        public async Task<IActionResult> UpdateSampleTypeStatus([FromBody] SampleType sampleType)
        {
            string status;
            var validateSampleType = await _unitOfWork.LabtestMasterlist.GetSampleTypeById(sampleType.Id);
            if (validateSampleType == null)
                return BadRequest($"Sample Type with id {sampleType.Id} is not exists");
            await _unitOfWork.LabtestMasterlist.UpdateSampleTypeStatus(sampleType);

            if (sampleType.IsActive == true)
            {
                status = "Activated";
            }
            else
            {
                status = "Inactivated";
            }
            await _unitOfWork.CompleteAsync();

            return Ok($" {validateSampleType.SampleTypeName} is sucessfully {status}");
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
            var validateTypeofSwab = await _unitOfWork.LabtestMasterlist.GetTypeofSwabByName(typeOfSwab.TypeofSwabName);
            if (validateTypeofSwab != null)
                return BadRequest($"Type of swab {typeOfSwab.TypeofSwabName} is already exist");
            await _unitOfWork.LabtestMasterlist.AddNewTypeOfSwab(typeOfSwab);
            await _unitOfWork.CompleteAsync();
            return Ok("Type of Swab successfully added");
        }

        [HttpGet]
        [Route("GetTypeOfSwabById{id}")]
        public async Task<IActionResult> GetztypeOfSwabById(int id)
        {
            var typeOfSwabResult = await _unitOfWork.LabtestMasterlist.GetTypeOfSwabById(id);

            if (typeOfSwabResult == null)
                return BadRequest("No records found");
            return Ok(typeOfSwabResult);
        }
        [HttpGet("GetAllTypeOfSwabByStatus/{status}")]
        public async Task<ActionResult<IEnumerable<TypeOfSwab>>> GetAllTypeOfSwabByStatus(bool status)
        {
            var typeofswab = await _unitOfWork.LabtestMasterlist.GetAllTypeOfSwabByStatus(status);
            return Ok(typeofswab);
        }

        [HttpPut]
        [Route("UpdateTypeOfSwab")]
        public async Task<IActionResult> UpdateTypeOfSwab([FromBody] TypeOfSwab typeOfSwab)
        {
            var validateTypeofSwab = await _unitOfWork.LabtestMasterlist.GetTypeOfSwabById(typeOfSwab.Id);
            var existingTypeofSwab = await _unitOfWork.LabtestMasterlist.GetTypeofSwabByName(typeOfSwab.TypeofSwabName);
            if (existingTypeofSwab != null) return BadRequest($"{typeOfSwab.TypeofSwabName} is already exist");
            if (validateTypeofSwab == null)
                return BadRequest($"Id {typeOfSwab.Id} is not exists");
            await _unitOfWork.LabtestMasterlist.UpdateTypeOfSwab(typeOfSwab);
            await _unitOfWork.CompleteAsync();
            return Ok("Type of Swab successfully updated");
        }

        [HttpGet]
        [Route("GetAllTypeOfSwabPagination")]
        public async Task<ActionResult<IEnumerable<TypeOfSwabDto>>> GetAllTypeOfSwabPagination(bool status, [FromQuery] UserParams userParams)
        {
            var typeOfSwab = await _unitOfWork.LabtestMasterlist.GetAllTypeOfSwabPagination(status, userParams);

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

            return Ok(typeOfSwabResult);
        }

        [HttpGet]
        [Route("GetAllTypeOfSwabPaginationOrig/{status}")]
        public async Task<ActionResult<IEnumerable<TypeOfSwabDto>>> GetAllTypeOfSwabPaginationOrig([FromRoute] bool status, [FromQuery] UserParams userParams, [FromQuery] string search)
        {

            if (search == null)
                return await GetAllTypeOfSwabPagination(status, userParams);

            var typeOfSwab = await _unitOfWork.LabtestMasterlist.GetAllTypeOfSwabPaginationOrig(search, status, userParams);

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

            return Ok(typeOfSwabResult);
        }


        [HttpPut]
        [Route("UpdateTypeOfSwabStatus")]
        public async Task<IActionResult> UpdateTypeofSwabStatus([FromBody] TypeOfSwab typeOfSwab)
        {
            string status;
            var validateTypeofSwab = await _unitOfWork.LabtestMasterlist.GetTypeOfSwabById(typeOfSwab.Id);
            if (validateTypeofSwab == null)
                return BadRequest($"Sample Type with id {typeOfSwab.Id} is not exists");
            await _unitOfWork.LabtestMasterlist.UpdateTypeOfSwabStatus(typeOfSwab);
            if (typeOfSwab.IsActive == true)
            {
                status = "Activated";
            }
            else
            {
                status = "Inactivated";
            }
            await _unitOfWork.CompleteAsync();
            return Ok($" {validateTypeofSwab.TypeofSwabName} is sucessfully {status}");
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
            var validatenalysis = await _unitOfWork.LabtestMasterlist.GetAnalaysisByName(analysis.AnalysisName);
            if (validatenalysis != null)
                return BadRequest($"Sample type {analysis.AnalysisName} is already exist");
            await _unitOfWork.LabtestMasterlist.AddNewAnalysis(analysis);
            await _unitOfWork.CompleteAsync();
            return Ok("Analysis successfully added");
        }

        [HttpGet]
        [Route("GetAnalysisById{id}")]
        public async Task<IActionResult> GetAnalysisById(int id)
        {
            var analysisResult = await _unitOfWork.LabtestMasterlist.GetAnalysisById(id);

            if (analysisResult == null)
                return BadRequest("No records found");
            return Ok(analysisResult);
        }

        [HttpGet("GetAllAnalysisByStatus/{status}")]
        public async Task<IActionResult> GetAllAnalysisByStatus([FromRoute] bool status)
        {
            var analysisResult = await _unitOfWork.LabtestMasterlist.GetAllAnalysisByStatus(status);
            if (analysisResult == null)
                return BadRequest("No Result Found");
            return Ok(analysisResult);
        }

        [HttpPut]
        [Route("UpdateAnalysis")]
        public async Task<IActionResult> UpdateAnalysis([FromBody] Analysis analysis)
        {
            var validateAnalysis = await _unitOfWork.LabtestMasterlist.GetAnalysisById(analysis.Id);
            var existingAnalysis = await _unitOfWork.LabtestMasterlist.GetAnalaysisByName(analysis.AnalysisName);
            if (existingAnalysis != null) return BadRequest($"{analysis.AnalysisName} is already exist");
            if (validateAnalysis == null)
                return BadRequest($"Id {analysis.Id} is not exists");
            await _unitOfWork.LabtestMasterlist.UpdateAnalysis(analysis);
            await _unitOfWork.CompleteAsync();
            return Ok("Analysis successfully updated");
        }

        [HttpGet]
        [Route("GetAllAnalysesPagination")]
        public async Task<ActionResult<IEnumerable<AnalysesDto>>> GetAllAnalysesPagination([FromQuery] bool status, [FromQuery] UserParams userParams)
        {
            var analyses = await _unitOfWork.LabtestMasterlist.GetAllAnalysesPagination(status, userParams);

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

        [HttpGet]
        [Route("GetAllAnalysesPaginationOrig/{status}")]
        public async Task<ActionResult<IEnumerable<AnalysesDto>>> GetAllAnalysesPaginationOrig([FromRoute] bool status, [FromQuery] UserParams userParams, [FromQuery] string search)
        {

            if (search == null)

                return await GetAllAnalysesPagination(status, userParams);

            var analyses = await _unitOfWork.LabtestMasterlist.GetAllAnalysesPaginationOrig(search, status, userParams);


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
        [Route("UpdateAnalysisStatus")]
        public async Task<IActionResult> UpdateAnalysisStatus([FromBody] Analysis analysis)
        {
            string status;
            var validateAnalysis = await _unitOfWork.LabtestMasterlist.GetAnalysisById(analysis.Id);
            if (validateAnalysis == null)
                return BadRequest($"Analysis with id {analysis.Id} is not exists");
            await _unitOfWork.LabtestMasterlist.UpdateAnalysisStatus(analysis);
            if (analysis.IsActive == true)
            {
                status = "Activated";
            }
            else
            {
                status = "Inactivated";
            }
            await _unitOfWork.CompleteAsync();
            return Ok($" {validateAnalysis.AnalysisName} is sucessfully {status}");
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
        public async Task<IActionResult> AddNewProduction(ProductCondition productCondition)
        {
            var validateProductCondition =
               await _unitOfWork.LabtestMasterlist.GetProductConditionByName(productCondition.ProductConditionName);
            if (validateProductCondition != null)
                return BadRequest($"Sample type {productCondition.ProductConditionName} is already exist");
            await _unitOfWork.LabtestMasterlist.AddNewProductCondition(productCondition);
            await _unitOfWork.CompleteAsync();
            return Ok("Product Condition successfully added");
        }

        [HttpGet]
        [Route("GetProductConditionById{id:int}")]
        public async Task<IActionResult> GetProductConditionById(int id)
        {
            var productConditionName =
                await _unitOfWork.LabtestMasterlist.GetProductConditionById(id);

            if (productConditionName == null)
                return BadRequest("No records found");
            return Ok(productConditionName);
        }

        [HttpPut]
        [Route("UpdateProductCondition")]
        public async Task<IActionResult> UpdateProductCondition([FromBody] ProductCondition productCondition)
        {
            var validateProductCondition = await _unitOfWork.LabtestMasterlist.GetProductConditionById(productCondition.Id);
            var existingProductCnodition = await _unitOfWork.LabtestMasterlist.GetProductConditionByName(productCondition.ProductConditionName);
            if (existingProductCnodition != null) return BadRequest($"{productCondition.ProductConditionName} is already exist");
            if (validateProductCondition == null)
                return BadRequest($"Id {productCondition.Id} is not exists");
            await _unitOfWork.LabtestMasterlist.UpdateProductCondition(productCondition);
            await _unitOfWork.CompleteAsync();
            return Ok("Product condition successfully updated");
        }

        [HttpGet]
        [Route("GetAllProductConditionPagination")]
        public async Task<ActionResult<IEnumerable<ProductConditionDto>>> GetAllProductConditionPagination([FromQuery] bool status, [FromQuery] UserParams userParams)
        {
            var productCondition = await _unitOfWork.LabtestMasterlist.GetAllProductConditionPagination(status, userParams);

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

        [HttpGet]
        [Route("GetAllProductConditionPaginationOrig/{status}")]
        public async Task<ActionResult<IEnumerable<ProductConditionDto>>> GetAllProductConditionPaginationOrig([FromRoute] bool status, [FromQuery] UserParams userParams, [FromQuery] string search)
        {
            if (search == null)
                return await GetAllProductConditionPagination(status, userParams);

            var productCondition = await _unitOfWork.LabtestMasterlist.GetAllProductConditionPaginationOrig(search, status, userParams);

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
        [Route("UpdateProductConditionStatus")]
        public async Task<IActionResult> UpdateProductConditionStatus([FromBody] ProductCondition productCondition)
        {
            string status;
            var validateProductCondition = await _unitOfWork.LabtestMasterlist.GetProductConditionById(productCondition.Id);
            if (validateProductCondition == null)
                return BadRequest($"Analysis with id {productCondition.Id} is not exists");
            await _unitOfWork.LabtestMasterlist.UpdateProductConditionStatus(productCondition);
            await _unitOfWork.CompleteAsync();
            status = productCondition.IsActive == true ? "Activated" : "Inactivated";
            return Ok($"{validateProductCondition.ProductConditionName} is successfully {status}");
        }
        #endregion

        #region Disposition

        [HttpPost]
        [Route("AddNewDisposition")]
        public async Task<IActionResult> AddNewDisposition(Disposition disposition)
        {
            var validateSampleType = await _unitOfWork.LabtestMasterlist.GetDispositonByName(disposition.DispositionName);
            if (validateSampleType != null)
                return BadRequest($"Disposition {disposition.DispositionName} is already exist");
            await _unitOfWork.LabtestMasterlist.AddNewDisposition(disposition);
            await _unitOfWork.CompleteAsync();
            return Ok("Disposition successfully added");
        }

        [HttpGet("GetAllDisposition")]
        public async Task<IActionResult> GetAllDisposition()
        {
            var dispositionResult = await _unitOfWork.LabtestMasterlist.GetAllDisposition();
            if (dispositionResult == null)
                return BadRequest("No Disposition Found");
            return Ok(dispositionResult);
        }

        [HttpGet("GetDispositionById/{id}")]
        public async Task<IActionResult> GetDispositionById([FromQuery]int id)
        {
            var dispositionResult = await _unitOfWork.LabtestMasterlist.GetDispositionById(id);
            if (dispositionResult == null)
                return BadRequest($"No Disposition found with id {id}");

            return Ok(dispositionResult);
        }

        [HttpGet("GetAllDispositionByStatus/{status}")]
        public async Task<ActionResult<IEnumerable<Disposition>>> GetAllDispositionByStatus(bool status)
        {
            var dispositionResult = await _unitOfWork.LabtestMasterlist.GetAllDispositionByStatus(status);
            if (dispositionResult == null)
                return BadRequest("No Result Found");
            return Ok(dispositionResult);
        }

        [HttpPut("UpdateDisposition")]
        public async Task<IActionResult> UpdateDisposition([FromBody] Disposition disposition)
        {
            var validateDisposition = await _unitOfWork.LabtestMasterlist.GetDispositionById(disposition.Id);
            var exisitingDeispositionName = await _unitOfWork.LabtestMasterlist.GetDispositonByName(disposition.DispositionName);
            if (exisitingDeispositionName != null)
                return BadRequest($"{disposition.DispositionName} is already exist");
            if (validateDisposition == null)
                return BadRequest("No paramaeters found.");
            await _unitOfWork.LabtestMasterlist.UpdateDisposition(disposition);
            await _unitOfWork.CompleteAsync();
            return Ok($" {validateDisposition.DispositionName} is sucessfully updated");
        }

        [HttpPut("UpdateDispositionStatus")]
        public async Task<IActionResult> UpdateDispositionStatus([FromBody] Disposition disposition)
        {
            string status;
            var validateDisposition = await _unitOfWork.LabtestMasterlist.GetDispositionById(disposition.Id);
            if (validateDisposition == null)
                return BadRequest("No paramaeters found.");
            await _unitOfWork.LabtestMasterlist.UpdateDispositionStatus(disposition);
            
            if (disposition.IsActive == true)
            {
                status = "Activated";
            }
            else
            {
                status = "Inactivated";
            }
            await _unitOfWork.CompleteAsync();
            return Ok($" {validateDisposition.DispositionName} is sucessfully {status}");
        }

        [HttpGet("GetAllDispositionPagination")]
        public async Task<ActionResult<IEnumerable<DispositionDto>>> GetAllDispositionPagination([FromQuery] bool status, [FromQuery] UserParams userParams)
        {
            var disposition = await _unitOfWork.LabtestMasterlist.GetAllDispositionPagination(status, userParams);

            Response.AddPaginationHeader(disposition.CurrentPage, disposition.PageSize, disposition.TotalCount, disposition.TotalPages, disposition.HasNextPage, disposition.HasPreviousPage);

            var dispositionResult = new
            {
                disposition,
                disposition.CurrentPage,
                disposition.PageSize,
                disposition.TotalCount,
                disposition.TotalPages,
                disposition.HasNextPage,
                disposition.HasPreviousPage
            };

            return Ok(dispositionResult);
        }

        [HttpGet("GetAllDispositionPaginationOrig/{status}")]
        public async Task<ActionResult<IEnumerable<DispositionDto>>> GetAllDispositionPaginationOrig([FromRoute] bool status, [FromQuery] string search,  [FromQuery] UserParams userParams)
        {
            if (search == null)
                return await GetAllDispositionPagination(status, userParams);

            var disposition = await _unitOfWork.LabtestMasterlist.GetAllDispositionPaginationOrig(search, status, userParams);

            Response.AddPaginationHeader(disposition.CurrentPage, disposition.PageSize, disposition.TotalCount, disposition.TotalPages, disposition.HasNextPage, disposition.HasPreviousPage);

            var dispositionResult = new
            {
                disposition,
                disposition.CurrentPage,
                disposition.PageSize,
                disposition.TotalCount,
                disposition.TotalPages,
                disposition.HasNextPage,
                disposition.HasPreviousPage
            };

            return Ok(dispositionResult);
        }
        #endregion

        #region Parameters

        [HttpPost]
        [Route("AddNewParameter")]
        public async Task<IActionResult> AddNewParameter(Parameters parameters)
        {
            var validateParameters = await _unitOfWork.LabtestMasterlist.GetParametersByName(parameters.ParameterName);
            if (validateParameters != null)
                return BadRequest($"Paramters {parameters.ParameterName} is already exist");
            await _unitOfWork.LabtestMasterlist.AddNewParameter(parameters);
            await _unitOfWork.CompleteAsync();
            return Ok("Parameter successfully added");
        }

        [HttpGet("GetAllParameter")]
        public async Task<ActionResult<IEnumerable<ParametersDto>>> GetAllParameters()
        {
            var parametersResult = await _unitOfWork.LabtestMasterlist.GetAllParameters();
            if (parametersResult == null) return BadRequest("No Result Found");
            return Ok(parametersResult);
        }
        [HttpGet("GetParametersById/{id}")]
        public async Task<ActionResult> GetParametersById([FromRoute] int id)
        {
            var parametersResult = await _unitOfWork.LabtestMasterlist.GetParametersById(id);
            return Ok(parametersResult);
        }
        [HttpGet("GetParametersByStatus/{status}")]
        public async Task<ActionResult<IEnumerable<Parameters>>> GetAllParametersByStatus([FromRoute] bool status)
        {
            var parametersResults = await _unitOfWork.LabtestMasterlist.GetAllParametersByStatus(status);
            return Ok(parametersResults);
        }

        [HttpGet("GetAllParametersPagination")]
        public async Task<ActionResult<IEnumerable<ParametersDto>>> GetAllParametersPagination([FromQuery] bool status, [FromQuery] UserParams userParams)
        {
            var parameters = await _unitOfWork.LabtestMasterlist.GetAllParametersPagination(status, userParams);
            Response.AddPaginationHeader(parameters.CurrentPage, parameters.PageSize, parameters.TotalCount, parameters.TotalPages, parameters.HasNextPage, parameters.HasPreviousPage);

            var parametersResult = new
            {
                parameters,
                parameters.CurrentPage,
                parameters.PageSize,
                parameters.TotalCount,
                parameters.TotalPages,
                parameters.HasNextPage,
                parameters.HasPreviousPage
            };

            return Ok(parametersResult);
        }

        [HttpGet("GetAllParametersPaginationOrig/{status}")]
        public async Task<ActionResult<IEnumerable<ParametersDto>>> GetAllParametersPaginationOrig([FromQuery] string search, [FromRoute] bool status, [FromQuery] UserParams userParams)
        {
            if (search == null)
                return await GetAllParametersPagination(status, userParams);

            var parameters = await _unitOfWork.LabtestMasterlist.GetAllParametersPaginationOrig(search, status, userParams);
            Response.AddPaginationHeader(parameters.CurrentPage, parameters.PageSize, parameters.TotalCount, parameters.TotalPages, parameters.HasNextPage, parameters.HasPreviousPage);

            var parametersResult = new
            {
                parameters,
                parameters.CurrentPage,
                parameters.PageSize,
                parameters.TotalCount,
                parameters.TotalPages,
                parameters.HasNextPage,
                parameters.HasPreviousPage
            };

            return Ok(parametersResult);
        }

        [HttpPut]
        [Route("UpdateParameter")]
        public async Task<IActionResult> UpdateParameter([FromBody] Parameters parameters)
        {
            var validateParameters = await _unitOfWork.LabtestMasterlist.GetParametersById(parameters.Id);
            if (validateParameters == null)
                return BadRequest($"Analysis with id {parameters.Id} is not exists");
            var validateParameterName = await _unitOfWork.LabtestMasterlist.GetParametersByName(parameters.ParameterName);
            if (validateParameterName != null)
                return BadRequest($"{parameters.ParameterName} is already exist");
            await _unitOfWork.LabtestMasterlist.UpdateParameters(parameters);
            await _unitOfWork.CompleteAsync();
            return Ok("Analysis updated successfully.");
        }

        [HttpPut("UpdateParameterStatus")]
        public async Task<IActionResult> UpdateParameterStatus([FromBody] Parameters parameters)
        {
            string status;
            var validateParameters = await _unitOfWork.LabtestMasterlist.GetParametersById(parameters.Id);
            if (validateParameters == null)
                return BadRequest("No paramaeters found.");
            await _unitOfWork.LabtestMasterlist.UpdateParameterStatus(parameters);
            
            if (parameters.IsActive == true)
            {
                status = "Activated";
            }
            else
            {
                status = "Inactivated";
            }
            await _unitOfWork.CompleteAsync();
            return Ok($" {validateParameters.ParameterName} is sucessfully {status}");
        }
        #endregion

    }
}