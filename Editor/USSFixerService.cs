using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Slothsoft.UnityExtensions.Editor;
using UnityEditor;
using UnityEngine;
using UObject = UnityEngine.Object;

namespace Strayfarer.UI.Editor {
    static class USSFixerService {
        internal sealed class Options {
            public bool includeAssets = true;
            public bool includePackages = true;
        }

        internal enum EStatus {
            OK,
            ShouldFix,
            Error,
        }

        internal struct Candidate {
            public string filePath;
            public int line;
            public string oldUrl;
            public string newUrl;
            public EStatus status;
            public string message;
        }

        static readonly string[] SUPPORTED_EXTENSIONS = new[] { ".uss", "" };

        static readonly Regex UrlRegex = new(
            @"\s*[""'](?<url>project://database/[^""']+)[""']\s*",
            RegexOptions.Compiled | RegexOptions.CultureInvariant);

        internal static List<Candidate> Scan(Options opts) {
            var result = new List<Candidate>();

            var roots = new List<string>();
            if (opts.includeAssets) {
                roots.Add(new DirectoryInfo("Assets").FullName);
            }

            if (opts.includePackages) {
                roots.Add(new DirectoryInfo("Packages").FullName);
            }

            var files = AssetDatabase
                .GetAllAssetPaths()
                .Where(path => SUPPORTED_EXTENSIONS.Any(path.EndsWith))
                .Select(path => new FileInfo(path))
                .Where(info => info.Exists && !info.IsReadOnly)
                .Select(info => info.FullName)
                .Where(path => roots.Any(path.StartsWith))
                .OrderBy(path => path)
                .ToList();

            int i = 0;
            try {
                foreach (string file in files) {
                    EditorUtility.DisplayProgressBar("Scanning USS", file, ++i / (float)Math.Max(1, files.Count));
                    ScanFile(file, result);
                }
            } finally {
                EditorUtility.ClearProgressBar();
            }

            return result;
        }

        static void ScanFile(string filePath, List<Candidate> result) {
            string text = File.ReadAllText(filePath);

            foreach (Match m in UrlRegex.Matches(text)) {
                string oldUrl = m.Groups["url"].Value;
                bool encodeEntities = oldUrl.Contains("&amp;");

                var candidate = new Candidate {
                    filePath = filePath,
                    line = GetLineNumberByCharacterIndex(text, m.Index),
                    oldUrl = oldUrl,
                };

                var parsed = ParseProjectUrl(oldUrl.Replace("&amp;", "&"));
                if (!string.IsNullOrEmpty(parsed.error)) {
                    candidate.newUrl = "(unparsed)";
                    candidate.status = EStatus.Error;
                    candidate.message = $"Cannot parse URL: {parsed.error}";
                    result.Add(candidate);

                    continue;
                }

                string newPath = AssetDatabase.GUIDToAssetPath(parsed.guid);
                if (string.IsNullOrEmpty(newPath)) {
                    candidate.newUrl = "(missing asset)";
                    candidate.status = EStatus.Error;
                    candidate.message = $"GUID not found in project: {parsed.guid}";
                    result.Add(candidate);

                    continue;
                }

                if (AssetUtils.TryLoadSubAssetAtPath(newPath, parsed.fileId, out UObject asset)) {
                    parsed.fragment = $"#{asset.name}";
                }

                string newUrl = $"project://database/{EncodeProjectPath(newPath)}{parsed.query}{parsed.fragment}";
                if (encodeEntities) {
                    newUrl = newUrl.Replace("&", "&amp;");
                }

                bool isDifferent = !string.Equals(newUrl, oldUrl, StringComparison.Ordinal);

                candidate.newUrl = newUrl;
                if (isDifferent) {
                    candidate.status = EStatus.ShouldFix;
                    candidate.message = $"Will replace to: {newUrl}";
                } else {
                    candidate.status = EStatus.OK;
                    candidate.message = "Everything OK!";
                }

                result.Add(candidate);
            }
        }

        internal static List<string> FixNow(IEnumerable<Candidate> candidates) {
            var changedFiles = new List<string>();

            try {
                AssetDatabase.StartAssetEditing();

                var byFile = candidates
                    .Where(c => c.status == EStatus.ShouldFix)
                    .GroupBy(c => c.filePath)
                    .ToList();

                for (int i = 0; i < byFile.Count; i++) {
                    string path = byFile[i].Key;
                    EditorUtility.DisplayProgressBar("Fixing USS", path, Mathf.InverseLerp(0, byFile.Count, i));
                    try {
                        changedFiles.Add(path);
                        string original = File.ReadAllText(path);
                        string replaced = original;
                        foreach (var candidate in byFile[i]) {
                            replaced = replaced.Replace(candidate.oldUrl, candidate.newUrl);
                        }

                        File.WriteAllText(path, replaced);
                    } catch (Exception e) {
                        Debug.LogError($"USS URL Fixer: failed to patch '{path}': {e.Message}");
                    }
                }
            } finally {
                AssetDatabase.StopAssetEditing();
                EditorUtility.ClearProgressBar();
            }

            return changedFiles;
        }

        static int GetLineNumberByCharacterIndex(string text, int index) {
            int line = 1;
            for (int i = 0; i < index && i < text.Length; i++) {
                if (text[i] == '\n') {
                    line++;
                }
            }

            return line;
        }

        static string EncodeProjectPath(string assetPath) {
            return string.Join('/', assetPath.Split('/').Select(Uri.EscapeDataString));
        }

        struct ParsedProjectUrl {
            public string query;
            public string fragment;

            public string guid;
            public long fileId;

            public string error;
        }

        static ParsedProjectUrl ParseProjectUrl(string url) {
            var parsed = new ParsedProjectUrl();

            if (Uri.TryCreate(url, UriKind.Absolute, out var uri)) {
                parsed.query = uri.Query;
                parsed.fragment = uri.Fragment;

                var query = new ParsedQuery(uri.Query);

                parsed.guid = query["guid"];
                parsed.fileId = long.Parse(query["fileID"]);
            } else {
                parsed.error = $"Failed to parse URL {url}";
            }

            return parsed;
        }
    }
}