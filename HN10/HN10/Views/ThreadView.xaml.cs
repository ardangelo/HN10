using HN10.API;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.Web.Http;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace HN10.Views {
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ThreadView : Page {
        class ThreadItem {
            public Thickness margin { get; set; }
            public SolidColorBrush color { get; set; }
            public HNItem item { get; set; }
            public ThreadItem(int level, HNItem item) {
                this.margin = new Thickness(level * 30, 0, 0, 0);
                this.item = item;
                if (item.dead || item.deleted) {
                    color = new SolidColorBrush(Windows.UI.Colors.Gray);
                } else {
                    color = new SolidColorBrush(Windows.UI.Colors.White); //change based on theme
                }
            }
        }

        int id;
        HNItem root;

        public ThreadView() {
            this.InitializeComponent();
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e) {
            base.OnNavigatedTo(e);

            this.id = (int)e.Parameter;
            this.root = await HNItem.fromID(id);
            this.ScoreText.Text = root.score.ToString();
            this.TitleText.Text = root.title;

            RefreshThread(id);
        }

        private async void RefreshThread(int id) {
            var client = new HttpClient();
            var thread = new List<ThreadItem>();

            if (this.root.text != string.Empty) {
                thread.Add(new ThreadItem(0, this.root));
            }
            ThreadList.ItemsSource = await buildThread(thread, 0, this.root.kids, client);
        }

        private async Task<List<ThreadItem>> buildThread(List<ThreadItem> thread, int level, List<int> nodes, HttpClient client) {
            if (nodes != null) {
                foreach (int kidid in nodes) {
                    var kid = await HNItem.fromID(kidid);
                    thread.Add(new ThreadItem(level, kid));

                    await buildThread(thread, level + 1, kid.kids, client);
                }
            }

            return thread;
        }

        private async void LinkButton_Click(object sender, RoutedEventArgs e) {
            var uri = new Uri(this.root.url);
            await Windows.System.Launcher.LaunchUriAsync(uri);
        }

        private void ThreadList_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            if (ThreadList.SelectedItems.Count > 1) {
                ThreadList.SelectedItem = ThreadList.SelectedItems[ThreadList.SelectedItems.Count - 1];
            }

            if (ThreadList.SelectedItems.Count == 0) {
                Shell.cb.ClosedDisplayMode = AppBarClosedDisplayMode.Hidden;
            } else {
                Shell.cb.ClosedDisplayMode = AppBarClosedDisplayMode.Compact;
                foreach (AppBarButton abb in Shell.cb.PrimaryCommands) {
                    abb.Content = ((List<ThreadItem>)ThreadList.ItemsSource)[ThreadList.SelectedIndex].item.id; //terrible terrible please no
                }
            }
        }
    }
}
