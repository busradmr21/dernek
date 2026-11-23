using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SigortaDernegi.Models.Account;

namespace SigortaDernegi.Controllers.Account
{
	public class AccountController : Controller
	{
        public ActionResult Giris()
        {

            // 1. Durum: Normal Ziyaretçi
            return View();
        }
    }
}
