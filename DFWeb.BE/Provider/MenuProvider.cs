using System;
using System.Collections.Generic;
using System.Linq;

using DFWeb.BE.Repository;
using DFWeb.BE.Models;

namespace DFWeb.BE.Provider
{
    public interface IMenuProvider
    {
        int GetDefaultId();
        List<MenuItem> GetTree( int pageId );
        List<MenuItem> SelectItem( int selectedItemId );
    }

    public class MenuProvider : IMenuProvider
    {
        public List<MenuItem> menuItems;
        private IMenuRepository menuRepository;

        public MenuProvider(IMenuRepository menuRepository)
        {
            this.menuRepository = menuRepository;
            menuItems = menuRepository.GetAllItems();
        }

        public int GetDefaultId()
        {
            if ( menuItems == null || menuItems.Count == 0 )
            {
                return 0;
            }
            return menuItems[0].ID;
        }

        public List<MenuItem> GetTree( int pageId )
        {
            List<int> selectedTree = new List<int>();
            GetSelectedTree(selectedTree, pageId);

            List<MenuItem> menuTreeList = new List<MenuItem>();

            foreach( var id in selectedTree)
            {
                var menuItem = GetMenuItem(id);
                if ( menuItem != null )
                {
                    menuTreeList.Add(menuItem);
                }
            }
            return menuTreeList;
        }

        public List<MenuItem> SelectItem( int selectedItemId )
        {
            // Create the expanded tree
            List<int> selectedTree = new List<int>();
            GetSelectedTree(selectedTree, selectedItemId);

            // Add top nodes
            List<MenuItem> visibleItems = new List<MenuItem>();
            AddItemsWithParent(visibleItems, selectedTree, 0, 0);
            return visibleItems;
        }

        private void AddItemsWithParent(List<MenuItem> visibleItems, List<int> selectedTree, int parentId, int padding )
        {
            int selectedId = selectedTree.LastOrDefault();
            foreach (var menuItem in menuItems)
            {
                if (menuItem.ParentID == parentId)
                {
                    menuItem.MenuClass = GetMenuClass(menuItem.IsPublished, parentId == 0, selectedId == menuItem.ID);
                    menuItem.Width = padding;
                    visibleItems.Add(menuItem);

                    // Expand child tree
                    if ( selectedTree.Contains( menuItem.ID ) )
                    {
                        AddItemsWithParent(visibleItems, selectedTree, menuItem.ID, padding + 20);
                    }
                }
            }
        }

        private string GetMenuClass(bool isPublished, bool isMainItem, bool isSelected)
        {
            if (isSelected && isMainItem)
            {
                return MenuItem.CLASS_SELECTED;
            }
            else if ( isSelected && !isMainItem )
            {
                return MenuItem.CLASS_SELECTED_SUB;
            }
            else if ( isPublished && isMainItem )
            {
                return MenuItem.CLASS_MENU;
            }
            else if ( isPublished && !isMainItem )
            {
                return MenuItem.CLASS_SUBMENU;
            }
            else if ( !isPublished && isMainItem )
            {
                return MenuItem.CLASS_DRAFTMENU;
            }
            else
            {
                return MenuItem.CLASS_DRAFTSUBMENU;
            }
        }

        private void GetSelectedTree(List<int> expandedTree, int itemId)
        {
            if ( itemId != 0)
            {
                var menuItem = GetMenuItem(itemId);
                if (menuItem != null && menuItem.ParentID != 0)
                {
                    GetSelectedTree(expandedTree, menuItem.ParentID);
                }
                expandedTree.Add(itemId);
            }
        }

        private MenuItem GetMenuItem( int menuId )
        {
            return menuItems.Where(x => x.ID == menuId).FirstOrDefault();
        }
    }
}
