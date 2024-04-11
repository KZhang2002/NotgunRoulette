using System.Collections.Generic;

namespace _Scripts {
    public enum ItemType {
        Beer,
        Cigs,
        Glass,
        Cuffs,
        Saw
    }
    
    public class ItemFactory {
        public string title(ItemType itemType) {
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
        
        public string description(ItemType itemType) {
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

    public class Item {
        public string title { set; private get; }
        public string description { set; private get; }
        public Item itemType { set; private get; }
        private ItemFactory IFac;

        public Item(ItemType itemType) {
            IFac = new ItemFactory();
            title = IFac.title(itemType);
            description = IFac.description(itemType);
        }
    }
    
    public class ItemManager {
        
    }
}