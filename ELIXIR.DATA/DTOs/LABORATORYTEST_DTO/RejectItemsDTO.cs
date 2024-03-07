using System;

namespace ELIXIR.DATA.DTOs.LABORATORYTEST_DTO
{
    public class RejectItemsDTO
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

        public string CreatedAt
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

        public string Disposition
        {
            get;
            set;
        }
    }
}