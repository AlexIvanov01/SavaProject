﻿using System;
using System.Threading.Tasks;
using OrderService.Models;

namespace OrderService.DataAccess;

public interface ICustomerRepo
{
    Task<bool> ExternalCustomerExistsAsync(Guid? externalId);
    Task<Customer?> GetCustomerAsync(Guid id);
    Task АddCustomerAsync(Customer customer);
    Task DeleteCustomerAsync(Guid id);
    Task UpdateCustomerAsync(Customer customer);

}
