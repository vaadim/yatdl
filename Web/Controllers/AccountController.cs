using System.Web.Mvc;
using YATDL.Models.Account;
using YATDL.Security;

namespace YATDL.Controllers
{
    [Authorize]
    public class AccountController : YATDLControllerBase
    {

        //
        // GET: /Account/Login

        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        //
        // POST: /Account/Login

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginModel model, string returnUrl)
        {
            if (!ModelState.IsValid)
                return View(model);

            if (WebSecurity.Login(model.UserName, model.Password, false) == WebSecurity.MembershipLoginStatus.Success)
                return RedirectToLocal(returnUrl);

            // If we got this far, something failed, redisplay form
            ModelState.AddModelError(string.Empty, "Указан неверный пароль.");
            return View(model);
        }

        //
        // POST: /Account/LogOff

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            WebSecurity.Logout();

            return RedirectToAction("Index", "Home");
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);
            
            return RedirectToAction("Index", "Home");
        }

    }
}