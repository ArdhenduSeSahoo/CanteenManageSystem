using CanteenManage.Models;
using Microsoft.AspNetCore.Mvc;

namespace CanteenManage.Controllers
{
    public class BotAssistantController : Controller
    {
        public IActionResult Index()
        {
            BotAssistantViewDataModel viewDataModel = new BotAssistantViewDataModel();

            return View(viewDataModel);
        }
    }
}
