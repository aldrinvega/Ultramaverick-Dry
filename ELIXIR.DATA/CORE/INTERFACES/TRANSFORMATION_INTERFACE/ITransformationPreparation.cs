using ELIXIR.DATA.DATA_ACCESS_LAYER.MODELS.TRANSFORMATION_MODEL;
using ELIXIR.DATA.DATA_ACCESS_LAYER.MODELS.WAREHOUSE_MODEL;
using ELIXIR.DATA.DTOs.TRANSFORMATION_DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELIXIR.DATA.CORE.INTERFACES.TRANSFORMATION_INTERFACE
{
    public interface ITransformationPreparation
    {


        Task<IReadOnlyList<TransformationPreparationDto>> GetAllListOfTransformationByTransformationId(TransformationPlanning planning);

        Task<bool> PrepareTransformationMaterials(TransformationPreparation preparation);
        Task<bool> AddPreparationMaterials(TransformationPreparation preparation);


        Task<bool> UpdatePrepareStatusInRequest(int id);
        Task<bool> ValidatePreparedMaterials(int id, string code);
        Task<bool> ValidateIfApproved(int id);
        Task<IReadOnlyList<TransformationPlanningDto>> GetAllListOfTransformationRequestForMixing();
        Task<IReadOnlyList<TransformationMixingRequirements>> GetAllRequirementsForMixing(int id);

        Task<bool> AddMixingTransformation(WarehouseReceiving warehouse);
        Task<bool> FinishedMixedMaterialsForWarehouse(WarehouseReceiving warehouse);

        Task<RawmaterialDetailsFromWarehouseDto>GetReceivingDetailsForRawmaterials(string code);
        Task<bool> UpdatedWarehouseStock(string code);


        Task<IReadOnlyList<TransformationPlanningDto>> GetAllTransformationFormulaInformation();

        Task<decimal> ValidatePreparedItems(TransformationPreparation preparation);

        Task<IReadOnlyList<ForTesting>> GetAllAvailableStocks();

        Task<IReadOnlyList<ItemStocks>> GetAllRemainingStocksPerReceivingId(string itemcode);

    }
}
