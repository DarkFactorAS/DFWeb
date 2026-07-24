using System.Net;
using AccountCommon.SharedModel;
using DarkFactorCoreNet;
using DFWeb.BE.Models;
using DFWeb.BE.Provider;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace DFWeb.FR.Tests;

public class LoginFailedPageIntegrationTests
{
    [Fact]
    public async Task LoginFailedPage_RendersFormPostingToLoginUserEndpoint()
    {
        using var server = BuildServer();
        using var client = server.CreateClient();

        var response = await client.GetAsync("/Login/LoginFailed");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var html = await response.Content.ReadAsStringAsync();
        Assert.Contains("action=\"/api/Login/LoginUser\"", html);
    }

    [Fact]
    public async Task ChangePassStep1_RendersCorrectFormActionAndPreviousLink()
    {
        using var server = BuildServer();
        using var client = server.CreateClient();

        var response = await client.GetAsync("/Login/ChangePassStep1");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var html = await response.Content.ReadAsStringAsync();
        Assert.Contains("action=\"/api/Login/ChangePassStep1\"", html);
        Assert.Contains("href=\"/Login/LoginFailed\"", html);
    }

    [Fact]
    public async Task ChangePassStep2_RendersCorrectFormActionAndPreviousLink()
    {
        using var server = BuildServer();
        using var client = server.CreateClient();

        var response = await client.GetAsync("/Login/ChangePassStep2");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var html = await response.Content.ReadAsStringAsync();
        Assert.Contains("action=\"/api/Login/ChangePassStep2\"", html);
        Assert.Contains("href=\"/Login/ChangePassStep1\"", html);
    }

    [Fact]
    public async Task ChangePassStep3_RendersCorrectFormActionAndPreviousLink()
    {
        using var server = BuildServer();
        using var client = server.CreateClient();

        var response = await client.GetAsync("/Login/ChangePassStep3");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var html = await response.Content.ReadAsStringAsync();
        Assert.Contains("action=\"/api/Login/ChangePassStep3\"", html);
        Assert.Contains("href=\"/Login/ChangePassStep2\"", html);
    }

    private static TestServer BuildServer()
    {
        var contentRoot = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "../../../../DFWeb.FR.BootStrap"));

        var webHostBuilder = new WebHostBuilder()
            .UseContentRoot(contentRoot)
            .ConfigureServices(services =>
            {
                services
                    .AddRazorPages()
                    .AddApplicationPart(typeof(Program).Assembly);

                services.AddSession();
                services.AddHttpContextAccessor();

                services.AddSingleton<IPageProvider, FakePageProvider>();
                services.AddSingleton<IMenuProvider, FakeMenuProvider>();
                services.AddSingleton<ILoginProvider, FakeLoginProvider>();
                services.AddSingleton<IImageProvider, FakeImageProvider>();
            })
            .Configure(app =>
            {
                app.UseRouting();
                app.UseSession();
                app.UseEndpoints(endpoints =>
                {
                    endpoints.MapRazorPages();
                });
            });

        return new TestServer(webHostBuilder);
    }

    private sealed class FakePageProvider : IPageProvider
    {
        public int GetMainPageId() => 1;

        public PageContentModel GetPage(int pageId)
        {
            return new PageContentModel
            {
                ContentTitle = "Login",
                RelatedTags = string.Empty
            };
        }

        public List<TeaserPageContentModel> GetPagesWithParentId(int parentId) => new();

        public List<TeaserPageContentModel> GetPagesWithTag(string tag) => new();

        public List<TeaserPageContentModel> GetNewArticles(int maxArticles) => new();

        public List<string> GetRelatedTags(int pageId) => new();

        public IList<ArticleSectionModel> GetArticleSections(int pageId) => new List<ArticleSectionModel>();
    }

    private sealed class FakeMenuProvider : IMenuProvider
    {
        public int GetDefaultId() => 1;

        public List<MenuItem> GetTree(int pageId)
        {
            return new()
            {
                new MenuItem { ID = 1, ParentID = 0, Name = "Home", IsPublished = true }
            };
        }

        public List<MenuItem> SelectItem(int selectedItemId)
        {
            return new()
            {
                new MenuItem { ID = 1, ParentID = 0, Name = "Home", IsPublished = true }
            };
        }
    }

    private sealed class FakeLoginProvider : ILoginProvider
    {
        public UserInfoModel GetLoginInfo()
        {
            return new UserInfoModel
            {
                IsLoggedIn = false,
                UserAccessLevel = 0,
                Handle = string.Empty
            };
        }

        public AccountData.ErrorCode LoginUser(string username, string password) => AccountData.ErrorCode.OK;

        public void Logout() { }

        public ReturnData ResetPasswordWithEmail(string email) => new();

        public ReturnData ResetPasswordWithCode(string code) => new();

        public ReturnData ResetPasswordWithToken(string password) => new();
    }

    private sealed class FakeImageProvider : IImageProvider
    {
        public Task<uint> UploadImage(int pageId, List<IFormFile> files) => Task.FromResult(0u);

        public Task<bool> UpdateImageData(int imageId, List<IFormFile> files) => Task.FromResult(false);

        public bool DeleteImage(int imageId) => false;

        public ImageModel GetImage(int imageId) => new();

        public byte[] GetRawImage(int imageId) => Array.Empty<byte>();

        public IList<ImageModel> GetImages(int imagesPrPage, int pageNumber) => new List<ImageModel>();

        public bool UpdateImage(int imageId, string filename) => false;
    }
}
