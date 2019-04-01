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
            var builder = new AdaptiveCardBuilder();
            builder.AddJob(new AdaptiveCardBuilder.PrintJob("Document 1", DateTime.Now, "Printing", "Joe Smith", 10, DateTime.Now));
            string card = builder.ToString();
            _ShowAdaptiveCard(card);
        }

        private void Button2_Click(object sender, RoutedEventArgs e)
        {
            string card = Util.ReadTextAsset("pjac_adaptivecards.json");
            _ShowAdaptiveCard(card);
        }
    }
}
