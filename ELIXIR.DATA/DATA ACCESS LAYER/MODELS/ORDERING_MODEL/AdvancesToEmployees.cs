using System;

namespace ELIXIR.DATA.DATA_ACCESS_LAYER.MODELS.ORDERING_MODEL
{
    public class AdvancesToEmployees : BaseEntity
    {
        public string EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}