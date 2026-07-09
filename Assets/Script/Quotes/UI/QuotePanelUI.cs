using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Quotes.Data;

/// <summary>
/// Updates the World Space Canvas UI elements with quote data.
/// Handles fade-in/fade-out transitions of the text elements.
/// Attached to the quote panel Canvas GameObject.
/// </summary>
namespace Quotes.UI
{
    public class QuotePanelUI : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private Text txtQuote;
        [SerializeField] private Text txtAuthor;

        [Header("Transition Settings")]
        [SerializeField] private float fadeDuration = 0.4f;

        private Coroutine transitionCoroutine;

        /// <summary>
        /// Displays a new quote with a fade transition.
        /// If a transition is already running, it is interrupted.
        /// </summary>
        public void DisplayQuote(QuoteResponse quote)
        {
            if (quote == null) return;

            if (transitionCoroutine != null)
                StopCoroutine(transitionCoroutine);

            transitionCoroutine = StartCoroutine(TransitionToQuote(quote));
        }

        /// <summary>
        /// Sets the quote text immediately without any transition.
        /// Used for the initial display before the first API call completes.
        /// </summary>
        public void SetInitialText(string quote, string author)
        {
            if (txtQuote != null)
                txtQuote.text = quote;
            if (txtAuthor != null)
                txtAuthor.text = author;
        }

        /// <summary>
        /// Coroutine that fades out text, updates it, then fades back in.
        /// </summary>
        private IEnumerator TransitionToQuote(QuoteResponse quote)
        {
            Color quoteOrig = txtQuote != null ? txtQuote.color : Color.white;
            Color authorOrig = txtAuthor != null ? txtAuthor.color : Color.white;

            // Fade out
            float elapsed = 0f;
            while (elapsed < fadeDuration)
            {
                elapsed += Time.deltaTime;
                float alpha = Mathf.Lerp(1f, 0f, elapsed / fadeDuration);
                SetTextAlpha(alpha, quoteOrig, authorOrig);
                yield return null;
            }
            SetTextAlpha(0f, quoteOrig, authorOrig);

            // Update text content
            if (txtQuote != null)
                txtQuote.text = $"\"{quote.quote}\"";

            if (txtAuthor != null)
                txtAuthor.text = $"- {quote.author}";

            // Fade in
            elapsed = 0f;
            while (elapsed < fadeDuration)
            {
                elapsed += Time.deltaTime;
                float alpha = Mathf.Lerp(0f, 1f, elapsed / fadeDuration);
                SetTextAlpha(alpha, quoteOrig, authorOrig);
                yield return null;
            }
            SetTextAlpha(1f, quoteOrig, authorOrig);

            transitionCoroutine = null;
        }

        private void SetTextAlpha(float alpha, Color quoteOrig, Color authorOrig)
        {
            if (txtQuote != null)
                txtQuote.color = new Color(quoteOrig.r, quoteOrig.g, quoteOrig.b, alpha);
            if (txtAuthor != null)
                txtAuthor.color = new Color(authorOrig.r, authorOrig.g, authorOrig.b, alpha);
        }
    }
}
