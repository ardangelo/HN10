using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Windows.Foundation.Metadata;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace HN10.Views
{
    public sealed partial class Shell : Page
    {
        private Frame contentFrame;
        public static CommandBar cb;

        public Shell(Frame frame)
        {
            Windows.UI.ViewManagement.ApplicationView.GetForCurrentView().SetPreferredMinSize(
                new Windows.Foundation.Size { Width = 320, Height = 320 }); //set this in appxmanifest when it b ecomes an option
           
            this.contentFrame = frame;
            this.InitializeComponent();
            cb = this.CommandBar;
            
            this.ShellSplitView.Content = frame;
            var update = new Action(() =>
            {
                this.ShellSplitView.IsPaneOpen = false;
            });
            frame.Navigated += (s, e) => update();
            
            this.Loaded += (s, e) => update();
            this.DataContext = this;
        }

        // menu
        Mvvm.Command _menuCommand;
        public Mvvm.Command MenuCommand { get { return _menuCommand ?? (_menuCommand = new Mvvm.Command(ExecuteMenu)); } }
        private void ExecuteMenu()
        {
            this.ShellSplitView.IsPaneOpen = !this.ShellSplitView.IsPaneOpen;
        }


        // nav
        Mvvm.Command<NavType> _navCommand;

        public Mvvm.Command<NavType> NavCommand { get { return _navCommand ?? (_navCommand = new Mvvm.Command<NavType>(ExecuteNav)); } }
        private void ExecuteNav(NavType navType)
        {
            var type = navType.Type;

            this.contentFrame.Navigate(navType.Type, navType.Parameter);
            // when we nav home, clear history
            if (type.Equals(typeof(Views.MainPage)))
            {
                this.contentFrame.BackStack.Clear();
            }
        }

        // utility
        public List<RadioButton> AllRadioButtons(DependencyObject parent)
        {
            var list = new List<RadioButton>();
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                if (child is RadioButton)
                {
                    list.Add(child as RadioButton);
                    continue;
                }
                list.AddRange(AllRadioButtons(child));
            }
            return list;
        }

        // prevent check
        private void DontCheck(object s, RoutedEventArgs e)
        {
            // don't let the radiobutton check
            (s as RadioButton).IsChecked = false;
        }

        private void ReplyButton_Click(object sender, RoutedEventArgs e) {
            int id = (int)((AppBarButton)sender).Content;
            var nav = new NavType();
            nav.Type = typeof(WebView);
            nav.Parameter = "https://news.ycombinator.com/reply?id=" + id + "&goto=item%3Fid%3D" + id;
            ExecuteNav(nav);
        }

        private void ShareButton_Click(object sender, RoutedEventArgs e) {
            int id = (int)((AppBarButton)sender).Content;
        }
    }

    public class NavType
    {
        public Type Type { get; set; }
        public string Parameter { get; set; }
    }
}
