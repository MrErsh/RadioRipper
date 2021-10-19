#pragma warning disable CA1416 // Validate platform compatibility
using Microsoft.UI.Input;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.Xaml.Interactivity;
using System.Linq;
using System.Text;

namespace MrErsh.RadioRipper.Client.UI.Behaviors
{
    public class ListViewCopyableBehavior : Behavior<ListView>
    {
        public static readonly DependencyProperty ValuePathProperty = DependencyProperty.Register(
            nameof(ValuePath),
            typeof(string),
            typeof(ListViewCopyableBehavior),
            null);

        public string ValuePath 
        { 
            get => (string)GetValue(ValuePathProperty);
            set => SetValue(ValuePathProperty, value);
        }

        protected override void OnAttached()
        {
            AssociatedObject.KeyDown += OnAssociatedObjectKeyUp;
            base.OnAttached();
        }

        protected override void OnDetaching()
        {
            AssociatedObject.KeyDown -= OnAssociatedObjectKeyUp;
            base.OnDetaching();
        }

        private void OnAssociatedObjectKeyUp(object sender, Microsoft.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            if (!(sender is ListView list) || string.IsNullOrWhiteSpace(ValuePath))
                return;        

            if (e.Key != Windows.System.VirtualKey.C)
                return;

            // WinUI wtf?
            var ctrlState = KeyboardInput.GetKeyStateForCurrentThread(Windows.System.VirtualKey.Control);
            if (ctrlState == Windows.UI.Core.CoreVirtualKeyStates.Down)
                return;

            var items = list.SelectedItems;
            if ((items?.Count ?? 0) == 0)
                return;

            var sb = new StringBuilder();
            var prop = items[0].GetType().GetProperty(ValuePath);
            var resultList = items.Select(x => prop.GetValue(x)?.ToString() ?? string.Empty);
            ClipboardHelper.CopyList(resultList);
        }
    }
}
#pragma warning restore CA1416 // Validate platform compatibility
