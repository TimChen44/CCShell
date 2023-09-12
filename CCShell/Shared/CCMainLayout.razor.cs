using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.Components;
using Radzen.Blazor;
using System.Reflection;

namespace CCShell.Shared
{
    public partial class CCMainLayout
    {
        [Parameter]
        [EditorRequired]
        public Assembly AppAssembly { get; set; }

        [Parameter]
        public RenderFragment MenuItems { get; set; }

        bool sidebar1Expanded = true;
        RadzenTabs Tabs;
        int SelectedIndex;

        Dictionary<string, Type> Routes = new Dictionary<string, Type>();
        List<PageData> PageDatas = new List<PageData>();

        protected override Task OnInitializedAsync()
        {
            Routes = AppAssembly.ExportedTypes.Where(x => x.GetCustomAttribute<RouteAttribute>() != null)
                .ToDictionary(x => x.GetCustomAttribute<RouteAttribute>().Template, x => x);
            return base.OnInitializedAsync();
        }

        public void RouterNavigate(NavigationContext context)
        {
            if (string.IsNullOrWhiteSpace(context.Path)) return;

            if ( Routes.TryGetValue(context.Path, out var route)==false )
            {
                //没有找到路由，可以尝试显示404
                return;
            }


            var pageIndex = PageDatas.FindIndex(x => x.Name == pageType.Name);
            if (pageIndex != -1)
            {
                SelectedIndex = pageIndex;
                return;
            }

            var pageConfig = pageType.GetCustomAttribute<PageConfigAttribute>();
            var title = pageConfig?.Title ?? pageType.Name;

            PageDatas.Add(new PageData(pageType.Name, title, new RouteData(pageType, new Dictionary<string, object>())));
            Tabs.Reload();
            SelectedIndex = PageDatas.Count - 1;
        }

        public record PageData(string Name, string Text, RouteData RouteData);

    }
}
