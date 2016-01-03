using SimpleHomeAuto.Data;
using SimpleHomeAuto.Views;
using SimpleHomeAuto.Voice;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Resources;
using Windows.Foundation;
using Windows.UI.StartScreen;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace SimpleHomeAuto
{
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
        }

        private async void OnLoaded(object sender, RoutedEventArgs e)
        {
            var context = Context.Instance;
            context.LoadSettings();
            ListScenarios.ItemsSource = context.Scenarios;

            try
            {
                await VoiceService.InstallOrUpdateCortana();
            }
            catch
            {
                ViewMessage.Visibility = Visibility.Visible;
                var loader = Windows.ApplicationModel.Resources.ResourceLoader.GetForCurrentView();
                Message.Text = loader.GetString("CortanaNotFound");
            }
        }
        
        private void OnSwipe(object sender, Universal.UI.Xaml.Controls.ItemSwipeEventArgs e)
        {
            var item = e.SwipedItem as Scenario;
            if (item != null)
            {
                var context = Context.Instance;
                context.Scenarios.Remove(item);

                context.SaveSettings();
            }
        }

        private void OnClick(object sender, ItemClickEventArgs e)
        {
            var item = e.ClickedItem as Scenario;
            if (item != null)
            {
                Frame.Navigate(typeof(ScenarioPage), item);
            }
        }

        private void OnAddScenario(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            Frame.Navigate(typeof(ScenarioPage), null);
        }

        private void OnLaunched(object sender, RoutedEventArgs e)
        {
            var item = (sender as Button).DataContext as Scenario;
            if (item != null)
            {
                Frame.Navigate(typeof(ActionPage), item);
            }
        }

        private void OnInfo(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(InfoPage));
        }
    }
}
