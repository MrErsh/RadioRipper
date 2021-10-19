using JetBrains.Annotations;
using Microsoft.UI.Xaml.Controls;
using MrErsh.RadioRipper.Client.Mvvm;
using Serilog;
using System;
using System.Threading;

namespace MrErsh.RadioRipper.Client.Services
{
    /// <summary>
    /// Простой сервис навигации. Только навигация первого уровня.
    /// </summary>
    public class SimpleNavigationService : INavigationService
    {
        #region Fields

        private readonly IThreadingService _threadingService;
        private readonly ILogger _logger;
        private readonly SemaphoreSlim _frameLoadedEvent;
        private readonly Lazy<Frame> _lazyRootFrame;

        private Frame RootFrame => _lazyRootFrame.Value;
        private Type _currentPage;

        private bool _isInitialized;

        #endregion

        #region Constructor

        public SimpleNavigationService([NotNull] IThreadingService threadingService,
                                       [NotNull] ILogger logger,
                                       [NotNull] Func<Frame> rootFrameFactory,
                                       [NotNull] SemaphoreSlim frameLoadedEvent)
        {
            _threadingService = threadingService;
            _lazyRootFrame = new Lazy<Frame>(rootFrameFactory);
            _frameLoadedEvent = frameLoadedEvent;
            _logger = logger;
        }

        #endregion

        #region Implementation of INavigationService

        public async void NavigateTo(Type pageType)
        {
            if (pageType == _currentPage)
                return;

            _logger.Information("Navigated to {pageType}", pageType);
            if (!_lazyRootFrame.IsValueCreated)
            {
                await _frameLoadedEvent.WaitAsync();
                _frameLoadedEvent.Release();

                if (!_isInitialized)
                {
                    _isInitialized = true;
                    RootFrame.Navigating += OnRootFrameNavigating;
                    RootFrame.Navigated += OnRootFrameNavigated;
                }
            }
            try
            {
                await _threadingService.OnUiThread(() => RootFrame.Navigate(pageType));
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Navigation to {Page} failed", pageType.Name);
            }

            _currentPage = pageType;
        }

        #endregion

        #region Private methods

        private async void OnRootFrameNavigating(object sender, Microsoft.UI.Xaml.Navigation.NavigatingCancelEventArgs e)
        {
            var navigable = GetFrameContent();
            if (navigable != null)
                await navigable.NavigatedFrom();
        }

        private async void OnRootFrameNavigated(object sender, Microsoft.UI.Xaml.Navigation.NavigationEventArgs e)
        {
            var navigable = GetFrameContent();
            if (navigable != null)
                await navigable.NavigatedTo();
        }

        [CanBeNull]
        private INavigable GetFrameContent()
        {
            INavigable navigable = null;
            _threadingService.OnUiThread(() => navigable = GetNavigable());
            return navigable;
        }

        private INavigable GetNavigable()
        {
            var page = RootFrame.Content as Page;
            var dc = page?.DataContext;
            return dc as INavigable;
        }

        #endregion
    }
}
