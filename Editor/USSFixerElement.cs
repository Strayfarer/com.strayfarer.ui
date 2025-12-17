#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Strayfarer.UI.Editor {
    sealed class USSFixerElement : VisualElement {
        const string NO_ACTION_NEEDED = "✔️";
        const string CAN_FIX = "⚠️";
        const string HAS_ERROR = "⛔";

        static readonly Dictionary<USSFixerService.EStatus, string> _icons = new() {
            [USSFixerService.EStatus.OK] = NO_ACTION_NEEDED,
            [USSFixerService.EStatus.ShouldFix] = CAN_FIX,
            [USSFixerService.EStatus.Error] = HAS_ERROR,
        };

        readonly Toggle _assetsToggle = new() { text = "Assets", value = true };
        readonly Toggle _packagesToggle = new() { text = "Packages", value = true };
        readonly Button _scanButton = new() { text = "Scan" };
        readonly Button _fixButton = new() { text = "Fix Now" };
        readonly Button _clearButton = new() { text = "Clear" };

        readonly ListView _list;
        List<USSFixerService.Candidate> _model = new();

        public USSFixerElement() {
            var toolbar = new Toolbar() {
                style = {
                    marginBottom = 4,
                }
            };

            toolbar.Add(_assetsToggle);
            toolbar.Add(_packagesToggle);

            toolbar.Add(_scanButton);
            toolbar.Add(_fixButton);
            toolbar.Add(_clearButton);

            Add(toolbar);

            _list = new ListView {
                virtualizationMethod = CollectionVirtualizationMethod.DynamicHeight,
                showAlternatingRowBackgrounds = AlternatingRowBackground.All,
                selectionType = SelectionType.None,
                makeItem = MakeRow,
                bindItem = BindRow
            };
            Add(_list);

            _scanButton.clicked += Scan;
            _fixButton.clicked += Fix;
            _clearButton.clicked += ClearModel;

            Refresh();
        }

        void UpdateButtons() {
            _fixButton.SetEnabled(_model.Any(c => c.status == USSFixerService.EStatus.ShouldFix));
        }

        void Refresh() {
            var list = _model
                .GroupBy(m => m.filePath)
                .ToList();
            _list.itemsSource = list;
            _list.Rebuild();

            UpdateButtons();
        }

        void Scan() {
            _model = USSFixerService.Scan(new USSFixerService.Options {
                includeAssets = _assetsToggle.value,
                includePackages = _packagesToggle.value
            });
            Refresh();
        }

        void Fix() {
            var changedFiles = USSFixerService.FixNow(_model);

            Debug.Log(string.Join(Environment.NewLine, changedFiles.Prepend($"Fixed {changedFiles.Count} file(s):")));

            AssetDatabase.Refresh();

            Scan();
        }

        void ClearModel() {
            _model.Clear();
            Refresh();
        }

        static VisualElement MakeRow() {
            var root = new Foldout {
                style = {
                    paddingBottom = EditorGUIUtility.singleLineHeight * 0.5f,
                }
            };

            return root;
        }

        void BindRow(VisualElement element, int index) {
            var group = _list.itemsSource[index] as IGrouping<string, USSFixerService.Candidate> ?? throw new InvalidCastException();
            if (element is Foldout root) {
                var candidates = group.ToList();
                var groupStatus = candidates.Max(c => c.status);
                root.text = $"{_icons[groupStatus]} {candidates.Count} URL(s) found in {group.Key}";
                root.value = groupStatus != USSFixerService.EStatus.OK;
                root.Clear();

                foreach (var candidate in candidates) {
                    string label = $"{_icons[candidate.status]} #{candidate.line}: {candidate.oldUrl}";
                    root.Add(new Label(label) { tooltip = candidate.message });
                }
            }
        }
    }
}