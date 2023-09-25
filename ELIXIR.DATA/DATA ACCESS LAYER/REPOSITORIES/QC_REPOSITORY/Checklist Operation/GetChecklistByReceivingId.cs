using ELIXIR.DATA.CORE.INTERFACES.QC_INTERFACE;
using ELIXIR.DATA.DATA_ACCESS_LAYER.HELPERS;
using ELIXIR.DATA.DATA_ACCESS_LAYER.MODELS.LABTEST_MODEL;
using ELIXIR.DATA.DATA_ACCESS_LAYER.MODELS.QC_CHECKLIST;
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
            public int? ReceivingId { get; set; }
        }

        public class GetChecklistByReceivingIdResult
        {
            public int ReceivingId { get; set; }
            public int? ProductTypeId { get; set; }
            public string ProductType { get; set; }
            public IList<ChecklistOpenFieldAnswer> OpenFieldAnswers { get; set; }
            public IList<ChecklistAnswer> ChecklistAnswers { get; set; }
            public IList<ChecklistProductDimension> ProductDimensions { get; set; }
            public ReviewVerificationLogs ReviewVerificationLog { get; set; }
            public ChecklistCompliances ChecklistCompliance { get; set; }
            public ChecklistOtherObservation ChecklistOtherObservations { get; set; }

            public class ChecklistAnswer
            {
                public int ChecklistQuestionId { get; set; }
                public string ChecklistQuestion { get; set; }
                public int ChecklistTypeId { get; set; }
                public string ChecklistType { get; set; }
                public bool Status { get; set; }
            }

            public class ChecklistOpenFieldAnswer
            {
                public int ChecklistQuestionId { get; set; }
                public string ChecklistQuestion { get; set; }
                public int ChecklistTypeId { get; set; }
                public string ChecklistType { get; set; }
                public string Remarks { get; set; }
            }

            public class ChecklistProductDimension
            {
                public int ChecklistQuestionId { get; set; }
                public string ChecklistQuestion { get; set; }
                public int ChecklistTypeId { get; set; }
                public string ChecklistType { get; set; }
                public string Standard { get; set; }
                public string Actual { get; set; }
            }

            public class ReviewVerificationLogs
            {
                public int DispositionId { get; set; }
                public int QtyAccepted { get; set; }
                public int QtyRejected { get; set; }
                public string MonitoredBy { get; set; }
                public string ReviewedBy { get; set; }
                public string VerifiedBy { get; set; }
                public string NotedBy { get; set; }
            }

            public class ChecklistCompliances
            {
                public string Compliance { get; set; }
                public string ComplianceDescription { get; set; }
                public string RootCause { get; set; }
            }

            public class ChecklistOtherObservation
            {
                public string Observation { get; set; }
            }
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
                    .Include(x => x.ProductType)
                    .Include(q => q.ChecklistAnswers)
                .Include(q => q.OpenFieldAnswers)
                    .ThenInclude(x => x.ChecklistQuestions)
                        .ThenInclude(x => x.ChecklistType)
                .Include(q => q.ProductDimension)
                    .ThenInclude(x => x.ChecklistQuestions)
                .Include(q => q.ChecklistReviewVerificationLog)
                .Include(q => q.ChecklistCompliance)
                .Include(q => q.ChecklistOtherObservation)
                .FirstOrDefaultAsync(x => x.Id == request.ReceivingId) ?? throw new Exception("No receving checklist data found");
                var result = new GetChecklistByReceivingIdResult
                {
                    ReceivingId = qCChecklist.ReceivingId,
                    ProductTypeId = qCChecklist.ProductTypeId,
                    ProductType = qCChecklist.ProductType.ProductTypeName,
                    OpenFieldAnswers = qCChecklist.OpenFieldAnswers
                         .Select(o => new GetChecklistByReceivingIdResult.ChecklistOpenFieldAnswer
                         {
                             ChecklistQuestionId = o.ChecklistQuestionId,
                             ChecklistQuestion = o.ChecklistQuestions.ChecklistQuestion,
                             ChecklistType = o.ChecklistQuestions.ChecklistType.ChecklistType,
                             ChecklistTypeId = o.ChecklistQuestions.ChecklistTypeId,
                             Remarks = o.Remarks
                         }).ToList(),
                    ChecklistAnswers = qCChecklist.ChecklistAnswers
                         .Select(a => new GetChecklistByReceivingIdResult.ChecklistAnswer
                         {
                             ChecklistQuestionId = a.ChecklistQuestionsId,
                             ChecklistQuestion = a.ChecklistQuestions.ChecklistQuestion,
                             ChecklistType = a.ChecklistQuestions.ChecklistType.ChecklistType,
                             ChecklistTypeId = a.ChecklistQuestions.ChecklistTypeId,
                             Status = a.Status
                         }).ToList(),
                    ProductDimensions = qCChecklist.ProductDimension
                         .Select(pd => new GetChecklistByReceivingIdResult.ChecklistProductDimension
                         {
                             ChecklistQuestionId = pd.ChecklistQuestionId,
                             ChecklistQuestion = pd.ChecklistQuestions.ChecklistQuestion,
                             ChecklistType = pd.ChecklistQuestions.ChecklistType.ChecklistType,
                             ChecklistTypeId = pd.ChecklistQuestions.ChecklistTypeId,
                             Standard = pd.Standard,
                             Actual = pd.Actual
                         }).ToList(),
                    ReviewVerificationLog = new GetChecklistByReceivingIdResult.ReviewVerificationLogs
                    {
                        DispositionId = qCChecklist.ChecklistReviewVerificationLog.DispositionId,
                        QtyAccepted = qCChecklist.ChecklistReviewVerificationLog.QtyAccepted,
                        QtyRejected = qCChecklist.ChecklistReviewVerificationLog.QtyRejected,
                        MonitoredBy = qCChecklist.ChecklistReviewVerificationLog.MonitoredBy,
                        ReviewedBy = qCChecklist.ChecklistReviewVerificationLog.ReviewedBy,
                        VerifiedBy = qCChecklist.ChecklistReviewVerificationLog.VerifiedBy,
                        NotedBy = qCChecklist.ChecklistReviewVerificationLog.NotedBy
                    },
                    ChecklistCompliance = new GetChecklistByReceivingIdResult.ChecklistCompliances
                    {
                        Compliance = qCChecklist.ChecklistCompliance.Compliance,
                        ComplianceDescription = qCChecklist.ChecklistCompliance.Compliance,
                        RootCause = qCChecklist.ChecklistCompliance.RootCause
                    },
                    ChecklistOtherObservations = new GetChecklistByReceivingIdResult.ChecklistOtherObservation
                    {
                        Observation = qCChecklist.ChecklistOtherObservation.Observation
                    }
                };

                return result;
            }
        }
    }
}
