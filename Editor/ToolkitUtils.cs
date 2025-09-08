using System;
using Slothsoft.UnityExtensions.Editor;
using UnityEditor;
using UObject = UnityEngine.Object;

namespace Strayfarer.UI.Editor {
    static class ToolkitUtils {
        internal static T LoadAsset<T>(string url) where T : UObject {
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
    }
}
