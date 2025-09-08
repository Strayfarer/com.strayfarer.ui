using NUnit.Framework;
using UnityEditor;
using UObject = UnityEngine.Object;

namespace Strayfarer.UI.Editor {
    [TestFixture]
    [TestOf(typeof(ToolkitUtils))]
    sealed class ToolkitUtilsTests {
        [TestCase("project://database/Packages/com.strayfarer.ui/Tests/EditMode/Assets/MainMenu_TestAsset.asset?fileID=11400000&guid=33de5180558967149b13f8ed93eaa2a0&type=2#MainMenu_TestAsset", "Packages/com.strayfarer.ui/Tests/Assets/MainMenu_TestAsset.asset")]
        public void GivenUrl_LoadAsset_ShouldReturnAsset(string url, string expectedPath) {
            var expected = AssetDatabase.LoadMainAssetAtPath(expectedPath);
            Assert.IsNotNull(expected);

            var actual = ToolkitUtils.LoadAsset<UObject>(url);

            Assert.That(actual, Is.EqualTo(expected));
        }

        [TestCase("project://database/Packages/com.strayfarer.ui/Tests/Assets/MainMenu_TestAsset.asset", "Packages/com.strayfarer.ui/Tests/Assets/MainMenu_TestAsset.asset")]
        [TestCase("project://database/Assets?fileID=11400000&guid=33de5180558967149b13f8ed93eaa2a0&type=2#MainMenu_TestAsset", "Packages/com.strayfarer.ui/Tests/Assets/MainMenu_TestAsset.asset")]
        [TestCase("project://database/Assets", "Assets")]
        public void GivenUrlWithMissingParts_LoadAsset_ShouldReturnAsset(string url, string expectedPath) {
            var expected = AssetDatabase.LoadMainAssetAtPath(expectedPath);
            Assert.IsNotNull(expected);

            var actual = ToolkitUtils.LoadAsset<UObject>(url);

            Assert.That(actual, Is.EqualTo(expected));
        }
    }
}
