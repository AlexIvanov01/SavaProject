using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using CustomerService.DataAccess;
using CustomerService.Dtos;
using CustomerService.Models;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace CustomerService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomersController : ControllerBase
    {
        private readonly ICustomerRepo _repository;
        private readonly IMapper _mapper;

        public CustomersController(ICustomerRepo repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CustomerReadDto>>> GetCustomers()
        {
            try
            {
                Log.Information("--> Getting all customers.........");

                var customers = await _repository.GetAllCustomersAsync();

                if(!customers.Any())
                {
                    Log.Warning("--> No customerts found in database.");
                    return NotFound();
                }

                Log.Information("--> Fetched all customers from database.");

                return Ok(_mapper.Map<IEnumerable<CustomerReadDto>>(customers));
            }
            catch (ArgumentNullException ex)
            {
                Log.Warning(ex, "--> No customers present in the database.");
                return NoContent();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Internal server error.");
                return StatusCode(500, "An internal server error occured.");
            }
        }

        [HttpGet("{id}", Name = "GetCustomerById")]
        public async Task<ActionResult<CustomerReadDto>> GetCustomerById(Guid id)
        {
            try
            {
                Log.Information("--> Getting a customer with id {Id}........", id);

                var customer = await _repository.GetCustomerAsync(id);

                if (customer == null)
                {
                    Log.Warning("Customer with id {Id} not found.", id);
                    return NotFound();
                }

                Log.Information("--> Fetched a customer with id {Id}.", id);

                return Ok(_mapper.Map<CustomerReadDto>(customer));
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Internal server error");
                return StatusCode(500, "An internal server error occured.");
            }
        }

        [HttpPost]
        public async Task<ActionResult<CustomerReadDto>> CreateCustomer(CustomerCreateDto customerCreateDto)
        {
            try
            {
                Log.Information("--> Creating a product.............");
                var customerModel = _mapper.Map<Customer>(customerCreateDto);
                await _repository.CreateCustomerAsync(customerModel);

                Log.Information("Customer created: {@CustomerModel}", customerModel);

                var custoemrReadDto = _mapper.Map<CustomerReadDto>(customerModel);

                return CreatedAtRoute(nameof(GetCustomerById), new { Id = custoemrReadDto.Id }, custoemrReadDto);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Internal server error.");
                return StatusCode(500, "An internal server error occured.");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCustomer(Guid id, CustomerUpdateDto customerUpdateDto)
        {
            try
            {
                Log.Information("--> Updating a product with id {Id}....................", id);

                var customerModel = _mapper.Map<Customer>(customerUpdateDto);
                customerModel.Id = id;

                var nullCHeck = await _repository.UpdateCustomerAsync(customerModel);

                if (nullCHeck == null)
                {
                    Log.Warning("Customer with id {Id} not found for updating.", id);
                    return NotFound();
                }

                Log.Information("--> Customer with id {Id} updated", id);

                return Ok(_mapper.Map<CustomerReadDto>(customerModel));
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Internal server error.");
                return StatusCode(500, "An internal server error occured.");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCustomer(Guid id)
        {
            try
            {
                Log.Information("--> Deleting a customer...........");

                var nullCheck = await _repository.DeleteCustomerAsync(id);


                if (nullCheck == null)
                {
                    Log.Warning("Customer with id {Id} not found for deleting.", id);
                    return NotFound();
                }

                Log.Information("--> Customer with id {Id} deleted", id);

                return NoContent();
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Internal server error.");
                return StatusCode(500, "An internal server error occured.");
            }
        }
    }
}
