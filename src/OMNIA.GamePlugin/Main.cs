using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using GTA;
using GameScreen = GTA.UI.Screen;

namespace OMNIA.GTA5.ModManager
{
    public sealed class Main : Script
    {
        private const int VisibleRows = 7;
        private readonly ModCatalog _catalog;
        private readonly ModStateStore _stateStore;
        private readonly string _gameRoot;
        private bool _visible;
        private int _selectedIndex;
        private bool _f10WasDown;

        public Main()
        {
            _gameRoot = AppDomain.CurrentDomain.BaseDirectory;
            var configDirectory = Path.Combine(_gameRoot, "scripts", "OMNIA");
            Directory.CreateDirectory(configDirectory);

            _stateStore = new ModStateStore(Path.Combine(configDirectory, "mod-state.ini"));
            _catalog = new ModCatalog(_stateStore);
            _catalog.Refresh(_gameRoot);

            Tick += OnTick;
            KeyDown += OnKeyDown;

            GameScreen.ShowSubtitle("~b~OMNIA~s~ Mod Manager loaded. Press F10 to open.", 5000);
        }

        private void OnTick(object sender, EventArgs e)
        {
            bool f10Down = Game.IsKeyPressed(Keys.F10);
            if (f10Down && !_f10WasDown)
            {
                _visible = !_visible;
                if (_visible)
                {
                    _catalog.Refresh(_gameRoot);
                    _selectedIndex = Math.Max(0, Math.Min(_selectedIndex, _catalog.Entries.Count - 1));
                }
            }
            _f10WasDown = f10Down;

            if (_visible)
                GameScreen.ShowSubtitle(BuildMenuText(), 1000);
        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (!_visible)
                return;

            if (e.KeyCode == Keys.Escape)
            {
                _visible = false;
                return;
            }

            if (e.KeyCode == Keys.Down)
            {
                _selectedIndex = Math.Min(_selectedIndex + 1, Math.Max(0, _catalog.Entries.Count - 1));
            }
            else if (e.KeyCode == Keys.Up)
            {
                _selectedIndex = Math.Max(0, _selectedIndex - 1);
            }
            else if (e.KeyCode == Keys.Enter && _catalog.Entries.Count > 0)
            {
                ModEntry entry = _catalog.Entries[_selectedIndex];
                bool desired = !entry.DesiredEnabled;
                _stateStore.SetDesiredState(entry.Id, desired);
                entry.DesiredEnabled = desired;

                string state = desired ? "ON" : "OFF";
                GameScreen.ShowSubtitle("~b~OMNIA~s~ " + entry.Name + ": requested " + state +
                    ". External plugins require restart and desktop apply.", 4500);
            }
        }

        private string BuildMenuText()
        {
            var lines = new List<string>
            {
                "~b~OMNIA MOD MANAGER~s~  |  GTA V LEGACY",
                "F10: close   ESC: close   UP/DOWN: select   ENTER: request ON/OFF",
                "Detected files only; actual runtime state is not guaranteed.",
                " "
            };

            if (_catalog.Entries.Count == 0)
            {
                lines.Add("No supported mod files detected.");
            }
            else
            {
                int start = (_selectedIndex / VisibleRows) * VisibleRows;
                int end = Math.Min(start + VisibleRows, _catalog.Entries.Count);
                for (int i = start; i < end; i++)
                {
                    ModEntry entry = _catalog.Entries[i];
                    string cursor = i == _selectedIndex ? "~b~> " : "  ";
                    string desired = entry.DesiredEnabled ? "~g~ON~s~" : "~r~OFF~s~";
                    lines.Add(cursor + entry.Name + "  " + desired + " [restart]");
                }
                lines.Add("Showing " + (start + 1) + "-" + end + " of " + _catalog.Entries.Count);
            }

            lines.Add(" ");
            lines.Add("External changes are queued, not unloaded live. Story Mode only.");
            return string.Join("\n", lines);
        }
    }
}
