using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace OMNIA.GTA5.ModManager
{
    public sealed class ModCatalog
    {
        private static readonly HashSet<string> AllowedCategories =
            new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "graphics", "audio", "vehicle-cosmetics", "maps", "ui", "quality-of-life"
            };

        private readonly ModStateStore _stateStore;

        public List<ModEntry> Entries { get; private set; } = new List<ModEntry>();
        public int UnclassifiedPluginCount { get; private set; }
        public bool ArchiveModsFolderDetected { get; private set; }

        public ModCatalog(ModStateStore stateStore)
        {
            _stateStore = stateStore;
        }

        public void Refresh(string gameRoot)
        {
            string scripts = Path.Combine(gameRoot, "scripts");
            UnclassifiedPluginCount = CountExternalPlugins(gameRoot, scripts);
            ArchiveModsFolderDetected = Directory.Exists(Path.Combine(gameRoot, "mods"));

            var found = new List<ModEntry>();
            string registry = Path.Combine(scripts, "OMNIA", "Mods");
            if (Directory.Exists(registry))
            {
                foreach (string folder in Directory.GetDirectories(registry))
                {
                    ModEntry entry = ReadSafeManifest(folder);
                    if (entry != null)
                    {
                        entry.DesiredEnabled = _stateStore.GetDesiredState(entry.Id, true);
                        found.Add(entry);
                    }
                }
            }

            Entries = found
                .GroupBy(x => x.Id, StringComparer.OrdinalIgnoreCase)
                .Select(x => x.First())
                .OrderBy(x => x.Name, StringComparer.OrdinalIgnoreCase)
                .ToList();
        }

        private static int CountExternalPlugins(string gameRoot, string scripts)
        {
            int count = 0;
            try
            {
                count += Directory.EnumerateFiles(gameRoot, "*.asi", SearchOption.TopDirectoryOnly)
                    .Count(path =>
                    {
                        string name = Path.GetFileName(path);
                        return !name.Equals("ScriptHookV.asi", StringComparison.OrdinalIgnoreCase)
                            && !name.Equals("ScriptHookVDotNet.asi", StringComparison.OrdinalIgnoreCase)
                            && !name.Equals("OpenIV.asi", StringComparison.OrdinalIgnoreCase);
                    });
            }
            catch { }

            if (!Directory.Exists(scripts))
                return count;

            var excluded = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "ScriptHookVDotNet2.dll", "ScriptHookVDotNet3.dll",
                "NativeUI.dll", "LemonUI.SHVDN3.dll", "LemonUI.SHVDN2.dll"
            };

            try
            {
                count += Directory.EnumerateFiles(scripts, "*.dll", SearchOption.AllDirectories)
                    .Count(path => !excluded.Contains(Path.GetFileName(path)));
                count += Directory.EnumerateFiles(scripts, "*.cs", SearchOption.AllDirectories).Count();
                count += Directory.EnumerateFiles(scripts, "*.vb", SearchOption.AllDirectories).Count();
            }
            catch { }

            return count;
        }

        private static ModEntry ReadSafeManifest(string folder)
        {
            string manifestPath = Path.Combine(folder, "mod.ini");
            if (!File.Exists(manifestPath))
                return null;

            var values = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            try
            {
                foreach (string raw in File.ReadAllLines(manifestPath))
                {
                    string line = raw.Trim();
                    if (line.Length == 0 || line.StartsWith("#") || line.StartsWith(";"))
                        continue;

                    int split = line.IndexOf('=');
                    if (split <= 0)
                        continue;

                    values[line.Substring(0, split).Trim()] = line.Substring(split + 1).Trim();
                }
            }
            catch
            {
                return null;
            }

            string id, name, category;
            if (!values.TryGetValue("id", out id) ||
                !values.TryGetValue("name", out name) ||
                !values.TryGetValue("category", out category))
                return null;

            if (!AllowedCategories.Contains(category))
                return null;

            return new ModEntry
            {
                Id = id,
                Name = name,
                Category = category,
                Kind = "OMNIA registered mod",
                Path = folder,
                CanToggleInGame = false,
                RequiresRestart = true
            };
        }
    }
}
