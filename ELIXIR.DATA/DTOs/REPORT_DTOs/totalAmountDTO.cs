namespace ELIXIR.DATA.DTOs.REPORT_DTOs
{
    public class totalAmountDTO
    {
        public string ItemCode
        {
            get;
            set;
        }

        public int Po_Number
        {
            get;
            set;
        }

        public decimal UnitPrice
        {
            get;
            set;
        }

        public decimal TotalStocks
        {
            get;
            set;
        }

        public decimal WeightedAverageUnitCost
        {
            get;
            set;
        }
    }
}