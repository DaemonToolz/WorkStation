using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Workstation.Model;
using WorkstationUWP.ViewModel;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace WorkstationUWP
{

    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class WorkstationFrame : Page
    {
        public UsersModel ThisUser { get; set; } = new UsersModel(){ username = "", profilepic = "Assets/Anonymous.jpg"};
        public WorkstationFrame() {
            this.InitializeComponent();
            ProfileSection.DataContext = ThisUser;
            //((Grid)FindChildByName(this, "ProfileGrid")).DataContext = ThisUser;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            var entity = e.Parameter as UsersModel;
            ThisUser = entity;
            //((TextBlock)FindChildByName(ProfileSectionDtemplate, "Rank")).Text = (ThisUser.rank);
            //ThisUser.profilepic = "Assets/" + ThisUser.profilepic;
        }

        public static DependencyObject FindChildByName(DependencyObject from, string name)
        {
         
            for (var i = 0; i < VisualTreeHelper.GetChildrenCount(from); i++)
            {
                var child = VisualTreeHelper.GetChild(from, i);
                if (child is FrameworkElement && ((FrameworkElement)child).Name == name)
                    return child;

                DependencyObject result;
                if ((result = FindChildByName(child, name)) != null)
                    return result;
            }

            return null;
        }
    }
}
