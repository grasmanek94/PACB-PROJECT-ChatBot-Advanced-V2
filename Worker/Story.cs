using System.Windows.Forms;
using Gecko;
using Gecko.DOM;
using Gecko.Collections;
using System.Diagnostics;
using System.Net;
using System;

namespace Worker
{

    public class Story
    {
        private int CurrentStoryMessage { get; set; }
        private Stopwatch story_stopwatch;
        private string[] story_mode =
        {
            "Ik ben een robot en ik ben hier om jou te vervelen. Ik heb wel een mooi verhaaltje voor je... Hou je het vol tot 't einde? Opmerkingen? Vragen? mail: het-gekke-robotje@0dl.nl / whatsapp: 06 86 366 906 / www.facebook.com/rt9x9",
            "Een robot in Zuid Beierland",
            "Die was aan lager wal beland",
            "En in café De Malle Moer",
            "Vielen zijn tranen op de vloer",
            "-----",
            "Want zijn vriendin, (afwasmachine,",
            "die werkzaam was in een kantine),",
            "Had de verloving uitgemaakt",
            "Daardoor was hij van streek geraakt",
            "-----",
            "De kroegbaas zei: \"Zo zijn de vrouwen,",
            "Ze zijn gewoon niet te vertrouwen\",",
            "Waarop de robot nog eens snikte",
            "En daarbij ook instemmend knikte",
            "-----",
            "Hij zei: \"Laatst leek ze 't te begeven",
            "Toen heb ik haar een beurt gegeven",
            "Ze heeft, na te zijn opgeknapt,",
            "Snel met een ander aangepapt",
            "-----",
            "Het is, geloof ik, een gokmachine",
            "Die, naar men zegt, veel zou verdienen",
            "Ik heb haar uit de goot gehaald",
            "Maar wordt met ondank terugbetaald!\"",
            "-----",
            "\"Drink nog een glaasje\", zei de waard",
            "Die meid is jouw verdriet niet waard,",
            "'t is beter dat gehuil te staken",
            "Voordat je vastgeroest gaat raken\"",
            "-----",
            "De robot zei: \"Ik zal haar mailen:",
            "\"Je kan me echt geen moer meer schelen!\",",
            "En geef, voor mijn melancholie,",
            "Me maar een dubbele smeerolie\"",
            "-----",
            "Einde.",
            "Er zit hier geen mens. Dus er word ook niet gereageerd. (behalve op hoi)",
            "Deze chatbot wordt mogelijk gemaakt door een irritant maar lief 21 jarig jongetje ;P",
            "Nog een fijne dag en veel succes verder!"
        };

        public Story(string filename)
        {
            story_stopwatch = new Stopwatch();
            CurrentStoryMessage = 0;

            try
            {
                story_mode = System.IO.File.ReadAllLines(filename);
            }
            catch { }
        }

        public string GetMessage()
        {
            if (story_mode == null || story_mode.Length <= CurrentStoryMessage)
            {
                return string.Empty;
            }

            if (CurrentStoryMessage >= 0 && CurrentStoryMessage < story_mode.Length && (story_stopwatch.ElapsedMilliseconds > 2750 || CurrentStoryMessage == 0))
            {
                story_stopwatch.Restart();
                return story_mode[CurrentStoryMessage++];
            }
            return string.Empty;
        }

        public void Stop()
        {
            CurrentStoryMessage = -1;
            story_stopwatch.Stop();
        }

        public void Restart()
        {
            CurrentStoryMessage = 0;
            story_stopwatch.Restart();
        }
    }
}
