using System.Windows.Forms;
using ExileCore.Shared.Attributes;
using ExileCore.Shared.Interfaces;
using ExileCore.Shared.Nodes;
using SharpDX;

namespace ExpeditionIcons
{
    public class Settings : ISettings
    {
        public ToggleNode Enable { get; set; } = new ToggleNode(false);
        public HotkeyNode optimalMap { get; set; } = new HotkeyNode(Keys.F6);
        public HotkeyNode optimalLogBook { get; set; } = new HotkeyNode(Keys.F7);
        [Menu("Optimal placement color")]
        public ColorNode OptimalColor { get; set; } = new ColorNode(Color.Yellow);
        [Menu("Highlight runic monsters")]
        public ToggleNode HiglightRunic { get; set; } = new ToggleNode(false);
        [Menu("Highlight selected chests in logbook")]
        public ToggleNode HiglightArtifact { get; set; } = new ToggleNode(false);
        [Menu("Show and highlight chests in maps")]
        public ToggleNode ShowChests { get; set; } = new ToggleNode(false);


        [Menu("Explosive settings", 102)]
        public EmptyNode SettingsExplosive { get; set; }
        [Menu("Explosive range (Don't change, use the button)", parentIndex = 102)]
        public RangeNode<float> ExplosiveRange { get; set; } = new RangeNode<float>(310, 310, 600);
        [Menu("Explosive distance (Don't change, use the button)", parentIndex = 102)]
        public RangeNode<float> ExplosiveDistance { get; set; } = new RangeNode<float>(970, 970, 2000);


        [Menu("Good mods", 100)]
        public EmptyNode SettingsEmptyGood { get; set; }
        [Menu("Show double runic monsters", parentIndex = 100)]
        public ToggleNode ShowDouble { get; set; } = new ToggleNode(true);
        [Menu("Color", parentIndex = 100)]
        public ColorNode DoubleColor { get; set; } = new ColorNode(Color.Gold);
        [Menu("Show logbook quantity", parentIndex = 100)]
        public ToggleNode ShowLogbooks { get; set; } = new ToggleNode(true);
        [Menu("Color", parentIndex = 100)]
        public ColorNode LogbookColor { get; set; } = new ColorNode(Color.Green);
        [Menu("Show basic currency", parentIndex = 100)]
        public ToggleNode ShowBasicCurrency { get; set; } = new ToggleNode(false);
        [Menu("Color", parentIndex = 100)]
        public ColorNode BasicColor { get; set; } = new ColorNode(Color.Green);
        [Menu("Show stacked decks", parentIndex = 100)]
        public ToggleNode ShowStackedDecks { get; set; } = new ToggleNode(false);
        [Menu("Color", parentIndex = 100)]
        public ColorNode StackedColor { get; set; } = new ColorNode(Color.Green);
        [Menu("Show item quantity", parentIndex = 100)]
        public ToggleNode ShowQuant { get; set; } = new ToggleNode(true);
        [Menu("Color", parentIndex = 100)]
        public ColorNode QuantColor { get; set; } = new ColorNode(Color.Green);
        [Menu("Show artifact quantity", parentIndex = 100)]
        public ToggleNode ShowArtifact { get; set; } = new ToggleNode(false);
        [Menu("Color", parentIndex = 100)]
        public ColorNode ArtifactColor { get; set; } = new ColorNode(Color.Green);
        [Menu("Show packsize", parentIndex = 100)]
        public ToggleNode ShowPacksize { get; set; } = new ToggleNode(true);
        [Menu("Color", parentIndex = 100)]
        public ColorNode PacksizeColor { get; set; } = new ColorNode(Color.Green);
        [Menu("Show jewellery", parentIndex = 100)]
        public ToggleNode ShowJewellery { get; set; } = new ToggleNode(false);
        [Menu("Color", parentIndex = 100)]
        public ColorNode JewelleryColor { get; set; } = new ColorNode(Color.Green);
        [Menu("Show influence", parentIndex = 100)]
        public ToggleNode ShowInfluence { get; set; } = new ToggleNode(false);
        [Menu("Color", parentIndex = 100)]
        public ColorNode InfluenceColor { get; set; } = new ColorNode(Color.Green);
        [Menu("Show fractured", parentIndex = 100)]
        public ToggleNode ShowFractured { get; set; } = new ToggleNode(false);
        [Menu("Color", parentIndex = 100)]
        public ColorNode FracturedColor { get; set; } = new ColorNode(Color.Gray);
        [Menu("Show harbinger", parentIndex = 100)]
        public ToggleNode ShowHarbinger { get; set; } = new ToggleNode(false);
        [Menu("Color", parentIndex = 100)]
        public ColorNode HarbingerColor { get; set; } = new ColorNode(Color.Green);
        [Menu("Show heist", parentIndex = 100)]
        public ToggleNode ShowHeist { get; set; } = new ToggleNode(false);
        [Menu("Color", parentIndex = 100)]
        public ColorNode HeistColor { get; set; } = new ColorNode(Color.Green);

        [Menu("Bad mods", 101)]
        public EmptyNode SettingsEmptyBad { get; set; }
        [Menu("Physical immune", parentIndex = 101)]
        public ToggleNode PhysImmune { get; set; } = new ToggleNode(false);
        [Menu("Fire immune", parentIndex = 101)]
        public ToggleNode FireImmune { get; set; } = new ToggleNode(false);
        [Menu("Cold immune", parentIndex = 101)]
        public ToggleNode ColdImmune { get; set; } = new ToggleNode(false);
        [Menu("Lightning immune", parentIndex = 101)]
        public ToggleNode LightningImmune { get; set; } = new ToggleNode(false);
        [Menu("Chaos immune", parentIndex = 101)]
        public ToggleNode ChaosImmune { get; set; } = new ToggleNode(false);
        [Menu("Crit immune", parentIndex = 101)]
        public ToggleNode CritImmune { get; set; } = new ToggleNode(false);
        [Menu("Ailment immune", parentIndex = 101)]
        public ToggleNode AilmentImmune { get; set; } = new ToggleNode(false);
        [Menu("Culling", parentIndex = 101)]
        public ToggleNode Culling { get; set; } = new ToggleNode(false);
        [Menu("Regen every 4 seconds", parentIndex = 101)]
        public ToggleNode Regen { get; set; } = new ToggleNode(false);
        [Menu("Corrupted items", parentIndex = 101)]
        public ToggleNode CorruptedItems { get; set; } = new ToggleNode(false);
        [Menu("Max Block", parentIndex = 101)]
        public ToggleNode BlockChance { get; set; } = new ToggleNode(false);  
        [Menu("30% elemental resistances", parentIndex = 101)]
        public ToggleNode MaxResistances { get; set; } = new ToggleNode(false);
        [Menu("No leech", parentIndex = 101)]
        public ToggleNode NoLeech { get; set; } = new ToggleNode(false);
        [Menu("No curse", parentIndex = 101)]
        public ToggleNode NoCurse { get; set; } = new ToggleNode(false);

        [Menu("Color for danger", parentIndex = 101)]
        public ColorNode ImmuneColor { get; set; } = new ColorNode(Color.Red);
    }
}
