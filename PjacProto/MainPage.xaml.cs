using System;
using System.Diagnostics;
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

using Windows.UI.ViewManagement;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace PjacProto
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private readonly Model m_model = new Model();
        private int m_pjacViewId = 0;
        private PjacPage m_pjacPage;

        public MainPage()
        {
            this.InitializeComponent();
        }

        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            m_model.Reset();
        }

        private void Button1_Click(object sender, RoutedEventArgs e)
        {
            m_model.AddRandomPrintJob();
        }

        private async void Button2_Click(object sender, RoutedEventArgs e)
        {
            if (m_pjacPage == null)
            {
                CoreApplicationView newView = CoreApplication.CreateNewView();
                await newView.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    Frame frame = new Frame();
                    bool navSucceeded = frame.Navigate(typeof(PjacPage));
                    Debug.Assert(navSucceeded);

                    m_pjacPage = (PjacPage)frame.Content;
                    m_pjacPage.Initialize(m_model);
                    m_pjacPage.Unloaded += M_pjacPage_Unloaded;

                    Window.Current.Content = frame;
                    // You have to activate the window in order to show it later.
                    Window.Current.Activate();

                    m_pjacViewId = ApplicationView.GetForCurrentView().Id;
                });
            }
            else
            {
                m_pjacPage.Refresh();
            }

            Debug.Assert(m_pjacViewId != 0);
            bool viewShown = await ApplicationViewSwitcher.TryShowAsStandaloneAsync(m_pjacViewId);
            Debug.Assert(viewShown);
        }

        private void M_pjacPage_Unloaded(object sender, RoutedEventArgs e)
        {
            m_pjacPage = null;
        }
    }
}
