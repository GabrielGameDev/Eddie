# Eddie — Interactive Character

**Versão:** 1.0.0 | **Unity:** 6.0.58+ | **Autor:** Sinergia Educação

Eddie é um personagem 3D interativo para Unity com:
- 🎙️ **Fila de áudios** com suporte a legenda e imagem sincronizados
- 👄 **Lip sync em tempo real** via BlendShape
- 🎲 **Animações e falas aleatórias** com intervalos configuráveis
- 📦 Pronto para uso como pacote UPM ou `.unitypackage`

---

## Requisitos

- Unity **6.0.58** ou superior
- **TextMeshPro** (instalado automaticamente pelo UPM)

---

## Instalação

### Opção 1 — Via UPM (Git URL)

1. Abra o **Package Manager** (`Window > Package Manager`)
2. Clique em **+** → **Add package from git URL...**
3. Cole a URL do repositório:
   ```
   https://github.com/SinergiaEducacao/eddie.git
   ```

### Opção 2 — Via UPM (arquivo local)

1. Copie a pasta `Assets/Eddie` para qualquer local no seu sistema
2. No **Package Manager**, clique em **+** → **Add package from disk...**
3. Selecione o arquivo `package.json` dentro da pasta copiada

### Opção 3 — Via `.unitypackage`

1. No projeto Eddie, acesse o menu **Eddie > Export .unitypackage**
2. O arquivo `Eddie_Package.unitypackage` será gerado na raiz do projeto
3. No projeto de destino, acesse **Assets > Import Package > Custom Package...**
4. Selecione o arquivo gerado e importe tudo

---

## Uso rápido

### 1. Adicionar o Prefab à cena

Arraste o prefab `Eddie` de `Packages/Eddie/Prefabs/` (ou `Assets/Eddie/Prefabs/`) para sua cena.

### 2. Criar Speech Assets

Crie os ScriptableObjects de fala:

1. Clique com botão direito no **Project** → **Create → Eddie → Speech**
2. Configure:
   - **Audio Clip** — o arquivo de áudio
   - **Text** — legenda (opcional, deixe vazio para não exibir)
   - **Image** — sprite/imagem (opcional, deixe nulo para não exibir)

### 3. Configurar o Prefab no Inspector

Selecione o GameObject `Eddie` e configure os componentes:

#### PlayAudios
| Campo | Descrição |
|---|---|
| `Captions Text` | TMP_Text para legenda (pode ser nulo) |
| `Monitor Image` | Image UI para imagem (pode ser nulo) |
| `Allow Repeat` | Se marcado, Speeches já tocados repetem |

#### LipSyncController
| Campo | Descrição |
|---|---|
| `Audio Source` | Auto-detectado se vazio |
| `Skinned Mesh` | SkinnedMeshRenderer do rosto |
| `Blend Shape Index` | Índice do BlendShape da boca (padrão: 0) |
| `Volume Multiplier` | Amplificador do volume (ajuste conforme o modelo) |

#### RandomAudio
| Campo | Descrição |
|---|---|
| `Play Audios` | Referência ao componente PlayAudios |
| `Speech Groups` | Grupos de falas temáticos |
| `Min Interval` | Tempo mínimo entre falas (segundos) |
| `Max Interval` | Tempo máximo entre falas (segundos) |

### 4. Configurar Speech Groups no RandomAudio

```
Speech Groups
├── [0] speechName: "Saudação"
│        speeches: [Speech_OlaEstudante, Speech_BemVindo]
├── [1] speechName: "Dica"
│        speeches: [Speech_DicaMath, Speech_DicaPortugues]
└── [2] speechName: "Curiosidade"
         speeches: [Speech_CuriosMarte, Speech_CuriosaOceano]
```

### 5. Iniciar o loop de falas aleatórias

O Eddie **não inicia automaticamente**. Você controla quando ele começa a falar:

```csharp
using Eddie;
using UnityEngine;

public class MinhaLogica : MonoBehaviour
{
    [SerializeField] private RandomAudio eddieAudio;

    private void Start()
    {
        // Inicia o loop — Eddie falará a cada minInterval~maxInterval segundos
        eddieAudio.StartLoop();
    }

    private void OnDestroy()
    {
        eddieAudio.StopLoop();
    }
}
```

---

## Referência da API

### `Speech` (ScriptableObject)

| Propriedade | Tipo | Descrição |
|---|---|---|
| `audioClip` | `AudioClip` | Áudio a reproduzir |
| `text` | `string` | Legenda (opcional) |
| `image` | `Sprite` | Imagem (opcional) |

---

### `PlayAudios` (MonoBehaviour)

#### Propriedades públicas

| Propriedade | Tipo | Descrição |
|---|---|---|
| `captionsText` | `TMP_Text` | Referência ao texto de legenda |
| `monitorImage` | `Image` | Referência à imagem do monitor |
| `allowRepeat` | `bool` | Permite repetição de Speeches |

#### Métodos

| Método | Descrição |
|---|---|
| `PlayOrEnqueue(Speech)` | Adiciona um Speech à fila |
| `StopAndClearQueue()` | Para imediatamente e limpa a fila |
| `ResetPlayedHistory()` | Limpa o histórico de falas reproduzidas |

#### Eventos (UnityEvent)

| Evento | Argumento | Quando dispara |
|---|---|---|
| `OnSpeechStarted` | `Speech` | Ao iniciar a reprodução de um Speech |
| `OnSpeechFinished` | `Speech` | Ao terminar a reprodução de um Speech |

**Exemplo de uso dos eventos:**
```csharp
playAudios.OnSpeechStarted.AddListener(speech => {
    Debug.Log($"Iniciando: {speech.name}");
});

playAudios.OnSpeechFinished.AddListener(speech => {
    Debug.Log($"Finalizado: {speech.name}");
});
```

---

### `LipSyncController` (MonoBehaviour)

| Propriedade | Tipo | Padrão | Descrição |
|---|---|---|---|
| `audioSource` | `AudioSource` | Auto | Fonte de áudio monitorada |
| `skinnedMesh` | `SkinnedMeshRenderer` | — | Mesh com BlendShape da boca |
| `blendShapeIndex` | `int` | `0` | Índice do BlendShape |
| `volumeMultiplier` | `float` | `200` | Amplificador do volume |
| `syncInterval` | `float` | `0.1` | Intervalo de atualização (s) |
| `sampleCount` | `int` | `1024` | Amostras de áudio por leitura |

---

### `RandomAudio` (MonoBehaviour)

#### Propriedades públicas

| Propriedade | Tipo | Padrão | Descrição |
|---|---|---|---|
| `playAudios` | `PlayAudios` | — | Referência ao PlayAudios |
| `speechGroups` | `SpeechGroup[]` | — | Grupos de falas |
| `minInterval` | `float` | `30` | Intervalo mínimo (s) |
| `maxInterval` | `float` | `150` | Intervalo máximo (s) |

#### Métodos

| Método | Descrição |
|---|---|
| `StartLoop()` | Inicia o loop de falas aleatórias |
| `StopLoop()` | Para o loop |
| `PlayRandomNow()` | Reproduz um grupo aleatório imediatamente |

---

## Estrutura do pacote

```
Assets/Eddie/
├── package.json              ← Manifesto UPM
├── README.md                 ← Esta documentação
├── CHANGELOG.md              ← Histórico de versões
├── Animations/
│   ├── Eddie.controller      ← Animator Controller
│   ├── Armature_Eddie_Aguardando_Resposta.anim
│   └── Armature_Eddie_Cumprimento.anim
├── Art/
│   ├── Materials/            ← Materiais do modelo
│   └── Modelos/              ← FBX do Eddie
├── Editor/
│   ├── Eddie.Editor.asmdef
│   └── EddieExporter.cs      ← Menu Eddie > Export .unitypackage
├── Prefabs/
│   └── Eddie.prefab          ← Prefab pronto para uso
├── Runtime/
│   ├── Eddie.asmdef
│   ├── Speech.cs
│   ├── PlayAudios.cs
│   ├── LipSyncController.cs
│   └── RandomAudio.cs
└── Samples~/
    └── BasicExample/         ← Cena de exemplo
```

---

## Perguntas frequentes

**O Eddie não move a boca.**  
Verifique se o `blendShapeIndex` está correto para o seu modelo. Inspecione o `SkinnedMeshRenderer` do rosto e localize o índice do BlendShape referente à abertura da boca.

**O loop não começa.**  
O loop é **manual por design**. Você precisa chamar `randomAudio.StartLoop()` no código ou via UnityEvent no Inspector.

**Quero que um Speech repita.**  
Marque `Allow Repeat` no Inspector do `PlayAudios`, ou chame `playAudios.ResetPlayedHistory()` para limpar o histórico.

**Posso usar o Eddie sem UI (sem legenda/imagem)?**  
Sim. Deixe `Captions Text` e `Monitor Image` como `None` no Inspector. O sistema ignora UI nula.

---

## Licença

© Sinergia Educação. Todos os direitos reservados.


