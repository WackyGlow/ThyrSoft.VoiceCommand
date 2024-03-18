using ThyreSoft.VoiceCommand.Domain.Utilities.Enums;

namespace ThyreSoft.VoiceCommand.Domain.ValueObjects;

public class MenuItem
{
    public bool IsMenu { get; }
    public Drink? Drink { get; } // Nullable to accommodate non-menu items
    public List<string>? Extras { get; } // Nullable list to accommodate non-menu items
    public double Price { get; }
    public ItemSize? Size { get; } // Nullable enum to accommodate non-menu items

    public MenuItem(bool isMenu, Drink? drink, List<string>? extras, double price, ItemSize? size = null)
    {
        IsMenu = isMenu;
        Drink = drink;
        Extras = extras;
        Price = price;
        Size = size;
    }
}