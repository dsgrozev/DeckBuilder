using PairReader;
using System.Collections.Generic;

namespace DeckBuilder
{
    class NoDuplicatesRestriction : IDeckRestriction
    {
        public List<string> Cards = new List<string>();
        public List<HeroClass> HeroClasses = new List<HeroClass>();

        public bool IsDeckRestricted(List<string> deck)
        {
            foreach(string card in Cards)
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
                if (!deck.Contains(card))
                {
                    deck.Add(card);
                }
            }
        }

        public bool IsCardRestricted(string card, List<string> deck)
        {
            return !deck.Contains(card);
        }

        public NoDuplicatesRestriction()
        {
            Cards.Add("Elise the Enlightened");
            Cards.Add("Dinotamer Brann");
            Cards.Add("Reno the Relicologist");
            Cards.Add("Sir Finley of the Sands");
            Cards.Add("Zephrys the Great");
            Cards.Add("Dragonqueen Alexstrasza");
            HeroClasses.Add(HeroClass.NONE);
        }
    }
}
