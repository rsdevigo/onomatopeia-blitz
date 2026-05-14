# Getting Started

Guia curto para **abrir o projeto**, **resolver dependências** e fazer um **smoke test offline**. Linguagem: equipa mista (arte, game design, código).

## 1. Requisitos

- **Unity Editor** alinhado ao projeto: consulta `ProjectSettings/ProjectVersion.txt` (ex.: **6000.4.5f1**).
- **Git** (clone/pull).
- Espaço em disco razoável para Library (primeira abertura cresce bastante).

## 2. Clonar e abrir

1. Clona o repositório para uma pasta local.
2. No **Unity Hub**, **Add** → escolhe a pasta do projeto (a que contém `Assets/` e `ProjectSettings/`).
3. Abre o projeto com a **versão do Editor** indicada em `ProjectSettings/ProjectVersion.txt`.
4. Na primeira abertura, deixa o import e a **compilação de scripts** terminarem (barra de progresso no canto inferior).

## 3. Pacotes críticos

Os pacotes estão em `Packages/manifest.json`. Para este protótipo, destacam-se:

- **Input System** (`com.unity.inputsystem`) — usado pelo grab offline (rato).
- **Netcode for GameObjects** (`com.unity.netcode.gameobjects`) e **Unity Transport** (`com.unity.transport`) — sessão em rede e `NetMatchSession`.
- **Test Framework** — testes EditMode em `Assets/Tests/`.

Se o Package Manager mostrar erros ou versões em conflito, usa **Window → Package Manager** para deixar o Unity resolver, depois **Assets → Reimport All** só se necessário (é pesado).

## 4. Mapa rápido do repositório

| Área | Caminho | Conteúdo típico |
|------|---------|-----------------|
| Código Core (regras puras) | `Assets/Scripts/Core/` | Cartas, resolver, gerador, modelos de match |
| Gameplay (Unity) | `Assets/Scripts/Gameplay/` | Sessão local, rondas, mesa, input grab, minijogos, lobby host |
| UI | `Assets/Scripts/UI/` | Views, presenters, view models, binding |
| Rede (NGO) | `Assets/Scripts/Networking/NGO/` | `NetMatchSession` |
| App / composição | `Assets/Scripts/App/` | Ex.: arranque rápido offline |
| UI Toolkit (ficheiros) | `Assets/UI Toolkit/UXML/`, `USS/` | UXML e estilos |
| Arte / áudio / prefabs | `Assets/Art/`, `Assets/Audio/`, `Assets/Prefabs/` | Conteúdo e variantes de mesa |
| Testes | `Assets/Tests/EditMode/Core/` | Testes do resolver/gerador |
| Blueprint | `.cursor/plans/blitz_onomatopoeico_architecture_d4df416d.plan.md` | Arquitetura alvo (não substitui manuais) |

## 5. Primeiro play offline (smoke test)

Objetivo: validar que o **loop de ronda** corre e que o **grab com rato** encontra objetos na mesa.

### Componentes envolvidos (nomes de classe)

- `LocalMatchSession` — `MonoBehaviour` que implementa `IMatchSession`; avança fases e aceita `TrySubmitGrab`.
- `OfflineQuickStart` — opcional; no `Start()` chama `StartMatch` com `MatchRules` e seed (útil para testar sem UI).
- `TableRuntimeRegistry` — regista `SoundObjectInstance` por slot e faz **raycast** a partir de um `Camera`.
- `SoundObjectInstance` — marca um objeto na mesa com **slot 0–2** e collider; regista-se no registry.
- `OfflineGrabInputDriver` — durante `GrabPhase`, clique esquerdo + raycast → `TrySubmitGrab`.

### O que precisas na cena (mínimo)

1. **Câmara** com tag/layer habitual; referência no `OfflineGrabInputDriver` se não usares `Camera.main`.
2. Um **GameObject** com `TableRuntimeRegistry`.
3. **Três** objetos com collider + `SoundObjectInstance` com `slotIndex` 0, 1 e 2 (ou configuração equivalente).
4. **GameObject** com `LocalMatchSession`.
5. Opcional: `OfflineQuickStart` no mesmo ou noutro objeto, com `session` arrastado.
6. Opcional: `OfflineGrabInputDriver` com referências a `Camera`, `TableRuntimeRegistry`, `LocalMatchSession` (ou deixa encontrar por `FindFirstObjectByType` onde o código já faz fallback).

### UI (opcional neste passo)

Para ver **HUD** de fase/pontos/carta:

- `UIDocument` + `HudView` com o campo **UXML** (`VisualTreeAsset`) apontando para `Assets/UI Toolkit/UXML/HUD/HUD.uxml`.

Se o `VisualTreeAsset` estiver vazio, o ecrã fica em branco: **atribui sempre o UXML no Inspector**.

## 6. Problemas comuns

| Sintoma | Causa provável | O que fazer |
|---------|----------------|-------------|
| Erros de assembly / tipo não encontrado | Asmdef ou pacote em falta | Abre `Packages/manifest.json`; deixa o Unity recompilar; vê [Manual do Programador](Manual-Programador.md) |
| NGO não spawna / RPC não chega | Falta `NetworkObject` no mesmo objeto que `NetMatchSession` | Adiciona `NetworkObject`; regista o prefab na lista de prefabs de rede do projeto |
| UI não aparece | `UIDocument` sem `VisualTreeAsset` | Arrasta o `.uxml` (importado como Visual Tree Asset) para o campo no componente ou na `HudView` / `MainMenuView` |
| Grab não faz nada | Fase não é `GrabPhase`, ou raycast não acerta collider | Confirma fase no `LocalMatchSession`; confirma layers, colliders e câmara |
| Dois `TableRuntimeRegistry` | Raycast ou registo inconsistente | Mantém **um** registry por cena de teste |

## 7. Onde ir a seguir

- **Montar arte/UI/cenas:** [Manual do Designer](Manual-Designer.md)  
- **Regras, pacing, GDD, pedidos de feature:** [Manual do Game Designer](Manual-Game-Designer.md)  
- **Arquitetura, extensão, rede, testes:** [Manual do Programador](Manual-Programador.md)  

Índice geral: [README desta pasta](README.md).
