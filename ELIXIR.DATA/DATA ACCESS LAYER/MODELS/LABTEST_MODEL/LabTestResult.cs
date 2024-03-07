using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace ELIXIR.DATA.DATA_ACCESS_LAYER.MODELS.LABTEST_MODEL
{
    public class LabTestResult : BaseEntity
    {
        public int ReceivedRequestId
        {
            get;
            set;
        }
        [Column(TypeName = "Date")]
        public DateTime DateAnalyze
        {
            get;
            set;
        }

        public string Remarks
        {
            get;
            set;
        }

        public string Result
        {
            get;
            set;
        }

        public string Status
        {
            get;
            set;
        } = "QA Apporval";

        public ReceiveRequest ReceiveRequest
        {
            get;
            set;
        }
    }
}