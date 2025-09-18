using Lenovo.NAT.Services.Admin;
using Lenovo.NAT.ViewModels;
using System.Dynamic;

public class PermissionService
{
    public PermissionService() { }

    public async Task<List<string>> GetAllowedItems(string userNetworkId, Dictionary<string, string> permissions)
    {
        // Verifica as permissões do usuário
        var allowedItems = new List<string>();

        foreach (var kv in permissions)
        {
            allowedItems.Add(kv.Key);
        }

        return allowedItems;
    }

    public List<Card> BuildCards(List<string> allowedItems, Dictionary<string, string> permissions)
    {
        return permissions
            .Where(p => allowedItems.Contains(p.Key))
            .GroupBy(p => p.Value)
            .Select(g => new Card
            {
                Title = g.Key,
                Items = g.Select(x => x.Key).ToArray()
            })
            .ToList();
    }
}
