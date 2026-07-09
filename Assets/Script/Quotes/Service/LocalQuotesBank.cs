using UnityEngine;
using Quotes.Data;

/// <summary>
/// Provides a local fallback bank of technology-related quotes.
/// Used when the API is unreachable, the API key is missing, or
/// the rate limit has been exceeded. Prevents consecutive duplicates.
/// </summary>
namespace Quotes.Service
{
    public static class LocalQuotesBank
    {
        private static int lastIndex = -1;

        private static readonly QuoteResponse[] quotes = new QuoteResponse[]
        {
            Q("The best way to predict the future is to invent it.", "Alan Kay"),
            Q("Talk is cheap. Show me the code.", "Linus Torvalds"),
            Q("Any sufficiently advanced technology is indistinguishable from magic.", "Arthur C. Clarke"),
            Q("The computer was born to solve problems that did not exist before.", "Bill Gates"),
            Q("Innovation distinguishes between a leader and a follower.", "Steve Jobs"),
            Q("The only way to do great work is to love what you do.", "Steve Jobs"),
            Q("Simplicity is the ultimate sophistication.", "Leonardo da Vinci"),
            Q("First, solve the problem. Then, write the code.", "John Johnson"),
            Q("Programs must be written for people to read, and only incidentally for machines to execute.", "Harold Abelson"),
            Q("The most dangerous phrase in the language is: We have always done it this way.", "Grace Hopper"),
            Q("A computer would deserve to be called intelligent if it could deceive a human into believing that it was human.", "Alan Turing"),
            Q("The function of good software is to make the complex appear to be simple.", "Grady Booch"),
            Q("Measuring programming progress by lines of code is like measuring aircraft building progress by weight.", "Bill Gates"),
            Q("It is not enough to do your best; you must know what to do, and then do your best.", "W. Edwards Deming"),
            Q("Technology is anything that was invented after you were born.", "Alan Kay"),
            Q("The advance of technology is based on making it fit in so that you do not really even notice it, so it is part of everyday life.", "Bill Gates"),
            Q("The science of today is the technology of tomorrow.", "Edward Teller"),
            Q("It has become appallingly obvious that our technology has exceeded our humanity.", "Albert Einstein"),
            Q("The great growling engine of change -- technology.", "Alvin Toffler"),
            Q("Technology is best when it brings people together.", "Matt Mullenweg"),
        };

        /// <summary>
        /// Returns a random quote from the local bank, avoiding
        /// returning the same quote twice in a row.
        /// </summary>
        public static QuoteResponse GetRandom()
        {
            if (quotes.Length == 0)
                return Q("No quotes available.", "System");

            int index;
            if (quotes.Length == 1)
            {
                index = 0;
            }
            else
            {
                do
                {
                    index = Random.Range(0, quotes.Length);
                } while (index == lastIndex);
            }

            lastIndex = index;
            return quotes[index];
        }

        private static QuoteResponse Q(string quote, string author)
        {
            return new QuoteResponse
            {
                quote = quote,
                author = author,
                work = "",
                categories = new string[] { "technology" }
            };
        }
    }
}
