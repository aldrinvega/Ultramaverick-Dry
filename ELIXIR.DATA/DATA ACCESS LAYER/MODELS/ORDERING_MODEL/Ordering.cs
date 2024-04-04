using System;
using ELIXIR.DATA.DATA_ACCESS_LAYER.MODELS.SETUP_MODEL;

namespace ELIXIR.DATA.DATA_ACCESS_LAYER.MODELS.ORDERING_MODEL;

public class Ordering : BaseEntity
{

    public int TransactId
    {
        get;
        set;
    }
    public string CustomerName
    {
        get;
        set;
    }
    public string CustomerPosition
    {
        get;
        set;
    }
    public string FarmType
    {
        get;
        set;
    }

    public string FarmCode
    {
        get;
        set;
    }
    public string FarmName
    {
        get;
        set;
    }
    public int OrderNo
    {
        get;
        set;
    }
    public string BatchNo
    {
        get;
        set;
    }

    public DateTime OrderDate
    {
        get;
        set;
    }
    public DateTime DateNeeded
    {
        get;
        set;
    }
    public string TimeNeeded
    {
        get;
        set;
    }
    public string TransactionType
    {
        get;
        set;
    }
    public string ItemCode
    {
        get;
        set;
    }
    public string ItemDescription
    {
        get;
        set;
    }
    public string Uom
    {
        get;
        set;
    }

    public decimal OriginalQuantityOrdered { get; set; }
    public decimal QuantityOrdered { get; set; }

    public string Category { get; set; }

    public bool IsActive { get; set; }

    public DateTime? PreparedDate { get; set; }

    public string PreparedBy { get; set; }

    public bool? IsApproved { get; set; }

    public DateTime? ApprovedDate { get; set; }

    public bool? IsReject { get; set; }

    public string RejectBy { get; set; }

    public DateTime? RejectedDate { get; set; }

    public bool IsPrepared { get; set; }

    public bool? IsCancel { get; set; }

    public string IsCancelBy { get; set; }

    public DateTime? CancelDate { get; set; }

    public string Remarks { get; set; }

    public int OrderNoPKey { get; set; }

    public bool IsMove { get; set; }

    public string PlateNumber { get; set; }

    public string DeliveryStatus { get; set; }

    public DateTime? ReceivedDate { get; set; }

    public bool? IsBeingPrepared { get; set; }

    public string? SetBy { get; set; }

    public int? AllocatedQuantity { get; set; }

    public bool? ForAllocation { get; set; }

    public bool? IsCancelledOrder { get; set; }

    public Customer Customer { get; set; }
    public int? CustomerId { get; set; }
    public string Reason { get; set; }

}