using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using ELIXIR.DATA.DATA_ACCESS_LAYER.MODELS.ORDERING_MODEL;

namespace ELIXIR.DATA.DATA_ACCESS_LAYER.MODELS.SETUP_MODEL
{
    public class Customer : BaseEntity
    {
        public string CustomerCode
        {
            get; set;
        }
        public string CustomerName
        {
            get; set;
        }
        public FarmType FarmType
        {
            get; set;
        }
        public int FarmTypeId
        {
            get; set;
        }
        public string CompanyName
        {
            get; set;
        }
        public string CompanyCode
        {
            get; set;
        }
        public string MobileNumber
        {
            get; set;
        }
        public string LeadMan
        {
            get; set;
        }
        public string Address
        {
            get; set;
        }
        public DateTime DateAdded
        {
            get; set;
        }
        public string AddedBy
        {
            get; set;
        }
        public bool IsActive
        {
            get; set;
        }
        public string Reason
        {
            get; set;
        }
        public string LocationName
        {
            get; set;
        }
        public string DepartmentName
        {
            get; set;
        }
        public string AccountTitle
        {
            get; set;
        }
        public List<CancelledOrders> CancelledOrders
        {
            get; set;
        }
        public List<Ordering> Orders
        {
            get;
            set;
        } = new();

    }

}
