#nullable enable
using System;
using System.Text.RegularExpressions;
using Slothsoft.UnityExtensions.Editor;
using UnityEditor;
using UnityEngine.UIElements;
using UObject = UnityEngine.Object;

namespace Strayfarer.UI.Editor {
    static class ToolkitUtils {
        internal static T? LoadAsset<T>(string url) where T : UObject {
            var uri = new Uri(url);

            var query = new ParsedQuery(uri.Query);
            if (query["guid"] is string guid && long.TryParse(query["fileID"], out long fileId)) {
                string pathFromQuery = AssetDatabase.GUIDToAssetPath(guid);
                if (!string.IsNullOrEmpty(pathFromQuery) && AssetUtils.TryLoadSubAssetAtPath<T>(pathFromQuery, fileId, out var asset)) {
                    return asset;
                }
            }

            string pathFromUri = uri.AbsolutePath[1..];

            if (!string.IsNullOrEmpty(pathFromUri)) {
                return AssetDatabase.LoadAssetAtPath<T>(pathFromUri);
            }

            return default;
        }

        public static void AddKebabToClassList(this VisualElement element, string className) {
            className = Regex.Replace(className, @"(?<!^)(?<!-)((?<=\p{Ll})\p{Lu}|\p{Lu}(?=\p{Ll}))", "-$1").ToLower();
            element.AddToClassList(className);
        }

        public static void AddFieldAlignmentToClassList<T>(this BaseField<T> field) {
            field.AddToClassList("unity-base-field__aligned");
        }

        const string USS_PATH = "Packages/com.strayfarer.ui/USS/Editor.uss";

        internal static void AddEditorStyleSheet(this VisualElement element) {
            element.styleSheets.Add(AssetDatabase.LoadAssetAtPath<StyleSheet>(USS_PATH));
        }
    }
}
