using ELIXIR.DATA.DATA_ACCESS_LAYER.MODELS.QC_CHECKLIST;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ELIXIR.DATA.DTOs.RECEIVING_DTOs
{
    public class ForViewingofChecklistResult
    {
        public int PO_Summary_Id {
            get; 
            set; 
        }
        
        public DateTime? Manufacturing_Date {
            get;
            set;
        }
        
        public decimal Expected_Delivery {
            get; 
            set;
        }
        
        public DateTime? Expiry_Date {
            get; 
            set; 
        }
        
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

        public decimal TotalReject {
            get;
            set;
        }
        public bool IsActive {
            get; 
            set; 
        }
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
        public DateTime? ExpiryDateOfApprove { 
            get; 
            set;
        }
        
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
        public List<ChecklistStringDTO> Checklists { get; set; }
    }
}