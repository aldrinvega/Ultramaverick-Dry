using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ELIXIR.DATA.CORE.ICONFIGURATION;
using ELIXIR.DATA.DATA_ACCESS_LAYER.EXTENSIONS;
using ELIXIR.DATA.DATA_ACCESS_LAYER.HELPERS;
using ELIXIR.DATA.DATA_ACCESS_LAYER.MODELS.LABTEST_MODEL;
using Microsoft.AspNetCore.Mvc;

namespace ELIXIR.API.Controllers.LABORATORYTEST_CONTROLLER
{
    public class LaboratoryTestController : BaseApiController
    {
        private readonly IUnitOfWork _unitOfWork;

        public LaboratoryTestController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        //Lab Test Request

        #region Lab Test Request

        [HttpPost]
        [Route("RequestLabTest")]
        public async Task<IActionResult> RequestLabTest(IEnumerable<LabTestRequests> labTestRequests)
        {
            try
            {
                await _unitOfWork.LaboratoryTest.RequestItemsForLabTest(labTestRequests);
                return Ok("Items are successfully requested");
            }
            catch (Exception e)
            {
                return Conflict(e.Message);
            }
        }

        [HttpGet]
        [Route("GetAllNearlyExpiryItemsPagination")]
        public async Task<IActionResult> GetAllNearlyExpiryItemsPagination([FromQuery] UserParams userParams)
        {
            try
            {
                var nearlyExpiryitems = await _unitOfWork.LaboratoryTest.GetAllNearlyExpiryItemsPagination(userParams);
                Response.AddPaginationHeader(nearlyExpiryitems.PageSize, nearlyExpiryitems.CurrentPage,
                    nearlyExpiryitems.TotalPages, nearlyExpiryitems.TotalCount, nearlyExpiryitems.HasPreviousPage,
                    nearlyExpiryitems.HasNextPage);
                var items = new
                {
                    itemsForLabTest = nearlyExpiryitems,
                    nearlyExpiryitems.CurrentPage,
                    nearlyExpiryitems.PageSize,
                    nearlyExpiryitems.TotalCount,
                    nearlyExpiryitems.TotalPages,
                    nearlyExpiryitems.HasPreviousPage,
                    nearlyExpiryitems.HasNextPage
                };
                return Ok(items);
            }
            catch (Exception e)
            {
                return Conflict(e.Message);
            }
        }

        [HttpGet]
        [Route("GetAllRequestedItemsForLabTest")]
        public async Task<IActionResult> GetAllRequestedItemsForLabTest()
        {
            try
            {
                var requestedItemsForLabTest = await _unitOfWork.LaboratoryTest.GetAllRequestedItemsForLabTest();
                return Ok(requestedItemsForLabTest);
            }
            catch (Exception e)
            {
                return Conflict(e.Message);
            }
            
        }
        #endregion
        
        //Accept LabTest Request

        #region Accept LabTest Request

        [HttpPost]
        [Route("AcceptLabTestRequest")]
        public async Task<IActionResult> AcceptLabTestRequest(ReceiveRequest acceptLabRequest)
        {
            await _unitOfWork.LaboratoryTest.AcceptLabTestRequest(acceptLabRequest);
            return Ok("Your item has been accepted!");
        }

        [HttpGet]
        [Route("GetAllAcceptedItemsForLabTest")]
        public async Task<IActionResult> GetAllAcceptedItemsForLabTest()
        {
            try
            {
                var acceptedItemsForLabTest = await _unitOfWork.LaboratoryTest.GetAllAcceptedItemsForLabTest();
                return Ok(acceptedItemsForLabTest);
            }
            catch (Exception e)
            {
                return Conflict(e.Message);
            }
            
        }

        [HttpGet]
        [Route("GetAllAcceptedItemsForLabTestPagination")]
        public async Task<IActionResult> GetAllAcceptedItemsForLabTestPagination([FromQuery]UserParams userParams)
        {
            var acceptedItemForLabTest = await _unitOfWork.LaboratoryTest.GetAllAcceptedItemsForLabTestPagination(userParams);
            
            Response.AddPaginationHeader(acceptedItemForLabTest.PageSize, acceptedItemForLabTest.CurrentPage, acceptedItemForLabTest.TotalPages, acceptedItemForLabTest.TotalCount, acceptedItemForLabTest.HasNextPage, acceptedItemForLabTest.HasPreviousPage);

            var acceptedItemsForLabTest = new
            {
                acceptedItemForLabTest,
                acceptedItemForLabTest.PageSize,
                acceptedItemForLabTest.CurrentPage,
                acceptedItemForLabTest.TotalCount,
                acceptedItemForLabTest.TotalPages,
                acceptedItemForLabTest.HasNextPage,
                acceptedItemForLabTest.HasPreviousPage
            };
            
            

            return Ok(acceptedItemsForLabTest);
        }
        
        #endregion
        
        //Return Items

        #region Return Items
        [HttpPost]
        [Route("ReturnItemsRequestedForLabTest")]
        public async Task<IActionResult> ReturnItemsRequestedForLabTest([FromBody]ReturnedItems returnedItems, [FromRoute] int labTestRequestId  )
        {
            await _unitOfWork.LaboratoryTest.ReturnItemsRequestedForLabTest(returnedItems, labTestRequestId);
            return Ok("Item has been returned!");
        }

        [HttpGet]
        [Route("GetAllReturnedItems")]
        public async Task<IActionResult> GetAllReturnedItems()
        {
            var returnedItems = await _unitOfWork.LaboratoryTest.GetAllReturnedItems();
            return Ok(returnedItems);
        }

        [HttpGet]
        [Route("GetAllReturnedItemsPagination")]
        public async Task<IActionResult> GetAllReturnedItemsPagination([FromQuery]UserParams userParams)
        {
            var returnedItem = await _unitOfWork.LaboratoryTest.GetAllReturnedItemsPagination(userParams);
            
            Response.AddPaginationHeader(returnedItem.PageSize, returnedItem.CurrentPage, returnedItem.TotalPages, returnedItem.TotalCount, returnedItem.HasNextPage, returnedItem.HasPreviousPage);

            var returnedItems = new
            {
                returnedItem,
                returnedItem.TotalPages,
                returnedItem.TotalCount,
                returnedItem.CurrentPage,
                returnedItem.PageSize,
                returnedItem.HasNextPage,
                returnedItem.HasPreviousPage
            };
            return Ok(returnedItems);
        }
        #endregion
        
        //Reject Items

        #region Reject Items
        
        [HttpPost]
        [Route("RejectItemsForLabTest")]
        public async Task<IActionResult> RejectItemsForLabTest(RejectedItems rejectItems, int labTestRequestId)
        {
            await _unitOfWork.LaboratoryTest.RejectItemsForLabTest(rejectItems, labTestRequestId);
            return Ok("Item has been rejected!");
        }

        [HttpGet]
        [Route("GetAllRejectedItemsForLabTest")]
        public async Task<IActionResult> GetAllRejectedItemsForLabTest()
        {
            var rejectedItems = await _unitOfWork.LaboratoryTest.GetAllRejectedItemsForLabTest();
            return Ok(rejectedItems);
        }

        [HttpGet]
        [Route("GetAllRejectedItemsForLabTestPagination")]
        public async Task<IActionResult> GetAllRejectedItemsForLabTestPagination([FromQuery]UserParams userParams)
        {
            var rejectedItem = await _unitOfWork.LaboratoryTest.GetAllRejectedItemsForLabTestPagination(userParams);

            Response.AddPaginationHeader(
                rejectedItem.PageSize,
                rejectedItem.CurrentPage,
                rejectedItem.TotalCount,
                rejectedItem.TotalPages,
                rejectedItem.HasNextPage,
                rejectedItem.HasPreviousPage
            );

            var rejectedItems = new
            {
                rejectedItem,
                rejectedItem.CurrentPage,
                rejectedItem.HasNextPage,
                rejectedItem.PageSize,
                rejectedItem.TotalPages,
                rejectedItem.TotalCount,
                rejectedItem.HasPreviousPage
            };

            return Ok(rejectedItems);
        }
        #endregion
    }
}