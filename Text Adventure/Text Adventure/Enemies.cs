using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExtensionMethods;

namespace TextGame {

    [Serializable]
    public class RandomTable<T> {

        public bool weighted = false;
        public int percentage = 100;
        public int weight = 1;
        public List<RandomTable<T>> branchTables = new List<RandomTable<T>>();
        public List<T> objects = new List<T>();

        Random random = new Random();

        public RandomTable () { }

        public RandomTable (List<string> parse){
            for (int i = 0; i < parse.Count; i++) {

            }
        }

        public RandomTable(List<T> _objects) {
            objects = _objects;
        }

        public int SelectChance(int[] numbers) {
            //Returns the index of the number which is randomly selected based on it's value.

            int[] addedNumbers = new int[numbers.Length];
            int totalNumbers = 0;
            for (int i = 0; i < numbers.Length; i++) {
                totalNumbers += numbers[i];
                addedNumbers[i] = totalNumbers;
            }

            int rand = random.Next(1, totalNumbers > 0 ? totalNumbers : 1);
            for (int i = 0; i < addedNumbers.Length; i++) {
                if ((i == 0 ? rand > 0 : rand > addedNumbers[i - 1]) && rand <= addedNumbers[i]) {
                    return i;
                }
            }
            return 0;
        }

        public List<T> Drops() {
            List<T> drops = new List<T>();

            drops.AddRange(objects);

            if (branchTables.Count > 0) {
                List<RandomTable<T>> weightBranches = branchTables.Where(x => x.weighted).ToList();
                List<RandomTable<T>> percentBranches = branchTables.Where(x => !x.weighted).ToList();

                //Select a random branch depending on the weights of the branches.

                int selectedIndex = SelectChance(weightBranches.Select(x => x.weight).ToArray());
                if (weightBranches.Count > 0) {
                    drops.AddRange(weightBranches[selectedIndex].Drops());
                }

                for (int i = 0; i < percentBranches.Count; i++) {
                    int rand = random.Next(1, 100);
                    if (rand <= percentBranches[i].percentage) {
                        drops.AddRange(percentBranches[i].Drops());
                    }
                }
            }
            return drops;
        }

    }

    [Serializable]
    public class Enemy : Entity {

        public int id;

        public List<Item> items = new List<Item>();

        public override List<Item> Inventory() {
            return items;
        }

        public RandomTable<Item> lootTable = new RandomTable<Item>();

        public int damage = 5;

        public Enemy() {

        }

        public void Attack(Entity entity) {
            entity.Damage(damage);
            Program.Write("<-- (" + damage.ToString() + ") " + name + " attacked " + entity.name + " for " + damage + " damage");
        }
    }

    static class Enemies {

        public static Dictionary<string, Enemy> enemies = new Dictionary<string, Enemy>();

        public static Enemy GetEnemy(string name) {
            try {
                Enemy enemy = enemies[name].DeepClone();
                return enemy;
            }
            catch {
                Console.WriteLine("ERROR: Enemies.cs - GetEnemy() - You searched for an enemy (" + name + ") that does not exist.");
                Program.Read();
                return new Enemy();
            }
        }


        public static void Initialize() {

            Enemy e;

            e = new Enemy() {
                name = "Wolf",
                damage = 5,
                HP = 20,
                maxHP = 20
            };
            enemies.Add(e.name, e);

            e = new Enemy() {
                name = "Living Lemon",
                damage = 2,
                HP = 5,
                maxHP = 5,
                lootTable = new RandomTable<Item>(){
                    
                    branchTables = new List<RandomTable<Item>>(){
                        new RandomTable<Item>() {
                            weighted = true,
                            weight = 3,
                            objects = new List<Item>() {
                                Items.GetItem("Gold") 
                            }
                        },
                        new RandomTable<Item>() {
                            percentage = 50,
                            objects = new List<Item>() {
                                Items.GetItem("Knife"),
                                Items.GetItem("Big Sword")
                            }
                        },
                        new RandomTable<Item>() {
                            percentage = 50,
                            objects = new List<Item>() {
                                Items.GetItem("Gold")
                            },
                            branchTables = new List<RandomTable<Item>>(){
                                new RandomTable<Item>() {
                                    percentage = 40,
                                    objects = new List<Item>() {
                                        Items.GetItem("Gold Bar")
                                    }
                                },
                            }
                        }
                    }
                }
            };
            enemies.Add(e.name, e);

        }

    }
}
