using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

using DFWeb.BE.Provider;
using DFWeb.FR.Models;
using DFCommonLib.Utils;
using AccountCommon.SharedModel;

namespace DarkFactorCoreNet.Pages
{
    public class LoginModel : BasePageModel
    {
        public LoginModel(ILoginProvider loginProvider,
            IPageProvider pageProvider, 
            IMenuProvider menuProvider,
            IImageProvider imageProvider) : base(pageProvider,menuProvider, loginProvider, imageProvider)
        {
            _loginProvider = loginProvider;
        }

        public void OnGet(int id = 0)
        {
            int menuId = id == 0 ? menuProvider.GetDefaultId() : id;
            GetMenuData(menuId);
            PageId = menuId;
            EditUrl = "/Editor/EditMainPage";
        }

        public IActionResult OnPostAsync([FromForm] String username, String password)
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                return Redirect("/Login/LoginFailed");
            }

            var encryptedPassword = DFCrypt.EncryptInput(password);
            var errorCode = _loginProvider.LoginUser(username, encryptedPassword);

            if (errorCode == AccountData.ErrorCode.OK)
            {
                return Redirect("/");
            }

            return Redirect("/Login/LoginFailed");
        }
    }
}
