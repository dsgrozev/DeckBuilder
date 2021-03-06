﻿using System;
using System.Collections.Generic;
using System.Text;

namespace DeckBuilder
{
    using PairReader;
    using System.IO;

    class Program
    {
        static void Main(string[] args)
        {
            CardPair.LoadPairs(CardPair.saveFile, CardPair.CardPairs);
            CardPair.LoadPairs(CardPair.fullSaveFile, CardPair.FullCardPairs);
            Cards.Load();
            foreach (HeroClass hero in Enum.GetValues(typeof(HeroClass)))
            {
                if (hero == HeroClass.NONE)
                {
                    continue;
                }
                List<CardPair> classPairs = CardPair.CardPairs.FindAll(x => x.Hero == hero);
                SaveHero(hero, classPairs, false);
                classPairs.Clear();
                classPairs = CardPair.FullCardPairs.FindAll(x => x.Hero == hero);
                SaveHero(hero, classPairs, true);
            }
        }

        private static void SaveHero(HeroClass hero, List<CardPair> classPairs, bool isFull)
        {
            classPairs.Sort();
            //int minCount = classPairs[0].Count();
            //classPairs.RemoveAll(x => x.Count() < minCount);
            RankedCard.rankedCards.Clear();
            foreach (CardPair pair in classPairs)
            {
                RankedCard.AddCard(pair.Card2, pair.DeckWinPercentage());
                RankedCard.AddCard(pair.Card1, pair.DeckWinPercentage());
            }
            RankedCard.rankedCards.Sort();
            bool allowNDR = true;
            bool allowNMR = true;
            bool allowNNR = true;
            bool oneMoreDeck = true;
            while (oneMoreDeck)
            {
                Deck deck = new Deck(hero, allowNDR, allowNMR, allowNNR);
                int i = 0;
                while (deck.cards.Count == 0)
                {
                    deck.AddCard(RankedCard.rankedCards[i++].name);
                }
                bool noMoreCards = false;
                while (deck.cards.Count < 40 && !noMoreCards)
                {
                    List<Candidate> candidates = new List<Candidate>();
                    foreach (RankedCard card in RankedCard.rankedCards)
                    {
                        candidates.Add(new Candidate(card.name, Calculate(card.name, classPairs, deck.cards)));
                    }
                    candidates.Sort();
                    int j = 0;
                    bool added;
                    do
                    {
                        added = deck.AddCard(candidates[j++].card);
                    } while (!added && j < candidates.Count);
                    if (j == candidates.Count)
                    {
                        noMoreCards = true;
                    }
                }
                oneMoreDeck = false;
                string restrictions = "";
                if (deck.ndr.IsDeckRestricted(deck.cards))
                {
                    allowNDR = false;
                    oneMoreDeck = true;
                    restrictions += "_NDR";
                }
                if (deck.nmr.IsDeckRestricted(deck.cards))
                {
                    allowNMR = false;
                    oneMoreDeck = true;
                    restrictions += "_NMR";
                }
                if (deck.nnr.IsDeckRestricted(deck.cards))
                {
                    allowNNR = false;
                    oneMoreDeck = true;
                    restrictions += "_NNR";
                }
                string full = "";
                if (isFull)
                {
                    full = "_FULL";
                }
                SaveDeck(deck.cards, hero.ToString() + full + "_" + deck.cards[0] + restrictions, classPairs);
            }
        }

        private static void SaveDeck(List<string> deck, string hero, List<CardPair> cardPairs)
        {
            StringBuilder sb = new StringBuilder();
            int i = 0;
            double deckScore = 0;
            foreach (string card in deck)
            {
                if (i == 30)
                {
                    sb.AppendLine("--------");
                }
                double cardScore = Calculate(card, cardPairs, deck);
                if (i < 31)
                {
                    deckScore += cardScore;
                }
                sb.AppendLine(card + ", " + cardScore);
                i++;
            }
            string saveFile = Path.GetDirectoryName(CardPair.saveFile) +
                "\\" +
                string.Format("{0:0.00}", deckScore) + 
                "_" +
                hero +
                ".txt";
            File.WriteAllText(saveFile, sb.ToString().Trim());
        }

        private static double Calculate(string card, List<CardPair> classPairs, List<string> deck)
        {
            List<CardPair> interestingPairs = classPairs.FindAll(x => x.Card1 == card || x.Card2 == card);
            double ret = 0.0;
            int i = 0;
            foreach (string deckCard in deck)
            {
                CardPair pair = interestingPairs.Find(x => x.Card1 == deckCard || x.Card2 == deckCard);
                if (pair != null)
                {
                    ret += pair.DeckWinPercentage() - .5;
                }
                if (++i == 31)
                {
                    break;
                }
            }
            return ret;
        }
    }
}

