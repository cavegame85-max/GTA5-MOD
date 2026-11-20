using System;

namespace OMNIA.GTA5.ModManager
{
    public sealed class ModEntry
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Kind { get; set; }
        public string Path { get; set; }
        public bool DesiredEnabled { get; set; }
        public bool CanToggleInGame { get; set; }
        public bool RequiresRestart { get; set; }
    }
}
