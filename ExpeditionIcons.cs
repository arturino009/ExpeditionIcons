using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ExileCore;
using ExileCore.PoEMemory.Components;
using ExileCore.PoEMemory.MemoryObjects;
using ExileCore.Shared;
using ExileCore.Shared.Abstract;
using ExileCore.Shared.Cache;
using ExileCore.Shared.Enums;
using ExileCore.Shared.Helpers;
using ImGuiNET;
using SharpDX;
using Map = ExileCore.PoEMemory.Elements.Map;

namespace ExpeditionIcons
{
    public class Core : BaseSettingsPlugin<Settings>
    {
        //public static List<StoredEntity> storedAreaEntities = new List<StoredEntity>();
        private CachedValue<RectangleF> _mapRect;
        private IngameUIElements ingameStateIngameUi;
        private float k;
        private float scale;
        private Vector2 screentCenterCache;
        private RectangleF MapRect => _mapRect?.Value ?? (_mapRect = new TimeCache<RectangleF>(() => mapWindow.GetClientRect(), 100)).Value;
        private Map mapWindow => GameController.Game.IngameState.IngameUi.Map;
        private Camera camera => GameController.Game.IngameState.Camera;
        private Vector2 screenCenter =>
            new Vector2(MapRect.Width / 2, MapRect.Height / 2 - 20) + new Vector2(MapRect.X, MapRect.Y) +
            new Vector2(mapWindow.LargeMapShiftX, mapWindow.LargeMapShiftY);
        List<Vector3> explosives = new List<Vector3>() { };
        List<Vector3> remnants = new List<Vector3>() { };
        List<Vector3> efficientPoints = new List<Vector3>() { };
        public override bool Initialise()
        {
            CanUseMultiThreading = true;
            return base.Initialise();
        }
        public override Job Tick()
        {
            //if (Settings.MultiThreading)
            //    return GameController.MultiThreadManager.AddJob(TickLogic, nameof(MinimapIcons));

            TickLogic();
            return null;
        }
        private void TickLogic()
        {
            try
            {
                ingameStateIngameUi = GameController.Game.IngameState.IngameUi;

                if (ingameStateIngameUi.Map.SmallMiniMap.IsVisibleLocal)
                {
                    var mapRect = ingameStateIngameUi.Map.SmallMiniMap.GetClientRectCache;
                    screentCenterCache = new Vector2(mapRect.X + mapRect.Width / 2, mapRect.Y + mapRect.Height / 2);
                }
                else if (ingameStateIngameUi.Map.LargeMap.IsVisibleLocal)
                {
                    screentCenterCache = screenCenter;
                }

                k = camera.Width < 1024f ? 1120f : 1024f;
                scale = k / camera.Height * camera.Width * 3f / 4f / mapWindow.LargeMapZoom;
            }
            catch (Exception e)
            {
                DebugWindow.LogError($"ExpeditionIcons.TickLogic: {e.Message}");
            }
        }

        public override void AreaChange(AreaInstance area)
        {
            remnants.Clear();
            efficientPoints.Clear();
        }
        public override void DrawSettings()
        {
            if (ImGui.Button("Calculate optimal explosive placement (will freeze for a while if in a logbook!!!!!!!! also load all runic markers before using)"))
            {
                efficientPoints.Clear();
                getBestPositions();
            }
            base.DrawSettings();
        }

        public override void Render()
        {
            explosives.Clear();
            foreach (var e in GameController.EntityListWrapper.OnlyValidEntities)
            {
                if (e.Path == null) continue;
                if (e.Path.Contains("ExpeditionExplosive") && !e.Path.Contains("Fuse"))
                {
                    var location = e.Pos;
                    location.Z = 0;
                    if (Settings.ShowExplosives.Value) 
                    {
                        DrawEllipseToWorld(location, Settings.ExplosiveRange, 50, 4, Settings.ExplosiveColor);
                    }
                    if (!explosives.Contains(location))
                    {
                        explosives.Add(location);
                    }
                    continue;
                }
            }

            if(efficientPoints.Count != 0)
            {
                foreach(var point in efficientPoints)
                {
                    DrawEllipseToWorld(point, Settings.ExplosiveRange, 50, 4, Settings.OptimalColor);
                }

            }

            //GetComponent
            foreach (var e in GameController.EntityListWrapper.OnlyValidEntities)
            //foreach (var e in GameController.EntityListWrapper.NotOnlyValidEntities)
            {
                if (e.Path == null) continue;
                // var renderComponent = e?.GetComponent<Render>();
                // if (renderComponent == null) continue;
                if (e.Path.Contains("Terrain/Leagues/Expedition/Tiles"))
				{
					var positionedComp = e.GetComponent<Positioned>();
					
					var text = "D";
					
					
					
					//if (modelPath == null) continue;
					//text = text && modelPath.Substring(0, modelPath.IndexOf("."));
						
					var TextInfo = new MinimapTextInfo
					{
						Text = text,
						FontSize = 16,
						FontColor = Color.Black,
						FontBackgroundColor = Color.Orange,
						TextWrapLength = 35
					};
					var ent = new StoredEntity(e.GetComponent<Render>().Z, positionedComp.GridPos, e.Id, TextInfo);
					if (GameController.Game.IngameState.IngameUi.Map.LargeMap.IsVisible)
							DrawToLargeMiniMapText(ent, ent.TextureInfo);
				}
				
				if(e.Path.Contains("ExpeditionMarker"))
				{
					var positionedComp = e.GetComponent<Positioned>();
					//var animatedMetaData = e.GetComponent<Animated>().BaseAnimatedObjectEntity.Metadata;
					var animatedMetaData = e.GetComponent<Animated>().ReadObjectAt<Entity>(0x1C0).Metadata;
                    if (animatedMetaData == null) continue;

					var text = "*";
                    if (animatedMetaData.Contains("elitemarker"))
                    {
                        text = "M";
                        //if (modelPath == null) continue;
                        //text = text && modelPath.Substring(0, modelPath.IndexOf("."));

                        var TextInfo = new MinimapTextInfo
                        {
                            Text = text,
                            FontSize = 16,
                            FontColor = Color.Yellow,
                            FontBackgroundColor = Color.Black,
                            TextWrapLength = 35
                        };

                        var location = e.Pos;
                        location.Z = 0;
                        var showElement = true;
                        foreach (var explosive in explosives)
                        {
                            if (Vector3.Distance(explosive, location) < Settings.ExplosiveRange + 20)
                            {
                                showElement = false;
                                break;
                            }
                        }
                        if (showElement)
                        {
                            DrawEllipseToWorld(location, 12, 15, 8, Color.Red);
                        }

                        var ent = new StoredEntity(e.GetComponent<Render>().Z, positionedComp.GridPos, e.Id, TextInfo);
                        if (GameController.Game.IngameState.IngameUi.Map.LargeMap.IsVisible && showElement){
                            DrawToLargeMiniMapText(ent, ent.TextureInfo);

                        if (!remnants.Contains(location))
                        {
                            remnants.Add(location);
                        }


                            // Graphics.DrawText(text, textPos, textColor, TextSize, FontAlign.Center);
                        } 
                    }
					// if (animatedMetaData.Contains("monstermarker"))
					// {
						// var background = Color.Orange;
					// //if (modelPath == null) continue;
					// //text = text && modelPath.Substring(0, modelPath.IndexOf("."));
						
						// var TextInfo = new MinimapTextInfo
						// {
							// Text = text,
							// FontSize = 20,
							// FontColor = Color.Red,
							// FontBackgroundColor = Color.Transparent,
							// TextWrapLength = 50
						// };
						// var ent = new StoredEntity(e.GetComponent<Render>().Z, positionedComp.GridPos, e.Id, TextInfo);
						// if (GameController.Game.IngameState.IngameUi.Map.LargeMap.IsVisible)
							// DrawToLargeMiniMapText(ent, ent.TextureInfo);
					// }
					if (animatedMetaData.Contains("chestmarker3"))
					{
						var background = Color.Orange;
					//if (modelPath == null) continue;
					//text = text && modelPath.Substring(0, modelPath.IndexOf("."));
						
						var TextInfo = new MinimapTextInfo
						{
							Text = text,
							FontSize = 20,
							FontColor = Color.Orange,
							FontBackgroundColor = Color.Orange,
							TextWrapLength = 50
						};
                        var location = e.Pos;
                        location.Z = 0;
                        var showElement = true;
                        foreach (var explosive in explosives)
                        {
                            if (Vector3.Distance(explosive, location) < Settings.ExplosiveRange + 20)
                            {
                                showElement = false;
                                break;
                            }
                        }

                        var ent = new StoredEntity(e.GetComponent<Render>().Z, positionedComp.GridPos, e.Id, TextInfo);
						if (GameController.Game.IngameState.IngameUi.Map.LargeMap.IsVisible && showElement)
							DrawToLargeMiniMapText(ent, ent.TextureInfo);
					}
					if (animatedMetaData.Contains("chestmarker2"))
					{
						var background = Color.Orange;
					//if (modelPath == null) continue;
					//text = text && modelPath.Substring(0, modelPath.IndexOf("."));
						
						var TextInfo = new MinimapTextInfo
						{
							Text = text,
							FontSize = 20,
							FontColor = Color.Yellow,
							FontBackgroundColor = Color.Yellow,
							TextWrapLength = 50
						};
                        var location = e.Pos;
                        location.Z = 0;
                        var showElement = true;
                        foreach (var explosive in explosives)
                        {
                            if (Vector3.Distance(explosive, location) < Settings.ExplosiveRange + 20)
                            {
                                showElement = false;
                                break;
                            }
                        }
                        var ent = new StoredEntity(e.GetComponent<Render>().Z, positionedComp.GridPos, e.Id, TextInfo);
						if (GameController.Game.IngameState.IngameUi.Map.LargeMap.IsVisible && showElement)
							DrawToLargeMiniMapText(ent, ent.TextureInfo);
					}
					if (animatedMetaData.Contains("chestmarker1"))
					{
						var background = Color.Orange;
					//if (modelPath == null) continue;
					//text = text && modelPath.Substring(0, modelPath.IndexOf("."));
						
						var TextInfo = new MinimapTextInfo
						{
							Text = text,
							FontSize = 20,
							FontColor = Color.White,
							FontBackgroundColor = Color.White,
							TextWrapLength = 50
						};
                        var location = e.Pos;
                        location.Z = 0;
                        var showElement = true;
                        foreach (var explosive in explosives)
                        {
                            if (Vector3.Distance(explosive, location) < Settings.ExplosiveRange + 20)
                            {
                                showElement = false;
                                break;
                            }
                        }
                        var ent = new StoredEntity(e.GetComponent<Render>().Z, positionedComp.GridPos, e.Id, TextInfo);
						if (GameController.Game.IngameState.IngameUi.Map.LargeMap.IsVisible && showElement)
							DrawToLargeMiniMapText(ent, ent.TextureInfo);
					}
					if (animatedMetaData.Contains("chestmarker_signpost"))
					{
						var background = Color.Orange;
					//if (modelPath == null) continue;
					//text = text && modelPath.Substring(0, modelPath.IndexOf("."));
						
						var TextInfo = new MinimapTextInfo
						{
							Text = text,
							FontSize = 20,
							FontColor = Color.White,
							FontBackgroundColor = Color.White,
							TextWrapLength = 50
						};
                        var location = e.Pos;
                        location.Z = 0;
                        var showElement = true;
                        foreach (var explosive in explosives)
                        {
                            if (Vector3.Distance(explosive, location) < Settings.ExplosiveRange + 20)
                            {
                                showElement = false;
                                break;
                            }
                        }
                        var ent = new StoredEntity(e.GetComponent<Render>().Z, positionedComp.GridPos, e.Id, TextInfo);
						if (GameController.Game.IngameState.IngameUi.Map.LargeMap.IsVisible && showElement)
							DrawToLargeMiniMapText(ent, ent.TextureInfo);
					}
					
					
				}
				
                //var expeditionChestComponent = e?.GetComponent<ObjectMagicProperties>();
                //if (expeditionChestComponent == null) continue;
                //var mods = expeditionChestComponent.Mods;
                //if (!mods.Any(x => x.Contains("ExpeditionRelicModifier"))) continue;

			}
			
            foreach (var e in GameController.EntityListWrapper.ValidEntitiesByType[EntityType.IngameIcon])
            //foreach (var e in GameController.EntityListWrapper.NotOnlyValidEntities)
            {
                var renderComponent = e?.GetComponent<Render>();
                if (renderComponent == null) continue;
                var expeditionChestComponent = e?.GetComponent<ObjectMagicProperties>();
                if (expeditionChestComponent == null) continue;
                var mods = expeditionChestComponent.Mods;
                if (!mods.Any(x => x.Contains("ExpeditionRelicModifier"))) continue;
                var positionedComp = e.GetComponent<Positioned>();
                var text = "";
                var background = Color.Green;
				if ((mods.Any(x => x.Contains("ExpeditionRelicModifierExpeditionLogbookQuantityChest")) ) && Settings.ShowLogbooks.Value)
                {
                    text = text + " " +"Log che";
                    background = Settings.LogbookColor;
                }
                if ((mods.Any(x => x.Contains("ExpeditionRelicModifierExpeditionLogbookQuantityMonster"))) && Settings.ShowLogbooks.Value)
                {
                    text = text + " " +"Log mon";
                    background = Settings.LogbookColor;
                }
				
				//if ((mods.Any(x => x.Contains("ExpeditionRelicModifierLegionSplintersElite"))) || 
				//(mods.Any(x => x.Contains("ExpeditionRelicModifierEternalEmpireLegionElite"))))
    //            {
    //                text = text + " " +"Leg Mon";
    //                background = Settings.BasicColor;
    //            }
				//if ((mods.Any(x => x.Contains("ExpeditionRelicModifierLegionSplintersChest")))|| 
				//(mods.Any(x => x.Contains("ExpeditionRelicModifierEternalEmpireLegionChest"))))
    //            {
    //                text = text + " " + "Leg Che";
    //                background = Settings.BasicColor;
    //            }
				//if ((mods.Any(x => x.Contains("ExpeditionRelicModifierExpeditionUniqueElite"))) || 
				//(mods.Any(x => x.Contains("ExpeditionRelicModifierLostMenUniqueElite"))))
    //            {
    //                text = text + " " +"Uniq Mon";
    //                background = Color.Orange;
    //            }
				//if ((mods.Any(x => x.Contains("ExpeditionRelicModifierExpeditionUniqueChest")))|| 
				//(mods.Any(x => x.Contains("ExpeditionRelicModifierLostMenUniqueChest"))))
    //            {
    //                text = text + " " + "Uniq Che";
    //                background = Color.Orange;
    //            }
				//if ((mods.Any(x => x.Contains("ExpeditionRelicModifierEssencesElite"))) || 
				//(mods.Any(x => x.Contains("ExpeditionRelicModifierLostMenEssenceElite"))))
    //            {
    //                text = text + " " + "Ess Mon";
    //                background = Settings.BasicColor;
    //            }
				//if ((mods.Any(x => x.Contains("ExpeditionRelicModifierLostMenEssenceChest")))|| 
				//(mods.Any(x => x.Contains("ExpeditionRelicModifierEssencesChest"))))
    //            {
    //                text = text + " " + "Ess Che";
    //                background = Settings.BasicColor;
    //            }

				//if ((mods.Any(x => x.Contains("ExpeditionRelicModifierVaalGemsElite"))) || 
				//(mods.Any(x => x.Contains("ExpeditionRelicModifierExpeditionGemsElite"))))
    //            {
    //                text = text + " " + "Gem Mon";
    //                background = Settings.BasicColor;
    //            }
				//if ((mods.Any(x => x.Contains("ExpeditionRelicModifierVaalGemsChest")))|| 
				//(mods.Any(x => x.Contains("ExpeditionRelicModifierExpeditionGemsChest"))))
    //            {
    //                text = text + " " + "Gem Che";
    //                background = Settings.BasicColor;
    //            }
				
				if ((mods.Any(x => x.Contains("Metadata/Terrain/Doodads/Leagues/Expedition/ChestMarkers"))))
                {
                    text = " c ";
                    background = Settings.BasicColor;
                }
				if ((mods.Any(x => x.Contains("Metadata/Terrain/Doodads/Leagues/Expedition/monstermarker_set"))))
                {
                    text = " . ";
                    background = Color.Red;
                }
				if ((mods.Any(x => x.Contains("Metadata/Terrain/Doodads/Leagues/Expedition/elitemarker_set"))))
                {
                    text = " * ";
                    background = Color.Yellow;
                }
				//if ((mods.Any(x => x.Contains("ExpeditionRelicModifierExpeditionRareTrinketElite"))))
    //            {
    //                text = text + " " +"J Mon";
    //                background = Settings.BasicColor;
    //            }
				//if ((mods.Any(x => x.Contains("ExpeditionRelicModifierExpeditionRareTrinketChest"))))
    //            {
    //                text = text + " " +"J Che";
    //                background = Settings.BasicColor;
    //            }
				//if ((mods.Any(x => x.Contains("ExpeditionRelicModifierEternalEmpireEnchantElite"))))
    //            {
    //                text = text + " " +"Ench Mon";
    //                background = Settings.BasicColor;
    //            }
				//if ((mods.Any(x => x.Contains("ExpeditionRelicModifierEternalEmpireEnchantChest"))))
    //            {
    //                text = text + " " +"Ench Che";
    //                background = Settings.BasicColor;
    //            }
				if ((mods.Any(x => x.Contains("ExpeditionRelicModifierSirensScarabElite"))))
                {
                    text = text + " " +"Scarab Mon";
                    background = Color.Purple;
                }
				if ((mods.Any(x => x.Contains("ExpeditionRelicModifierSirensScarabChest"))))
                {
                    text = text + " " +"Scarab Che";
                    background = Color.Purple;
                }

				//if ((mods.Any(x => x.Contains("ExpeditionRelicModifierBreachSplintersElite"))) || 
				//(mods.Any(x => x.Contains("ExpeditionRelicModifierSirensBreachElite"))))
    //            {
    //                text = text + " " + "Breach Mon";
    //                background = Color.Purple;
    //            }
				//if ((mods.Any(x => x.Contains("ExpeditionRelicModifierBreachSplintersChest")))|| 
				//(mods.Any(x => x.Contains("ExpeditionRelicModifierSirensBreachChest"))))
    //            {
    //                text = text + " " + "Breach Che";
    //                background = Color.Purple;
    //            }
				
				//if ((mods.Any(x => x.Contains("ExpeditionRelicModifierExpeditionInfluencedItemsElite"))))
    //            {
    //                text = text + " " +"Infl Mon";
    //                background = Color.Purple;
    //            }
				//if ((mods.Any(x => x.Contains("ExpeditionRelicModifierExpeditionInfluencedtemsChest"))))
    //            {
    //                text = text + " " +"Infl Che";
    //                background = Color.Purple;
    //            }
				
				//if ((mods.Any(x => x.Contains("ExpeditionRelicModifierExpeditionMapsElite"))))
    //            {
    //                text = text + " " +"Map Mon";
    //                background = Color.Purple;
    //            }
				//if ((mods.Any(x => x.Contains("ExpeditionRelicModifierExpeditionMapsChest"))))
    //            {
    //                text = text + " " +"Map Che";
    //                background = Color.Purple;
    //            }
				//if ((mods.Any(x => x.Contains("ExpeditionRelicModifierExpeditionFracturedItemsElite"))))
    //            {
    //                text = text + " " +"Frac Mon";
    //                background = Color.Purple;
    //            }
				//if ((mods.Any(x => x.Contains("ExpeditionRelicModifierExpeditionFracturedItemsChest"))))
    //            {
    //                text = text + " " +"Frac Che";
    //                background = Color.Purple;
    //            }
				
				//if ((mods.Any(x => x.Contains("ExpeditionRelicModifierHarbingerCurrencyElite"))))
    //            {
    //                text = text + " " +"Harb Mon";
    //                background = Color.Purple;
    //            }
				//if ((mods.Any(x => x.Contains("ExpeditionRelicModifierHarbingerCurrencyChest"))))
    //            {
    //                text = text + " " +"Harb Che";
    //                background = Color.Purple;
    //            }
				if ((mods.Any(x => x.Contains("ExpeditionRelicModifierPackSize"))) && Settings.ShowPacksize.Value)
                {
                    text = text + " " +"Pack ";
                    background = Settings.PacksizeColor;
                }
				
				//if ((mods.Any(x => x.Contains("ExpeditionRelicModifierRareMonsterChance"))))
    //            {
    //                text = text + " " +"Rare";
    //                background = Settings.DoubleColor;
    //            }

                if ((mods.Any(x => x.Contains("ExpeditionRelicModifierExpeditionCurrencyQuantityChest")) ) && Settings.ShowArtifact.Value)
                {
                    text = text + " " +"Art che";
                    background = Settings.ArtifactColor;
                }
				
				if ((
                    mods.Any(x => x.Contains("ExpeditionRelicModifierExpeditionCurrencyQuantityMonster"))) && Settings.ShowArtifact.Value)
                {
                    text = text + " " +"Art mon";
                    background = Settings.ArtifactColor;
                }

                if ((mods.Any(x => x.Contains("ExpeditionRelicModifierItemQuantityChest"))) && Settings.ShowQuant.Value)
                {
                    text = text + " " +"Quant che";
                    background = Settings.QuantColor;
                }
				if ((
                    mods.Any(x => x.Contains("ExpeditionRelicModifierItemQuantityMonster"))) && Settings.ShowQuant.Value)
                {
                    text = text + " " +"Quant mon";
                    background = Settings.QuantColor;
                }

                if ((mods.Any(x => x.Contains("ExpeditionRelicModifierExpeditionBasicCurrencyChest"))) && Settings.ShowBasicCurrency.Value)
                {
                    text = text + " " +"Curr che";
                    background = Settings.BasicColor;
                }
                if ((
                    mods.Any(x => x.Contains("ExpeditionRelicModifierExpeditionBasicCurrencyElite"))) && Settings.ShowBasicCurrency.Value)
                {
                    text = text + " " +"Curr mon";
                    background = Settings.BasicColor;
                }
                if ((mods.Any(x => x.Contains("ExpeditionRelicModifierStackedDeckChest"))) && Settings.ShowStackedDecks.Value)
                {
                    text = text + " " +"Deck che";
                    background = Settings.StackedColor;
                }
                if ((
                    mods.Any(x => x.Contains("ExpeditionRelicModifierStackedDeckElite"))) && Settings.ShowStackedDecks.Value)
                {
                    text = text + " " +"Deck mon";
                    background = Settings.StackedColor;
                }
                if (mods.Any(x => x.Contains("ExpeditionRelicModifierElitesDuplicated")) && Settings.ShowDouble.Value)
                {
                    text = text + " " +"POG*2";
                    background = Settings.DoubleColor;
                }

                if ((mods.Any(x => x.Contains("ExpeditionRelicModifierImmunePhysicalDamage")) && Settings.PhysImmune.Value) ||
                    (mods.Any(x => x.Contains("ExpeditionRelicModifierImmuneFireDamage")) && Settings.FireImmune.Value) ||
                    (mods.Any(x => x.Contains("ExpeditionRelicModifierImmuneColdDamage")) && Settings.ColdImmune.Value) ||
                    (mods.Any(x => x.Contains("ExpeditionRelicModifierImmuneLightningDamage")) && Settings.LightningImmune.Value) ||
                    (mods.Any(x => x.Contains("ExpeditionRelicModifierImmuneChaosDamage")) && Settings.ChaosImmune.Value) ||
                    (mods.Any(x => x.Contains("ExpeditionRelicModifierCannotBeCrit")) && Settings.CritImmune.Value) || (mods.Any(x => x.Contains("ExpeditionRelicModifierImmuneStatusAilments")) && Settings.FireImmune.Value))
					
                {
                    text = "WARN";
                    background = Settings.ImmuneColor;
                }
                
                if (text == "") continue;

                var TextInfo = new MinimapTextInfo
                {
                    Text = text,
                    FontSize = 10,
                    FontColor = Color.White,
                    FontBackgroundColor = background,
                    TextWrapLength = 50
                };

                var location = e.Pos;
                location.Z = 0;
                var showElement = true;
                foreach (var explosive in explosives)
                {
                    if (Vector3.Distance(explosive, location) < Settings.ExplosiveRange + 20)
                    {
                        showElement = false;
                        break;
                    }
                }
                var ent = new StoredEntity(e.GetComponent<Render>().Z, positionedComp.GridPos, e.Id, TextInfo);
                if (GameController.Game.IngameState.IngameUi.Map.LargeMap.IsVisible && showElement)
                    DrawToLargeMiniMapText(ent, ent.TextureInfo);
            }
        }

        private void DrawToLargeMiniMapText(StoredEntity entity, MinimapTextInfo info)
        {
            var camera = GameController.Game.IngameState.Camera;
            var mapWindow = GameController.Game.IngameState.IngameUi.Map;
            if(GameController.Game.IngameState.UIRoot.Scale == 0)
            {
                DebugWindow.LogError("ExpeditionIcons: Seems like UIRoot.Scale is 0. Icons will not be drawn because of that.");
            }
            var mapRect = mapWindow.GetClientRect();
            var playerPos = GameController.Player.GetComponent<Positioned>().GridPos;
            var posZ = GameController.Player.GetComponent<Render>().Z;
            var screenCenter = new Vector2(mapRect.Width / 2, mapRect.Height / 2).Translate(0, -20) + new Vector2(mapRect.X, mapRect.Y) + new Vector2(mapWindow.LargeMapShiftX, mapWindow.LargeMapShiftY);
            var diag = (float)Math.Sqrt(camera.Width * camera.Width + camera.Height * camera.Height);
            var k = camera.Width < 1024f ? 1120f : 1024f;
            var scale = k / camera.Height * camera.Width * 3f / 4f / mapWindow.LargeMapZoom;
            var iconZ = entity.EntityZ;
            var point = screenCenter + MapIcon.DeltaInWorldToMinimapDelta(entity.GridPos - playerPos, diag, scale, (iconZ - posZ) / (9f / mapWindow.LargeMapZoom));
            var size = Graphics.DrawText(WordWrap(info.Text, info.TextWrapLength), point, info.FontColor, info.FontSize, FontAlign.Center);
            float maxWidth = 0;
            float maxheight = 0;
            //not sure about sizes below, need test
            point.Y += size.Y;
            maxheight += size.Y;
            maxWidth = Math.Max(maxWidth, size.X);
            var background = new RectangleF(point.X - maxWidth / 2 - 3, point.Y - maxheight, maxWidth + 6, maxheight);
            Graphics.DrawBox(background, info.FontBackgroundColor);
        }
		 public void DrawEllipseToWorld(Vector3 vector3Pos, int radius, int points, int lineWidth, Color color)
        {
            var camera = GameController.Game.IngameState.Camera;

            var plottedCirclePoints = new List<Vector3>();
            var slice = 2 * Math.PI / points;
            for (var i = 0; i < points; i++)
            {
                var angle = slice * i;
                var x = (decimal)vector3Pos.X + decimal.Multiply((decimal)radius, (decimal)Math.Cos(angle));
                var y = (decimal)vector3Pos.Y + decimal.Multiply((decimal)radius, (decimal)Math.Sin(angle));
                plottedCirclePoints.Add(new Vector3((float)x, (float)y, vector3Pos.Z));
            }

            for (var i = 0; i < plottedCirclePoints.Count; i++)
            {
                if (i >= plottedCirclePoints.Count - 1)
                {
                    var pointEnd1 = camera.WorldToScreen(plottedCirclePoints.Last());
                    var pointEnd2 = camera.WorldToScreen(plottedCirclePoints[0]);
                    Graphics.DrawLine(pointEnd1, pointEnd2, lineWidth, color);
                    return;
                }

                var point1 = camera.WorldToScreen(plottedCirclePoints[i]);
                var point2 = camera.WorldToScreen(plottedCirclePoints[i + 1]);
                Graphics.DrawLine(point1, point2, lineWidth, color);
            }
        }
    
        public void getBestPositions()
        {
            try
            {
                int placements = Int32.Parse(ingameStateIngameUi.GetChildFromIndices(118, 7, 12, 2, 0, 0, 0).Text); //the number of explosives from UI
                for (int x = 0; x < placements; x++)
                {
                    var minX = remnants.OrderBy(p => p.X).FirstOrDefault();
                    var maxX = remnants.OrderBy(p => p.X).LastOrDefault();
                    var minY = remnants.OrderBy(p => p.Y).FirstOrDefault();
                    var maxY = remnants.OrderBy(p => p.Y).LastOrDefault();
                    Vector2 minCorner = new Vector2(minX.X, minY.Y);
                    Vector2 maxCorner = new Vector2(maxX.X, maxY.Y);
                    List<Vector3> pointsToDelete = new List<Vector3>() { };
                    Vector3 optimalPoint = new Vector3(0, 0, 0);
                    float smallestSum = 1000000;
                    int biggestCount = 0;
                    for (var i = minCorner.X + 10; i < maxCorner.X; i += 10)
                    {
                        for (var j = minCorner.Y + 10; j < maxCorner.Y; j += 10)
                        {
                            Vector3 currentCenter = new Vector3(i, j, 0);
                            List<Vector3> temp = new List<Vector3>() { };
                            float overallSum = 0;
                            int currentCount = 0;
                            foreach (Vector3 remnant in remnants)
                            {
                                float distance = Vector3.Distance(remnant, currentCenter);
                                if (distance < Settings.ExplosiveRange + 10)
                                {
                                    currentCount++;
                                    overallSum += distance;
                                    temp.Add(remnant);
                                }
                            }
                            if (currentCount > biggestCount)
                            {
                                biggestCount = currentCount;
                                smallestSum = overallSum / currentCount;
                                pointsToDelete = temp;
                                optimalPoint = currentCenter;
                            }
                            else if (currentCount == biggestCount && smallestSum > overallSum / currentCount)
                            {
                                smallestSum = overallSum / currentCount;
                                pointsToDelete = temp;
                                optimalPoint = currentCenter;
                            }
                        }
                    }
                    efficientPoints.Add(optimalPoint);
                    if (minCorner == maxCorner)
                    {
                        optimalPoint.X = minCorner.X;
                        optimalPoint.Y = minCorner.Y;
                        efficientPoints.Add(optimalPoint);
                        break;
                    }
                    remnants = remnants.Except(pointsToDelete).ToList();
                }
            }catch{ }
        }

        private void DrawToLargeMiniMapSquare(StoredEntity entity, MinimapTextInfo info)
        {
            var camera = GameController.Game.IngameState.Camera;
            var mapWindow = GameController.Game.IngameState.IngameUi.Map;
            if(GameController.Game.IngameState.UIRoot.Scale == 0)
            {
                DebugWindow.LogError("ExpeditionIcons: Seems like UIRoot.Scale is 0. Icons will not be drawn because of that.");
            }
            var mapRect = mapWindow.GetClientRect();
            var playerPos = GameController.Player.GetComponent<Positioned>().GridPos;
            var posZ = GameController.Player.GetComponent<Render>().Z;
            var screenCenter = new Vector2(mapRect.Width / 2, mapRect.Height / 2).Translate(0, -20) + new Vector2(mapRect.X, mapRect.Y) + new Vector2(mapWindow.LargeMapShiftX, mapWindow.LargeMapShiftY);
            var diag = (float)Math.Sqrt(camera.Width * camera.Width + camera.Height * camera.Height);
            var k = camera.Width < 1024f ? 1120f : 1024f;
            var scale = k / camera.Height * camera.Width * 3f / 4f / mapWindow.LargeMapZoom;
            var iconZ = entity.EntityZ;
            var point = screenCenter + MapIcon.DeltaInWorldToMinimapDelta(entity.GridPos - playerPos, diag, scale, (iconZ - posZ) / (9f / mapWindow.LargeMapZoom));
            var size = Graphics.DrawText(WordWrap(info.Text, info.TextWrapLength), point, info.FontColor, info.FontSize, FontAlign.Center);
            float maxWidth = 0;
            float maxheight = 0;
            //not sure about sizes below, need test
			
            point.Y += size.Y;
            maxheight += size.Y;
            maxWidth = Math.Max(maxWidth, size.X);
            var background = new RectangleF(point.X - maxWidth / 2 - 3, point.Y - maxheight, maxWidth + 6, maxheight);
//Graphics.DrawRectangle(Color.Orange,background);
			Graphics.DrawBox(background, Color.Black);
        }
        public static string WordWrap(string input, int maxCharacters)
        {
            var lines = new List<string>();
            if (!input.Contains(" "))
            {
                var start = 0;
                while (start < input.Length)
                {
                    lines.Add(input.Substring(start, Math.Min(maxCharacters, input.Length - start)));
                    start += maxCharacters;
                }
            }
            else
            {
                var words = input.Split(' ');
                var line = "";
                foreach (var word in words)
                {
                    if ((line + word).Length > maxCharacters)
                    {
                        lines.Add(line.Trim());
                        line = "";
                    }

                    line += string.Format("{0} ", word);
                }

                if (line.Length > 0)
                {
                    lines.Add(line.Trim());
                }
            }

            var conectedLines = "";
            foreach (var line in lines)
            {
                conectedLines += line + "\n\r";
            }

            return conectedLines;
        }
        public class StoredEntity
        {
            public float EntityZ;
            public Vector2 GridPos;
            public long LongID;
            public MinimapTextInfo TextureInfo;

            public StoredEntity(float entityZ, Vector2 gridPos, long longID, MinimapTextInfo textureInfo)
            {
                EntityZ = entityZ;
                GridPos = gridPos;
                LongID = longID;
                TextureInfo = textureInfo;
            }
        }
        public class MinimapTextInfo
        {
            public MinimapTextInfo() { }

            public MinimapTextInfo(string text, int fontSize, Color fontColor, Color fontBackgroundColor, int textWrapLength, int textOffsetY)
            {
                Text = text;
                FontSize = fontSize;
                FontColor = fontColor;
                FontBackgroundColor = fontBackgroundColor;
                TextWrapLength = textWrapLength;
                TextOffsetY = textOffsetY;
            }

            public string Text { get; set; }
            public int FontSize { get; set; }
            public Color FontColor { get; set; }
            public Color FontBackgroundColor { get; set; }
            public int TextWrapLength { get; set; }
            public int TextOffsetY { get; set; }
        }
    }
}
