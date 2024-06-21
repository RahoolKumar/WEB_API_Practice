using AutoMapper;
using LearnAPI.Helper;
using LearnAPI.Modal;
using LearnAPI.Repos;
using LearnAPI.Repos.Models;
using LearnAPI.Service;
using Microsoft.EntityFrameworkCore;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace LearnAPI.Container
{
    public class CustomerService : ICustomerService
    {
        public readonly LearndataContext _context;
        private readonly IMapper _mapper;
        public readonly ILogger<CustomerService> _logger;
        public CustomerService(LearndataContext learndataContext, IMapper mapper, ILogger<CustomerService> logger)
        {
            _context = learndataContext;
            _mapper = mapper;
            _logger = logger;

        }

        public async Task<APIResponse> Create(Customermodal data)
        {
           APIResponse response = new APIResponse();
            try
            {
                _logger.LogInformation("Create Begin");
                TblCustomer tblCustomer = _mapper.Map<TblCustomer>(data);
                await _context.TblCustomers.AddAsync(tblCustomer);
                await _context.SaveChangesAsync();
                response.ResponseCode = 201;
                response.Result = data.Code;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message,ex);
                response.Message = ex.Message;
                response.ResponseCode = 400;
            }
            return response;
        }

        public async Task<List<Customermodal>> GetAllData()
        {
            List<Customermodal> data = new List<Customermodal>();
            var CustomerData =  await _context.TblCustomers.ToListAsync();
            
            if(CustomerData!=null)
            {
                data = _mapper.Map<List<Customermodal>>(CustomerData);
            }
            return data;
        }

        public async Task<Customermodal> Getbycode(string code)
        {
            Customermodal data = new Customermodal();
            var CustomerData = await _context.TblCustomers.FirstOrDefaultAsync(x => x.Code == code);

            if (CustomerData != null)
            {
                data = _mapper.Map<Customermodal>(CustomerData);
            }
            return data;
        }

        public async Task<APIResponse> Remove(string code)
        {
            APIResponse response = new APIResponse();
            try
            {
                var customer = await _context.TblCustomers.FirstOrDefaultAsync(x => x.Code==code);

                if(customer!=null)
                {
                    _context.TblCustomers.Remove(customer);
                    await _context.SaveChangesAsync();
                    response.ResponseCode = 200;
                    response.Result = code;
                }
                else
                {
                    response.ResponseCode = 404;
                    response.Result = "Data not found";
                }

              /*  TblCustomer tblCustomer = _mapper.Map<TblCustomer>(data);
                await _context.TblCustomers.AddAsync(tblCustomer);
                await _context.SaveChangesAsync();*/
               
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
                response.ResponseCode = 400;
            }
            return response;
        }

        public async Task<APIResponse> Update(Customermodal data, string code)
        {
            APIResponse response = new APIResponse();
            try
            {
                var _customer = await this._context.TblCustomers.FindAsync(code);
                if (_customer != null)
                {
                    _customer.Name = data.Name;
                    _customer.Email = data.Email;
                    _customer.Phone = data.Phone;
                    _customer.IsActive = data.IsActive;
                    _customer.Creditlimit = data.Creditlimit;
                    await this._context.SaveChangesAsync();
                    response.ResponseCode = 200;
                    response.Result = "pass";
                }
                else
                {
                    response.ResponseCode = 404;
                    response.Message = "Data not found";
                }


            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
                response.ResponseCode = 400;
            }
            return response;
        }
    }
}
