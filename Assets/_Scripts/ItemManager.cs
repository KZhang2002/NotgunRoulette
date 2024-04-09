using System.Collections.Generic;

namespace _Scripts {
    public class ItemFactory {
        public enum Item {
            Beer,
            Cigs,
            Glass,
            Cuffs,
            Saw
        }

        public string title(Item item) {
            switch (item) {
                case Item.Beer:
                    return "Beer";
                case Item.Cigs:
                    return "Cigarette Pack";
                case Item.Cuffs:
                    return "Handcuffs";
                case Item.Glass:
                    return "Magnifying Glass";
                case Item.Saw:
                    return "Hand Saw";
                default:
                    return "";
            }
        }
        
        public string description(Item item) {
            switch (item) {
                case Item.Beer:
                    return "Racks the shotgun. Ejects current shell.";
                case Item.Cigs:
                    return "Takes the edge off. Regain 1 charge.";
                case Item.Cuffs:
                    return "Dealer skips the next turn.";
                case Item.Glass:
                    return "Check the current round in the chamber.";
                case Item.Saw:
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
    }
    
    public class ItemManager {
        
    }
}