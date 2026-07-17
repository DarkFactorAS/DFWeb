
using System;
using System.Security.Cryptography;
using System.Linq;

using Microsoft.AspNetCore.Cryptography.KeyDerivation;

using DFCommonLib.Config;
using DFCommonLib.Utils;
using AccountClientModule.Client;
using AccountCommon.SharedModel;

using DFWeb.BE.Models;
using DFWeb.BE.Repository;
using DFWeb.BE.ConfigModel;

namespace DFWeb.BE.Provider
{
    public interface ILoginProvider
    {
        UserInfoModel GetLoginInfo();
        AccountData.ErrorCode LoginUser(string username, string password);
        void Logout();
        
        ReturnData ResetPasswordWithEmail(string email);
        ReturnData ResetPasswordWithCode(string code);
        ReturnData ResetPasswordWithToken(string password);
    }

    public class LoginProvider : ILoginProvider
    {
        IUserSessionProvider _userSession;
        ILoginRepository _loginRepository;
        IAccountClient _accountClient;
        ICookieProvider _cookieProvider;

        const string COOKIE_NAME = "DFToken";
        const string RESET_EMAIL_COOKIE = "DFResetEmail";
        const string RESET_TOKEN_COOKIE = "DFResetToken";

        public LoginProvider(IUserSessionProvider userSession, 
                            IAccountClient accountClient,
                            ILoginRepository loginRepository,
                            ICookieProvider cookieProvider)
        {
            _userSession = userSession;
            _accountClient = accountClient;
            _loginRepository = loginRepository;
            _cookieProvider = cookieProvider;

            IConfigurationHelper configuration = DFServices.GetService<IConfigurationHelper>();
            var customer = configuration.Settings as WebConfig;
            if ( customer != null )
            {
                _accountClient.SetEndpoint(customer.AccountServer?.Endpoint);
            }
        }

        public UserInfoModel GetLoginInfo()
        {
            UserInfoModel userInfo = new UserInfoModel();

            var user = GetLoggedInUser();
            if ( user != null )
            {
                userInfo.IsLoggedIn = user.IsLoggedIn;
                userInfo.UserAccessLevel = (int) user.UserAccessLevel; 
                userInfo.Handle = user.Username;
                return userInfo;
            }

            var loginToken = _cookieProvider.GetCookie(COOKIE_NAME);
            if ( !string.IsNullOrEmpty(loginToken) )
            {
                LoginTokenData loginTokenData = new LoginTokenData()
                {
                    token = loginToken
                };
                var accountData = _accountClient.LoginToken(loginTokenData);
                var loggedInUser = SetLoggedInAccount(accountData);
                if ( loggedInUser != null )
                {
                    return loggedInUser;
                }
            }

            return userInfo;
        }
        
        public UserModel GetLoggedInUser()
        {
            return _userSession.GetUser();
        }

        public AccountData.ErrorCode LoginUser(string username, string password)
        {
            LoginData loginData = new LoginData()
            {
                username = username,
                password = password
            };
            var accountData = _accountClient.LoginAccount(loginData);
            SetLoggedInAccount(accountData);
            return accountData.errorCode;
        }

        private UserInfoModel SetLoggedInAccount(AccountData accountData)
        {
            if  ( accountData.errorCode == AccountData.ErrorCode.OK )
            {
                AccessLevel accessLevel = _loginRepository.GetAccessForUser(accountData.nickname);

                UserModel userModel = new UserModel()
                {
                    Username = accountData.nickname,
                    IsLoggedIn = true,
                    UserAccessLevel = accessLevel,
                    Token = accountData.token
                };

                _userSession.SetUser(userModel);
                _cookieProvider.RemoveCookie(COOKIE_NAME);
                _cookieProvider.SetCookie(COOKIE_NAME, accountData.token);

                UserInfoModel userInfo = new UserInfoModel()
                {
                    IsLoggedIn = true,
                    UserAccessLevel = (int) userModel.UserAccessLevel,
                    Handle = userModel.Username
                };
                return userInfo;
            }
            _userSession.RemoveSession();
//            _cookieProvider.RemoveCookie(COOKIE_NAME);
            return null;
        }


        public void Logout()
        {
            _userSession.RemoveSession();
            _cookieProvider.RemoveCookie(COOKIE_NAME);
        }

        public ReturnData ResetPasswordWithEmail(string email)
        {
            _cookieProvider.RemoveCookie(RESET_TOKEN_COOKIE);

            var response = _accountClient.ResetPasswordWithEmail(email);
            if (response != null && response.errorCode == (int)ReturnData.ReturnCode.OK)
            {
                _cookieProvider.SetCookie(RESET_EMAIL_COOKIE, email);
            }
            return response;
        }

        public ReturnData ResetPasswordWithCode(string code)
        {
            var email = _cookieProvider.GetCookie(RESET_EMAIL_COOKIE);
            var response = _accountClient.ResetPasswordWithCode(code, email);
            if (response != null && response.errorCode == (int)ReturnData.ReturnCode.OK)
            {
                _cookieProvider.SetCookie(RESET_TOKEN_COOKIE, response.message);
            }
            return response;
        }

        public ReturnData ResetPasswordWithToken(string password)
        {
            var token = _cookieProvider.GetCookie(RESET_TOKEN_COOKIE);
            var response = _accountClient.ResetPasswordWithToken(token, password);
            if (response != null && response.errorCode == (int)ReturnData.ReturnCode.OK)
            {
                _cookieProvider.RemoveCookie(RESET_EMAIL_COOKIE);
                _cookieProvider.RemoveCookie(RESET_TOKEN_COOKIE);
            }
            return response;
        }
    }
}