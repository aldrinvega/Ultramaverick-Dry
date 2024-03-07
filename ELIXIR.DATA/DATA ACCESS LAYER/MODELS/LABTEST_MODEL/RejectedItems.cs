using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace ELIXIR.DATA.DATA_ACCESS_LAYER.MODELS.LABTEST_MODEL
{
    public class RejectedItems : BaseEntity
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

        public string Disposition
        {
            get;
            set;
        }

        public LabTestRequests LabTestRequests
        {
            get;
            set;
        }
    }
}