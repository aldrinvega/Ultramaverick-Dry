using System.Collections.Generic;
using System.Threading.Tasks;
using ELIXIR.DATA.DATA_ACCESS_LAYER.HELPERS;
using ELIXIR.DATA.DATA_ACCESS_LAYER.MODELS.LABTEST_MODEL;
using ELIXIR.DATA.DTOs.LABORATORYTEST_DTO;

namespace ELIXIR.DATA.CORE.INTERFACES.LABTEST_INTERFACE
{
    public interface ILabTestRepository
    {
        Task<bool> RequestItemsForLabTest(IEnumerable<LabTestRequests> labTestRequests);
        Task<PagedList<NearlyExpiryitemsDTO>> GetAllNearlyExpiryItemsPagination(UserParams userParams);
        Task<IReadOnlyList<NearlyExpiryitemsDTO>> GetAllNearlyExpiryItemsCount();
        Task<IEnumerable<LabTestRequests>> GetAllRequestedItemsForLabTest();

        //Accept Lab Test Request
        Task<bool> AcceptLabTestRequest(ReceiveRequest receiveRequest);
        Task<IEnumerable<ReceiveRequest>> GetAllAcceptedItemsForLabTest();
        Task<PagedList<ReceiveRequest>> GetAllAcceptedItemsForLabTestPagination(UserParams userParams);

        //Return Items
        Task<bool> ReturnItemsRequestedForLabTest(ReturnedItems returnItems, int labTestRequestId);
        Task<IEnumerable<ReturnedItemsDTO>> GetAllReturnedItems();
        Task<PagedList<ReturnedItemsDTO>> GetAllReturnedItemsPagination(UserParams userParams);
        
        //Reject items
        Task<bool> RejectItemsForLabTest(RejectedItems rejectItems, int labTestRequestId);
        Task<IEnumerable<RejectItemsDTO>> GetAllRejectedItemsForLabTest();
        Task<PagedList<RejectItemsDTO>> GetAllRejectedItemsForLabTestPagination(UserParams userParams);
        
        
    }
}