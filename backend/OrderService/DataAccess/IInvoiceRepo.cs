﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using OrderService.Models;

namespace OrderService.DataAccess;

public interface IInvoiceRepo
{
    Task<IEnumerable<Invoice>> GetAllInvociesAsync(int? cursor, int pageSize);
    Task<Invoice?> GetInvocieByIdAsync(int id);
    Task<Invoice?> GetInvocieByOrderIdAsync(Guid orderId);
    Task<Invoice?> CreateInvocieAsync(Invoice invoice, Guid orderId);
    Task<Invoice?> UpdateInvoiceAsync(Invoice invoice);
    Task<Invoice?> DeleteInvoiceAsync(int id);
    Task<int> GetInvoiceCountAsync();
}
