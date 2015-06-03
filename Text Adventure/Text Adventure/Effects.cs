using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExtensionMethods;

namespace TextGame {

    public static class Effects {

        public static void Heal(Entity entity, int amount) {
            entity.HP += amount;
            if (entity.HP > entity.maxHP) {
                entity.HP = entity.maxHP;
            }
            Program.Write(entity.name + " healed " + amount + " HP.");
            Program.Read();
        }
    }
}
