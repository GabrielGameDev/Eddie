using System.Threading;
using UnityEngine;

namespace Eddie
{
    /// <summary>
    /// Grupo de Speeches com um nome identificador.
    /// Configure grupos temáticos (ex: "Saudação", "Dica", "Curiosidade")
    /// e o Eddie selecionará um grupo aleatório para reproduzir.
    /// </summary>
    [System.Serializable]
    public class SpeechGroup
    {
        [Tooltip("Nome do grupo (apenas para identificação no Editor).")]
        public string groupName;

        [Tooltip("Speeches que fazem parte deste grupo. Serão tocados em sequência.")]
        public Speech[] speeches;
    }

    /// <summary>
    /// Reproduz grupos de Speeches de forma aleatória em um loop com intervalo configurável.
    /// 
    /// Uso:
    ///   1. Atribua os SpeechGroups no Inspector.
    ///   2. Chame <see cref="StartLoop"/> para iniciar (ex: no Start() do seu script ou por evento).
    ///   3. Chame <see cref="StopLoop"/> para parar.
    /// </summary>
    public class RandomAudio : MonoBehaviour
    {
        // ─── Referências ───────────────────────────────────────────────────────
        [Header("Referências")]
        [Tooltip("Referência ao PlayAudios do Eddie. Geralmente no mesmo GameObject.")]
        public PlayAudios playAudios;

        // ─── Grupos de Speech ──────────────────────────────────────────────────
        [Header("Grupos de Speech")]
        [Tooltip("Configure os grupos temáticos de Speeches. Um grupo aleatório será selecionado a cada intervalo.")]
        public SpeechGroup[] speechGroups;

        // ─── Configurações ─────────────────────────────────────────────────────
        [Header("Configurações de Intervalo")]
        [Tooltip("Tempo mínimo (em segundos) entre reproduções aleatórias.")]
        public float minInterval = 30f;

        [Tooltip("Tempo máximo (em segundos) entre reproduções aleatórias.")]
        public float maxInterval = 150f;

        // ─── Privados ──────────────────────────────────────────────────────────
        private CancellationTokenSource cts;
        private bool isLooping = false;

        // ──────────────────────────────────────────────────────────────────────

        private void OnDestroy()
        {
            StopLoop();
        }

        /// <summary>
        /// Inicia o loop de reprodução aleatória.
        /// Chame este método manualmente (ex: no Start() do seu script, por um evento, ou por botão).
        /// </summary>
        public void StartLoop()
        {
            if (isLooping)
            {
                Debug.LogWarning("[Eddie] RandomAudio: loop já está em execução.", this);
                return;
            }

            if (speechGroups == null || speechGroups.Length == 0)
            {
                Debug.LogWarning("[Eddie] RandomAudio: nenhum SpeechGroup configurado.", this);
                return;
            }

            if (playAudios == null)
            {
                Debug.LogError("[Eddie] RandomAudio: PlayAudios não atribuído.", this);
                return;
            }

            cts = new CancellationTokenSource();
            isLooping = true;
            RunLoop(cts.Token);
        }

        /// <summary>
        /// Para o loop de reprodução aleatória.
        /// </summary>
        public void StopLoop()
        {
            if (!isLooping) return;

            cts?.Cancel();
            cts?.Dispose();
            cts = null;
            isLooping = false;
            Debug.Log("[Eddie] RandomAudio: loop parado.");
        }

        /// <summary>
        /// Reproduz imediatamente um grupo de Speeches aleatório, sem esperar o intervalo.
        /// Útil para disparar o Eddie sob demanda (ex: ao clicar em um botão).
        /// </summary>
        public void PlayRandomNow()
        {
            if (speechGroups == null || speechGroups.Length == 0) return;
            if (playAudios == null) return;

            EnqueueRandomGroup();
        }

        // ─── Internos ──────────────────────────────────────────────────────────

        private async void RunLoop(CancellationToken token)
        {
            Debug.Log("[Eddie] RandomAudio: loop iniciado.");

            while (!token.IsCancellationRequested)
            {
                float waitTime = Random.Range(minInterval, maxInterval);
                Debug.Log($"[Eddie] RandomAudio: próxima fala em {waitTime:F0}s.");

                try
                {
                    await Awaitable.WaitForSecondsAsync(waitTime, token);
                }
                catch (System.OperationCanceledException)
                {
                    break;
                }

                if (!token.IsCancellationRequested)
                    EnqueueRandomGroup();
            }

            isLooping = false;
            Debug.Log("[Eddie] RandomAudio: loop encerrado.");
        }

        private void EnqueueRandomGroup()
        {
            int randomIndex = Random.Range(0, speechGroups.Length);
            SpeechGroup selected = speechGroups[randomIndex];

            Debug.Log($"[Eddie] RandomAudio: reproduzindo grupo '{selected.groupName}'.");

            foreach (Speech speech in selected.speeches)
                playAudios.PlayOrEnqueue(speech);
        }
    }
}
