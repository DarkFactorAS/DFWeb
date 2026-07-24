using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;

using DFWeb.BE.Models;
using DFWeb.BE.Repository;

namespace DFWeb.BE.Provider
{
    public interface IEditPageProvider
    {
        bool CreatePage( int pageId, string pageTitle );
        bool SaveMainPage(PageContentModel mainPage);
        bool CreateChildPage( int parentPageId, string pageTitle );
        bool SaveFullPage(PageContentModel pageModel);
        bool CreateArticleSection(int pageId, string title, string content);
        bool UpdateArticleSection(ArticleSectionModel articleSectionModel);
        bool DeleteArticleSection(ArticleSectionModel articleSectionModel);
        bool ChangeSectionLayout(int articleId, int layout);
        bool MovePageUp(TeaserPageContentModel page);
        bool MovePageDown(TeaserPageContentModel page);
        bool CanEditPage();
        bool AddImage(int pageID, uint imageId);
        bool AddImageToSection(int sectionId, uint imageId);
        bool DeletePage(int pageId);
        bool ChangeAccess(int pageId, int accessLevel);
    }

    public class EditPageProvider : IEditPageProvider
    {
        private IPageProvider _pageProvider;
        private IPageRepository _pageRepository;
        private IEditPageRepository _editPageRepository;
        private IUserSessionProvider _userSession;
        private ILoginRepository _loginRepository;

        public EditPageProvider(
            IPageProvider pageProvider, 
            IEditPageRepository editPageRepository,
            IUserSessionProvider userSession, 
            ILoginRepository loginRepository,
            IPageRepository pageRepository)
        {
            _pageProvider = pageProvider;
            _editPageRepository = editPageRepository;
            _userSession = userSession;
            _loginRepository = loginRepository;
            _pageRepository = pageRepository;
        }

        public bool CreatePage( int pageId, string pageTitle )
        {
            if ( !CanEditPage() )
            {
                return false;
            }

            var page = _pageProvider.GetPage(pageId);
            if ( page != null )
            {
                return CreateChildPage(page.ParentId,pageTitle);
            }
            return false;
        }

        public bool SaveMainPage(PageContentModel mainPage)
        {
            if ( !CanEditPage() )
            {
                return false;
            }

            var editPage = _pageRepository.GetPage(mainPage.PageId);
            if ( editPage != null )
            {
                editPage.ContentTitle = mainPage.ContentTitle;
                editPage.Acl = mainPage.Acl;
                editPage.RelatedTags = mainPage.RelatedTags;
                return _editPageRepository.SavePage(editPage);
            }
            return false;
        }

        public bool CreateChildPage( int parentPageId, string pageTitle )
        {
            if ( !CanEditPage() )
            {
                return false;
            }

            // Fix sort id
            int sortId = 0;
            List<TeaserPageContentModel> pageList = _pageProvider.GetPagesWithParentId(parentPageId);
            TeaserPageContentModel page = pageList.OrderBy(x => x.SortId).LastOrDefault();
            if ( page != null )
            {
                sortId = page.SortId;
            }

            return _editPageRepository.CreatePageWithParent(parentPageId, pageTitle, sortId);
        }


        public bool SaveFullPage(PageContentModel pageModel)
        {
            if ( !CanEditPage() )
            {
                return false;
            }

            var editPage = _pageRepository.GetPage( pageModel.PageId );
            editPage.PromoText = pageModel.PromoText;
            editPage.ContentText = pageModel.ContentText;
            editPage.ContentTitle = pageModel.ContentTitle;
            editPage.Tags = pageModel.Tags;
            editPage.RelatedTags = pageModel.RelatedTags;
            return _editPageRepository.SavePage(editPage);
        }

        public bool CreateArticleSection(int pageId, string title, string content)
        {
            if ( !CanEditPage() )
            {
                return false;
            }

            int sortId = _editPageRepository.GetArticleSectionMaxSortId(pageId);
            return _editPageRepository.CreateArticleSection(pageId,title,content, sortId + 1 );
        }

        public bool UpdateArticleSection(ArticleSectionModel articleSectionModel)
        {
            if ( !CanEditPage() )
            {
                return false;
            }

            if ( articleSectionModel.PageId == 0 )
            {
                return false;
            }

            if ( articleSectionModel.ID == 0 )
            {
                return false;
            }

            if ( articleSectionModel.Text == null )
            {
                articleSectionModel.Text = "";
            }

            return _editPageRepository.UpdateArticleSection(articleSectionModel);
        }

        public bool DeleteArticleSection(ArticleSectionModel articleSectionModel)
        {
            if ( !CanEditPage() )
            {
                return false;
            }

            if ( articleSectionModel.ID == 0 || articleSectionModel.PageId == 0)
            {
                return false;
            }

            return _editPageRepository.DeleteArticleSection(articleSectionModel);
        }


        public bool ChangeSectionLayout(int articleId, int layout)
        {
            if ( !CanEditPage() )
            {
                return false;
            }
            return _editPageRepository.ChangeSectionLayout(articleId,layout);            
        }


        public bool MovePageUp(TeaserPageContentModel page)
        {
            if ( !CanEditPage() )
            {
                return false;
            }

            if ( page == null )
            {
                return false;
            }

            var pageList = _pageProvider.GetPagesWithParentId(page.ParentId);
            TeaserPageContentModel swapPage = pageList.OrderBy(x => x.SortId).Where(x => x.SortId < page.SortId).LastOrDefault();
            if (swapPage == null)
            {
                return false;
            }

            return SwapSort(page,swapPage);
       }

        public bool MovePageDown(TeaserPageContentModel page)
        {
            if ( !CanEditPage() )
            {
                return false;
            }

            if ( page == null )
            {
                return false;
            }

            var pageList = _pageProvider.GetPagesWithParentId(page.ParentId);
            TeaserPageContentModel swapPage = pageList.OrderBy(x => x.SortId).Where(x => x.SortId > page.SortId).FirstOrDefault();
            if (swapPage == null)
            {
                return false;
            }

            return SwapSort(page,swapPage);
        }

        private bool SwapSort(TeaserPageContentModel page1, TeaserPageContentModel page2)
        {
            var databasePage1 = _pageRepository.GetPage( page1.PageId );
            var databasePage2 = _pageRepository.GetPage( page2.PageId);

            databasePage1.SortId = page2.SortId;
            databasePage2.SortId = page1.SortId;

            bool didSave1 = _editPageRepository.SavePage(databasePage1);
            bool didSave2 = _editPageRepository.SavePage(databasePage2);
            return didSave1 && didSave2;
        }

        public bool CanEditPage()
        {
            return _userSession.CanEditPage();
        }

        public bool AddImage(int pageID, uint imageId)
        {
            if ( !CanEditPage() )
            {
                return false;
            }
            return _editPageRepository.AddImage(pageID,imageId);
        }

        public bool AddImageToSection(int sectionId, uint imageId)
        {
            if ( !CanEditPage() )
            {
                return false;
            }
            return _editPageRepository.AddImageToSection(sectionId,imageId);
        }

        public bool DeletePage(int pageId)
        {
            if ( !CanEditPage() )
            {
                return false;
            }

            // Do not delete page if it has children
            var childList = _pageRepository.GetPagesWithParentId(pageId);
            if ( childList.Count > 0 )
            {
                return false;
            }

            return _editPageRepository.DeletePage(pageId);
        }

        public bool ChangeAccess(int pageId, int accessLevel)
        {
            if ( !CanEditPage() )
            {
                return false;
            }

            return _editPageRepository.ChangeAccess(pageId,accessLevel);
        }
    }
}