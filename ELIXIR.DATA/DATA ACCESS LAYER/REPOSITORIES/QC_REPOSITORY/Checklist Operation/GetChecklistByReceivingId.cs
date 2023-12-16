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
            public int PO_Summary_Id { get; set; }
            public DateTime? Manufacturing_Date { get; set; }
            public decimal Expected_Delivery { get; set; }
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


            public int ChecklistType { get; set; }
            public IEnumerable<ChecklistAnswer> ChecklistAnswers { get; set; }
            public IEnumerable<ChecklistOpenFieldAnswer> ChecklistOpenFieldAnswers { get; set; }
            public IEnumerable<ChecklistProductDimension> ChecklistProductDimensions { get; set; }

            public class ChecklistAnswer
            {
                public int QCChecklistId { get; set; }
                public int ChecklistTypeId { get; set; }
                public string ChecklistType { get; set; }
                public int ChecklistQuestionId { get; set; }
                public string ChecklistQuestion { get; set; }
                public bool Status { get; set; }
            }

            public class ChecklistOpenFieldAnswer
            {
                public int QCChecklistId { get; set; }
                public int ChecklistTypeId { get; set; }
                public string ChecklistType { get; set; }
                public int ChecklistQuestionId { get; set; }
                public string ChecklistQuestion { get; set; }
                public string Remarks { get; set; }
            }

            public class ChecklistProductDimension
            {
                public int QCChecklistId { get; set; }
                public int ChecklistTypeId { get; set; }
                public string ChecklistType { get; set; }
                public int ChecklistQuestionId { get; set; }
                public string ChecklistQuestion { get; set; }
                public string Standard { get; set; }
                public string Actual { get; set; }
            }


            public class Handler : IRequestHandler<GetChecklistByReceivingIdQuery, GetChecklistByReceivingIdResult>
            {
                private readonly StoreContext _context;

                public Handler(StoreContext context)
                {
                    _context = context;
                }

                public StoreContext Context => _context;

                public async Task<GetChecklistByReceivingIdResult> Handle(GetChecklistByReceivingIdQuery request,
                    CancellationToken cancellationToken)
                {
                    var qCChecklist = await _context.QcChecklists
                                          .Include(x => x.PoReceiving)
                                          .Include(x => x.ProductType)
                                          .Include(x => x.OpenFieldAnswers)
                                          .ThenInclude(x => x.ChecklistQuestion)
                                          .ThenInclude(x => x.ChecklistType)
                                          .Include(x => x.ProductDimension)
                                          .ThenInclude(x => x.ChecklistQuestion)
                                          .ThenInclude(x => x.ChecklistType)
                                          .Include(x => x.ChecklistAnswers)
                                          .ThenInclude(x => x.ChecklistQuestions)
                                          .ThenInclude(x => x.ChecklistType)
                                          .Where(x => x.ReceivingId == request.ReceivingId)
                                          .FirstOrDefaultAsync(cancellationToken) ??
                                      throw new Exception("No receving checklist data found");

                    var resut = new GetChecklistByReceivingIdResult
                    {
                        ReceivingId = qCChecklist.ReceivingId,
                        PO_Summary_Id = qCChecklist.PoReceiving.PO_Summary_Id,
                        Manufacturing_Date = qCChecklist.PoReceiving.Manufacturing_Date,
                        Expected_Delivery = qCChecklist.PoReceiving.Expected_Delivery,
                        Expiry_Date = qCChecklist.PoReceiving.Expiry_Date,
                        Actual_Delivered = qCChecklist.PoReceiving.Actual_Delivered,
                        ItemCode = qCChecklist.PoReceiving.ItemCode,
                        Batch_No = qCChecklist.PoReceiving.Batch_No,
                        TotalReject = qCChecklist.PoReceiving.TotalReject,
                        IsActive = qCChecklist.PoReceiving.IsActive,
                        CancelDate = qCChecklist.PoReceiving.CancelDate,
                        CancelBy = qCChecklist.PoReceiving.CancelBy,
                        Reason = qCChecklist.PoReceiving.Reason,
                        ExpiryIsApprove = qCChecklist.PoReceiving.ExpiryIsApprove,
                        IsNearlyExpire = qCChecklist.PoReceiving.IsNearlyExpire,
                        ExpiryApproveBy = qCChecklist.PoReceiving.ExpiryApproveBy,
                        ExpiryDateOfApprove = qCChecklist.PoReceiving.ExpiryDateOfApprove,
                        QC_ReceiveDate = qCChecklist.PoReceiving.QC_ReceiveDate,
                        ConfirmRejectByQc = qCChecklist.PoReceiving.ConfirmRejectByQc,
                        IsWareHouseReceive = qCChecklist.PoReceiving.IsWareHouseReceive,
                        CancelRemarks = qCChecklist.PoReceiving.CancelRemarks,
                        QcBy = qCChecklist.PoReceiving.QcBy,
                        MonitoredBy = qCChecklist.PoReceiving.MonitoredBy,
                        ProductTypeId = qCChecklist.ProductTypeId,
                        ProductType = qCChecklist.ProductType.ProductTypeName,
                        ChecklistAnswers = qCChecklist.ChecklistAnswers
                            .Select(cq => new ChecklistAnswer
                            {
                                QCChecklistId = cq.Id,
                                ChecklistType = cq.ChecklistQuestions.ChecklistType.ChecklistType,
                                ChecklistQuestion = cq.ChecklistQuestions.ChecklistQuestion,
                                ChecklistQuestionId = cq.ChecklistQuestionsId,
                                Status = cq.Status
                            }),
                        ChecklistProductDimensions = qCChecklist.ProductDimension
                            .Select(ca => new ChecklistProductDimension
                            {
                                QCChecklistId = ca.QCChecklistId,
                                ChecklistType = ca.ChecklistQuestion.ChecklistType.ChecklistType,
                                ChecklistQuestionId = ca.ChecklistQuestionId,
                                ChecklistQuestion = ca.ChecklistQuestion.ChecklistQuestion,
                                Standard = ca.Standard,
                                Actual = ca.Actual
                            }),
                        ChecklistOpenFieldAnswers = qCChecklist.OpenFieldAnswers
                            .Select(cq => new ChecklistOpenFieldAnswer
                            {
                                QCChecklistId = cq.Id,
                                ChecklistType = cq.ChecklistQuestion.ChecklistType.ChecklistType,
                                ChecklistQuestion = cq.ChecklistQuestion.ChecklistQuestion,
                                ChecklistQuestionId = cq.ChecklistQuestionId,
                                Remarks = cq.Remarks
                            }),
                    };

                    return resut;
                }
            }
        }
    }
}