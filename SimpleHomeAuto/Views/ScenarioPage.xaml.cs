using SimpleHomeAuto.Data;
using SimpleHomeAuto.Voice;
using System;
using System.Text.RegularExpressions;
using Windows.Foundation;
using Windows.UI.StartScreen;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace SimpleHomeAuto
{
    public sealed partial class ScenarioPage : Page
    {
        Scenario scenario;

        public ScenarioPage()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            scenario = (e.Parameter as Scenario);
            if (scenario == null)
            {
                // New Scenario
                scenario = new Scenario();
                SaveGrid.Visibility = Visibility.Visible;
                ViewLaunch.Visibility = Visibility.Collapsed;
            } else
            {
                // Edit existing
                SaveGrid.Visibility = Visibility.Collapsed;
                ViewLaunch.Visibility = Visibility.Visible;
            }
        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            // Fix for binding not updating ?
            scenario.Title = Title.Text;
            scenario.Url = Url.Text;
            scenario.VoiceCommand = VoiceCommand.Text;

            Context.Instance.SaveSettings();
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            this.DataContext = scenario;

            // Pinning
            if (!string.IsNullOrWhiteSpace(scenario.Title))
            {
                if (SecondaryTile.Exists(scenario.Title)) PinToStart.IsOn = true;
            }
        }
                
        private async void OnPinToStart(object sender, RoutedEventArgs e)
        {
            try
            {
                var fixedtitle = Regex.Replace(scenario.Title, @"\W", "-");
                if (PinToStart.IsOn)
                {
                    if (string.IsNullOrWhiteSpace(fixedtitle))
                    {
                        PinToStart.IsOn = false;
                    }
                    else if (!SecondaryTile.Exists(fixedtitle))
                    {
                        Uri logo = new Uri(@"ms-appx:///Assets/Square150x150Logo.scale-200.png");
                        var tile = new SecondaryTile(fixedtitle,
                                                     scenario.Title,
                                                     scenario.Title,
                                                     logo,
                                                     TileSize.Default);

                        await tile.RequestCreateForSelectionAsync(GetElementRect((FrameworkElement)sender));
                    }
                }
                else if (!string.IsNullOrWhiteSpace(fixedtitle))
                {
                    if (SecondaryTile.Exists(fixedtitle))
                    {
                        SecondaryTile secondaryTile = new SecondaryTile(fixedtitle);
                        Windows.Foundation.Rect rect = GetElementRect((FrameworkElement)sender);
                        Windows.UI.Popups.Placement placement = Windows.UI.Popups.Placement.Above;
                        bool isUnpinned = await secondaryTile.RequestDeleteForSelectionAsync(rect, placement);
                    }
                }
            }
            catch (Exception ex)
            {
                ex.ToString();
                throw;
            }
        }

        private void OnLaunch(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(scenario.Url))
            {
                this.Frame.Navigate(typeof(ActionPage), scenario);
            }
        }

        private void OnSave(object sender, RoutedEventArgs e)
        {
            // Fix for binding not updating ?
            scenario.Title = Title.Text;
            scenario.Url = Url.Text;
            scenario.VoiceCommand = VoiceCommand.Text;

            Context.Instance.Scenarios.Add(scenario);
            Context.Instance.SaveSettings();

            this.Frame.Navigate(typeof(MainPage));
        }

        private void OnCancel(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(MainPage));
        }

        public static Rect GetElementRect(FrameworkElement element)
        {
            GeneralTransform buttonTransform = element.TransformToVisual(null);
            Point point = buttonTransform.TransformPoint(new Point());
            return new Rect(point, new Size(element.ActualWidth, element.ActualHeight));
        }
    }
}
