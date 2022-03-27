using ImageSharePassword.Data;
using ImageSharePassword.Web.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ImageSharePassword.Web.Controllers
{
    public class HomeController : Controller
    {
        private string _connectionString = @"Data Source=.\sqlexpress;Initial Catalog=ImageShare; Integrated Security=true;";

        private readonly IWebHostEnvironment _webHostEnvironment;

        private readonly string ImageIdsSessionName = "ImageIdsAndPswd";

        public HomeController(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Upload(IFormFile imageFile, string password)
        {
            string fileName = $"{Guid.NewGuid()}-{imageFile.FileName}";

            string filePath = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", fileName);
            using var fs = new FileStream(filePath, FileMode.CreateNew);
            imageFile.CopyTo(fs);

            var repo = new ImageRepository(_connectionString);
            int id = repo.AddImage(fileName, password);
            return View(new UploadViewModel
            {
                Password = password,
                Id = id
            });
        }


        public IActionResult EnterPassword(int id)
        {
            var imageIds = HttpContext.Session.Get<List<int>>(ImageIdsSessionName);
            if (imageIds != null && imageIds.Contains(id))
            {
                return Redirect($"/home/viewImage?id={id}");
            }

            return View(new PasswordViewModel
            {
                Id = id,
                IncorrectPassword = (string)TempData["incorrectPassword"]
            });
        }
        public IActionResult ViewImage(int id)
        {
            var imageIds = HttpContext.Session.Get<List<int>>(ImageIdsSessionName);
            if (imageIds == null || !imageIds.Contains(id))
            {
                return Redirect($"/home/enterpassword?id={id}");
            }
            var repo = new ImageRepository(_connectionString);
            Image image = repo.GetImage(id);
            repo.UpdateView(id);
            return View(new ImageViewModel
            {
                Image = image
            });
        }
        public IActionResult AddImage(int id, string password)
        {
            var repo = new ImageRepository(_connectionString);
            string correctPassword = repo.GetPassword(id);
            var imageIds = HttpContext.Session.Get<List<int>>(ImageIdsSessionName);
            if (imageIds == null)
            {
                imageIds = new List<int>();
            }

            if (password == correctPassword)
            {
                imageIds.Add(id);
                HttpContext.Session.Set(ImageIdsSessionName, imageIds);
                return Redirect($"/home/viewimage?id={id}");
            }
            else
            {
                TempData["incorrectPassword"] = "Invalid Password! Please try again!";
                return Redirect($"/home/enterpassword?id={id}");
            }

        }

    }
}
