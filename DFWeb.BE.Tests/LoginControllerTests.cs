using AccountCommon.SharedModel;
using DFWeb.BE.Api;
using DFWeb.BE.Models;
using DFWeb.BE.Provider;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace DFWeb.BE.Tests;

public class LoginControllerTests
{
    [Fact]
    public void ChangePassStep1_EmptyEmail_RedirectsWithValidationMessage()
    {
        var provider = new FakeLoginProvider();
        var controller = new LoginController(provider);

        var result = controller.ChangePassStep1(string.Empty);

        var redirect = Assert.IsType<RedirectResult>(result);
        Assert.Equal("/Login/ChangePassStep1?msg=Please+enter+a+valid+email+address", redirect.Url);
    }

    [Fact]
    public void ChangePassStep1_NullProviderResponse_RedirectsWithFailureMessage()
    {
        var provider = new FakeLoginProvider { ResetWithEmailResult = null };
        var controller = new LoginController(provider);

        var result = controller.ChangePassStep1("user@example.com");

        var redirect = Assert.IsType<RedirectResult>(result);
        Assert.Equal("/Login/ChangePassStep1?msg=Unable+to+process+password+reset+request", redirect.Url);
    }

    [Fact]
    public void ChangePassStep2_EmptyCode_RedirectsWithValidationMessage()
    {
        var provider = new FakeLoginProvider();
        var controller = new LoginController(provider);

        var result = controller.ChangePassStep2(" ");

        var redirect = Assert.IsType<RedirectResult>(result);
        Assert.Equal("/Login/ChangePassStep2?msg=Please+enter+your+security+code", redirect.Url);
    }

    [Fact]
    public void ChangePassStep3_WeakPassword_RedirectsWithPolicyMessage()
    {
        var provider = new FakeLoginProvider();
        var controller = new LoginController(provider);

        var result = controller.ChangePassStep3("weakpass", "weakpass");

        var redirect = Assert.IsType<RedirectResult>(result);
        Assert.Equal("/Login/ChangePassStep3?msg=Password+must+be+at+least+8+characters+and+contain+at+least+2+digits", redirect.Url);
    }

    [Fact]
    public void ChangePassStep3_NullProviderResponse_RedirectsWithFailureMessage()
    {
        var provider = new FakeLoginProvider { ResetWithTokenResult = null };
        var controller = new LoginController(provider);

        var result = controller.ChangePassStep3("valid12x", "valid12x");

        var redirect = Assert.IsType<RedirectResult>(result);
        Assert.Equal("/Login/ChangePassStep3?msg=Unable+to+set+a+new+password", redirect.Url);
    }

    private sealed class FakeLoginProvider : ILoginProvider
    {
        public ReturnData? ResetWithEmailResult { get; set; } = new ReturnData
        {
            errorCode = (int)ReturnData.ReturnCode.OK,
            message = string.Empty
        };

        public ReturnData? ResetWithCodeResult { get; set; } = new ReturnData
        {
            errorCode = (int)ReturnData.ReturnCode.OK,
            message = string.Empty
        };

        public ReturnData? ResetWithTokenResult { get; set; } = new ReturnData
        {
            errorCode = (int)ReturnData.ReturnCode.OK,
            message = string.Empty
        };

        public UserInfoModel GetLoginInfo() => new();

        public AccountData.ErrorCode LoginUser(string username, string password) => AccountData.ErrorCode.OK;

        public void Logout() { }

        public ReturnData ResetPasswordWithEmail(string email) => ResetWithEmailResult!;

        public ReturnData ResetPasswordWithCode(string code) => ResetWithCodeResult!;

        public ReturnData ResetPasswordWithToken(string password) => ResetWithTokenResult!;
    }
}
