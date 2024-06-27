using EIOMS.Data;
using EIOMS.Extensions;
using EIOMS.Models;
using EIOMS.Models.DataTableViewModels;
using EIOMS.Models.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace EIOMS.DataRepo
{
    public interface IPurchaseOrderRepo
    {
        List<PurchaseOrderFilterResponse> POFilter(ReportDataTableRequest req);
        List<PurchaseOrder> GetPurchaseOrders();
        PurchaseOrder GetById(int id);
        void CompletePayment(int id);
        PurchaseOrderDetail GetPurchaseOrderDetails(int PurchaseOrderId);
        PurchaseOrder Insert(PurchaseOrder model);

        PurchaseOrderLine InsertLine(PurchaseOrderLine model);
        int InsertLines(List<PurchaseOrderLine> list);
        List<PurchaseOrderLine> GetOrderItems(int PurchaseOrderId);
        List<string> GetItemSuggestion(string name, int limit = 4);

        List<PurchasePayment> GetOrderPayments(int PurchaseOrderId);
        PurchasePayment Add(PurchasePayment model);
    }
    public class PurchaseOrderRepo : IPurchaseOrderRepo
    {
        private readonly ApplicationDbContext _db;
        public PurchaseOrderRepo(ApplicationDbContext context)
        {
            _db = context;
        }

        #region PurchaseOrder
        public List<PurchaseOrderFilterResponse> POFilter(ReportDataTableRequest req)
        {
            string StoredProc = "exec GetPurchaseOrderByFullFilter " +
                $"@DisplayLength={req.Length}, " +
                $"@DisplayStart={req.Start}, " +
                $"@SortCol='{req.Columns[req.Order[0].Column].Data}', " +
                $"@SortDir='{req.Order[0].Dir}', " +
                $"@SearchCol='{req.SearchCol}', " +
                $"@Search='{req.SearchText ?? ""}', " +
                $"@BetweenCol='{req.BetweenCol}', " +
                $"@StartDate='{req.StartDate}', " +
                $"@EndDate='{req.EndDate}', " +
                $"@Status='{req.Status}', " +
                $"@UserId={1}";
            List<PurchaseOrderFilterResponse> list = _db.PurchaseOrderFilter.FromSqlRaw(StoredProc).ToList();
            return list;
        }
        public PurchaseOrderDetail GetPurchaseOrderDetails(int PurchaseOrderId)
        {
            string StoredProc = $"exec GetPurchaseOrderDetails @PurchaseOrderId={PurchaseOrderId}";
            PurchaseOrderDetail result = _db.PurchaseOrderDetail.FromSqlRaw(StoredProc).ToList().FirstOrDefault();
            return result;
        }
        public PurchaseOrder Insert(PurchaseOrder model)
        {
            _db.PurchaseOrder.Add(model);
            _db.SaveChanges();
            return model;
        }
        public List<PurchaseOrder> GetPurchaseOrders()
        {
            List<PurchaseOrder> list = _db.PurchaseOrder.ToList();
            return list;
        }
        public PurchaseOrder GetById(int id)
        {
            PurchaseOrder result = _db.PurchaseOrder.Where(x => x.Id.Equals(id)).FirstOrDefault();
            return result;
        }

        public void CompletePayment(int id)
        {
            PurchaseOrder po = _db.PurchaseOrder.FirstOrDefault(x => x.Id == id);
            if (po != null)
            {
                po.IsPaymentDone = true;
                po.CashBalance = 0;
                po.GoldBalance = 0;
                po.UpdatedOn = DateTime.Now;
                po.UpdatedBy = string.IsNullOrEmpty(po.CreatedBy) ? "1,Admin" : po.CreatedBy;
                _db.SaveChanges();
            }
        }

        #endregion

        #region PurchaseOrderLine
        public PurchaseOrderLine InsertLine(PurchaseOrderLine model)
        {
            _db.PurchaseOrderLine.Add(model);
            _db.SaveChanges();
            return model;
        }
        public int InsertLines(List<PurchaseOrderLine> list)
        {
            _db.PurchaseOrderLine.AddRange(list);
            _db.SaveChanges();
            return list.Count;
        }
        public List<PurchaseOrderLine> GetOrderItems(int PurchaseOrderId)
        {
            List<PurchaseOrderLine> list = _db.PurchaseOrderLine.Where(x => x.PurchaseOrderId.Equals(PurchaseOrderId)).ToList();
            return list;
        }
        public List<string> GetItemSuggestion(string name, int limit = 4)
        {
            List<string> list = new List<string>();
            // Assuming 'Name' is the property to match against
            list = _db.PurchaseOrderLine.Where(i => i.ItemName.Contains(name))
                .Select(i => i.ItemName).Distinct().Take(limit).ToList();
            return list;
        }
        #endregion

        #region PurchasePayment
        public List<PurchasePayment> GetOrderPayments(int PurchaseOrderId)
        {
            List<PurchasePayment> list = _db.PurchasePayment.Where(x => x.PurchaseOrderId.Equals(PurchaseOrderId)).OrderByDescending(x => x.PayDate).ToList();
            return list;
        }
        public PurchasePayment Add(PurchasePayment model)
        {
            model.CreatedOn = DateTime.Now;
            model.CreatedBy = string.IsNullOrEmpty(model.CreatedBy) ? "1,Admin" : model.CreatedBy;
            _db.PurchasePayment.Add(model);
            _db.SaveChanges();
            //errmsg = CommonUtility.SUCCESS;
            return model;
        }
        #endregion
    }
}
