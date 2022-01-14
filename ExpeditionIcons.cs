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
        private bool largeMap;
        private float scale;
        private Vector2 screentCenterCache;
        private RectangleF MapRect => _mapRect?.Value ?? (_mapRect = new TimeCache<RectangleF>(() => mapWindow.GetClientRect(), 100)).Value;
        private Map mapWindow => GameController.Game.IngameState.IngameUi.Map;
        private Camera camera => GameController.Game.IngameState.Camera;
        private Vector2 screenCenter =>
            new Vector2(MapRect.Width / 2, MapRect.Height / 2 - 20) + new Vector2(MapRect.X, MapRect.Y) +
            new Vector2(mapWindow.LargeMapShiftX, mapWindow.LargeMapShiftY);
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
                    largeMap = false;
                }
                else if (ingameStateIngameUi.Map.LargeMap.IsVisibleLocal)
                {
                    screentCenterCache = screenCenter;
                    largeMap = true;
                }

                k = camera.Width < 1024f ? 1120f : 1024f;
                scale = k / camera.Height * camera.Width * 3f / 4f / mapWindow.LargeMapZoom;
            }
            catch (Exception e)
            {
                DebugWindow.LogError($"ExpeditionIcons.TickLogic: {e.Message}");
            }
        }

        public override void Render()
        {
			
			//GetComponent
			foreach (var e in GameController.EntityListWrapper.OnlyValidEntities)
            //foreach (var e in GameController.EntityListWrapper.NotOnlyValidEntities)
            {
                // var renderComponent = e?.GetComponent<Render>();
                // if (renderComponent == null) continue;
				if(e.Path.Contains("ExpeditionMarker"))
				{
					var positionedComp = e.GetComponent<Positioned>();
					var modelPath = e.GetComponent<RenderItem>();
					var text = modelPath.Substring(0, modelPath.IndexOf("."));
					var background = Color.Orange;
					var TextInfo = new MinimapTextInfo
					{
						Text = text,
						FontSize = 60,
						FontColor = Color.Orange,
						//FontBackgroundColor = background,
						TextWrapLength = 50
					};
					var ent = new StoredEntity(e.GetComponent<Render>().Z, positionedComp.GridPos, e.Id, TextInfo);
					if (GameController.Game.IngameState.IngameUi.Map.LargeMap.IsVisible)
						DrawToLargeMiniMapSquare(ent, ent.TextureInfo);
					
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
				
				
				
				if ((mods.Any(x => x.Contains("ExpeditionRelicModifierLegionSplintersElite"))) || 
				(mods.Any(x => x.Contains("ExpeditionRelicModifierEternalEmpireLegionElite"))))
                {
                    text = text + " " +"Leg Mon";
                    background = Settings.BasicColor;
                }
				if ((mods.Any(x => x.Contains("ExpeditionRelicModifierLegionSplintersChest")))|| 
				(mods.Any(x => x.Contains("ExpeditionRelicModifierEternalEmpireLegionChest"))))
                {
                    text = text + " " + "Leg Che";
                    background = Settings.BasicColor;
                }
				if ((mods.Any(x => x.Contains("ExpeditionRelicModifierEssencesElite"))) || 
				(mods.Any(x => x.Contains("ExpeditionRelicModifierLostMenEssenceElite"))))
                {
                    text = text + " " + "Ess Mon";
                    background = Settings.BasicColor;
                }
				if ((mods.Any(x => x.Contains("ExpeditionRelicModifierLostMenEssenceChest")))|| 
				(mods.Any(x => x.Contains("ExpeditionRelicModifierEssencesChest"))))
                {
                    text = text + " " + "Ess Che";
                    background = Settings.BasicColor;
                }
				
				
				if ((mods.Any(x => x.Contains("ExpeditionRelicModifierVaalGemsElite"))) || 
				(mods.Any(x => x.Contains("ExpeditionRelicModifierExpeditionGemsElite"))))
                {
                    text = text + " " + "Gem Mon";
                    background = Settings.BasicColor;
                }
				if ((mods.Any(x => x.Contains("ExpeditionRelicModifierVaalGemsChest")))|| 
				(mods.Any(x => x.Contains("ExpeditionRelicModifierExpeditionGemsChest"))))
                {
                    text = text + " " + "Gem Che";
                    background = Settings.BasicColor;
                }
				
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
				if ((mods.Any(x => x.Contains("ExpeditionRelicModifierExpeditionRareTrinketElite"))))
                {
                    text = text + " " +"J Mon";
                    background = Settings.BasicColor;
                }
				if ((mods.Any(x => x.Contains("ExpeditionRelicModifierExpeditionRareTrinketChest"))))
                {
                    text = text + " " +"J Che";
                    background = Settings.BasicColor;
                }
				if ((mods.Any(x => x.Contains("ExpeditionRelicModifierEternalEmpireEnchantElite"))))
                {
                    text = text + " " +"Ench Mon";
                    background = Settings.BasicColor;
                }
				if ((mods.Any(x => x.Contains("ExpeditionRelicModifierEternalEmpireEnchantChest"))))
                {
                    text = text + " " +"Ench Che";
                    background = Settings.BasicColor;
                }
				if ((mods.Any(x => x.Contains("ExpeditionRelicModifierExpeditionInfluencedItemsElite"))))
                {
                    text = text + " " +"Infl Mon";
                    background = Settings.BasicColor;
                }
				if ((mods.Any(x => x.Contains("ExpeditionRelicModifierExpeditionInfluencedtemsChest"))))
                {
                    text = text + " " +"Infl Che";
                    background = Settings.BasicColor;
                }
				
				if ((mods.Any(x => x.Contains("ExpeditionRelicModifierExpeditionMapsElite"))))
                {
                    text = text + " " +"Map Mon";
                    background = Settings.BasicColor;
                }
				if ((mods.Any(x => x.Contains("ExpeditionRelicModifierExpeditionMapsChest"))))
                {
                    text = text + " " +"Map Che";
                    background = Settings.BasicColor;
                }
				if ((mods.Any(x => x.Contains("ExpeditionRelicModifierExpeditionFracturedItemsElite"))))
                {
                    text = text + " " +"Frac Mon";
                    background = Settings.BasicColor;
                }
				if ((mods.Any(x => x.Contains("ExpeditionRelicModifierExpeditionFracturedItemsChest"))))
                {
                    text = text + " " +"Frac Che";
                    background = Settings.BasicColor;
                }
				
				if ((mods.Any(x => x.Contains("ExpeditionRelicModifierHarbingerCurrencyElite"))))
                {
                    text = text + " " +"Harb Mon";
                    background = Settings.BasicColor;
                }
				if ((mods.Any(x => x.Contains("ExpeditionRelicModifierHarbingerCurrencyChest"))))
                {
                    text = text + " " +"Harb Che";
                    background = Settings.BasicColor;
                }
				if ((mods.Any(x => x.Contains("ExpeditionRelicModifierPackSize"))))
                {
                    text = text + " " +"Pack ";
                    background = Settings.DoubleColor;
                }
				
				if ((mods.Any(x => x.Contains("ExpeditionRelicModifierRareMonsterChance"))))
                {
                    text = text + " " +"Rare";
                    background = Settings.DoubleColor;
                }

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
                var ent = new StoredEntity(e.GetComponent<Render>().Z, positionedComp.GridPos, e.Id, TextInfo);
                if (GameController.Game.IngameState.IngameUi.Map.LargeMap.IsVisible)
                    DrawToLargeMiniMapText(ent, ent.TextureInfo);

        //        if (storedAreaEntities.Any(x => x.GridPos == positionedComp.GridPos))
        //        {
        //            var findIndex = storedAreaEntities.FindIndex(x => x.GridPos == positionedComp.GridPos);
        //            storedAreaEntities[findIndex] = new StoredEntity(e.GetComponent<Render>().Z, positionedComp.GridPos, e.Id, TextInfo);
        //        }
        //        else
        //        {
        //            storedAreaEntities.Add(new StoredEntity(e.GetComponent<Render>().Z, positionedComp.GridPos, e.Id, TextInfo));
        //        }

        //    }
        //    foreach (var storedAreaEntity in storedAreaEntities)
        //    {
        //        if (GameController.Game.IngameState.IngameUi.Map.LargeMap.IsVisible)
        //            DrawToLargeMiniMapText(storedAreaEntity, storedAreaEntity.TextureInfo);
        //    }
            }
			// foreach (var e in GameController.EntityListWrapper.NotOnlyValidEntities)
			// {
                // var renderComponent = e?.GetComponent<Render>();
                // if (renderComponent == null) continue;

                // //if (!e?.Path?.Contains("Metadata/MiscellaneousObjects/Expedition/")== true) continue;
                // var positionedComp = e.GetComponent<Positioned>();
                // var text = "";
                // var background = Color.Green;			
				
				
				// //if ((e?.Path?.Contains("Metadata/Terrain/Doodads/Leagues/Expedition/monstermarker_set")== true))
				// if ((e?.Path?.Contains("Expedition")== true))
                // {
                    // text = " c ";
                    // background = Settings.BasicColor;
                // }
				// //if ((e?.Path?.Contains("Metadata/Terrain/Doodads/Leagues/Expedition/elitemarker_set")== true))
				// if ((e?.Path?.Contains("ExpeditionMarker")== true))
                // {
                    // text = " . ";
                    // background = Color.Red;
                // }
				// if ((e?.Path?.Contains("Metadata/Terrain/Doodads/Leagues/Expedition/ChestMarkers") == true))
                // {
                    // text = " * ";
                    // background = Color.Yellow;
                // }
				
                
                // if (text == "") continue;

                // var TextInfo = new MinimapTextInfo
                // {
                    // Text = text,
                    // FontSize = 10,
                    // FontColor = Color.White,
                    // FontBackgroundColor = background,
                    // TextWrapLength = 50
                // };
                // var ent = new StoredEntity(e.GetComponent<Render>().Z, positionedComp.GridPos, e.Id, TextInfo);
                // if (GameController.Game.IngameState.IngameUi.Map.LargeMap.IsVisible)
                    // DrawToLargeMiniMapText(ent, ent.TextureInfo);

        // //        if (storedAreaEntities.Any(x => x.GridPos == positionedComp.GridPos))
        // //        {
        // //            var findIndex = storedAreaEntities.FindIndex(x => x.GridPos == positionedComp.GridPos);
        // //            storedAreaEntities[findIndex] = new StoredEntity(e.GetComponent<Render>().Z, positionedComp.GridPos, e.Id, TextInfo);
        // //        }
        // //        else
        // //        {
        // //            storedAreaEntities.Add(new StoredEntity(e.GetComponent<Render>().Z, positionedComp.GridPos, e.Id, TextInfo));
        // //        }

        // //    }
        // //    foreach (var storedAreaEntity in storedAreaEntities)
        // //    {
        // //        if (GameController.Game.IngameState.IngameUi.Map.LargeMap.IsVisible)
        // //            DrawToLargeMiniMapText(storedAreaEntity, storedAreaEntity.TextureInfo);
        // //    }
            // }
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

    
    //ExpeditionRelicModifierImmuneChaosDamage
    //ExpeditionRelicModifierImmuneColdDamage
    //ExpeditionRelicModifierImmuneFireDamage
    //ExpeditionRelicModifierImmuneLightningDamage
    //ExpeditionRelicModifierImmunePhysicalDamage
    //ExpeditionRelicModifierImmuneStatusAilments
    //ExpeditionRelicModifierImmuneToCurses
    //ExpeditionRelicModifierCannotBeCrit
    

    //ExpeditionRelicModifierElitesDuplicated
    //ExpeditionRelicModifierStackedDeckChest
    //ExpeditionRelicModifierStackedDeckElite
    //ExpeditionRelicModifierExpeditionBasicCurrencyChest
    //ExpeditionRelicModifierExpeditionBasicCurrencyElite
    //ExpeditionRelicModifierExpeditionLogbookQuantityChest
    //ExpeditionRelicModifierExpeditionLogbookQuantityMonster
    //ExpeditionRelicModifierExpeditionCurrencyQuantityChest
    //ExpeditionRelicModifierExpeditionCurrencyQuantityMonster
    //ExpeditionRelicModifierItemQuantityChest
    //ExpeditionRelicModifierItemQuantityMonster