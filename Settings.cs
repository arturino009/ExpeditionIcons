using ExileCore.Shared.Attributes;
using ExileCore.Shared.Interfaces;
using ExileCore.Shared.Nodes;

namespace ExpeditionIcons
{
    public class Settings : ISettings
    {
        public ToggleNode Enable { get; set; } = new ToggleNode(true);
        [Menu("Warn for physical immune")]
        public ToggleNode PhysImmune { get; set; } = new ToggleNode(true);
        [Menu("Warn for fire immune")]
        public ToggleNode FireImmune { get; set; } = new ToggleNode(true);
        [Menu("Warn for cold immune")]
        public ToggleNode ColdImmune { get; set; } = new ToggleNode(true);
        [Menu("Warn for lightning immune")]
        public ToggleNode LightningImmune { get; set; } = new ToggleNode(true);
        [Menu("Warn for chaos immune")]
        public ToggleNode ChaosImmune { get; set; } = new ToggleNode(true);
        [Menu("Warn for crit immune")]
        public ToggleNode CritImmune { get; set; } = new ToggleNode(true);
    }
}