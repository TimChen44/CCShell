using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using Radzen.Blazor;
using System.Reflection;

namespace CCShell.Components.Layout
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
                .ToDictionary(x => x.GetCustomAttribute<RouteAttribute>().Template.TrimStart('/'), x => x);
            return base.OnInitializedAsync();
        }

        public void RouterNavigate(NavigationContext context)
        {
            if (string.IsNullOrWhiteSpace(context.Path)) return;

            if (Routes.TryGetValue(context.Path, out var routeType) == false)
            {
                //没有找到路由，可以尝试显示404
                return;
            }

            var pageIndex = PageDatas.FindIndex(x => x.Path == context.Path);
            if (pageIndex != -1)
            {
                SelectedIndex = pageIndex;
                return;
            }

            var pageConfig = routeType.GetCustomAttribute<PageConfigAttribute>();
            var title = pageConfig?.Title ?? routeType.Name;

            PageDatas.Add(new PageData(context.Path, title, new Microsoft.AspNetCore.Components.RouteData(routeType, new Dictionary<string, object>())));
            Tabs.Reload();
            SelectedIndex = PageDatas.Count - 1;
        }

        public record PageData(string Path, string Text, Microsoft.AspNetCore.Components.RouteData RouteData);

    }
}
