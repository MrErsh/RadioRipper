using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using MrErsh.RadioRipper.Client.Mvvm;
using System;

namespace MrErsh.RadioRipper.Client.UI.Controls
{
    [TemplateVisualState(GroupName = STATES_GROUP_NAME, Name = IS_ON_STATE)]
    [TemplateVisualState(GroupName = STATES_GROUP_NAME, Name = IN_PROGRESS_STATE)]
    [TemplateVisualState(GroupName = STATES_GROUP_NAME, Name = IS_OFF_STATE)]
    public sealed class AsyncToggleSwitch : ButtonBase
    {
        #region Constants

        public const string STATES_GROUP_NAME = "ToggleStates";

        public const string IS_ON_STATE = "IsOn";
        public const string IN_PROGRESS_STATE = "InProgress";
        public const string IS_OFF_STATE = "IsOff";

        #endregion

        private string _curState = IS_OFF_STATE;

        #region Dependency properties

        public static readonly DependencyProperty IsOnProperty = DependencyProperty.Register(
            nameof(IsOn),
            typeof(bool),
            typeof(AsyncToggleSwitch),
            new PropertyMetadata(false, OnIsOnPropertyChanged));

        public static readonly DependencyProperty OnBrushProperty = DependencyProperty.Register(
            nameof(OnBrush),
            typeof(Brush),
            typeof(AsyncToggleSwitch),
            new PropertyMetadata(null));

        public static readonly DependencyProperty AsyncCommandProperty = DependencyProperty.Register(
            nameof(AsyncCommand),
            typeof(AsyncExecutionCommandBase<bool>),
            typeof(AsyncToggleSwitch),
            new PropertyMetadata(null));

        #endregion

        public AsyncToggleSwitch()
        {
            DefaultStyleKey = typeof(AsyncToggleSwitch);
        }

        #region Overrides

        protected override async void OnTapped(TappedRoutedEventArgs e)
        {
            if (AsyncCommand == null || !AsyncCommand.CanExecute(CommandParameter))
                return;

            var prevState = _curState;
            VisualStateManager.GoToState(this, IN_PROGRESS_STATE, false);
            await AsyncCommand.ExecuteAsync(CommandParameter);
            var state = GetState(AsyncCommand.Execution, prevState);
            ToState(state);
        }

        #endregion

        #region Properties

        public bool IsOn
        {
            get => (bool)GetValue(IsOnProperty);
            set => SetValue(IsOnProperty, value);
        }

        public Brush OnBrush
        {
            get => (Brush)GetValue(OnBrushProperty);
            set => SetValue(OnBrushProperty, value);
        }

        public AsyncExecutionCommandBase<bool> AsyncCommand
        {
            get => (AsyncExecutionCommandBase<bool>)GetValue(AsyncCommandProperty);
            set => SetValue(AsyncCommandProperty, value);
        }

        #endregion

        #region Private methods

        private static void OnIsOnPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            if (obj is not AsyncToggleSwitch ctrl)
                return;

            var val = (bool)args.NewValue;
            VisualStateManager.GoToState(ctrl, val ? IS_ON_STATE : IS_OFF_STATE, false);
        }

        private void ToState(string state)
        {
            VisualStateManager.GoToState(this, state, false);
            _curState = state;
        }

        private static string GetState(NotifyTaskCompletion<bool> execution, string prevState)
        {
            if (execution.IsCompletedSuccessfully)
                return execution.Result ? IS_ON_STATE : IS_OFF_STATE;

            return prevState;
        }

        #endregion
    }
}
