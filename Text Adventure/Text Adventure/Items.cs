using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExtensionMethods;

namespace TextGame {

    public interface IStackable
    {
        string Name { get; set; }
        int Amount { get; set; }
        bool CanStack { get; set; }
    }

    [Serializable]
    public class Item : IStackable {

        public int? id = 0; //A null ID means it is a unique item created in game. (Not stored in a data file)
        public int amount = 1; //For items that can stack, they are exactly the same - So the item is only removed when amount == 0
        public string name = "";
        public string description = "";
        public int baseCost = 0;
        public List<string> uses = new List<string>();
        public bool stack = false;
        public string messagedestroy = "";

        public delegate void EntityMethod(Entity _entity);

        //Passive system.
        /*
         * Throughout the gameplay code I will call events to an event handler - When an event is called,
         * passive effects may activate. These are called event passives, and have methods that activate when a 
         * specific event is called. Another type of passive is one that returns a single object, for example a float from 0-1
         * denoting a passive's ability to lower prices. 
         * In the shop interface, the price may be calculated with this passive as a coefficient.
         */
        //public delegate void PurchaseEventMethod(PurchaseEventArgs e);
        //public PurchaseEventMethod purchasePassive;

        public EntityMethod OnHit;     //For weapons
        public EntityMethod OnConsume; //For consumables

        //Consumable
        public bool consumable = false;
        public string consumeMessage = "";

        //Equippable
        public bool equippable;
        public EquipType equipType = EquipType.None;

        public string[] poolTags;

        //Equippable
        //public EquipType equipType;
        //Passive value changes/tick effects/event passives?
        //eg a passive -20% off of stock prices - Make stock prices depend on a magnitude as well, and passives change that magnitude.

        //Grammar
        public bool useMuch = false;
        public string ManyMuch { get { return useMuch ? "much" : "many"; } }
        public bool sPlural = true;
        public string Plural { get { return sPlural ? name + "s" : name; } }

        public string Display { get { return Amount == 1 ? name : name + " * " + Amount.ToString(); } }
       
        public int Cost { get { return baseCost * Amount; } set { } }

        public bool HasUse(string use) {
            for (int i = 0; i < uses.Count; i++) {
                if (uses[i] == use) return true;
            }
            return false;
        }

        //public virtual Delegate DoThing { get; set; }

        /*public static Item operator +(Item _item, int change)
        {
            _item.amount += change;
            return _item;
        }*/

        public void DisplayInfo() {
            //Writes a "tooltip" with information about the item.
            Console.Clear();
            Program.Write(Display);
            Program.HalfSegment();
            Program.Write("Value: " + baseCost + (Amount > 1 ? " each (" + Cost + ")" : ""));
            Program.HalfSegment();
            Program.Write(description);
            Program.HalfSegment();
        }

        #region Interface Methods
        //IStackable
        public string Name { get { return name; } set { } }
        public bool CanStack { get { return stack; } set { } }
        public int Amount { get { return amount; } set { amount = value; } }
        #endregion
    }

    static class Items {

        public static Dictionary<string, Item> items = new Dictionary<string, Item>();

        public static Item GetItem(string name) {
            try
            {
                Item item = items[name].DeepClone();
                return item;
            }
            catch {
                Console.WriteLine("ERROR: Items.cs - GetItem() - You searched for an item (" + name + ") that does not exist.");
                Program.Read();
                return new Item();
            }
        }


        public static void Initialize() {

            Item i;

            i = new Item() {
                name = "Gold",
                description = "Better than lemons",
                baseCost = 1,
                stack = true,
                sPlural = false,
                useMuch = true,
                poolTags = new string[] { "Gold" }
            };
            items.Add(i.name, i);

            i = new Item() {
                name = "Gold Bar",
                description = "Many moneys!",
                baseCost = 10,
                poolTags = new string[] { "Crafting", "Smithing", "Gold", "Bar" }
            };
            items.Add(i.name, i);

            i = new Consumable("Drink") {
                name = "Non Suspicious Potion",
                description = "Heals you... Yes...",
                baseCost = 15,
                stack = true,
                poolTags = new string[] { "Potion", "Medical" }
            };
            i.OnConsume = (Entity entity) => {
                Program.Write("HAHAAHHAH You have falled into my chasm of tricks!\nYour inventory has been uberbojangled to the point of non existence.");
                entity.Inventory().Clear();
            };
            items.Add(i.name, i);

            i = new Weapon(EquipType.Smallheld) {
                name = "Knife",
                description = "This is a knife",
                baseCost = 12,
                poolTags = new string[] { "Weapon", "Smallheld", "Knife", "Blade" }
            };
            i.OnHit = (Entity entity) => {
                entity.Damage(5);
            };
            items.Add(i.name, i);

            i = new Weapon(EquipType.Largeheld) {
                name = "Big Sword",
                description = "By my honour I will defend this planet!",
                baseCost = 500,
                poolTags = new string[] { "Weapon", "Largeheld", "Sword", "Blade" }
            };
            i.OnHit = (Entity entity) => {
                entity.Damage(40);
            };
            items.Add(i.name, i);

            i = new Item() {
                name = "Mall Cop Armour",
                description = "Certified peep beep.",
                equippable = true,
                equipType = EquipType.Torso,
                baseCost = 70
            };
            items.Add(i.name, i);


        }




        public static Weapon AttackHelicopter {
            get {
                return new Weapon(EquipType.Mediumheld) {

                    name = "Attack Helicopter",
                    description = "Not one, but FOUR BLADES",
                    baseCost = 3500,
                    poolTags = new string[] { "Twohanded", "Helicopter", "Vehicle" },

                    OnHit = (Entity entity) => {

                        Program.Write("You raise your Attack Helicopter.");
                        Program.Segment();

                        int attackAmount = 0;
                        switch (Program.OptionInput("Fwoop", "Fwoop fwoop", "Fwoop fwoop fwoop", "Fwoop fwoop fwoop fwoop"))
                        {
                            case "Fwoop":
                                attackAmount = 1;
                                break;
                            case "Fwoop fwoop":
                                attackAmount = 2;
                                break;
                            case "Fwoop fwoop fwoop":
                                attackAmount = 3;
                                break;
                            case "Fwoop fwoop fwoop fwoop":
                                attackAmount = 4;
                                break;
                        }
                        Console.Clear();
                        for (int i = 0; i < attackAmount; i++)
                        {
                            Program.Write("--> (" + 10 + ") A helicopter blade hit " + entity.name + " for " + 10 + " damage!");
                            Program.Write("<-- (" + 4 + ") A helicopter accidentally scrapes you for " + 4 + " damage!");
                            entity.Damage(10);
                            Program.player.Damage(4);
                        }
                    }
                };
            }
        }
    }

    public enum EquipType : byte {
        None,
        Smallheld,
        Mediumheld,
        Largeheld,
        Head,
        Torso,
        Legs,
        Feet
    }

    [Serializable]
    public class BodyModule {
        //Controls what and how an entity equips items.
        //TODO: Join arbitrary number of equipAreas to equipTypes. 
        //For example, a humanoid body module would connect EquipType.Smallheld -> "Left Hand" or "Right Hand"
        //EquipType.Mediumheld -> "Left Hand" and "Right Hand"
        //BodyModule handles equip logic and body mechanisms for any entity, such as a quadrupedal, spirit or humanoid.

        public List<BodyConnection> equipConnections = new List<BodyConnection>();
        public List<EquipArea> equipAreas = new List<EquipArea>();

        public void EquipInterface() {

            Console.Clear();

            List<string> options = new List<string>() { "Back" };

            List<string> leftSide = equipAreas.Select(x => x.name.UpperFirst()).ToList();
            List<string> rightSide = equipAreas.Select(x => x.equipped == null ? "" : "[" + x.equipped.name + "]").ToList();

            options.AddRange(Program.DisplaySides(leftSide, rightSide, 2));

            int optionInput = Program.IndexInput(options);
            Console.Clear();

            if (optionInput == 0) {
            }
            else {
                EquipArea area = equipAreas[optionInput - 1];

                if (area.equipped != null) {
                    Item selectItem = area.equipped;
                    Program.ItemOptions(selectItem);
                }
                else {
                    Console.Clear();

                    List<EquipType> equippableTypes = equipConnections.Where(x => x.equipAreas.Contains(area.name)).Select(x => x.equipType).ToList();
                    List<Item> equippableItems = Program.player.Items.Where(x => equippableTypes.Contains(x.equipType)).ToList();

                    List<string> equipOptions = new List<string>() { "Back" }.Concat(equippableItems.Select(x => x.Display)).ToList();

                    int equipInput = Program.IndexInput(equipOptions);

                    if (equipInput == 0) {
                        EquipInterface();
                    }
                    else {
                        Program.ItemOptions(equippableItems[equipInput - 1]);
                    }
                }

                EquipInterface();
            }
        }

        public bool Equipped(Item item) {
            return equipAreas.Select(x => x.equipped).Contains(item);
        }
        public List<EquipArea> EquippedTo(Item item) {
            return equipAreas.Where(x => x.equipped == item).ToList();
        }

        public void Unequip(Item item) {
            Console.Clear();
            List<EquipArea> equippedTo = EquippedTo(item);

            if (equippedTo.Count == 0) Program.Write(item.name + " is not equipped.");

            else {
                for (int i = 0; i < equippedTo.Count; i++) {
                    equippedTo[i].equipped = null;
                }
                Program.Write("You unequipped " + item.name);
            }
        }

        public void Equip(Item item) {

            if (item.equippable) {

                List<BodyConnection> canEquipTo = new List<BodyConnection>();

                for (int i = 0; i < equipConnections.Count; i++) {
                    if (equipConnections[i].equipType == item.equipType) {
                        canEquipTo.Add(equipConnections[i]);
                    }
                }
                if (canEquipTo.Count < 1) {
                    Program.Write("You can not equip " + item.name + ".");
                }
                else {
                    Program.Write("How would you like to equip " + item.name + "?");
                    Program.Segment();

                    List<string> options = new List<string>();

                    for (int i = 0; i < canEquipTo.Count; i++) {

                        string option = "";
                        for (int g = 0; g < canEquipTo[i].equipAreas.Count; g++) {

                            option += canEquipTo[i].equipAreas[g];
                            if(g < canEquipTo[i].equipAreas.Count - 1) option +=  " and ";
                            option = option.UpperFirst();
                        }
                        options.Add(option);
                    }

                    BodyConnection equipTo = canEquipTo[Program.IndexInput(options)];

                    string message = "You equipped " + item.name + " to your ";
                    for (int i = 0; i < equipTo.equipAreas.Count; i++) {
                        GetEquipArea(equipTo.equipAreas[i]).equipped = item;
                        message += equipTo.equipAreas[i];
                        message += i == equipTo.equipAreas.Count - 1 ? "." : " and ";
                    }
                    Program.Write(message);
                }
            }
            else Program.Write(item.name + " is not equippable.");
            Program.Read();
        }

        public EquipArea GetEquipArea(string name) {
            //Body connections refer to equip areas by their name to make it easier to create the equip logic.

            EquipArea area = equipAreas.Find(x => x.name == name);
            if (area != null) return area;
            
            Program.Write("ERROR: Tried to get equip area of body module that does not exist.\nA BodyModule may not have been structured correctly.");
            Program.Read();
            return null;
        }

        #region Body Module presets
        public static BodyModule Humanoid {
            get {
                BodyModule module = new BodyModule() {

                    equipAreas = new List<EquipArea>() {
                        new EquipArea("left hand", "hand"),
                        new EquipArea("right hand", "hand"),
                        new EquipArea("head"),
                        new EquipArea("torso"),
                        new EquipArea("legs"),
                        new EquipArea("feet")
                    },

                    equipConnections = new List<BodyConnection>() {
                        new BodyConnection(EquipType.Smallheld, "left Hand"),
                        new BodyConnection(EquipType.Smallheld, "right Hand"),
                        new BodyConnection(EquipType.Largeheld, "left hand", "right hand"),
                        new BodyConnection(EquipType.Head, "head"),
                        new BodyConnection(EquipType.Torso, "torso"),
                        new BodyConnection(EquipType.Legs, "legs"),
                        new BodyConnection(EquipType.Feet, "feet")
                    }
                };
                return module;
            }
        }

        public static BodyModule FloatingHead {
            get {
                BodyModule module = new BodyModule() {
                    equipAreas = new List<EquipArea>() {
                        new EquipArea("head")
                    },
                    equipConnections = new List<BodyConnection>() {
                        new BodyConnection(EquipType.Head, "head")
                    }
                };
                return module;
            }
        }

        public static BodyModule Giant {
            get {
                BodyModule module = new BodyModule() {

                    equipAreas = new List<EquipArea>() {
                        new EquipArea("left hand", "hand"),
                        new EquipArea("right hand", "hand"),
                        new EquipArea("head"),
                        new EquipArea("torso"),
                        new EquipArea("legs"),
                        new EquipArea("feet")
                    },

                    //TODO: EquipType to a group of EquipAreas (Eg, a creature with 8 arms, you wouldn't have to write every combination)
                    equipConnections = new List<BodyConnection>() {
                        new BodyConnection(EquipType.Smallheld, "left hand"),
                        new BodyConnection(EquipType.Smallheld, "right hand"),
                        new BodyConnection(EquipType.Largeheld, "left hand"),
                        new BodyConnection(EquipType.Largeheld, "right hand"),
                        new BodyConnection(EquipType.Largeheld, "left hand", "right hand"),
                        new BodyConnection(EquipType.Head, "head"),
                        new BodyConnection(EquipType.Torso, "torso"),
                        new BodyConnection(EquipType.Legs, "legs"),
                        new BodyConnection(EquipType.Feet, "feet")
                    }
                };
                return module;
            }
        }
        #endregion

        [Serializable]
        public class BodyConnection {

            //Allows connection of an item of an EquipType to specific equip areas

            public List<string> equipAreas;
            public EquipType equipType = EquipType.None;

            public BodyConnection(EquipType _equipType, params string[] _equipAreas) {
                equipAreas = _equipAreas.ToList();
                equipType = _equipType;
            }
        }

        [Serializable]
        public class EquipArea {

            public string name = "";
            public string groupName = ""; //Group name such as "hand" for equip areas left Hand" and "Right Hand"
            public Item equipped;

            public EquipArea(string _name) {
                name = _name;
                groupName = _name;
            }
            public EquipArea(string _name, string _groupName) {
                name = _name;
                groupName = _groupName;
            }
        }
        

    }

    [Serializable]
    public class Weapon : Item {
        public Weapon(EquipType _equipType) {
            equippable = true;
            equipType = _equipType;
            OnHit = (Entity _entity) => _entity.Damage(10);
        }
        public Weapon() {
        }
    }

}
