using System;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using UObject = UnityEngine.Object;

namespace Strayfarer.UI.Editor {
    public sealed class TransientObjectField<T> : ObjectField where T : UObject {
        public TransientObjectField(string label, Func<T> getter, Action<T> setter) : base(label) {
            objectType = typeof(T);
            value = getter();
            this.RegisterValueChangedCallback(evt => {
                setter(evt.newValue as T);
            });

            style.marginRight = 0;
        }
    }
}
