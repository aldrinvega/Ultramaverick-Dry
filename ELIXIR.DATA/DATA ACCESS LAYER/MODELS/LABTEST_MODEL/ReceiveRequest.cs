using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace ELIXIR.DATA.DATA_ACCESS_LAYER.MODELS.LABTEST_MODEL
{
    public class ReceiveRequest : BaseEntity
    {
        [ForeignKey("LabTestRequests")]
        public int LabTestRequestsId
        {
            get;
            set;
        }

        public DateTime ExtendedExpirationDate
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

        public virtual LabTestRequests LabTestRequests
        {
            get;
            set;
        }
    }
}