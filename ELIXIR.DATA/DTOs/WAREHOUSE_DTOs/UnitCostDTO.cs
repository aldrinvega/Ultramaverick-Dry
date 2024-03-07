namespace ELIXIR.DATA.DTOs.WAREHOUSE_DTOs
{
    public class UnitCostDTO
    {
        public int Id
        {
            get;
            set;
        }

        public string ItemCode
        {
            get;
            set;
        }

        public decimal? UnitCost
        {
            get;
            set;
        }

        public decimal ActualGood
        {
            get;
            set;
        }

        public decimal InventoryQuantity
        {
            get;
            set;
        }

        public decimal TotalAmount
        {
            get;
            set;
        }

        public decimal WeigthedAverageUnitCost
        {
            get;
            set;
        }

        public decimal MoveOrderQuantity
        {
            get;
            set;
        }

        public decimal Difference
        {
            get;
            set;
        }
    }
}