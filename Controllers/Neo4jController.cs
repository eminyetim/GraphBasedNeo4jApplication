
using Microsoft.AspNetCore.Mvc;
using Projem.Models;
using Projem.Services;


namespace Projem.Controllers
{
    public class Neo4jController : Controller
    {
        private readonly Neo4jService _neo4jService;

        public Neo4jController(Neo4jService neo4jService)
        {
            _neo4jService = neo4jService;
        }

        public async Task<IActionResult> ShowData()
        {
            var users = await _neo4jService.GetAllNodes();
            return View(users);
        }

        public async Task<IActionResult> ShowDataNode()
        {
            var users = await _neo4jService.GetAllUsers();
            return View(users);
        }
        public IActionResult Apply()
        {
            return View();
        }

        [HttpPost]//Default(Standart) olarak HttpGet gelir. Burada veri göndereceğimiz anlamına gelir.
        [ValidateAntiForgeryToken] // Güvenlik amaçlı tarayıcıyı doğrular.
        public IActionResult Apply([FromForm] KullaniciSifre model) // Apply fonksiyonu overloading yaptık. 
        {
            _neo4jService.AddNewUser(model);
            return  View("FeedBack",model);
        }

    }
}


