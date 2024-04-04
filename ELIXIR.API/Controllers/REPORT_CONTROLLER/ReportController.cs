using ELIXIR.DATA.CORE.ICONFIGURATION;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using ELIXIR.DATA.DATA_ACCESS_LAYER.EXTENSIONS;
using ELIXIR.DATA.DATA_ACCESS_LAYER.HELPERS;
using System.Drawing;

namespace ELIXIR.API.Controllers.REPORT_CONTROLLER
{
    public class ReportController : BaseApiController
    {
        private readonly IUnitOfWork _unitOfWork;

        public ReportController(IUnitOfWork unitofwork)
        {
            _unitOfWork = unitofwork;
        }

        [HttpGet]
        [Route("QcReceivingReport")]
        public async Task<IActionResult> QcReceivingReport([FromQuery] string DateFrom, [FromQuery] string DateTo)
        {
            var orders = await _unitOfWork.Report.QcRecevingReport(DateFrom, DateTo);

            return Ok(orders);
        }

        [HttpGet]
        [Route("QcReceivingReportPagination")]
        public async Task<IActionResult> QcReceivingReportPagination([FromQuery] string dateFrom,
           [FromQuery] string dateTo, [FromQuery] UserParams userParams)
        {
            var qCReports = await _unitOfWork.Report
                .QcRecevingReportPagination(dateFrom, dateTo, userParams);

            Response.AddPaginationHeader(
                qCReports.CurrentPage,
                qCReports.PageSize,
                qCReports.TotalCount,
                qCReports.TotalPages,
                qCReports.HasPreviousPage,
                qCReports.HasNextPage
                );

            var result = new
            {
                qCReports,
                qCReports.CurrentPage,
                qCReports.PageSize,
                qCReports.TotalCount,
                qCReports.TotalPages,
                qCReports.HasPreviousPage,
                qCReports.HasNextPage
            };

            return Ok(result);
        }

        [HttpGet]
        [Route("WarehouseReceivingReport")]
        public async Task<IActionResult> WarehouseReceivingReport([FromQuery] string DateFrom,
            [FromQuery] string DateTo)
        {
            var orders = await _unitOfWork.Report.WarehouseReceivingReport(DateFrom, DateTo);

            return Ok(orders);
        }

        [HttpGet]
        [Route("WarehouseReceivingReportPagination")]
        public async Task<IActionResult> WarehouseReceivingReportPagination([FromQuery] string dateFrom,
          [FromQuery] string dateTo, [FromQuery] UserParams userParams)
        {
            var warehouseReports = await _unitOfWork.Report
                .WarehouseReceivingReportPagination(dateFrom, dateTo, userParams);

            Response.AddPaginationHeader(
                warehouseReports.CurrentPage,
                warehouseReports.PageSize,
                warehouseReports.TotalCount,
                warehouseReports.TotalPages,
                warehouseReports.HasPreviousPage,
                warehouseReports.HasNextPage
                );

            var result = new
            {
                warehouseReports,
                warehouseReports.CurrentPage,
                warehouseReports.PageSize,
                warehouseReports.TotalCount,
                warehouseReports.TotalPages,
                warehouseReports.HasPreviousPage,
                warehouseReports.HasNextPage
            };

            return Ok(result);
        }


        [HttpGet]
        [Route("TransformationHistoryReport")]
        public async Task<IActionResult> TransformationHistoryReport([FromQuery] string DateFrom,
            [FromQuery] string DateTo)
        {
            var orders = await _unitOfWork.Report.TransformationReport(DateFrom, DateTo);

            return Ok(orders);
        }

        [HttpGet]
        [Route("TransformationHistoryReportPagination")]
        public async Task<IActionResult> TransformationHistoryReportPagination([FromQuery] string dateFrom,
          [FromQuery] string dateTo, [FromQuery] UserParams userParams)
        {
            var transformationReports = await _unitOfWork.Report
                .TransformationReportPagination(dateFrom, dateTo, userParams);

            Response.AddPaginationHeader(
                transformationReports.CurrentPage,
                transformationReports.PageSize,
                transformationReports.TotalCount,
                transformationReports.TotalPages,
                transformationReports.HasPreviousPage,
                transformationReports.HasNextPage
                );

            var result = new
            {
                transformationReports,
                transformationReports.CurrentPage,
                transformationReports.PageSize,
                transformationReports.TotalCount,
                transformationReports.TotalPages,
                transformationReports.HasPreviousPage,
                transformationReports.HasNextPage
            };

            return Ok(result);
        }

        [HttpGet]
        [Route("MoveOrderHistory")]
        public async Task<IActionResult> MoveOrderHistory([FromQuery] string DateFrom, [FromQuery] string DateTo)
        {
            var orders = await _unitOfWork.Report.MoveOrderReport(DateFrom, DateTo);

            return Ok(orders);
        }

        [HttpGet]
        [Route("MoveOrderHistoryPagination")]
        public async Task<IActionResult> MoveOrderHistoryPagination([FromQuery] string dateFrom,
        [FromQuery] string dateTo, [FromQuery] UserParams userParams)
        {
            var moveOrderReports = await _unitOfWork.Report
                .MoveOrderReportPagination(dateFrom, dateTo, userParams);

            Response.AddPaginationHeader(
                moveOrderReports.CurrentPage,
                moveOrderReports.PageSize,
                moveOrderReports.TotalCount,
                moveOrderReports.TotalPages,
                moveOrderReports.HasPreviousPage,
                moveOrderReports.HasNextPage
                );

            var result = new
            {
                moveOrderReports,
                moveOrderReports.CurrentPage,
                moveOrderReports.PageSize,
                moveOrderReports.TotalCount,
                moveOrderReports.TotalPages,
                moveOrderReports.HasPreviousPage,
                moveOrderReports.HasNextPage
            };

            return Ok(result);
        }

        [HttpGet]
        [Route("MiscellaneousReceiptReport")]
        public async Task<IActionResult> MiscellaneousReceiptReport([FromQuery] string DateFrom,
            [FromQuery] string DateTo)
        {
            var receipt = await _unitOfWork.Report.MReceiptReport(DateFrom, DateTo);

            return Ok(receipt);
        }

        [HttpGet]
        [Route("MiscellaneousReceiptReportPagination")]
        public async Task<IActionResult> MiscellaneousReceiptReportPagination([FromQuery] string dateFrom,
  [FromQuery] string dateTo, [FromQuery] UserParams userParams)
        {
            var miscellaneousReceiptReports = await _unitOfWork.Report
                .MReceiptReportPagination(dateFrom, dateTo, userParams);

            Response.AddPaginationHeader(
                miscellaneousReceiptReports.CurrentPage,
                miscellaneousReceiptReports.PageSize,
                miscellaneousReceiptReports.TotalCount,
                miscellaneousReceiptReports.TotalPages,
                miscellaneousReceiptReports.HasPreviousPage,
                miscellaneousReceiptReports.HasNextPage
                );

            var result = new
            {
                miscellaneousReceiptReports,
                miscellaneousReceiptReports.CurrentPage,
                miscellaneousReceiptReports.PageSize,
                miscellaneousReceiptReports.TotalCount,
                miscellaneousReceiptReports.TotalPages,
                miscellaneousReceiptReports.HasPreviousPage,
                miscellaneousReceiptReports.HasNextPage
            };

            return Ok(result);
        }

        [HttpGet]
        [Route("MiscellaneousIssueReport")]
        public async Task<IActionResult> MiscellaneousIssueReport([FromQuery] string DateFrom,
            [FromQuery] string DateTo)
        {
            var issue = await _unitOfWork.Report.MIssueReport(DateFrom, DateTo);

            return Ok(issue);
        }

        [HttpGet]
        [Route("MiscellaneousIssueReportPagination")]
        public async Task<IActionResult> MiscellaneousIssueReportPagination([FromQuery] string dateFrom,
  [FromQuery] string dateTo, [FromQuery] UserParams userParams)
        {
            var miscellaneousIssueReports = await _unitOfWork.Report
                .MIssueReportPagination(dateFrom, dateTo, userParams);

            Response.AddPaginationHeader(
                miscellaneousIssueReports.CurrentPage,
                miscellaneousIssueReports.PageSize,
                miscellaneousIssueReports.TotalCount,
                miscellaneousIssueReports.TotalPages,
                miscellaneousIssueReports.HasPreviousPage,
                miscellaneousIssueReports.HasNextPage
                );

            var result = new
            {
                miscellaneousIssueReports,
                miscellaneousIssueReports.CurrentPage,
                miscellaneousIssueReports.PageSize,
                miscellaneousIssueReports.TotalCount,
                miscellaneousIssueReports.TotalPages,
                miscellaneousIssueReports.HasPreviousPage,
                miscellaneousIssueReports.HasNextPage
            };

            return Ok(result);
        }

        [HttpGet]
        [Route("NearlyExpireItemsReport")]
        public async Task<IActionResult> NearlyExpireItemsReport([FromQuery] int expirydays)
        {
            var expiry = await _unitOfWork.Report.NearlyExpireItemsReport(expirydays);

            return Ok(expiry);
        }
        [HttpGet]
        [Route("NearlyExpireItemsReportPagination")]
        public async Task<IActionResult> NearlyExpireItemsReportPagination([FromQuery] int expirydays, [FromQuery] UserParams userParams)
        {
            var nealyExpiryItemsReports = await _unitOfWork.Report
                .NearlyExpireItemsReportPagination(expirydays, userParams);

            Response.AddPaginationHeader(
                nealyExpiryItemsReports.CurrentPage,
                nealyExpiryItemsReports.PageSize,
                nealyExpiryItemsReports.TotalCount,
                nealyExpiryItemsReports.TotalPages,
                nealyExpiryItemsReports.HasPreviousPage,
                nealyExpiryItemsReports.HasNextPage
                );

            var result = new
            {
                nealyExpiryItemsReports,
                nealyExpiryItemsReports.CurrentPage,
                nealyExpiryItemsReports.PageSize,
                nealyExpiryItemsReports.TotalCount,
                nealyExpiryItemsReports.TotalPages,
                nealyExpiryItemsReports.HasPreviousPage,
                nealyExpiryItemsReports.HasNextPage
            };

            return Ok(result);
        }

        [HttpGet]
        [Route("TransactedMoveOrderReport")]
        public async Task<IActionResult> TransactedMoveOrderReport([FromQuery] string DateFrom,
            [FromQuery] string DateTo)
        {
            var transact = await _unitOfWork.Report.TransactedMoveOrderReport(DateFrom, DateTo);

            return Ok(transact);
        }

        [HttpGet]
        [Route("TransactedMoveOrderReportPagination")]
        public async Task<IActionResult> TransactedMoveOrderReportPagination([FromQuery] string DateFrom, [FromQuery] string DateTo, [FromQuery] UserParams userParams)
        {
            var transactedMoveOrderReports = await _unitOfWork.Report
                .TransactedMoveOrderReportPagination(DateFrom, DateTo, userParams);

            Response.AddPaginationHeader(
                transactedMoveOrderReports.CurrentPage,
                transactedMoveOrderReports.PageSize,
                transactedMoveOrderReports.TotalCount,
                transactedMoveOrderReports.TotalPages,
                transactedMoveOrderReports.HasPreviousPage,
                transactedMoveOrderReports.HasNextPage
                );

            var result = new
            {
                transactedMoveOrderReports,
                transactedMoveOrderReports.CurrentPage,
                transactedMoveOrderReports.PageSize,
                transactedMoveOrderReports.TotalCount,
                transactedMoveOrderReports.TotalPages,
                transactedMoveOrderReports.HasPreviousPage,
                transactedMoveOrderReports.HasNextPage
            };

            return Ok(result);
        }

        [HttpGet]
        [Route("CancelledOrderReport")]
        public async Task<IActionResult> CancelledOrderReport([FromQuery] string DateFrom, [FromQuery] string DateTo)
        {
            var cancel = await _unitOfWork.Report.CancelledOrderedReports(DateFrom, DateTo);

            return Ok(cancel);
        }

        [HttpGet]
        [Route("CancelledOrderReportsPagination")]
        public async Task<IActionResult> CancelledOrderReportsPagination([FromQuery] string DateFrom, [FromQuery] string DateTo, [FromQuery] UserParams userParams)
        {
            var cancelledOrderReports = await _unitOfWork.Report
                .CancelledOrderedReportsPagination(DateFrom, DateTo, userParams);

            Response.AddPaginationHeader(
                cancelledOrderReports.CurrentPage,
                cancelledOrderReports.PageSize,
                cancelledOrderReports.TotalCount,
                cancelledOrderReports.TotalPages,
                cancelledOrderReports.HasPreviousPage,
                cancelledOrderReports.HasNextPage
                );

            var result = new
            {
                cancelledOrderReports,
                cancelledOrderReports.CurrentPage,
                cancelledOrderReports.PageSize,
                cancelledOrderReports.TotalCount,
                cancelledOrderReports.TotalPages,
                cancelledOrderReports.HasPreviousPage,
                cancelledOrderReports.HasNextPage
            };

            return Ok(result);
        }

        [HttpGet]
        [Route("InventoryMovementReport")]
        public async Task<IActionResult> InventoryMovementReport([FromQuery] string DateFrom, [FromQuery] string DateTo,
            [FromQuery] string PlusOne)
        {
            var cancel = await _unitOfWork.Report.InventoryMovementReport(DateFrom, DateTo, PlusOne);
            return Ok(cancel);
        }

        [HttpGet]
        [Route("InventoryMovementReportPagination")]
        public async Task<IActionResult> InventoryMovementReportPagination([FromQuery] string DateFrom, [FromQuery] string DateTo, [FromQuery] UserParams userParams)
        {
            var inventoryMovementReports = await _unitOfWork.Report
                .ApprovedMoveOrderReportPagination(DateFrom, DateTo, userParams);

            Response.AddPaginationHeader(
                inventoryMovementReports.CurrentPage,
                inventoryMovementReports.PageSize,
                inventoryMovementReports.TotalCount,
                inventoryMovementReports.TotalPages,
                inventoryMovementReports.HasPreviousPage,
                inventoryMovementReports.HasNextPage
                );

            var result = new
            {
                inventoryMovementReports,
                inventoryMovementReports.CurrentPage,
                inventoryMovementReports.PageSize,
                inventoryMovementReports.TotalCount,
                inventoryMovementReports.TotalPages,
                inventoryMovementReports.HasPreviousPage,
                inventoryMovementReports.HasNextPage
            };

            return Ok(result);
        }

        [HttpGet]
        [Route("ConsolidatedReport")]
        public async Task<IActionResult> ConsolidatedReports([FromQuery] string dateFrom, [FromQuery] string dateTo)
        {
            var consolidatedReport = await _unitOfWork.Report.ConsolidatedReport(dateFrom, dateTo);

            return Ok(consolidatedReport);
        }

        [HttpGet("ApprovedMoveOrdersReports")]
        public async Task<IActionResult> ApprovedMoveOrdersReports([FromQuery] string dateFrom, string dateTo)
        {
            var approvedMoveOrderReports = await _unitOfWork.Report.ApprovedMoveOrderReport(dateFrom, dateTo);
            return Ok(approvedMoveOrderReports);
        }



        [HttpGet]
        [Route("ApprovedMoveOrdersReportsPagination")]
        public async Task<IActionResult> ApprovedMoveOrdersReportsPagination([FromQuery] string DateFrom, [FromQuery] string DateTo, [FromQuery] UserParams userParams)
        {
            var approvedMoveOrderReports = await _unitOfWork.Report
                .ApprovedMoveOrderReportPagination(DateFrom, DateTo, userParams);

            Response.AddPaginationHeader(
                approvedMoveOrderReports.CurrentPage,
                approvedMoveOrderReports.PageSize,
                approvedMoveOrderReports.TotalCount,
                approvedMoveOrderReports.TotalPages,
                approvedMoveOrderReports.HasPreviousPage,
                approvedMoveOrderReports.HasNextPage
                );

            var result = new
            {
                approvedMoveOrderReports,
                approvedMoveOrderReports.CurrentPage,
                approvedMoveOrderReports.PageSize,
                approvedMoveOrderReports.TotalCount,
                approvedMoveOrderReports.TotalPages,
                approvedMoveOrderReports.HasPreviousPage,
                approvedMoveOrderReports.HasNextPage
            };

            return Ok(result);
        }

        [HttpGet]
        [Route("OrderVsServeReports")]
        public async Task<IActionResult> OrderVsServeReports([FromQuery] string dateFrom,
            [FromQuery] string dateTo)
        {
            var orderVsServeReports = await _unitOfWork.Report.OrderVsServeReports(dateFrom, dateTo);

            return Ok(orderVsServeReports);
        }
        
        [HttpGet]
        [Route("OrderVsServeReportsPagination")]
        public async Task<IActionResult> OrderVsServeReportPagination([FromQuery] string dateFrom,
            [FromQuery] string dateTo, [FromQuery] UserParams userParams)
        {
            var orderVsServeReports = await _unitOfWork.Report
                .OrderVsServeReportsPagination(dateFrom, dateTo, userParams);
            
            Response.AddPaginationHeader(
                orderVsServeReports.CurrentPage,
                orderVsServeReports.PageSize,
                orderVsServeReports.TotalCount,
                orderVsServeReports.TotalPages,
                orderVsServeReports.HasPreviousPage,
                orderVsServeReports.HasNextPage
                );

            var result = new
            {
                orderVsServeReports,
                orderVsServeReports.CurrentPage,
                orderVsServeReports.PageSize,
                orderVsServeReports.TotalCount,
                orderVsServeReports.TotalPages,
                orderVsServeReports.HasPreviousPage,
                orderVsServeReports.HasNextPage
            };

            return Ok(result);
        }

        [HttpGet("ItemsWithBbdPagination")]
        public async Task<IActionResult> ItemsWithBbdPagination([FromQuery] UserParams userParams)
        {
            var itemsWithBbd = await _unitOfWork.Report
                .ItemswithBBDDTOsPagination(userParams);

            Response.AddPaginationHeader(
                itemsWithBbd.CurrentPage,
                itemsWithBbd.PageSize,
                itemsWithBbd.TotalCount,
                itemsWithBbd.TotalPages,
                itemsWithBbd.HasPreviousPage,
                itemsWithBbd.HasNextPage
                );

            var result = new
            {
                itemsWithBbd,
                itemsWithBbd.CurrentPage,
                itemsWithBbd.PageSize,
                itemsWithBbd.TotalCount,
                itemsWithBbd.TotalPages,
                itemsWithBbd.HasPreviousPage,
                itemsWithBbd.HasNextPage
            };

            return Ok(result);
        }

        //[HttpGet]
        //[Route("ConsolidatedReportsPagination")]
        //public async Task<IActionResult> ConsolidatedReportsPagination([FromQuery] string dateFrom,
        //    [FromQuery] string dateTo, [FromQuery] UserParams userParams)
        //{
        //    var consolidatedReports = await _unitOfWork.Report
        //        .ConsolidatedReportPagination(dateFrom, dateTo, userParams);

        //    Response.AddPaginationHeader(
        //        consolidatedReports.CurrentPage,
        //        consolidatedReports.PageSize,
        //        consolidatedReports.TotalCount,
        //        consolidatedReports.TotalPages,
        //        consolidatedReports.HasPreviousPage,
        //        consolidatedReports.HasNextPage
        //        );

        //    var result = new
        //    {
        //        consolidatedReports,
        //        consolidatedReports.CurrentPage,
        //        consolidatedReports.PageSize,
        //        consolidatedReports.TotalCount,
        //        consolidatedReports.TotalPages,
        //        consolidatedReports.HasPreviousPage,
        //        consolidatedReports.HasNextPage
        //    };

        //    return Ok(result);
        //}
    }
}