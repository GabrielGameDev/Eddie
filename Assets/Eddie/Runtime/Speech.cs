using UnityEngine;

namespace Eddie
{
    /// <summary>
    /// ScriptableObject que representa um discurso do Eddie.
    /// Crie via: botão direito no Project → Create → Eddie → Speech
    /// </summary>
    [CreateAssetMenu(fileName = "Speech", menuName = "Eddie/Speech")]
    public class Speech : ScriptableObject
    {
        [Tooltip("Clipe de áudio a ser reproduzido.")]
        public AudioClip audioClip;

        [Tooltip("Texto de legenda exibido durante o áudio. Deixe vazio para não exibir nada.")]
        [TextArea(2, 5)]
        public string text;

        [Tooltip("Imagem exibida no monitor durante o áudio. Deixe nulo para não exibir nada.")]
        public Sprite image;
    }
}
