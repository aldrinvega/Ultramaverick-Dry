using System;

namespace ELIXIR.DATA.DATA_ACCESS_LAYER.MODELS.LABTEST_MODEL
{
    public class OnGoingLabTest : BaseEntity
    {
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

        public DateTime? UpdatedAt
        {
            get;
            set;
        }

        public DateTime ExtensionDate
        {
            get;
            set;
        }

        public string Reason
        {
            get;
            set;
        }

        public string Status
        {
            get;
            set;
        }
    }
}