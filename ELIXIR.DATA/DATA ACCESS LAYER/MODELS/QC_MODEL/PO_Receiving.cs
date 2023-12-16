using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ELIXIR.DATA.DATA_ACCESS_LAYER.MODELS.QC_CHECKLIST;

namespace ELIXIR.DATA.DATA_ACCESS_LAYER.MODELS.QC_MODEL
{
    public class PO_Receiving : BaseEntity
    {

        public int PO_Summary_Id {
            get; 
            set; 
        }

        [Column(TypeName = "Date")]
        public DateTime? Manufacturing_Date {
            get;
            set;
        }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Expected_Delivery {
            get; 
            set;
        }

        [Column(TypeName = "Date")]
        public DateTime? Expiry_Date {
            get; 
            set; 
        }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Actual_Delivered {
            get; 
            set;
        }
        public string ItemCode {
            get;
            set; 
        }
        public string Batch_No
        {
            get;
            set;
        }

        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalReject {
            get;
            set;
        }
        public bool IsActive {
            get; 
            set; 
        }

        [Column(TypeName = "Date")]
        public DateTime? CancelDate {
            get;
            set; 
        }
        public string CancelBy {
            get; 
            set; 
        }
        public string Reason { 
            get;
            set; 
        }
        
        public bool? ExpiryIsApprove {
            get; 
            set;
        }

        public bool? IsNearlyExpire { 
            get; 
            set;
        }

        public string ExpiryApproveBy {
            get;
            set;
        }

        [Column(TypeName = "Date")]
        public DateTime? ExpiryDateOfApprove { 
            get; 
            set;
        }

        [Column(TypeName = "Date")]
        public DateTime QC_ReceiveDate {
            get; 
            set; 
        }

        public bool ConfirmRejectByQc { 
            get;
            set;
        }
        public bool? IsWareHouseReceive {
            get; 
            set;
        }
        public string CancelRemarks {
            get;
            set; 
        }
        public string QcBy {
            get; 
            set;
        }
        
        public string MonitoredBy { get; set; }

        public string ProductType
        {
            get;
            set;
        }
    }
}
