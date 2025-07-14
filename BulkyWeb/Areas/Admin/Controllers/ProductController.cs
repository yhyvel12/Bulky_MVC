using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models;
using Bulky.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BulkyWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public ProductController(IUnitOfWork db, IWebHostEnvironment webHostEnvironment)
        {
            _unitOfWork = db;
            _webHostEnvironment = webHostEnvironment;
        }

        public IActionResult Index()
        {
            List<Product> objProductList = _unitOfWork.Product.GetAll().ToList();

            return View(objProductList);
        }

        public IActionResult Upsert(int? id) //update and insert
        {
            //model for view
            ProductVM productVM = new ProductVM()
            {
                Product = new Product(),
                //ef projections
                CategoryList = _unitOfWork.Category
                               .GetAll().Select(u => new SelectListItem
                               {
                                   Text = u.Name,
                                   Value = u.Id.ToString()
                               })
            };

            if (id == null || id == 0) //create
                return View(productVM);

            else
            {
                //update
                productVM.Product = _unitOfWork.Product.Get(u => u.Id == id);
                return View(productVM);
            }
        }
        [HttpPost]
        public IActionResult Upsert(ProductVM productVM, IFormFile? file)
        {
            if (ModelState.IsValid)
            {
                string wwwRootPath = _webHostEnvironment.WebRootPath;

                if (file != null)
                {
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName); //random name and extension of file
                    string productPath = Path.Combine(wwwRootPath, @"images\product"); //exact path of file

                    if (string.IsNullOrEmpty(productVM.Product.ImageUrl)) //if excist then we have to delete the old image
                    {
                        var oldImagePath = Path.Combine(wwwRootPath, productVM.Product.ImageUrl.TrimStart('\\')); //in database it's start the \

                        if (System.IO.File.Exists(oldImagePath)) //delete file
                        {
                            System.IO.File.Delete(oldImagePath);
                        }
                    }

                    using (var fileStream = new FileStream(Path.Combine(productPath, fileName), FileMode.Create)) //new image
                    {
                        file.CopyTo(fileStream); //copy file new location
                    }

                    productVM.Product.ImageUrl = @"\images\product\" + fileName;
                }

                if (productVM.Product.Id == 0)
                {
                    _unitOfWork.Product.Add(productVM.Product);
                }
                else
                {
                    _unitOfWork.Product.Update(productVM.Product);
                }

                _unitOfWork.Save();
                TempData["success"] = "Product created successfuly!!";

                return RedirectToAction("Index");
            }
            else
            {
                productVM.CategoryList = _unitOfWork.Category
                                .GetAll().Select(u => new SelectListItem
                                {
                                    Text = u.Name,
                                    Value = u.Id.ToString()
                                });

                return View(productVM);
            }
        }

        //public IActionResult Edit(int? id)
        //{
        //    if (id == null || id == 0)
        //    {
        //        return NotFound();
        //    }

        //    Product? Product = _unitOfWork.Product.Get(u => u.Id == id);
        //    if (Product == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(Product);
        //}
        //[HttpPost]
        //public IActionResult Edit(Product obj)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        _unitOfWork.Product.Update(obj);
        //        _unitOfWork.Save();
        //        TempData["success"] = "Product updated successfuly!!";

        //        return RedirectToAction("Index");
        //    }

        //    return View();
        //}

        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }

            Product? Product = _unitOfWork.Product.Get(u => u.Id == id);
            if (Product == null)
            {
                return NotFound();
            }

            return View(Product);
        }
        [HttpPost, ActionName("Delete")]
        public IActionResult DeletePost(int? id)
        {
            Product? obj = _unitOfWork.Product.Get(u => u.Id == id);
            if (obj == null)
            {
                return NotFound();
            }

            _unitOfWork.Product.Remove(obj);
            _unitOfWork.Save();
            TempData["success"] = "Product deleted successfuly!!";

            return RedirectToAction("Index");
        }
    }
}
