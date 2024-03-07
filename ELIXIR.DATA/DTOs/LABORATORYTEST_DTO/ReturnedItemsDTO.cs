using System;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;

namespace ELIXIR.DATA.DTOs.LABORATORYTEST_DTO
{
    public class ReturnedItemsDTO
    {
        public int WarehouseId
        {
            get;
            set;
        }

        public string SampleName
        {
            get;
            set;
        }

        public DateTime? OriginalExpirationDate
        {
            get;
            set;
        }

        public int Quantity
        {
            get;
            set;
        }

        public DateTime AllowableDate
        {
            get;
            set;
        }

        public int DaysToExpire
        {
            get;
            set;
        }

        public string Status
        {
            get;
            set;
        }

        public string Remarks
        {
            get;
            set;
        }
    }
}