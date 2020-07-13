using PairReader;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace DeckBuilder
{
    internal class RankedCard : IComparable<RankedCard>
    {
        internal static List<RankedCard> rankedCards = new List<RankedCard>();
        internal static void AddCard(string name, double score)
        {
            score -= .5;
            RankedCard card = rankedCards.Find(x => x.name == name);
            if (card == null)
            {
                rankedCards.Add(new RankedCard(name, score));
            }
            else
            {
                card.rank += score;
            }
        }

        public int CompareTo(RankedCard other)
        {
            return -1 * rank.CompareTo(other.rank);
        }

        public RankedCard(string name, double rank)
        {
            this.name = name;
            this.rank = rank;
        }

        internal readonly string name;
        double rank;

        internal static void Save(string filename = "")
        {
            rankedCards.Sort();
            double score = 0;
            StringBuilder sb = new StringBuilder();
            foreach(RankedCard card in rankedCards)
            {
                sb.AppendLine(card.name + ": " + card.rank);
                score += card.rank;
            }
            string saveFile = Path.GetDirectoryName(CardPair.saveFile) +
                "\\" + filename + "_" + score.ToString() + ".txt";
            File.WriteAllText(saveFile, sb.ToString().Trim());
        }
    }
}