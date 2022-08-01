using ELIXIR.DATA.DTOs.REPORT_DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELIXIR.DATA.CORE.INTERFACES.REPORT_INTERFACE
{
    public interface IReportRepository
    {

        Task<IReadOnlyList<QCReport>> GetAllRawMaterialsForWarehouseReceiving(string DateFrom, string DateTo);


    }
}
