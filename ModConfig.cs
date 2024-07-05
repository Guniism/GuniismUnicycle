using StardewModdingAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuniismUnicycle
{
    public class ModConfig
    {
        public SButton RideButton { get; set; } = SButton.MouseRight;
        public int Speed { get; set; } = 6;
        public int RecipePrice { get; set; } = 5000;
        public string CraftingRecipe { get; set; } = "787 1 337 1 335 5 382 10";
    }
}