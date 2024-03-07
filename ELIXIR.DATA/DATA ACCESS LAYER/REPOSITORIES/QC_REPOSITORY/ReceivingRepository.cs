using ELIXIR.DATA.CORE.INTERFACES.QC_INTERFACE;
using ELIXIR.DATA.DATA_ACCESS_LAYER.MODELS.IMPORT_MODEL;
using ELIXIR.DATA.DATA_ACCESS_LAYER.MODELS.QC_MODEL;
using ELIXIR.DATA.DATA_ACCESS_LAYER.STORE_CONTEXT;
using ELIXIR.DATA.DTOs.RECEIVING_DTOs;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ELIXIR.DATA.DATA_ACCESS_LAYER.MODELS.WAREHOUSE_MODEL;
using ELIXIR.DATA.DTOs.WAREHOUSE_DTOs;
using ELIXIR.DATA.DATA_ACCESS_LAYER.HELPERS;

namespace ELIXIR.DATA.DATA_ACCESS_LAYER.REPOSITORIES.QC_REPOSITORY
{
    public class ReceivingRepository : IReceivingRepository
    {
        private readonly StoreContext _context;

        public ReceivingRepository(StoreContext context)
        {
            _context = context;
        }

        public async Task<bool> AddNewReceivingInformation(PO_Receiving receiving)
        {
            await _context.QC_Receiving.AddAsync(receiving);
            var validateIfExpriable = await _context.RawMaterials.Where(x => x.ItemCode == receiving.ItemCode)
                .Where(x => x.IsExpirable == false).FirstOrDefaultAsync();

            DateTime dateAdd = DateTime.Now.AddDays(30);

            receiving.IsActive = true;

            if (receiving.Expiry_Date < dateAdd)
                receiving.IsNearlyExpire = true;

            if (receiving.Expiry_Date > dateAdd)
                receiving.ExpiryIsApprove = true;

            if (validateIfExpriable != null)
                receiving.ExpiryIsApprove = true;

            receiving.QC_ReceiveDate = DateTime.Now;

            return true;
        }

        public async Task<bool> UpdateReceivingInfo(PO_Receiving receiving)
        {
            DateTime dateAdd = DateTime.Now.AddDays(30);

            var existingInfo = await _context.QC_Receiving.Where(x => x.PO_Summary_Id == receiving.PO_Summary_Id)
                .FirstOrDefaultAsync();

            receiving.IsActive = true;

            if (receiving.Expiry_Date < dateAdd)
                receiving.IsNearlyExpire = true;

            if (receiving.Expiry_Date > dateAdd)
                receiving.ExpiryIsApprove = true;

            receiving.QC_ReceiveDate = DateTime.Now;

            return await AddNewReceivingInformation(receiving);
        }

        public async Task<bool> AddNewRejectInfo(PO_Reject reject)
        {
            await _context.QC_Reject.AddAsync(reject);

            return true;
        }

        public async Task<bool> UpdateRejectInfo(PO_Reject reject)
        {
            var existingInfo = await _context.QC_Reject.Where(x => x.PO_ReceivingId == reject.PO_ReceivingId)
                .FirstOrDefaultAsync();

            var validateActualRemaining = await _context.QC_Receiving.Where(x => x.Id == reject.PO_ReceivingId)
                .Where(x => x.IsActive == true)
                .FirstOrDefaultAsync();
            if (validateActualRemaining == null)
                return false;

            if (existingInfo == null)
                return await AddNewRejectInfo(reject);

            existingInfo.Quantity = reject.Quantity;
            existingInfo.Remarks = reject.Remarks;

            return true;
        }

        public async Task<bool> ValidatePoId(int id)
        {
            var validateExisting = await _context.POSummary.Where(x => x.Id == id)
                .Where(x => x.IsActive == true)
                .FirstOrDefaultAsync();
            if (validateExisting == null)
                return false;

            return true;
        }

        public async Task<IReadOnlyList<PoSummaryChecklistDto>> GetAllAvailablePo()
        {
            var summaryx = (from posummary in _context.POSummary
                where posummary.IsActive == true
                join receive in _context.QC_Receiving
                    on posummary.Id equals receive.PO_Summary_Id into leftJ
                from receive in leftJ.DefaultIfEmpty()
                select new PoSummaryChecklistDto
                {
                    Id = posummary.Id,
                    PO_Number = posummary.PO_Number,
                    ItemCode = posummary.ItemCode,
                    ItemDescription = posummary.ItemDescription,
                    Supplier = posummary.VendorName,
                    UOM = posummary.UOM,
                    QuantityOrdered = posummary.Ordered,
                    ActualGood =
                        (receive != null && receive.IsActive != false && receive.IsWareHouseReceive != false
                            ? receive.Actual_Delivered
                            : 0) + receive.TotalReject,
                    IsActive = posummary.IsActive,
                    IsQcReceiveIsActive = receive != null && receive.IsActive != false ? receive.IsActive : true,
                    ActualRemaining = 0,
                    TotalReject = receive != null ? (int)receive.TotalReject : 0
                });

            return await summaryx
                .GroupBy(x => new
                {
                    x.Id,
                    x.PO_Number,
                    x.ItemCode,
                    x.ItemDescription,
                    x.UOM,
                    x.Supplier,
                    x.QuantityOrdered,
                    x.IsActive,
                    x.IsQcReceiveIsActive,
                    x.TotalReject
                })
                .Select(receive => new PoSummaryChecklistDto
                {
                    Id = receive.Key.Id,
                    PO_Number = receive.Key.PO_Number,
                    ItemCode = receive.Key.ItemCode,
                    ItemDescription = receive.Key.ItemDescription,
                    UOM = receive.Key.UOM,
                    Supplier = receive.Key.Supplier,
                    QuantityOrdered = receive.Key.QuantityOrdered,
                    ActualGood = receive.Sum(x => x.ActualGood),
                    ActualRemaining = receive.Key.QuantityOrdered - (receive.Sum(x => x.ActualGood)),
                    IsActive = receive.Key.IsActive,
                    IsQcReceiveIsActive = receive.Key.IsQcReceiveIsActive,
                    TotalReject = receive.Key.TotalReject
                })
                .OrderBy(x => x.PO_Number)
                .Where(x => x.ActualRemaining != 0)
                .Where(x => x.IsActive == true)
                .ToListAsync();
        }

        public async Task<bool> CancelPo(ImportPOSummary summary)
        {
            var existingPo = await _context.POSummary.Where(x => x.Id == summary.Id)
                .FirstOrDefaultAsync();

            existingPo.IsActive = false;
            existingPo.Date_Cancellation = DateTime.Now;
            existingPo.Reason = summary.Reason;

            if (summary.Reason == null)
                existingPo.Reason = "Wrong Input";

            return true;
        }

        public async Task<IReadOnlyList<CancelledPoDto>> GetAllCancelledPo()
        {
            var cancelpo = (from posummary in _context.POSummary
                    join receive in _context.QC_Receiving on posummary.Id equals receive.PO_Summary_Id into leftJ
                    from receive in leftJ.DefaultIfEmpty()
                    group new
                        {
                            receive,
                            posummary
                        }
                        by new
                        {
                            posummary.Id,
                            posummary.PO_Number,
                            posummary.ItemCode,
                            posummary.ItemDescription,
                            posummary.VendorName,
                            posummary.Ordered,
                            posummary.Date_Cancellation,
                            posummary.Reason,
                            posummary.IsActive,
                        }
                    into result
                    select new CancelledPoDto
                    {
                        Id = result.Key.Id,
                        PO_Number = result.Key.PO_Number,
                        ItemCode = result.Key.ItemCode,
                        ItemDescription = result.Key.ItemDescription,
                        Supplier = result.Key.VendorName,
                        QuantityOrdered = result.Key.Ordered,
                        QuantityCancel = result.Key.Ordered - result.Sum(x => x.receive.Actual_Delivered),
                        QuantityGood = result.Sum(x => x.receive.Actual_Delivered),
                        DateCancelled = result.Key.Date_Cancellation.ToString(),
                        Remarks = result.Key.Reason,
                        IsActive = result.Key.IsActive
                    }).Where(x => x.IsActive == false)
                .Where(x => x.DateCancelled != null)
                .Where(x => x.Remarks != null);

            return await cancelpo.ToListAsync();
        }

        public async Task<bool> ReturnPoInAvailableList(ImportPOSummary summary)
        {
            var existingInfo = await _context.POSummary.Where(x => x.Id == summary.Id)
                .FirstOrDefaultAsync();
            if (existingInfo == null)
                return false;

            existingInfo.IsActive = true;
            existingInfo.Date_Cancellation = null;
            existingInfo.Reason = null;

            return true;
        }

        public async Task<IReadOnlyList<NearlyExpireDto>> GetAllNearlyExpireRawMaterial()
        {
            DateTime dateNow = DateTime.Now;
            DateTime dateadd = DateTime.Now.AddDays(30);

            var deliveredItem = _context.QC_Receiving.GroupBy(x => new
            {
                x.PO_Summary_Id
            }).Select(x => new
            {
                x.Key.PO_Summary_Id,
                ACtual_Delivered = x.Sum(x => x.Actual_Delivered)
            });

            var posummary1 = _context.POSummary
                .Select(x => new
                {
                    x.Id,
                    x.PO_Number,
                    x.ItemCode,
                    x.ItemDescription,
                    x.VendorName,
                    x.UOM,
                    x.Ordered,
                    x.IsActive
                })
                .GroupJoin(deliveredItem,
                    x => x.Id,
                    y => y.PO_Summary_Id,
                    (x, y) => new { Summary = x, Delivered = y })
                .SelectMany(
                    xy => xy.Delivered.DefaultIfEmpty(),
                    (x, y) => new
                    {
                        x.Summary.Id,
                        x.Summary.PO_Number,
                        x.Summary.ItemCode,
                        x.Summary.ItemDescription,
                        x.Summary.VendorName,
                        x.Summary.UOM,
                        x.Summary.Ordered,
                        x.Summary.IsActive,
                        ActualGood = y.ACtual_Delivered,
                        ActualRemaining = x.Summary.Ordered - (y.ACtual_Delivered)
                    });

            var nearlyExpiryItems = (from posummary in posummary1
                join received in _context.QC_Receiving on posummary.Id equals received.PO_Summary_Id into leftj2
                from received in leftj2.DefaultIfEmpty()
                where received.ExpiryIsApprove == null
                where received.IsNearlyExpire == true
                group new
                    {
                        posummary,
                        received
                    }
                    by new
                    {
                        posummary.Id,
                        posummary.PO_Number,
                        posummary.ItemCode,
                        posummary.ItemDescription,
                        posummary.VendorName,
                        posummary.UOM,
                        QuantityOrdered = posummary.Ordered,
                        posummary.IsActive,
                        ActutalGood = received.Actual_Delivered,
                        posummary.ActualRemaining,
                        received.TotalReject,
                        posummary.ActualGood,
                        received.ExpiryIsApprove,
                        ReceivingId = received.Id,
                        received.IsNearlyExpire,
                        received.Expiry_Date
                    }
                into result
                select new NearlyExpireDto
                {
                    Id = result.Key.Id,
                    PO_Number = result.Key.PO_Number,
                    ItemCode = result.Key.ItemCode,
                    ItemDescription = result.Key.ItemDescription,
                    Supplier = result.Key.VendorName,
                    Uom = result.Key.UOM,
                    QuantityOrdered = result.Key.QuantityOrdered,
                    IsActive = result.Key.IsActive,
                    ActualGood = result.Key.ActutalGood,
                    ActualRemaining = result.Key.ActualRemaining,
                    TotalReject = result.Key.TotalReject,
                    ExpiryDate = result.Key.Expiry_Date != null ? result.Key.Expiry_Date.ToString() : null,
                    Days = result.Key.Expiry_Date.HasValue
                        ? (int)Math.Round(result.Key.Expiry_Date.Value.Subtract(dateNow).TotalDays)
                        : 0,
                    IsNearlyExpire = result.Key.IsNearlyExpire,
                    ExpiryIsApprove = result.Key.ExpiryIsApprove,
                    ReceivingId = result.Key.ReceivingId
                });


            return await nearlyExpiryItems.ToListAsync();
        }

        public async Task<bool> ApproveNearlyExpireRawMaterials(PO_Receiving receive)
        {
            var existingInfo = await _context.QC_Receiving.Where(x => x.Id == receive.Id)
                .FirstOrDefaultAsync();
            if (existingInfo == null)
                return false;

            existingInfo.ExpiryIsApprove = true;
            existingInfo.ExpiryApproveBy = receive.ExpiryApproveBy;
            existingInfo.ExpiryDateOfApprove = DateTime.Now;
            return true;
        }

        public async Task<IReadOnlyList<WarehouseReceivingDto>> GetAllRawMaterialsForWarehouseReceiving()
        {
            var warehouse = (from posummary in _context.POSummary
                join receive in _context.QC_Receiving on posummary.Id equals receive.PO_Summary_Id
                join rawmats in _context.RawMaterials on posummary.ItemCode equals rawmats.ItemCode
                select new WarehouseReceivingDto
                {
                    Id = receive.Id,
                    PO_Number = posummary.PO_Number,
                    ItemCode = posummary.ItemCode,
                    ItemDescription = posummary.ItemDescription,
                    Supplier = posummary.VendorName,
                    QuantityOrdered = posummary.Ordered,
                    ActualGood = receive.Actual_Delivered,
                    Reject = receive.TotalReject,
                    ExpirationDate = receive.Expiry_Date != null ? receive.Expiry_Date.ToString() : null,
                    QC_ReceivedDate = receive.QC_ReceiveDate.ToString("MM/dd/yyyy"),
                    IsActive = receive.IsActive,
                    IsWareHouseReceive = receive.IsWareHouseReceive != null,
                    IsExpiryApprove = receive.ExpiryIsApprove != null,
                    IsExpirable = rawmats.IsExpirable
                });

            return await warehouse.Where(x => x.IsWareHouseReceive == false)
                .Where(x => x.IsExpiryApprove == true)
                .Where(x => x.IsActive == true)
                .ToListAsync();
        }

        public async Task<bool> CancelPartialRecevingInQC(PO_Receiving receiving)
        {
            var receivingpo = await _context.QC_Receiving.Where(x => x.Id == receiving.Id)
                .FirstOrDefaultAsync();

            receivingpo.IsActive = false;
            receivingpo.CancelDate = DateTime.Now;
            receivingpo.CancelBy = receiving.CancelBy;
            receivingpo.CancelRemarks = receiving.CancelRemarks;
            receivingpo.Reason = receiving.Reason;

            return true;
        }

        public async Task<bool> RejectRawMaterialsNearlyExpire(PO_Receiving receiving)
        {
            var validateNearlyExpire = await _context.QC_Receiving.Where(x => x.Id == receiving.Id)
                .FirstOrDefaultAsync();


            if (validateNearlyExpire == null)
                return false;

            validateNearlyExpire.IsActive = false;
            validateNearlyExpire.ExpiryIsApprove = false;

            return true;
        }

        public async Task<bool> WarehouseConfirmRejectByQc(WarehouseReceiving warehouse)
        {
            var existingInfo = await _context.WarehouseReceived.Where(x => x.QcReceivingId == warehouse.QcReceivingId)
                .FirstOrDefaultAsync();

            var validateQcReceiving =
                await _context.QC_Receiving.FirstOrDefaultAsync(x => x.Id == warehouse.QcReceivingId);


            var rejectWarehouse = await _context.Warehouse_Reject.Where(x => x.WarehouseReceivingId == existingInfo.Id)
                .SumAsync(x => x.Quantity);

            var validateWarehouseReject = await _context.Warehouse_Reject
                .Where(x => x.WarehouseReceivingId == existingInfo.Id)
                .ToListAsync();


            if (existingInfo == null)
                return false;

            validateQcReceiving.Actual_Delivered = validateQcReceiving.Actual_Delivered - rejectWarehouse;
            existingInfo.ConfirmRejectbyQc = true;
            validateQcReceiving.ConfirmRejectByQc = true;


            return true;
        }

        public async Task<bool> WarehouseReturnRejectByQc(PO_Receiving receiving)
        {
            var itemWarehouse = await _context.WarehouseReceived
                .FirstOrDefaultAsync(x => x.QcReceivingId == receiving.Id);

            var itemRejectWarehouse = await _context.Warehouse_Reject
                .FirstOrDefaultAsync(x => x.WarehouseReceivingId == itemWarehouse.Id);
            var itemQc = await _context.QC_Receiving
                .FirstOrDefaultAsync(x => x.Id == receiving.Id);

            receiving.Actual_Delivered = itemRejectWarehouse.Quantity;
            receiving.Expiry_Date = itemQc.Expiry_Date;
            receiving.Manufacturing_Date = itemQc.Manufacturing_Date;
            receiving.Expected_Delivery = itemQc.Expected_Delivery;
            receiving.Batch_No = itemQc.Batch_No;
            receiving.Expiry_Date = itemQc.Expiry_Date;
            receiving.ExpiryIsApprove = itemQc.ExpiryIsApprove;
            receiving.PO_Summary_Id = itemQc.PO_Summary_Id;
            receiving.IsActive = itemQc.IsActive;
            receiving.QC_ReceiveDate = itemQc.QC_ReceiveDate;
            receiving.Id = 0;

            itemWarehouse.ConfirmRejectbyWarehouse = false;

            await AddNewReceivingInformation(receiving);

            return true;
        }

        public async Task<IReadOnlyList<RejectWarehouseReceivingDto>> GetAllWarehouseConfirmReject()
        {
            var reject = (from posummary in _context.POSummary
                join receive in _context.QC_Receiving on posummary.Id equals receive.PO_Summary_Id
                select new
                {
                    Id = receive.Id,
                    PO_Number = posummary.PO_Number,
                    ItemCode = posummary.ItemCode,
                    ItemDescription = posummary.ItemDescription,
                    Supplier = posummary.VendorName,
                    Uom = posummary.UOM,
                    QuantityOrderded = posummary.Ordered
                });

            return await reject
                .Join(_context.WarehouseReceived,
                    reject => reject.Id,
                    warehouse => warehouse.QcReceivingId,
                    (reject, warehouse) => new RejectWarehouseReceivingDto
                    {
                        Id = warehouse.Id,
                        PO_Number = reject.PO_Number,
                        ItemCode = reject.ItemCode,
                        ItemDescription = reject.ItemDescription,
                        Supplier = reject.Supplier,
                        Uom = reject.Uom,
                        QuantityOrdered = reject.QuantityOrderded,
                        ActualGood = warehouse.ActualGood,
                        ReceivingDate = warehouse.ReceivingDate.ToString("MM/dd/yyyy"),
                        IsWarehouseReceived = warehouse.IsWarehouseReceive,
                        Remarks = warehouse.Reason,
                        ConfirmRejectByWarehouse = warehouse.ConfirmRejectbyWarehouse,
                        ConfirmRejectByQc = warehouse.ConfirmRejectbyQc
                    }).Where(x => x.IsWarehouseReceived == true)
                .Where(x => x.ConfirmRejectByWarehouse == true)
                .Where(x => x.ConfirmRejectByQc == true)
                .ToListAsync();
        }

        public async Task<bool> ValidateActualRemaining(PO_Receiving receiving)
        {
            var validateActualRemaining = await (from posummary in _context.POSummary
                    join receive in _context.QC_Receiving on posummary.Id equals receive.PO_Summary_Id into leftJ
                    from receive in leftJ.DefaultIfEmpty()
                    where posummary.IsActive == true
                    select new PoSummaryChecklistDto
                    {
                        Id = posummary.Id,
                        PO_Number = posummary.PO_Number,
                        ItemCode = posummary.ItemCode,
                        ItemDescription = posummary.ItemDescription,
                        Supplier = posummary.VendorName,
                        UOM = posummary.UOM,
                        QuantityOrdered = posummary.Ordered,
                        ActualGood = receive != null && receive.IsActive != false ? receive.Actual_Delivered : 0,
                        IsActive = posummary.IsActive,
                        ActualRemaining = 0,
                        IsQcReceiveIsActive = receive != null ? receive.IsActive : true
                    })
                .GroupBy(x => new
                {
                    x.Id,
                    x.PO_Number,
                    x.ItemCode,
                    x.ItemDescription,
                    x.UOM,
                    x.QuantityOrdered,
                    x.IsActive,
                    x.IsQcReceiveIsActive
                })
                .Select(receive => new PoSummaryChecklistDto
                {
                    Id = receive.Key.Id,
                    PO_Number = receive.Key.PO_Number,
                    ItemCode = receive.Key.ItemCode,
                    ItemDescription = receive.Key.ItemDescription,
                    UOM = receive.Key.UOM,
                    QuantityOrdered = receive.Key.QuantityOrdered,
                    ActualGood = receive.Sum(x => x.ActualGood),
                    ActualRemaining = ((receive.Key.QuantityOrdered + (receive.Key.QuantityOrdered / 100) * 10) -
                                       (receive.Sum(x => x.ActualGood))),
                    IsActive = receive.Key.IsActive,
                    IsQcReceiveIsActive = receive.Key.IsQcReceiveIsActive
                }).Where(x => x.IsQcReceiveIsActive == true)
                .FirstOrDefaultAsync(x => x.Id == receiving.PO_Summary_Id);

            if (validateActualRemaining == null)
                return true;

            return validateActualRemaining.ActualRemaining >= receiving.Actual_Delivered;
        }

        public async Task<bool> ValidateForCancelPo(ImportPOSummary summary)
        {
            var forcancel = await (from posummary in _context.POSummary
                    join receive in _context.QC_Receiving on posummary.Id equals receive.PO_Summary_Id into leftJ
                    from receive in leftJ.DefaultIfEmpty()
                    where posummary.IsActive == true
                    select new PoSummaryChecklistDto
                    {
                        Id = posummary.Id,
                        PO_Number = posummary.PO_Number,
                        ItemCode = posummary.ItemCode,
                        ItemDescription = posummary.ItemDescription,
                        Supplier = posummary.VendorName,
                        UOM = posummary.UOM,
                        QuantityOrdered = posummary.Ordered,
                        ActualGood = receive != null && receive.IsActive != false ? receive.Actual_Delivered : 0,
                        IsActive = posummary.IsActive,
                        IsQcReceiveIsActive = receive != null && receive.IsActive != false ? receive.IsActive : true,
                        IsWarehouseReceived = receive.IsWareHouseReceive != null,
                        ActualRemaining = 0
                    }).GroupBy(x => new
                {
                    x.Id,
                    x.PO_Number,
                    x.ItemCode,
                    x.ItemDescription,
                    x.UOM,
                    x.Supplier,
                    x.QuantityOrdered,
                    x.IsActive,
                    x.IsQcReceiveIsActive,
                    x.IsWarehouseReceived,
                })
                .Select(receive => new PoSummaryChecklistDto
                {
                    Id = receive.Key.Id,
                    PO_Number = receive.Key.PO_Number,
                    ItemCode = receive.Key.ItemCode,
                    ItemDescription = receive.Key.ItemDescription,
                    UOM = receive.Key.UOM,
                    Supplier = receive.Key.Supplier,
                    QuantityOrdered = receive.Key.QuantityOrdered,
                    ActualGood = receive.Sum(x => x.ActualGood),
                    ActualRemaining = receive.Key.QuantityOrdered - (receive.Sum(x => x.ActualGood)),
                    IsActive = receive.Key.IsActive,
                    IsQcReceiveIsActive = receive.Key.IsQcReceiveIsActive,
                    IsWarehouseReceived = receive.Key.IsWarehouseReceived
                })
                .OrderBy(x => x.PO_Number)
                .Where(x => x.ActualRemaining != 0)
                .Where(x => x.IsActive == true)
                .Where(x => x.IsQcReceiveIsActive == true)
                .FirstOrDefaultAsync(x => x.Id == summary.Id);

            return forcancel.ActualGood == 0;
        }

        public async Task<PagedList<PoSummaryChecklistDto>> GetAllPoSummaryWithPagination(UserParams userParams)
        {
            var poSummary = (from posummary in _context.POSummary
                    where posummary.IsActive == true
                    join receive in _context.QC_Receiving
                        on posummary.Id equals receive.PO_Summary_Id into lejt1
                    from receive in lejt1.DefaultIfEmpty()
                    join rawmats in _context.RawMaterials on posummary.ItemCode equals rawmats.ItemCode
                        into leftJ
                    from rawmats in leftJ.DefaultIfEmpty()
                    group new
                        {
                            posummary,
                            rawmats,
                            receive
                        }
                        by new
                        {
                            posummary.Id,
                            posummary.PO_Number,
                            posummary.PO_Date,
                            posummary.PR_Number,
                            posummary.PR_Date,
                            posummary.ItemCode,
                            posummary.ItemDescription,
                            Supplier = posummary.VendorName,
                            posummary.UOM,
                            QuantityOrdered = posummary.Ordered,
                            posummary.IsActive,
                            IsQcReceiveIsActive = receive == null || receive.IsActive == false || receive.IsActive,
                            ActualRemaining = 0,
                            Expirable = rawmats != null && rawmats.IsExpirable
                        }
                    into result
                    select new PoSummaryChecklistDto
                    {
                        Id = result.Key.Id,
                        PO_Number = result.Key.PO_Number,
                        PO_Date = result.Key.PO_Date,
                        PR_Number = result.Key.PR_Number,
                        PR_Date = result.Key.PR_Date,
                        ItemCode = result.Key.ItemCode,
                        ItemDescription = result.Key.ItemDescription,
                        Supplier = result.Key.Supplier,
                        UOM = result.Key.UOM,
                        QuantityOrdered = result.Key.QuantityOrdered,
                        ActualGood = result.Sum(x =>
                            x.receive != null && x.receive.IsActive
                                ? x.receive.Actual_Delivered - x.receive.TotalReject
                                : 0),
                        IsActive = result.Key.IsActive,
                        IsQcReceiveIsActive = result.Key.IsQcReceiveIsActive,
                        ActualRemaining = result.Key.QuantityOrdered - result.Sum(x =>
                            x.receive != null && x.receive.IsActive
                                ? x.receive.Actual_Delivered - x.receive.TotalReject
                                : 0),
                        IsExpirable = result.Key.Expirable,
                    }).OrderBy(x => x.PO_Number)
                .Where(x => x.ActualRemaining != 0 && (x.ActualRemaining > 0))
                .Where(x => x.IsActive == true);

            return await PagedList<PoSummaryChecklistDto>.CreateAsync(poSummary, userParams.PageNumber,
                userParams.PageSize);
        }

        public async Task<PagedList<PoSummaryChecklistDto>> GetPoSummaryByStatusWithPaginationOrig(
            UserParams userParams, string search)
        {
            var poSummary = (from posummary in _context.POSummary
                    where posummary.IsActive == true
                    join receive in _context.QC_Receiving
                        on posummary.Id equals receive.PO_Summary_Id into leftJ
                    from receive in leftJ.DefaultIfEmpty()
                    join rawmats in _context.RawMaterials
                        on posummary.ItemCode equals rawmats.ItemCode into rawMatsJ
                    from rawmats in rawMatsJ.DefaultIfEmpty()
                    select new PoSummaryChecklistDto
                    {
                        Id = posummary.Id,
                        PO_Number = posummary.PO_Number,
                        PO_Date = posummary.PO_Date,
                        PR_Number = posummary.PR_Number,
                        PR_Date = posummary.PR_Date,
                        ItemCode = posummary.ItemCode,
                        ItemDescription = posummary.ItemDescription,
                        Supplier = posummary.VendorName,
                        UOM = posummary.UOM,
                        QuantityOrdered = posummary.Ordered,
                        ActualGood = receive != null && receive.IsActive != false ? receive.Actual_Delivered : 0,
                        IsActive = posummary.IsActive,
                        IsQcReceiveIsActive = receive != null && receive.IsActive != false ? receive.IsActive : true,
                        ActualRemaining = 0,
                        IsExpirable = rawmats != null && rawmats.IsExpirable != null ? rawmats.IsExpirable : false
                    }).GroupBy(x => new
                {
                    x.Id,
                    x.PO_Number,
                    x.PO_Date,
                    x.PR_Number,
                    x.PR_Date,
                    x.ItemCode,
                    x.ItemDescription,
                    x.UOM,
                    x.Supplier,
                    x.QuantityOrdered,
                    x.IsActive,
                    x.IsQcReceiveIsActive,
                    x.IsExpirable
                })
                .Select(receive => new PoSummaryChecklistDto
                {
                    Id = receive.Key.Id,
                    PO_Number = receive.Key.PO_Number,
                    PO_Date = receive.Key.PO_Date,
                    PR_Number = receive.Key.PR_Number,
                    PR_Date = receive.Key.PR_Date,
                    ItemCode = receive.Key.ItemCode,
                    ItemDescription = receive.Key.ItemDescription,
                    UOM = receive.Key.UOM,
                    Supplier = receive.Key.Supplier,
                    QuantityOrdered = receive.Key.QuantityOrdered,
                    ActualGood = receive.Sum(x => x.ActualGood),
                    ActualRemaining = receive.Key.QuantityOrdered - receive.Sum(x => x.ActualGood),
                    IsActive = receive.Key.IsActive,
                    IsQcReceiveIsActive = receive.Key.IsQcReceiveIsActive,
                    IsExpirable = receive.Key.IsExpirable
                })
                .OrderBy(x => x.PO_Number)
                .Where(x => x.ActualRemaining != 0
                            && x.ActualRemaining > 0
                            && x.IsActive == true
                            && (Convert.ToString(x.PO_Number).ToLower().Contains(search.Trim().ToLower())
                                || Convert.ToString(x.ItemDescription).ToLower().Contains(search.Trim().ToLower())
                                || Convert.ToString(x.ItemCode).ToLower().Contains(search.Trim().ToLower())));

            return await PagedList<PoSummaryChecklistDto>.CreateAsync(poSummary, userParams.PageNumber,
                userParams.PageSize);
        }

        public async Task<PagedList<WarehouseReceivingDto>> GetAllAvailableForWarehouseWithPagination(
            UserParams userParams)
        {
            var warehouse = (from posummary in _context.POSummary
                    join receive in _context.QC_Receiving on posummary.Id equals receive.PO_Summary_Id
                    select new WarehouseReceivingDto
                    {
                        Id = receive.Id,
                        PO_Number = posummary.PO_Number,
                        PO_Date = posummary.PO_Date.ToString("MM/dd/yyyy"),
                        PR_Number = posummary.PR_Number,
                        PR_Date = posummary.PR_Date.ToString("MM/dd/yyyy"),
                        ItemCode = posummary.ItemCode,
                        ItemDescription = posummary.ItemDescription,
                        UnitPrice = posummary.UnitPrice,
                        Supplier = posummary.VendorName,
                        QuantityOrdered = posummary.Ordered,
                        ActualGood = receive.Actual_Delivered - receive.TotalReject,
                        Reject = receive.TotalReject,
                        ExpirationDate1 = receive.Expiry_Date != null
                            ? receive.Expiry_Date.Value
                            : null,
                        ExpirationDate = receive.Expiry_Date != null
                            ? receive.Expiry_Date.ToString()
                            : null,
                        QC_ReceivedDate = receive.QC_ReceiveDate.ToString("MM/dd/yyyy"),
                        IsActive = receive.IsActive,
                        IsWareHouseReceive = receive.IsWareHouseReceive != null,
                        IsExpiryApprove = receive.ExpiryIsApprove != null,
                        ManufacturingDate = receive.Manufacturing_Date.ToString()
                    }).OrderBy(x => x.PO_Number)
                .Where(x => x.IsWareHouseReceive == false)
                .Where(x => x.IsExpiryApprove == true && (x.ExpirationDate1 == null || x.ExpirationDate1 != null))
                .Where(x => x.IsActive == true);

            return await PagedList<WarehouseReceivingDto>.CreateAsync(warehouse, userParams.PageNumber,
                userParams.PageSize);
        }

        public async Task<PagedList<WarehouseReceivingDto>> GetAllAvailableForWarehouseWithPaginationOrig(
            UserParams userParams, string search)
        {
            var warehouse = (from posummary in _context.POSummary
                    join receive in _context.QC_Receiving on posummary.Id equals receive.PO_Summary_Id
                    join rawmats in _context.RawMaterials on posummary.ItemCode equals rawmats.ItemCode
                    select new WarehouseReceivingDto
                    {
                        Id = receive.Id,
                        PO_Number = posummary.PO_Number,
                        PO_Date = posummary.PO_Date.ToString("MM/dd/yyyy"),
                        PR_Number = posummary.PR_Number,
                        PR_Date = posummary.PR_Date.ToString("MM/dd/yyyy"),
                        ItemCode = posummary.ItemCode,
                        ItemDescription = posummary.ItemDescription,
                        Supplier = posummary.VendorName,
                        QuantityOrdered = posummary.Ordered,
                        ActualGood = receive.Actual_Delivered - receive.TotalReject,
                        Reject = receive.TotalReject,
                        ExpirationDate1 = receive.Expiry_Date != null
                            ? receive.Expiry_Date.Value
                            : null,
                        ExpirationDate = receive.Expiry_Date != null ? receive.Expiry_Date.ToString() : null,
                        QC_ReceivedDate = receive.QC_ReceiveDate.ToString("MM/dd/yyyy"),
                        IsActive = receive.IsActive,
                        IsWareHouseReceive = receive.IsWareHouseReceive != null,
                        IsExpiryApprove = receive.ExpiryIsApprove != null,
                        ManufacturingDate = receive.Manufacturing_Date.ToString(),
                        IsExpirable = rawmats.IsExpirable
                    }).OrderBy(x => x.PO_Number)
                .Where(x => x.IsWareHouseReceive == false)
                .Where(x => x.IsExpiryApprove == true && (x.ExpirationDate1 == null || x.ExpirationDate1 != null))
                .Where(x => x.IsActive == true)
                .Where(x => Convert.ToString(x.PO_Number).ToLower()
                    .Contains(search.Trim().ToLower()));

            return await PagedList<WarehouseReceivingDto>.CreateAsync(warehouse, userParams.PageNumber,
                userParams.PageSize);
        }

        public async Task<PagedList<CancelledPoDto>> GetAllCancelledPOWithPagination(UserParams userParams)
        {
            var cancelpo = (from posummary in _context.POSummary
                    join receive in _context.QC_Receiving on posummary.Id equals receive.PO_Summary_Id into leftJ
                    from receive in leftJ.DefaultIfEmpty()
                    group new
                        {
                            receive,
                            posummary
                        }
                        by new
                        {
                            posummary.Id,
                            posummary.PO_Number,
                            posummary.ItemCode,
                            posummary.ItemDescription,
                            posummary.VendorName,
                            posummary.Ordered,
                            posummary.Date_Cancellation,
                            posummary.Reason,
                            posummary.IsActive
                        }
                    into result
                    select new CancelledPoDto
                    {
                        Id = result.Key.Id,
                        PO_Number = result.Key.PO_Number,
                        ItemCode = result.Key.ItemCode,
                        ItemDescription = result.Key.ItemDescription,
                        Supplier = result.Key.VendorName,
                        QuantityOrdered = result.Key.Ordered,
                        QuantityCancel = result.Key.Ordered - result.Sum(x => x.receive.Actual_Delivered),
                        QuantityGood = result.Sum(x => x.receive.Actual_Delivered),
                        DateCancelled = result.Key.Date_Cancellation.ToString(),
                        Remarks = result.Key.Reason,
                        IsActive = result.Key.IsActive
                    }).Where(x => x.IsActive == false)
                .Where(x => x.DateCancelled != null)
                .Where(x => x.Remarks != null);

            return await PagedList<CancelledPoDto>.CreateAsync(cancelpo, userParams.PageNumber, userParams.PageSize);
        }

        public async Task<PagedList<CancelledPoDto>> GetAllCancelledPOWithPaginationOrig(UserParams userParams,
            string search)
        {
            var cancelpo = (from posummary in _context.POSummary
                    join receive in _context.QC_Receiving on posummary.Id equals receive.PO_Summary_Id into leftJ
                    from receive in leftJ.DefaultIfEmpty()
                    group new
                        {
                            receive,
                            posummary
                        }
                        by new
                        {
                            posummary.Id,
                            posummary.PO_Number,
                            posummary.ItemCode,
                            posummary.ItemDescription,
                            posummary.VendorName,
                            posummary.Ordered,
                            posummary.Date_Cancellation,
                            posummary.Reason,
                            posummary.IsActive
                        }
                    into result
                    select new CancelledPoDto
                    {
                        Id = result.Key.Id,
                        PO_Number = result.Key.PO_Number,
                        ItemCode = result.Key.ItemCode,
                        ItemDescription = result.Key.ItemDescription,
                        Supplier = result.Key.VendorName,
                        QuantityOrdered = result.Key.Ordered,
                        QuantityCancel = result.Key.Ordered - result.Sum(x => x.receive.Actual_Delivered),
                        QuantityGood = result.Sum(x => x.receive.Actual_Delivered),
                        DateCancelled = result.Key.Date_Cancellation.ToString(),
                        Remarks = result.Key.Reason,
                        IsActive = result.Key.IsActive
                    }).Where(x => x.IsActive == false)
                .Where(x => x.DateCancelled != null)
                .Where(x => x.Remarks != null)
                .Where(x => Convert.ToString(x.PO_Number).ToLower()
                    .Contains(search.Trim().ToLower()));


            return await PagedList<CancelledPoDto>.CreateAsync(cancelpo, userParams.PageNumber, userParams.PageSize);
        }

        public async Task<PagedList<NearlyExpireDto>> GetAllNearlyExpireWithPagination(UserParams userParams)
        {
            DateTime dateNow = DateTime.Now;
            DateTime dateadd = DateTime.Now.AddDays(30);

            var deliveredItem = _context.QC_Receiving.GroupBy(x => new
            {
                x.PO_Summary_Id
            }).Select(x => new
            {
                x.Key.PO_Summary_Id,
                Actual_Delivered = x.Sum(x => x.Actual_Delivered - x.TotalReject)
            });

            var receiving = _context.QC_Receiving.Select(x => new QC_ReceivingDTO
            {
                PO_Summary_Id = x.PO_Summary_Id,
                Manufacturing_Date = x.Manufacturing_Date,
                Expected_Delivery = x.Expected_Delivery,
                MonitoredBy = x.MonitoredBy,
                QcBy = x.QcBy
            });

            var posummary1 = _context.POSummary
                .Select(x => new
                {
                    x.Id,
                    x.PO_Number,
                    x.ItemCode,
                    x.ItemDescription,
                    x.VendorName,
                    x.UOM,
                    x.Ordered,
                    x.IsActive
                })
                .GroupJoin(deliveredItem,
                    x => x.Id,
                    y => y.PO_Summary_Id,
                    (x, y) => new { Summary = x, Delivered = y })
                .SelectMany(
                    xy => xy.Delivered.DefaultIfEmpty(),
                    (x, y) => new
                    {
                        x.Summary.Id,
                        x.Summary.PO_Number,
                        x.Summary.ItemCode,
                        x.Summary.ItemDescription,
                        x.Summary.VendorName,
                        x.Summary.UOM,
                        x.Summary.Ordered,
                        x.Summary.IsActive,
                        ActualGood = y.Actual_Delivered,
                        ActualRemaining = x.Summary.Ordered - (y.Actual_Delivered)
                    });

            var nearlyExpiryItems = (from posummary in posummary1
                join received in _context.QC_Receiving on posummary.Id equals received.PO_Summary_Id into leftj2
                from received in leftj2.DefaultIfEmpty()
                where received.ExpiryIsApprove == null
                where received.IsNearlyExpire == true
                group new
                    {
                        posummary,
                        received
                    }
                    by new
                    {
                        posummary.Id,
                        posummary.PO_Number,
                        posummary.ItemCode,
                        posummary.ItemDescription,
                        posummary.VendorName,
                        posummary.UOM,
                        QuantityOrdered = posummary.Ordered,
                        posummary.IsActive,
                        ActutalGood = received.Actual_Delivered,
                        posummary.ActualRemaining,
                        received.TotalReject,
                        posummary.ActualGood,
                        received.ExpiryIsApprove,
                        ReceivingId = received.Id,
                        received.IsNearlyExpire,
                        received.Expiry_Date,
                        received.Manufacturing_Date,
                        received.Expected_Delivery,
                        received.MonitoredBy,
                        received.QcBy
                    }
                into result
                select new NearlyExpireDto
                {
                    Id = result.Key.Id,
                    PO_Number = result.Key.PO_Number,
                    ItemCode = result.Key.ItemCode,
                    ItemDescription = result.Key.ItemDescription,
                    Supplier = result.Key.VendorName,
                    Uom = result.Key.UOM,
                    QuantityOrdered = result.Key.QuantityOrdered,
                    IsActive = result.Key.IsActive,
                    ActualGood = result.Key.ActutalGood - result.Key.TotalReject,
                    ActualRemaining = result.Key.QuantityOrdered - result.Sum(x =>
                        x.received != null && x.received.IsActive
                            ? x.received.Actual_Delivered - x.received.TotalReject
                            : 0),
                    TotalReject = result.Key.TotalReject,
                    ExpiryDate = result.Key.Expiry_Date != null ? result.Key.Expiry_Date.ToString() : null,
                    Days = result.Key.Expiry_Date.HasValue
                        ? (int)Math.Round(result.Key.Expiry_Date.Value.Subtract(dateNow).TotalDays)
                        : 0,
                    IsNearlyExpire = result.Key.IsNearlyExpire,
                    ExpiryIsApprove = result.Key.ExpiryIsApprove,
                    ReceivingId = result.Key.ReceivingId,
                    ManufacturingDate = result.Key.Manufacturing_Date.ToString(),
                    ExpectedDelivery = result.Key.Expected_Delivery,
                    MonitoredBy = result.Key.MonitoredBy,
                    QcBy = result.Key.QcBy
                });
            return await PagedList<NearlyExpireDto>.CreateAsync(nearlyExpiryItems, userParams.PageNumber,
                userParams.PageSize);
        }

        public async Task<PagedList<NearlyExpireDto>> GetAllNearlyExpireWithPaginationOrig(UserParams userParams,
            string search)
        {
            DateTime dateNow = DateTime.Now;
            DateTime dateadd = DateTime.Now.AddDays(30);

            var expiry = (from summary in _context.POSummary
                    join receiving in _context.QC_Receiving on summary.Id equals receiving.PO_Summary_Id
                    where receiving.Expiry_Date <= dateadd
                    group new
                        {
                            summary,
                            receiving
                        }
                        by new
                        {
                            summary.Id,
                            summary.PO_Number,
                            summary.ItemCode,
                            summary.ItemDescription,
                            summary.VendorName,
                            summary.UOM,
                            QuantityOrdered = summary.Ordered,
                            summary.IsActive,
                            ActutalGood = receiving.Actual_Delivered,
                            receiving.TotalReject,
                            receiving.Actual_Delivered,
                            receiving.ExpiryIsApprove,
                            ReceivingId = receiving.Id,
                            receiving.IsNearlyExpire,
                            receiving.Expiry_Date,
                            receiving.Manufacturing_Date,
                            receiving.Expected_Delivery,
                            receiving.MonitoredBy,
                            receiving.QcBy
                        }
                    into result
                    select new NearlyExpireDto
                    {
                        Id = result.Key.Id,
                        PO_Number = result.Key.PO_Number,
                        ItemCode = result.Key.ItemCode,
                        ItemDescription = result.Key.ItemDescription,
                        Supplier = result.Key.VendorName,
                        Uom = result.Key.UOM,
                        QuantityOrdered = result.Key.QuantityOrdered,
                        ActualGood = result.Key.Actual_Delivered - result.Key.TotalReject,
                        ActualRemaining = result.Key.QuantityOrdered - result.Sum(x =>
                            x.receiving != null && x.receiving.IsActive
                                ? x.receiving.Actual_Delivered - x.receiving.TotalReject
                                : 0),
                        ExpiryDate = result.Key.Expiry_Date != null
                            ? result.Key.Expiry_Date.Value.ToString("MM/dd/yyyy")
                            : null,
                        Days = result.Key.Expiry_Date.HasValue
                            ? (int)Math.Round(result.Key.Expiry_Date.Value.Subtract(dateNow).TotalDays)
                            : 0,
                        IsActive = result.Key.IsActive,
                        IsNearlyExpire = result.Key.IsNearlyExpire != null,
                        ExpiryIsApprove = result.Key.ExpiryIsApprove != null,
                        ReceivingId = result.Key.Id,
                        ManufacturingDate = result.Key.Manufacturing_Date.ToString(),
                        ExpectedDelivery = result.Key.Expected_Delivery,
                        MonitoredBy = result.Key.MonitoredBy,
                        QcBy = result.Key.QcBy
                    }).Where(x => x.IsNearlyExpire == true)
                .Where(x => x.ExpiryIsApprove == null)
                .Where(x => Convert.ToString(x.PO_Number).ToLower()
                    .Contains(search.Trim().ToLower()));

            return await PagedList<NearlyExpireDto>.CreateAsync(expiry, userParams.PageNumber, userParams.PageSize);
        }

        public async Task<PagedList<RejectWarehouseReceivingDto>> GetAllConfirmRejectWithPagination(
            UserParams userParams)
        {
            var reject = (from posummary in _context.POSummary
                    join receive in _context.QC_Receiving on posummary.Id equals receive.PO_Summary_Id
                    select new
                    {
                        Id = receive.Id,
                        PO_Number = posummary.PO_Number,
                        ItemCode = posummary.ItemCode,
                        ItemDescription = posummary.ItemDescription,
                        Supplier = posummary.VendorName,
                        Uom = posummary.UOM,
                        QuantityOrderded = posummary.Ordered
                    }).Join(_context.WarehouseReceived,
                    reject => reject.Id,
                    warehouse => warehouse.QcReceivingId,
                    (reject, warehouse) => new RejectWarehouseReceivingDto
                    {
                        Id = warehouse.Id,
                        PO_Number = reject.PO_Number,
                        ItemCode = reject.ItemCode,
                        ItemDescription = reject.ItemDescription,
                        Supplier = reject.Supplier,
                        Uom = reject.Uom,
                        QuantityOrdered = reject.QuantityOrderded,
                        ActualGood = warehouse.ActualGood,
                        ReceivingDate = warehouse.ReceivingDate.ToString("MM/dd/yyyy"),
                        IsWarehouseReceived = warehouse.IsWarehouseReceive,
                        Remarks = warehouse.Reason,
                        ConfirmRejectByWarehouse = warehouse.ConfirmRejectbyWarehouse,
                        ConfirmRejectByQc = warehouse.ConfirmRejectbyQc
                    }).Where(x => x.IsWarehouseReceived == true)
                .Where(x => x.ConfirmRejectByWarehouse == true)
                .Where(x => x.ConfirmRejectByQc == true);

            return await PagedList<RejectWarehouseReceivingDto>.CreateAsync(reject, userParams.PageNumber,
                userParams.PageSize);
        }

        public async Task<PagedList<RejectWarehouseReceivingDto>> GetAllConfirmRejectWithPaginationOrig(
            UserParams userParams, string search)
        {
            var reject = (from posummary in _context.POSummary
                    join receive in _context.QC_Receiving on posummary.Id equals receive.PO_Summary_Id
                    select new
                    {
                        Id = receive.Id,
                        PO_Number = posummary.PO_Number,
                        ItemCode = posummary.ItemCode,
                        ItemDescription = posummary.ItemDescription,
                        Supplier = posummary.VendorName,
                        Uom = posummary.UOM,
                        QuantityOrderded = posummary.Ordered
                    }).Join(_context.WarehouseReceived,
                    reject => reject.Id,
                    warehouse => warehouse.QcReceivingId,
                    (reject, warehouse) => new RejectWarehouseReceivingDto
                    {
                        Id = warehouse.Id,
                        PO_Number = reject.PO_Number,
                        ItemCode = reject.ItemCode,
                        ItemDescription = reject.ItemDescription,
                        Supplier = reject.Supplier,
                        Uom = reject.Uom,
                        QuantityOrdered = reject.QuantityOrderded,
                        ActualGood = warehouse.ActualGood,
                        ReceivingDate = warehouse.ReceivingDate.ToString("MM/dd/yyyy"),
                        IsWarehouseReceived = warehouse.IsWarehouseReceive,
                        Remarks = warehouse.Reason,
                        ConfirmRejectByWarehouse = warehouse.ConfirmRejectbyWarehouse,
                        ConfirmRejectByQc = warehouse.ConfirmRejectbyQc
                    }).Where(x => x.IsWarehouseReceived == true)
                .Where(x => x.ConfirmRejectByWarehouse == true)
                .Where(x => x.ConfirmRejectByQc == true)
                .Where(x => Convert.ToString(x.PO_Number).ToLower()
                    .Contains(search.Trim().ToLower()));

            return await PagedList<RejectWarehouseReceivingDto>.CreateAsync(reject, userParams.PageNumber,
                userParams.PageSize);
        }

        public async Task<IReadOnlyList<NearlyExpireDto>> GetItemDetailsForNearlyExpire(int id)
        {
            var summary = (from posummary in _context.POSummary
                join receiving in _context.QC_Receiving
                    on posummary.Id equals receiving.PO_Summary_Id
                    into leftJ1
                from receiving in leftJ1.DefaultIfEmpty()
                select new NearlyExpireDto
                {
                    Id = receiving.Id,
                    PO_Number = posummary.PO_Number,
                    PO_Date = posummary.PO_Date.ToString(),
                    PR_Number = posummary.PR_Number,
                    PR_Date = posummary.PR_Date.ToString(),
                    ItemCode = posummary.ItemCode,
                    ItemDescription = posummary.ItemDescription,
                    Supplier = posummary.VendorName,
                    QuantityOrdered = posummary.Ordered,
                    ManufacturingDate = receiving.Manufacturing_Date.ToString(),
                    DateOfChecking = receiving.QC_ReceiveDate.ToString(),
                    ExpiryIsApprove = receiving.ExpiryIsApprove != null,
                    ExpiryDate = receiving.Expiry_Date.ToString(),
                    Uom = posummary.UOM,
                });

            return await summary.Where(x => x.Id == id)
                .ToListAsync();
        }

        public async Task<bool> ValidatePOForCancellation(int id)
        {
            var validatePo = await _context.QC_Receiving.Where(x => x.PO_Summary_Id == id)
                .Where(x => x.IsActive == true)
                .Where(x => x.IsWareHouseReceive != true)
                .ToListAsync();

            if (validatePo.Count != 0)
                return false;

            return true;
        }
    }
}