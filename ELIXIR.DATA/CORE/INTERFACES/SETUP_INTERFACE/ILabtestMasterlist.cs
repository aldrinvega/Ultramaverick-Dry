using System.Collections.Generic;
using System.Threading.Tasks;
using ELIXIR.DATA.DATA_ACCESS_LAYER.HELPERS;
using ELIXIR.DATA.DATA_ACCESS_LAYER.MODELS.SETUP_MODEL;
using ELIXIR.DATA.DTOs.SETUP_DTOs;

namespace ELIXIR.DATA.CORE.INTERFACES.SETUP_INTERFACE
{
    public interface ILabtestMasterlist
    {
        #region Sample Types
        Task<bool> AddNewSampleType(SampleType sampleType);
        Task<bool> UpdateSampleType(SampleType sampleTypes);
        Task<IReadOnlyList<SampleType>> GetAllSampleType();
        Task<SampleType> GetSampleTypeById(int id);
        Task<SampleType> GetSampleTypeByName(string sampleTypeName);
        Task<PagedList<SampleTypeDto>> GetAllSampleTypePagination(UserParams userParams);
        Task<bool> UpdateSampleTypeStatus(SampleType sampleTypes);

        #endregion

        #region Type Of Swabs

        Task<bool> AddNewTypeOfSwab(TypeOfSwab typeOfSwab);
        Task<bool> UpdateTypeOfSwab(TypeOfSwab typeOfSwab);
        Task<IReadOnlyList<TypeOfSwab>> GetAllTypeOfSwab();
        Task<TypeOfSwab> GetTypeOfSwabById(int id);
        Task<PagedList<TypeOfSwabDto>> GetAllTypeOfSwabPagination(UserParams userParams);
        Task<bool> UpdateTypeOfSwabStatus(TypeOfSwab typeOfSwab);

        #endregion

        #region Analysis

        Task<bool> AddNewAnalysis(Analysis analysis);
        Task<bool> UpdateAnalysis(Analysis analysis);
        Task<IReadOnlyList<Analysis>> GetAllAnalysis();
        Task<PagedList<AnalysesDto>> GetAllAnalysesPagination(UserParams userParams);
        Task<bool> UpdateAnalysisStatus(Analysis analysis);
        Task<Analysis> GetAnalysisById(int id);

        #endregion

        #region Parameteres

        Task<bool> AddNewParameter(Parameters parameters);
        Task<bool> UpdateParameters(Parameters parameters);
        Task<Parameters> GetParametersById(int id);
        Task<PagedList<ParametersDto>> GetAllParametersPagination(UserParams userParams);
        Task<bool> UpdateParameterStatus(Parameters parameters);

        #endregion

        #region Product Condition

        Task<bool> AddNewProductCondition(ProductCondition productCondition);
        Task<bool> UpdateProductCondition(ProductCondition productCondition);
        Task<IReadOnlyList<ProductCondition>> GetAllProductCondition();
        Task<ProductCondition> GetProductConditionById(int id);
        Task<PagedList<ProductConditionDto>> GetAllProductConditionPagination(UserParams userParams);
        Task<bool> UpdateProductConditionStatus(ProductCondition productCondition);
        Task<ProductCondition> GetProductConditionByName(string productConditionName);

        #endregion

        #region Disposition
        Task<bool> AddNewDisposition(Disposition disposition);
        Task<bool> UpdateProductDisposition(Disposition disposition);
        Task<IReadOnlyList<Disposition>> GetAllDisposition();
        Task<Disposition> GetDispositionById(int id);
        Task<PagedList<DispositionDto>> GetAllDispositionPagination(UserParams userParams);
        Task<bool> UpdateDispositionStatus(Disposition disposition);
        #endregion
    }
}