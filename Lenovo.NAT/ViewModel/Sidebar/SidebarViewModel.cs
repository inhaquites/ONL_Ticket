// Em Lenovo.NAT.ViewModels ou similar
namespace Lenovo.NAT.ViewModels
{
    public class SidebarViewModel
    {
        public List<Card> Cards { get; set; }
        public Dictionary<string, string> Routes { get; set; }
        public Dictionary<string, string> Permissions { get; set; }
    }

    public class Card
    {
        public string Title { get; set; }
        public string[] Items { get; set; }
    }

    public class SidebarItem
    {
        public string Icon { get; set; }
        public string Label { get; set; }
        public string FullKey { get; set; } // usado para lookup na rota
    }
}
