namespace Fundipedia.TechnicalInterview.Model.Extensions;

public static class SupplierExtensions
{
    public static bool IsActive(this Supplier.Supplier supplier)
    {
        return supplier.ActivationDate != null;
    }
}