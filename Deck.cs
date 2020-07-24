using PairReader;
using System.Collections.Generic;
using System.Linq;

namespace DeckBuilder
{
    class Deck
    {
        internal List<string> cards = new List<string>();
        internal HeroClass hero;
        internal NoDuplicatesRestriction ndr = new NoDuplicatesRestriction();
        internal NoMinionsRestriction nmr = new NoMinionsRestriction();
        internal NoNeutralsRestriction nnr = new NoNeutralsRestriction();
        bool allowNDR;
        bool allowNMR;
        bool allowNNR;
        public Deck(HeroClass hero, bool allowNDR, bool allowNMR, bool allowNNR)
        {
            this.hero = hero;
            this.allowNDR = allowNDR;
            this.allowNMR = allowNMR;
            this.allowNNR = allowNNR;
        }
        internal bool AddCard(string card)
        {
            int cardCount = cards.Count(x => x == card);
            if (cardCount == 2)
            {
                return false;
            }
            if (cardCount == 1)
            {
                if (!(Cards.FindByNameNotHeroCollectible(card).rarity == "LEGENDARY") && !ndr.IsDeckRestricted(cards))
                {
                    cards.Add(card);
                    return true;
                }
                else
                {
                    return false;
                }
            }
            if (nmr.IsDeckRestricted(cards) && nmr.IsCardRestricted(card, cards))
            {
                return false;
            }
            if (nnr.IsDeckRestricted(cards) && nnr.IsCardRestricted(card, cards))
            {
                return false;
            }
            if (ndr.Cards.Contains(card))
            {
                if (allowNDR)
                {
                    ndr.RemoveOffenders(cards);
                }
                else 
                { 
                    return false; 
                }
            }
            if (nnr.Cards.Contains(card))
            {
                if (allowNNR)
                {
                    nnr.RemoveOffenders(cards);
                }
                else
                {
                    return false;
                }
            }
            if (nmr.Cards.Contains(card))
            {
                if (allowNMR)
                {
                    nmr.RemoveOffenders(cards);
                }
                else
                {
                    return false;
                }
            }
            cards.Add(card);
            return true;
        }
    }
}
