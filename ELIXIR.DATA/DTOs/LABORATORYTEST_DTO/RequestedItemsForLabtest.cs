using System;
using System.Collections.Generic;

namespace ELIXIR.DATA.DTOs.LABORATORYTEST_DTO
{
    public class RequestedItemsForLabtest
    {
        public int BatchId
        {
            get;
            set;
        }
        
        public int WarehouseReceivingId
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
    }
}