using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;
using UnityEngine.UI;

namespace Eddie
{
    /// <summary>
    /// Gerencia uma fila de Speech assets, reproduzindo um de cada vez.
    /// Controla legenda (TMP_Text) e imagem (Image) sincronizados ao áudio.
    ///
    /// Uso básico:
    ///   playAudios.PlayOrEnqueue(mySpeech);
    ///
    /// Para permitir repetição de Speeches já tocados:
    ///   playAudios.allowRepeat = true;
    ///   // ou chame:
    ///   playAudios.ResetPlayedHistory();
    /// </summary>
    [RequireComponent(typeof(AudioSource))]
    public class PlayAudios : MonoBehaviour
    {
        // ─── Referências de UI ─────────────────────────────────────────────────
        [Header("UI (opcional)")]
        [Tooltip("Componente TMP_Text onde a legenda será exibida. Pode ser nulo.")]
        public TMP_Text captionsText;

        [Tooltip("Componente Image onde a imagem do Speech será exibida. Pode ser nulo.")]
        public Image monitorImage;

        // ─── Configurações ─────────────────────────────────────────────────────
        [Header("Configurações")]
        [Tooltip("Se verdadeiro, Speeches já tocados podem ser tocados novamente.")]
        public bool allowRepeat = false;

        // ─── Eventos ───────────────────────────────────────────────────────────
        [Header("Eventos")]
        [Tooltip("Disparado quando um Speech começa a tocar. Passa o Speech como argumento.")]
        public UnityEvent<Speech> OnSpeechStarted;

        [Tooltip("Disparado quando um Speech termina de tocar. Passa o Speech como argumento.")]
        public UnityEvent<Speech> OnSpeechFinished;

        // ─── Privados ──────────────────────────────────────────────────────────
        private AudioSource audioSource;
        private Queue<Speech> audioQueue;
        private Coroutine playbackCoroutine;
        private HashSet<Speech> speechPlayedSet;

        /// <summary>Lista dos Speeches já reproduzidos (para inspeção no Editor).</summary>
        [HideInInspector]
        public List<Speech> speechPlayed = new();

        // ──────────────────────────────────────────────────────────────────────

        private void Awake()
        {
            audioSource = GetComponent<AudioSource>();
            audioQueue = new Queue<Speech>();
            speechPlayedSet = new HashSet<Speech>();
        }

        /// <summary>
        /// Adiciona um Speech à fila de reprodução.
        /// Se <see cref="allowRepeat"/> for false, Speeches já tocados são ignorados.
        /// </summary>
        public void PlayOrEnqueue(Speech speech)
        {
            if (speech == null)
            {
                Debug.LogWarning("[Eddie] Tentou enfileirar um Speech nulo.", this);
                return;
            }

            if (!allowRepeat && speechPlayedSet.Contains(speech))
                return;

            if (!allowRepeat)
            {
                speechPlayedSet.Add(speech);
                speechPlayed.Add(speech);
            }

            audioQueue.Enqueue(speech);

            if (playbackCoroutine == null)
                playbackCoroutine = StartCoroutine(ProcessQueue());
        }

        /// <summary>
        /// Para o áudio atual e limpa toda a fila.
        /// </summary>
        public void StopAndClearQueue()
        {
            if (playbackCoroutine != null)
            {
                StopCoroutine(playbackCoroutine);
                playbackCoroutine = null;
            }

            audioSource.Stop();
            audioQueue.Clear();

            HideUI();
            Debug.Log("[Eddie] Fila de áudio interrompida e limpa.");
        }

        /// <summary>
        /// Limpa o histórico de Speeches reproduzidos,
        /// permitindo que sejam enfileirados novamente.
        /// </summary>
        public void ResetPlayedHistory()
        {
            speechPlayedSet.Clear();
            speechPlayed.Clear();
            Debug.Log("[Eddie] Histórico de reprodução resetado.");
        }

        // ─── Internos ──────────────────────────────────────────────────────────

        private IEnumerator ProcessQueue()
        {
            while (audioQueue.Count > 0)
            {
                Speech current = audioQueue.Dequeue();

                // Atualiza UI
                if (captionsText != null)
                    captionsText.text = current.text;

                if (monitorImage != null)
                {
                    if (current.image != null)
                    {
                        monitorImage.sprite = current.image;
                        monitorImage.enabled = true;
                    }
                    // Se não houver imagem no Speech, mantém o estado atual
                }

                // Toca o áudio
                audioSource.clip = current.audioClip;
                audioSource.Play();

                OnSpeechStarted?.Invoke(current);

                // Aguarda o fim do áudio
                yield return new WaitWhile(() => audioSource.isPlaying);

                OnSpeechFinished?.Invoke(current);
            }

            // Fila vazia — oculta UI
            HideUI();
            playbackCoroutine = null;
        }

        private void HideUI()
        {
            if (captionsText != null)
                captionsText.text = string.Empty;

            if (monitorImage != null)
                monitorImage.enabled = false;
        }
    }
}
