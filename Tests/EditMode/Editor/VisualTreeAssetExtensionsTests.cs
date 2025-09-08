using System;
using System.Linq;
using NUnit.Framework;
using UnityEditor;
using UnityEngine.UIElements;

namespace Strayfarer.UI.Editor {
    [TestFixture]
    [TestOf(typeof(VisualTreeAssetExtensions))]
    sealed class VisualTreeAssetExtensionsTests {
        const string TEST_ASSET = "Packages/com.strayfarer.ui/Tests/Assets/UXML_TestAsset.uxml";

        VisualTreeAsset sut;
        SerializedObject serialized;

        [SetUp]
        public void SetUp() {
            sut = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(TEST_ASSET);
            Assert.IsNotNull(sut);
            serialized = new(sut);
        }

        [TestCase("m_VisualElementAssets.Array.data[1]", "Strayfarer.UI.PageView")]
        [TestCase("m_VisualElementAssets.Array.data[2].m_Id", "UnityEngine.UIElements.VisualElement")]
        [TestCase("m_VisualElementAssets.Array.data[5].m_SerializedData.text", "Strayfarer.UI.BindableButton")]
        public void GivenSerializedProperty_GetUxmlTag_ShouldReturnFullTypeName(string propertyPath, string expected) {
            var property = serialized.FindProperty(propertyPath);
            Assert.IsNotNull(property);

            string actual = property.GetUxmlTag();

            Assert.That(actual, Is.EqualTo(expected));
        }

        [TestCase("m_VisualElementAssets.Array.data[1]", typeof(PageView))]
        [TestCase("m_VisualElementAssets.Array.data[2].m_Id", typeof(VisualElement))]
        [TestCase("m_VisualElementAssets.Array.data[5].m_SerializedData.text", typeof(BindableButton))]
        public void GivenSerializedProperty_GetUxmlType_ShouldReturnFullType(string propertyPath, Type expected) {
            var property = serialized.FindProperty(propertyPath);
            Assert.IsNotNull(property);

            var actual = property.GetUxmlType();

            Assert.That(actual, Is.EqualTo(expected));
        }

        [TestCase("m_VisualElementAssets.Array.data[1]", "active-index")]
        [TestCase("m_VisualElementAssets.Array.data[1]", "data-source")]
        [TestCase("m_VisualElementAssets.Array.data[2].m_Id", "name")]
        [TestCase("m_VisualElementAssets.Array.data[5].m_SerializedData.text", "text")]
        public void GivenExistingProperty_TryGetUxmlAttribute_ShouldReturnTrue(string propertyPath, string attribute) {
            var property = serialized.FindProperty(propertyPath);
            Assert.IsNotNull(property);

            bool actual = property.TryGetUxmlAttribute(attribute, out _);

            Assert.That(actual, Is.True);
        }

        [TestCase("m_VisualElementAssets.Array.data[1]", "active-index", "0")]
        [TestCase("m_VisualElementAssets.Array.data[1]", "data-source", "project://database/Packages/com.strayfarer.ui/Tests/Assets/MainMenu_TestAsset.asset?fileID=11400000&guid=33de5180558967149b13f8ed93eaa2a0&type=2#MainMenu_TestAsset")]
        [TestCase("m_VisualElementAssets.Array.data[2].m_Id", "name", "MainMenu")]
        [TestCase("m_VisualElementAssets.Array.data[5].m_SerializedData.text", "text", "Button")]
        public void GivenExistingProperty_TryGetUxmlAttribute_ShouldSetValue(string propertyPath, string attribute, string expected) {
            var property = serialized.FindProperty(propertyPath);
            Assert.IsNotNull(property);

            property.TryGetUxmlAttribute(attribute, out string actual);

            Assert.That(actual, Is.EqualTo(expected));
        }

        [TestCase("m_VisualElementAssets.Array.data[1]", "active-index-no")]
        [TestCase("m_VisualElementAssets.Array.data[1]", "data-source-no")]
        [TestCase("m_VisualElementAssets.Array.data[2].m_Id", "name-no")]
        [TestCase("m_VisualElementAssets.Array.data[5].m_SerializedData.text", "text-no")]
        public void GivenMissingProperty_TryGetUxmlAttribute_ShouldReturnFalse(string propertyPath, string attribute) {
            var property = serialized.FindProperty(propertyPath);
            Assert.IsNotNull(property);

            bool actual = property.TryGetUxmlAttribute(attribute, out _);

            Assert.That(actual, Is.False);
        }

        [TestCase("m_VisualElementAssets.Array.data[5].m_SerializedData.text", "active-index")]
        [TestCase("m_VisualElementAssets.Array.data[5].m_SerializedData.text", "data-source")]
        [TestCase("m_VisualElementAssets.Array.data[5].m_SerializedData.text", "name")]
        [TestCase("m_VisualElementAssets.Array.data[5].m_SerializedData.text", "text")]
        public void GivenExistingProperty_TryGetUxmlAttributeInParent_ShouldReturnTrue(string propertyPath, string attribute) {
            var property = serialized.FindProperty(propertyPath);
            Assert.IsNotNull(property);

            bool actual = property.TryGetUxmlAttributeInParent(attribute, out _);

            Assert.That(actual, Is.True);
        }

        [TestCase("m_VisualElementAssets.Array.data[5].m_SerializedData.text", "active-index", "0")]
        [TestCase("m_VisualElementAssets.Array.data[5].m_SerializedData.text", "data-source", "project://database/Packages/com.strayfarer.ui/Tests/Assets/MainMenu_TestAsset.asset?fileID=11400000&guid=33de5180558967149b13f8ed93eaa2a0&type=2#MainMenu_TestAsset")]
        [TestCase("m_VisualElementAssets.Array.data[5].m_SerializedData.text", "name", "Quit")]
        [TestCase("m_VisualElementAssets.Array.data[5].m_SerializedData.text", "text", "Button")]
        public void GivenExistingProperty_TryGetUxmlAttributeInParent_ShouldSetValue(string propertyPath, string attribute, string expected) {
            var property = serialized.FindProperty(propertyPath);
            Assert.IsNotNull(property);

            property.TryGetUxmlAttributeInParent(attribute, out string actual);

            Assert.That(actual, Is.EqualTo(expected));
        }

        [TestCase("m_VisualElementAssets.Array.data[5].m_SerializedData.text", "active-index-no")]
        [TestCase("m_VisualElementAssets.Array.data[5].m_SerializedData.text", "data-source-no")]
        [TestCase("m_VisualElementAssets.Array.data[5].m_SerializedData.text", "name-no")]
        [TestCase("m_VisualElementAssets.Array.data[5].m_SerializedData.text", "text-no")]
        public void GivenMissingProperty_TryGetUxmlAttributeInParent_ShouldReturnFalse(string propertyPath, string attribute) {
            var property = serialized.FindProperty(propertyPath);
            Assert.IsNotNull(property);

            bool actual = property.TryGetUxmlAttributeInParent(attribute, out _);

            Assert.That(actual, Is.False);
        }

        [TestCase("m_VisualElementAssets.Array.data[1]")]
        [TestCase("m_VisualElementAssets.Array.data[5].m_SerializedData.text")]
        public void GivenDataSource_TryGetUxmlDataSourceType_ShouldReturnTrue(string propertyPath) {
            var property = serialized.FindProperty(propertyPath);
            Assert.IsNotNull(property);

            bool actual = property.TryGetUxmlDataSourceType(out _);

            Assert.That(actual, Is.True);
        }

        [TestCase("m_VisualElementAssets.Array.data[1]", typeof(TestAsset))]
        [TestCase("m_VisualElementAssets.Array.data[5].m_SerializedData.text", typeof(UnityEngine.InputSystem.HID.HID.Button))]
        public void GivenDataSource_TryGetUxmlDataSourceType_ShouldSetType(string propertyPath, Type expected) {
            var property = serialized.FindProperty(propertyPath);
            Assert.IsNotNull(property);

            property.TryGetUxmlDataSourceType(out var actual);

            Assert.That(actual, Is.EqualTo(expected));
        }

        [TestCase("m_VisualElementAssets.Array.data[0]")]
        public void GivenNoDataSource_TryGetUxmlDataSourceType_ShouldReturnFalse(string propertyPath) {
            var property = serialized.FindProperty(propertyPath);
            Assert.IsNotNull(property);

            bool actual = property.TryGetUxmlDataSourceType(out _);

            Assert.That(actual, Is.False);
        }

        [TestCase("m_VisualElementAssets.Array.data[0]", "m_VisualElementAssets.Array.data[1]")]
        [TestCase("m_VisualElementAssets.Array.data[1]", "m_VisualElementAssets.Array.data[2]", "m_VisualElementAssets.Array.data[6]", "m_VisualElementAssets.Array.data[11]")]
        [TestCase("m_VisualElementAssets.Array.data[5].m_SerializedData.text")]
        public void GivenChildren_GetUxmlChildElements_ShouldReturnChildren(string propertyPath, params string[] expected) {
            var property = serialized.FindProperty(propertyPath);
            Assert.IsNotNull(property);

            var elements = property.GetUxmlChildElements();

            Assert.That(elements.Select(e => e.propertyPath), Is.EqualTo(expected));
        }

        [TestCase("m_VisualElementAssets.Array.data[0]", "")]
        [TestCase("m_VisualElementAssets.Array.data[1]", "m_VisualElementAssets.Array.data[1]")]
        [TestCase("m_VisualElementAssets.Array.data[5].m_SerializedData.text", "m_VisualElementAssets.Array.data[1]")]
        public void GivenDescendantOfPageView_GetUxmlAncestorOrSelf_ShouldReturnPageView(string propertyPath, string expectedPath) {
            var property = serialized.FindProperty(propertyPath);
            Assert.IsNotNull(property);

            bool result = property.TryGetUxmlAncestorOrSelf<PageView>(out var ancestor);

            if (string.IsNullOrEmpty(expectedPath)) {
                Assert.That(result, Is.False);
            } else {
                Assert.That(result, Is.True);
                Assert.That(ancestor, Is.Not.Null);
                Assert.That(ancestor.propertyPath, Is.EqualTo(expectedPath));
            }
        }

        [TestCase("m_VisualElementAssets.Array.data[1]", "PageView")]
        [TestCase("m_VisualElementAssets.Array.data[2]", "#MainMenu")]
        [TestCase("m_VisualElementAssets.Array.data[5].m_SerializedData.text", "#Quit")]
        public void GivenElement_GetUxmlDisplayName_ShouldUseName(string propertyPath, string expected) {
            var property = serialized.FindProperty(propertyPath);
            Assert.IsNotNull(property);

            string actual = property.GetUxmlDisplayName();

            Assert.That(actual, Is.EqualTo(expected));
        }
    }
}
