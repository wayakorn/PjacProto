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

        readonly string[] DocumentNames = new string[]
        {
            "Back to the Future",
            "Desperado",
            "Night at the Museum",
            "Robocop",
            "Ghostbusters",
            "Cool World",
            "Donnie Darko",
            "Double Indemnity",
            "The Spanish Prisoner",
            "The Smurfs",
            "Dead Alive",
            "Army of Darkness",
            "Peter Pan",
            "The Jungle Story",
            "Red Planet",
            "Deep Impact",
            "The Long Kiss Goodnight",
            "Juno",
            "(500) Days of Summer",
            "The Dark Knight",
            "Bringing Down the House",
            "Se7en",
            "Chocolat",
            "The American",
            "The American President",
            "Hudsucker Proxy",
            "Conan the Barbarian",
            "Shrek",
            "The Fox and the Hound",
            "Lock, Stock, and Two Barrels",
            "Date Night",
            "200 Cigarettes",
            "9 1/2 Weeks",
            "Iron Man 2",
            "Tombstone",
            "Young Guns",
            "Fight Club",
            "The Cell",
            "The Unborn",
            "Black Christmas",
            "The Change-Up",
            "The Last of the Mohicans",
            "Shutter Island",
            "Ronin",
            "Ocean’s 11",
            "Philadelphia",
            "Chariots of Fire",
            "M*A*S*H",
            "Walking and Talking",
            "Walking Tall",
            "The 40 Year Old Virgin",
            "Superman III",
            "The Hour",
            "The Slums of Beverly Hills",
            "Secretary",
            "Secretariat",
            "Pretty Woman",
            "Sleepless in Seattle",
            "The Iron Mask",
            "Smoke",
            "Schindler’s List",
            "The Beverly Hillbillies",
            "The Ugly Truth",
            "Bounty Hunter",
            "Say Anything",
            "8 Seconds",
            "Metropolis",
            "Indiana Jones and the Temple of Doom",
            "Kramer vs. Kramer",
            "The Manchurian Candidate",
            "aging Bull",
            "Heat",
            "About Schmidt",
            "Re-Animator",
            "Evolution",
            "Gone in 60 Seconds",
            "Wanted",
            "The Man with One Red Shoe",
            "The Jerk",
            "Whip It",
            "Spanking the Monkey",
            "Steel Magnolias",
            "Horton Hears a Who",
            "Honey",
            "Brazil",
            "Gorillas in the Mist",
            "Before Sunset",
            "After Dark",
            "From Dusk til Dawn",
            "Cloudy with a Chance of Meatballs",
            "Harvey",
            "Mr. Smith Goes to Washington",
            "L.A. Confidential",
            "Little Miss Sunshine",
            "The Future",
            "Howard the Duck",
            "Howard’s End",
            "The Innkeeper",
            "Revolutionary Road"
        };

        readonly string[] IconUrls = new string[]
        {
            "http://icons.iconarchive.com/icons/dtafalonso/modern-xp/512/ModernXP-31-Filetype-Word-icon.png",
            "https://cdn.windowsfileviewer.com/images/types/xlsx.png",
            "https://www.plmworld.org/media/hmvmbwgr.png",
            "https://cdn2.iconfinder.com/data/icons/picons-basic-1/57/basic1-029_url_web_address-512.png"
        };

        readonly string[] Owners = new string[]
        {
            "Dan Altavilla",
            "Shawn Armstrong",
            "Gerson Bautista",
            "Tim Beckham",
            "Braden Bishop",
            "Chasen Bradford",
            "Jay Bruce",
            "Roenis Elías",
            "Edwin Encarnación",
            "Matt Festa",
            "David Freitas",
            "Cory Gearrin",
            "Marco Gonzales",
            "Dee Gordon",
            "Mitch Haniger",
            "Ryon Healy",
            "Félix Hernández",
            "Mike Leake",
            "Wade LeBlanc",
            "Tom Murphy",
            "Omar Narváez",
            "Zac Rosscup",
            "Nick Rumbelow",
            "Domingo Santana",
            "Kyle Seager",
            "Mallex Smith",
            "Hunter Strickland",
            "Ichiro Suzuki",
            "Anthony Swarzak",
            "Sam Tuivailala",
            "Daniel Vogelbach"
        };

        private DateTime AddRandomPrintJob(AdaptiveCardBuilder builder, DateTime current, AdaptiveCardBuilder.PrintJob.JobStatus status)
        {
            var rnd = new Random((int)(current.Ticks + DateTime.Now.Ticks));
            int dateTimeDelta = 1 + (rnd.Next() % 10);
            DateTime newDateTime = current.AddMinutes(dateTimeDelta);

            string iconUrl = IconUrls[(rnd.Next() % IconUrls.Length)];
            string docName = DocumentNames[(rnd.Next() % DocumentNames.Length)];
            string owner = Owners[(rnd.Next() % Owners.Length)];
            int pages = dateTimeDelta;

            builder.AddJob(new AdaptiveCardBuilder.PrintJob(docName, newDateTime, status, iconUrl, owner, pages, newDateTime));
            return newDateTime;
        }

        private void Button1_Click(object sender, RoutedEventArgs e)
        {
            DateTime current = DateTime.Parse("2019-04-01T11:11:11Z");
            var builder = new AdaptiveCardBuilder();
            for (int i = 0; i < 10; ++i)
            {
                current = AddRandomPrintJob(builder, current, AdaptiveCardBuilder.PrintJob.JobStatus.Printed);
            }
            AddRandomPrintJob(builder, current, AdaptiveCardBuilder.PrintJob.JobStatus.Printing);
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
