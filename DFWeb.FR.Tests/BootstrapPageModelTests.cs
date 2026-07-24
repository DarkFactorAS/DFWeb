using DarkFactorCoreNet.Pages;
using DFWeb.BE.Models;
using DFWeb.BE.Provider;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using AccountCommon.SharedModel;
using DFCommonLib.Utils;
using Xunit;

namespace DFWeb.FR.Tests;

public class BootstrapPageModelTests
{
    [Fact]
    public void IndexModel_OnGet_UsesMainPageAndLoadsNewArticles()
    {
        var pageProvider = new FakePageProvider
        {
            MainPageId = 42,
            MainPage = new PageContentModel { ContentTitle = "Home", RelatedTags = "alpha beta" },
            Articles = new List<TeaserPageContentModel>
            {
                new() { PageId = 100 },
                new() { PageId = 101 }
            },
            PagesByTag = new Dictionary<string, List<TeaserPageContentModel>>
            {
                ["alpha"] = new List<TeaserPageContentModel> { new() { PageId = 200 } },
                ["beta"] = new List<TeaserPageContentModel>()
            }
        };

        var model = new IndexModel(
            pageProvider,
            new FakeMenuProvider(),
            new FakeLoginProvider(),
            new FakeImageProvider());

        model.OnGet(id: 999);

        Assert.Equal(42, pageProvider.LastGetPageId);
        Assert.Equal(10, pageProvider.LastNewArticleLimit);
        Assert.Equal(42, model.PageId);
        Assert.Equal("/Editor/EditMainPage", model.EditUrl);
        Assert.Equal(2, model.mainPageItems.Count);
    }

    [Fact]
    public void LoginModel_OnPostAsync_LogsInAndRedirectsToRoot()
    {
        var loginProvider = new FakeLoginProvider();
        var model = new LoginModel(
            loginProvider,
            new FakePageProvider(),
            new FakeMenuProvider(),
            new FakeImageProvider());

        var result = model.OnPostAsync("thor", "secret");

        var redirect = Assert.IsType<RedirectResult>(result);
        Assert.Equal("/", redirect.Url);
        Assert.Equal("thor", loginProvider.LastUsername);
        Assert.Equal(DFCrypt.EncryptInput("secret"), loginProvider.LastPassword);
    }

    [Fact]
    public void LoginModel_OnPostAsync_LoginFailure_RedirectsToLoginFailed()
    {
        var loginProvider = new FakeLoginProvider
        {
            LoginUserResult = AccountData.ErrorCode.WrongPassword
        };
        var model = new LoginModel(
            loginProvider,
            new FakePageProvider(),
            new FakeMenuProvider(),
            new FakeImageProvider());

        var result = model.OnPostAsync("thor", "secret");

        var redirect = Assert.IsType<RedirectResult>(result);
        Assert.Equal("/Login/LoginFailed", redirect.Url);
    }

    [Fact]
    public void LoginModel_OnGet_InitializesMenuStateAndEditUrl()
    {
        var menuProvider = new FakeMenuProvider
        {
            DefaultId = 7
        };

        var model = new LoginModel(
            new FakeLoginProvider(),
            new FakePageProvider(),
            menuProvider,
            new FakeImageProvider());

        model.OnGet();

        Assert.Equal(7, menuProvider.LastSelectedId);
        Assert.Equal("/Editor/EditMainPage", model.EditUrl);
    }

    private sealed class FakePageProvider : IPageProvider
    {
        public int MainPageId { get; set; } = 1;
        public int LastGetPageId { get; private set; }
        public int LastNewArticleLimit { get; private set; }
        public PageContentModel MainPage { get; set; } = new();
        public List<TeaserPageContentModel> Articles { get; set; } = new();
        public Dictionary<string, List<TeaserPageContentModel>> PagesByTag { get; set; } = new();

        public int GetMainPageId() => MainPageId;

        public PageContentModel GetPage(int pageId)
        {
            LastGetPageId = pageId;
            return MainPage;
        }

        public List<TeaserPageContentModel> GetPagesWithParentId(int parentId) => new();

        public List<TeaserPageContentModel> GetPagesWithTag(string tag)
        {
            if (PagesByTag.TryGetValue(tag, out var pages))
            {
                return pages;
            }

            return new List<TeaserPageContentModel>();
        }

        public List<TeaserPageContentModel> GetNewArticles(int maxArticles)
        {
            LastNewArticleLimit = maxArticles;
            return Articles;
        }

        public List<string> GetRelatedTags(int pageId) => new();

        public IList<ArticleSectionModel> GetArticleSections(int pageId) => new List<ArticleSectionModel>();
    }

    private sealed class FakeMenuProvider : IMenuProvider
    {
        public int DefaultId { get; set; } = 1;
        public int LastSelectedId { get; private set; }

        public int GetDefaultId() => DefaultId;

        public List<MenuItem> GetTree(int pageId) => new();

        public List<MenuItem> SelectItem(int selectedItemId)
        {
            LastSelectedId = selectedItemId;
            return new();
        }
    }

    private sealed class FakeLoginProvider : ILoginProvider
    {
        public string LastUsername { get; private set; } = string.Empty;
        public string LastPassword { get; private set; } = string.Empty;
        public AccountData.ErrorCode LoginUserResult { get; set; } = AccountData.ErrorCode.OK;

        public UserInfoModel GetLoginInfo() => new() { IsLoggedIn = false, UserAccessLevel = 0, Handle = string.Empty };

        public AccountData.ErrorCode LoginUser(string username, string password)
        {
            LastUsername = username;
            LastPassword = password;
            return LoginUserResult;
        }

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
