using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewModdingAPI;
using StardewValley;
using StardewModdingAPI.Events;
using StardewValley.GameData.Objects;
using StardewValley.Buffs;
using StardewValley.GameData.Shops;
using System.Threading;
using StardewValley.Locations;

namespace GuniismUnicycle
{
    /// <summary>The mod entry point.</summary>
    public partial class ModEntry : Mod
    {
        public static int CurrentAnimationIndex { get; set; } = 0;

        public static readonly string UnicyclingKey = "Guniism.GuniismUnicycle_GuniismUnicycling";

        public static Texture2D? UnicycleTexture { get; set; }

        private bool isUnicycleSelected = false;
        private Texture2D? buffIcon;
        private Texture2D? UnicycleIcon;

        private Buff? buff;
        private Buff? cancelBuff;

        private ModConfig Config = null!;

        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            Config = Helper.ReadConfig<ModConfig>();

            helper.Events.Input.ButtonPressed += OnButtonPressed;
            helper.Events.Content.AssetRequested += OnAssetRequested;
            helper.Events.GameLoop.UpdateTicked += OnUpdateTicked;
            helper.Events.GameLoop.GameLaunched += OnGameLaunched;

            buffIcon = helper.ModContent.Load<Texture2D>("assets/UnicycleBuff.png");
            UnicycleIcon = helper.ModContent.Load<Texture2D>("assets/UnicycleIcon.png");
            UnicycleTexture = helper.ModContent.Load<Texture2D>("assets/UnicycleTexture.png");

            var harmony = new Harmony(this.ModManifest.UniqueID);
            harmony.Patch(
            original: AccessTools.Method(typeof(Farmer), nameof(Farmer.draw), new Type[] { typeof(SpriteBatch) }),
            prefix: new HarmonyMethod(typeof(DrawPatches), nameof(DrawPatches.Prefix))
            );
        }

        private void OnUpdateTicked(object? sender, UpdateTickedEventArgs e)
        {
            if (!Context.IsWorldReady || !Game1.player.IsLocalPlayer)
                return;

            if (Game1.player != null && Game1.activeClickableMenu == null && !Game1.player.modData.ContainsKey(UnicyclingKey))
            {
                if (Game1.player.Items.ContainsId("Guniism.GuniismUnicycle_GuniismUnicycle"))
                {
                    if (Game1.player.CurrentItem == null)
                    {
                        isUnicycleSelected = false;

                    }
                    else if (Game1.player.CurrentItem.Name == "GuniismUnicycle")
                    {
                        isUnicycleSelected = true;
                    }
                    else
                    {
                        isUnicycleSelected = false;
                    }
                }
                else
                {
                    isUnicycleSelected = false;
                }
            }

            if (Game1.player == null)
            {

            }
            else if (e.IsMultipleOf(8) && Game1.player.isMoving() == true)
            {
                CurrentAnimationIndex += 1;
                if (CurrentAnimationIndex >= 4)
                {
                    CurrentAnimationIndex = 0;
                }
            }

            if (Game1.player != null && Game1.player.modData.ContainsKey(UnicyclingKey) && Game1.currentLocation != null)
            {
                if ((Game1.currentLocation.IsOutdoors == false || (bool)Game1.player.bathingClothes.Value || Game1.player.swimming.Value || Game1.player.isRidingHorse()))
                {
                    cancelBuff = new Buff(
                        id: "Guniism.GuniismUnicycle_GuniismUnicycleBuff",
                        displayName: "Unicycle",
                        iconSheetIndex: 0,
                        duration: 1_000,
                        iconTexture: null,
                        effects: new BuffEffects()
                        {
                            Speed = { 0 }
                        }
                    );

                    Item myCustomItem = new StardewValley.Object("Guniism.GuniismUnicycle_GuniismUnicycle", 1);
                    if (Game1.player.Items.CountItemStacks() < Game1.player.maxItems.Value)
                    {
                        Game1.player.applyBuff(cancelBuff);
                        Game1.player.addItemToInventory(myCustomItem);
                        Game1.player.drawOffset = new Vector2(0, 0);
                        Game1.player.modData.Remove(UnicyclingKey);
                    }
                    else if (Game1.player.Items.CountItemStacks() == Game1.player.maxItems.Value)
                    {
                        if (Game1.player.Items.ContainsId("Guniism.GuniismUnicycle_GuniismUnicycle"))
                        {
                            Game1.player.applyBuff(cancelBuff);
                            Game1.player.addItemToInventory(myCustomItem);
                            Game1.player.drawOffset = new Vector2(0, 0);
                            Game1.player.modData.Remove(UnicyclingKey);
                        }
                        else
                        {
                            Game1.player.drawOffset = new Vector2(0, 0);
                            Game1.player.applyBuff(cancelBuff);
                            Game1.createItemDebris(myCustomItem, Game1.player.Position, 1, Game1.player.currentLocation);
                            Game1.player.modData.Remove(UnicyclingKey);
                        }
                    }
                }
            }
        }

        private void OnButtonPressed(object? sender, ButtonPressedEventArgs e)
        {
            if (!Context.IsWorldReady)
                return;

            if (isUnicycleSelected == true)
            {
                buff = new Buff(
                    id: "Guniism.GuniismUnicycle_GuniismUnicycleBuff",
                    displayName: "Unicycle",
                    iconTexture: buffIcon,
                    iconSheetIndex: 0,
                    duration: -2,
                    effects: new BuffEffects()
                    {
                        Speed = { Convert.ToInt32(Config.Speed) }
                    }
                );
                cancelBuff = new Buff(
                    id: "Guniism.GuniismUnicycle_GuniismUnicycleBuff",
                    displayName: "Unicycle",
                    iconSheetIndex: 0,
                    duration: 1_000,
                    iconTexture: null,
                    effects: new BuffEffects()
                    {
                        Speed = { 0 }
                    }
                );
            }
            if (Game1.activeClickableMenu == null)
            {
                if ((e.Button == Config.RideButton) && (isUnicycleSelected == true) && (!Game1.player.modData.ContainsKey(UnicyclingKey)) && (Game1.currentLocation.IsOutdoors == true))
                {
                    Game1.player.applyBuff(buff);
                    isUnicycleSelected = false;
                    Game1.player.Items.ReduceId("Guniism.GuniismUnicycle_GuniismUnicycle", 1);
                    Game1.player.drawOffset = new Vector2(0, -25);
                    Game1.player.modData[UnicyclingKey] = "true";
                }
                else if ((e.Button == Config.RideButton) && (Game1.player.modData.ContainsKey(UnicyclingKey)))
                {
                    Item myCustomItem = new StardewValley.Object("Guniism.GuniismUnicycle_GuniismUnicycle", 1);

                    if (Game1.player.Items.CountItemStacks() < Game1.player.maxItems.Value)
                    {
                        isUnicycleSelected = false;
                        Game1.player.applyBuff(cancelBuff);
                        Game1.player.addItemToInventory(myCustomItem);
                        Game1.player.drawOffset = new Vector2(0, 0);
                        Game1.player.modData.Remove(UnicyclingKey);
                    }
                    else if (Game1.player.Items.CountItemStacks() == Game1.player.maxItems.Value)
                    {
                        if (Game1.player.Items.ContainsId("Guniism.GuniismUnicycle_GuniismUnicycle"))
                        {
                            Game1.player.applyBuff(cancelBuff);
                            Game1.player.addItemToInventory(myCustomItem);
                            Game1.player.drawOffset = new Vector2(0, 0);
                            Game1.player.modData.Remove(UnicyclingKey);
                        }
                        else
                        {
                            Game1.addHUDMessage(new HUDMessage("Inventory Full", HUDMessage.error_type));
                            Game1.player.modData[UnicyclingKey] = "true";
                        }
                    }
                }
                if ((e.Button == Config.RideButton) && (isUnicycleSelected == true) && (!Game1.player.modData.ContainsKey(UnicyclingKey)) && (Game1.currentLocation.IsOutdoors == false))
                {
                    Game1.addHUDMessage(new HUDMessage("You are in the building.", HUDMessage.error_type));
                }
            }
        }
        private void OnAssetRequested(object? sender, AssetRequestedEventArgs e)
        {
            if (e.NameWithoutLocale.IsEquivalentTo("Data/Objects", false))
            {
                e.Edit((Action<IAssetData>)delegate (IAssetData asset)
                {
                    IDictionary<string, ObjectData> data = ((IAssetData<IDictionary<string, ObjectData>>)(object)asset.AsDictionary<string, ObjectData>()).Data;
                    data.TryAdd("Guniism.GuniismUnicycle_GuniismUnicycle", new ObjectData
                    {
                        Name = "GuniismUnicycle",
                        DisplayName = "Unicycle",
                        Description = "An electric Unicycle in your inventory to increase speed.",
                        Price = 500,
                        Texture = "Mods/Guniism.GuniismUnicycle_GuniismUnicycle/Data",
                        Edibility = -300,
                        Type = "objects",
                        Category = -98,
                        ExcludeFromRandomSale = true,
                        ExcludeFromFishingCollection = true,
                        ExcludeFromShippingCollection = true,
                    });
                }, (AssetEditPriority)0, null);
            }
            if (e.NameWithoutLocale.IsEquivalentTo("Data/Shops", false))
            {
                e.Edit((Action<IAssetData>)delegate (IAssetData asset)
                {
                    IDictionary<string, ShopData> data = ((IAssetData<IDictionary<string, ShopData>>)(object)asset.AsDictionary<string, ShopData>()).Data;
                    data["Blacksmith"].Items.Add(new ShopItemData
                    {
                        Price = Convert.ToInt32(Config.RecipePrice),
                        ItemId = "Guniism.GuniismUnicycle_GuniismUnicycle",
                        IsRecipe = true,
                        IgnoreShopPriceModifiers = true,
                    });
                });
            }
            if (e.NameWithoutLocale.IsEquivalentTo("Mods/Guniism.GuniismUnicycle_GuniismUnicycle/Data", false))
            {
                e.LoadFromModFile<Texture2D>("assets/UnicycleIcon.png", AssetLoadPriority.Medium);
            }
            if (e.NameWithoutLocale.IsEquivalentTo("Data/CraftingRecipes"))
            {
                e.Edit((Action<IAssetData>)delegate (IAssetData asset)
                {
                    IDictionary<string, string> data = asset.AsDictionary<string, string>().Data;
                    data.Add("GuniismUnicycle", $"{Config.CraftingRecipe}/Field/Guniism.GuniismUnicycle_GuniismUnicycle/false/null/");
                });
            }
        }

        private void OnGameLaunched(object? sender, GameLaunchedEventArgs e)
        {
            // get Generic Mod Config Menu's API (if it's installed)
            var configMenu = this.Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
            if (configMenu is null)
                return;

            // register mod
            configMenu.Register(
                mod: this.ModManifest,
                reset: () => Config = new ModConfig(),
                save: () => this.Helper.WriteConfig(Config)
            );

            // add some config options
            configMenu.AddKeybind(
                mod: ModManifest,
                name: () => "Ride Button",
                getValue: () => Config.RideButton,
                setValue: value => Config.RideButton = value
            );
            configMenu.AddNumberOption(
                mod: this.ModManifest,
                name: () => "Speed",
                getValue: () => Config.Speed,
                setValue: value => Config.Speed = value,
                min: 0,
                max: 20
            );
            configMenu.AddNumberOption(
                mod: this.ModManifest,
                name: () => "Recipe Price",
                getValue: () => Config.RecipePrice,
                setValue: value => Config.RecipePrice = value
            );
            configMenu.AddTextOption(
                mod: this.ModManifest,
                name: () => "Crafting Recipe",
                getValue: () => Config.CraftingRecipe,
                setValue: value => Config.CraftingRecipe = value
            );

        }
    }
}
