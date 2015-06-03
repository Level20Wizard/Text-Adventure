using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

using ExtensionMethods;
using TextGame;

namespace TextGame {

    public class Program {

        //Files
        public static string directory = @"C:\Users\\draco_000\Documents\Visual Studio 2013\Projects\ConsoleApplication2";
        public static List<string> saveData = new List<string>();

        public static Player player = new Player();

        public static int level = 1; //Each area is a new level

        #region Utils
        static bool writtenSinceLastSegment = true;
        static bool writtenSinceLastRead = true;

        public static void Write(string arg)
        {
            Console.WriteLine(arg);
            writtenSinceLastRead = true;
            writtenSinceLastSegment = true;
        }
        public static void Write(int arg)
        {
            Write(arg.ToString());
        }
        public static void Write(List<string> args) {
            for (int i = 0; i < args.Count; i++) {
                Write(args[i]);
            }
        }
        public static string Read()
        {
            if (writtenSinceLastRead) Write("\n");
            string line = Console.ReadLine();
            writtenSinceLastRead = false;
            return line;
        }
        public static void Segment()
        {
            //Only writes a segmentation line if something was written since the last segmentation line
            if (writtenSinceLastSegment)
            {
                Write("_____________________________________________________________");
                writtenSinceLastSegment = false;
            }
        }
        public static void HalfSegment()
        {
            if (writtenSinceLastSegment)
            {
                Write("_____________________________");
                writtenSinceLastSegment = false;
            }
        }

        public static List<string> DisplaySides(List<string> leftSide, List<string> rightSide, int minGap = 1)
        {
            //Displays 2 equal length lists of strings side by side, with the leftSide aligned to the left and the rightSide aligned to the right.
            //The minGap is the minimum amount of spaces between each set of strings.
            List<string> display = new List<string>();

            int maxSize = 0;

            //Finds the largest length of one pair of strings.
            for (int i = 0; i < leftSide.Count; i++)
            {
                if (leftSide[i].Length + rightSide[i].Length > maxSize)
                {
                    maxSize = leftSide[i].Length + rightSide[i].Length;
                }
            }

            //Creates whitespace between each pair of strings to make all string pairs the same length, then displays them.
            for (int i = 0; i < leftSide.Count; i++)
            {
                display.Add(leftSide[i] + new string(' ', maxSize - leftSide[i].Length - rightSide[i].Length + minGap) + rightSide[i]);
            }
            return display;
        }
        public static int SelectFromRange(int min, int max)
        {
            while (true)
            {
                string input = Read();
                int a = 0;

                if (int.TryParse(input, out a) && a >= min && a <= max)
                {
                    return a;
                }
                else if (input.ToUpper() == "ALL") return max;
            }
        }
        public static string OptionInput(List<string> optionNames) {

            for (int i = 0; i < optionNames.Count(); i++) {
                Write((i + 1).ToString() + ": " + optionNames[i]);
            }

            while (true) {
                string input = Read();
                int numberInput;
                if (int.TryParse(input, out numberInput) && numberInput >= 1 && numberInput <= optionNames.Count()) {
                    Console.Clear();
                    return optionNames[numberInput - 1];
                }
                else for (int i = 0; i < optionNames.Count; i++) {
                    if (input.ToUpper().Trim() == optionNames[i].ToUpper()) {
                        Console.Clear();
                        return optionNames[i];
                    }
                }
            }
        }

        public static int IndexInput(List<string> optionNames) {

            for (int i = 0; i < optionNames.Count(); i++) {
                Write((i + 1).ToString() + ": " + optionNames[i]);
            }
            while (true) {
                string input = Read();
                int numberInput;
                if (int.TryParse(input, out numberInput) && numberInput >= 1 && numberInput <= optionNames.Count()) {
                    Console.Clear();
                    return numberInput - 1;
                }
                else for (int i = 0; i < optionNames.Count; i++) {
                        if (input.ToUpper().Trim() == optionNames[i].ToUpper()) {
                            Console.Clear();
                            return i;
                        }
                    }
            }
        }

        public static T SelectObject<T>(List<T> objects, List<string> objectNames) where T : new() {
           



            return new T();
        }

        //Optional parameter for easier creation of options.
        public static string OptionInput(params string[] optionNames) {
            return OptionInput(optionNames.ToList());
        }
        public static int IndexInput(params string[] optionNames) {
            return IndexInput(optionNames.ToList());
        }
        #endregion

        static void Main(string[] args) {

            Items.  Initialize();
            Enemies.Initialize();

            Game();
        } 
        static void Game() {

            Console.Clear();
            player = new Player() { Gold = 500 };

            player.Items.AddStack(Items.AttackHelicopter);
            player.Items.AddStack(Items.GetItem("Non Suspicious Potion"));

            Write("What race would you like to be?");

            Segment();
            switch(OptionInput("Human", "Giant", "Spooky Floating Head")) {
                case "Human":
                    player.body = BodyModule.Humanoid;
                    break;
                case "Giant":
                    player.body = BodyModule.Giant;
                    break;
                case "Spooky Floating Head":
                    player.body = BodyModule.FloatingHead;
                    break;
            }

            Item weapon = Items.GetItem("Big Sword");

            player.Items.AddStack(weapon);
            player.body.Equip(weapon);

            player.Items.AddStack(Items.GetItem("Mall Cop Armour"));

            Write("What is your name?");
            while (true) {

                string nameInput = Read().Trim();
                if (nameInput.Length > 0) {
                    player.name = nameInput;
                    break;
                }
            }

            //Example of creating a custom item in one line that does not have an ID (it is not from a data file)
            player.Items.AddStack(new Item() { name = "Chill Octogon", baseCost = 5, description = "He is chill", messagedestroy = "Why would you do this, he did nothing to you", id = null });

            Consumable strongPotion = new Consumable("Drink") { name = "Potionseller's Strongest Potion", baseCost = 270, description = "Heals 10 health", id = null };
            strongPotion.OnConsume = (Entity _entity) => {
                Effects.Heal(_entity, 10);
            };


            player.Items.AddStack(strongPotion);

            Console.Clear();

            while (true) {

                PlayerOptions();
                Encounter(Enemies.GetEnemy("Living Lemon"));
                PlayerOptions();
                ShopEncounter();

            }
        }


        //Item pools and loot
        static Random lootRandom = new Random();

        //Todo: Redo loot pool to be defined in code!
        /*static List<Item> Loot(int id)
        {
            //Generates loot from an enemy id
            List<Item> loot = new List<Item>();

            List<string> data = GetData(enemyData, id);
            List<string> lootTable = GetDataSection(data, "loot");

            double latestChance = 100;
            List<Item> toChoose = new List<Item>();

            List<double> weights = new List<double>();
            double totalWeight = 0;

            for (int i = 0; i < lootTable.Count; i++)
            {
                if (lootTable[i].EndsWith("%")) //This line notes the percentage chance of the next defined loot to be dropped. At the start the chance is by default 100%.
                {
                    latestChance = Convert.ToDouble(lootTable[i].Substring(0, lootTable[i].Length - 1)); //Removes the % syntax and converts to a double.
                    totalWeight = 0;
                    weights.Clear();
                }
                else
                {
                    if (lootRandom.Next(0, 101) <= latestChance)
                    {
                        string itemName = lootTable[i];
                        int amountOfItem = 1;
                        if (lootTable[i].Contains('*'))
                        {
                            string amount = lootTable[i].Substring(lootTable[i].IndexOf('*') + 1);
                            itemName = lootTable[i].Substring(0, lootTable[i].IndexOf('*'));
                            if (lootTable[i].Contains(".."))
                            {
                                int min = Convert.ToInt32(amount.Substring(0, amount.IndexOf('.')));
                                int max = Convert.ToInt32(amount.Substring(amount.LastIndexOf('.') + 1));
                                amountOfItem = lootRandom.Next(min, max + 1);
                            }
                            else amountOfItem = Convert.ToInt32(lootTable[i].Substring(lootTable[i].IndexOf('*') + 1));
                        }
                        for (int g = 0; g < amountOfItem; g++) loot.AddStack(GetItem(itemName));
                    }
                    /*else for (int g = 0; g < lootTable[i].Length; g++)
                    {
                        if (lootTable[i][g] == '.') //Finds the weight of the loot
                        {
                            double weight = Convert.ToDouble(lootTable[i].Substring(0, g - 2));
                            weights.Add(weight);
                            totalWeight += weight;
                            break;
                        }
                    }*/
                /*}
            }

            return loot;
        }*/
        /*public static List<Item> ItemsInPool(List<string> lootPoolNames)
        {
            List<Item> items = new List<Item>();

            string currentName = "";
            for (int i = 0; i < itemData.Count; i++)
            {
                if (itemData[i].StartsWith("name:")) currentName = itemData[i].Substring(6);
                else if (itemData[i].StartsWith("pool:"))
                {
                    List<string> data = GetItemData(currentName);
                    List<string> pool = GetDataProperties(data, "pool");

                    //Check if the pool has all the required pool names

                    bool getItem = true;

                    for (int g = 0; g < lootPoolNames.Count; g++)
                    {
                        bool hasValue = false;
                        for (int h = 0; h < pool.Count; h++)
                        {
                            if (lootPoolNames[g] == pool[h])
                            {
                                hasValue = true;
                            }
                        }
                        if (!hasValue)
                        {
                            getItem = false;
                            break;
                        }
                    }
                    if (getItem) items.Add(GetItem(data));
                }
            }

            return items;
        }
        public static List<Item> ItemsInPool(params string[] lootPoolNames)
        {
            return ItemsInPool(lootPoolNames.ToList());
        }*/

        //Complete random generators. Mainly used for testing. They pull from a pool of all data.
        #region Random Generators
        /*public static Enemy RandomEnemy()
        {
            return GetEnemy(new Random().Next(0, enemyAmount));
        }*/

        static Random itemRandom = new Random();

        #endregion

        static void Encounter(params Enemy[] enemies) {

            Encounter(enemies.ToList());
        }
        static void Encounter(List<Enemy> enemies) {

            //Generates a battle with a set of enemies

            Console.Clear();
            Write("You encountered some enemies.");
            Segment();

            List<Enemy> enemiesInEncounter = new List<Enemy>();
            for (int i = 0; i < enemies.Count; i++) enemiesInEncounter.Add(enemies[i]); //Creates a new list of the enemies that are not removed when they are killed.

            while (enemies.Count > 0) {

                for (int i = 0; i < enemies.Count; i++) {
                    enemies[i].Attack(player);
                    //Sleep(500);
                }

                if (player.HP <= 0) {
                    PlayerDeath();
                    break;
                }
                else Write("\nYou are now on " + player.HP.ToString() + " HP");

                Segment();
                Write("1: Attack, 2: Use item, 3: Prepare to block");

                while (true) {

                    string input = Read();

                    if (input == "1") {

                        Console.Clear();
                        if (enemies.Count > 1) {

                            for (int i = 0; i < enemies.Count; i++) {

                                Write((i + 1).ToString() + " : " + enemies[i].name + " (" + enemies[i].HP + "), ");
                            }
                        }

                        bool pass = true;
                        while (pass) {

                            string input2 = "";
                            if (enemies.Count > 1) input2 = Read(); //No target message if only one enemy

                            Console.Clear();

                            for (int i = 0; i < enemies.Count; i++) {

                                if (enemies.Count == 1 || input2 == (i + 1).ToString()) {

                                    player.body.GetEquipArea("left hand").equipped.OnHit(enemies[i]);
                                    Segment();

                                    if (enemies[i].HP <= 0) {
                                        Write(enemies[i].name + " is now dead");
                                        enemies.Remove(enemies[i]);
                                    }
                                    else Write(enemies[i].name + " now has " + enemies[i].HP + " health.");
                                    Segment();
                                    pass = false;
                                    break;
                                }
                            }
                        }
                        break;
                    }
                    else if (input == "2") {
                        Console.Clear();
                        break;
                    }
                    else if (input == "3") {
                        Console.Clear();
                        break;
                    }
                }
            }

            Segment();

            for (int i = 0; i < enemiesInEncounter.Count; i++) {

                for (int h = 0; h < 1600; h++) { //Repeat for debug purposes (have a good range of random results)
                    List<Item> items = enemiesInEncounter[i].lootTable.Drops();
                    for (int g = 0; g < items.Count; g++) {
                        Write(enemiesInEncounter[i].name + " dropped " + items[g].Display + ".");
                        player.Items.AddStack(items[g]);
                    }
                    Segment();
                }
            }
            Segment();
            Read();
            Console.Clear();

        }

        static void ShopEncounter() {

            Console.Clear();

            Shopkeeper shopkeeper = Shopkeeper.GenerateShopkeeper(level);

            Write("You find a shopkeeper.");
            Segment();

            MeetInterface:
            Write("Shopkeeper");
            Segment();

            switch (OptionInput("Leave", "Buy", "Sell", "Steal")) {

                case "Leave":
                    Console.Clear();
                    break;
                case "Buy":
                BuyInterface:

                    Console.Clear();

                    Write("You have " + player.Gold.ToString() + " gold.");
                    Segment();
                    Write("1: Back");
                    Segment();

                    if (shopkeeper.stock.Count < 1) {
                        Write("I do not have any stock right now.");
                    }
                    else {
                        List<string> buyDisplays = new List<string>();
                        List<string> buyPrices = new List<string>();

                        for (int i = 0; i < shopkeeper.stock.Count; i++) {

                            buyDisplays.Add((i + 2).ToString() + ": " + shopkeeper.stock[i].Display);
                            buyPrices.Add("(" + shopkeeper.stock[i].baseCost.ToString() + " gold)");
                        }
                        Program.Write(DisplaySides(buyDisplays, buyPrices, 2));
                    }


                    int selected = SelectFromRange(1, shopkeeper.stock.Count + 1);
                    switch (selected) {

                        case 1:
                            Console.Clear();
                            goto MeetInterface;
                        default:

                            Item selectedItem = shopkeeper.stock[selected - 2];
                            selectedItem.DisplayInfo();

                            Write("\n");
                            switch (OptionInput("Back", "Buy")) {

                                case "Back":
                                    goto BuyInterface;
                                case "Buy":

                                    Console.Clear();

                                    if (selectedItem.Amount > 1) Write("How " + selectedItem.ManyMuch + " " + selectedItem.Plural + " would you like to buy? (" + selectedItem.Amount.ToString() + " available, " + selectedItem.baseCost.ToString() + " gold each)");

                                    int amount = selectedItem.amount != 1 ? SelectFromRange(0, selectedItem.Amount) : 1;

                                    int batchPrice = selectedItem.baseCost * amount;

                                    Console.Clear();
                                    if (amount == 0) {
                                        Write("You didn't buy any " + selectedItem.Plural + ".");
                                    }
                                    else if (player.Gold >= batchPrice) {

                                        if (amount > 1) Write("You bought " + amount.ToString() + " " + selectedItem.Plural + " for " + batchPrice.ToString() + " gold.");
                                        else Write("You bought " + selectedItem.name + " for " + batchPrice.ToString() + " gold.");

                                        Segment();

                                        player.Gold -= batchPrice;
                                        shopkeeper.Gold += batchPrice;

                                        player.Items.AddStack(selectedItem.DeepClone(), amount);
                                        shopkeeper.stock.RemoveStack(selectedItem, amount);
                                    }
                                    else {

                                        if (amount > 1) Write("You do not have enough gold to buy " + amount + " " + selectedItem.Plural);
                                        else Write("You do not have enough gold to buy " + selectedItem.name);

                                        Segment();
                                    }
                                    Read();
                                    Console.Clear();
                                    goto BuyInterface;
                            }
                            break;
                    }
                    break;

                case "Sell":
                SellInterface:

                    Console.Clear();

                    Write("The shopkeeper has " + shopkeeper.Gold.ToString() + " gold");
                    Segment();

                    Write("1: Back");
                    Segment();

                    List<string> sellDisplays = new List<string>();
                    List<string> sellPrices = new List<string>();

                    for (int i = 0; i < player.Items.Count; i++) {

                        sellDisplays.Add((i + 2).ToString() + ": " + player.Items[i].Display);
                        sellPrices.Add("(" + player.Items[i].baseCost.ToString() + " gold)");
                    }
                    DisplaySides(sellDisplays, sellPrices, 2);

                    int sellSelected = SelectFromRange(1, player.Items.Count + 1);

                    switch (sellSelected) {

                        case 1:
                            Console.Clear();
                            goto MeetInterface;

                        default:

                            ItemOptionsInterface:

                            Item selectedItem = player.Items[sellSelected - 2];
                            Write("The shopkeeper has " + shopkeeper.Gold.ToString() + " gold");
                            Segment();

                            int offer = shopkeeper.Offer(selectedItem);

                            selectedItem.DisplayInfo();
                            Write("\n");
                            Write("The shopkeeper offers you " + offer.ToString() + " gold" + (selectedItem.Amount > 1 ? " for each." : "."));
                            Segment();

                            switch (OptionInput("Back", "Sell")) {

                                case "Back":
                                    goto SellInterface;

                                case "Sell":
                                    Console.Clear();
                                    if (selectedItem.Amount > 1) Write("How " + selectedItem.ManyMuch + " " + selectedItem.Plural + " would you like to sell?  (" + selectedItem.Amount.ToString() + ")");
                                    int amount = selectedItem.Amount > 1 ? SelectFromRange(1, selectedItem.Amount) : 1;

                                    int batchPrice = offer * amount;
                                    if (batchPrice <= shopkeeper.Gold) {

                                        Item item = selectedItem;
                                        Console.Clear();

                                        if (selectedItem.Amount > 1) Write("You sold " + amount.ToString() + " " + selectedItem.Plural + " for " + batchPrice.ToString() + " gold.");
                                        else Write("You sold " + selectedItem.name + " for " + batchPrice.ToString() + " gold.");

                                        shopkeeper.stock.AddStack(selectedItem.DeepClone(), amount); //Creates an exact copy of the item.
                                        player.Items.RemoveStack(selectedItem, amount);

                                        shopkeeper.Gold -= batchPrice;
                                        player.Gold += batchPrice;

                                        Read();
                                        Console.Clear();
                                        goto SellInterface;
                                    }
                                    else {
                                        Console.Clear();
                                        Write("The shopkeeper does not have enough gold to buy this.");
                                        Read();
                                        Console.Clear();
                                        goto ItemOptionsInterface;
                                    }
                            }
                            break;
                    }
                    break;

                case "Steal":

                    StealInterface:

                    Console.Clear();
                    Write("Shopkeeper - Steal");
                    Segment();

                    switch (OptionInput("Back", "Threaten", "Attack", "Take")) {

                        case "Back":
                            Console.Clear();
                            goto MeetInterface;

                        case "Threaten":

                            break;

                        case "Attack":

                            break;

                        case "Take":

                            TakeInterface:

                            Console.Clear();
                            if (shopkeeper.stock.Count > 0) Write("On display you see some items you could steal.");
                            else Write("You cannot see any items on display.");
                            Segment();

                            Write("1: Back");
                            Segment();

                            List<string> leftSide = new List<string>();
                            for (int i = 0; i < shopkeeper.stock.Count; i++) {
                                leftSide.Add((i + 2).ToString() + ": " + shopkeeper.stock[i].Display);
                            }
                            List<string> rightSide = new List<string>();
                            for (int i = 0; i < shopkeeper.stock.Count; i++) {
                                rightSide.Add(" [Steal chance: " + shopkeeper.StealChance(shopkeeper.stock[i]) + "%]");
                            }

                            DisplaySides(leftSide, rightSide, 1);

                            int selectedSteal = SelectFromRange(1, shopkeeper.stock.Count + 1);
                            switch (selectedSteal) {

                                case 1:

                                    Console.Clear();
                                    goto StealInterface;

                                default:

                                    Item stealItem = shopkeeper.stock[selectedSteal - 2];

                                    int stealChance = shopkeeper.StealChance(stealItem);

                                    stealItem.DisplayInfo();
                                    Write("\n");
                                    Segment();
                                    Write("Steal chance: " + stealChance + "%");
                                    Segment();

                                    switch (OptionInput("Back", "Steal")) {

                                        case "Back":

                                            Console.Clear();
                                            goto TakeInterface;

                                        case "Steal":

                                            Console.Clear();

                                            Random random = new Random();
                                            Write("How " + stealItem.ManyMuch + " " + stealItem.Plural + " would you like to steal? (" + stealItem.Amount + ") [" + stealChance + "%]");
                                            int amount = SelectFromRange(1, stealItem.Amount);

                                            if (random.Next(1, 100) <= stealChance) {
                                                Console.Clear();
                                                Write("You stole without getting caught!");

                                                player.Items.AddStack(stealItem.DeepClone(), amount);
                                                shopkeeper.stock.RemoveStack(stealItem, amount);
                                            }
                                            else {
                                                Console.Clear();
                                                Write("Oh no! You got caught! The keep is anger!");
                                            }
                                            Read();
                                            goto TakeInterface;
                                    }

                                    break;
                            }

                            break;
                    }

                    break;
            }
        }


        static void PlayerOptions() {

            Write("What do you want to do?");
            Segment();

            switch (OptionInput("Keep Moving", "Manage Items", "Manage Character")) {

                case "Keep Moving":
                    break;

                case "Manage Items":
                    ManageItems();
                    break;
                case "Manage Character":
                    ManageCharacter();
                    break;
            }
        }

        static void ManageCharacter() {

            ManageInterface:
            Console.Clear();
            switch (OptionInput("Back", "Equipment", "Skills")) {

                case "Back":
                    PlayerOptions();
                    break;
                case "Equipment":

                    player.body.EquipInterface();
                    ManageCharacter();

                    break;
                case "Skills":
                    break;

            }
        }

        static void ManageItems() {

            ManageInterface:

            Console.Clear();

            switch (OptionInput("Back", "All Items", "Consumables")) {
                
                case "Back":

                    PlayerOptions();

                    break;

                case "All Items":

                    ItemInterface:

                    Write("1: Back");
                    Segment();
                    for (int i = 0; i < player.Items.Count; i++) {
                        Write((i + 2).ToString() + ": " + player.Items[i].Display);
                    }
                    while (true) {

                        string input2 = Read();
                        if (input2 == "1") {
                            goto ManageInterface;
                        }
                        else for (int i = 0; i < player.Items.Count; i++) {
                            if (input2 == (i + 2).ToString())
                            {
                                ItemOptions(player.Items[i]);
                                Console.Clear();
                                goto ItemInterface;
                            }
                        }
                    }

                case "Consumables":

                    break;
            }
        }

        public static void ItemOptions (Item item) {
            
            ItemOptionsInterface:

            item.DisplayInfo();
            /* Debug message */
            Write(item.consumable ? "This is consumable!" : "Not consumable.");
            Write("\n");

            List<string> options = new List<string>() { "Back", "Destroy", "Inspect" };

            if (item.equippable) {
                if (player.body.Equipped(item)) options.Add("Unequip");
                else options.Add("Equip");
            }
            if (item.consumable) options.Add(item.consumeMessage);

            string option = OptionInput(options);

            if (option == "Back") {
                Console.Clear();
            }
            else if (option == "Destroy") {

                Console.Clear();

                Write("How " + item.ManyMuch + " " + item.Plural + " would you like to destroy? (" + item.Amount.ToString() + ")");

                int a = SelectFromRange(1, item.Amount);

                Console.Clear();
                if (a == 1) Write("You removed " + item.name + " from your inventory.");
                else Write("You removed " + a.ToString() + " " + item.Plural + " from your inventory");

                //Unequips the item from the player before destroying.
                player.body.Unequip(item);

                player.Items.RemoveStack(item, a);
                Segment();
                Read();
                Console.Clear();

                //If all of the items are now destroyed, go back to the item select menu.
                if (item.Amount > 0 && player.Items.Contains(item)) goto ItemOptionsInterface;
            }
            else if (option == "Inspect") {
                Console.Clear();
                Write("It looks like this is an item. Coolio!");
                Read();
            }
            else if (option == "Equip") {
                Console.Clear();
                player.body.Equip(item);
            }
            else if (option == "Unequip") {
                Console.Clear();
                player.body.Unequip(item);
                Read();
            }
            else if (option == item.consumeMessage) {
                Console.Clear();

                item.OnConsume(Program.player);
                int amount = item.Amount;
                player.Items.RemoveStack(item, 1);

                Read();
                if (amount - 1 > 0) goto ItemOptionsInterface;
            }
        }

        static void PlayerDeath() {
            Segment();
            Write("You are dead...");
            Segment();
            Write("\nWould you like to restart?\n\nY/N\n");
            while (true) {
                string input = Read();
                if (input.ToUpper() == "Y" || input.ToUpper() == "YES") {
                    Game();
                }
                if (input.ToUpper() == "N" || input.ToUpper() == "NO") {
                    Environment.Exit(0);
                    break;
                }
            }
        }
    }

    public class Shopkeeper : Entity {

        public List<Item> stock = new List<Item>();
        public List<Item> owned = new List<Item>(); //The shopkeeper's items that are not for sale.

        public override List<Item> Inventory() {
            List<Item> inventory = new List<Item>();
            inventory.AddRange(stock);
            inventory.AddRange(owned);
            return inventory;
        }

        public int Gold {
            get {
                return owned.AmountOfName("Gold");
            }
            set {
                if (value < 0) value = 0;
                owned.RemoveAllOfName("Gold");
                owned.AddStack(Items.GetItem("Gold"), value);
            }
        }

        public int Offer(Item item) {
            //Create an offer for an item the player wants to sell.

            if (item.baseCost <= 1) return item.baseCost;
            return (int)(item.baseCost * 0.7f);
        }

        public int StealChance(Item item) {
            return 50;
        }

        public static Shopkeeper GenerateShopkeeper(int level) {

            Shopkeeper keep = new Shopkeeper();
            Random random = new Random();

            keep.Gold = level * 50 + random.Next(-level * 25, level * 25);

            /*int amount = random.Next(1, 7);
            if (amount <= 2) amount = random.Next(1, 4);
            if (amount >= 6) amount = random.Next(3, 7);*/
            int amount = random.Next(5, 10);

            //keep.stock.AddStack(Program.ItemsInLootPool("crafting").RandomAmount(amount));

            //keep.stock = Program.ItemsInPool("gold").RandomAmount(amount);
            /*List<Item> lootStock = Program.ItemsInLootPool("crafting");
            Program.Write(lootStock.Count);
            for (int i = 0; i < lootStock.Count; i++)
            {
                Program.Write(lootStock[i].name);
            }*/

            //Duplicates one item. This is for testing purposes to test how stacks work in the shopkeeper's inventory.
            for (int i = 0; i < keep.stock.Count; i++) {
                if (keep.stock[i].CanStack) {
                    keep.stock[i].Amount += random.Next(50, 100);
                }
            }

            return keep;
        }
    }

    [Serializable]
    public class Consumable : Item
    {

        public Consumable()
        {
            consumable = true;
            consumeMessage = "Consume";
        }
        public Consumable(string _consumeMessage)
        {
            consumable = true;
            consumeMessage = _consumeMessage;
        }
    }



    public class Player : Entity {

        public Player() {
            //equippedLeftHand = fist;
            //equippedRightHand = fist;
            body = BodyModule.Humanoid;
        }

        public Weapon equippedWeapon;
        private List<Item> items = new List<Item>();

        public override List<Item> Inventory (){
            return items;
        }

        public List<Item> Items {
            get {
                return items;
            }
            private set { items = value; }
        }

        public int Gold {
            //Gold is treated like a normal item to avoid inconsistency and exceptions.
            //Gold's ID is 0.
            get { return items.AmountOfName("Gold"); }
            set {
                if (value < 0) value = 0;
                items.RemoveAllOfName("Gold");
                items.AddStack(TextGame.Items.GetItem("Gold"), value);
            }
        }

        //Equipped weapon defaults to fist
        public Weapon fist = new Weapon(EquipType.Smallheld) { name = "Fist", OnHit = (Entity _entity) => _entity.Damage(10) };

        //Handle equipping items by changing equip modules

        /*public bool Equipped(Item item)
        {
            if (equippedLeftHand == item) return true;
            if (equippedRightHand == item) return true;
            return false;
        }
        public void Unequip(Item item) 
        {
            if (equippedLeftHand == item) equippedLeftHand = fist;
            if (equippedRightHand == item) equippedRightHand = fist;
            Program.Write("You unequipped " + item.name + ".");
        }
        public void Equip(Item item)
        {
            Console.Clear();

            switch (item.equipType)
            {
                case EquipType.None:

                    Program.Write("You cannot equip " + item.name + ".");
                    break;

                case EquipType.Smallheld:

                    Program.Write("Which hand do you want to equip " + item.name + " in?");
                    Program.Segment();
                    switch (Program.OptionInput("Left Hand", "Right Hand"))
                    {
                        case "Left Hand":
                            equippedLeftHand = item;
                            if (equippedRightHand.equipType == EquipType.Mediumheld) equippedRightHand = fist;
                            break;
                        case "Right Hand":
                            equippedRightHand = item;
                            if (equippedLeftHand.equipType == EquipType.Mediumheld) equippedLeftHand = fist;
                            break;
                    }
                    Console.Clear();
                    Program.Write("You equipped " + item.name + " in one hand.");
                    break;

                case EquipType.Mediumheld:

                    Program.Write("You equipped " + item.name + " in both hands.");
                    equippedLeftHand = item;
                    equippedRightHand = item;
                    break;
            }
        }*/

    }
    [Serializable]
    public abstract class Entity
    {

        public string name = "";
        public int maxHP = 100;
        public int HP = 100;
        public bool alive = true;

        //Entity classes implement an inventory which is a way to find all items that are owned or being held by that entity.
        public abstract List<Item> Inventory();

        public BodyModule body = BodyModule.Humanoid;

        public void Damage(int d)
        {
            HP -= d;
            if (HP < 0)
            {
                HP = 0;
                Kill();
            }
        }
        public void Kill()
        {
            HP = 0;
            alive = false;
        }
    }

}

namespace ExtensionMethods {

    public static class ListExtensions {
        ///<summary>
        ///An alternate Add function for lists of objects that inheret from IStackable. Items that can stack 
        ///and have the same ID will instead change the amount variable in the one that is already in the list.
        ///</summary>
        public static void AddStack<T>(this List<T> objs, T obj, int? overrideAmount = null) where T : IStackable {

            if (obj.CanStack) {
                for (int i = 0; i < objs.Count; i++) {
                    if (objs[i].Name == obj.Name) {
                        objs[i].Amount += overrideAmount ?? obj.Amount;
                        return;
                    }
                }
            }
            if (overrideAmount != null) obj.Amount = (int)overrideAmount;
            if (obj.Amount > 0) objs.Add(obj);
        }

        public static void AddStack<T>(this List<T> objs, List<T> addObjs) where T : IStackable {

            for (int i = 0; i < addObjs.Count; i++) {
                objs.AddStack(addObjs[i]);
            }

        }

        /// <summary>
        /// Stacks a list of IStackables to the most compact possible stack.
        /// </summary>
        public static List<T> Stack<T>(List<T> objs) where T : IStackable {
            List<T> stacked = new List<T>();
            for (int i = 0; i < objs.Count; i++) {
                stacked.AddStack(objs[i]);
            }
            return stacked;
        }


        public static int AmountOfName<T>(this List<T> objs, string name) where T : IStackable {
            int amount = 0;
            for (int i = 0; i < objs.Count; i++) {
                if (objs[i].Name == name) amount += objs[i].Amount;
            }
            return amount;
        }

        public static void RemoveAllOfName<T>(this List<T> objs, string name) where T : IStackable {
            for (int i = 0; i < objs.Count; i++) {
                if (objs[i].Name == name) {
                    objs.Remove(objs[i]);
                    i--;
                }
            }
        }

        public static T DeepClone<T>(this T a) {
            //Creates an exact clone of an object that has new space in memory.
            using (MemoryStream stream = new MemoryStream()) {

                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(stream, a);
                stream.Position = 0;
                return (T)formatter.Deserialize(stream);
            }
        }

        public static void RemoveStack<T>(this List<T> objs, T obj, int? overrideAmount = null) where T : IStackable {
            int amountToRemove = overrideAmount ?? obj.Amount;

            if (obj.CanStack) {
                for (int i = 0; i < objs.Count; i++) {
                    if (objs[i].Name == obj.Name) {

                        int tempAmount = objs[i].Amount;
                        objs[i].Amount -= amountToRemove;
                        amountToRemove -= tempAmount;
                        if (objs[i].Amount <= 0) objs.Remove(objs[i]);
                        return;
                    }
                }
            }
            objs.Remove(obj);
        }
        public static string[] Displays(this List<Item> items) {

            string[] displays = new string[items.Count];
            for (int i = 0; i < displays.Length; i++)
            {
                displays[i] = items[i].Display;
            }
            return displays;
        }

        //Select a random amount of objects from a list 
        static Random randomAmountRandom = new Random();
        public static List<T> RandomAmount<T>(this List<T> objects, int amount) where T : IStackable {

            List<T> randomObjects = new List<T>();

            for (int i = 0; i < amount; i++) {

                T randomObject = objects[randomAmountRandom.Next(0, objects.Count)];

                randomObjects.AddStack(randomObject);
            }
            return randomObjects;
        }


        public static string UpperFirst(this string str) {
            if (String.IsNullOrWhiteSpace(str)) return str;
            else if (str.Length > 1) return char.ToUpper(str[0]) + str.Substring(1);
            return str.ToUpper();
        }
    }
}
