﻿using ELIXIR.DATA.CORE.INTERFACES.REPORT_INTERFACE;
using ELIXIR.DATA.DATA_ACCESS_LAYER.STORE_CONTEXT;
using ELIXIR.DATA.DTOs.INVENTORY_DTOs;
using ELIXIR.DATA.DTOs.REPORT_DTOs;
using ELIXIR.DATA.DTOs.TRANSFORMATION_DTOs;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ELIXIR.DATA.DATA_ACCESS_LAYER.HELPERS;
using ELIXIR.DATA.DATA_ACCESS_LAYER.MODELS.ORDERING_MODEL;

namespace ELIXIR.DATA.DATA_ACCESS_LAYER.REPOSITORIES.REPORT_REPOSITORY;

public class ReportRepository : IReportRepository
{
    private readonly StoreContext _context;

    public ReportRepository(StoreContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<QCReport>> QcRecevingReport(string DateFrom, string DateTo)
    {
        var items = (from rawmaterials in _context.RawMaterials
                     where rawmaterials.IsActive == true
                     join category in _context.ItemCategories
                         on rawmaterials.ItemCategoryId equals category.Id into leftJ
                     from category in leftJ.DefaultIfEmpty()
                     select new
                     {
                         ItemCode = rawmaterials.ItemCode,
                         ItemCategory = category.ItemCategoryName
                     });


        var report = (from receiving in _context.QC_Receiving
                      where receiving.QC_ReceiveDate >= DateTime.Parse(DateFrom) &&
                            receiving.QC_ReceiveDate <= DateTime.Parse(DateTo) && receiving.IsActive == true
                      join posummary in _context.POSummary
                          on receiving.PO_Summary_Id equals posummary.Id into leftJ
                      from posummary in leftJ.DefaultIfEmpty()
                      join category in items
                          on posummary.ItemCode equals category.ItemCode
                          into leftJ2
                      from category in leftJ2.DefaultIfEmpty()
                      select new QCReport
                      {
                          Id = receiving.Id,
                          QcDate = receiving.QC_ReceiveDate.ToString(),
                          PONumber = posummary != null ? posummary.PO_Number : 0,
                          ItemCode = receiving.ItemCode,
                          ItemDescription = posummary != null ? posummary.ItemDescription : null,
                          Uom = posummary != null ? posummary.UOM : null,
                          Category = category != null ? category.ItemCategory : null,
                          Quantity = receiving.Actual_Delivered,
                          ManufacturingDate = receiving.Manufacturing_Date.ToString(),
                          ExpirationDate = receiving.Expiry_Date.ToString(),
                          TotalReject = receiving.TotalReject,
                          SupplierName = posummary != null ? posummary.VendorName : null,
                          Price = posummary != null ? posummary.UnitPrice : 0,
                          QcBy = receiving.QcBy,
                      });
        return await report.ToListAsync();
    }
    public async Task<PagedList<QCReport>> QcRecevingReportPagination(string DateFrom, string DateTo, UserParams userParams)
    {
        var items = (from rawmaterials in _context.RawMaterials
                     where rawmaterials.IsActive == true
                     join category in _context.ItemCategories
                         on rawmaterials.ItemCategoryId equals category.Id into leftJ
                     from category in leftJ.DefaultIfEmpty()
                     select new
                     {
                         ItemCode = rawmaterials.ItemCode,
                         ItemCategory = category.ItemCategoryName
                     });


        var report = (from receiving in _context.QC_Receiving
                      where receiving.QC_ReceiveDate >= DateTime.Parse(DateFrom) &&
                            receiving.QC_ReceiveDate <= DateTime.Parse(DateTo) && receiving.IsActive == true
                      join posummary in _context.POSummary
                          on receiving.PO_Summary_Id equals posummary.Id into leftJ
                      from posummary in leftJ.DefaultIfEmpty()
                      join category in items
                          on posummary.ItemCode equals category.ItemCode
                          into leftJ2
                      from category in leftJ2.DefaultIfEmpty()
                      select new QCReport
                      {
                          Id = receiving.Id,
                          QcDate = receiving.QC_ReceiveDate.ToString(),
                          PONumber = posummary != null ? posummary.PO_Number : 0,
                          ItemCode = receiving.ItemCode,
                          ItemDescription = posummary != null ? posummary.ItemDescription : null,
                          Uom = posummary != null ? posummary.UOM : null,
                          Category = category != null ? category.ItemCategory : null,
                          Quantity = receiving.Actual_Delivered,
                          ManufacturingDate = receiving.Manufacturing_Date.ToString(),
                          ExpirationDate = receiving.Expiry_Date.ToString(),
                          TotalReject = receiving.TotalReject,
                          SupplierName = posummary != null ? posummary.VendorName : null,
                          Price = posummary != null ? posummary.UnitPrice : 0,
                          QcBy = receiving.QcBy,
                      });

        return await PagedList<QCReport>.CreateAsync(report, userParams.PageNumber, userParams.PageSize);
    }
    public async Task<IReadOnlyList<WarehouseReport>> WarehouseReceivingReport(string DateFrom, string DateTo)
    {
        var warehouse = _context.WarehouseReceived.Where(x =>
                x.ReceivingDate >= DateTime.Parse(DateFrom) && x.ReceivingDate <= DateTime.Parse(DateTo) && x.MiscellaneousReceiptId == null)
            .Where(x => x.IsActive == true)
            .Select(x => new WarehouseReport
            {
                WarehouseId = x.Id,
                PONumber = x.PO_Number,
                ReceiveDate = x.ReceivingDate.ToString(),
                ItemCode = x.ItemCode,
                ItemDescription = x.ItemDescription,
                Uom = x.Uom,
                Category = x.LotCategory,
                Quantity = x.ActualGood,
                ManufacturingDate = x.ManufacturingDate.ToString(),
                ExpirationDate = x.Expiration.ToString(),
                TotalReject = x.TotalReject,
                SupplierName = x.Supplier,
                ReceivedBy = x.ReceivedBy,
                TransactionType = x.TransactionType
            });

        return await warehouse.ToListAsync();
    }
    public async Task<PagedList<WarehouseReport>> WarehouseReceivingReportPagination(string DateFrom, string DateTo, UserParams userParams)
    {
        var warehouse = _context.WarehouseReceived.Where(x =>
                x.ReceivingDate.Date >= DateTime.Parse(DateFrom) && x.ReceivingDate.Date <= DateTime.Parse(DateTo) && x.MiscellaneousReceiptId == null)
            .Where(x => x.IsActive == true)
            .Select(x => new WarehouseReport
            {
                WarehouseId = x.Id,
                PONumber = x.PO_Number,
                ReceiveDate = x.ReceivingDate.ToString(),
                ItemCode = x.ItemCode,
                ItemDescription = x.ItemDescription,
                Uom = x.Uom,
                Category = x.LotCategory,
                Quantity = x.ActualGood,
                ManufacturingDate = x.ManufacturingDate.ToString(),
                ExpirationDate = x.Expiration.ToString(),
                TotalReject = x.TotalReject,
                SupplierName = x.Supplier,
                ReceivedBy = x.ReceivedBy,
                TransactionType = x.TransactionType
            });

        return await PagedList<WarehouseReport>.CreateAsync(warehouse, userParams.PageNumber, userParams.PageSize);
    }

    //Transformation Report
    public async Task<IReadOnlyList<TransformationReport>> TransformationReport(string DateFrom, string DateTo)
    {
        var transform = (from planning in _context.Transformation_Planning
                         where planning.ProdPlan >= DateTime.Parse(DateFrom) && planning.ProdPlan <= DateTime.Parse(DateTo) &&
                               planning.Status == true
                         join preparation in _context.Transformation_Preparation
                             on planning.Id equals preparation.TransformId into leftJ
                         from preparation in leftJ.DefaultIfEmpty()
                         join warehouse in _context.WarehouseReceived
                             on planning.Id equals warehouse.TransformId into left2
                         from warehouse in left2.DefaultIfEmpty()
                         select new TransformationReport
                         {
                             TransformationId = planning.Id,
                             PlanningDate = planning.ProdPlan.ToString(),
                             ItemCode_Formula = planning.ItemCode,
                             ItemDescription_Formula = planning.ItemDescription,
                             Version = planning.Version,
                             Batch = planning.Batch,
                             Formula_Quantity = planning.Quantity,
                             ItemCode_Recipe = preparation.ItemCode != null ? preparation.ItemCode : null,
                             ItemDescription_Recipe = preparation.ItemDescription != null ? preparation.ItemDescription : null,
                             Recipe_Quantity = preparation.WeighingScale != null ? preparation.WeighingScale : 0,
                             DateTransformed = warehouse.ManufacturingDate.ToString()
                         });

        return await transform.ToListAsync();
    }
    public async Task<PagedList<TransformationReport>> TransformationReportPagination(string DateFrom, string DateTo, UserParams userParams)
    {
        var transform = (from planning in _context.Transformation_Planning
                         where planning.ProdPlan >= DateTime.Parse(DateFrom) && planning.ProdPlan <= DateTime.Parse(DateTo) &&
                               planning.Status == true
                         join preparation in _context.Transformation_Preparation
                             on planning.Id equals preparation.TransformId into leftJ
                         from preparation in leftJ.DefaultIfEmpty()
                         join warehouse in _context.WarehouseReceived
                             on planning.Id equals warehouse.TransformId into left2
                         from warehouse in left2.DefaultIfEmpty()
                         select new TransformationReport
                         {
                             TransformationId = planning.Id,
                             PlanningDate = planning.ProdPlan.ToString(),
                             ItemCode_Formula = planning.ItemCode,
                             ItemDescription_Formula = planning.ItemDescription,
                             Version = planning.Version,
                             Batch = planning.Batch,
                             Formula_Quantity = planning.Quantity,
                             ItemCode_Recipe = preparation.ItemCode != null ? preparation.ItemCode : null,
                             ItemDescription_Recipe = preparation.ItemDescription != null ? preparation.ItemDescription : null,
                             Recipe_Quantity = preparation.WeighingScale != null ? preparation.WeighingScale : 0,
                             DateTransformed = warehouse.ManufacturingDate.ToString()
                         });

        return await PagedList<TransformationReport>.CreateAsync(transform, userParams.PageNumber, userParams.PageSize);
    }
    public async Task<IReadOnlyList<MoveOrderReport>> MoveOrderReport(string DateFrom, string DateTo)
    {
        var soh = await _context.WarehouseReceived
            .GroupBy(x => new { x.ItemCode })
            .Select(x => new SOHDTO
            {
                ItemCode = x.Key.ItemCode,
                ActualGood = x.Sum(g => g.ActualGood)
            }).ToListAsync();

        var orders = from moveorder in _context.MoveOrders
                .Include(x => x.AdvancesToEmployees)
                     where moveorder.ApprovedDate.Value.Date >= DateTime.Parse(DateFrom).Date &&
                           moveorder.ApprovedDate.Value.Date <= DateTime.Parse(DateTo).Date &&
                           moveorder.IsActive == true &&
                           moveorder.IsRejectForPreparation == null
                     join transactmoveorder in _context.TransactMoveOrder
                         on moveorder.OrderNo equals transactmoveorder.OrderNo into leftJ
                     from transactmoveorder in leftJ.DefaultIfEmpty()
                     join material in _context.RawMaterials.Include(x => x.ItemCategory).Include(x => x.UOM)
                        on moveorder.ItemCode equals material.ItemCode into left2
                     from material in left2.DefaultIfEmpty()
                     select new
                     {
                         moveorder,
                         transactmoveorder,
                         material
                     };

        var moveOrderReports = orders.Select(order => new MoveOrderReport
        {
            MoveOrderId = order.moveorder.OrderNo,
            CustomerCode = order.moveorder.FarmCode,
            CustomerName = order.moveorder.FarmName,
            ItemCode = order.material.ItemCode,
            ItemDescription = order.material.ItemDescription,
            Uom = order.material.UOM.UOM_Description,
            Category = order.material.ItemCategory.ItemCategoryName,
            Quantity = order.moveorder.QuantityOrdered,
            ExpirationDate = order.moveorder.ExpirationDate.ToString(),
            TransactionType = order.moveorder.DeliveryStatus,
            MoveOrderBy = order.moveorder.PreparedBy,
            MoveOrderDate = order.moveorder.PreparedDate.ToString(),
            TransactedBy = order.transactmoveorder.PreparedBy ?? "N/A",
            TransactedDate = order.transactmoveorder.PreparedDate.ToString() ?? "N/A",
            EmployeeId = order.moveorder.AdvancesToEmployees.EmployeeId,
            EmployeeName = order.moveorder.AdvancesToEmployees.EmployeeName,
            Status = order.transactmoveorder != null ? "Transacted" : "Pending"
        });

        return moveOrderReports.ToList();
    }
    public async Task<PagedList<MoveOrderReport>> MoveOrderReportPagination(string DateFrom, string DateTo, UserParams userParams)
    {
        var soh = _context.WarehouseReceived
            .GroupBy(x => new { x.ItemCode })
            .Select(x => new SOHDTO
            {
                ItemCode = x.Key.ItemCode,
                ActualGood = x.Sum(g => g.ActualGood)
            });

        var orders = from moveorder in _context.MoveOrders
                .Include(x => x.AdvancesToEmployees)
                     where moveorder.ApprovedDate.Value.Date >= DateTime.Parse(DateFrom).Date &&
                           moveorder.ApprovedDate.Value.Date <= DateTime.Parse(DateTo).Date &&
                           moveorder.IsActive == true &&
                           moveorder.IsRejectForPreparation == null
                     join transactmoveorder in _context.TransactMoveOrder
                         on moveorder.OrderNo equals transactmoveorder.OrderNo into leftJ
                     from transactmoveorder in leftJ.DefaultIfEmpty()
                     select new
                     {
                         moveorder,
                         transactmoveorder
                     };

        var moveOrderReports = orders.Select(order => new MoveOrderReport
        {
            MoveOrderId = order.moveorder.OrderNo,
            CustomerCode = order.moveorder.FarmCode,
            CustomerName = order.moveorder.FarmName,
            ItemCode = order.moveorder.ItemCode,
            ItemDescription = order.moveorder.ItemDescription,
            Uom = order.moveorder.Uom,
            Category = order.moveorder.Category,
            Quantity = order.moveorder.QuantityOrdered,
            ExpirationDate = order.moveorder.ExpirationDate.ToString(),
            TransactionType = order.moveorder.DeliveryStatus,
            MoveOrderBy = order.moveorder.PreparedBy,
            MoveOrderDate = order.moveorder.PreparedDate.ToString(),
            TransactedBy = order.transactmoveorder.PreparedBy ?? "N/A",
            TransactedDate = order.transactmoveorder.PreparedDate.ToString() ?? "N/A",
            EmployeeId = order.moveorder.AdvancesToEmployees.EmployeeId,
            EmployeeName = order.moveorder.AdvancesToEmployees.EmployeeName,
            Status = order.transactmoveorder != null ? "Transacted" : "Pending"
        });

        return await PagedList<MoveOrderReport>.CreateAsync(moveOrderReports, userParams.PageNumber, userParams.PageSize);
    }
    public async Task<IReadOnlyList<MiscellaneousReceiptReport>> MReceiptReport(string DateFrom, string DateTo)
    {
        var receipts = (from receiptHeader in _context.MiscellaneousReceipts
                        join receipt in _context.WarehouseReceived
                            on receiptHeader.Id equals receipt.MiscellaneousReceiptId into leftJ
                        from receipt in leftJ.DefaultIfEmpty()
                        where receiptHeader.TransactionDate >= DateTime.Parse(DateFrom) &&
                              receiptHeader.TransactionDate <= DateTime.Parse(DateTo) && receipt.IsActive == true &&
                              receipt.TransactionType == "MiscellaneousReceipt"
                        select new MiscellaneousReceiptReport
                        {
                            ReceiptId = receiptHeader.Id,
                            SupplierCode = receiptHeader.SupplierCode,
                            SupplierName = receiptHeader.Supplier,
                            Details = receiptHeader.Details,
                            ItemCode = receipt.ItemCode,
                            ItemDescription = receipt.ItemDescription,
                            Uom = receipt.Uom,
                            Category = receipt.LotCategory,
                            Quantity = receipt.ActualGood,
                            ExpirationDate = receipt.Expiration.ToString(),
                            TransactBy = receiptHeader.PreparedBy,
                            TransactDate = receipt.ReceivingDate.ToString(),
                            Reason = receiptHeader.Reason
                        });

        return await receipts.ToListAsync();
    }
    public async Task<PagedList<MiscellaneousReceiptReport>> MReceiptReportPagination(string DateFrom, string DateTo, UserParams userParams)
    {
        var receipts = (from receiptHeader in _context.MiscellaneousReceipts
                        join receipt in _context.WarehouseReceived
                            on receiptHeader.Id equals receipt.MiscellaneousReceiptId into leftJ
                        from receipt in leftJ.DefaultIfEmpty()
                        where receipt.ReceivingDate >= DateTime.Parse(DateFrom) &&
                              receipt.ReceivingDate <= DateTime.Parse(DateTo) && receipt.IsActive == true &&
                              receipt.TransactionType == "MiscellaneousReceipt"
                        select new MiscellaneousReceiptReport
                        {
                            ReceiptId = receiptHeader.Id,
                            SupplierCode = receiptHeader.SupplierCode,
                            SupplierName = receiptHeader.Supplier,
                            Details = receiptHeader.Details,
                            ItemCode = receipt.ItemCode,
                            ItemDescription = receipt.ItemDescription,
                            Uom = receipt.Uom,
                            Category = receipt.LotCategory,
                            Quantity = receipt.ActualGood,
                            ExpirationDate = receipt.Expiration.ToString(),
                            TransactBy = receiptHeader.PreparedBy,
                            TransactDate = receipt.ReceivingDate.ToString(),
                            Reason = receiptHeader.Reason
                        });

        return await PagedList<MiscellaneousReceiptReport>.CreateAsync(receipts, userParams.PageNumber, userParams.PageSize);
    }
    public async Task<IReadOnlyList<MiscellaneousIssueReport>> MIssueReport(string DateFrom, string DateTo)
    {
        var issues = (from issue in _context.MiscellaneousIssues
                      where issue.TransactionDate >= DateTime.Parse(DateFrom) &&
                         issue.TransactionDate <= DateTime.Parse(DateTo) && issue.IsActive == true
                      join issuedetails in _context.MiscellaneousIssueDetails
                          on issue.Id equals issuedetails.IssuePKey into leftJ
                      from issuedetails in leftJ.DefaultIfEmpty()
                      where issuedetails.IsActive == true && issuedetails.IsTransact == true
                      select new MiscellaneousIssueReport
                      {
                          IssueId = issue.Id,
                          CustomerCode = issue.CustomerCode,
                          CustomerName = issue.Customer,
                          Details = issue.Remarks,
                          ItemCode = issuedetails != null ? issuedetails.ItemCode : null,
                          ItemDescription = issuedetails != null ? issuedetails.ItemDescription : null,
                          Uom = issuedetails != null ? issuedetails.Uom : null,
                          Quantity = issuedetails != null ? issuedetails.Quantity : 0,
                          ExpirationDate = issuedetails != null ? issuedetails.ExpirationDate.ToString() : null,
                          TransactBy = issue.PreparedBy,
                          TransactDate = issue != null ? issue.TransactionDate.ToString("MM-dd-yyyy") : null,
                          CreatedAt = issuedetails != null ? issuedetails.PreparedDate.ToString("MM-dd-yyyy"): null
                      });

        return await issues.ToListAsync();
    }
    public async Task<PagedList<MiscellaneousIssueReport>> MIssueReportPagination(string DateFrom, string DateTo, UserParams userParams)
    {
        var issues = (from issue in _context.MiscellaneousIssues
                      join issuedetails in _context.MiscellaneousIssueDetails
                          on issue.Id equals issuedetails.IssuePKey into leftJ
                      from issuedetails in leftJ.DefaultIfEmpty()
                      where issuedetails.PreparedDate >= DateTime.Parse(DateFrom) &&
                            issuedetails.PreparedDate <= DateTime.Parse(DateTo) && issuedetails.IsActive == true &&
                            issuedetails.IsTransact == true
                      select new MiscellaneousIssueReport
                      {
                          IssueId = issue.Id,
                          CustomerCode = issue.CustomerCode,
                          CustomerName = issue.Customer,
                          Details = issue.Remarks,
                          ItemCode = issuedetails != null ? issuedetails.ItemCode : null,
                          ItemDescription = issuedetails != null ? issuedetails.ItemDescription : null,
                          Uom = issuedetails != null ? issuedetails.Uom : null,
                          Quantity = issuedetails != null ? issuedetails.Quantity : 0,
                          ExpirationDate = issuedetails != null ? issuedetails.ExpirationDate.ToString() : null,
                          TransactBy = issue.PreparedBy,
                          TransactDate = issuedetails != null ? issuedetails.PreparedDate.ToString() : null
                      });

        return await PagedList<MiscellaneousIssueReport>.CreateAsync(issues, userParams.PageNumber, userParams.PageSize);
    }
    public async Task<IReadOnlyList<WarehouseReport>> NearlyExpireItemsReport(int expirydays)
    {
        var preparationOut = _context.Transformation_Preparation.Where(x => x.IsActive == true)
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
                                  where warehouse.ExpirationDays <= expirydays
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
                                          warehouse.PO_Number,
                                          warehouse.ItemCode,
                                          warehouse.ItemDescription,
                                          warehouse.ManufacturingDate,
                                          warehouse.ReceivingDate,
                                          warehouse.LotCategory,
                                          warehouse.Uom,
                                          warehouse.ActualGood,
                                          warehouse.Expiration,
                                          warehouse.ExpirationDays,
                                          warehouse.Supplier,
                                          warehouse.ReceivedBy,
                                          PreparationOut = preparation.Out != null ? preparation.Out : 0,
                                          MoveOrderOut = moveorder.Out != null ? moveorder.Out : 0,
                                          IssueOut = issue.Out != null ? issue.Out : 0
                                      }
                                 into total
                                  orderby total.Key.ItemCode, total.Key.ExpirationDays ascending
                                  select new WarehouseReport
                                  {
                                      WarehouseId = total.Key.Id,
                                      PONumber = total.Key.PO_Number,
                                      ItemCode = total.Key.ItemCode,
                                      ItemDescription = total.Key.ItemDescription,
                                      Uom = total.Key.Uom,
                                      Category = total.Key.LotCategory,
                                      ReceiveDate = total.Key.ReceivingDate.ToString(),
                                      ManufacturingDate = total.Key.ManufacturingDate.ToString(),
                                      Quantity = total.Key.ActualGood - total.Key.PreparationOut - total.Key.MoveOrderOut -
                                                 total.Key.IssueOut,
                                      ExpirationDate = total.Key.Expiration.ToString(),
                                      ExpirationDays = total.Key.ExpirationDays,
                                      SupplierName = total.Key.Supplier,
                                      ReceivedBy = total.Key.ReceivedBy
                                  });

        return await warehouseInventory.Where(x => x.Quantity != 0)
            .ToListAsync();
    }
    public async Task<PagedList<WarehouseReport>> NearlyExpireItemsReportPagination(int expirydays, UserParams userParams)
    {
        var preparationOut = _context.Transformation_Preparation.Where(x => x.IsActive == true)
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
                                  where warehouse.ExpirationDays <= expirydays
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
                                          warehouse.PO_Number,
                                          warehouse.ItemCode,
                                          warehouse.ItemDescription,
                                          warehouse.ManufacturingDate,
                                          warehouse.ReceivingDate,
                                          warehouse.LotCategory,
                                          warehouse.Uom,
                                          warehouse.ActualGood,
                                          warehouse.Expiration,
                                          warehouse.ExpirationDays,
                                          warehouse.Supplier,
                                          warehouse.ReceivedBy,
                                          PreparationOut = preparation.Out != null ? preparation.Out : 0,
                                          MoveOrderOut = moveorder.Out != null ? moveorder.Out : 0,
                                          IssueOut = issue.Out != null ? issue.Out : 0
                                      }
            into total
                                  orderby total.Key.ItemCode, total.Key.ExpirationDays ascending
                                  select new WarehouseReport
                                  {
                                      WarehouseId = total.Key.Id,
                                      PONumber = total.Key.PO_Number,
                                      ItemCode = total.Key.ItemCode,
                                      ItemDescription = total.Key.ItemDescription,
                                      Uom = total.Key.Uom,
                                      Category = total.Key.LotCategory,
                                      ReceiveDate = total.Key.ReceivingDate.ToString(),
                                      ManufacturingDate = total.Key.ManufacturingDate.ToString(),
                                      Quantity = total.Key.ActualGood - total.Key.PreparationOut - total.Key.MoveOrderOut -
                                                 total.Key.IssueOut,
                                      ExpirationDate = total.Key.Expiration.ToString(),
                                      ExpirationDays = total.Key.ExpirationDays,
                                      SupplierName = total.Key.Supplier,
                                      ReceivedBy = total.Key.ReceivedBy
                                  });

        var result = warehouseInventory.Where(x => x.Quantity != 0);

        return await PagedList<WarehouseReport>.CreateAsync(result, userParams.PageNumber, userParams.PageSize);
    }
    public async Task<IReadOnlyList<MoveOrderReport>> TransactedMoveOrderReport(string dateFrom, string dateTo)
    {
        DateTime fromDate = DateTime.Parse(dateFrom);
        DateTime toDate = DateTime.Parse(dateTo);

        var orders = await _context.MoveOrders
            .Where(moveorder => moveorder.IsActive == true &&
                                moveorder.IsRejectForPreparation != true)
            .Join(_context.TransactMoveOrder,
                moveorder => moveorder.OrderNo,
                transact => transact.OrderNo,
                (moveorder, transact) => new { moveorder, transact })
            .Where(joinResult => joinResult.transact.IsActive == true
                                 && joinResult.transact.IsTransact == true
                                 && joinResult.transact.DeliveryDate >= fromDate
                                 && joinResult.transact.DeliveryDate <= toDate)
            .Select(s => new MoveOrderReport
            {
                MIRId = s.moveorder.TransactionId,
                MoveOrderId = s.moveorder.OrderNo,
                CustomerName = s.moveorder.FarmName,
                CustomerCode = s.moveorder.FarmCode,
                ItemCode = s.moveorder.ItemCode,
                ItemDescription = s.moveorder.ItemDescription,
                Uom = s.moveorder.Uom,
                Quantity = s.moveorder.QuantityOrdered,
                MoveOrderDate = s.moveorder.ApprovedDate.ToString(),
                TransactedBy = s.transact.PreparedBy,
                TransactionType = s.moveorder.DeliveryStatus,
                TransactedDate = s.transact.PreparedDate.ToString(),
                DeliveryDate = s.transact.DeliveryDate.ToString()
            })
            .ToListAsync();

        return orders;
    }
    public async Task<PagedList<MoveOrderReport>> TransactedMoveOrderReportPagination(string dateFrom, string dateTo, UserParams userParams)
    {
        DateTime fromDate = DateTime.Parse(dateFrom);
        DateTime toDate = DateTime.Parse(dateTo);

        var orders = _context.MoveOrders
            .Where(moveorder => moveorder.IsActive == true && moveorder.IsRejectForPreparation != true)
            .Join(_context.TransactMoveOrder,
                moveorder => moveorder.OrderNo,
                transact => transact.OrderNo,
                (moveorder, transact) => new { moveorder, transact })
            .Where(joinResult => joinResult.transact.IsActive
                                 && joinResult.transact.IsTransact
                                 && joinResult.transact.DeliveryDate >= fromDate
                                 && joinResult.transact.DeliveryDate <= toDate)
            .Join(_context.Customers,
                joinResult => joinResult.moveorder.FarmCode,
                customer => customer.CustomerCode,
                (joinResult, customer) => new { joinResult, customer })
            .Select(s => new MoveOrderReport
            {
                MoveOrderId = s.joinResult.moveorder.OrderNo,
                CustomerName = s.customer.CustomerName,
                CustomerCode = s.customer.CustomerCode,
                ItemCode = s.joinResult.moveorder.ItemCode,
                ItemDescription = s.joinResult.moveorder.ItemDescription,
                Uom = s.joinResult.moveorder.Uom,
                Quantity = s.joinResult.moveorder.QuantityOrdered,
                MoveOrderDate = s.joinResult.moveorder.ApprovedDate.ToString(),
                TransactedBy = s.joinResult.transact.PreparedBy,
                TransactionType = s.joinResult.moveorder.DeliveryStatus,
                TransactedDate = s.joinResult.transact.PreparedDate.ToString(),
                DeliveryDate = s.joinResult.transact.DeliveryDate.ToString()
            });


        return await PagedList<MoveOrderReport>.CreateAsync(orders, userParams.PageNumber, userParams.PageSize);
    }
    public async Task<IReadOnlyList<CancelledOrderReport>> OrderSummaryReports(string DateFrom, string DateTo)
    {
        var orders = (from ordering in _context.Orders
                      where ordering.OrderDate >= DateTime.Parse(DateFrom) && ordering.OrderDate <= DateTime.Parse(DateTo)
                      select new CancelledOrderReport
                      {
                          MirId = ordering.TransactId.ToString(),
                          OrderId = ordering.Id,
                          DateNeeded = ordering.DateNeeded.ToString("MM-dd-yyyy"),
                          DateOrdered = ordering.OrderDate.ToString("MM-dd-yyyy"),
                          CustomerCode = ordering.FarmCode,
                          CustomerName = ordering.FarmName,
                          ItemCode = ordering.ItemCode,
                          ItemDescription = ordering.ItemDescription,
                          QuantityOrdered = ordering.QuantityOrdered,
                          CancelledDate = ordering.CancelDate.Value.ToString("MM-dd-yyyy"),
                          Status = ordering.IsActive == false ? "Cancelled" : "Active",
                          CancelledBy = ordering.IsCancelBy,
                          Reason = ordering.OrderCancellationRemarks,
                          Uom = ordering.Uom,
                          Category = ordering.Category
                      }) ;

        return await orders.ToListAsync();
    }
    public async Task<PagedList<CancelledOrderReport>> OrderSummaryReportsPagination(string DateFrom, string DateTo, UserParams userParams)
    {
        var orders = (from ordering in _context.Orders
                      where ordering.OrderDate >= DateTime.Parse(DateFrom) && ordering.OrderDate <= DateTime.Parse(DateTo)
                      select new CancelledOrderReport
                      {
                          MirId = ordering.TransactId.ToString(),
                          OrderId = ordering.Id,
                          DateNeeded = ordering.DateNeeded.ToString("MM-dd-yyyy"),
                          DateOrdered = ordering.OrderDate.ToString("MM-dd-yyyy"),
                          CustomerCode = ordering.FarmCode,
                          CustomerName = ordering.FarmName,
                          ItemCode = ordering.ItemCode,
                          ItemDescription = ordering.ItemDescription,
                          QuantityOrdered = ordering.QuantityOrdered,
                          CancelledDate = ordering.CancelDate.Value.ToString("MM-dd-yyyy"),
                          Status = ordering.IsActive == false ? "Cancelled" : "Active",
                          CancelledBy = ordering.IsCancelBy,
                          Reason = ordering.OrderCancellationRemarks,
                          Uom = ordering.Uom,
                          Category = ordering.Category
                      });

        return await PagedList<CancelledOrderReport>.CreateAsync(orders, userParams.PageNumber, userParams.PageSize);
    }

    public async Task<IReadOnlyList<CancelledOrderReport>> CancelledOrderedReports(string DateFrom, string DateTo)
    {
        var orders = (from ordering in _context.Orders
                      where ordering.OrderDate >= DateTime.Parse(DateFrom) && ordering.OrderDate <= DateTime.Parse(DateTo) &&
                           ordering.IsCancel == true && ordering.IsActive == false
                      select new CancelledOrderReport
                      {
                          MirId = ordering.TransactId.ToString(),
                          OrderId = ordering.Id,
                          DateNeeded = ordering.DateNeeded.ToString("MM-dd-yyyy"),
                          DateOrdered = ordering.OrderDate.ToString("MM-dd-yyyy"),
                          CustomerCode = ordering.FarmCode,
                          CustomerName = ordering.FarmName,
                          ItemCode = ordering.ItemCode,
                          ItemDescription = ordering.ItemDescription,
                          QuantityOrdered = ordering.QuantityOrdered,
                          CancelledDate = ordering.CancelDate.Value.ToString("MM-dd-yyyy"),
                          Status = ordering.IsActive == false ? "Cancelled" : "Active",
                          CancelledBy = ordering.IsCancelBy,
                          Reason = ordering.OrderCancellationRemarks,
                          Uom = ordering.Uom,
                          Category = ordering.Category
                      });

        return await orders.ToListAsync();
    }
    public async Task<PagedList<CancelledOrderReport>> CancelledOrderedReportsPagination(string DateFrom, string DateTo, UserParams userParams)
    {
        var orders = (from ordering in _context.Orders
                      where ordering.OrderDate >= DateTime.Parse(DateFrom) && ordering.OrderDate <= DateTime.Parse(DateTo) &&
                           ordering.IsCancel == true && ordering.IsActive == false
                      select new CancelledOrderReport
                      {
                          OrderId = ordering.Id,
                          DateNeeded = ordering.DateNeeded.ToString(),
                          DateOrdered = ordering.OrderDate.ToString(),
                          CustomerCode = ordering.FarmCode,
                          CustomerName = ordering.FarmName,
                          ItemCode = ordering.ItemCode,
                          ItemDescription = ordering.ItemDescription,
                          QuantityOrdered = ordering.QuantityOrdered,
                          CancelledDate = ordering.CancelDate.ToString(),
                          CancelledBy = ordering.IsCancelBy,
                          Reason = ordering.OrderCancellationRemarks
                      });

        return await PagedList<CancelledOrderReport>.CreateAsync(orders, userParams.PageNumber, userParams.PageSize);
    }
    public async Task<IReadOnlyList<InventoryMovementReport>> InventoryMovementReport(string DateFrom,
       string DateTo, string PlusOne)
    {
        var dateToday = DateTime.Now.ToString("MM/dd/yyyy");


        var getWarehouseStock = _context.WarehouseReceived.Where(x => x.IsActive == true)
            .GroupBy(x => new
            {
                x.ItemCode,
            }).Select(x => new WarehouseInventory
            {
                ItemCode = x.Key.ItemCode,
                ActualGood = x.Sum(x => x.ActualGood)
            });

        var getMoveOrderOutByDate = _context.MoveOrders.Where(x => x.IsActive == true)
            .Where(x => x.IsPrepared == true)
            .Where(x => x.PreparedDate >= DateTime.Parse(DateFrom) && x.PreparedDate <= DateTime.Parse(DateTo) &&
                        x.ApprovedDate != null)
            .GroupBy(x => new
            {
                x.ItemCode,
            }).Select(x => new MoveOrderInventory
            {
                ItemCode = x.Key.ItemCode,
                QuantityOrdered = x.Sum(x => x.QuantityOrdered)
            });

        var getMoveOrderOutByDatePlus = _context.MoveOrders.Where(x => x.IsActive == true)
            .Where(x => x.IsPrepared == true)
            .Where(x => x.TimeStamp.Value.Date >= DateTime.Parse(PlusOne) && x.TimeStamp.Value.Date <= DateTime.Parse(dateToday) &&
                        x.ApprovedDate != null)
            .GroupBy(x => new
            {
                x.ItemCode,
            }).Select(x => new MoveOrderInventory
            {
                ItemCode = x.Key.ItemCode,
                QuantityOrdered = x.Sum(x => x.QuantityOrdered)
            });
        var getTransformationByDate = _context.Transformation_Preparation.Where(x => x.IsActive == true)
            .Where(x => x.PreparedDate >= DateTime.Parse(DateFrom) && x.PreparedDate <= DateTime.Parse(DateTo))
            .GroupBy(x => new
            {
                x.ItemCode,
            }).Select(x => new TransformationInventory
            {
                ItemCode = x.Key.ItemCode,
                WeighingScale = x.Sum(x => x.WeighingScale)
            });

        var getTransformationByDatePlus = _context.Transformation_Preparation.Where(x => x.IsActive == true)
            .Where(x => x.PreparedDate >= DateTime.Parse(PlusOne) && x.PreparedDate <= DateTime.Parse(dateToday))
            .GroupBy(x => new
            {
                x.ItemCode,
            }).Select(x => new TransformationInventory
            {
                ItemCode = x.Key.ItemCode,
                WeighingScale = x.Sum(x => x.WeighingScale)
            });

        var getIssueOutByDate = _context.MiscellaneousIssueDetails.Where(x => x.IsActive == true)
            .Where(x => x.IsTransact == true)
            .Where(x => x.PreparedDate >= DateTime.Parse(DateFrom) && x.PreparedDate <= DateTime.Parse(DateTo))
            .GroupBy(x => new
            {
                x.ItemCode,
            }).Select(x => new IssueInventory
            {
                ItemCode = x.Key.ItemCode,
                Quantity = x.Sum(x => x.Quantity)
            });


        var getIssueOutByDatePlus = _context.MiscellaneousIssueDetails.Where(x => x.IsActive == true)
            .Where(x => x.IsTransact == true)
            .Where(x => x.PreparedDate >= DateTime.Parse(PlusOne) && x.PreparedDate <= DateTime.Parse(dateToday))
            .GroupBy(x => new
            {
                x.ItemCode,
            }).Select(x => new IssueInventory
            {
                ItemCode = x.Key.ItemCode,
                Quantity = x.Sum(x => x.Quantity)
            });


        var getReceivetIn = _context.WarehouseReceived.Where(x => x.IsActive == true)
            .Where(x => x.TransactionType == "Receiving")
            .Where(x => x.ReceivingDate.Date >= DateTime.Parse(DateFrom) && x.ReceivingDate.Date <= DateTime.Parse(DateTo))
            .GroupBy(x => new
            {
                x.ItemCode,
            }).Select(x => new ReceiptInventory
            {
                ItemCode = x.Key.ItemCode,
                Quantity = x.Sum(x => x.ActualGood)
            });

        var getReceivetInPlus = _context.WarehouseReceived.Where(x => x.IsActive == true)
            .Where(x => x.TransactionType == "Receiving")
            .Where(x => x.ReceivingDate.Date >= DateTime.Parse(PlusOne) && x.ReceivingDate.Date <= DateTime.Parse(dateToday))
            .GroupBy(x => new
            {
                x.ItemCode,
            }).Select(x => new ReceiptInventory
            {
                ItemCode = x.Key.ItemCode,
                Quantity = x.Sum(x => x.ActualGood)
            });


        var getTransformIn = _context.WarehouseReceived.Where(x => x.IsActive == true)
            .Where(x => x.TransactionType == "Transformation")
            .Where(x => x.ReceivingDate >= DateTime.Parse(DateFrom) && x.ReceivingDate <= DateTime.Parse(DateTo))
            .GroupBy(x => new
            {
                x.ItemCode,
            }).Select(x => new ReceiptInventory
            {
                ItemCode = x.Key.ItemCode,
                Quantity = x.Sum(x => x.ActualGood)
            });

        var getTransformInPlus = _context.WarehouseReceived.Where(x => x.IsActive == true)
            .Where(x => x.TransactionType == "Transformation")
            .Where(x => x.ReceivingDate >= DateTime.Parse(PlusOne) && x.ReceivingDate <= DateTime.Parse(dateToday))
            .GroupBy(x => new
            {
                x.ItemCode,
            }).Select(x => new ReceiptInventory
            {
                ItemCode = x.Key.ItemCode,
                Quantity = x.Sum(x => x.ActualGood)
            });


        var getReceiptIn = _context.WarehouseReceived.Where(x => x.IsActive == true)
            .Where(x => x.TransactionType == "MiscellaneousReceipt")
            .Where(x => x.ReceivingDate >= DateTime.Parse(DateFrom) && x.ReceivingDate <= DateTime.Parse(DateTo))
            .GroupBy(x => new
            {
                x.ItemCode,
            }).Select(x => new ReceiptInventory
            {
                ItemCode = x.Key.ItemCode,
                Quantity = x.Sum(x => x.ActualGood)
            });

        var getReceiptInPlus = _context.WarehouseReceived.Where(x => x.IsActive == true)
            .Where(x => x.TransactionType == "MiscellaneousReceipt")
            .Where(x => x.ReceivingDate >= DateTime.Parse(PlusOne) && x.ReceivingDate <= DateTime.Parse(dateToday))
            .GroupBy(x => new
            {
                x.ItemCode,
            }).Select(x => new ReceiptInventory
            {
                ItemCode = x.Key.ItemCode,
                Quantity = x.Sum(x => x.ActualGood)
            });

        var getMoveOrderOut = _context.MoveOrders.Where(x => x.IsActive == true)
            .Where(x => x.IsPrepared == true)
            .GroupBy(x => new
            {
                x.ItemCode,
            }).Select(x => new MoveOrderInventory
            {
                ItemCode = x.Key.ItemCode,
                QuantityOrdered = x.Sum(x => x.QuantityOrdered)
            });


        var getTransformation = _context.Transformation_Preparation.Where(x => x.IsActive == true)
            .GroupBy(x => new
            {
                x.ItemCode,
            }).Select(x => new TransformationInventory
            {
                ItemCode = x.Key.ItemCode,
                WeighingScale = x.Sum(x => x.WeighingScale)
            });

        var getIssueOut = _context.MiscellaneousIssueDetails.Where(x => x.IsActive == true)
            .Where(x => x.IsTransact == true)
            .GroupBy(x => new
            {
                x.ItemCode,
            }).Select(x => new IssueInventory
            {
                ItemCode = x.Key.ItemCode,
                Quantity = x.Sum(x => x.Quantity)
            });

        var getSOH = (from warehouse in getWarehouseStock
                      join preparation in getTransformation
                          on warehouse.ItemCode equals preparation.ItemCode
                          into leftJ1
                      from preparation in leftJ1.DefaultIfEmpty()
                      join issue in getIssueOut
                          on warehouse.ItemCode equals issue.ItemCode
                          into leftJ2
                      from issue in leftJ2.DefaultIfEmpty()
                      join moveorder in getMoveOrderOut
                          on warehouse.ItemCode equals moveorder.ItemCode
                          into leftJ3
                      from moveorder in leftJ3.DefaultIfEmpty()
                      join receipt in getReceiptIn
                          on warehouse.ItemCode equals receipt.ItemCode
                          into leftJ4
                      from receipt in leftJ4.DefaultIfEmpty()
                      group new
                      {
                          warehouse,
                          preparation,
                          moveorder,
                          receipt,
                          issue
                      }
                          by new
                          {
                              warehouse.ItemCode
                          }
            into total
                      select new SOHInventory
                      {
                          ItemCode = total.Key.ItemCode,
                          SOH = total.Sum(x => x.warehouse.ActualGood == 0 ? 0 : x.warehouse.ActualGood) -
                                total.Sum(x => x.preparation.WeighingScale == 0 ? 0 : x.preparation.WeighingScale) -
                                total.Sum(x => x.moveorder.QuantityOrdered == 0 ? 0 : x.moveorder.QuantityOrdered)
                      });

        var movementInventory = (from rawmaterial in _context.RawMaterials
                                 join moveorder in getMoveOrderOutByDate
                                     on rawmaterial.ItemCode equals moveorder.ItemCode
                                     into leftJ
                                 from moveorder in leftJ.DefaultIfEmpty()
                                 join transformation in getTransformationByDate
                                     on rawmaterial.ItemCode equals transformation.ItemCode
                                     into leftJ2
                                 from transformation in leftJ2.DefaultIfEmpty()
                                 join issue in getIssueOutByDate
                                     on rawmaterial.ItemCode equals issue.ItemCode
                                     into leftJ3
                                 from issue in leftJ3.DefaultIfEmpty()
                                 join receive in getReceivetIn
                                     on rawmaterial.ItemCode equals receive.ItemCode
                                     into leftJ4
                                 from receive in leftJ4.DefaultIfEmpty()
                                 join transformIn in getTransformIn
                                     on rawmaterial.ItemCode equals transformIn.ItemCode
                                     into leftJ5
                                 from transformIn in leftJ5.DefaultIfEmpty()
                                 join receipt in getReceiptIn
                                     on rawmaterial.ItemCode equals receipt.ItemCode
                                     into leftJ6
                                 from receipt in leftJ6.DefaultIfEmpty()
                                 join SOH in getSOH
                                     on rawmaterial.ItemCode equals SOH.ItemCode
                                     into leftJ7
                                 from SOH in leftJ7.DefaultIfEmpty()
                                 join receivePlus in getReceivetInPlus
                                     on rawmaterial.ItemCode equals receivePlus.ItemCode
                                     into leftJ8
                                 from receivePlus in leftJ8.DefaultIfEmpty()
                                 join transformPlus in getTransformInPlus
                                     on rawmaterial.ItemCode equals transformPlus.ItemCode
                                     into leftJ9
                                 from transformPlus in leftJ9.DefaultIfEmpty()
                                 join receiptPlus in getReceiptInPlus
                                     on rawmaterial.ItemCode equals receiptPlus.ItemCode
                                     into leftJ10
                                 from receiptPlus in leftJ10.DefaultIfEmpty()
                                 join moveorderPlus in getMoveOrderOutByDatePlus
                                     on rawmaterial.ItemCode equals moveorderPlus.ItemCode
                                     into leftJ11
                                 from moveorderPlus in leftJ11.DefaultIfEmpty()
                                 join transformoutPlus in getTransformationByDatePlus
                                     on rawmaterial.ItemCode equals transformoutPlus.ItemCode
                                     into leftJ12
                                 from transformoutPlus in leftJ12.DefaultIfEmpty()
                                 join issuePlus in getIssueOutByDatePlus
                                     on rawmaterial.ItemCode equals issuePlus.ItemCode
                                     into leftJ13
                                 from issuePlus in leftJ13.DefaultIfEmpty()
                                 group new
                                 {
                                     rawmaterial,
                                     moveorder,
                                     transformation,
                                     issue,
                                     receive,
                                     transformIn,
                                     receipt,
                                     SOH,
                                     receivePlus,
                                     transformPlus,
                                     receiptPlus,
                                     moveorderPlus,
                                     transformoutPlus,
                                     issuePlus
                                 }
                                     by new
                                     {
                                         rawmaterial.ItemCode,
                                         rawmaterial.ItemDescription,
                                         rawmaterial.ItemCategory.ItemCategoryName,
                                         MoveOrder = moveorder.QuantityOrdered != null ? moveorder.QuantityOrdered : 0,
                                         Transformation = transformation.WeighingScale != null ? transformation.WeighingScale : 0,
                                         Issue = issue.Quantity != null ? issue.Quantity : 0,
                                         ReceiptIn = receipt.Quantity != null ? receipt.Quantity : 0,
                                         ReceiveIn = receive.Quantity != null ? receive.Quantity : 0,
                                         TransformIn = transformIn.Quantity != null ? transformIn.Quantity : 0,
                                         SOH = SOH.SOH != null ? SOH.SOH : 0,
                                         ReceivePlus = receivePlus.Quantity != null ? receivePlus.Quantity : 0,
                                         TransformPlus = transformPlus.Quantity != null ? transformPlus.Quantity : 0,
                                         ReceiptPlus = receiptPlus.Quantity != null ? receiptPlus.Quantity : 0,
                                         MoveOrderPlus = moveorderPlus.QuantityOrdered != null ? moveorderPlus.QuantityOrdered : 0,
                                         TransformOutPlus = transformoutPlus.WeighingScale != null ? transformoutPlus.WeighingScale : 0,
                                         IssuePlus = issuePlus.Quantity != null ? issuePlus.Quantity : 0,
                                     }
            into total
                                 select new InventoryMovementReport
                                 {
                                     ItemCode = total.Key.ItemCode,
                                     ItemDescription = total.Key.ItemDescription,
                                     ItemCategory = total.Key.ItemCategoryName,
                                     TotalMoveOrderedOut = total.Key.MoveOrder + total.Key.Transformation + total.Key.Issue,
                                     TotalMiscIssue = total.Key.Issue,
                                     TotalMicReceipt = total.Key.ReceiptIn,
                                     TotalReceived = total.Key.ReceiveIn,
                                     Ending = (total.Key.ReceiveIn + total.Key.ReceiptIn + total.Key.TransformIn) -
                                              (total.Key.MoveOrder + total.Key.Transformation + total.Key.Issue),
                                     CurrentStock = total.Key.SOH - total.Key.Issue,
                                     PurchasedOrder = total.Key.ReceivePlus + total.Key.TransformPlus + total.Key.ReceiptPlus,
                                     OthersPlus = total.Key.MoveOrderPlus + total.Key.TransformOutPlus + total.Key.IssuePlus
                                 });

        return await movementInventory.ToListAsync();
    }
    public async Task<PagedList<InventoryMovementReport>> InventoryMovementReportPagination(string DateFrom,
        string DateTo, string PlusOne, UserParams userParams)
    {
        var dateToday = DateTime.Now.ToString("MM/dd/yyyy");


        var getWarehouseStock = _context.WarehouseReceived.Where(x => x.IsActive == true)
            .GroupBy(x => new
            {
                x.ItemCode,
            }).Select(x => new WarehouseInventory
            {
                ItemCode = x.Key.ItemCode,
                ActualGood = x.Sum(x => x.ActualGood)
            });

        var getMoveOrderOutByDate = _context.MoveOrders.Where(x => x.IsActive == true)
            .Where(x => x.IsPrepared == true)
            .Where(x => x.TimeStamp.Value.Date >= DateTime.Parse(DateFrom) && x.TimeStamp.Value.Date <= DateTime.Parse(DateTo) &&
                        x.ApprovedDate != null)
            .GroupBy(x => new
            {
                x.ItemCode,
            }).Select(x => new MoveOrderInventory
            {
                ItemCode = x.Key.ItemCode,
                QuantityOrdered = x.Sum(x => x.QuantityOrdered)
            });

        var getMoveOrderOutByDatePlus = _context.MoveOrders.Where(x => x.IsActive == true)
            .Where(x => x.IsPrepared == true)
            .Where(x => x.TimeStamp.Value.Date >= DateTime.Parse(PlusOne) && x.TimeStamp.Value.Date <= DateTime.Parse(dateToday) &&
                        x.ApprovedDate != null)
            .GroupBy(x => new
            {
                x.ItemCode,
            }).Select(x => new MoveOrderInventory
            {
                ItemCode = x.Key.ItemCode,
                QuantityOrdered = x.Sum(x => x.QuantityOrdered)
            });
        var getTransformationByDate = _context.Transformation_Preparation.Where(x => x.IsActive == true)
            .Where(x => x.PreparedDate >= DateTime.Parse(DateFrom) && x.PreparedDate <= DateTime.Parse(DateTo))
            .GroupBy(x => new
            {
                x.ItemCode,
            }).Select(x => new TransformationInventory
            {
                ItemCode = x.Key.ItemCode,
                WeighingScale = x.Sum(x => x.WeighingScale)
            });

        var getTransformationByDatePlus = _context.Transformation_Preparation.Where(x => x.IsActive == true)
            .Where(x => x.PreparedDate.Date >= DateTime.Parse(PlusOne) && x.PreparedDate.Date <= DateTime.Parse(dateToday))
            .GroupBy(x => new
            {
                x.ItemCode,
            }).Select(x => new TransformationInventory
            {
                ItemCode = x.Key.ItemCode,
                WeighingScale = x.Sum(x => x.WeighingScale)
            });

        var getIssueOutByDate = _context.MiscellaneousIssueDetails.Where(x => x.IsActive == true)
            .Where(x => x.IsTransact == true)
            .Where(x => x.PreparedDate.Date >= DateTime.Parse(DateFrom) && x.PreparedDate.Date <= DateTime.Parse(DateTo))
            .GroupBy(x => new
            {
                x.ItemCode,
            }).Select(x => new IssueInventory
            {
                ItemCode = x.Key.ItemCode,
                Quantity = x.Sum(x => x.Quantity)
            });


        var getIssueOutByDatePlus = _context.MiscellaneousIssueDetails.Where(x => x.IsActive == true)
            .Where(x => x.IsTransact == true)
            .Where(x => x.PreparedDate.Date >= DateTime.Parse(PlusOne) && x.PreparedDate.Date <= DateTime.Parse(dateToday))
            .GroupBy(x => new
            {
                x.ItemCode,
            }).Select(x => new IssueInventory
            {
                ItemCode = x.Key.ItemCode,
                Quantity = x.Sum(x => x.Quantity)
            });


        var getReceivetIn = _context.WarehouseReceived.Where(x => x.IsActive == true)
            .Where(x => x.TransactionType == "Receiving")
            .Where(x => x.ReceivingDate.Date >= DateTime.Parse(DateFrom) && x.ReceivingDate.Date <= DateTime.Parse(DateTo))
            .GroupBy(x => new
            {
                x.ItemCode,
            }).Select(x => new ReceiptInventory
            {
                ItemCode = x.Key.ItemCode,
                Quantity = x.Sum(x => x.ActualGood)
            });

        var getReceivetInPlus = _context.WarehouseReceived.Where(x => x.IsActive == true)
            .Where(x => x.TransactionType == "Receiving")
            .Where(x => x.ReceivingDate.Date >= DateTime.Parse(PlusOne) && x.ReceivingDate.Date <= DateTime.Parse(dateToday))
            .GroupBy(x => new
            {
                x.ItemCode,
            }).Select(x => new ReceiptInventory
            {
                ItemCode = x.Key.ItemCode,
                Quantity = x.Sum(x => x.ActualGood)
            });


        var getTransformIn = _context.WarehouseReceived.Where(x => x.IsActive == true)
            .Where(x => x.TransactionType == "Transformation")
            .Where(x => x.ReceivingDate >= DateTime.Parse(DateFrom) && x.ReceivingDate <= DateTime.Parse(DateTo))
            .GroupBy(x => new
            {
                x.ItemCode,
            }).Select(x => new ReceiptInventory
            {
                ItemCode = x.Key.ItemCode,
                Quantity = x.Sum(x => x.ActualGood)
            });

        var getTransformInPlus = _context.WarehouseReceived.Where(x => x.IsActive == true)
            .Where(x => x.TransactionType == "Transformation")
            .Where(x => x.ReceivingDate >= DateTime.Parse(PlusOne) && x.ReceivingDate <= DateTime.Parse(dateToday))
            .GroupBy(x => new
            {
                x.ItemCode,
            }).Select(x => new ReceiptInventory
            {
                ItemCode = x.Key.ItemCode,
                Quantity = x.Sum(x => x.ActualGood)
            });


        var getReceiptIn = _context.WarehouseReceived.Where(x => x.IsActive == true)
            .Where(x => x.TransactionType == "MiscellaneousReceipt")
            .Where(x => x.ReceivingDate >= DateTime.Parse(DateFrom) && x.ReceivingDate <= DateTime.Parse(DateTo))
            .GroupBy(x => new
            {
                x.ItemCode,
            }).Select(x => new ReceiptInventory
            {
                ItemCode = x.Key.ItemCode,
                Quantity = x.Sum(x => x.ActualGood)
            });

        var getReceiptInPlus = _context.WarehouseReceived.Where(x => x.IsActive == true)
            .Where(x => x.TransactionType == "MiscellaneousReceipt")
            .Where(x => x.ReceivingDate >= DateTime.Parse(PlusOne) && x.ReceivingDate <= DateTime.Parse(dateToday))
            .GroupBy(x => new
            {
                x.ItemCode,
            }).Select(x => new ReceiptInventory
            {
                ItemCode = x.Key.ItemCode,
                Quantity = x.Sum(x => x.ActualGood)
            });

        var getMoveOrderOut = _context.MoveOrders.Where(x => x.IsActive == true)
            .Where(x => x.IsPrepared == true)
            .GroupBy(x => new
            {
                x.ItemCode,
            }).Select(x => new MoveOrderInventory
            {
                ItemCode = x.Key.ItemCode,
                QuantityOrdered = x.Sum(x => x.QuantityOrdered)
            });


        var getTransformation = _context.Transformation_Preparation.Where(x => x.IsActive == true)
            .GroupBy(x => new
            {
                x.ItemCode,
            }).Select(x => new TransformationInventory
            {
                ItemCode = x.Key.ItemCode,
                WeighingScale = x.Sum(x => x.WeighingScale)
            });

        var getIssueOut = _context.MiscellaneousIssueDetails.Where(x => x.IsActive == true)
            .Where(x => x.IsTransact == true)
            .GroupBy(x => new
            {
                x.ItemCode,
            }).Select(x => new IssueInventory
            {
                ItemCode = x.Key.ItemCode,
                Quantity = x.Sum(x => x.Quantity)
            });

        var getSOH = (from warehouse in getWarehouseStock
                      join moveorder in getMoveOrderOut
                          on warehouse.ItemCode equals moveorder.ItemCode
                          into leftJ3
                      from moveorder in leftJ3.DefaultIfEmpty()
                      group new
                      {
                          warehouse,
                          moveorder
                      }
                          by new
                          {
                              warehouse.ItemCode
                          }
            into total
                      select new SOHInventory
                      {
                          ItemCode = total.Key.ItemCode,
                          SOH = total.Sum(x => x.warehouse.ActualGood == 0 ? 0 : x.warehouse.ActualGood) -
                                total.Sum(x => x.moveorder.QuantityOrdered == 0 ? 0 : x.moveorder.QuantityOrdered)
                      });

        var movementInventory = (from rawmaterial in _context.RawMaterials
                                 join moveorder in getMoveOrderOutByDate
                                     on rawmaterial.ItemCode equals moveorder.ItemCode
                                     into leftJ
                                 from moveorder in leftJ.DefaultIfEmpty()
                                 join transformation in getTransformationByDate
                                     on rawmaterial.ItemCode equals transformation.ItemCode
                                     into leftJ2
                                 from transformation in leftJ2.DefaultIfEmpty()
                                 join issue in getIssueOutByDate
                                     on rawmaterial.ItemCode equals issue.ItemCode
                                     into leftJ3
                                 from issue in leftJ3.DefaultIfEmpty()
                                 join receive in getReceivetIn
                                     on rawmaterial.ItemCode equals receive.ItemCode
                                     into leftJ4
                                 from receive in leftJ4.DefaultIfEmpty()
                                 join transformIn in getTransformIn
                                     on rawmaterial.ItemCode equals transformIn.ItemCode
                                     into leftJ5
                                 from transformIn in leftJ5.DefaultIfEmpty()
                                 join receipt in getReceiptIn
                                     on rawmaterial.ItemCode equals receipt.ItemCode
                                     into leftJ6
                                 from receipt in leftJ6.DefaultIfEmpty()
                                 join SOH in getSOH
                                     on rawmaterial.ItemCode equals SOH.ItemCode
                                     into leftJ7
                                 from SOH in leftJ7.DefaultIfEmpty()
                                 join receivePlus in getReceivetInPlus
                                     on rawmaterial.ItemCode equals receivePlus.ItemCode
                                     into leftJ8
                                 from receivePlus in leftJ8.DefaultIfEmpty()
                                 join transformPlus in getTransformInPlus
                                     on rawmaterial.ItemCode equals transformPlus.ItemCode
                                     into leftJ9
                                 from transformPlus in leftJ9.DefaultIfEmpty()
                                 join receiptPlus in getReceiptInPlus
                                     on rawmaterial.ItemCode equals receiptPlus.ItemCode
                                     into leftJ10
                                 from receiptPlus in leftJ10.DefaultIfEmpty()
                                 join moveorderPlus in getMoveOrderOutByDatePlus
                                     on rawmaterial.ItemCode equals moveorderPlus.ItemCode
                                     into leftJ11
                                 from moveorderPlus in leftJ11.DefaultIfEmpty()
                                 join transformoutPlus in getTransformationByDatePlus
                                     on rawmaterial.ItemCode equals transformoutPlus.ItemCode
                                     into leftJ12
                                 from transformoutPlus in leftJ12.DefaultIfEmpty()
                                 join issuePlus in getIssueOutByDatePlus
                                     on rawmaterial.ItemCode equals issuePlus.ItemCode
                                     into leftJ13
                                 from issuePlus in leftJ13.DefaultIfEmpty()
                                 group new
                                 {
                                     rawmaterial,
                                     moveorder,
                                     transformation,
                                     issue,
                                     receive,
                                     transformIn,
                                     receipt,
                                     SOH,
                                     receivePlus,
                                     transformPlus,
                                     receiptPlus,
                                     moveorderPlus,
                                     transformoutPlus,
                                     issuePlus
                                 }
                                     by new
                                     {
                                         rawmaterial.ItemCode,
                                         rawmaterial.ItemDescription,
                                         rawmaterial.ItemCategory.ItemCategoryName,
                                         MoveOrder = moveorder.QuantityOrdered != null ? moveorder.QuantityOrdered : 0,
                                         Transformation = transformation.WeighingScale != null ? transformation.WeighingScale : 0,
                                         Issue = issue.Quantity != null ? issue.Quantity : 0,
                                         ReceiptIn = receipt.Quantity != null ? receipt.Quantity : 0,
                                         ReceiveIn = receive.Quantity != null ? receive.Quantity : 0,
                                         TransformIn = transformIn.Quantity != null ? transformIn.Quantity : 0,
                                         SOH = SOH.SOH != null ? SOH.SOH : 0,
                                         ReceivePlus = receivePlus.Quantity != null ? receivePlus.Quantity : 0,
                                         TransformPlus = transformPlus.Quantity != null ? transformPlus.Quantity : 0,
                                         ReceiptPlus = receiptPlus.Quantity != null ? receiptPlus.Quantity : 0,
                                         MoveOrderPlus = moveorderPlus.QuantityOrdered != null ? moveorderPlus.QuantityOrdered : 0,
                                         TransformOutPlus = transformoutPlus.WeighingScale != null ? transformoutPlus.WeighingScale : 0,
                                         IssuePlus = issuePlus.Quantity != null ? issuePlus.Quantity : 0,
                                     }
            into total
                                 select new InventoryMovementReport
                                 {
                                     ItemCode = total.Key.ItemCode,
                                     ItemDescription = total.Key.ItemDescription,
                                     ItemCategory = total.Key.ItemCategoryName,
                                     TotalMoveOrderedOut = total.Key.MoveOrder,
                                     TotalMiscIssue = total.Key.Issue,
                                     TotalReceived = total.Key.ReceiveIn,
                                     TotalMicReceipt = total.Key.ReceiptIn,
                                     Ending = (total.Key.ReceiveIn + total.Key.ReceiptIn + total.Key.TransformIn) -
                                              (total.Key.MoveOrder + total.Key.Transformation + total.Key.Issue),
                                     CurrentStock = total.Key.SOH - total.Key.Issue,
                                     PurchasedOrder = total.Key.ReceivePlus + total.Key.ReceiptPlus,
                                     OthersPlus = total.Key.MoveOrderPlus + total.Key.TransformOutPlus + total.Key.IssuePlus
                                 });

        return await PagedList<InventoryMovementReport>.CreateAsync(movementInventory, userParams.PageNumber, userParams.PageSize);
    }
        /// <summary>
        /// Retrieves a consolidated report based on the specified date range and department.
        /// </summary>
        /// <param name="dateFrom">The start date of the report in "MM/dd/yyyy" format.</param>
        /// <param name="dateTo">The end date of the report in "MM/dd/yyyy" format.</param>
        /// <param name="Department">The department for which the report is generated.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a read-only list of ConsolidatedReport objects.</returns>
    public async Task<IReadOnlyList<ConsolidatedReport>> ConsolidatedReport(string dateFrom, string dateTo, string Department)
    {
        var fromDate = DateTime.Parse(dateFrom).Date;
        var toDate = DateTime.Parse(dateTo).Date;

        var individualDifferences = from wr in _context.WarehouseReceived
                                    join mo in _context.MoveOrders
                                        on wr.Id equals mo.WarehouseId
                                        into moveOrders
                                    from mo in moveOrders.DefaultIfEmpty()
                                    where wr.IsActive && wr.IsWarehouseReceive
                                    select new
                                    {
                                        wr.ItemCode,
                                        wr.ActualGood,
                                        QuantityOrdered = mo != null ? mo.QuantityOrdered : 0,
                                        CostByWarehouse = wr.UnitCost * (wr.ActualGood - (mo != null ? mo.QuantityOrdered : 0))
                                    };

        // Calculate the sum of differences per ItemCode
        var totalDifferences = individualDifferences
            .GroupBy(id => id.ItemCode)
            .Select(g => new
            {
                ItemCode = g.Key,
                TotalDifference = g.Sum(id => id.CostByWarehouse)
            });

        // Calculate the average UnitCost per ItemCode
        var averageUnitCosts = individualDifferences
            .GroupBy(id => id.ItemCode)
            .Select(g => new
            {
                ItemCode = g.Key,
                AvgUnitCost = g.Average(id =>
                    (id.ActualGood - id.QuantityOrdered) == 0
                        ? 0
                        : id.CostByWarehouse / (id.ActualGood - id.QuantityOrdered))
            });

        // Combine the results
        var finalResult = from id in individualDifferences
                          join td in totalDifferences
                              on id.ItemCode equals td.ItemCode
                          join auc in averageUnitCosts
                              on id.ItemCode equals auc.ItemCode
                          select new
                          {
                              id.ItemCode,
                              id.ActualGood,
                              id.QuantityOrdered,
                              Difference = id.CostByWarehouse,
                              td.TotalDifference,
                              auc.AvgUnitCost
                          };


        var consolidatedReports = new List<ConsolidatedReport>();
        var rawMaterials = await _context.RawMaterials
            .Include(x => x.ItemCategory)
            .Include(uom => uom.UOM )
            .Select(rawMaterial => new
            {
                rawMaterial.ItemCode,
                rawMaterial.ItemDescription,
                rawMaterial.UOM,
                rawMaterial.ItemCategory.ItemCategoryName,
                rawMaterial.UOM.UOM_Description
            })
            .ToListAsync();

        var warehouseReceived = await _context.WarehouseReceived
            .Select(warehouseReceived => new
            {
                warehouseReceived.PO_Number,
                warehouseReceived.QcReceivingId,
                warehouseReceived.Id,
                warehouseReceived.ItemCode,
                warehouseReceived.Uom,
                warehouseReceived.MiscellaneousReceiptId,
                warehouseReceived.UnitCost,
                warehouseReceived.ActualGood
            })
            .ToListAsync();

        var receivingReports = await _context.QC_Receiving
            .Where(wr => wr.IsWareHouseReceive == true)
            .Select(receiving => new
            {
                receiving.Id,
                receiving.QC_ReceiveDate,
                receiving.ItemCode,
                receiving.Actual_Delivered,
                TransactionType = "Receiving"
            })
            .ToListAsync();

        var cancelledOrders = await (from ordering in _context.Orders
                      where ordering.OrderDate.Date >= fromDate && ordering.OrderDate.Date <= toDate &&
                            ordering.IsCancel == true && ordering.IsActive == false
                      select new CancelledOrderReport
                      {
                          MirId = ordering.TransactId.ToString(),
                          OrderId = ordering.Id,
                          DateNeeded = ordering.DateNeeded.ToString(),
                          DateOrdered = ordering.OrderDate.ToString(),
                          CustomerCode = ordering.FarmCode,
                          CustomerName = ordering.FarmName,
                          ItemCode = ordering.ItemCode,
                          ItemDescription = ordering.ItemDescription,
                          QuantityOrdered = ordering.QuantityOrdered,
                          CancelledDate = ordering.CancelDate.ToString(),
                          CancelledBy = ordering.IsCancelBy,
                          Reason = ordering.OrderCancellationRemarks
                      }).ToListAsync();

        var moveOrderReports = await

        //(from transact in _context.TransactMoveOrder
        //    where transact.IsActive == true && transact.IsTransact == true &&
        //          transact.PreparedDate.Value.Date >= fromDate && transact.PreparedDate.Value.Date <= toDate
        //    join moveorder in _context.MoveOrders.Where(x => x.IsActive)
        //        on transact.OrderNo equals moveorder.OrderNo into leftJ
        // from moveorder in leftJ.DefaultIfEmpty()

            (from transactmoveorder in _context.TransactMoveOrder
             where transactmoveorder.DeliveryDate.Value.Date >= fromDate && transactmoveorder.DeliveryDate.Value.Date <= toDate
             join moveorder in _context.MoveOrders.Where(x => x.IsActive == true && x.IsRejectForPreparation == null)
                 on transactmoveorder.OrderNo equals moveorder.OrderNo into leftJ
            from moveorder in leftJ.DefaultIfEmpty()
          //_context.MoveOrders
          //   .Include(x => x.AdvancesToEmployees)
          //   where moveorder.IsActive == true
          //   join transactmoveorder in _context.TransactMoveOrder.Where(x => x.DeliveryDate.Value >= fromDate && x.DeliveryDate.Value <= toDate)
          //       on moveorder.OrderNo equals transactmoveorder.OrderNo into leftJ
          //   from transactmoveorder in leftJ.DefaultIfEmpty()
             select new
             {
                 moveorder.TransactionId,
                 moveorder.Id,
                 moveorder.ItemCode,
                 moveorder.ItemDescription,
                 moveorder.Category,
                 moveorder.OrderNo,
                 moveorder.FarmName,
                 moveorder.FarmCode,
                 moveorder.Uom,
                 moveorder.CompanyCode,
                 moveorder.CompanyName,
                 moveorder.DepartmentCode,
                 moveorder.DepartmentName,
                 moveorder.AccountTitleCode,
                 moveorder.AccountTitles,
                 moveorder.LocationCode,
                 moveorder.LocationName,
                 moveorder.QuantityOrdered,
                 moveorder.WarehouseId,
                 transactmoveorder.PreparedDate,
                 transactmoveorder.PreparedBy,
                 moveorder.DeliveryStatus,
                 moveorder.AdvancesToEmployees.EmployeeName,
                 moveorder.AdvancesToEmployees.EmployeeId
             }).ToListAsync();

        //var receiptInReport = await _context.MiscellaneousReceipts
        //    .Where(receipt => receipt.IsActive == true)
        //    .Where(receipt => receipt.TransactionDate.Date >= fromDate && receipt.TransactionDate.Date <= toDate)
        //    .Select(receiptIn => new
        //    {
        //        receiptIn.Id,
        //        receiptIn.CompanyName,
        //        receiptIn.CompanyCode,
        //        receiptIn.DepartmentCode,
        //        receiptIn.DepartmentName,
        //        receiptIn.AccountTitles,
        //        receiptIn.LocationCode,
        //        receiptIn.LocationName,
        //        receiptIn.TotalQuantity,
        //        receiptIn.PreparedDate
        //    })
        //    .ToListAsync();

        
            consolidatedReports = _context.WarehouseReceived
                    .Where(wr => wr.IsActive == true && wr.MiscellaneousReceiptId == null)
                    .Where(wr => wr.ReceivingDate.Date >= fromDate && wr.ReceivingDate.Date <= toDate)
                    .Join(
                        _context.RawMaterials
                            .Include(rm => rm.UOM)
                            .Include(rm => rm.ItemCategory),
                        receiving => receiving.ItemCode,
                        rawMaterial => rawMaterial.ItemCode,
                        (receiving, rawMaterialsGroup) =>
                            new { Receiving = receiving, RawMaterialsGroup = rawMaterialsGroup })
                     .Join(
                        _context.QC_Receiving,
                        joinResult => joinResult.Receiving.QcReceivingId,
                        qc => qc.Id,
                        (joinResult, qc) => new { Receiving = joinResult.Receiving, RawMaterialsGroup = joinResult.RawMaterialsGroup, QC = qc })
                    .AsEnumerable()
                    .Select(
                        (joinResult, rawMaterial) => new ConsolidatedReport
                        {
                            Id = joinResult.Receiving.Id,
                            TransactDate = joinResult.Receiving.ReceivingDate.ToString("MM-dd-yyyy"),
                            ItemCode = rawMaterials.Where(item => item.ItemCode == joinResult.RawMaterialsGroup.ItemCode)
                            .Select(item => item.ItemCode)
                            .FirstOrDefault(),
                            ItemDescription = rawMaterials.Where(item => item.ItemCode == joinResult.RawMaterialsGroup.ItemCode)
                            .Select(item => item.ItemDescription)
                            .FirstOrDefault(),
                            Category = rawMaterials
                            .Where(item => item.ItemCode == joinResult.RawMaterialsGroup.ItemCode)
                            .Select(item => item.ItemCategoryName)
                            .FirstOrDefault(),
                            UOM = rawMaterials
                            .Where(item => item.ItemCode == joinResult.RawMaterialsGroup.ItemCode)
                            .Select(item => item.UOM_Description)
                            .FirstOrDefault(),
                            Quantity = joinResult.Receiving.ActualGood,
                            UnitPrice = Math.Round(warehouseReceived
                            .Where(wr => wr.PO_Number == joinResult.Receiving.PO_Number && wr.ItemCode == joinResult.Receiving.ItemCode && wr.QcReceivingId == joinResult.QC.Id)
                            .Select(wr => wr.UnitCost)
                            .FirstOrDefault() ?? 0, 2),
                            WarehouseId = warehouseReceived
                                .Where(wr => wr.PO_Number == joinResult.Receiving.PO_Number && wr.ItemCode == joinResult.Receiving.ItemCode && wr.QcReceivingId == joinResult.QC.Id)
                                .Select(wr => wr.Id)
                                .FirstOrDefault(),
                            TransactionType = "Receiving",
                            CompanyCode = "31",
                            CompanyName = "Fresh Options",
                            DepartmentCode = "5001",
                            DepartmentName = "Meatshop Administration",
                            LocationCode = "0",
                            LocationName = "Common",
                            AccountTitle = "Accrued Expense Payable",
                            AccountTitleCode = "211201",
                            Reason = joinResult.Receiving.PO_Number.ToString(),
                            MIRId = "-",
                            Source = joinResult.Receiving.PO_Number,
                            Reference = joinResult.Receiving.Supplier,
                            Encoded = joinResult.Receiving.ReceivedBy,
                            Details = null
                        }).ToList();


            foreach (var moveOrderReport in moveOrderReports)
            {
                consolidatedReports.Add(new ConsolidatedReport
                {
                    Id = moveOrderReport.Id,
                    TransactDate = moveOrderReport.PreparedDate.HasValue
                        ? moveOrderReport.PreparedDate.Value.ToString("MM-dd-yyyy")
                        : null,
                    ItemCode = rawMaterials.Where(item => item.ItemCode == moveOrderReport.ItemCode)
                            .Select(item => item.ItemCode)
                            .FirstOrDefault(),
                    ItemDescription = rawMaterials.Where(item => item.ItemCode == moveOrderReport.ItemCode)
                            .Select(item => item.ItemDescription)
                            .FirstOrDefault(),
                    Category = rawMaterials.Where(item => item.ItemCode == moveOrderReport.ItemCode)
                            .Select(item => item.ItemCategoryName)
                            .FirstOrDefault(),
                    UOM = rawMaterials.Where(item => item.ItemCode == moveOrderReport.ItemCode)
                            .Select(item => item.UOM_Description)
                            .FirstOrDefault(),
                    Quantity = moveOrderReport.QuantityOrdered,
                    WarehouseId = moveOrderReport.WarehouseId,
                    UnitPrice = warehouseReceived.Where(wr => wr.Id == moveOrderReport.WarehouseId)
                        .Select(x => x.UnitCost).FirstOrDefault(),
                    TransactionType = "Move Order",
                    CompanyCode = moveOrderReport.CompanyCode,
                    CompanyName = moveOrderReport.CompanyName,
                    DepartmentCode = moveOrderReport.DepartmentCode,
                    DepartmentName = moveOrderReport.DepartmentName,
                    LocationCode = moveOrderReport.LocationCode,
                    LocationName = moveOrderReport.LocationName,
                    AccountTitleCode = moveOrderReport.AccountTitleCode,
                    AccountTitle = moveOrderReport.AccountTitles,
                    MIRId = moveOrderReport.TransactionId,
                    Source = moveOrderReport.OrderNo,
                    Reference = moveOrderReport.FarmName,
                    Encoded = moveOrderReport.PreparedBy,
                    Reason = moveOrderReport.DeliveryStatus,
                    Details = null,
                    Status = moveOrderReport.PreparedDate != null ? "Transacted" : "Pending",
                    EmployeeName = moveOrderReport.EmployeeName
                });
            }

        if (!string.IsNullOrEmpty(Department) && Department == "Audit")
        {
            foreach (var cancelledOrder in cancelledOrders)
            {
                consolidatedReports.Add(new ConsolidatedReport
                {
                    Id = cancelledOrder.OrderId,
                    TransactDate = cancelledOrder.CancelledDate,
                    ItemCode = rawMaterials.Where(item => item.ItemCode == cancelledOrder.ItemCode)
                            .Select(item => item.ItemCode)
                            .FirstOrDefault(),
                    ItemDescription = rawMaterials.Where(item => item.ItemCode == cancelledOrder.ItemCode)
                            .Select(item => item.ItemDescription)
                            .FirstOrDefault(),
                    Category = rawMaterials.Where(item => item.ItemCode == cancelledOrder.ItemCode)
                            .Select(item => item.ItemCategoryName)
                            .FirstOrDefault(),
                    UOM = rawMaterials.Where(item => item.ItemCode == cancelledOrder.ItemCode)
                            .Select(item => item.UOM_Description)
                            .FirstOrDefault(),
                    Quantity = cancelledOrder.QuantityOrdered,
                    WarehouseId = 0,
                    UnitPrice = 0,
                    TransactionType = "Cancelled Orders",
                    CompanyCode = "-",
                    CompanyName = "-",
                    DepartmentCode = "-",
                    DepartmentName = "-",
                    LocationCode = "-",
                    LocationName = "-",
                    AccountTitleCode = "-",
                    AccountTitle = "-",
                    MIRId = cancelledOrder.MirId,
                    Source = Convert.ToInt32(cancelledOrder.MirId),
                    Reference = cancelledOrder.CustomerName,
                    Encoded = cancelledOrder.CancelledBy,
                    Reason = cancelledOrder.Reason,
                    Details = null,
                    Status = "-"
                });
            };
        }

            var miscellaneousReceipts = await (from receiptInReports in _context.MiscellaneousReceipts
                                               join warehouseRec in _context.WarehouseReceived
                                                   on receiptInReports.Id equals warehouseRec.MiscellaneousReceiptId
                                               join rm in _context.RawMaterials
                                                .Include(category => category.ItemCategory)
                                                .Include(uom => uom.UOM)
                                                   on warehouseRec.ItemCode equals rm.ItemCode
                                               join itemCategory in _context.ItemCategories
                                                   on rm.ItemCategoryId equals itemCategory.Id
                                               where receiptInReports.IsActive
                                                     && receiptInReports.TransactionDate.Date >= fromDate
                                                     && receiptInReports.TransactionDate.Date <= toDate
                                               select new ConsolidatedReport
                                               {
                                                   Id = receiptInReports.Id,
                                                   TransactDate = receiptInReports.TransactionDate.ToString("MM-dd-yyyy"),
                                                   ItemCode = rm.ItemCode,
                                                   ItemDescription = rm.ItemDescription,
                                                   UOM = rm.UOM.UOM_Description,
                                                   Category = rm.ItemCategory.ItemCategoryName,
                                                   Quantity = warehouseRec.ActualGood,
                                                   UnitPrice = warehouseRec.UnitCost,
                                                   WarehouseId = warehouseRec.Id,
                                                   TransactionType = "Miscellaneous Receipt",
                                                   CompanyName = receiptInReports.CompanyName,
                                                   CompanyCode = receiptInReports.CompanyCode,
                                                   DepartmentCode = receiptInReports.DepartmentCode,
                                                   DepartmentName = receiptInReports.DepartmentName,
                                                   AccountTitleCode = receiptInReports.AccountTitleCode,
                                                   AccountTitle = receiptInReports.AccountTitles,
                                                   LocationCode = receiptInReports.LocationCode,
                                                   LocationName = receiptInReports.LocationName,
                                                   Encoded = receiptInReports.PreparedBy,
                                                   Source = receiptInReports.Id,
                                                   Reference = receiptInReports.CompanyName,
                                                   Reason = receiptInReports.Remarks,
                                                   Details = receiptInReports.Details,
                                                   Status = "-",
                                                   MIRId = "-"
                                               }).ToListAsync();

            consolidatedReports.AddRange(miscellaneousReceipts);

            var miscellaneousIssues = _context.MiscellaneousIssues
                .Where(wr => wr.IsTransact == true && wr.IsActive == true)
                .Where(x => x.TransactionDate.Date >= fromDate && x.TransactionDate.Date <= toDate)
                .Join(
                    _context.MiscellaneousIssueDetails.Where(x => x.IsActive == true),
                    issue => issue.Id,
                    details => details.IssuePKey,
                    (issue, details) => new { Issue = issue, Details = details })
                .Join(
                    _context.RawMaterials
                    .Include(uom => uom.UOM),
                    result => result.Details.ItemCode,
                    rawMaterial => rawMaterial.ItemCode,
                    (result, rawMaterial) => new { Result = result, RawMaterial = rawMaterial })
                .Join(
                    _context.ItemCategories,
                    combined => combined.RawMaterial.ItemCategoryId,
                    itemCategory => itemCategory.Id,
                    (combined, itemCategory) => new { Combined = combined, ItemCategory = itemCategory })
                .Select(consolidated => new ConsolidatedReport
                {
                    Id = consolidated.Combined.Result.Issue.Id,
                    TransactDate = consolidated.Combined.Result.Issue.TransactionDate.ToString("MM-dd-yyyy"),
                    ItemCode = consolidated.Combined.RawMaterial.ItemCode,
                    ItemDescription = consolidated.Combined.RawMaterial.ItemDescription,
                    UOM = consolidated.Combined.RawMaterial.UOM.UOM_Description,
                    Category = consolidated.ItemCategory.ItemCategoryName,
                    Quantity = consolidated.Combined.Result.Details.Quantity,
                    WarehouseId = consolidated.Combined.Result.Details.WarehouseId,
                    UnitPrice = consolidated.Combined.Result.Details.UnitCost,
                    TransactionType = "Miscellaneous Issue",
                    CompanyCode = consolidated.Combined.Result.Issue.CompanyCode,
                    CompanyName = consolidated.Combined.Result.Issue.CompanyName,
                    DepartmentCode = consolidated.Combined.Result.Issue.DepartmentCode,
                    DepartmentName = consolidated.Combined.Result.Issue.DepartmentName,
                    LocationCode = consolidated.Combined.Result.Issue.LocationCode,
                    LocationName = consolidated.Combined.Result.Issue.LocationName,
                    AccountTitleCode = consolidated.Combined.Result.Issue.AccountTitleCode,
                    AccountTitle = consolidated.Combined.Result.Issue.AccountTitles,
                    Source = consolidated.Combined.Result.Issue.Id,
                    Reason = consolidated.Combined.Result.Details.Remarks,
                    Reference = consolidated.Combined.Result.Details.Customer,
                    Encoded = consolidated.Combined.Result.Issue.PreparedBy,
                    Details = consolidated.Combined.Result.Issue.Details,
                    Status = "-",
                    MIRId = "-"
                   
                })
                .ToList();
            consolidatedReports.AddRange(miscellaneousIssues);
       

        var result = consolidatedReports.Select(report => new ConsolidatedReport
        {
            Id = report.Id,
            TransactDate = report.TransactDate,
            ItemCode = report.ItemCode,
            ItemDescription = report.ItemDescription,
            Category = report.Category,
            UOM = report.UOM,
            Quantity = report.Quantity,
            WarehouseId = report.WarehouseId,
            TransactionType = report.TransactionType,
            CompanyCode = report.CompanyCode,
            CompanyName = report.CompanyName,
            DepartmentCode = report.DepartmentCode,
            DepartmentName = report.DepartmentName,
            LocationCode = report.LocationCode,
            LocationName = report.LocationName,
            AccountTitleCode = report.AccountTitleCode,
            AccountTitle = report.AccountTitle,
            Amount = (report.Quantity.HasValue && report.UnitPrice.HasValue)
                ? Math.Round((report.Quantity.Value * report.UnitPrice.Value), 2)
                : 0,
            UnitPrice = report.UnitPrice.HasValue ? Math.Round((decimal)report.UnitPrice, 2) : 0,
            Source = report.Source,
            Reference = report.Reference,
            Reason = report.Reason,
            Encoded = report.Encoded,
            Details = report.Details,
            Status = report.Status,
            MIRId = report.MIRId
        });

        return result.ToList();

    }
    #region Conso Pagination
    //public async Task<PagedList<ConsolidatedReport>> ConsolidatedReportPagination(string dateFrom, string dateTo, UserParams userParams)
    //{
    //    var fromDate = DateTime.Parse(dateFrom).Date;
    //    var toDate = DateTime.Parse(dateTo).Date;

    //    var warehouseReceived = await _context.WarehouseReceived
    //        .Select(warehouseReceived => new
    //        {
    //            warehouseReceived.PO_Number,
    //            warehouseReceived.Id,
    //            warehouseReceived.ItemCode,
    //            warehouseReceived.Uom,
    //            warehouseReceived.MiscellaneousReceiptId,
    //            warehouseReceived.UnitCost
    //        })
    //        .ToListAsync();

    //    var consolidatedReportsQuery =
    //        from wr in _context.WarehouseReceived
    //        where wr.IsActive && wr.IsWarehouseReceive
    //        join mo in _context.MoveOrders on wr.Id equals mo.WarehouseId into moveOrders
    //        from mo in moveOrders.DefaultIfEmpty()
    //        let QuantityOrdered = mo != null ? mo.QuantityOrdered : 0
    //        let CostByWarehouse = wr.UnitCost * (wr.ActualGood - QuantityOrdered)
    //        select new
    //        {
    //            wr,
    //            QuantityOrdered,
    //            CostByWarehouse
    //        };

    //    // Handle Receiving transaction type
    //    var receivingReportsQuery = await _context.QC_Receiving
    //        .Where(wr => wr.IsWareHouseReceive == true)
    //        .Where(wr => wr.QC_ReceiveDate.Date >= fromDate && wr.QC_ReceiveDate.Date <= toDate)
    //        .Join(
    //            _context.RawMaterials
    //                .Include(rm => rm.UOM)
    //                .Include(rm => rm.ItemCategory),
    //            receiving => receiving.ItemCode,
    //            rawMaterial => rawMaterial.ItemCode,
    //            (receiving, rawMaterialsGroup) =>
    //                new { Receiving = receiving, RawMaterialsGroup = rawMaterialsGroup })
    //        .Join(
    //            _context.POSummary,
    //            receiving => receiving.Receiving.PO_Summary_Id,
    //            posummary => posummary.Id,
    //            (receiving, posummary) => new
    //            {
    //                Receiving = receiving,
    //                PoSummary = posummary
    //            })
    //        .ToListAsync(); // Materialize the join results here

    //    //// Perform the join in memory
    //    //var consolidatedReports = receivingReportsQuery
    //    //    .Select(joinResult => new ConsolidatedReport
    //    //    {
    //    //        Id = joinResult.Receiving.Receiving.Id,
    //    //        TransactDate = joinResult.Receiving.Receiving.QC_ReceiveDate.ToString("MM-dd-yyyy"),
    //    //        ItemCode = joinResult.Receiving.Receiving.ItemCode,
    //    //        ItemDescription = joinResult.Receiving.RawMaterialsGroup.ItemDescription,
    //    //        Category = joinResult.Receiving.RawMaterialsGroup.ItemCategory.ItemCategoryName,
    //    //        UOM = joinResult.Receiving.RawMaterialsGroup.UOM.UOM_Description,
    //    //        Quantity = joinResult.Receiving.Receiving.Actual_Delivered,
    //    //        UnitPrice = warehouseReceived
    //    //            .Where(wr => wr.PO_Number == joinResult.PoSummary.PO_Number)
    //    //            .Select(wr => wr.UnitCost)
    //    //            .FirstOrDefault(),
    //    //        WarehouseId = warehouseReceived
    //    //            .Where(wr => wr.PO_Number == joinResult.PoSummary.PO_Number)
    //    //            .Select(wr => wr.Id)
    //    //            .FirstOrDefault(),
    //    //        TransactionType = "Receiving",
    //    //        CompanyCode = "31",
    //    //        CompanyName = "Fresh Options",
    //    //        DepartmentCode = "5001",
    //    //        DepartmentName = "Meatshop Administration",
    //    //        LocationCode = "0",
    //    //        LocationName = "Common",
    //    //        AccountTitle = "Accrued Expense Payable",
    //    //        AccountTitleCode = "211201",
    //    //        Reason = joinResult.PoSummary.PO_Number.ToString(),
    //    //        Source = joinResult.PoSummary.PO_Number,
    //    //        Reference = joinResult.PoSummary.VendorName,
    //    //        Encoded = joinResult.Receiving.Receiving.QcBy,
    //    //        Details = null
    //    //    });

    //    // Handle Move Order transaction type
    //    var moveOrderReportsQuery =
    //        from moveorder in _context.MoveOrders
    //        where moveorder.ApprovedDate >= fromDate && moveorder.ApprovedDate <= toDate && moveorder.IsActive
    //        select new ConsolidatedReport
    //        {
    //            Id = moveorder.Id,
    //            TransactDate = moveorder.PreparedDate.HasValue
    //                ? moveorder.PreparedDate.Value.ToString("MM-dd-yyyy")
    //                : null,
    //            ItemCode = moveorder.ItemCode,
    //            ItemDescription = moveorder.ItemDescription,
    //            Category = moveorder.Category,
    //            UOM = moveorder.Uom,
    //            Quantity = moveorder.QuantityOrdered,
    //            WarehouseId = moveorder.WarehouseId,
    //            UnitPrice = warehouseReceived.Where(wr => wr.Id == moveorder.WarehouseId)
    //                .Select(x => x.UnitCost).FirstOrDefault(),
    //            TransactionType = "Move Order",
    //            CompanyCode = moveorder.CompanyCode,
    //            CompanyName = moveorder.CompanyName,
    //            DepartmentCode = moveorder.DepartmentCode,
    //            DepartmentName = moveorder.DepartmentName,
    //            LocationCode = moveorder.LocationCode,
    //            LocationName = moveorder.LocationName,
    //            AccountTitleCode = moveorder.AccountTitleCode,
    //            AccountTitle = moveorder.AccountTitles,
    //            Source = moveorder.OrderNo,
    //            Reference = moveorder.FarmName,
    //            Encoded = moveorder.PreparedBy,
    //            Reason = moveorder.DeliveryStatus,
    //            Details = null,
    //            Status = moveorder.PreparedDate != null ? "Transacted" : "Pending"
    //        };

    //    // Handle Miscellaneous Receipt transaction type
    //    var miscellaneousReceiptsQuery =
    //        from receiptInReports in _context.MiscellaneousReceipts
    //        join warehouseRec in _context.WarehouseReceived
    //            on receiptInReports.Id equals warehouseRec.MiscellaneousReceiptId
    //        join rm in _context.RawMaterials
    //            on warehouseRec.ItemCode equals rm.ItemCode
    //        join itemCategory in _context.ItemCategories
    //            on rm.ItemCategoryId equals itemCategory.Id
    //        where receiptInReports.IsActive
    //              && receiptInReports.TransactionDate >= fromDate
    //              && receiptInReports.TransactionDate <= toDate
    //              && warehouseRec.ReceivingDate >= fromDate
    //              && warehouseRec.ReceivingDate <= toDate
    //        select new ConsolidatedReport
    //        {
    //            Id = receiptInReports.Id,
    //            TransactDate = receiptInReports.TransactionDate.ToString("MM-dd-yyyy"),
    //            ItemCode = warehouseRec.ItemCode,
    //            ItemDescription = warehouseRec.ItemDescription,
    //            UOM = warehouseRec.Uom,
    //            Category = itemCategory.ItemCategoryName,
    //            Quantity = warehouseRec.ActualGood,
    //            UnitPrice = warehouseRec.UnitCost,
    //            WarehouseId = warehouseRec.Id,
    //            TransactionType = "Miscellaneous Receipt",
    //            CompanyName = receiptInReports.CompanyName,
    //            CompanyCode = receiptInReports.CompanyCode,
    //            DepartmentCode = receiptInReports.DepartmentCode,
    //            DepartmentName = receiptInReports.DepartmentName,
    //            AccountTitleCode = receiptInReports.AccountTitleCode,
    //            AccountTitle = receiptInReports.AccountTitles,
    //            LocationCode = receiptInReports.LocationCode,
    //            LocationName = receiptInReports.LocationName,
    //            Encoded = receiptInReports.PreparedBy,
    //            Source = receiptInReports.Id,
    //            Reference = receiptInReports.CompanyName,
    //            Reason = receiptInReports.Remarks,
    //            Details = receiptInReports.Details,
    //            Status = "-"
    //        };

    //    // Handle Miscellaneous Issue transaction type
    //    var miscellaneousIssuesQuery =
    //        _context.MiscellaneousIssues
    //        .Where(wr => wr.IsTransact == true)
    //        .Where(x => x.TransactionDate.Date >= fromDate && x.TransactionDate.Date <= toDate)
    //        .Join(
    //            _context.MiscellaneousIssueDetails,
    //            issue => issue.Id,
    //            details => details.IssuePKey,
    //            (issue, details) => new { Issue = issue, Details = details })
    //        .Join(
    //            _context.RawMaterials,
    //            result => result.Details.ItemCode,
    //            rawMaterial => rawMaterial.ItemCode,
    //            (result, rawMaterial) => new { Result = result, RawMaterial = rawMaterial })
    //        .Join(
    //            _context.ItemCategories,
    //            combined => combined.RawMaterial.ItemCategoryId,
    //            itemCategory => itemCategory.Id,
    //            (combined, itemCategory) => new { Combined = combined, ItemCategory = itemCategory })
    //        .Select(consolidated => new ConsolidatedReport
    //        {
    //            Id = consolidated.Combined.Result.Issue.Id,
    //            TransactDate = consolidated.Combined.Result.Issue.TransactionDate.ToString("MM-dd-yyyy"),
    //            ItemCode = consolidated.Combined.Result.Details.ItemCode,
    //            ItemDescription = consolidated.Combined.Result.Details.ItemDescription,
    //            UOM = consolidated.Combined.Result.Details.Uom,
    //            Category = consolidated.ItemCategory.ItemCategoryName,
    //            Quantity = consolidated.Combined.Result.Details.Quantity,
    //            WarehouseId = consolidated.Combined.Result.Details.WarehouseId,
    //            UnitPrice = consolidated.Combined.Result.Details.UnitCost,
    //            TransactionType = "Miscellaneous Issue",
    //            CompanyCode = consolidated.Combined.Result.Issue.CompanyCode,
    //            CompanyName = consolidated.Combined.Result.Issue.CompanyName,
    //            DepartmentCode = consolidated.Combined.Result.Issue.DepartmentCode,
    //            DepartmentName = consolidated.Combined.Result.Issue.DepartmentName,
    //            LocationCode = consolidated.Combined.Result.Issue.LocationCode,
    //            LocationName = consolidated.Combined.Result.Issue.LocationName,
    //            AccountTitleCode = consolidated.Combined.Result.Issue.AccountTitleCode,
    //            AccountTitle = consolidated.Combined.Result.Issue.AccountTitles,
    //            Source = consolidated.Combined.Result.Issue.Id,
    //            Reason = consolidated.Combined.Result.Details.Remarks,
    //            Reference = consolidated.Combined.Result.Details.Customer,
    //            Encoded = consolidated.Combined.Result.Issue.PreparedBy,
    //            Details = consolidated.Combined.Result.Issue.Details,
    //            Status = "-"
    //        });

    //    // Combine all transaction types into a single queryable result
    //    var combinedQuery = moveOrderReportsQuery
    //        .Concat(miscellaneousReceiptsQuery)
    //        .Concat(miscellaneousIssuesQuery);

    //    // Convert the list to a paged list and return
    //    return await PagedList<ConsolidatedReport>.CreateAsync(combinedQuery, userParams.PageNumber, userParams.PageSize);
    //}
    #endregion

    public async Task<IReadOnlyList<MoveOrderReport>> ApprovedMoveOrderReport(string dateFrom, string dateTo)
    {
        DateTime toDate = DateTime.Parse(dateTo);
        DateTime fromDate = DateTime.Parse(dateFrom);

        var orders = _context.MoveOrders
            .Where(x => x.ApproveDateTempo >= fromDate && x.ApproveDateTempo <= toDate)
            .Where(X => X.IsActive == true)
            .GroupBy(x => new
            {
                x.Id,
                x.OrderNo,
                x.ItemCode,
                x.ItemDescription,
                x.FarmName,
                x.FarmCode,
                x.FarmType,
                x.PreparedDate,
                x.IsApprove,
                x.DeliveryStatus,
                x.IsPrepared,
                x.IsReject,
                x.ApproveDateTempo,
                x.IsPrint,
                x.IsTransact,
                x.PreparedBy
            })
            .Where(x => x.Key.IsApprove == true)
            .Where(x => x.Key.DeliveryStatus != null)
            .Where(x => x.Key.IsReject != true)
            .Select(x => new MoveOrderReport
            {
                MoveOrderId = x.Key.OrderNo,
                CustomerName = x.Key.FarmName,
                CustomerCode = x.Key.FarmCode,
                ItemCode = x.Key.ItemCode,
                ItemDescription = x.Key.ItemDescription,
                TransactionType = "Move Order",
                Category = x.Key.FarmType,
                Quantity = x.Sum(y => y.QuantityOrdered),
                PreparedDate = x.Key.PreparedDate.ToString(),
                DeliveryStatus = x.Key.DeliveryStatus,
                TransactedBy = x.Key.PreparedBy,
            });

        return await orders.ToListAsync();
    }

    public async Task<PagedList<MoveOrderReport>> ApprovedMoveOrderReportPagination(string dateFrom, string dateTo, UserParams userParams)
    {
        DateTime toDate = DateTime.Parse(dateTo);
        DateTime fromDate = DateTime.Parse(dateFrom);

        var orders = _context.MoveOrders
            .Where(x => x.ApproveDateTempo >= fromDate && x.ApproveDateTempo <= toDate)
            .Where(X => X.IsActive == true)
            .GroupBy(x => new
            {
                x.Id,
                x.OrderNo,
                x.ItemCode,
                x.ItemDescription,
                x.FarmName,
                x.FarmCode,
                x.FarmType,
                x.PreparedDate,
                x.IsApprove,
                x.DeliveryStatus,
                x.IsPrepared,
                x.IsReject,
                x.ApproveDateTempo,
                x.IsPrint,
                x.IsTransact,
                x.PreparedBy
            })
            .Where(x => x.Key.IsApprove == true)
            .Where(x => x.Key.DeliveryStatus != null)
            .Where(x => x.Key.IsReject != true)
            .Select(x => new MoveOrderReport
            {
                MoveOrderId = x.Key.OrderNo,
                CustomerName = x.Key.FarmName,
                CustomerCode = x.Key.FarmCode,
                ItemCode = x.Key.ItemCode,
                ItemDescription = x.Key.ItemDescription,
                TransactionType = "Move Order",
                Category = x.Key.FarmType,
                Quantity = x.Sum(y => y.QuantityOrdered),
                PreparedDate = x.Key.PreparedDate.ToString(),
                DeliveryStatus = x.Key.DeliveryStatus,
                TransactedBy = x.Key.PreparedBy,
            });

        return await PagedList<MoveOrderReport>.CreateAsync(orders, userParams.PageNumber, userParams.PageSize);
    }

    public async Task<IReadOnlyList<OrderVsServeReportsDTO>> OrderVsServeReports(string dateFrom, string dateTo)
    {
        var fromDate = DateTime.Parse(dateFrom);
        var toDate = DateTime.Parse(dateTo);

        var orders = await _context.Orders
            .Where(order => order.IsActive &&
                            !order.IsReject != true &&
                            order.PreparedDate != null &&
                            order.PreparedDate >= fromDate &&
                            order.PreparedDate <= toDate)
            .Select(order => new
            {
                order.Id,
                order.FarmCode,
                order.FarmName,
                order.ItemCode,
                order.ItemDescription,
                order.Uom,
                order.Category,
                order.QuantityOrdered,
                order.PreparedDate
            }).ToListAsync();

        var serveOrders = await _context.MoveOrders
            .Where(moveOrder => moveOrder.IsTransact &&
                                moveOrder.IsActive &&
                                moveOrder.IsPrepared &&
                                moveOrder.IsRejectForPreparation != true &&
                                moveOrder.ApprovedDate >= fromDate &&
                                moveOrder.ApprovedDate <= toDate)
            .GroupBy(x => x.OrderNoPKey)
            .Select(group => new
            {
                OrderNoPKey = group.Key,
                TotalQuantityOrdered = group.Sum(x => x.QuantityOrdered)
            })
            .ToListAsync();

        var reportList = serveOrders.Join(orders,
            serveOrder => serveOrder.OrderNoPKey,
            order => order.Id,
            (serveOrder, order) => new OrderVsServeReportsDTO
            {
                OrderNo = order.Id,
                CustomerCode = order.FarmCode,
                CustomerName = order.FarmName,
                ItemCode = order.ItemCode,
                ItemDescription = order.ItemDescription,
                Uom = order.Uom,
                Category = order.Category,
                QuantityOrdered = order.QuantityOrdered,
                QuantityServed = serveOrder.TotalQuantityOrdered,
                Variance = order.QuantityOrdered - serveOrder.TotalQuantityOrdered,
                Percentage = Math.Round(serveOrder.TotalQuantityOrdered / order.QuantityOrdered * 100, 2)
            }).ToList();

        return reportList;
    }

    public async Task<PagedList<OrderVsServeReportsDTO>> OrderVsServeReportsPagination(string dateFrom, string dateTo,
        UserParams userParams)
    {
        var fromDate = DateTime.Parse(dateFrom);
        var toDate = DateTime.Parse(dateTo);

        var orders = _context.Orders
            .Where(order => order.IsActive &&
                            !order.IsReject != true &&
                            order.PreparedDate != null &&
                            order.PreparedDate >= fromDate &&
                            order.PreparedDate <= toDate)
            .Select(order => new OrderVsServeReportsDTO
            {
                OrderNo = order.Id,
                CustomerCode = order.FarmCode,
                CustomerName = order.FarmName,
                ItemCode = order.ItemCode,
                ItemDescription = order.ItemDescription,
                Uom = order.Uom,
                Category = order.Category,
                QuantityOrdered = order.QuantityOrdered,
                QuantityServed = 0, // Initialize with 0, since we'll update this later,
            });

        var serveOrders = _context.MoveOrders
            .Where(moveOrder => moveOrder.IsTransact &&
                                moveOrder.IsActive &&
                                moveOrder.IsPrepared &&
                                moveOrder.IsRejectForPreparation != true &&
                                moveOrder.ApprovedDate >= fromDate &&
                                moveOrder.ApprovedDate <= toDate)
            .GroupBy(x => x.OrderNoPKey)
            .Select(group => new
            {
                OrderNoPKey = group.Key,
                TotalQuantityOrdered = group.Sum(x => x.QuantityOrdered)
            });

        var reportList = from serveOrder in serveOrders
            join order in orders on serveOrder.OrderNoPKey equals order.OrderNo
            select new OrderVsServeReportsDTO
            {
                OrderNo = order.OrderNo,
                CustomerCode = order.CustomerCode,
                CustomerName = order.CustomerName,
                ItemCode = order.ItemCode,
                ItemDescription = order.ItemDescription,
                Uom = order.Uom,
                Category = order.Category,
                QuantityOrdered = order.QuantityOrdered,
                QuantityServed = serveOrder.TotalQuantityOrdered,
                Variance = order.QuantityServed - serveOrder.TotalQuantityOrdered,
                Percentage = Math.Round((order.QuantityServed / (decimal)order.QuantityOrdered) * 100, 2)
            };

        return await PagedList<OrderVsServeReportsDTO>.CreateAsync(reportList, userParams.PageNumber,
            userParams.PageSize);
    }

    public async Task<IReadOnlyList<ItemswithBBDDTO>> ItemswithBBDReport()
    {
        var EndDate = DateTime.Now;
        var StartDate = EndDate.AddDays(-30);

        var getWarehouseIn = await _context.WarehouseReceived
            .Where(x => x.IsActive == true)
            .GroupBy(x => new { 
                x.ItemCode, 
                x.Id, 
                x.ActualGood, 
                x.ItemDescription, 
                x.Uom, 
                x.Expiration })
            .Select(x => new WarehouseInventoryWIthBbd
            {
                WarehouseId = x.Key.Id,
                ItemCode = x.Key.ItemCode,
                Uom = x.Key.Uom,
                ActualGood = x.Key.ActualGood,
                ItemDescription = x.Key.ItemDescription,
                ExpirationDate = x.Key.Expiration
            })
            .ToListAsync();

        var getMoveOrderOut = await _context.MoveOrders
            .Where(x => x.IsActive == true && x.IsPrepared == true)
            .GroupBy(x => new { x.ItemCode, x.WarehouseId})
            .Select(x => new MoveOrderInventory
            {
                WarehouseId = x.Key.WarehouseId,
                ItemCode = x.Key.ItemCode,
                QuantityOrdered = x.Sum(x => x.QuantityOrdered)
            })
            .ToListAsync();

        var getReceiptIn = await _context.WarehouseReceived
            .Where(x => x.IsActive == true && x.TransactionType == "MiscellaneousReceipt")
            .GroupBy(x => new { x.ItemCode, x.Id })
            .Select(x => new ReceiptInventoryWithBBDDTO
            {
                WarehouseId = x.Key.Id,
                ItemCode = x.Key.ItemCode,
                Quantity = x.Sum(x => x.ActualGood)
            })
            .ToListAsync();

        var getIssueOut = await _context.MiscellaneousIssueDetails
            .Where(x => x.IsActive == true)
            .GroupBy(x => new { x.ItemCode, x.WarehouseId })
            .Select(x => new IssueInventoryWithBBDDTO
            {
                WarehouseId = x.Key.WarehouseId,
                ItemCode = x.Key.ItemCode,
                Quantity = x.Sum(x => x.Quantity)
            })
            .ToListAsync();

        var items = (from warehouse in getWarehouseIn
                     join moveOrder in getMoveOrderOut on new { warehouse.ItemCode, warehouse.WarehouseId } equals new { moveOrder.ItemCode, moveOrder.WarehouseId } into moveOrders
                     from moveOrder in moveOrders.DefaultIfEmpty()
                     join receipt in getReceiptIn on new { warehouse.ItemCode, warehouse.WarehouseId } equals new { receipt.ItemCode, receipt.WarehouseId } into receipts
                     from receipt in receipts.DefaultIfEmpty()
                     join issue in getIssueOut on new { warehouse.ItemCode, warehouse.WarehouseId } equals new { issue.ItemCode, issue.WarehouseId } into issues
                     from issue in issues.DefaultIfEmpty()
                     group new
                     {
                         warehouse,
                         moveOrder,
                         receipt,
                         issue
                     } by new
                     {
                         warehouse.WarehouseId,
                         warehouse.ItemCode,
                         warehouse.ItemDescription,
                         warehouse.Uom,
                         warehouse.ExpirationDate,
                         warehouse.ActualGood,
                         ReceiptQuantity = receipt?.Quantity,
                         IssueQuantity = issue?.Quantity,
                         MoveOrderQuantity = moveOrder?.QuantityOrdered
                     } into total
                     select new ItemswithBBDDTO
                     {
                         WarehouseId = total.Key.WarehouseId,
                         ItemCode = total.Key.ItemCode,
                         ItemDescription = total.Key.ItemDescription,
                         UOM = total.Key.Uom,
                         Receipt = total.Key.ReceiptQuantity,
                         Issue = total.Key.IssueQuantity,
                         MoveOrder = total.Key.MoveOrderQuantity,
                         Warehouse = total.Key.ActualGood,
                         SOH = (total.Sum(x => x.warehouse?.ActualGood ?? 0)) -
                               ((total.Sum(x => x.moveOrder?.QuantityOrdered ?? 0)) +
                               (total.Sum(x => x.issue?.Quantity ?? 0))),
                         BBD = total.Key.ExpirationDate?.ToString("MM-dd-yyyy") ?? "-"
                     })
                     .Where(items => items.SOH != 0)
                     .ToList();

        return items;
    }


    public async Task<PagedList<ItemswithBBDDTO>> ItemswithBBDDTOsPagination(UserParams userParams)
    {

        var EndDate = DateTime.Now;
        var StartDate = EndDate.AddDays(-30);

        var getWarehouseIn = _context.WarehouseReceived
            .Where(x => x.IsActive == true)
            .GroupBy(x => new {
                x.ItemCode,
                x.Id,
                x.ActualGood,
                x.ItemDescription,
                x.Uom,
                x.Expiration
            })
            .Select(x => new WarehouseInventoryWIthBbd
            {
                WarehouseId = x.Key.Id,
                ItemCode = x.Key.ItemCode,
                Uom = x.Key.Uom,
                ActualGood = x.Key.ActualGood,
                ItemDescription = x.Key.ItemDescription,
                ExpirationDate = x.Key.Expiration
            });

        var getMoveOrderOut = _context.MoveOrders
            .Where(x => x.IsActive == true && x.IsPrepared == true)
            .GroupBy(x => new { x.ItemCode, x.WarehouseId })
            .Select(x => new MoveOrderInventory
            {
                WarehouseId = x.Key.WarehouseId,
                ItemCode = x.Key.ItemCode,
                QuantityOrdered = x.Sum(x => x.QuantityOrdered)
            });

        var getReceiptIn = _context.WarehouseReceived
            .Where(x => x.IsActive == true && x.TransactionType == "MiscellaneousReceipt")
            .GroupBy(x => new { x.ItemCode, x.Id })
            .Select(x => new ReceiptInventoryWithBBDDTO
            {
                WarehouseId = x.Key.Id,
                ItemCode = x.Key.ItemCode,
                Quantity = x.Sum(x => x.ActualGood)
            });

        var getIssueOut = _context.MiscellaneousIssueDetails
            .Where(x => x.IsActive == true)
            .GroupBy(x => new { x.ItemCode, x.WarehouseId })
            .Select(x => new IssueInventoryWithBBDDTO
            {
                WarehouseId = x.Key.WarehouseId,
                ItemCode = x.Key.ItemCode,
                Quantity = x.Sum(x => x.Quantity)
            });

        var items = (from warehouse in getWarehouseIn
                     join moveOrder in getMoveOrderOut on new { warehouse.ItemCode, warehouse.WarehouseId } equals new { moveOrder.ItemCode, moveOrder.WarehouseId } into moveOrders
                     from moveOrder in moveOrders.DefaultIfEmpty()
                     join receipt in getReceiptIn on new { warehouse.ItemCode, warehouse.WarehouseId } equals new { receipt.ItemCode, receipt.WarehouseId } into receipts
                     from receipt in receipts.DefaultIfEmpty()
                     join issue in getIssueOut on new { warehouse.ItemCode, warehouse.WarehouseId } equals new { issue.ItemCode, issue.WarehouseId } into issues
                     from issue in issues.DefaultIfEmpty()
                     group new
                     {
                         warehouse,
                         moveOrder,
                         receipt,
                         issue
                     } by new
                     {
                         warehouse.WarehouseId,
                         warehouse.ItemCode,
                         warehouse.ItemDescription,
                         warehouse.Uom,
                         warehouse.ExpirationDate,
                         warehouse.ActualGood,
                         ReceiptQuantity = receipt.Quantity,
                         IssueQuantity = issue.Quantity,
                         MoveOrderQuantity = moveOrder.QuantityOrdered
                     } into total
                     select new ItemswithBBDDTO
                     {
                         WarehouseId = total.Key.WarehouseId,
                         ItemCode = total.Key.ItemCode,
                         ItemDescription = total.Key.ItemDescription,
                         UOM = total.Key.Uom,
                         Receipt = total.Key.ReceiptQuantity,
                         Issue = total.Key.IssueQuantity,
                         MoveOrder = total.Key.MoveOrderQuantity,
                         Warehouse = total.Key.ActualGood,
                         SOH = (total.Sum(x => x.warehouse.ActualGood != 0 ? x.warehouse.ActualGood : 0)) -
                               ((total.Sum(x => x.moveOrder.QuantityOrdered != 0 ? x.moveOrder.QuantityOrdered : 0)) +
                               (total.Sum(x => x.issue.Quantity != 0 ? x.issue.Quantity : 0))),
                         BBD = total.Key.ExpirationDate.Value.ToString("MM-dd-yyyy") ?? "-"
                     })
                     .Where(items => items.SOH != 0);

        return await PagedList<ItemswithBBDDTO>.CreateAsync(items, userParams.PageNumber, userParams.PageSize);
    }
}