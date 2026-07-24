
using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Core;

using DFWeb.BE.Models;
using DFWeb.BE.Provider;

using DFCommonLib.Utils;
using AccountCommon.SharedModel;
using System.Net;
using System.Linq;

namespace DFWeb.BE.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        ILoginProvider _loginProvider;

        public LoginController(ILoginProvider loginProvider)
        {
            _loginProvider = loginProvider;
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
            if (string.IsNullOrWhiteSpace(email))
            {
                return Redirect("/Login/ChangePassStep1?msg=" + WebUtility.UrlEncode("Please enter a valid email address"));
            }

            var ret = _loginProvider.ResetPasswordWithEmail(email);
            if (ret == null)
            {
                return Redirect("/Login/ChangePassStep1?msg=" + WebUtility.UrlEncode("Unable to process password reset request"));
            }

            if ( ret.errorCode == (int)ReturnData.ReturnCode.OK )
            {
                return Redirect("/Login/ChangePassStep2");
            }
            return Redirect("/Login/ChangePassStep1?msg=" + WebUtility.UrlEncode(ret.message));
        }

        [HttpPost]
        [Route("ChangePassStep2")]
        public IActionResult ChangePassStep2([FromForm] string code)
        {
            if (string.IsNullOrWhiteSpace(code))
            {
                return Redirect("/Login/ChangePassStep2?msg=" + WebUtility.UrlEncode("Please enter your security code"));
            }

            var ret = _loginProvider.ResetPasswordWithCode(code);
            if (ret == null)
            {
                return Redirect("/Login/ChangePassStep2?msg=" + WebUtility.UrlEncode("Unable to verify security code"));
            }

            if ( ret.errorCode == (int)ReturnData.ReturnCode.OK )
            {
                return Redirect("/Login/ChangePassStep3");
            }
            return Redirect("/Login/ChangePassStep2?msg=" + WebUtility.UrlEncode(ret.message));
        }

        [HttpPost]
        [Route("ChangePassStep3")]
        public IActionResult ChangePassStep3([FromForm] string password, [FromForm] string password2)
        {
            if ( string.IsNullOrEmpty( password ) || string.IsNullOrEmpty( password2 ) || !password.Equals(password2) )
            {
                return Redirect("/Login/ChangePassStep3?msg=" + WebUtility.UrlEncode("Passwords do not match"));
            }

            if (password.Length < 8 || password.Count(char.IsDigit) < 2)
            {
                return Redirect("/Login/ChangePassStep3?msg=" + WebUtility.UrlEncode("Password must be at least 8 characters and contain at least 2 digits"));
            }

            var ret = _loginProvider.ResetPasswordWithToken(password);
            if (ret == null)
            {
                return Redirect("/Login/ChangePassStep3?msg=" + WebUtility.UrlEncode("Unable to set a new password"));
            }

            if ( ret.errorCode == (int)ReturnData.ReturnCode.OK )
            {
                _loginProvider.Logout();
                return Redirect("/Admin/Login?msg=" + WebUtility.UrlEncode("Password changed successfully"));
            }

            return Redirect("/Login/ChangePassStep3?msg=" + WebUtility.UrlEncode(ret.message));
       }
    }
}