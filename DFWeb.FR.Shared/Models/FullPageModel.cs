using System.Collections.Generic;
using DFWeb.BE.Models;
using DFWeb.BE.Provider;

namespace DFWeb.FR.Models
{
    public class FullPageModel : BasePageModel
    {

        public IList<ArticleSectionModel> articleSections;

        public FullPageModel(IPageProvider pageProvider, IMenuProvider menuProvider, ILoginProvider loginProvider, IImageProvider imageProvider)
         : base(pageProvider,menuProvider, loginProvider, imageProvider)
        {
            articleSections = new List<ArticleSectionModel>();
        }

        override
        public void OnGet(int id)
        {
            base.OnGet(id);
            articleSections = pageProvider.GetArticleSections(PageId);
            EditUrl = "/Editor/EditPage";
        }
    }
}
