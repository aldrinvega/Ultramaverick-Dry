using ELIXIR.DATA.CORE.INTERFACES.WAREHOUSE_INTERFACE;
using ELIXIR.DATA.DATA_ACCESS_LAYER.HELPERS;
using ELIXIR.DATA.DATA_ACCESS_LAYER.MODELS.QC_MODEL;
using ELIXIR.DATA.DATA_ACCESS_LAYER.MODELS.WAREHOUSE_MODEL;
using ELIXIR.DATA.DATA_ACCESS_LAYER.STORE_CONTEXT;
using ELIXIR.DATA.DTOs.RECEIVING_DTOs;
using ELIXIR.DATA.DTOs.TRANSFORMATION_DTOs;
using ELIXIR.DATA.DTOs.WAREHOUSE_DTOs;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ELIXIR.DATA.DATA_ACCESS_LAYER.REPOSITORIES.WAREHOUSE_REPOSITORY
{
    public class WarehouseReceiveRepository : IWarehouseReceive

    {
        private readonly StoreContext _context;

        public WarehouseReceiveRepository(StoreContext context)
        {
            _context = context;
        }

        public async Task<WareHouseScanBarcode> GetScanBarcodeByReceivedId(string scanbarcode)
        {
            DateTime dateNow = DateTime.Now;


            var computeStock = await _context.WarehouseReceived.Where(x => x.ItemCode == scanbarcode)
                .Where(x => x.IsWarehouseReceive == true)
                .SumAsync(x => x.ActualGood);

            var summary = (from receive in _context.QC_Receiving
                where receive.IsActive == true &&
                      (receive.IsWareHouseReceive == false || receive.IsWareHouseReceive == null) &&
                      receive.ExpiryIsApprove == true && (receive.Expiry_Date == null || receive.Expiry_Date != null)
                join posummary in _context.POSummary
                    on receive.PO_Summary_Id equals posummary.Id into leftJ
                from posummary in leftJ.DefaultIfEmpty()
                where posummary.ItemCode == scanbarcode
                group
                    receive
                    by new
                    {
                        receive.Id,
                        posummary.ItemCode,
                        posummary.ItemDescription,
                        posummary.PO_Number,
                        posummary.UOM,
                        posummary.VendorName,
                        receive.Actual_Delivered,
                        posummary.UnitPrice,
                        computeStock,
                        receive.IsWareHouseReceive,
                        receive.Expiry_Date,
                        receive.Manufacturing_Date,
                        receive.Expected_Delivery,
                        receive.IsActive,
                        receive.TotalReject
                    }
                into total
                select new WareHouseScanBarcode
                {
                    Id = total.Key.Id,
                    ItemCode = total.Key.ItemCode,
                    ItemDescription = total.Key.ItemDescription,
                    PO_Number = total.Key.PO_Number,
                    Uom = total.Key.UOM,
                    Supplier = total.Key.VendorName,
                    ActualDelivered = total.Key.Actual_Delivered - total.Key.TotalReject,
                    TotalReject = total.Key.TotalReject,
                    ExpirationDate1 = total.Key.Expiry_Date != null
                        ? total.Key.Expiry_Date.Value
                        : null,
                    UnitPrice = total.Key.UnitPrice,
                    Expiration = (total.Key.Expiry_Date).ToString() != null ? (total.Key.Expiry_Date).ToString() : null,
                    //ExpirationDays = total.Key.Expiry_Date == null ? 0 : total.Key.Expiry_Date.Value.Subtract(dateNow).Days,
                    TotalStock = total.Key.computeStock,
                    IsWarehouseReceived = total.Key.IsWareHouseReceive != null,
                    ManufacturingDate = total.Key.Manufacturing_Date.ToString(),
                    ExpectedDelivery = total.Key.Expected_Delivery,
                    IsActive = total.Key.IsActive
                });

            return await summary.FirstOrDefaultAsync();
        }

        public async Task<bool> ReceiveMaterialsFromWarehouse(WarehouseReceiving warehouse)
        {
            //var ActualGood = warehouse.ActualGood - warehouse.TotalReject;


            var scanbarcode = (from posummary in _context.POSummary
                    join receive in _context.QC_Receiving on posummary.Id equals receive.PO_Summary_Id
                    select new WareHouseScanBarcode
                    {
                        Id = receive.Id,
                        ItemCode = posummary.ItemCode,
                        ItemDescription = posummary.ItemDescription,
                        PO_Number = posummary.PO_Number,
                        Supplier = posummary.VendorName,
                        Uom = posummary.UOM,
                        ManufacturingDate = receive.Manufacturing_Date.ToString(),
                        ActualDelivered = receive.Actual_Delivered,
                        TotalReject = receive.TotalReject,
                        Expiration = receive.Expiry_Date != null ? receive.Expiry_Date.ToString() : null,
                        ExpirationDays = receive.Expiry_Date.HasValue
                            ? (int)Math.Round(receive.Expiry_Date.Value.Subtract(DateTime.Now).TotalDays)
                            : 0,
                        IsActive = receive.IsActive,
                        IsWarehouseReceived = receive.IsWareHouseReceive != null,
                        ExpiryIsApprove = receive.ExpiryIsApprove != null,
                        UnitCost = posummary.UnitPrice
                    }).Where(x => x.IsWarehouseReceived == false)
                .Where(x => x.IsActive == true)
                .Where(x => x.ExpiryIsApprove == true)
                .FirstOrDefault(x => x.ItemCode == warehouse.ItemCode);

            var ActualDelivered = scanbarcode.ActualDelivered - scanbarcode.TotalReject;

            if (ActualDelivered < warehouse.ActualGood)
                return false;

            warehouse.ItemDescription = scanbarcode.ItemDescription;
            warehouse.Supplier = scanbarcode.Supplier;
            warehouse.PO_Number = scanbarcode.PO_Number;
            warehouse.Uom = scanbarcode.Uom;
            warehouse.ActualDelivered = scanbarcode.ActualDelivered;
            warehouse.QuantityGood = scanbarcode.ActualDelivered;
            warehouse.ManufacturingDate = Convert.ToDateTime(scanbarcode.ManufacturingDate);
            warehouse.Expiration = scanbarcode.Expiration == null ? null : Convert.ToDateTime(scanbarcode.Expiration);
            warehouse.ExpirationDays = scanbarcode.ExpirationDays;
            warehouse.IsWarehouseReceive = true;
            warehouse.IsActive = true;
            warehouse.QcReceivingId = scanbarcode.Id;
            warehouse.TransactionType = "Receiving";

            var qcreceived = _context.QC_Receiving.FirstOrDefault(x => x.Id == scanbarcode.Id);
            qcreceived.IsWareHouseReceive = true;


            await AddMaterialsInWarehouse(warehouse);

            return true;
        }

        // public async Task<bool> CheckOrderInAllocationModule(WarehouseReceiving warehouseReceiving)
        // {
        //
        //     var totalOrders = await _context.Orders.Where(x => x.ForAllocation == true)
        //                 .Where(x => x.ItemCode == warehouseReceiving.ItemCode).ToListAsync();
        //     var totalAllocated = await _context.Orders.Where(x => x.ItemCode == warehouseReceiving.ItemCode)
        //                 .SumAsync(x => x.QuantityOrdered);
        //
        //     if (totalOrders == null)
        //                return true;
        //     var orderingReserve = await _context.Orders.Where(x => x.IsActive == true)
        //                                     .Where(x => x.PreparedDate != null)
        //                                     .Where(x => x.ItemCode == warehouseReceiving.ItemCode)
        //                                     .SumAsync(order => order.AllocatedQuantity ?? (int)order.QuantityOrdered);
        //
        //     var receivedStocks = await _context.WarehouseReceived
        //         .Where(x => x.ItemCode == warehouseReceiving.ItemCode)
        //         .Where(x => x.IsActive == true)
        //         .SumAsync(x => x.ActualGood);
        //
        //     var reserve = receivedStocks - orderingReserve;
        //
        //     foreach (var order in totalOrders)
        //     {
        //         if (reserve >= totalAllocated)
        //             order.ForAllocation = null;
        //     }
        //     return true;
        // }

        public async Task<bool> AddMaterialsInWarehouse(WarehouseReceiving warehouse)
        {
            await _context.WarehouseReceived.AddAsync(warehouse);

            return true;
        }

        public async Task<bool> AddRejectMaterialsInWarehouse(Warehouse_Reject reject)
        {
            await _context.Warehouse_Reject.AddAsync(reject);

            return true;
        }

        public async Task<IReadOnlyList<RejectWarehouseReceivingDto>> GetAllRejectRawmaterialsInWarehouse()
        {
            var qcreceiving = (from posummary in _context.POSummary
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


            var warehousereject = (from warehouse in _context.WarehouseReceived
                join rejectwarehouse in _context.Warehouse_Reject
                    on warehouse.Id equals rejectwarehouse.WarehouseReceivingId into leftJ
                from rejectwarehouse in leftJ.DefaultIfEmpty()
                where warehouse.ConfirmRejectbyWarehouse == true &&
                      warehouse.IsWarehouseReceive == true &&
                      warehouse.ConfirmRejectbyQc == false
                join qc in qcreceiving on warehouse.QcReceivingId equals qc.Id
                group rejectwarehouse by new
                {
                    qc.PO_Number,
                    qc.ItemCode,
                    qc.ItemDescription,
                    qc.Supplier,
                    qc.Uom,
                    qc.QuantityOrderded,
                    warehouse.QuantityGood,
                    warehouse.Id,
                    warehouse.QcReceivingId,
                    warehouse.ReceivingDate,
                    warehouse.TotalReject,
                    warehouse.Reason,
                    warehouse.ConfirmRejectbyQc,
                    warehouse.ConfirmRejectbyWarehouse,
                    warehouse.IsWarehouseReceive
                }
                into total
                where total.Key.ConfirmRejectbyWarehouse == false
                select new RejectWarehouseReceivingDto
                {
                    Id = total.Key.Id,
                    PO_Number = total.Key.PO_Number,
                    ItemCode = total.Key.ItemCode,
                    ItemDescription = total.Key.ItemDescription,
                    Supplier = total.Key.Supplier,
                    Uom = total.Key.Uom,
                    QuantityOrdered = total.Key.QuantityOrderded,
                    ActualGood = total.Key.QuantityGood - total.Sum(x => x.Quantity),
                    QcReceivingId = total.Key.QcReceivingId,
                    ReceivingDate = total.Key.ReceivingDate.ToString("MM/dd/yyyy"),
                    ActualReject = total.Key.TotalReject,
                    Remarks = total.Key.Reason,
                    ConfirmRejectByQc = total.Key.ConfirmRejectbyQc,
                    ConfirmRejectByWarehouse = total.Key.ConfirmRejectbyWarehouse,
                    IsWarehouseReceived = total.Key.IsWarehouseReceive
                });

            return await warehousereject.ToListAsync();
        }

        public async Task<bool> RejectRawMaterialsByWarehouse(Warehouse_Reject reject)
        {
            var validate = _context.WarehouseReceived.FirstOrDefault(x => x.Id == reject.WarehouseReceivingId);

            var validateQcReceiving = _context.QC_Receiving.FirstOrDefault(x => x.Id == validate.QcReceivingId);

            if (validateQcReceiving == null)
                return false;

            if (validate.TotalReject < reject.Quantity)
                return false;

            reject.RejectByDate = DateTime.Now;
            reject.IsActive = true;

            await _context.Warehouse_Reject.AddAsync(reject);

            validate.ConfirmRejectbyWarehouse = true;
            validate.Reason = reject.Remarks;

            return true;
        }

        public async Task<bool> ScanBarcode(WarehouseReceiving warehouse)
        {
            DateTime dateNow = DateTime.Now;

            var scanbarcode = await (from posummary in _context.POSummary
                    join receive in _context.QC_Receiving on posummary.Id equals receive.PO_Summary_Id
                    select new WareHouseScanBarcode
                    {
                        Id = receive.Id,
                        ItemCode = posummary.ItemCode,
                        ItemDescription = posummary.ItemDescription,
                        PO_Number = posummary.PO_Number,
                        Supplier = posummary.VendorName,
                        Uom = posummary.UOM,
                        ManufacturingDate = receive.Manufacturing_Date.ToString(),
                        ActualDelivered = receive.Actual_Delivered,
                        Expiration = receive.Expiry_Date != null
                            ? receive.Expiry_Date.Value.ToString("MM/dd/yyyy")
                            : null,
                        // ExpirationDays = receive.Expiry_Date.HasValue ? receive.Expiry_Date.Value.Subtract(dateNow).Days : 0,
                        IsActive = receive.IsActive,
                        IsWarehouseReceived = receive.IsWareHouseReceive != null,
                        ExpiryIsApprove = receive.ExpiryIsApprove != null,
                        UnitCost = posummary.UnitPrice
                    }).Where(x => x.IsWarehouseReceived == false)
                .Where(x => x.IsActive == true)
                .Where(x => x.ExpiryIsApprove == true)
                .FirstOrDefaultAsync(x => x.ItemCode == warehouse.ItemCode);

            if (scanbarcode == null)
                return false;


            return true;
        }

        public async Task<IReadOnlyList<WarehouseReceivingDto>> ListForWarehouseReceiving()
        {
            var warehouse = (from posummary in _context.POSummary
                join receive in _context.QC_Receiving on posummary.Id equals receive.PO_Summary_Id
                select new WarehouseReceivingDto
                {
                    Id = receive.Id,
                    PO_Number = posummary.PO_Number,
                    ItemCode = posummary.ItemCode,
                    ItemDescription = posummary.ItemDescription,
                    Supplier = posummary.VendorName,
                    QuantityOrdered = posummary.Ordered,
                    ActualGood = receive.Actual_Delivered - receive.TotalReject,
                    Reject = receive.TotalReject,
                    ExpirationDate = receive.Expiry_Date != null ? receive.Expiry_Date.ToString() : null,
                    QC_ReceivedDate = receive.QC_ReceiveDate.ToString("MM/dd/yyyy"),
                    IsActive = receive.IsActive,
                    IsWareHouseReceive = receive.IsWareHouseReceive != null,
                    IsExpiryApprove = receive.ExpiryIsApprove != null
                });

            return await warehouse.Where(x => x.IsWareHouseReceive == false)
                .Where(x => x.IsExpiryApprove == true)
                .Where(x => x.IsActive == true)
                .ToListAsync();
        }

        public async Task<bool> ReturnRawmaterialsByWarehouse(PO_Receiving receiving)
        {
            var validate = await _context.QC_Receiving.Where(x => x.Id == receiving.Id)
                .FirstOrDefaultAsync();

            if (validate == null)
                return false;


            validate.IsActive = false;

            return true;
        }

        public async Task<bool> ValidateActualAndRejectInput(WarehouseReceiving warehouse)
        {
            DateTime dateNow = DateTime.Now;

            var scanbarcode = (from posummary in _context.POSummary
                    join receive in _context.QC_Receiving on posummary.Id equals receive.PO_Summary_Id
                    select new WareHouseScanBarcode
                    {
                        Id = receive.Id,
                        ItemCode = posummary.ItemCode,
                        ItemDescription = posummary.ItemDescription,
                        PO_Number = posummary.PO_Number,
                        Supplier = posummary.VendorName,
                        Uom = posummary.UOM,
                        ManufacturingDate = receive.Manufacturing_Date.ToString(),
                        ActualDelivered = receive.Actual_Delivered,
                        Expiration = receive.Expiry_Date != null
                            ? receive.Expiry_Date.Value.ToString("MM/dd/yyyy")
                            : null,
                        ExpirationDays = receive.Expiry_Date.HasValue
                            ? (int)Math.Round(receive.Expiry_Date.Value.Subtract(dateNow).TotalDays)
                            : 0,
                        IsActive = receive.IsActive,
                        IsWarehouseReceived = receive.IsWareHouseReceive != null,
                        ExpiryIsApprove = receive.ExpiryIsApprove != null,
                        TotalReject = receive.TotalReject
                    }).Where(x => x.IsWarehouseReceived == false)
                .Where(x => x.IsActive == true)
                .Where(x => x.ExpiryIsApprove == true)
                .FirstOrDefault(x => x.ItemCode == warehouse.ItemCode);


            if ((warehouse.ActualGood + warehouse.TotalReject) != scanbarcode.ActualDelivered - scanbarcode.TotalReject)
                return false;


            return true;
        }

        public async Task<IReadOnlyList<WarehouseRejectDto>> GetAllRejectedMaterialsByWarehouseId(int id)
        {
            var reject = _context.Warehouse_Reject.Where(x => x.WarehouseReceivingId == id)
                .Where(x => x.IsActive == true);

            return await reject.Select(reject => new WarehouseRejectDto
            {
                warehouseReceivingId = reject.WarehouseReceivingId,
                Remarks = reject.Remarks,
                Quantity = reject.Quantity,
                RejectDate = reject.RejectByDate.ToString("MM/dd/yyyy")
            }).ToListAsync();
        }

        public async Task<bool> ValidateTotalReject(Warehouse_Reject reject)
        {
            var validate = await _context.Warehouse_Reject
                .Where(x => x.WarehouseReceivingId == reject.WarehouseReceivingId)
                .SumAsync(x => x.Quantity);


            if (validate > reject.Quantity)
                return false;

            return true;
        }

        public async Task<IReadOnlyList<WarehouseReceived>> GetAllWarehouseReceived(string DateFrom, string DateTo)
        {
            var received = _context.WarehouseReceived.Where(x =>
                    x.ReceivingDate >= DateTime.Parse(DateFrom) && x.ReceivingDate <= DateTime.Parse(DateTo))
                .Where(x => x.IsActive == true)
                .GroupBy(x => new
                {
                    x.Id,
                    x.PO_Number,
                    x.ItemCode,
                    x.ItemDescription,
                    x.Uom,
                    x.Supplier,
                    x.ManufacturingDate,
                    x.ReceivingDate,
                    x.ActualGood,
                    x.IsActive
                })
                .Select(warehouse => new WarehouseReceived
                {
                    Id = warehouse.Key.Id,
                    PO_Number = warehouse.Key.PO_Number,
                    ItemCode = warehouse.Key.ItemCode,
                    ItemDescription = warehouse.Key.ItemDescription,
                    Uom = warehouse.Key.Uom,
                    Supplier = warehouse.Key.Supplier,
                    ManufacturingDate = warehouse.Key.ManufacturingDate.ToString("MM/dd/yyyy"),
                    ReceivedDate = warehouse.Key.ReceivingDate.ToString("MM/dd/yyyy"),
                    TotalStock = warehouse.Key.ActualGood,
                    IsActive = warehouse.Key.IsActive
                });
            return await received.ToListAsync();
        }

        public async Task<bool> CancelAndReturnMaterialsForWarehouseReceive(WarehouseReceiving receive)
        {
            var cancelreceived = await _context.WarehouseReceived.FirstOrDefaultAsync(x => x.Id == receive.Id);

            var cancelinqc = await _context.QC_Receiving.FirstOrDefaultAsync(x => x.Id == cancelreceived.QcReceivingId);


            if (cancelreceived == null)
                return false;


            cancelreceived.IsActive = false;
            cancelinqc.IsWareHouseReceive = null;


            return true;
        }

        public async Task<bool> CancelAndReturnMaterialsInPoSummary(WarehouseReceiving receive)
        {
            var cancelreceived = await _context.WarehouseReceived.FirstOrDefaultAsync(x => x.Id == receive.Id);

            var cancelinqc = await _context.QC_Receiving.FirstOrDefaultAsync(x => x.Id == cancelreceived.QcReceivingId);


            if (cancelreceived == null)
                return false;


            cancelreceived.IsActive = false;
            cancelinqc.IsWareHouseReceive = null;
            cancelinqc.IsActive = false;
            cancelinqc.CancelDate = DateTime.Now;

            return true;
        }

        public async Task<PagedList<WarehouseReceivingDto>> ListForWarehouseReceivingWithPagination(
            UserParams userParams)
        {
            var warehouse = (from posummary in _context.POSummary
                    join receive in _context.QC_Receiving on posummary.Id equals receive.PO_Summary_Id
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
                        IsExpiryApprove = receive.ExpiryIsApprove != null
                    }).Where(x => x.IsWareHouseReceive == false)
                .Where(x => x.IsExpiryApprove == true)
                .Where(x => x.IsActive == true);

            return await PagedList<WarehouseReceivingDto>.CreateAsync(warehouse, userParams.PageNumber,
                userParams.PageSize);
        }

        public async Task<PagedList<WarehouseReceivingDto>> ListForWarehouseReceivingWithPaginationOrig(
            UserParams userParams, string search)
        {
            var warehouse = (from posummary in _context.POSummary
                    join receive in _context.QC_Receiving on posummary.Id equals receive.PO_Summary_Id
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
                        IsExpiryApprove = receive.ExpiryIsApprove != null
                    }).Where(x => x.IsWareHouseReceive == false)
                .Where(x => x.IsExpiryApprove == true)
                .Where(x => x.IsActive == true)
                .Where(x => x.ItemCode.ToLower()
                    .Contains(search.Trim().ToLower()));

            return await PagedList<WarehouseReceivingDto>.CreateAsync(warehouse, userParams.PageNumber,
                userParams.PageSize);
        }

        public async Task<PagedList<RejectWarehouseReceivingDto>> RejectRawMaterialsByWarehousePagination(
            UserParams userParams)
        {
            var qcreceiving = (from posummary in _context.POSummary
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

            var warehousereject = (from warehouse in _context.WarehouseReceived
                join rejectwarehouse in _context.Warehouse_Reject
                    on warehouse.Id equals rejectwarehouse.WarehouseReceivingId into leftJ
                from rejectwarehouse in leftJ.DefaultIfEmpty()
                where warehouse.ConfirmRejectbyWarehouse == true &&
                      warehouse.IsWarehouseReceive == true &&
                      warehouse.ConfirmRejectbyQc == false
                join qc in qcreceiving on warehouse.QcReceivingId equals qc.Id
                group rejectwarehouse by new
                {
                    qc.PO_Number,
                    qc.ItemCode,
                    qc.ItemDescription,
                    qc.Supplier,
                    qc.Uom,
                    qc.QuantityOrderded,
                    warehouse.QuantityGood,
                    warehouse.Id,
                    warehouse.QcReceivingId,
                    warehouse.ReceivingDate,
                    warehouse.TotalReject,
                    warehouse.Reason,
                    warehouse.ConfirmRejectbyQc,
                    warehouse.ConfirmRejectbyWarehouse,
                    warehouse.IsWarehouseReceive
                }
                into total
                select new RejectWarehouseReceivingDto
                {
                    Id = total.Key.Id,
                    PO_Number = total.Key.PO_Number,
                    ItemCode = total.Key.ItemCode,
                    ItemDescription = total.Key.ItemDescription,
                    Supplier = total.Key.Supplier,
                    Uom = total.Key.Uom,
                    QuantityOrdered = total.Key.QuantityOrderded,
                    ActualGood = total.Key.QuantityGood - total.Sum(x => x.Quantity),
                    QcReceivingId = total.Key.QcReceivingId,
                    ReceivingDate = total.Key.ReceivingDate.ToString(),
                    ActualReject = total.Key.TotalReject,
                    Remarks = total.Key.Reason,
                    ConfirmRejectByQc = total.Key.ConfirmRejectbyQc,
                    ConfirmRejectByWarehouse = total.Key.ConfirmRejectbyWarehouse,
                    IsWarehouseReceived = total.Key.IsWarehouseReceive
                });

            return await PagedList<RejectWarehouseReceivingDto>.CreateAsync(warehousereject, userParams.PageNumber,
                userParams.PageSize);
        }

        public async Task<PagedList<RejectWarehouseReceivingDto>> RejectRawMaterialsByWarehousePaginationOrig(
            UserParams userParams, string search)
        {
            var qcreceiving = (from posummary in _context.POSummary
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


            var warehousereject = (from warehouse in _context.WarehouseReceived
                join rejectwarehouse in _context.Warehouse_Reject
                    on warehouse.Id equals rejectwarehouse.WarehouseReceivingId into leftJ
                from rejectwarehouse in leftJ.DefaultIfEmpty()
                where warehouse.ConfirmRejectbyWarehouse == true &&
                      warehouse.IsWarehouseReceive == true &&
                      warehouse.ConfirmRejectbyQc == false
                join qc in qcreceiving on warehouse.QcReceivingId equals qc.Id
                group rejectwarehouse by new
                {
                    qc.PO_Number,
                    qc.ItemCode,
                    qc.ItemDescription,
                    qc.Supplier,
                    qc.Uom,
                    qc.QuantityOrderded,
                    warehouse.QuantityGood,
                    warehouse.Id,
                    warehouse.QcReceivingId,
                    warehouse.ReceivingDate,
                    warehouse.TotalReject,
                    warehouse.Reason,
                    warehouse.ConfirmRejectbyQc,
                    warehouse.ConfirmRejectbyWarehouse,
                    warehouse.IsWarehouseReceive
                }
                into total
                select new RejectWarehouseReceivingDto
                {
                    Id = total.Key.Id,
                    PO_Number = total.Key.PO_Number,
                    ItemCode = total.Key.ItemCode,
                    ItemDescription = total.Key.ItemDescription,
                    Supplier = total.Key.Supplier,
                    Uom = total.Key.Uom,
                    QuantityOrdered = total.Key.QuantityOrderded,
                    ActualGood = total.Key.QuantityGood - total.Sum(x => x.Quantity),
                    QcReceivingId = total.Key.QcReceivingId,
                    ReceivingDate = total.Key.ReceivingDate.ToString(),
                    ActualReject = total.Key.TotalReject,
                    Remarks = total.Key.Reason,
                    ConfirmRejectByQc = total.Key.ConfirmRejectbyQc,
                    ConfirmRejectByWarehouse = total.Key.ConfirmRejectbyWarehouse,
                    IsWarehouseReceived = total.Key.IsWarehouseReceive
                }).Where(x => Convert.ToString(x.PO_Number).ToLower()
                .Contains(search.Trim().ToLower()));

            return await PagedList<RejectWarehouseReceivingDto>.CreateAsync(warehousereject, userParams.PageNumber,
                userParams.PageSize);
        }

        public async Task<IReadOnlyList<WarehouseReceivingDto>> ListOfWarehouseReceivingId()
        {
            var preparationOut = _context.Transformation_Preparation.Where(x => 
                    x.IsActive)
                .GroupBy(x => new
                {
                    x.ItemCode,
                    x.WarehouseId,
                }).Select(x => new ItemStocks
                {
                    ItemCode = x.Key.ItemCode,
                    Out = x.Sum(x => x.WeighingScale),
                    WarehouseId = x.Key.WarehouseId
                });

            var moveorderOut = _context.MoveOrders.Where(x => x.IsActive == true)
                .Where(x => x.IsPrepared == true)
                .GroupBy(x => new
                {
                    x.ItemCode,
                    x.WarehouseId,
                }).Select(x => new ItemStocks
                {
                    ItemCode = x.Key.ItemCode,
                    Out = x.Sum(x => x.QuantityOrdered),
                    WarehouseId = x.Key.WarehouseId
                });

            var issueOut = _context.MiscellaneousIssueDetails.Where(x => x.IsActive == true)
                .Where(x => x.IsTransact == true)
                .GroupBy(x => new
                {
                    x.ItemCode,
                    x.WarehouseId,
                }).Select(x => new ItemStocks
                {
                    ItemCode = x.Key.ItemCode,
                    Out = x.Sum(x => x.Quantity),
                    WarehouseId = x.Key.WarehouseId
                });

            var warehouseInventory = (from warehouse in _context.WarehouseReceived
                where warehouse.IsActive == true
                join preparation in preparationOut
                    on warehouse.Id equals preparation.WarehouseId
                    into leftJ
                from preparation in leftJ.DefaultIfEmpty()
                join moveorder in moveorderOut
                    on warehouse.Id equals moveorder.WarehouseId
                    into leftJ2
                from moveorder in leftJ2.DefaultIfEmpty()
                join issue in issueOut
                    on warehouse.Id equals issue.WarehouseId
                    into leftJ3
                from issue in leftJ3.DefaultIfEmpty()
                group new
                    {
                        warehouse,
                        preparation,
                        moveorder,
                        issue
                    }
                    by new
                    {
                        warehouse.Id,
                        warehouse.QcReceivingId,
                        warehouse.PO_Number,
                        warehouse.ItemCode,
                        warehouse.ItemDescription,
                        warehouse.ManufacturingDate,
                        warehouse.ReceivingDate,
                        warehouse.LotCategory,
                        warehouse.Uom,
                        warehouse.ActualGood,
                        ExpirationDate = warehouse.Expiration != null ? warehouse.Expiration.ToString() : null,
                        warehouse.ExpirationDays,
                        warehouse.Supplier,
                        warehouse.ReceivedBy,
                        PreparationOut = preparation.Out != null ? preparation.Out : 0,
                        MoveOrderOut = moveorder.Out != null ? moveorder.Out : 0,
                        IssueOut = issue.Out != null ? issue.Out : 0
                    }
                into total
                orderby total.Key.ItemCode, total.Key.ExpirationDays ascending
                select new WarehouseReceivingDto
                {
                    Id = total.Key.Id,
                    QCReceivingId = total.Key.QcReceivingId,
                    ItemCode = total.Key.ItemCode,
                    ItemDescription = total.Key.ItemDescription,
                    ManufacturingDate = total.Key.ManufacturingDate.ToString("MM/dd/yyyy"),
                    ActualGood = total.Key.ActualGood - total.Key.PreparationOut - total.Key.MoveOrderOut -
                                 total.Key.IssueOut,
                    ExpirationDate = total.Key.ExpirationDate == null ? null : total.Key.ExpirationDate.ToString(),
                    ExpirationDay = total.Key.ExpirationDays,
                });

            return await warehouseInventory.ToListAsync();
        }


        public async Task<IReadOnlyList<WarehouseReceivingDto>> ListOfWarehouseReceivingId(string search)
        {
            var preparationOut = _context.Transformation_Preparation.Where(x => 
                    x.IsActive == true && 
                    x.ItemCode == search)
                .GroupBy(x => new
                {
                    x.ItemCode,
                    x.WarehouseId,
                }).Select(x => new ItemStocks
                {
                    ItemCode = x.Key.ItemCode,
                    Out = x.Sum(x => x.WeighingScale),
                    WarehouseId = x.Key.WarehouseId
                });

            var moveorderOut = _context.MoveOrders.Where(x => x.IsActive == true)
                .Where(x => x.IsPrepared == true && 
                            x.ItemCode == search)
                .GroupBy(x => new
                {
                    x.ItemCode,
                    x.WarehouseId,
                }).Select(x => new ItemStocks
                {
                    ItemCode = x.Key.ItemCode,
                    Out = x.Sum(x => x.QuantityOrdered),
                    WarehouseId = x.Key.WarehouseId
                });

            var issueOut = _context.MiscellaneousIssueDetails.Where(x => x.IsActive == true)
                .Where(x => x.IsTransact == true &&
                            x.ItemCode == search)
                .GroupBy(x => new
                {
                    x.ItemCode,
                    x.WarehouseId,
                }).Select(x => new ItemStocks
                {
                    ItemCode = x.Key.ItemCode,
                    Out = x.Sum(x => x.Quantity),
                    WarehouseId = x.Key.WarehouseId
                });

            var warehouseInventory = (from warehouse in _context.WarehouseReceived
                where warehouse.IsActive == true
                join preparation in preparationOut
                    on warehouse.Id equals preparation.WarehouseId
                    into leftJ
                from preparation in leftJ.DefaultIfEmpty()
                join moveorder in moveorderOut
                    on warehouse.Id equals moveorder.WarehouseId
                    into leftJ2
                from moveorder in leftJ2.DefaultIfEmpty()
                join issue in issueOut
                    on warehouse.Id equals issue.WarehouseId
                    into leftJ3
                from issue in leftJ3.DefaultIfEmpty()
                group new
                    {
                        warehouse,
                        preparation,
                        moveorder,
                        issue
                    }
                    by new
                    {
                        warehouse.Id,
                        warehouse.QcReceivingId,
                        warehouse.PO_Number,
                        warehouse.ItemCode,
                        warehouse.ItemDescription,
                        warehouse.ManufacturingDate,
                        warehouse.ReceivingDate,
                        warehouse.LotCategory,
                        warehouse.Uom,
                        warehouse.ActualGood,
                        ExpirationDate = warehouse.Expiration == null ? null : warehouse.Expiration.ToString(),
                        warehouse.ExpirationDays,
                        warehouse.Supplier,
                        warehouse.ReceivedBy,
                        PreparationOut = preparation.Out != null ? preparation.Out : 0,
                        MoveOrderOut = moveorder.Out != null ? moveorder.Out : 0,
                        IssueOut = issue.Out != null ? issue.Out : 0
                    }
                into total
                orderby total.Key.ItemCode, total.Key.ExpirationDays ascending
                where total.Key.ItemCode.ToLower().Contains(search.Trim().ToLower())
                select new WarehouseReceivingDto
                {
                    Id = total.Key.Id,
                    QCReceivingId = total.Key.QcReceivingId,
                    ItemCode = total.Key.ItemCode,
                    ItemDescription = total.Key.ItemDescription,
                    ManufacturingDate = total.Key.ManufacturingDate.ToString("MM/dd/yyyy"),
                    ActualGood = total.Key.ActualGood - (total.Key.PreparationOut + total.Key.MoveOrderOut +
                                 total.Key.IssueOut),
                    ExpirationDate = total.Key.ExpirationDate != null ? total.Key.ExpirationDate.ToString() : null,
                    ExpirationDay = total.Key.ExpirationDays,
                });

            return await warehouseInventory.ToListAsync();
        }

        public async Task<PagedList<WarehouseReceivingDto>> GetAllWarehouseIdWithPagination(UserParams userParams)
        {
            var warehouse = _context.WarehouseReceived.OrderByDescending(x => x.ReceivingDate)
                .Where(x => x.IsActive == true)
                .Select(x => new WarehouseReceivingDto
                {
                    Id = x.Id,
                    ItemCode = x.ItemCode,
                    ItemDescription = x.ItemDescription,
                    PO_Number = x.PO_Number,
                    Supplier = x.Supplier,
                    ActualGood = x.ActualGood,
                    ManufacturingDate = x.ManufacturingDate.ToString("MM/dd/yyyy"),
                    ExpirationDate = x.Expiration.ToString(),
                    ExpirationDay = x.ExpirationDays
                });

            return await PagedList<WarehouseReceivingDto>.CreateAsync(warehouse, userParams.PageNumber,
                userParams.PageSize);
        }

        public async Task<PagedList<WarehouseReceivingDto>> GetAllWarehouseIdWithPaginationOrig(UserParams userParams,
            string search)
        {
            var warehouse = _context.WarehouseReceived.OrderByDescending(x => x.ReceivingDate)
                .Where(x => x.IsActive == true)
                .Select(x => new WarehouseReceivingDto
                {
                    Id = x.Id,
                    ItemCode = x.ItemCode,
                    ItemDescription = x.ItemDescription,
                    PO_Number = x.PO_Number,
                    Supplier = x.Supplier,
                    ActualGood = x.ActualGood,
                    ManufacturingDate = x.ManufacturingDate.ToString("MM/dd/yyyy"),
                    ExpirationDate = x.Expiration.ToString(),
                    ExpirationDay = x.ExpirationDays
                }).Where(x => x.ItemCode.ToLower()
                    .Contains(search.Trim().ToLower()));

            return await PagedList<WarehouseReceivingDto>.CreateAsync(warehouse, userParams.PageNumber,
                userParams.PageSize);
        }
    }
}