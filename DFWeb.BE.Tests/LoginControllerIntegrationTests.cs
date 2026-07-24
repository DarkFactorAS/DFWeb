using System.Net;
using System.Net.Http;
using AccountCommon.SharedModel;
using DFWeb.BE.Api;
using DFWeb.BE.Models;
using DFWeb.BE.Provider;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace DFWeb.BE.Tests;

public class LoginControllerIntegrationTests
{
    [Fact]
    public async Task LoginUser_WrongPassword_RedirectsToLoginFailed()
    {
        using var host = BuildHost(new FakeLoginProvider
        {
            LoginResult = AccountData.ErrorCode.WrongPassword
        });

        using var client = host.CreateClient();
        client.DefaultRequestVersion = HttpVersion.Version11;

        var response = await client.PostAsync("/api/Login/LoginUser", new FormUrlEncodedContent(new Dictionary<string, string>
        {
            ["username"] = "thor",
            ["password"] = "secret"
        }));

        Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
        Assert.Equal("/Login/LoginFailed", response.Headers.Location?.ToString());
    }

    [Fact]
    public async Task ChangePassStep1_EmptyEmail_RedirectsWithValidationMessage()
    {
        using var host = BuildHost(new FakeLoginProvider());
        using var client = host.CreateClient();

        var response = await client.PostAsync("/api/Login/ChangePassStep1", new FormUrlEncodedContent(new Dictionary<string, string>
        {
            ["email"] = string.Empty
        }));

        Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
        Assert.Equal("/Login/ChangePassStep1?msg=Please+enter+a+valid+email+address", response.Headers.Location?.ToString());
    }

    [Fact]
    public async Task ChangePassStep3_WeakPassword_RedirectsWithPolicyMessage()
    {
        using var host = BuildHost(new FakeLoginProvider());
        using var client = host.CreateClient();

        var response = await client.PostAsync("/api/Login/ChangePassStep3", new FormUrlEncodedContent(new Dictionary<string, string>
        {
            ["password"] = "weakpass",
            ["password2"] = "weakpass"
        }));

        Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
        Assert.Equal(
            "/Login/ChangePassStep3?msg=Password+must+be+at+least+8+characters+and+contain+at+least+2+digits",
            response.Headers.Location?.ToString());
    }

    private static TestServer BuildHost(ILoginProvider loginProvider)
    {
        var builder = new WebHostBuilder()
            .ConfigureServices(services =>
            {
                services.AddSingleton(loginProvider);
                services.AddControllers().AddApplicationPart(typeof(LoginController).Assembly);
            })
            .Configure(app =>
            {
                app.UseRouting();
                app.UseEndpoints(endpoints =>
                {
                    endpoints.MapControllers();
                });
            });

        return new TestServer(builder);
    }

    private sealed class FakeLoginProvider : ILoginProvider
    {
        public AccountData.ErrorCode LoginResult { get; set; } = AccountData.ErrorCode.OK;

        public UserInfoModel GetLoginInfo() => new();

        public AccountData.ErrorCode LoginUser(string username, string password) => LoginResult;

        public void Logout() { }

        public ReturnData ResetPasswordWithEmail(string email) => new()
        {
            errorCode = (int)ReturnData.ReturnCode.OK,
            message = string.Empty
        };

        public ReturnData ResetPasswordWithCode(string code) => new()
        {
            errorCode = (int)ReturnData.ReturnCode.OK,
            message = string.Empty
        };

        public ReturnData ResetPasswordWithToken(string password) => new()
        {
            errorCode = (int)ReturnData.ReturnCode.OK,
            message = string.Empty
        };
    }
}
