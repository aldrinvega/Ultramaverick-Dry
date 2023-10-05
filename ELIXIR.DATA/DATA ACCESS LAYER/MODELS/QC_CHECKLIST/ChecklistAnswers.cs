using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELIXIR.DATA.DATA_ACCESS_LAYER.MODELS.QC_CHECKLIST
{
    public class ChecklistAnswers : BaseEntity
    {
        public int QCChecklistId { get; set; }
        public int ChecklistQuestionsId { get; set; }
        public bool Status { get; set; }


        public virtual QCChecklist QCChecklist { get; set; }
        public virtual ChecklistQuestions ChecklistQuestions { get; set; }
    }
}