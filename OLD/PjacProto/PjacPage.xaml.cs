using System.Diagnostics;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

using AdaptiveCards.Rendering.Uwp;

namespace PjacProto
{
    public sealed partial class PjacPage : Page
    {
        private Model m_model;

        public PjacPage()
        {
            this.InitializeComponent();
        }

        public void Initialize(Model model)
        {
            m_model = model;

            // TODO - use dataBinding instead of manually populating the page
            Refresh();
        }

        public void Refresh()
        {
            Debug.Assert(m_model != null);
            string card = m_model.AdaptiveCard.ToString();
            _AddCardIntoFrame(card, this.Frame);
        }

        private void _AddCardIntoFrame(string cardJson, Frame frame)
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
                var uiCard = renderedAdaptiveCard.FrameworkElement;
                frame.Content = uiCard;
            }
        }

    }
}
