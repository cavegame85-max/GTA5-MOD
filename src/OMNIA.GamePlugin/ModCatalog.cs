using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace OMNIA.GTA5.ModManager
{
    public sealed class ModCatalog
    {
        private static readonly HashSet<string> RootAsiLoaders =
            new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "ScriptHookV.asi",
                "ScriptHookVDotNet.asi",
                "OpenIV.asi"
            };

        private static readonly HashSet<string> ScriptDependencies =
            new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "ScriptHookVDotNet2.dll",
                "ScriptHookVDotNet3.dll",
                "NativeUI.dll",
                "LemonUI.SHVDN3.dll",
                "LemonUI.SHVDN2.dll"
            };

        private readonly ModStateStore _stateStore;

        public List<ModEntry> Entries { get; private set; } = new List<ModEntry>();

        public ModCatalog(ModStateStore stateStore)
        {
            _stateStore = stateStore;
        }

        public void Refresh(string gameRoot)
        {
            var found = new List<ModEntry>();
            ScanFiles(gameRoot, "*.asi", "ASI plugin", RootAsiLoaders, found, false);

            string scripts = Path.Combine(gameRoot, "scripts");
            if (Directory.Exists(scripts))
            {
                ScanFiles(scripts, "*.dll", "ScriptHookVDotNet assembly", ScriptDependencies, found, true);
                ScanFiles(scripts, "*.cs", "ScriptHookVDotNet source", new HashSet<string>(), found, true);
                ScanFiles(scripts, "*.vb", "ScriptHookVDotNet source", new HashSet<string>(), found, true);
            }

            string modsFolder = Path.Combine(gameRoot, "mods");
            if (Directory.Exists(modsFolder))
            {
                found.Add(CreateEntry("openiv-mods-folder", "OpenIV mods folder", "Archive mod container",
                    modsFolder, false));
            }

            Entries = found
                .GroupBy(x => x.Id, StringComparer.OrdinalIgnoreCase)
                .Select(x => x.First())
                .OrderBy(x => x.Name, StringComparer.OrdinalIgnoreCase)
                .ToList();
        }

        private void ScanFiles(string folder, string pattern, string kind, HashSet<string> exclusions,
            List<ModEntry> output, bool recursive)
        {
            if (!Directory.Exists(folder))
                return;

            SearchOption option = recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;
            IEnumerable<string> files;
            try
            {
                files = Directory.EnumerateFiles(folder, pattern, option).ToArray();
            }
            catch
            {
                return;
            }

            foreach (string file in files)
            {
                string name = System.IO.Path.GetFileName(file);
                if (exclusions.Contains(name))
                    continue;

                output.Add(CreateEntry(kind + ":" + file, name, kind, file, true));
            }
        }

        private ModEntry CreateEntry(string id, string name, string kind, string path, bool requiresRestart)
        {
            return new ModEntry
            {
                Id = id,
                Name = name,
                Kind = kind,
                Path = path,
                DesiredEnabled = _stateStore.GetDesiredState(id, true),
                CanToggleInGame = false,
                RequiresRestart = requiresRestart
            };
        }
    }
}
