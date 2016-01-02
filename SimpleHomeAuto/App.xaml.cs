using SimpleHomeAuto.Data;
using SimpleHomeAuto.Voice;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace SimpleHomeAuto
{
    sealed partial class App : Application
    {
        public App()
        {
            Microsoft.ApplicationInsights.WindowsAppInitializer.InitializeAsync(
                Microsoft.ApplicationInsights.WindowsCollectors.Metadata |
                Microsoft.ApplicationInsights.WindowsCollectors.Session);

            this.InitializeComponent();
            this.Suspending += OnSuspending;
        }

        protected override void OnActivated(IActivatedEventArgs e)
        {
            base.OnActivated(e);
            
            if (e.Kind == ActivationKind.VoiceCommand)
            {
                var commandArgs = e as VoiceCommandActivatedEventArgs;

                Scenario scenario = VoiceService.GetScenarioFromInput(commandArgs);

                Frame rootFrame = Window.Current.Content as Frame;
                if (rootFrame != null)
                {
                    if (scenario != null)
                    {
                        rootFrame.Navigate(typeof(ActionPage), scenario);
                    }
                    else
                    {
                        rootFrame.Navigate(typeof(MainPage));
                    }
                }
            }
        }

        protected override void OnLaunched(LaunchActivatedEventArgs e)
        {
            Frame rootFrame = Window.Current.Content as Frame;

            // Do not repeat app initialization when the Window already has content,
            // just ensure that the window is active
            if (rootFrame == null)
            {
                // Create a Frame to act as the navigation context and navigate to the first page
                rootFrame = new Frame();

                rootFrame.NavigationFailed += OnNavigationFailed;
                rootFrame.Navigated += OnNavigated;

                // Place the frame in the current Window
                Window.Current.Content = rootFrame;

                var navMgr = SystemNavigationManager.GetForCurrentView();
                navMgr.BackRequested += OnBackRequested;
                navMgr.AppViewBackButtonVisibility =
                    rootFrame.CanGoBack ?
                    AppViewBackButtonVisibility.Visible :
                    AppViewBackButtonVisibility.Collapsed;

                //var titlebar = ApplicationView.GetForCurrentView().TitleBar;
                //titlebar.BackgroundColor = (Color)Resources["SystemColorWindowColor"];
            }

            Scenario scenario = null;
            if (!string.IsNullOrWhiteSpace(e.Arguments))
            {
                var all = Context.Instance.Scenarios;
                scenario = all.FirstOrDefault(s => s.Title == e.Arguments);
            }

            if (rootFrame.Content == null)
            {
                if (scenario != null)
                {
                    rootFrame.Navigate(typeof(ActionPage), scenario);
                }
                else
                {
                    rootFrame.Navigate(typeof(MainPage));
                }
            }
            else if (scenario != null)
            {
                rootFrame.Navigate(typeof(ActionPage), scenario);
            }

            // Ensure the current window is active
            Window.Current.Activate();
        }

        private void OnBackRequested(object sender, BackRequestedEventArgs e)
        {
            Frame rootFrame = Window.Current.Content as Frame;

            if (rootFrame.CanGoBack)
            {
                e.Handled = true;
                rootFrame.GoBack();
            }
        }

        private void OnNavigated(object sender, NavigationEventArgs e)
        {
            // Each time a navigation event occurs, update the Back button's visibility
            SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility =
                ((Frame)sender).CanGoBack ?
                AppViewBackButtonVisibility.Visible :
                AppViewBackButtonVisibility.Collapsed;
        }

        void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
        }
   
        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            deferral.Complete();
        }
    }
}
