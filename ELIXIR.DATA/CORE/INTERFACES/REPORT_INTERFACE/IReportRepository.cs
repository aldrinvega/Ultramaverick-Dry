using ELIXIR.DATA.DTOs.REPORT_DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

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
    }
}