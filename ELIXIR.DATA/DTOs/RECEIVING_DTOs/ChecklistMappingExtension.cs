using System.Linq;
using ELIXIR.DATA.DATA_ACCESS_LAYER.MODELS.QC_CHECKLIST;
using ELIXIR.DATA.DATA_ACCESS_LAYER.MODELS.SETUP_MODEL;
using ELIXIR.DATA.DATA_ACCESS_LAYER.REPOSITORIES.QC_REPOSITORY;

namespace ELIXIR.DATA.DTOs.RECEIVING_DTOs
{
    /*public static class ChecklistMappingExtension
    {
        public static GetAllChecklists.GetAllChecklistsQueryResult
            ChecklistsQueryResult(this ProductType productType)
        {
            return new GetAllChecklists.GetAllChecklistsQueryResult
            {
                ProductTypeId = productType.Id,
                ProductType = productType.ProductTypeName,
                ChecklistDescription = productType.ChecklistDescription.Select(x => new GetAllChecklists.GetAllChecklistsQueryResult.ChecklistDescriptions
                {
                    Id = x.Id,
                    ChecklistDescription = x.ChecklistQuestion,
                    IsActive = x.IsActive,
                    CreatedAt = x.CreatedAt,
                    UpdatedAt = x.UpdatedAt,
                    AddedBy = x.AddedByUser.FullName
                }).ToList()
            };
        }
    }*/
}