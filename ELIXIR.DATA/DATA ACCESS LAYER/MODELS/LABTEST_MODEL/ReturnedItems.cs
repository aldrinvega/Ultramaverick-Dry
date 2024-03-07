using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace ELIXIR.DATA.DATA_ACCESS_LAYER.MODELS.LABTEST_MODEL
{
    public class ReturnedItems : BaseEntity
    {
        [ForeignKey("LabTestRequests")]
        public int LabTestRequestId
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

        public DateTime AllowableDate
        {
            get;
            set;
        }

        public string Reason
        {
            get;
            set;
        }

        public LabTestRequests LabTestRequest
        {
            get;
            set;
        }
    }
}