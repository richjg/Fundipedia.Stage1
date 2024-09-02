using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Fundipedia.TechnicalInterview.Model.Supplier;
using Fundipedia.TechnicalInterview.Domain;
using System.Net;
using Fundipedia.TechnicalInterview.Model.Extensions;

namespace Fundipedia.TechnicalInterview.Controllers;

[Route("api/[controller]")]
[ApiController]
public class SuppliersController : ControllerBase
{
    private readonly ISupplierService _supplierService;

    public SuppliersController(ISupplierService supplierService)
    {
        _supplierService = supplierService;
    }

    // GET: api/Suppliers
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Supplier>>> GetSuppliersAsync()
    {
        return Ok( await _supplierService.GetSuppliersAsync() );
    }

    // GET: api/Suppliers/5
    [HttpGet("{id}")]
    public async Task<ActionResult<Supplier>> GetSupplierAsync(Guid id)
    {
        var supplier = await _supplierService.GetSupplierAsync(id);

        if (supplier == null)
        {
            return NotFound();
        }

        return Ok(supplier);
    }

    // POST: api/Suppliers
    [HttpPost]
    public async Task<ActionResult<Supplier>> PostSupplierAsync(Supplier supplier)
    {
        await _supplierService.InsertSupplier(supplier);
        return CreatedAtAction("GetSupplier", new { id = supplier.Id }, supplier);
    }

    // DELETE: api/Suppliers/5
    [HttpDelete("{id}")]
    public async Task<ActionResult<Supplier>> DeleteSupplierAsync(Guid id)
    {
        //Dont like the idea of exceptions being used as program flow (task-cancelled the exception)
        //This brings up the discussion of validation? If validation is move in the service and the service return a wrapper type XXresult<T> with status,result,errors then all validation can be carried out in one place?

        if((await _supplierService.GetSupplierAsync(id))?.IsActive() == true)
        {
            return BadRequest("Supplier is active and cannot be deleted");
        }

        var supplier = await _supplierService.DeleteSupplierAsync(id);
        return Ok(supplier);
    }
}