using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ELIXIR.DATA.CORE.INTERFACES.LABTEST_INTERFACE;
using ELIXIR.DATA.DATA_ACCESS_LAYER.HELPERS;
using ELIXIR.DATA.DATA_ACCESS_LAYER.MODELS.LABTEST_MODEL;
using ELIXIR.DATA.DATA_ACCESS_LAYER.STORE_CONTEXT;
using ELIXIR.DATA.DTOs.LABORATORYTEST_DTO;
using Microsoft.EntityFrameworkCore;

namespace ELIXIR.DATA.DATA_ACCESS_LAYER.REPOSITORIES.LABTEST_REPOSITORY
{
    public class LabTestRepository : ILabTestRepository
    {
        private readonly StoreContext _context;

        public LabTestRepository(StoreContext context)
        {
            _context = context;
        }
        
        //Nearly Expiry
        
        public async Task<PagedList<NearlyExpiryitemsDTO>> GetAllNearlyExpiryItemsPagination(UserParams userParams)
        {
            var today = DateTime.Now.Date;
            var targetDate = today.AddDays(10);
            var items = _context.WarehouseReceived
                .Where(x => x.Expiration <= targetDate)
                .Select(items => new NearlyExpiryitemsDTO
                {
                    Id = items.Id,
                    ItemCode = items.ItemCode,
                    ItemDescription = items.ItemDescription,
                    ExpirationDate = items.Expiration.ToString(),
                    WarehouseId = items.Id,
                });

            return await PagedList<NearlyExpiryitemsDTO>.CreateAsync(items, userParams.PageNumber, userParams.PageSize);
        }
        public async Task<bool> RequestItemsForLabTest(IEnumerable<LabTestRequests> labTestRequests)
        {
            foreach (var labTestRequest in labTestRequests)
            {
                var validateItemForLabTest = await
                    _context.LabTestRequests.FirstOrDefaultAsync(x =>
                        x.WarehouseReceivingId == labTestRequest.WarehouseReceivingId);
                var lastBatchNo =
                    await _context.LabTestRequests.OrderByDescending(x => x.BatchId).FirstOrDefaultAsync();
                if (validateItemForLabTest != null)
                {
                    throw new Exception(
                        $"Item Code {labTestRequest.WarehouseReceived.ItemCode} with warehouseId {labTestRequest.WarehouseReceivingId} is already requested");
                }

                var requestedItemsForLabTest = new LabTestRequests
                {
                    WarehouseReceivingId = labTestRequest.WarehouseReceivingId,
                    ProductCondition = labTestRequest.ProductCondition,
                    TestType = labTestRequest.TestType,
                    Quantity = labTestRequest.Quantity,
                    DateNeeded = labTestRequest.DateNeeded,
                    SampleType = labTestRequest.SampleType,
                    TypeOfSwab = labTestRequest.TypeOfSwab,
                    Analysis = labTestRequest.Analysis,
                    Parameters = labTestRequest.Parameters,
                    Remarks = labTestRequest.Remarks,
                    BatchId = lastBatchNo.BatchId + 1,
                    Status = "Requested",
                };

                await _context.LabTestRequests.AddAsync(requestedItemsForLabTest);
                await _context.SaveChangesAsync();
            }

            return true;

        }
        public async Task<IReadOnlyList<NearlyExpiryitemsDTO>> GetAllNearlyExpiryItemsCount()
        {
            var today = DateTime.Now.Date;
            var targetDate = today.AddDays(10);
            var items = await _context.WarehouseReceived
                .Where(x => x.Expiration <= targetDate)
                .Select(items => new NearlyExpiryitemsDTO
                {
                    Id = items.Id,
                    ItemCode = items.ItemCode,
                    ItemDescription = items.ItemDescription,
                    ExpirationDate = items.Expiration.ToString(),
                    WarehouseId = items.Id,
                }).ToListAsync();
            return items;
        }
        public async Task<IEnumerable<LabTestRequests>> GetAllRequestedItemsForLabTest()
        {
            var requestedItems = await _context.LabTestRequests.Where(x => x.Status == "Requested").ToListAsync();
            if (requestedItems == null)
            {
                throw new Exception("No result Found");
            }

            return requestedItems;

        }

        //Receive LabTest Request

        public async Task<bool> AcceptLabTestRequest(ReceiveRequest receiveRequest)
        {
            var validateLabTestRequest = await _context.ReceiveRequests
                .FirstOrDefaultAsync(x => x.LabTestRequestsId == receiveRequest.LabTestRequestsId);

            if (validateLabTestRequest == null)
            {
                return false;
            }

            var acceptedLabRequests = new ReceiveRequest
            {
                LabTestRequestsId = receiveRequest.LabTestRequestsId,
                Status = "Accepted For LabTest",
                Disposition = receiveRequest.Disposition
            };

            await _context.ReceiveRequests.AddAsync(acceptedLabRequests);
            await _context.SaveChangesAsync();
            return true;
        }
        public async Task<IEnumerable<ReceiveRequest>> GetAllAcceptedItemsForLabTest()
        {
            var acceptedItemForLabTest = await _context.ReceiveRequests
                .Include(x => x.LabTestRequests)
                .ToListAsync();

            if (acceptedItemForLabTest != null)
            {
                return acceptedItemForLabTest;
            }

            throw new Exception("No Available Items");
        }
        public async Task<PagedList<ReceiveRequest>> GetAllAcceptedItemsForLabTestPagination(UserParams userParams)
        {
            var acceptedItemForLabTest = _context.ReceiveRequests
                .Include(x => x.LabTestRequests);

            if (acceptedItemForLabTest == null)
            {
                throw new Exception("No Items Found");
            }

            return await PagedList<ReceiveRequest>.CreateAsync(acceptedItemForLabTest, userParams.PageNumber,
                userParams.PageSize);

        }

        //Return Item to Warehouse Supervisor
        
        public async Task<bool> ReturnItemsRequestedForLabTest(ReturnedItems returnItems, int labTestRequestId)
        {
            var validateRequestedItem =
                await _context.LabTestRequests.FirstOrDefaultAsync(x => x.Id == labTestRequestId);
        
            if (validateRequestedItem == null)
                throw new Exception("Request Not Found");
        
            validateRequestedItem.Status = "Returned";

            await _context.ReturnedItems.AddAsync(returnItems);
            await _context.SaveChangesAsync();
            return true;
        }
        public async Task<IEnumerable<ReturnedItemsDTO>> GetAllReturnedItems()
        {
            var returnedItems = await _context.ReturnedItems
                .Where(x => x.Status == "For Approval")
                .Include(x => x.LabTestRequest)
                .Select(x => new ReturnedItemsDTO
                {
                    WarehouseId = x.LabTestRequest.WarehouseReceivingId,
                    SampleName = x.LabTestRequest.WarehouseReceived.ItemCode,
                    OriginalExpirationDate = x.LabTestRequest.WarehouseReceived.Expiration,
                    Quantity = x.LabTestRequest.Quantity,
                    AllowableDate = x.AllowableDate,
                    DaysToExpire = x.LabTestRequest.WarehouseReceived.Expiration.HasValue
                        ? (int)(x.LabTestRequest.WarehouseReceived.Expiration.Value - DateTime.Now).TotalDays
                        : 0,
                    Status = x.Status,
                    Remarks = x.Reason
                })
                .ToListAsync();

            if (returnedItems == null)
            {
                throw new Exception("No result found!");
            }

            return returnedItems;

        }
        public async Task<PagedList<ReturnedItemsDTO>> GetAllReturnedItemsPagination(UserParams userParams)
        {
            var returnedItems = _context.ReturnedItems
                .Where(x => x.Status == "For Approval")
                .Include(x => x.LabTestRequest)
                .Select(x => new ReturnedItemsDTO
                {
                    WarehouseId = x.LabTestRequest.WarehouseReceivingId,
                    SampleName = x.LabTestRequest.WarehouseReceived.ItemCode,
                    OriginalExpirationDate = x.LabTestRequest.WarehouseReceived.Expiration,
                    Quantity = x.LabTestRequest.Quantity,
                    AllowableDate = x.AllowableDate,
                    DaysToExpire = x.LabTestRequest.WarehouseReceived.Expiration.HasValue
                     ? (int)(x.LabTestRequest.WarehouseReceived.Expiration.Value - DateTime.Now).TotalDays
                    : 0,
                    Status = x.Status,
                    Remarks = x.Reason
                });

            if (returnedItems == null)
            {
                throw new Exception("No result found!");
            }

            return await PagedList<ReturnedItemsDTO>.CreateAsync(returnedItems, userParams.PageSize, userParams.PageSize);

        }
        
        //Reject Items for LabTest
        
        public async Task<bool> RejectItemsForLabTest(RejectedItems rejectItems, int labTestRequestId)
        {
            var validateLabRequest = await _context.LabTestRequests.FirstOrDefaultAsync(x => x.Id == labTestRequestId);

            if (validateLabRequest == null)
            {
                throw new Exception("No result found");
            }

            await _context.RejectedItems.AddAsync(rejectItems);
            await _context.SaveChangesAsync();

            return true;
        }
        
        public async Task<IEnumerable<RejectItemsDTO>> GetAllRejectedItemsForLabTest()
        {
            var rejectedItems = await _context.RejectedItems
                .Where(x => x.Status == "For Approval")
                .Include(x => x.LabTestRequests)
                .ThenInclude(x => x.WarehouseReceived)
                .Select(x => new RejectItemsDTO
                {
                    WarehouseId = x.LabTestRequests.WarehouseReceivingId,
                    SampleName = x.LabTestRequests.WarehouseReceived.ItemCode,
                    OriginalExpirationDate = x.LabTestRequests.WarehouseReceived.Expiration,
                    Quantity = x.LabTestRequests.Quantity,
                    AllowableDate = x.AllowableDate,
                    CreatedAt = x.CreatedAt.ToString("MM/dd/yyyy"),
                    Status = x.Status,
                    Disposition = x.Disposition
                })
                .ToListAsync();

            if (rejectedItems == null)
            {
                throw new Exception("No result found");
            }

            return rejectedItems;

        }
        public async Task<PagedList<RejectItemsDTO>> GetAllRejectedItemsForLabTestPagination(UserParams userParams)
        {
            var rejectedItems = _context.RejectedItems
                .Where(x => x.Status == "For Approval")
                .Include(x => x.LabTestRequests)
                .ThenInclude(x => x.WarehouseReceived)
                .Select(x => new RejectItemsDTO
                {
                    WarehouseId = x.LabTestRequests.WarehouseReceivingId,
                    SampleName = x.LabTestRequests.WarehouseReceived.ItemCode,
                    OriginalExpirationDate = x.LabTestRequests.WarehouseReceived.Expiration,
                    Quantity = x.LabTestRequests.Quantity,
                    AllowableDate = x.AllowableDate,
                    CreatedAt = x.CreatedAt.ToString("MM/dd/yyyy"),
                    Status = x.Status,
                    Disposition = x.Disposition
                });

            if (rejectedItems == null)
            {
                throw new Exception("No result found");
            }

            return await PagedList<RejectItemsDTO>.CreateAsync(
                rejectedItems,
                userParams.PageSize,
                userParams.PageNumber
                );
        } 
        
        //LabTest Result
        
        public async Task<bool> AddLabTestResult(LabTestResult labTestResult, int labTestRequestId)
        {
            var itemToLabTest =
                await _context.ReceiveRequests.FirstOrDefaultAsync(x => x.LabTestRequestsId == labTestRequestId);
            itemToLabTest.Status = labTestResult.Result;
            
            await _context.LabTestResults.AddAsync(labTestResult);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}