#pragma warning disable CA1416 // Validate platform compatibility
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.Xaml.Interactivity;
using System;
using System.Collections.Generic;

namespace MrErsh.RadioRipper.Client.UI.Behaviors
{
    public class CalendarAvailableDatesBehavior : Behavior<CalendarDatePicker>
    {
        #region Dependency properties

        public static readonly DependencyProperty DatesProperty = DependencyProperty.Register(
            nameof(Dates),
            typeof(IEnumerable<DateTime>),
            typeof(CalendarAvailableDatesBehavior),
            new PropertyMetadata(null, OnDatesPropertyChanged));

        #endregion

        #region Fields

        private HashSet<DateTime> _dates = new HashSet<DateTime>();
        private CalendarPanel _calendarPanel;

        #endregion

        #region Overrides

        protected override void OnAttached()
        {
            AssociatedObject.DataContextChanged += OnAssociatedObjectDataContextChanged;
            AssociatedObject.CalendarViewDayItemChanging += OnAssociatedObjectCalendarViewDayItemChanging;
            AssociatedObject.Opened += OnAssociatedObjectOpened;
            base.OnAttached();
        }

        protected override void OnDetaching()
        {
            AssociatedObject.DataContextChanged -= OnAssociatedObjectDataContextChanged;
            AssociatedObject.CalendarViewDayItemChanging -= OnAssociatedObjectCalendarViewDayItemChanging;
            AssociatedObject.Opened -= OnAssociatedObjectOpened;
            base.OnDetaching();
        }

        #endregion

        #region Properties

        public IEnumerable<DateTime> Dates
        {
            get => (IEnumerable<DateTime>)GetValue(DatesProperty);
            set => SetValue(DatesProperty, value);
        }

        #endregion

        #region Event handlers

        private void OnAssociatedObjectOpened(object sender, object e)
        {
            if (_calendarPanel == null)
                return;

            var viewDayItems = _calendarPanel.GetAllChildren<CalendarViewDayItem>();
            foreach (var item in viewDayItems )
            {
                item.IsBlackout = !_dates.Contains(item.Date.Date);
            }
        }

        private void OnAssociatedObjectCalendarViewDayItemChanging(CalendarView sender, CalendarViewDayItemChangingEventArgs e)
        {
            if (_calendarPanel == null)
                _calendarPanel = e.Item.GetParent() as CalendarPanel;
        }

        private void OnAssociatedObjectDataContextChanged(FrameworkElement sender, DataContextChangedEventArgs args)
            => Update();

        private static void OnDatesPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            var calendar = (obj as CalendarAvailableDatesBehavior);
            if (calendar == null)
                return;

            calendar.Update();
        }

        #endregion

        private void Update()
        {
            _dates = new HashSet<DateTime>(Dates ?? Array.Empty<DateTime>());
        }
    }
}
#pragma warning disable CA1416 // Validate platform compatibility
