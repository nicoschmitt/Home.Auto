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
        public Scenario Scenario { get; set; }
        public bool IsNewOne { get; set; }

        public ScenarioPage()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            Scenario = (e.Parameter as Scenario);
            if (Scenario == null)
            {
                // New Scenario
                Scenario = new Scenario();
                IsNewOne = true;
            } else
            {
                // Edit existing
                IsNewOne = false;
            }

            Title.Text = Scenario.Title ?? "";
            Url.Text = Scenario.Url ?? "";
            VoiceCommand.Text = Scenario.VoiceCommand ?? "";
        }

        //protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        //{
        //    // Fix for binding not updating ?
        //    scenario.Title = Title.Text;
        //    scenario.Url = Url.Text;
        //    scenario.VoiceCommand = VoiceCommand.Text;

        //    Context.Instance.SaveSettings();
        //}

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            // Pinning
            if (!string.IsNullOrWhiteSpace(Scenario.Title))
            {
                var fixedtitle = Regex.Replace(Scenario.Title, @"\W", "-");
                if (SecondaryTile.Exists(fixedtitle)) PinToStart.IsOn = true;
            }
        }
                
        private async void OnPinToStart(object sender, RoutedEventArgs e)
        {
            try
            {
                var fixedtitle = Regex.Replace(Scenario.Title, @"\W", "-");
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
                                                     Scenario.Title,
                                                     Scenario.Title,
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

        //private void OnLaunch(object sender, RoutedEventArgs e)
        //{
        //    if (!string.IsNullOrWhiteSpace(scenario.Url))
        //    {
        //        this.Frame.Navigate(typeof(ActionPage), scenario);
        //    }
        //}

        private async void OnSave(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(Title.Text) && !string.IsNullOrWhiteSpace(Url.Text))
            {
                Scenario.Title = Title.Text;
                Scenario.Url = Url.Text;
                Scenario.VoiceCommand = VoiceCommand.Text;

                if (IsNewOne) Context.Instance.Scenarios.Add(Scenario);
                Context.Instance.SaveSettings();

                await VoiceService.UpdateAvailableScenarios();

                this.Frame.Navigate(typeof(MainPage));
            }
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
