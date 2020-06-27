﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeckBuilder
{
    using PairReader;
    using System.IO;
    using System.Runtime.InteropServices;

    class Program
    {
        static void Main(string[] args)
        {
            CardPair.LoadPairs();
            Cards.Load();
            foreach (HeroClass hero in Enum.GetValues(typeof(HeroClass)))
            {
                if (hero == HeroClass.NONE)
                {
                    continue;
                }
                bool isHighlander = false;
                List<CardPair> classPairs = CardPair.CardPairs.FindAll(x => x.Hero == hero);
                classPairs.Sort();
                int minCount = classPairs[0].Count();
                classPairs.RemoveAll(x => x.Count() < minCount);
                double topPercent = classPairs[0].DeckWinPercentage();
                List<String> deck = new List<String>();
                AddTopCards(classPairs, deck, minCount, topPercent, ref isHighlander);
                while (deck.Count < 30)
                {
                    AddNextCard(classPairs, deck, ref isHighlander);
                }
                SaveDeck(deck, hero);
            }
        }

        private static void SaveDeck(List<string> deck, HeroClass hero)
        {
            StringBuilder sb = new StringBuilder();
            foreach(string card in deck)
            {
                sb.AppendLine(card);
            }
            string saveFile = Path.GetDirectoryName(CardPair.saveFile) + "\\" + hero.ToString() + ".txt";
            File.WriteAllText(saveFile, sb.ToString().Trim());
        }

        private static void AddNextCard(List<CardPair> classPairs, List<string> deck, ref bool isHighlander)
        {
            List<Candidate> candidates = new List<Candidate>();
            foreach(CardPair pair in classPairs)
            {
                if ((deck.Contains(pair.Card1) && deck.Contains(pair.Card2)) ||
                    (!deck.Contains(pair.Card1) && !deck.Contains(pair.Card2)))
                {
                    continue;
                }
                Candidate candidate = new Candidate(pair.Card1, 0.0);
                if (deck.Contains(pair.Card1))
                {
                    candidate.card = pair.Card2;
                }
                candidate.coef = Calculate(candidate.card, classPairs, deck, isHighlander);
                candidates.Add(candidate);
            }
            if (candidates.Count == 0)
            {
                classPairs.Sort();
                AddCard(classPairs[0].Card1, deck, ref isHighlander);
                AddCard(classPairs[0].Card2, deck, ref isHighlander);
                return;
            }
            candidates.Sort();
            AddCard(candidates[0].card, deck, ref isHighlander);
        }

        private static double Calculate(string card, List<CardPair> classPairs, List<string> deck, bool isHighlander)
        {
            List<CardPair> interestingPairs = classPairs.FindAll(x => x.Card1 == card || x.Card2 == card);
            int count = 2;
            if (deck.Count == 29 || isHighlander || Cards.FindByNameCollectible(card).rarity == "LEGENDARY")
            {
                count = 1;
            }
            double ret = 0.0;
            foreach(string deckCard in deck)
            {
                CardPair pair = interestingPairs.Find(x => x.Card1 == deckCard || x.Card2 == deckCard);
                if (pair != null)
                {
                    ret += pair.DeckWinPercentage() * count;
                }
            }
            return ret;
        }

        private static void AddTopCards(List<CardPair> classPairs, List<string> deck, int minCount, double topPercent, ref bool isHighlander)
        {
            int i = 0;
            while (classPairs[i].DeckWinPercentage() == topPercent && classPairs[i].Count() == minCount)
            {
                AddCard(classPairs[i].Card1, deck, ref isHighlander);
                AddCard(classPairs[i].Card2, deck, ref isHighlander);
                i++;
            }
        }

        private static void AddCard(string card, List<string> deck, ref bool isHighlander)
        {
            if (deck.Contains(card))
            {
                return;
            }
            if (isHighlander)
            {
                deck.Add(card);
                return;
            }
            Card realCard = Cards.FindByNameCollectible(card);
            if (realCard.text != null && realCard.text.Contains("no duplicates"))
            {
                isHighlander = true;
                RemoveDuplicates(deck);
                deck.Add(card);
                return;
            }
            if (realCard.rarity == "LEGENDARY" || deck.Count() == 29)
            {
                deck.Add(card);
                return;
            }
            deck.Add(card);
            deck.Add(card);
        }

        private static void RemoveDuplicates(List<string> deck)
        {
            List<string> deck1 = new List<string>(deck);
            deck.Clear();
            foreach(string card in deck1)
            {
                if (!deck.Contains(card))
                {
                    deck.Add(card);
                }
            }
        }
    }
}