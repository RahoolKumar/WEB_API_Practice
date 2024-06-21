using AutoMapper;
using ClosedXML.Excel;
using LearnAPI.Modal;
using LearnAPI.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using System.Data;
using System.Runtime.InteropServices;

namespace LearnAPI.Controllers
{

    [Authorize]
    //[DisableCors]
    [EnableRateLimiting("fixedWindow")]
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        public readonly ICustomerService _customerService;
        public readonly IMapper _mapper;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public CustomerController(ICustomerService customerService,IMapper mapper, IWebHostEnvironment webHostEnvironment)
        {
            _customerService = customerService;
            _mapper = mapper;
            _webHostEnvironment = webHostEnvironment;
        }

        [AllowAnonymous]
        //[EnableCors("corspolicy1")]
        [HttpGet("GetAll")]
        public async Task<IActionResult> Get()
        {
          
            var data = await _customerService.GetAllData();
            if(data==null)
            {
                return NotFound();
            }
            return Ok(data);
        }


        [DisableRateLimiting]
        [HttpGet("GetByCode")]
        public async Task<IActionResult> GetbyCode(string code)
        {

            var data = await _customerService.Getbycode(code);
            if (data == null)
            {
                return NotFound();
            }
            return Ok(data);
        }
        [HttpPost("Create")]
        public async Task<IActionResult> Create(Customermodal cusData)
        {

            var data = await _customerService.Create(cusData);
            
            return Ok(data);
        }
        [HttpPost("Update")]
        public async Task<IActionResult> Update(Customermodal cusData,string code)
        {

            var data = await _customerService.Update(cusData,code);
            return Ok(data);
        }
        [HttpDelete("Remove")]
        public async Task<IActionResult> Remove(string code)
        {

            var data = await _customerService.Remove(code);
            return Ok(data);
        }

        [AllowAnonymous]
        [HttpGet("Exportexcel")]
        public async Task<IActionResult> Exportexcel()
        {
            try
            {
                string Filepath = GetFilepath();
                string excelpath = Filepath + "\\customerinfo.xlsx";
                
                DataTable dt = new DataTable();
                dt.Columns.Add("Code", typeof(string));
                dt.Columns.Add("Name", typeof(string));
                dt.Columns.Add("Email", typeof(string));
                dt.Columns.Add("Phone", typeof(string));
                dt.Columns.Add("CreditLimit", typeof(int));
                var data = await _customerService.GetAllData();
                if (data != null && data.Count > 0)
                {
                    data.ForEach(item =>
                    {
                        dt.Rows.Add(item.Code, item.Name, item.Email, item.Phone, item.Creditlimit);
                    });
                }
                using (XLWorkbook wb = new XLWorkbook())
                {
                    wb.AddWorksheet(dt, "Customer Info");
                    using (MemoryStream stream = new MemoryStream())
                    {
                        wb.SaveAs(stream);

                        if (System.IO.File.Exists(excelpath))
                        {
                            System.IO.File.Delete(excelpath);
                        }
                        wb.SaveAs(excelpath);

                        return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Customer.xlsx");
                    }
                }
            }
            catch (Exception ex)
            {
                return NotFound();
            }
        }
        [NonAction]
        private string GetFilepath()
        {
            return this._webHostEnvironment.WebRootPath + "\\Export";
        }

    }

}
