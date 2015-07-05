using Windows.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using HN10.API;
using Windows.Web.Http;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Core;
using Windows.UI.Xaml.Navigation;

namespace HN10.Views
{

    public sealed partial class MainPage : Page {
        public MainPage() {
            this.InitializeComponent();

            //new back
            SystemNavigationManager.GetForCurrentView().BackRequested += (s, e) => {
                if (this.VisualStateGroup.CurrentState.Name == "narrowThread") {
                    VisualStateManager.GoToState(this, "narrowMessage", true);
                    SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Collapsed;
                    MessageList.SelectedIndex = -1;
                    e.Handled = true;
                }
            };
        }

        protected override void OnNavigatedTo(NavigationEventArgs e) {
            base.OnNavigatedTo(e);

            RefreshVisualState();
            string category = (e.Parameter == null || (string)e.Parameter == "") ? "top" : (string)e.Parameter;
            RefreshMessages(category);
        }

        private async void RefreshMessages(string category) {
            var client = new HttpClient();
            var raw = await client.GetStringAsync(new Uri("https://hacker-news.firebaseio.com/v0/" + category + "stories.json"));

            List<int> topids = JsonConvert.DeserializeObject<List<int>>(raw);
            var topitems = new List<HNItem>();
            while (topitems.Count < 20) {
                topitems.Add(await HNItem.fromID(topids[topitems.Count], client));
            }

            MessageList.ItemsSource = topitems;
        }

        private void MessageList_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            if (((ListBox)sender).SelectedItem != null) {
                var item = (HNItem)((ListBox)sender).SelectedItem;

                if (VisualStateGroup.CurrentState.Name == "narrowMessage") {
                    VisualStateManager.GoToState(this, "narrowThread", true);
                    SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Visible;
                } else {
                    
                }
                ThreadFrame.Navigate(typeof(ThreadView), item.id);
            }
        }

        private void Page_SizeChanged(object sender, SizeChangedEventArgs e) {
            RefreshVisualState();
        }

        private void RefreshVisualState() {
            String state = "wideView";

            if (Window.Current.Bounds.Width < 500) {
                if (MessageList.SelectedItem != null) {
                    state = "narrowThread";
                    SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Visible;
                }
                else {
                    state = "narrowMessage";
                    SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Collapsed;
                }
            }
            else {
                SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Collapsed;
            }

            VisualStateManager.GoToState(this, state, true);
        }
    }
}
