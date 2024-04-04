using ELIXIR.DATA.DTOs.REPORT_DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;
using ELIXIR.DATA.DATA_ACCESS_LAYER.HELPERS;

namespace ELIXIR.DATA.CORE.INTERFACES.REPORT_INTERFACE
{
    public interface IReportRepository
    {
        Task<IReadOnlyList<QCReport>> QcRecevingReport(string DateFrom, string DateTo);
        Task<IReadOnlyList<WarehouseReport>> WarehouseReceivingReport(string DateFrom, string DateTo);
        Task<IReadOnlyList<TransformationReport>> TransformationReport(string DateFrom, string DateTo);
        Task<IReadOnlyList<MoveOrderReport>> MoveOrderReport(string DateFrom, string DateTo);
        Task<IReadOnlyList<MiscellaneousReceiptReport>> MReceiptReport(string DateFrom, string DateTo);
        Task<IReadOnlyList<MiscellaneousIssueReport>> MIssueReport(string DateFrom, string DateTo);

        Task<IReadOnlyList<MoveOrderReport>> TransactedMoveOrderReport(string DateFrom, string DateTo);

        Task<IReadOnlyList<WarehouseReport>> NearlyExpireItemsReport(int expirydays);

        Task<IReadOnlyList<CancelledOrderReport>> CancelledOrderedReports(string DateFrom, string DateTo);

        Task<IReadOnlyList<InventoryMovementReport>> InventoryMovementReport(string DateFrom, string DateTo,
            string PlusOne);

        Task<IReadOnlyList<ConsolidatedReport>> ConsolidatedReport(string dateFrom, string dateTo);
        Task<IReadOnlyList<MoveOrderReport>> ApprovedMoveOrderReport(string dateFrom, string dateTo);
        Task<IReadOnlyList<OrderVsServeReportsDTO>> OrderVsServeReports(string dateFrom, string dateTo);
        Task<PagedList<OrderVsServeReportsDTO>> OrderVsServeReportsPagination(string dateFrom, string dateTo,
            UserParams userParams);
        Task<IReadOnlyList<ItemswithBBDDTO>> ItemswithBBDReport();
        //Task<PagedList<ConsolidatedReport>> ConsolidatedReportPagination(string dateFrom, string dateTo, UserParams userParams);
        Task<PagedList<ItemswithBBDDTO>> ItemswithBBDDTOsPagination(UserParams userParams);

        Task<PagedList<QCReport>> QcRecevingReportPagination(string DateFrom, string DateTo, UserParams userParams);
        Task<PagedList<WarehouseReport>> WarehouseReceivingReportPagination(string DateFrom, string DateTo, UserParams userParams);
        Task<PagedList<TransformationReport>> TransformationReportPagination(string DateFrom, string DateTo, UserParams userParams);
        Task<PagedList<MoveOrderReport>> MoveOrderReportPagination(string DateFrom, string DateTo, UserParams userParams);
        Task<PagedList<MiscellaneousReceiptReport>> MReceiptReportPagination(string DateFrom, string DateTo, UserParams userParams);
        Task<PagedList<MiscellaneousIssueReport>> MIssueReportPagination(string DateFrom, string DateTo, UserParams userParams);
        Task<PagedList<WarehouseReport>> NearlyExpireItemsReportPagination(int expirydays, UserParams userParams);
        Task<PagedList<MoveOrderReport>> TransactedMoveOrderReportPagination(string dateFrom, string dateTo, UserParams userParams);
        Task<PagedList<CancelledOrderReport>> CancelledOrderedReportsPagination(string DateFrom, string DateTo, UserParams userParams);
        Task<PagedList<InventoryMovementReport>> InventoryMovementReportPagination(string DateFrom,
        string DateTo, string PlusOne, UserParams userParams);
        Task<PagedList<MoveOrderReport>> ApprovedMoveOrderReportPagination(string dateFrom, string dateTo, UserParams userParams);
 
    }
}