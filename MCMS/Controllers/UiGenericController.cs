using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

namespace MCMS.Controllers
{
    public abstract class UiGenericController<T> : Controller 
        where T : class, new()
    {
        //TODO: Replace with DAL
        private IEnumerable<T> modelList = new List<T>();
        
        [HttpGet]
        public IActionResult Index()
        {
            //TODO: use DAL
            return View(modelList);
        }
        
        [HttpGet]
        public IActionResult Add()
        {
            //TODO: use DAL
            return PartialView("_Add");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Add(string id, T model)
        {
            //TODO: Use DAL
            return Ok();
        }
        
        [HttpGet]
        public IActionResult Edit()
        {
            //TODO: use DAL
            var model = new T();
            return PartialView("_Edit" , model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(string id, T model)
        {
            //TODO: Use DAL
            return Ok();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(string id, T model)
        {
            //TODO: Use DAL
            return Ok();
        }
    }
}