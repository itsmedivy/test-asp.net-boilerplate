using EIOMS.DataRepo;
using EIOMS.Extensions;
using EIOMS.Models;
using EIOMS.Models.DataTableViewModels;
using EIOMS.Models.ViewModels;
using EIOMS.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EIOMS.Controllers.Api
{
    [Authorize]
    [Produces("application/json")]
    [Route("api/PurchaseOrder")]
    public class PurchaseOrderController : Controller
    {
        private readonly INumberSequence _numberSequence;
        private readonly IPurchaseOrderRepo _poRepo;
        private readonly IStockRepo _stock;
        public PurchaseOrderController(INumberSequence numberSequence, IPurchaseOrderRepo poRepo, IStockRepo stock)
        {
            _numberSequence = numberSequence;
            _poRepo = poRepo;
            _stock = stock;
        }

        // GET: api/PurchaseOrder
        [HttpGet]
        public IActionResult GetPurchaseOrder()
        {
            List<PurchaseOrder> list = _poRepo.GetPurchaseOrders();
            return Ok(list);
        }

        [HttpGet("[action]")]
        public IActionResult ItemSuggest(string term, int limit = 4)
        {
            List<string> list = new List<string>();
            if (!string.IsNullOrEmpty(term))
                list = _poRepo.GetItemSuggestion(term, limit);
            return Ok(list);
        }

        [HttpPost("[action]")]
        public IActionResult Filter(ReportDataTableRequest req)
        {
            int TotalCount = 0;
            List<PurchaseOrderFilterResponse> list = _poRepo.POFilter(req);
            if (list.Count > 0)
                TotalCount = list[0].TotalCount ?? 0;
            return Ok(new
            {
                draw = req.Draw,
                recordsTotal = TotalCount,
                recordsFiltered = TotalCount,
                data = list.ToArray()
            });
        }

        [HttpGet("[action]/{id}")]
        public IActionResult GetById(int id)
        {
            PurchaseOrder result = _poRepo.GetById(id);
            return Ok(result);
        }

        [HttpPost("[action]")]
        public IActionResult Insert(PurchaseOrderViewModel model)
        {
            int purchaseId = 0;
            if (model != null && model.Purchase != null && model.PurchaseOrderLines != null && model.Payment != null && model.PurchaseOrderLines.Count > 0)
            {
                try
                {
                    if (model.Purchase.TotalAmount > 0
                        && model.Payment.CashBalance >= 0 && model.Payment.GoldBalance >= 0
                        && model.Purchase.TotalAmount == (model.Payment.GoldAmount + model.Payment.CashAmount + model.Payment.CashBalance))
                    {
                        //Insert Purchase Order
                        PurchaseOrder order = new PurchaseOrder
                        {
                            PurchaseOrderNo = _numberSequence.GetNumberSequence("PO"),
                            BranchId = model.Purchase.BranchId,
                            VendorId = model.Purchase.VendorId,
                            GuildId = model.Purchase.GuildId,
                            TaxId = model.Purchase.TaxId,
                            OrderDate = model.Purchase.OrderDate,
                            CurrencyId = model.Purchase.CurrencyId,
                            Remarks = model.Purchase.Remarks,
                            TotalItems = model.PurchaseOrderLines.Count,
                            TotalWeight = model.PurchaseOrderLines.Sum(x => x.TotalWeight),
                            TotalFineWeight = model.PurchaseOrderLines.Sum(x => x.FineWeight),
                            SubTotal = model.Purchase.SubTotal,
                            TaxPerc = model.Purchase.TaxPerc,
                            TaxAmount = model.Purchase.TaxAmount,
                            DiscountAmount = model.Purchase.DiscountAmount,
                            TotalAmount = model.Purchase.TotalAmount,
                            CashBalance = model.Payment.CashBalance,
                            GoldBalance = model.Payment.GoldBalance,
                            IsPaymentDone = (model.Payment.CashBalance == 0 && model.Payment.GoldBalance == 0),
                            CreatedOn = DateTime.Now,
                            CreatedBy = "1,Admin"
                        };
                        order = _poRepo.Insert(order);

                        //Insert Items
                        purchaseId = order.Id;
                        model.PurchaseOrderLines.ForEach(x => { x.PurchaseOrderId = purchaseId; });
                        _poRepo.InsertLines(model.PurchaseOrderLines);

                        //Add To Stock
                        Stock stock = new Stock
                        {
                            GuildId = order.GuildId,
                            EntityId = purchaseId,
                            Mode = CommonUtility.STM_CREDIT,
                            Quantity = order.TotalFineWeight,
                            CreatedBy = "1,Admin"
                        };
                        _stock.Add(stock, out string stockErrMsg);

                        //Add Purchase Payment
                        model.Payment.PurchaseOrderId = purchaseId;
                        model.Payment.ExtraAmount = 0;
                        model.Payment.CreatedBy = "1,Admin";
                        _poRepo.Add(model.Payment);

                        //Add to Account
                        //TODO

                        return Ok(new { Status = "Success", PurchaseId = purchaseId });
                    }
                    else
                    {
                        return BadRequest(new { message = "Total Amount and Payment Mismatch" });
                    }
                }
                catch (Exception ex)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Error occured while adding new purchase order." });
                }
            }
            else
                return BadRequest(new { message = "Unable to process request. Model was empty!" });
        }


        [HttpPost("[action]")]
        public IActionResult AddPayment(PurchasePayment model)
        {
            if (model != null)
            {
                try
                {
                    //Add Purchase Payment
                    model.PayDate = DateTime.Now;
                    model.PurchaseOrderId = model.PurchaseOrderId;
                    model.CreatedBy = "1,Admin";
                    _poRepo.Add(model);
                    
                    _poRepo.CompletePayment(model.PurchaseOrderId);

                    return Ok(new { Status = "Success", PurchaseId = model.PurchaseOrderId });
                }
                catch (Exception ex)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Error occured while adding new purchase payment." });
                }
            }
            else
                return BadRequest(new { message = "Unable to process request. Model was empty!" });
        }

    }
}