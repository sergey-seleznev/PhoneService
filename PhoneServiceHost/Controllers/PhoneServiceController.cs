using Microsoft.AspNetCore.Mvc;

namespace Proekspert.PhoneServiceTask.Host.Controllers
{
    public class PhoneServiceController : Controller
    {
        private readonly IPhoneService _phoneService;

        public PhoneServiceController(IPhoneService phoneService)
        {
            _phoneService = phoneService;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        [ResponseCache(NoStore = true, Duration = 0)]
        public IActionResult Data()
        {
            return new ObjectResult(_phoneService.GetCurrentData());
        }
    }
}
