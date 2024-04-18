using System.Collections.Generic;

namespace _Scripts {
    public enum ItemType {
        Beer,
        Cigs,
        Glass,
        Cuffs,
        Saw
    }

    public class Item {
        public string title { get; private set; }
        public string description { get; private set; }
        public ItemType itemType { get; private set; }

        public Item(int itemNum) : this((ItemType)itemNum) {}

        public Item(ItemType type) {
            title = getTitle(type);
            description = getDescription(type);
            itemType = type;
        }
        
        public static string getTitle(ItemType itemType) {
            switch (itemType) {
                case ItemType.Beer:
                    return "Beer";
                case ItemType.Cigs:
                    return "Cigarette Pack";
                case ItemType.Cuffs:
                    return "Handcuffs";
                case ItemType.Glass:
                    return "Magnifying Glass";
                case ItemType.Saw:
                    return "Hand Saw";
                default:
                    return "";
            }
        }
        
        public static string getUnit(ItemType itemType) {
            switch (itemType) {
                case ItemType.Beer:
                    return "can of beer";
                case ItemType.Cigs:
                    return "pack of cigarettes";
                case ItemType.Cuffs:
                    return "pair of handcuffs";
                case ItemType.Glass:
                    return "magnifying glass";
                case ItemType.Saw:
                    return "hand saw";
                default:
                    return "";
            }
        }
        
        public static string getDescription(ItemType itemType) {
            switch (itemType) {
                case ItemType.Beer:
                    return "Racks the shotgun. Ejects current shell.";
                case ItemType.Cigs:
                    return "Takes the edge off. Regain 1 charge.";
                case ItemType.Cuffs:
                    return "Dealer skips the next turn.";
                case ItemType.Glass:
                    return "Check the current round in the chamber.";
                case ItemType.Saw:
                    return "Shotgun deals 2 damage.";
                default:
                    return "";
            }
        }
    }
    
    public class ItemManager {
        
    }
}