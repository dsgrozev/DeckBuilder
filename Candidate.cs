using System;

namespace DeckBuilder
{
    internal class Candidate : IComparable<Candidate>
    {
        internal string card;
        internal double coef;

        public Candidate(string card, double coef)
        {
            this.card = card;
            this.coef = coef;
        }

        public int CompareTo(Candidate other)
        {
            return -1 * coef.CompareTo(other.coef);
        }
    }
}