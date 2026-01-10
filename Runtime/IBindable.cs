#nullable enable

namespace Strayfarer.UI {
    public interface IBindable<T> {
        void Bind(T data);
    }
}