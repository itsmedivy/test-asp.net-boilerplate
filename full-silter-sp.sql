
GO
/****** Object:  StoredProcedure [dbo].[GetPurchaseOrderByFullFilter]    Script Date: 26-06-2024 10:53:09 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- exec GetPurchaseOrderByFullFilter 10,0,'PurchaseOrderNo','asc','Any','','Any',null,null,'Any'  
ALTER Procedure [dbo].[GetPurchaseOrderByFullFilter]  
@DisplayLength INT,  
@DisplayStart INT,  
@SortCol NVARCHAR(100),  
@SortDir NVARCHAR(100),  
@SearchCol NVARCHAR(100) = NULL,  
@Search NVARCHAR(500) = NULL,  
@BetweenCol varchar(100), @StartDate varchar(100), @EndDate varchar(100),  
@Status varchar(100),  
@VendorID int=null,  
@UserId int=null  
AS  
BEGIN  
 DECLARE @FirstRec INT, @LastRec INT  
  SET @FirstRec = @DisplayStart;  
  SET @LastRec = @DisplayStart + @DisplayLength;  
  
  Declare @DateNow date = Convert(DATE,getdate());  
  
  WITH CTE_Loan AS   
       (  
        SELECT ROW_NUMBER() OVER (ORDER BY  
    CASE WHEN (@SortCol = 'PurchaseOrderNo' AND @SortDir='asc')  
            THEN PO.PurchaseOrderNo  
          END ASC,  
          CASE WHEN (@SortCol = 'PurchaseOrderNo' AND @SortDir='desc')  
            THEN PO.PurchaseOrderNo  
          END DESC,  
  
    CASE WHEN (@SortCol = 'OrderDate' AND @SortDir='asc')  
            THEN PO.OrderDate  
          END ASC,  
          CASE WHEN (@SortCol = 'OrderDate' AND @SortDir='desc')  
            THEN PO.OrderDate  
          END DESC,  
  
    CASE WHEN (@SortCol = 'VendorName' AND @SortDir='asc')  
            THEN V.VendorName  
          END ASC,  
          CASE WHEN (@SortCol = 'VendorName' AND @SortDir='desc')  
            THEN V.VendorName  
          END DESC  
          )  
  AS RowNum,  
  COUNT(*) OVER() AS TotalCount,  
   PO.Id as 'PurchaseOrderId',PO.PurchaseOrderNo,GT.Alias,C.CurrencyCode,V.VendorName,  
   PO.TotalItems,PO.TotalWeight,PO.TotalFineWeight,PO.TotalAmount,PO.IsPaymentDone,  
   PO.OrderDate, PO.CashBalance,PO.GoldBalance  
  from PurchaseOrder PO   
   join GuildType GT on PO.GuildId=GT.Id  
   join Currency C on PO.CurrencyId=C.CurrencyId  
   join Vendor V on PO.VendorId=v.VendorId  
  WHERE (  
   PO.PurchaseOrderNo like case @SearchCol when 'PONo' then '%'+@Search+'%' else PO.PurchaseOrderNo end and   
   V.VendorName like case @SearchCol when 'VendorName' then '%'+@Search+'%' else V.VendorName end and  
   GT.Alias like case @SearchCol when 'Alias' then '%'+@Search+'%' else GT.Alias end   
   and  
   (  
    PO.PurchaseOrderNo like case @SearchCol when 'Any' then '%'+@Search+'%' else PO.PurchaseOrderNo end or   
    V.VendorName like case @SearchCol when 'Any' then '%'+@Search+'%' else V.VendorName end or  
    GT.Alias like case @SearchCol when 'Any' then '%'+@Search+'%' else GT.Alias end  
   )
   and 
	(
		PO.IsPaymentDone = case @Status when 'IsPaymentDone' then 1 else PO.IsPaymentDone end and
		PO.IsPaymentDone = case @Status when 'IsPaymentNotDone' then 0 else PO.IsPaymentDone end
	)
   and  
   (  
    isnull(Convert(DATE,PO.OrderDate),@DateNow) between case @BetweenCol when 'OrderDate' then   
     @StartDate else isnull(Convert(DATE,PO.OrderDate),@DateNow) end and   
      case @BetweenCol when 'OrderDate' then @EndDate else isnull(Convert(DATE,PO.OrderDate),@DateNow) end and  
  
    isnull(Convert(DATE,PO.CreatedOn),@DateNow) between case @BetweenCol when 'CreatedOn' then   
     @StartDate else isnull(Convert(DATE,PO.CreatedOn),@DateNow) end and   
      case @BetweenCol when 'CreatedOn' then @EndDate else isnull(Convert(DATE,PO.CreatedOn),@DateNow) end   
   )  
   and PO.VendorId = case when @VendorID is null then PO.VendorId else @VendorID end  
   --and v.CreatedBy = case when @UserId is null then v.CreatedBy else @UserId end  
  )  
    )  
    SELECT * FROM CTE_Loan  
    WHERE RowNum > @FirstRec AND RowNum <= @LastRec;  
END