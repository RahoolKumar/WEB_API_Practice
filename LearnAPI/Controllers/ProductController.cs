using LearnAPI.Helper;
using LearnAPI.Repos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IO;

namespace LearnAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly LearndataContext _learndataContext;

        public ProductController(IWebHostEnvironment webHostEnvironment, LearndataContext learndataContext)
        {
            _webHostEnvironment = webHostEnvironment;
            _learndataContext = learndataContext;
        }


        [HttpPut("UploadImage")]
        public async Task<IActionResult> UploadImage(IFormFile formFile, string productcode)
        {
            APIResponse response = new APIResponse();
            try
            {
                string Filepath = GetFilepath(productcode);
                if (!System.IO.Directory.Exists(Filepath))
                {
                    System.IO.Directory.CreateDirectory(Filepath);
                }

                string imagepath = Filepath + "\\" + productcode + ".png";
                if (System.IO.File.Exists(imagepath))
                {
                    System.IO.File.Delete(imagepath);
                }
                using (FileStream stream = System.IO.File.Create(imagepath))
                {
                    await formFile.CopyToAsync(stream);
                    response.ResponseCode = 200;
                    response.Result = "pass";
                }
            }
            catch (Exception ex)
            {
                response.Result = ex.Message;
            }
            return Ok(response);
        }

        [HttpGet("GetImage")]
        public async Task<IActionResult> GetImage(string productCode)
        {
            string ImageURL = string.Empty;
            string hosturl = $"{this.Request.Scheme}://{this.Request.Host}{this.Request.PathBase}";
            
            try
            {
                string FilePath = GetFilepath(productCode);
                string imagepath = FilePath + "\\" + productCode + ".png";

                if (System.IO.File.Exists(imagepath))
                {
                    //ImageURL = hosturl + "Upload/product/" + productCode + "/" + productCode + ".png";
                    ImageURL = hosturl + "/Upload/product/" + productCode + "/" + productCode + ".png";
                }
                else
                {
                    return NotFound();
                }

            }
            catch(Exception e)
            {

            }
            return Ok(ImageURL);
        }

        [HttpGet("GetMultiImage")]
        public async Task<IActionResult> GetMultiImage(string productCode)
        {
            List<string> ImageURL = new List<string>();
            string hosturl = $"{this.Request.Scheme}://{this.Request.Host}{this.Request.PathBase}";

            try
            {
                string FilePath = GetFilepath(productCode);

                if(System.IO.Directory.Exists(FilePath))
                {
                    DirectoryInfo directoryInfo = new DirectoryInfo(FilePath);
                    FileInfo[] fileInfos = directoryInfo.GetFiles();

                    foreach(FileInfo fileInfo in fileInfos)
                    {
                        string fileName = fileInfo.Name;
                        string imagepath = FilePath + "\\" + fileName;

                        if (System.IO.File.Exists(imagepath))
                        {
                            //ImageURL = hosturl + "Upload/product/" + productCode + "/" + productCode + ".png";
                            string _imageURL = hosturl + "/Upload/product/" + productCode + "/" + fileName;
                            ImageURL.Add(_imageURL);
                        }

                    }

                }

           

            }
            catch (Exception e)
            {

            }
            return Ok(ImageURL);
        }



        [HttpGet("download")]
        public async Task<IActionResult> download(string productCode)
        {
           // string ImageURL = string.Empty;
          //  string hosturl = $"{this.Request.Scheme}://{this.Request.Host}{this.Request.PathBase}";

            try
            {
                string FilePath = GetFilepath(productCode);
                string imagepath = FilePath + "\\" + productCode + ".png";

                if (System.IO.File.Exists(imagepath))
                {
                    MemoryStream memoryStream = new MemoryStream(); 
                    using(FileStream fileStream = new FileStream(imagepath,FileMode.Open))
                    {
                        await fileStream.CopyToAsync(memoryStream);
                    }
                    memoryStream.Position = 0;
                    return File(memoryStream, "image/png", productCode + ".png");
                    //ImageURL = hosturl + "Upload/product/" + productCode + "/" + productCode + ".png";
                   // ImageURL = hosturl + "/Upload/product/" + productCode + "/" + productCode + ".png";
                }
                else
                {
                    return NotFound();
                }

            }
            catch (Exception e)
            {
                return NotFound(e);
            }
            
        }

        [HttpGet("remove")]
        public async Task<IActionResult> Remove(string productCode)
        {
            // string ImageURL = string.Empty;
            //  string hosturl = $"{this.Request.Scheme}://{this.Request.Host}{this.Request.PathBase}";

            try
            {
                string FilePath = GetFilepath(productCode);
                string imagepath = FilePath + "\\" + productCode + ".png";

                if (System.IO.File.Exists(imagepath))
                {
                   System.IO.File.Delete(imagepath);
                    return Ok("Pass");
                }
                else
                {
                    return NotFound();
                }

            }
            catch (Exception e)
            {
                return NotFound(e);
            }

        }


        [HttpGet("Multiremove")]
        public async Task<IActionResult> Multiremove(string productCode)
        {
            // string ImageURL = string.Empty;
            //  string hosturl = $"{this.Request.Scheme}://{this.Request.Host}{this.Request.PathBase}";

            try
            {
                string FilePath = GetFilepath(productCode);
                //string imagepath = FilePath + "\\" + productCode + ".png";

                if(System.IO.Directory.Exists(FilePath))
                {
                    DirectoryInfo directoryInfo = new DirectoryInfo(FilePath);
                    FileInfo[] fileInfos = directoryInfo.GetFiles();

                    foreach (FileInfo fileInfo in fileInfos)
                    {
                        fileInfo.Delete();

                    }
                    return Ok("Pass");

                }
                else
                {
                    return NotFound() ;
                }


                

            }
            catch (Exception e)
            {
                return NotFound(e);
            }

        }

        [HttpPut("MultiUploadImage")]
        public async Task<IActionResult> MultiUploadImage(IFormFileCollection filecollection, string productcode)
        {
            APIResponse response = new APIResponse();
            int passCount = 0;int errrCount = 0;
            
            try
            {
                string Filepath = GetFilepath(productcode);
                if (!System.IO.Directory.Exists(Filepath))
                {
                    System.IO.Directory.CreateDirectory(Filepath);
                }

                foreach(var file in filecollection)
                {
                    string imagepath = Filepath + "\\" + file.FileName;
                    if (System.IO.File.Exists(imagepath))
                    {
                        System.IO.File.Delete(imagepath);
                    }
                    using (FileStream stream = System.IO.File.Create(imagepath))
                    {
                        await file.CopyToAsync(stream);
                       
                        passCount++;
                    
                    }
                }

               
              
                
            }
            catch (Exception ex)
            {
                errrCount++;
                response.Result = ex.Message;
            }
            response.ResponseCode = 200;
            response.Result = $"{passCount} File Uploaded \n {errrCount} file Failed";
            return Ok(response);
        }


        [HttpPut("DBMultiUploadImage")]
        public async Task<IActionResult> DBMultiUploadImage(IFormFileCollection filecollection, string productcode)
        {
            APIResponse response = new APIResponse();
            int passCount = 0; int errrCount = 0;

            try
            {
              
                foreach (var file in filecollection)
                {

                    using(MemoryStream  stream = new MemoryStream())
                    {
                        await file.CopyToAsync(stream);

                        _learndataContext.TblProductimages.Add(new Repos.Models.TblProductimage 
                        { 
                            Productcode = productcode,
                            Productimage = stream.ToArray()
                        
                         });

                        await _learndataContext.SaveChangesAsync();
                        passCount++;
                    }
                  
                }




            }
            catch (Exception ex)
            {
                errrCount++;
                response.Result = ex.Message;
            }
            response.ResponseCode = 200;
            response.Result = $"{passCount} File Uploaded \n {errrCount} file Failed";
            return Ok(response);
        }


        [HttpGet("GetDBMultiImage")]
        public async Task<IActionResult> GetDBMultiImage(string productCode)
        {
            List<string> ImageURL = new List<string>();
           // string hosturl = $"{this.Request.Scheme}://{this.Request.Host}{this.Request.PathBase}";

            try
            {


                var productImage =  _learndataContext.TblProductimages.Where(item => item.Productcode == productCode).ToList();
                
                
                if(productImage!=null && productImage.Count > 0)
                {
                    productImage.ForEach(item => {
                        ImageURL.Add(Convert.ToBase64String(item.Productimage)); 
                    });
                }

                else
                {
                    return NotFound();
                }    
                



            }
            catch (Exception e)
            {

            }
            return Ok(ImageURL);
        }


        [HttpGet("Dbdownload")]
        public async Task<IActionResult> Dbdownload(string productCode)
        {
            // string ImageURL = string.Empty;
            //  string hosturl = $"{this.Request.Scheme}://{this.Request.Host}{this.Request.PathBase}";

            try
            {


                var _productImage = await _learndataContext.TblProductimages.FirstOrDefaultAsync(item => item.Productcode == productCode);


                if (_productImage != null)
                {
                    return File(_productImage.Productimage, "image/png", productCode + ".png");
                }


              /*  string FilePath = GetFilepath(productCode);
                string imagepath = FilePath + "\\" + productCode + ".png";

                if (System.IO.File.Exists(imagepath))
                {
                    MemoryStream memoryStream = new MemoryStream();
                    using (FileStream fileStream = new FileStream(imagepath, FileMode.Open))
                    {
                        await fileStream.CopyToAsync(memoryStream);
                    }
                    memoryStream.Position = 0;
                    return File(memoryStream, "image/png", productCode + ".png");
                    //ImageURL = hosturl + "Upload/product/" + productCode + "/" + productCode + ".png";
                    // ImageURL = hosturl + "/Upload/product/" + productCode + "/" + productCode + ".png";
                }*/
                else
                {
                    return NotFound();
                }

            }
            catch (Exception e)
            {
                return NotFound(e);
            }

        }

        [NonAction]
        private string GetFilepath(string productcode)
        {
            return this._webHostEnvironment.WebRootPath + "\\Upload\\product\\" + productcode;
        }
    }
}
