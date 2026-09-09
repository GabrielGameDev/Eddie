# Changelog

Todas as mudanças notáveis neste pacote serão documentadas aqui.

O formato é baseado em [Keep a Changelog](https://keepachangelog.com/pt-BR/1.0.0/),
e este projeto adere ao [Semantic Versioning](https://semver.org/lang/pt-BR/).

## [1.0.0] - 2026-09-09

### Adicionado
- Estrutura inicial do pacote UPM (`com.SinergiaEducacao.eddie`)
- `Speech` ScriptableObject com campos `AudioClip`, `text` e `image`
- `PlayAudios`: sistema de fila de áudios com suporte a legenda e imagem
  - Eventos `OnSpeechStarted` e `OnSpeechFinished`
  - Método `ResetPlayedHistory()` para permitir repetição
  - Opção `allowRepeat` para controlar deduplicação
  - Ocultação automática da imagem ao fim do áudio
- `LipSyncController`: lip sync em tempo real via BlendShape
  - `blendShapeIndex` configurável para suportar diferentes rigs
  - Detecção automática do `AudioSource` no mesmo GameObject
- `RandomAudio`: reprodução aleatória de grupos de Speech
  - `StartLoop()` / `StopLoop()` para controle manual
  - `minInterval` e `maxInterval` configuráveis pelo Inspector
  - Suporte a `allowRepeat` herdado do `PlayAudios`
- Namespace `Eddie` em todos os scripts
- Assembly Definition `Eddie.asmdef`
- Prefab `Eddie` pré-configurado
- Cena de exemplo em `Samples~/BasicExample`
- Editor script `EddieExporter` para geração de `.unitypackage`
