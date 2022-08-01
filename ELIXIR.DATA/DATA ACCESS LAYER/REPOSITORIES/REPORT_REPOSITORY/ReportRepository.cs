using ELIXIR.DATA.CORE.INTERFACES.REPORT_INTERFACE;
using ELIXIR.DATA.DATA_ACCESS_LAYER.STORE_CONTEXT;
using ELIXIR.DATA.DTOs.REPORT_DTOs;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELIXIR.DATA.DATA_ACCESS_LAYER.REPOSITORIES.REPORT_REPOSITORY
{
    public class ReportRepository : IReportRepository
    {
        private readonly StoreContext _context;

        public ReportRepository(StoreContext context)
        {
            _context = context;


        }

        public async Task<IReadOnlyList<QCReport>> GetAllRawMaterialsForWarehouseReceiving(string DateFrom, string DateTo)
        {
            var report = (from receiving in _context.QC_Receiving
                          join posummary in _context.POSummary
                          on receiving.PO_Summary_Id equals posummary.Id into leftJ
                          from posummary in leftJ.DefaultIfEmpty()

                          select new QCReport
                          {

                              Id = receiving.Id,
                              QcDate = receiving.QC_ReceiveDate.ToString(),
                              PONumber = posummary.PO_Number,
                              ItemCode = posummary.ItemCode, 
                              ItemDescription = posummary.ItemDescription, 
                              Uom = posummary.UOM,
                            //  Category = posummary.
                              Quantity = receiving.Actual_Delivered,
                              ManufacturingDate = receiving.Manufacturing_Date.ToString(),
                              ExpirationDate = receiving.Expiry_Date.ToString(),
                              TotalReject = receiving.TotalReject, 
                              SupplierName = posummary.VendorName,
                              Price = posummary.UnitPrice, 
                              QcBy = receiving.QcBy

                          });


            return await report.ToListAsync();

        }
    }
}
