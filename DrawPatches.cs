using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewModdingAPI;
using StardewValley;
using Rectangle = Microsoft.Xna.Framework.Rectangle;

namespace GuniismUnicycle
{
    internal class DrawPatches
    {
        public static int index = 0;

        // patches need to be static!
        public static void Prefix(Farmer __instance, SpriteBatch b)
        {
            int FacingDirection = 0;
            
            if (__instance.modData.ContainsKey(ModEntry.UnicyclingKey))
            {
                
                Vector2 offset = new(0, 32);

                __instance.drawOffset = new Vector2(0, -25);

                
                switch (__instance.FacingDirection)
                {
                    case 0:
                        FacingDirection = 0;
                        break;
                    case 1:
                        FacingDirection = 1;
                        break;
                    case 2:
                        FacingDirection = 2;
                        break;
                    case 3:
                        FacingDirection = 3;
                        break;
                }

                if (__instance.isMoving() == true)
                {
                    index = ModEntry.CurrentAnimationIndex;
                }
                else
                {
                    index = 0;
                }

                b.Draw(ModEntry.UnicycleTexture, Game1.GlobalToLocal(__instance.Position) - offset, new Rectangle(index * 16, (FacingDirection * 16) + 64, 16, 16), Color.White, 0, Vector2.Zero, 4, SpriteEffects.None, __instance.getDrawLayer() - 0.00001f);

                if (__instance.IsMale == false)
                {
                    if (FacingDirection == 1)
                    {
                        FacingDirection = 8;
                    }
                    if (FacingDirection == 3)
                    {
                        FacingDirection = 9;
                    }
                }

                b.Draw(ModEntry.UnicycleTexture, Game1.GlobalToLocal(__instance.Position) - offset, new Rectangle(index * 16, FacingDirection * 16, 16, 16), Color.White, 0, Vector2.Zero, 4, SpriteEffects.None, __instance.getDrawLayer() + 0.00001f);

                foreach (Farmer farmer in Game1.getOnlineFarmers())
                {
                    if (farmer.modData.ContainsKey(ModEntry.UnicyclingKey))
                    {
                        if (farmer.CanMove && farmer.isMoving() && !farmer.swimming.Value && !farmer.IsEmoting && !farmer.UsingTool && !farmer.usingSlingshot)  
                        {
                            farmer.Halt();
                            switch (farmer.FacingDirection)
                            {
                                case 0:
                                    if (farmer.IsCarrying())
                                    {
                                        farmer.FarmerSprite.setCurrentSingleAnimation(144);
                                    }
                                    else
                                    {
                                        farmer.FarmerSprite.setCurrentSingleAnimation(12);
                                    }     
                                    break;
                                case 1:
                                    if (farmer.IsCarrying())
                                    {
                                        farmer.FarmerSprite.setCurrentSingleAnimation(136);
                                    }
                                    else
                                    {
                                        farmer.FarmerSprite.setCurrentSingleAnimation(6);
                                    }
                                    break;
                                case 2:
                                    if (farmer.IsCarrying())
                                    {
                                        farmer.FarmerSprite.setCurrentSingleAnimation(128);
                                    }
                                    else
                                    {
                                        farmer.FarmerSprite.setCurrentSingleAnimation(-1);
                                    }
                                    break;
                                case 3:
                                    if (farmer.IsCarrying())
                                    {
                                        farmer.FarmerSprite.setCurrentSingleAnimation(152);
                                    }
                                    else
                                    {
                                        farmer.FarmerSprite.setCurrentSingleAnimation(88);
                                    }
                                    break;
                            }
                        }
                        
                        
                    }
                }
            }
            else
            {
                __instance.drawOffset = new Vector2(0, 0);
            }
        }
    }
}