using Microsoft.EntityFrameworkCore;

namespace EIOMS.Models.ViewModels
{
    public class PurchaseOrderViewModel
    {
        public PurchaseOrder Purchase {  get; set; }
        public PurchasePayment Payment { get; set; }
        public List<PurchaseOrderLine> PurchaseOrderLines { get; set; }
    }

    [Keyless]
    public class PurchaseOrderFilterResponse
    {
        public long? RowNum { get; set; }
        public int? TotalCount { get; set; }
        public int PurchaseOrderId { get; set; }
        public string PurchaseOrderNo { get; set; }
        public string Alias { get; set; }
        public string CurrencyCode { get; set; }
        public string VendorName { get; set; }
        public int TotalItems { get; set; }
        public decimal TotalWeight { get; set; }
        public decimal TotalFineWeight { get; set; }
        public decimal TotalAmount { get; set; }
        public bool IsPaymentDone { get; set; }
        public decimal CashBalance { get; set; }
        public decimal GoldBalance { get; set; }
        public DateTime OrderDate { get; set; }
    }

    [Keyless]
    public class PurchaseOrderDetail
    {
        public int Id { get; set; }
        public string PurchaseOrderNo { get; set; }
        public int BranchId { get; set; }
        public int VendorId { get; set; }
        public int GuildId { get; set; }
        public int? TaxId { get; set; }
        public DateTime OrderDate { get; set; }
        public int CurrencyId { get; set; }
        public string Remarks { get; set; }
        public int TotalItems { get; set; }
        public decimal TotalWeight { get; set; }
        public decimal TotalFineWeight { get; set; }
        public decimal SubTotal { get; set; }
        public decimal TaxPerc { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal TotalAmount { get; set; }
        public bool IsPaymentDone { get; set; }
        public decimal CashBalance { get; set; }
        public decimal GoldBalance { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public string UpdatedBy { get; set; }
        public bool? IsDeleted { get; set; } = false;
        public DateTime? DeletedOn { get; set; }
        public string DeletedBy { get; set; }
        public string BranchName { get; set; }
        public string CurrencyCode { get; set; }
        public string Alias { get; set; }
        public string VendorName { get; set; }
        public string VendorAddress { get; set; }
        public string VendorCity { get; set; }
        public string VendorState { get; set; }
        public string VendorZipCode { get; set; }
        public string VendorPhone { get; set; }
        public string VendorEmail { get; set; }
        public string VendorContactPerson { get; set; }
    }
}
