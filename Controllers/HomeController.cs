using Microsoft.AspNetCore.Mvc;

namespace Travel.Controllers;

public class HomeController : Controller
{
    // Khi gọi vào URL "/" (Trang chủ), hàm này sẽ kích hoạt và tìm đến View tương ứng
    public IActionResult Index()
    {
        return View();
    }
}