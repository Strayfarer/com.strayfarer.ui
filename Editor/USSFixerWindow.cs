using UnityEditor;

namespace Strayfarer.UI.Editor {
    sealed class USSFixerWindow : EditorWindow {
        const string TITLE = "USS Fixer";

        [MenuItem("Window/UI Toolkit/" + TITLE)]
        static void Open() => GetWindow<USSFixerWindow>(TITLE).Show();

        public void CreateGUI() {
            rootVisualElement.Add(new USSFixerElement());
        }
    }
}