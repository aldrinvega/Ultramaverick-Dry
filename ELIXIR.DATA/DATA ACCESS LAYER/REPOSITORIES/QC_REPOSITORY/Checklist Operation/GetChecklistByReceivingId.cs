/*using ELIXIR.DATA.CORE.INTERFACES.QC_INTERFACE;
using ELIXIR.DATA.CORE.INTERFACES.WAREHOUSE_INTERFACE;
using ELIXIR.DATA.DATA_ACCESS_LAYER.HELPERS;
using ELIXIR.DATA.DATA_ACCESS_LAYER.MODELS.LABTEST_MODEL;
using ELIXIR.DATA.DATA_ACCESS_LAYER.MODELS.QC_CHECKLIST;
using ELIXIR.DATA.DATA_ACCESS_LAYER.MODELS.SETUP_MODEL;
using ELIXIR.DATA.DATA_ACCESS_LAYER.STORE_CONTEXT;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ELIXIR.DATA.DATA_ACCESS_LAYER.REPOSITORIES.QC_REPOSITORY.Checklist_Operation
{
    public class GetChecklistByReceivingId
    {
        public class GetChecklistByReceivingIdQuery : IRequest<GetChecklistByReceivingIdResult>
        {
            public int? Id { get; set; }
        }

        public class GetChecklistByReceivingIdResult
        {
            public int ReceivingId { get; set; }
            public int PO_Summary_Id { get; set; }
            public DateTime? Manufacturing_Date { get; set; }
            public int Expected_Delivery { get; set; }
            public DateTime? Expiry_Date { get; set; }
            public decimal Actual_Delivered { get; set; }
            public string ItemCode { get; set; }
            public string Batch_No { get; set; }
            public decimal TotalReject { get; set; }
            public bool IsActive { get; set; }
            public DateTime? CancelDate { get; set; }
            public string CancelBy { get; set; }
            public string Reason { get; set; }
            public bool? ExpiryIsApprove { get; set; }
            public bool? IsNearlyExpire { get; set; }
            public string ExpiryApproveBy { get; set; }
            public DateTime? ExpiryDateOfApprove { get; set; }
            public DateTime QC_ReceiveDate { get; set; }
            public bool ConfirmRejectByQc { get; set; }
            public bool? IsWareHouseReceive { get; set; }
            public string CancelRemarks { get; set; }
            public string QcBy { get; set; }
            public string MonitoredBy { get; set; }
            public int? ProductTypeId { get; set; }
            public string ProductType { get; set; }


            public class ChecklistQuestionsForOpenField
            {
                public int ChecklistQuestionId { get; set; }
                public string ChecklistQuestion { get; set; }
                public int ChecklistTypeId { get; set; }
                public string ChecklistType { get; set; }
                public string Remarks { get; set; }
            }
            public class ChecklistQuestionsForYesOrNo
            {
                public int ChecklistQuestionId { get; set; }
                public string ChecklistQuestion { get; set; }
                public int ChecklistTypeId { get; set; }
                public string ChecklistType { get; set; }
                public bool Status { get; set; }
            }

            public class ChecklistQuestionForProductDimentions
            {
                public int ChecklistQuestionId { get; set; }
                public string ChecklistQuestion { get; set; }
                public int ChecklistTypeId { get; set; }
                public string ChecklistType { get; set; }
                public string Standard { get; set; }
                public string Actual { get; set; }
            }

        }

        public class Handler : IRequestHandler<GetChecklistByReceivingIdQuery, GetChecklistByReceivingIdResult>
        {
            private readonly StoreContext _context;

            public Handler(StoreContext context)
            {
                _context = context;
            }

            public class Handler : IRequestHandler<GetChecklistByReceivingIdQuery, GetChecklistByReceivingIdResult>
            {
                private readonly StoreContext _context;

                public Handler(StoreContext context)
                {
                    _context = context;
                }

                public async Task<GetChecklistByReceivingIdResult> Handle(GetChecklistByReceivingIdQuery request, CancellationToken cancellationToken)
                {
                    var qCChecklist = await _context.QcChecklists
                        .Include(x => x.PoReceiving)
                        .Include(x => x.ProductType)
                        .Include(q => q.ChecklistAnswers)
                            .ThenInclude(ans => ans.ChecklistQuestions)
                                .ThenInclude(cht => cht.ChecklistType)
                        .Include(q => q.OpenFieldAnswers)
                            .ThenInclude(x => x.ChecklistQuestions)
                                .ThenInclude(x => x.ChecklistType)
                        .Include(q => q.ProductDimension)
                            .ThenInclude(x => x.ChecklistQuestions)
                        .Include(q => q.ChecklistReviewVerificationLog)
                        .Include(q => q.ChecklistCompliance)
                        .Include(q => q.ChecklistOtherObservation)
                   .FirstOrDefaultAsync(x => x.Id == request.Id) ?? throw new Exception("No receving checklist data found");

                    // Determine the answer type (you need to adjust this part based on how you fetch it)
                    int answerType = qCChecklist; // Replace 'AnswerType' with the actual property that holds the answer type.

                    // Create a result variable with common properties
                    GetChecklistByReceivingIdResult result = new GetChecklistByReceivingIdResult
                    {
                        // Populate common properties here
                    };

                    // Use a switch statement to handle different answer types
                    switch (answerType)
                    {
                        case 1:
                        case 2:
                            result.ChecklistQuestionsForYesOrNo = qCChecklist.ChecklistAnswers
                                .Select(a => new GetChecklistByReceivingIdResult.ChecklistQuestionsForYesOrNo
                                {
                                    ChecklistQuestionId = a.ChecklistQuestionsId,
                                    ChecklistQuestion = a.ChecklistQuestions.ChecklistQuestion,
                                    ChecklistType = a.ChecklistQuestions.ChecklistType.ChecklistType,
                                    ChecklistTypeId = a.ChecklistQuestions.ChecklistTypeId,
                                    Status = a.Status
                                }).ToList();
                            break;
                        case 3:
                            result.ChecklistQuestionForProductDimentions = qCChecklist.ProductDimension
                                .Select(pd => new GetChecklistByReceivingIdResult.ChecklistQuestionForProductDimentions
                                {
                                    ChecklistQuestionId = pd.ChecklistQuestionId,
                                    ChecklistQuestion = pd.ChecklistQuestions.ChecklistQuestion,
                                    ChecklistType = pd.ChecklistQuestions.ChecklistType.ChecklistType,
                                    ChecklistTypeId = pd.ChecklistQuestions.ChecklistTypeId,
                                    Standard = pd.Standard,
                                    Actual = pd.Actual
                                }).ToList();
                            break;
                        default:
                            // Handle other cases or throw an exception for unsupported answer types.
                            throw new Exception("Unsupported answer type");
                    }

                    return result;
                }
            }

        }
    }
}
*/