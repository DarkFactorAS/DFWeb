
using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Core;

using DFWeb.BE.Models;
using DFWeb.BE.Provider;

using DFCommonLib.Utils;
using AccountCommon.SharedModel;

namespace DFWeb.BE.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        ILoginProvider _loginProvider;
        IEmailProvider _emailProvider;

        public LoginController(ILoginProvider loginProvider, IEmailProvider emailProvider)
        {
            _loginProvider = loginProvider;
            _emailProvider = emailProvider;
        }

        [HttpPost]
        [Route("LoginUser")]
        public IActionResult LoginUser([FromForm] string username, [FromForm] string password)
        {
            var encryptedPassword = DFCrypt.EncryptInput(password);
            var errorCode = _loginProvider.LoginUser(username, encryptedPassword);
            switch(errorCode)
            {
                case AccountData.ErrorCode.UserDoesNotExist:
                    return Redirect("/Login/LoginFailed");
                case AccountData.ErrorCode.WrongPassword:
                    return Redirect("/Login/LoginFailed");
                case AccountData.ErrorCode.OK:
                    return Redirect("/");
                default:
                    return Redirect("/");
           }
        }

        [HttpPost]
        [Route("Logout")]
        public IActionResult LogoutUser()
        {
            _loginProvider.Logout();
            return Redirect("/");
        }

        [HttpPost]
        [Route("ChangePassStep1")]
        public IActionResult ChangePassStep1([FromForm] string email)
        {
            var ret = _loginProvider.ResetPasswordWithEmail(email);
            if ( ret.errorCode == (int)ReturnData.ReturnCode.OK )
            {
                return Redirect("/Login/ChangePassStep2");
            }
            return Redirect("/Login/ChangePassStep1?msg=" + ret.message);
        }

        [HttpPost]
        [Route("ChangePassStep2")]
        public IActionResult ChangePassStep2([FromForm] string code)
        {
            var ret = _loginProvider.ResetPasswordWithCode(code);
            if ( ret.errorCode == (int)ReturnData.ReturnCode.OK )
            {
                return Redirect("/Login/ChangePassStep3");
            }
            return Redirect("/Login/ChangePassStep1?msg=" + ret.message);
        }

        [HttpPost]
        [Route("ChangePassStep3")]
        public IActionResult ChangePassStep3([FromForm] string password, [FromForm] string password2)
        {
            if ( string.IsNullOrEmpty( password ) || string.IsNullOrEmpty( password2 ) || !password.Equals(password2) )
            {
                return Redirect("/Login/ChangePassStep3?msg=Passwords do not match");
            }

            var ret = _loginProvider.ResetPasswordWithToken(password);
            if ( ret.errorCode == (int)ReturnData.ReturnCode.OK )
            {
                _loginProvider.Logout();

                // TODO: Go to login page
                return Redirect("/");
            }

            return Redirect("/Login/ChangePassStep1");
       }
    }
}