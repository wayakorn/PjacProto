using System;
using System.Collections.ObjectModel;

namespace PjacProto
{
    public class Model
    {
        public ObservableCollection<string> PrinterList { get; } = new ObservableCollection<string>();
        public AdaptiveCardBuilder AdaptiveCard { get; } = new AdaptiveCardBuilder();

        private DateTime Current;

        public void Reset()
        {
            Current = DateTime.Parse("2019-04-01T11:11:11Z");

            AdaptiveCard.Clear();

            PrinterList.Clear();
            PrinterList.Add("Microsoft Print to PDF");
            PrinterList.Add("HP Envy 8888");
        }

        public void AddRandomPrintJob()
        {
            var rnd = new Random((int)(Current.Ticks + DateTime.Now.Ticks));
            int dateTimeDelta = 1 + (rnd.Next() % 10);
            Current = Current.AddMinutes(dateTimeDelta);

            string iconUrl = IconUrls[(rnd.Next() % IconUrls.Length)];
            string docName = DocumentNames[(rnd.Next() % DocumentNames.Length)];
            string owner = Owners[(rnd.Next() % Owners.Length)];
            int pages = dateTimeDelta;

            AdaptiveCard.AddJob(new AdaptiveCardBuilder.PrintJob(docName, Current, AdaptiveCardBuilder.PrintJob.JobStatus.Printing, iconUrl, owner, pages, Current));
        }

        private readonly string[] DocumentNames = new string[]
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

        private readonly string[] IconUrls = new string[]
        {
            "http://icons.iconarchive.com/icons/dtafalonso/modern-xp/512/ModernXP-31-Filetype-Word-icon.png",
            "https://cdn.windowsfileviewer.com/images/types/xlsx.png",
            "https://www.plmworld.org/media/hmvmbwgr.png",
            "https://cdn2.iconfinder.com/data/icons/picons-basic-1/57/basic1-029_url_web_address-512.png"
        };

        private readonly string[] Owners = new string[]
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


    }
}
