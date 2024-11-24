using CrayonCloudSale.Infrastructure.Data.Models;

namespace CrayonCloudSale.Services.Interfaces;

public interface IPurchasedSoftwareService
{
    Task<PurchasedSoftware> CancelPurchase(long id);
    Task<PurchasedSoftware> ChangeQuantity(long id, int quantity);
    Task<PurchasedSoftware> ExtendExpiryDate(long id, DateTime validTo);
}
