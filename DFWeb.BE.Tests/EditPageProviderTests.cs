using System;
using System.Collections.Generic;
using DFWeb.BE.Models;
using DFWeb.BE.Provider;
using DFWeb.BE.Repository;
using Xunit;

namespace DFWeb.BE.Tests;

public class EditPageProviderTests
{
    [Fact]
    public void SaveFullPage_UpdatesEditableFieldsAndPersistsPage()
    {
        var storedPage = new PageContentModel
        {
            PageId = 7,
            ParentId = 3,
            ImageId = 12,
            SortId = 4,
            Acl = 1,
            ContentTitle = "Original title",
            ContentText = "Original body",
            PromoText = "Original promo",
            Tags = "old",
            RelatedTags = "older"
        };
        var pageProvider = new FakePageProvider { StoredPage = storedPage };
        var pageRepository = new FakePageRepository { StoredPage = storedPage };
        var editRepository = new FakeEditPageRepository();
        var provider = new EditPageProvider(
            pageProvider,
            editRepository,
            new FakeUserSessionProvider(),
            new FakeLoginRepository(),
            pageRepository);

        var page = new PageContentModel
        {
            PageId = 7,
            ContentTitle = "Updated title",
            ContentText = "Updated body",
            PromoText = "Updated promo",
            Tags = "tag-a,tag-b",
            RelatedTags = "related-a"
        };

        var saved = provider.SaveFullPage(page);

        Assert.True(saved);
        Assert.NotNull(editRepository.SavedPage);
        Assert.Same(storedPage, editRepository.SavedPage);
        Assert.Equal("Updated title", editRepository.SavedPage!.ContentTitle);
        Assert.Equal("Updated body", editRepository.SavedPage.ContentText);
        Assert.Equal("Updated promo", editRepository.SavedPage.PromoText);
        Assert.Equal("tag-a,tag-b", editRepository.SavedPage.Tags);
        Assert.Equal("related-a", editRepository.SavedPage.RelatedTags);
        Assert.Equal(3, editRepository.SavedPage.ParentId);
        Assert.Equal(12, editRepository.SavedPage.ImageId);
        Assert.Equal(4, editRepository.SavedPage.SortId);
        Assert.Equal(1, editRepository.SavedPage.Acl);
    }

    [Fact]
    public void SaveFullPage_ReturnsFalseWhenUserCannotEditPage()
    {
        var storedPage = new PageContentModel { PageId = 7 };
        var editRepository = new FakeEditPageRepository();
        var provider = new EditPageProvider(
            new FakePageProvider { StoredPage = storedPage },
            editRepository,
            new FakeUserSessionProvider { CanEdit = false },
            new FakeLoginRepository(),
            new FakePageRepository { StoredPage = storedPage });

        var saved = provider.SaveFullPage(new PageContentModel { PageId = 7, ContentTitle = "Updated title" });

        Assert.False(saved);
        Assert.Null(editRepository.SavedPage);
    }

    private sealed class FakePageProvider : IPageProvider
    {
        public PageContentModel StoredPage { get; set; } = new();

        public int GetMainPageId() => 0;

        public PageContentModel GetPage(int pageId) => StoredPage;

        public List<TeaserPageContentModel> GetPagesWithParentId(int parentId) => new();

        public List<TeaserPageContentModel> GetPagesWithTag(string tag) => new();

        public List<TeaserPageContentModel> GetNewArticles(int maxArticles) => new();

        public List<string> GetRelatedTags(int pageId) => new();

        public IList<ArticleSectionModel> GetArticleSections(int pageId) => new List<ArticleSectionModel>();
    }

    private sealed class FakePageRepository : IPageRepository
    {
        public PageContentModel StoredPage { get; set; } = new();

        public int GetMainPageId() => 0;

        public PageContentModel GetPage(int pageId) => StoredPage;

        public List<TeaserPageContentModel> GetPagesWithParentId(int parentId) => new();

        public List<TeaserPageContentModel> GetPagesWithTag(string tag) => new();

        public List<TeaserPageContentModel> GetNewArticles(int maxArticles) => new();

        public List<TagModel> GetTagsForPage(int pageId) => new();

        public List<string> GetRelatedTags(int pageId) => new();

        public IList<ArticleSectionModel> GetArticleSections(int pageId) => new List<ArticleSectionModel>();
    }

    private sealed class FakeEditPageRepository : IEditPageRepository
    {
        public PageContentModel? SavedPage { get; private set; }

        public bool SavePage(PageContentModel pageModel)
        {
            SavedPage = pageModel;
            return true;
        }

        public bool DeletePage(int pageId) => false;

        public bool CreatePageWithParent(int parentPageId, string pageTotle, int sortId) => false;

        public int GetArticleSectionMaxSortId(int pageId) => 0;

        public bool CreateArticleSection(int pageId, string title, string content, int sortId) => false;

        public bool UpdateArticleSection(ArticleSectionModel articleSectionModel) => false;

        public bool DeleteArticleSection(ArticleSectionModel articleSectionModel) => false;

        public bool ChangeSectionLayout(int articleId, int layout) => false;

        public bool AddImage(int pageID, uint imageId) => false;

        public bool AddImageToSection(int sectionId, uint imageId) => false;

        public bool ChangeAccess(int pageId, int accessLevel) => false;
    }

    private sealed class FakeUserSessionProvider : IUserSessionProvider
    {
        public bool CanEdit { get; set; } = true;

        public void RemoveSession() { }

        public void SetUser(UserModel user) { }

        public UserModel GetUser() => new();

        public string GetUsername() => string.Empty;

        public string GetToken() => string.Empty;

        public bool IsLoggedIn() => true;

        public bool CanEditPage() => CanEdit;
    }

    private sealed class FakeLoginRepository : ILoginRepository
    {
        public AccessLevel GetAccessForUser(string username) => AccessLevel.Editor;
    }
}