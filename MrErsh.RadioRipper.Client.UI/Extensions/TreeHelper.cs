using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media;
using System.Linq;
using System.Collections.Generic;

namespace MrErsh.RadioRipper.Client.UI
{
    public static class DependencyObjectExtensions
    {
        public static IEnumerable<DependencyObject> GetAllChildren(this DependencyObject el)
        {
            var count = VisualTreeHelper.GetChildrenCount(el);
            for (var i = 0; i < count; i++)
            {
                var ch = VisualTreeHelper.GetChild(el, i);
                yield return ch;
                foreach (var child in ch.GetAllChildren())
                    yield return child;
            }
        }

        public static IEnumerable<T> GetAllChildren<T>(this DependencyObject el)
        {
            return el.GetAllChildren().OfType<T>();
        }

        public static DependencyObject GetParent(this DependencyObject el)
        {
            return VisualTreeHelper.GetParent(el);
        }
    }
}
