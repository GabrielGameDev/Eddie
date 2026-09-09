using System.Collections;
using UnityEngine;

namespace Eddie
{
    /// <summary>
    /// Realiza lip sync em tempo real lendo os dados de saída do AudioSource
    /// e aplicando o volume como peso de um BlendShape no SkinnedMeshRenderer.
    ///
    /// Configure no Inspector:
    ///   - skinnedMesh: o SkinnedMeshRenderer do rosto do personagem
    ///   - blendShapeIndex: índice do BlendShape da boca (padrão: 0)
    ///   - volumeMultiplier: amplificador do volume (ajuste conforme o modelo)
    /// </summary>
    public class LipSyncController : MonoBehaviour
    {
        // ─── Referências ───────────────────────────────────────────────────────
        [Header("Referências")]
        [Tooltip("AudioSource que será monitorado. Se nulo, busca automaticamente no mesmo GameObject.")]
        public AudioSource audioSource;

        [Tooltip("SkinnedMeshRenderer do rosto do personagem com BlendShape de boca.")]
        public SkinnedMeshRenderer skinnedMesh;

        // ─── Configurações ─────────────────────────────────────────────────────
        [Header("Configurações")]
        [Tooltip("Índice do BlendShape que representa a abertura da boca. Padrão: 0.")]
        public int blendShapeIndex = 0;

        [Tooltip("Multiplicador aplicado ao volume para ampliar o movimento da boca.")]
        public float volumeMultiplier = 200f;

        [Tooltip("Intervalo (em segundos) entre cada leitura de volume.")]
        public float syncInterval = 0.1f;

        [Tooltip("Número de amostras de áudio coletadas por canal para calcular o volume.")]
        public int sampleCount = 1024;

        // ─── Privados ──────────────────────────────────────────────────────────
        private float[] samples;

        // ──────────────────────────────────────────────────────────────────────

        private void Start()
        {
            // Fallback automático: tenta pegar AudioSource no mesmo GameObject
            if (audioSource == null)
                audioSource = GetComponent<AudioSource>();

            if (audioSource == null)
                Debug.LogError("[Eddie] LipSyncController: nenhum AudioSource encontrado.", this);

            if (skinnedMesh == null)
                Debug.LogError("[Eddie] LipSyncController: SkinnedMeshRenderer não atribuído.", this);

            samples = new float[sampleCount];
            StartCoroutine(LipSyncing());
        }

        private IEnumerator LipSyncing()
        {
            while (true)
            {
                if (skinnedMesh != null)
                    skinnedMesh.SetBlendShapeWeight(blendShapeIndex, CalculateVolume() * volumeMultiplier);

                yield return new WaitForSeconds(syncInterval);
            }
        }

        private float CalculateVolume()
        {
            if (audioSource == null || !audioSource.isPlaying)
                return 0f;

            float volume = 0f;
            int channels = AudioSettings.speakerMode == AudioSpeakerMode.Mono ? 1 : 2;

            for (int channel = 0; channel < channels; channel++)
            {
                audioSource.GetOutputData(samples, channel);
                for (int i = 0; i < sampleCount; i++)
                    volume += Mathf.Abs(samples[i]);
            }

            return volume / sampleCount;
        }
    }
}
