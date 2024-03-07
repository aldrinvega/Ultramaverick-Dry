using System;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace ELIXIR.DATA.DTOs.ORDERING_DTOs
{
    public class TotalListOfPreparedDateDTO
    {
        public int Id { get; set; }
        public string FarmName { get; set; }
        public string FarmCode { get; set; }
        public string PreparedDate { get; set; }
        public bool IsMove { get; set; }
        public bool? IsReject { get; set; }
        public string Remarks { get; set; }
        public int? QuantityOrder { get; set; }
        public string LocationName { get; set; }
        public string CompanyName { get; set; }
        public string CompanyCode { get; set; }
        public string DepartmentName { get; set; }
    }
}