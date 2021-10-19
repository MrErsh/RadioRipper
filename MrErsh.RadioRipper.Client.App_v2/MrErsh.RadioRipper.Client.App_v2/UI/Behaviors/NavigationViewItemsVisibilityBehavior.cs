#pragma warning disable CA1416 // Validate platform compatibility
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.Xaml.Interactivity;
using MrErsh.RadioRipper.Client.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace MrErsh.RadioRipper.Client.UI.Behaviors
{
    [Obsolete]
    // TODO: вообще выпилить
    public sealed class NavigationViewItemsVisibilityBehavior : Behavior<NavigationView>
    {
        public static readonly DependencyProperty BoolValueProperty = DependencyProperty.Register(
            nameof(BoolValue),
            typeof(bool),
            typeof(NavigationViewItemsVisibilityBehavior),
            new PropertyMetadata(false, OnBoolValuePropertyChanged));


        public bool BoolValue
        {
            get => (bool)GetValue(BoolValueProperty);
            set => SetValue(BoolValueProperty, value);
        }

        protected override void OnAttached()
        {
            AssociatedObject.DataContextChanged += OnAssociatedObjectDataContextChanged;
            base.OnAttached();
        }

        protected override void OnDetaching()
        {
            AssociatedObject.DataContextChanged -= OnAssociatedObjectDataContextChanged;
            base.OnDetaching();
        }

        private void OnAssociatedObjectDataContextChanged(FrameworkElement sender, DataContextChangedEventArgs args)
        {
            var view = AssociatedObject;
            var items = AssociatedObject.MenuItemsSource as IEnumerable<NavItemBase>;
            if (items == null)
                return;

            var content = view.Content as Grid;
            if (content == null)
                return;

            var list = items.ToList();
            foreach (var item in list)
            {
                 var container = view.ContainerFromMenuItem(item as object);
                Debug.WriteLine(container);
            }
        }

        private static void OnBoolValuePropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            var view = obj as NavigationView;
            if (view == null)
                return;

            var items = view.MenuItemsSource as IEnumerable<NavItemBase>;
            if (items == null)
                return;

            var t = view.SelectedItem;
            var container = view.ContainerFromMenuItem(t);
            Debug.WriteLine(container);
        }
    }
}
#pragma warning disable CA1416 // Validate platform compatibility
