using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using ELIXIR.DATA.DATA_ACCESS_LAYER.MODELS.WAREHOUSE_MODEL;
using ELIXIR.DATA.DTOs.WAREHOUSE_DTOs;

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
        public int? WarehouseId
        {
            get;
            set;
        }

        public List<string> Analysis
        {
            get;
            set;
        }

        public List<string> Disposition
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