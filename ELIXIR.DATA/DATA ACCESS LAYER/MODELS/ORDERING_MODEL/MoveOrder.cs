﻿using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace ELIXIR.DATA.DATA_ACCESS_LAYER.MODELS.ORDERING_MODEL
{
    public class MoveOrder : BaseEntity
    {
        public int OrderNo { get; set; }
        public string FarmType { get; set; }
        public string FarmCode { get; set; }
        public string FarmName { get; set; }
        public string ItemCode { get; set; }
        public string ItemDescription { get; set; }
        public string Uom { get; set; }
        [Column(TypeName = "decimal(18,2)")] public decimal QuantityOrdered { get; set; }
        public string Category { get; set; }
        [Column(TypeName = "Date")] public DateTime OrderDate { get; set; }
        [Column(TypeName = "Date")] public DateTime DateNeeded { get; set; }
        public int WarehouseId { get; set; }
        public bool IsActive { get; set; }
        public bool? IsApprove { get; set; }
        public DateTime? ApprovedDate { get; set; }
        [Column(TypeName = "Date")] public DateTime? ApproveDateTempo { get; set; }
        public bool IsPrepared { get; set; }
        public string PreparedBy { get; set; }
        public DateTime? PreparedDate { get; set; }

        public bool? IsCancel { get; set; }

        public string CancelBy { get; set; }
        public DateTime? CancelledDate { get; set; }
        public int OrderNoPKey { get; set; }
        public DateTime? ExpirationDate { get; set; }
        public string PlateNumber { get; set; }
        public string DeliveryStatus { get; set; }
        public bool? IsReject { get; set; } = null;
        public string RejectBy { get; set; }
        public DateTime? RejectedDate { get; set; }
        [Column(TypeName = "Date")] public DateTime? RejectedDateTempo { get; set; }
        public string Remarks { get; set; }
        public bool IsTransact { get; set; }
        public bool? IsPrint { get; set; }
        public bool? IsApproveReject { get; set; }
        public bool? IsRejectForPreparation { get; set; }
        public string AccountTitleCode { get; set; }
        public string AccountTitles { get; set; }
        public string LocationCode { get; set; }
        public string LocationName { get; set; }
        public string DepartmentName { get; set; }
        public string DepartmentCode { get; set; }
        public string CompanyName { get; set; }
        public string CompanyCode { get; set; }
        public string AddedBy { get; set; }
        public string CheckedBy { get; set; }
        public DateTime? TimeStamp { get; set; } = DateTime.Now;
        public int? AdvancesToEmployeesId { get; set; }
        public virtual AdvancesToEmployees AdvancesToEmployees { get; set; }
    }
}