namespace CCShell.Components.Layout
{
    [AttributeUsage(AttributeTargets.Class)]
    public class PageConfigAttribute : Attribute
    {
        public string Title { get; set; }

        public string Icon { get; set; }
    }
}
