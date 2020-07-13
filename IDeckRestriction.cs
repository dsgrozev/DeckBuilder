using PairReader;
using System.Collections.Generic;

namespace DeckBuilder
{
    interface IDeckRestriction
    {
        bool IsDeckRestricted(List<string> deck);
        void RemoveOffenders(List<string> deck);
        bool IsCardRestricted(string card, List<string> deck);
    }
}
