using ExileCore.Shared.Attributes;
using ExileCore.Shared.Interfaces;
using ExileCore.Shared.Nodes;
using SharpDX;

namespace ExpeditionIcons
{
    public class Settings : ISettings
    {
        public ToggleNode Enable { get; set; } = new ToggleNode(false);
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
        public ToggleNode ShowBasicCurrency { get; set; } = new ToggleNode(true);
        [Menu("Color", parentIndex = 100)]
        public ColorNode BasicColor { get; set; } = new ColorNode(Color.Green);

        [Menu("Show stacked decks", parentIndex = 100)]
        public ToggleNode ShowStackedDecks { get; set; } = new ToggleNode(true);
        [Menu("Color", parentIndex = 100)]
        public ColorNode StackedColor { get; set; } = new ColorNode(Color.Green);

        [Menu("Show item quantity", parentIndex = 100)]
        public ToggleNode ShowQuant { get; set; } = new ToggleNode(true);
        [Menu("Color", parentIndex = 100)]
        public ColorNode QuantColor { get; set; } = new ColorNode(Color.Green);

        [Menu("Show artifact quantity", parentIndex = 100)]
        public ToggleNode ShowArtifact { get; set; } = new ToggleNode(true);
        [Menu("Color", parentIndex = 100)]
        public ColorNode ArtifactColor { get; set; } = new ColorNode(Color.Green);


        [Menu("Bad mods", 101)]
        public EmptyNode SettingsEmptyBad { get; set; }
        [Menu("Warn for physical immune", parentIndex = 101)]
        public ToggleNode PhysImmune { get; set; } = new ToggleNode(false);
        [Menu("Warn for fire immune", parentIndex = 101)]
        public ToggleNode FireImmune { get; set; } = new ToggleNode(false);
        [Menu("Warn for cold immune", parentIndex = 101)]
        public ToggleNode ColdImmune { get; set; } = new ToggleNode(false);
        [Menu("Warn for lightning immune", parentIndex = 101)]
        public ToggleNode LightningImmune { get; set; } = new ToggleNode(false);
        [Menu("Warn for chaos immune", parentIndex = 101)]
        public ToggleNode ChaosImmune { get; set; } = new ToggleNode(false);
        [Menu("Warn for crit immune", parentIndex = 101)]
        public ToggleNode CritImmune { get; set; } = new ToggleNode(false);
        [Menu("Color for immunities", parentIndex = 101)]
        public ColorNode ImmuneColor { get; set; } = new ColorNode(Color.Red);


        [Menu("Explosive settings", 102)]
        public EmptyNode SettingsExplosive { get; set; }
        [Menu("Show explosive range", parentIndex = 102)]
        public ToggleNode ShowExplosives { get; set; } = new ToggleNode(true);
        [Menu("Color for explosive range", parentIndex = 102)]
        public ColorNode ExplosiveColor { get; set; } = new ColorNode(Color.Gray);
        [Menu("Explosive range (configure manually so it matches ingame)", 102)] 
        public RangeNode<int> ExplosiveRange { get; set; } = new RangeNode<int>(10, 310, 600);
    }
}