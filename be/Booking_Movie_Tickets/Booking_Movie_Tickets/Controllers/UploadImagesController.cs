using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Booking_Movie_Tickets.Controllers
{
    [Route("api/uploads")]
    [ApiController]
    public class UploadImagesController : ControllerBase
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        public UploadImagesController(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }
        //public async Task<IActionResult> UploadImage(IFormFile file)
        //{
        //    if (file == null || file.Length == 0)
        //    {
        //        return BadRequest("No file uploaded.");
        //    }

        //    try
        //    {
        //        var webRootPath = _webHostEnvironment.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
        //        var uploadPath = Path.Combine(webRootPath, "uploads");

        //        if (!Directory.Exists(uploadPath))
        //        {
        //            Directory.CreateDirectory(uploadPath);
        //        }

        //        var fileName = Guid.NewGuid().ToString("N") + Path.GetExtension(file.FileName);
        //        var filePath = Path.Combine(uploadPath, fileName);

        //        using (var fileStream = new FileStream(filePath, FileMode.Create))
        //        {
        //            await file.CopyToAsync(fileStream);
        //        }

        //        var fileUrl = $"{Request.Scheme}://{Request.Host}/uploads/{fileName}";
        //        return Ok(new { fileName, fileUrl });
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(new { message = "File upload failed", error = ex.Message });
        //    }
        //}
    }
}
