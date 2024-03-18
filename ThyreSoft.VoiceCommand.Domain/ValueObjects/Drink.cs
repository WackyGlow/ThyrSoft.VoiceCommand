using ItemSize = ThyreSoft.VoiceCommand.Domain.Utilities.Enums.ItemSize;

namespace ThyreSoft.VoiceCommand.Domain.ValueObjects;

public class Drink
{
    public string Name { get; }
    public ItemSize Size { get; }
}