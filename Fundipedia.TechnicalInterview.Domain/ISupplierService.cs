using Fundipedia.TechnicalInterview.Model.Supplier;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Fundipedia.TechnicalInterview.Domain;

public interface ISupplierService
{
    Task<List<Supplier>> GetSuppliersAsync();

    Task<Supplier> GetSupplierAsync(Guid id);

    Task InsertSupplier(Supplier supplier);

    Task<Supplier> DeleteSupplierAsync(Guid id);
}