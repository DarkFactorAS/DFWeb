using DFWeb.BE.Models;
using DFWeb.BE.Provider;
using DFWeb.BE.Repository;
using Xunit;

namespace DFWeb.BE.Tests;

public class MenuProviderTests
{
    [Fact]
    public void GetDefaultId_ReturnsFirstMenuItemId()
    {
        var menuItems = CreateMenuItems();
        var provider = new MenuProvider(new FakeMenuRepository(menuItems));

        var defaultId = provider.GetDefaultId();

        Assert.Equal(1, defaultId);
    }

    [Fact]
    public void GetTree_ReturnsSelectedLineage()
    {
        var menuItems = CreateMenuItems();
        var provider = new MenuProvider(new FakeMenuRepository(menuItems));

        var tree = provider.GetTree(3);

        Assert.Equal(new[] { 1, 2, 3 }, tree.Select(item => item.ID).ToArray());
    }

    [Fact]
    public void SelectItem_ExpandsSelectedTreeAndSetsVisualState()
    {
        var menuItems = CreateMenuItems();
        var provider = new MenuProvider(new FakeMenuRepository(menuItems));

        var visible = provider.SelectItem(3);

        Assert.Equal(new[] { 1, 2, 3, 4 }, visible.Select(item => item.ID).ToArray());

        var root = visible[0];
        Assert.Equal(MenuItem.CLASS_MENU, root.MenuClass);
        Assert.Equal(0, root.Width);

        var child = visible[1];
        Assert.Equal(MenuItem.CLASS_SUBMENU, child.MenuClass);
        Assert.Equal(20, child.Width);

        var selected = visible[2];
        Assert.Equal(MenuItem.CLASS_SELECTED_SUB, selected.MenuClass);
        Assert.Equal(40, selected.Width);

        var draftRoot = visible[3];
        Assert.Equal(MenuItem.CLASS_DRAFTMENU, draftRoot.MenuClass);
        Assert.Equal(0, draftRoot.Width);
    }

    private static List<MenuItem> CreateMenuItems()
    {
        return
        [
            new MenuItem { ID = 1, ParentID = 0, Name = "Root", IsPublished = true },
            new MenuItem { ID = 2, ParentID = 1, Name = "Child", IsPublished = true },
            new MenuItem { ID = 3, ParentID = 2, Name = "Leaf", IsPublished = false },
            new MenuItem { ID = 4, ParentID = 0, Name = "Draft Root", IsPublished = false }
        ];
    }

    private sealed class FakeMenuRepository : IMenuRepository
    {
        private readonly List<MenuItem> _items;

        public FakeMenuRepository(List<MenuItem> items)
        {
            _items = items;
        }

        public List<MenuItem> GetAllItems()
        {
            return _items;
        }
    }
}
