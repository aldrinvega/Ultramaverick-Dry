using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Threading.Tasks;
using ELIXIR.DATA.CORE.INTERFACES.SETUP_INTERFACE;
using ELIXIR.DATA.DATA_ACCESS_LAYER.HELPERS;
using ELIXIR.DATA.DATA_ACCESS_LAYER.MODELS.SETUP_MODEL;
using ELIXIR.DATA.DATA_ACCESS_LAYER.STORE_CONTEXT;
using ELIXIR.DATA.DTOs.SETUP_DTOs;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace ELIXIR.DATA.DATA_ACCESS_LAYER.REPOSITORIES.SETUP_REPOSITORY
{
    public class LabTestMasterList : ILabtestMasterlist
    {
        private readonly StoreContext _context;
        public LabTestMasterList(StoreContext context)
        {
            _context = context;
        }

        #region Sample Type
        
        public async Task<bool> AddNewSampleType(SampleType sampleType)
        {
            await _context.SampleTypes.AddAsync(sampleType);
            return true;
        }

        public async Task<bool> UpdateSampleType(SampleType sampleTypes)
        {
            var sampleType = await _context.SampleTypes.FirstOrDefaultAsync(x => x.Id == sampleTypes.Id);
            if (sampleType == null)
                return false;
            sampleType.SampleTypeName = sampleTypes.SampleTypeName;
            sampleType.Reason = sampleTypes.Reason;
            sampleType.ModifiedBy = sampleTypes.SampleTypeName;
            return true;
        }

        public async Task<IReadOnlyList<SampleType>> GetAllSampleType()
        {
            return await _context.SampleTypes.ToListAsync();
        }

        public async Task<SampleType> GetSampleTypeById(int id)
        {
            return await _context.SampleTypes.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<SampleType> GetSampleTypeByName(string sampleTypeName)
        {
            return await _context.SampleTypes.FirstOrDefaultAsync(x => x.SampleTypeName == sampleTypeName);
        }
        public async Task<PagedList<SampleTypeDto>> GetAllSampleTypePagination(bool status, UserParams userParams)
        {
            var sampleType = _context.SampleTypes.Where(x => x.IsActive == status).Select(x => new SampleTypeDto
            {
                SampleTypeName = x.SampleTypeName,
                IsActive = x.IsActive,
                Reason = x.Reason,
                DateAdded = x.DateAdded,
                ModifiedBy = x.ModifiedBy
            });

            return await PagedList<SampleTypeDto>.CreateAsync(sampleType, userParams.PageNumber, userParams.PageSize);

        }
        public async Task<PagedList<SampleTypeDto>> GetAllSampleTypePaginationOrig(string search, bool status, UserParams userParams)
        {
            var sampleType = _context.SampleTypes.Where(x => x.IsActive == true).Select(x => new SampleTypeDto
            {
                SampleTypeName = x.SampleTypeName,
                IsActive = x.IsActive,
                Reason = x.Reason,
                DateAdded = x.DateAdded,
                ModifiedBy = x.ModifiedBy
            }).OrderBy(x => x.SampleTypeName)
              .Where(x => x.IsActive == status)
              .Where(x => x.SampleTypeName.ToLower()
              .Contains(search.Trim().ToLower()));

            return await PagedList<SampleTypeDto>.CreateAsync(sampleType, userParams.PageNumber, userParams.PageSize);
        }

        public async Task<PagedList<SampleTypeDto>> GetAllSampleTypeByStatusPagination(bool status, UserParams userParams)
        {
            var sampleType = _context.SampleTypes.Where(x => x.IsActive == status).Select(x => new SampleTypeDto
            {
                SampleTypeName = x.SampleTypeName,
                IsActive = x.IsActive,
                Reason = x.Reason,
                DateAdded = x.DateAdded,
                ModifiedBy = x.ModifiedBy
            }).OrderBy(x => x.SampleTypeName)
             .Where(x => x.IsActive == status);

            return await PagedList<SampleTypeDto>.CreateAsync(sampleType, userParams.PageNumber, userParams.PageSize);
        }

        public async Task<PagedList<SampleTypeDto>> GetAllSampleTypeByStatusPaginationOrig(string search, bool status, UserParams userParams)
        {
            var sampleType = _context.SampleTypes.Where(x => x.IsActive == status).Select(x => new SampleTypeDto
            {
                SampleTypeName = x.SampleTypeName,
                IsActive = x.IsActive,
                Reason = x.Reason,
                DateAdded = x.DateAdded,
                ModifiedBy = x.ModifiedBy
            }).OrderBy(x => x.SampleTypeName)
             .Where(x => x.IsActive == status)
               .Where(x => x.SampleTypeName.ToLower()
              .Contains(search.Trim().ToLower()));

            return await PagedList<SampleTypeDto>.CreateAsync(sampleType, userParams.PageNumber, userParams.PageSize);

        }

        public async Task<bool> UpdateSampleTypeStatus(SampleType sampleTypes)
        {
            var sampleType = await _context.SampleTypes.FirstOrDefaultAsync(x => x.Id == sampleTypes.Id);
            if (sampleType == null)
                return false;
            sampleType.IsActive = sampleTypes.IsActive;
            sampleType.ModifiedBy = sampleTypes.ModifiedBy;
            return true;
        }
        #endregion

        #region Type Of Swab
        
        public async Task<bool> AddNewTypeOfSwab(TypeOfSwab typeOfSwab)
        {
            await _context.TypeOfSwabs.AddAsync(typeOfSwab);
            return true;
        }

        public async Task<bool> UpdateTypeOfSwab(TypeOfSwab typeOfSwab)
        {
            var typeOfSwabs = await _context.TypeOfSwabs.FirstOrDefaultAsync(x => x.Id == typeOfSwab.Id);

            if (typeOfSwab == null)
                return false;

            typeOfSwabs.TypeofSwabName = typeOfSwab.TypeofSwabName;
            typeOfSwabs.Reason = typeOfSwab.Reason;
            typeOfSwabs.ModifiedBy = typeOfSwab.ModifiedBy;

            return true;
        }

        public async Task<IReadOnlyList<TypeOfSwab>> GetAllTypeOfSwab()
        {
            return await _context.TypeOfSwabs.ToListAsync();
        }

        public async Task<TypeOfSwab> GetTypeOfSwabById(int id)
        {
            return await _context.TypeOfSwabs.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<PagedList<TypeOfSwabDto>> GetTypeOfSwabByStatusPagination(bool status, UserParams userParams)
        {
            var typeofSwabs = _context.TypeOfSwabs.Where(x => x.IsActive == status).Select(x => new TypeOfSwabDto
            {
                TypeofSwabName = x.TypeofSwabName,
                IsActive = x.IsActive,
                DateAdded = x.DateAdded,
                ModifiedBy = x.ModifiedBy,
                Reason = x.Reason
            });

            return await PagedList<TypeOfSwabDto>.CreateAsync(typeofSwabs, userParams.PageNumber, userParams.PageSize);
        }

        public async Task<PagedList<TypeOfSwabDto>> GetTypeOfSwabByStatusPaginationOrig(string search, bool status, UserParams userParams)
        {
            var typeofSwabs = _context.TypeOfSwabs.Where(x => x.IsActive == status).Select(x => new TypeOfSwabDto
            {
                TypeofSwabName = x.TypeofSwabName,
                IsActive = x.IsActive,
                DateAdded = x.DateAdded,
                ModifiedBy = x.ModifiedBy,
                Reason = x.Reason
            }).OrderBy(x => x.TypeofSwabName)
              .Where(x => x.IsActive == status)
              .Where(x => x.TypeofSwabName.ToLower()
              .Contains(search.Trim().ToLower()));

            return await PagedList<TypeOfSwabDto>.CreateAsync(typeofSwabs, userParams.PageNumber, userParams.PageSize);
        }

        public async Task<PagedList<TypeOfSwabDto>> GetAllTypeOfSwabPagination(bool status,UserParams userParams)
        {
            var typeofSwabs = _context.TypeOfSwabs.Where(x => x.IsActive == status).Select(x => new TypeOfSwabDto
            {
                TypeofSwabName = x.TypeofSwabName,
                IsActive = x.IsActive,
                DateAdded = x.DateAdded,
                ModifiedBy = x.ModifiedBy,
                Reason = x.Reason
            });

            return await PagedList<TypeOfSwabDto>.CreateAsync(typeofSwabs, userParams.PageNumber, userParams.PageSize);
        }
        public async Task<PagedList<TypeOfSwabDto>> GetAllTypeOfSwabPaginationOrig(string search, bool status, UserParams userParams)
        {
            var typeofSwabs = _context.TypeOfSwabs.Where(x => x.IsActive == true).Select(x => new TypeOfSwabDto
            {
                TypeofSwabName = x.TypeofSwabName,
                IsActive = x.IsActive,
                DateAdded = x.DateAdded,
                ModifiedBy = x.ModifiedBy,
                Reason = x.Reason
            }).OrderBy(x => x.TypeofSwabName)
              .Where(x => x.IsActive == status)
              .Where(x => x.TypeofSwabName.ToLower()
              .Contains(search.Trim().ToLower()));

            return await PagedList<TypeOfSwabDto>.CreateAsync(typeofSwabs, userParams.PageNumber, userParams.PageSize);
        }

        public async Task<bool> UpdateTypeOfSwabStatus(TypeOfSwab typeOfSwab)
        {
            var typeOfSwabResult = await _context.TypeOfSwabs.FirstOrDefaultAsync(x => x.Id == typeOfSwab.Id);
            if (typeOfSwabResult == null)
                return false;
            typeOfSwabResult.IsActive = typeOfSwab.IsActive;
            typeOfSwabResult.ModifiedBy = typeOfSwab.ModifiedBy;
            return true;
        }
        #endregion

        #region Analyses
        public async Task<bool> AddNewAnalysis(Analysis analysis)
        {
            await _context.Analyses.AddAsync(analysis);
            return true;
        }

        public async Task<bool> UpdateAnalysis(Analysis analysis)
        {
            var analysisResult = await _context.Analyses.FirstOrDefaultAsync(x => x.Id == analysis.Id);
            if (analysisResult == null)
                return false;
            analysisResult.AnalysisName = analysis.AnalysisName;
            analysisResult.Reason = analysis.Reason;
            analysisResult.ModifiedBy = analysis.ModifiedBy;
            return true;
        }

        public async Task<IReadOnlyList<Analysis>> GetAllAnalysis()
        {
            return await _context.Analyses.ToListAsync();
        }

        public async Task<Analysis> GetAnalysisById(int id)
        {
            return await _context.Analyses.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<PagedList<AnalysesDto>> GetAllAnalysesPagination(bool status, UserParams userParams)
        {
            var analysesResult = _context.Analyses.Where(x => x.IsActive == status).Select(x => new AnalysesDto
            {
                AnalysisName = x.AnalysisName,
                IsActive = x.IsActive,
                Reason = x.Reason,
                DateAdded = x.DateAdded.ToString(),
                ModifiedBy = x.ModifiedBy
            });
            return await PagedList<AnalysesDto>.CreateAsync(analysesResult, userParams.PageNumber, userParams.PageSize);
        }
        public async Task<PagedList<AnalysesDto>> GetAllAnalysesPaginationOrig(string search, bool status, UserParams userParams)
        {
            var analysesResult = _context.Analyses.Where(x => x.IsActive == true).Select(x => new AnalysesDto
            {
                AnalysisName = x.AnalysisName,
                IsActive = x.IsActive,
                Reason = x.Reason,
                DateAdded = x.DateAdded.ToString(),
                ModifiedBy = x.ModifiedBy
            }).OrderBy(x => x.AnalysisName)
              .Where(x => x.IsActive == status)
              .Where(x => x.AnalysisName.ToLower()
              .Contains(search.Trim().ToLower()));

            return await PagedList<AnalysesDto>.CreateAsync(analysesResult, userParams.PageNumber, userParams.PageSize);
        }

        public async Task<PagedList<AnalysesDto>> GetAnalysisByStatusPaginationOrig(string search, bool status, UserParams userParams)
        {
            var analysesResult = _context.Analyses.Where(x => x.IsActive == true).Select(x => new AnalysesDto
            {
                AnalysisName = x.AnalysisName,
                IsActive = x.IsActive,
                Reason = x.Reason,
                DateAdded = x.DateAdded.ToString(),
                ModifiedBy = x.ModifiedBy
            }).OrderBy(x => x.AnalysisName)
              .Where(x => x.IsActive == status)
              .Where(x => x.AnalysisName.ToLower()
              .Contains(search.Trim().ToLower()));

            return await PagedList<AnalysesDto>.CreateAsync(analysesResult, userParams.PageNumber, userParams.PageSize);
        }
        public async Task<bool> UpdateAnalysisStatus(Analysis analysis)
        {
            var analysisResult = await _context.Analyses.FirstOrDefaultAsync(x => x.Id == analysis.Id);
            if (analysisResult == null)
                return false;
            analysisResult.IsActive = analysis.IsActive;
            analysisResult.ModifiedBy = analysis.ModifiedBy;
            return true;
        }

        #endregion

        #region Parameters

        public async Task<bool> AddNewParameter(Parameters parameters)
        {
            await _context.Parameters.AddAsync(parameters);
            return true;
        }

        public async Task<bool> UpdateParameters(Parameters parameters)
        {
            var parameter = await _context.Parameters.FirstOrDefaultAsync(x => x.Id == parameters.Id);
            if (parameter == null)
                return false;
            parameter.ParameterName = parameters.ParameterName;
            parameter.Reason = parameters.Reason;
            parameter.IsActive = parameters.IsActive;
            parameter.ModifiedBy = parameters.ModifiedBy;

            return true;
        }

        public async Task<IReadOnlyList<Parameters>> GetAllParameters()
        {
            return await _context.Parameters.ToListAsync();
        }

        public async Task<Parameters> GetParametersById(int id)
        {
            return await _context.Parameters.FirstOrDefaultAsync(x => x.Id == id);
        }
        
        public async Task<PagedList<ParametersDto>> GetAllParametersPagination(bool status, UserParams userParams)
        {
            var parammeters = _context.Parameters.Where(x => x.IsActive == status).Select(x => new ParametersDto
            {
                Id = x.Id,
                ParameterName = x.ParameterName,
                DateAdded = x.DateAdded.ToString(),
                IsActive = x.IsActive,
                Reason = x.Reason,
                ModifiedBy = x.ModifiedBy,
            });

            return await PagedList<ParametersDto>.CreateAsync(parammeters, userParams.PageNumber, userParams.PageSize);
        }

        public async Task<PagedList<ParametersDto>> GetAllParametersPaginationOrig(string search, bool status, UserParams userParams)
        {
            var parammeters = _context.Parameters.Where(x => x.IsActive == true).Select(x => new ParametersDto
            {
                Id = x.Id,
                ParameterName = x.ParameterName,
                DateAdded = x.DateAdded.ToString(),
                IsActive = x.IsActive,
                Reason = x.Reason,
                ModifiedBy = x.ModifiedBy,
            }).OrderBy(x => x.ParameterName)
              .Where(x => x.IsActive == status)
              .Where(x => x.ParameterName.ToLower()
              .Contains(search.Trim().ToLower()));

            return await PagedList<ParametersDto>.CreateAsync(parammeters, userParams.PageNumber, userParams.PageSize);
        }

        public async Task<bool> UpdateParameterStatus(Parameters parameters)
        {
            var parametersResult = await _context.Parameters.FirstOrDefaultAsync(x => x.Id == parameters.Id);
            if (parametersResult == null)
                return false;
            parametersResult.IsActive = parameters.IsActive;
            parametersResult.ModifiedBy = parameters.ModifiedBy;

            return true;
        }
        #endregion

        #region Product Condition

        public async Task<bool> AddNewProductCondition(ProductCondition productCondition)
        {
            await _context.ProductConditions.AddAsync(productCondition);
            return true;
        }

        public async Task<bool> UpdateProductCondition(ProductCondition productCondition)
        {
            var productConditionResult =
                await _context.ProductConditions.FirstOrDefaultAsync(x => x.Id == productCondition.Id);

            if (productConditionResult == null)
                return false;
            productConditionResult.ProductConditionName = productCondition.ProductConditionName;
            productConditionResult.DateAdded = productCondition.DateAdded;
            productConditionResult.IsActive = productCondition.IsActive;
            productConditionResult.Reason = productCondition.Reason;
            return true;
        }

        public async Task<IReadOnlyList<ProductCondition>> GetAllProductCondition()
        {
            return await _context.ProductConditions.ToListAsync();
        }

        public async Task<ProductCondition> GetProductConditionById(int id)
        {
            return await _context.ProductConditions.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<ProductCondition> GetProductConditionByName(string productConditionName)
        {
            return await _context.ProductConditions.FirstOrDefaultAsync(x =>
                x.ProductConditionName == productConditionName);
        }
        public async Task<PagedList<ProductConditionDto>> GetAllProductConditionPagination(bool status, UserParams userParams)
        {
            var productConditions = _context.ProductConditions.Where(x => x.IsActive == status).Select(x =>
                new ProductConditionDto
                {
                    ProductConditionName = x.ProductConditionName,
                    IsActive = x.IsActive,
                    Reason = x.Reason,
                    DateAdded = x.DateAdded,
                    ModifiedBy = x.ModifiedBy,
                });
            return await PagedList<ProductConditionDto>.CreateAsync(productConditions, userParams.PageNumber,
                userParams.PageSize);
        }

        public async Task<PagedList<ProductConditionDto>> GetAllProductConditionPaginationOrig(string search, bool status, UserParams userParams)
        {
            var productConditions = _context.ProductConditions.Where(x => x.IsActive == true).Select(x =>
                new ProductConditionDto
                {
                    ProductConditionName = x.ProductConditionName,
                    IsActive = x.IsActive,
                    Reason = x.Reason,
                    DateAdded = x.DateAdded,
                    ModifiedBy = x.ModifiedBy,
                }).OrderBy(x => x.ProductConditionName)
              .Where(x => x.IsActive == status)
              .Where(x => x.ProductConditionName.ToLower()
              .Contains(search.Trim().ToLower()));

            return await PagedList<ProductConditionDto>.CreateAsync(productConditions, userParams.PageNumber,
                userParams.PageSize);
        }

        public async Task<bool> UpdateProductConditionStatus(ProductCondition productCondition)
        {
            var productConditionResult =
                await _context.ProductConditions.FirstOrDefaultAsync(x => x.Id == productCondition.Id);
            if (productConditionResult == null)
                return false;

            productConditionResult.IsActive = productCondition.IsActive;
            productConditionResult.ModifiedBy = productCondition.ModifiedBy;

            return true;

        }
        #endregion

        #region Disposition

        public async Task<bool> AddNewDisposition(Disposition disposition)
        { 
            await _context.Dispositions.AddAsync(disposition);
            return true;
        }

        public async Task<bool> UpdateProductDisposition(Disposition disposition)
        {
            var dispositionResult = await _context.Dispositions.FirstOrDefaultAsync(x => x.Id == disposition.Id);

            if (dispositionResult == null)
                return false;
            dispositionResult.DispositionName = disposition.DispositionName;
            dispositionResult.ModifiedBy = disposition.ModifiedBy;
            dispositionResult.Reason = disposition.Reason;
            return true;
        }

        public async Task<IReadOnlyList<Disposition>> GetAllDisposition()
        {
            return await _context.Dispositions.ToListAsync();
        }

        public async Task<Disposition> GetDispositionById(int id)
        {
            return await _context.Dispositions.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<PagedList<DispositionDto>> GetAllDispositionPagination(bool status, UserParams userParams)
        {
            var dispositions = _context.Dispositions.Where(x => x.IsActive == true).Select(x => new DispositionDto
            {
                Id = x.Id,
                DispositionName = x.DispositionName,
                DateAdded = x.DateAdded,
                Reason = x.Reason,
                ModifiedBy = x.ModifiedBy
            });

            return await PagedList<DispositionDto>.CreateAsync(dispositions, userParams.PageNumber,
                userParams.PageSize);
        }
        public async Task<PagedList<DispositionDto>> GetAllDispositionPaginationOrig(string search, bool status, UserParams userParams)
        {
            var dispositions = _context.Dispositions.Where(x => x.IsActive == true).Select(x => new DispositionDto
            {
                Id = x.Id,
                DispositionName = x.DispositionName,
                DateAdded = x.DateAdded,
                Reason = x.Reason,
                ModifiedBy = x.ModifiedBy
            }).OrderBy(x => x.DispositionName)
              .Where(x => x.IsActive == status)
              .Where(x => x.DispositionName.ToLower()
              .Contains(search.Trim().ToLower()));

            return await PagedList<DispositionDto>.CreateAsync(dispositions, userParams.PageNumber,
                userParams.PageSize);
        }

        public async Task<bool> UpdateDispositionStatus(Disposition disposition)
        {
            var dispositionResult = await _context.Dispositions.FirstOrDefaultAsync(x => x.Id == disposition.Id);

            if (dispositionResult == null)
                return false;

            dispositionResult.IsActive = disposition.IsActive;
            dispositionResult.ModifiedBy = disposition.ModifiedBy;

            return true;
        }
        #endregion
    }
}