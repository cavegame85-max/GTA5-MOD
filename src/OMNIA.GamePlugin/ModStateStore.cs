using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace OMNIA.GTA5.ModManager
{
    public sealed class ModStateStore
    {
        private readonly string _path;
        private readonly Dictionary<string, bool> _states =
            new Dictionary<string, bool>(StringComparer.OrdinalIgnoreCase);

        public ModStateStore(string path)
        {
            _path = path;
            Load();
        }

        public bool GetDesiredState(string id, bool defaultValue = true)
        {
            bool value;
            return _states.TryGetValue(id, out value) ? value : defaultValue;
        }

        public void SetDesiredState(string id, bool enabled)
        {
            _states[id] = enabled;
            Save();
        }

        private void Load()
        {
            if (!File.Exists(_path))
                return;

            foreach (string raw in File.ReadAllLines(_path))
            {
                string line = raw.Trim();
                if (line.Length == 0 || line.StartsWith("#"))
                    continue;

                int split = line.IndexOf('=');
                if (split <= 0)
                    continue;

                string key = line.Substring(0, split).Trim();
                string value = line.Substring(split + 1).Trim();
                bool enabled;
                if (bool.TryParse(value, out enabled))
                    _states[key] = enabled;
            }
        }

        private void Save()
        {
            Directory.CreateDirectory(Path.GetDirectoryName(_path));
            var lines = new List<string>
            {
                "# OMNIA desired states. External plugin changes require the desktop installer and a game restart."
            };
            lines.AddRange(_states.OrderBy(x => x.Key)
                .Select(x => x.Key + "=" + x.Value.ToString().ToLowerInvariant()));
            File.WriteAllLines(_path, lines);
        }
    }
}
