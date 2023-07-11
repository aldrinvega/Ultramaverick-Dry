using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using ELIXIR.DATA.DATA_ACCESS_LAYER.MODELS.WAREHOUSE_MODEL;

namespace ELIXIR.DATA.DATA_ACCESS_LAYER.MODELS.LABTEST_MODEL
{
    public class LabTestRequests : BaseEntity
    {
        public int BatchId
        {
            get;
            set;
        }

        [ForeignKey("WarehouseReceiving")]
        public int WarehouseReceivingId
        {
            get;
            set;
        }

        public int Quantity
        {
            get;
            set;
        }

        public string TestType
        {
            get;
            set;
        }

        public DateTime DateRequested
        {
            get;
            set;
        } = DateTime.Now;

        public DateTime DateNeeded
        {
            get;
            set;
        }

        public List<string> Analysis
        {
            get;
            set;
        }

        public List<string> Parameters
        {
            get;
            set;
        }

        public List<string> ProductCondition
        {
            get;
            set;
        }

        public List<string> SampleType
        {
            get;
            set;
        }

        public List<string> TypeOfSwab
        {
            get;
            set;
        }

        public DateTime CreatedAt
        {
            get;
            set;
        } = DateTime.Now;

        public string Remarks
        {
            get;
            set;
        }

        public string Status
        {
            get;
            set;
        }

        public WarehouseReceiving WarehouseReceived
        {
            get;
            set;
        }
    }
}