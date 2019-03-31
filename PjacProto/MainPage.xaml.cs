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

using AdaptiveCards.Rendering.Uwp;
using Windows.Storage;
using System.Runtime.Serialization.Json;
using Windows.Data.Json;
using System.Text;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace PjacProto
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
        }

        string _GetAdaptiveCardJson()
        {
            string result = AdaptiveCardTemplate.TemplateBegin;
            result += AdaptiveCardTemplate.GetJobCard("Document 1", DateTime.Now, "Printing", "Joe Smith", 10, DateTime.Now);
///            result += AdaptiveCardTemplate.GetJobCard("Document 2", DateTime.Now, "Printing", "Way", 2, DateTime.Now);
            result += AdaptiveCardTemplate.TemplateEnd;
            return result;
        }

        void _ShowAdaptiveCard(string cardJson)
        {
            var hostConfig = new AdaptiveHostConfig()
            {
                FontSizes = {
                    Small = 15,
                    Default = 20,
                    Medium = 25,
                    Large = 30,
                    ExtraLarge= 40
                }
            };

            var renderer = new AdaptiveCardRenderer
            {
                HostConfig = hostConfig
            };

            var parseResult = AdaptiveCard.FromJsonString(cardJson);
            RenderedAdaptiveCard renderedAdaptiveCard = renderer.RenderAdaptiveCard(parseResult.AdaptiveCard);

            // Check if the render was successful
            if (renderedAdaptiveCard.FrameworkElement != null)
            {
                // Get the framework element
                var uiCard = renderedAdaptiveCard.FrameworkElement;

                // Add it to your UI
                myGrid.Children.Add(uiCard);
            }
        }

        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            myGrid.Children.Clear();
        }

        private void Button1_Click(object sender, RoutedEventArgs e)
        {
            _ShowAdaptiveCard(_GetAdaptiveCardJson());
        }

        private void Button2_Click(object sender, RoutedEventArgs e)
        {
            Uri appUri = new Uri("ms-appx:///Assets/pjac_adaptivecards.json"); //File name should be prefixed with 'ms-appx:///Assets/* 
            StorageFile anjFile = StorageFile.GetFileFromApplicationUriAsync(appUri).AsTask().ConfigureAwait(false).GetAwaiter().GetResult();
            string json = FileIO.ReadTextAsync(anjFile).AsTask().ConfigureAwait(false).GetAwaiter().GetResult();

            _ShowAdaptiveCard(json);
        }
    }
}
