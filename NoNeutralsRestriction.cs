using PairReader;
using System.Collections.Generic;

namespace DeckBuilder
{
    class NoNeutralsRestriction : IDeckRestriction
    {
        public List<string> Cards = new List<string>();
        public List<HeroClass> HeroClasses = new List<HeroClass>();

        public bool IsDeckRestricted(List<string> deck)
        {
            foreach (string card in Cards)
            {
                if (deck.Contains(card))
                {
                    return true;
                }
            }
            return false;
        }

        public void RemoveOffenders(List<string> deck)
        {
            List<string> temp = new List<string>(deck);
            deck.Clear();
            foreach(string card in temp)
            {
                if (PairReader.Cards.FindByName(card).cardClass != "NEUTRAL")
                {
                    deck.Add(card);
                }
            }
        }

        public bool IsCardRestricted(string card, List<string> deck)
        {
            return PairReader.Cards.FindByName(card).cardClass == "NEUTRAL";
        }

        public NoNeutralsRestriction()
        {
            Cards.Add("Lightforged Crusader");
            Cards.Add("Lightforged Zealot");
            HeroClasses.Add(HeroClass.PALADIN);
        }
    }
}
