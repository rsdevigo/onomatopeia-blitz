# Primeiros passos

Guia curto para **abrir o projeto**, **resolver dependências** e fazer um **smoke test offline**. Linguagem pensada para equipe mista (arte, game design, código).

## 1. Requisitos

- **Unity Editor** alinhado ao projeto: consulte `ProjectSettings/ProjectVersion.txt` (ex.: **6000.4.5f1**).
- **Git** (clone/pull).
- Espaço em disco razoável para a pasta Library (na primeira abertura ela cresce bastante).

## 2. Clonar e abrir

1. Clone o repositório em uma pasta local.
2. No **Unity Hub**, **Add** → escolha a pasta do projeto (a que contém `Assets/` e `ProjectSettings/`).
3. Abra o projeto com a **versão do Editor** indicada em `ProjectSettings/ProjectVersion.txt`.
4. Na primeira abertura, deixe o import e a **compilação de scripts** terminarem (barra de progresso no canto inferior).

## 3. Pacotes críticos

Os pacotes estão em `Packages/manifest.json`. Neste protótipo, destacam-se:

- **Input System** (`com.unity.inputsystem`) — usado pelo grab offline (mouse).
- **Netcode for GameObjects** (`com.unity.netcode.gameobjects`) e **Unity Transport** (`com.unity.transport`) — sessão em rede e `NetMatchSession`.
- **Test Framework** — testes EditMode em `Assets/Tests/`.

Se o Package Manager mostrar erros ou versões em conflito, use **Window → Package Manager** para o Unity resolver; use **Assets → Reimport All** só se necessário (é pesado).

## 4. Mapa rápido do repositório

| Área | Caminho | Conteúdo típico |
|------|---------|-----------------|
| Código Core (regras puras) | `Assets/Scripts/Core/` | Cartas, resolver, gerador, modelos de match |
| Gameplay (Unity) | `Assets/Scripts/Gameplay/` | Sessão local, rodadas, mesa, input de grab, minijogos, host do lobby |
| UI | `Assets/Scripts/UI/` | Views, presenters, view models, binding |
| Rede (NGO) | `Assets/Scripts/Networking/NGO/` | `NetMatchSession` |
| App / composição | `Assets/Scripts/App/` | Ex.: arranque rápido offline |
| UI Toolkit (arquivos) | `Assets/UI Toolkit/UXML/`, `USS/` | UXML e estilos |
| Arte / áudio / prefabs | `Assets/Art/`, `Assets/Audio/`, `Assets/Prefabs/` | Conteúdo e variantes de mesa |
| Testes | `Assets/Tests/EditMode/Core/` | Testes do resolver/gerador |
| Blueprint | `.cursor/plans/blitz_onomatopoeico_architecture_d4df416d.plan.md` | Arquitetura alvo (não substitui os manuais) |

## 5. Primeiro play offline (smoke test)

Objetivo: validar que o **loop de rodada** roda e que o **grab com mouse** encontra objetos na mesa.

### Componentes envolvidos (nomes de classe)

- `LocalMatchSession` — `MonoBehaviour` que implementa `IMatchSession`; avança fases e aceita `TrySubmitGrab`.
- `OfflineQuickStart` — opcional; no `Start()` chama `StartMatch` com `MatchRules` e seed (útil para testar sem UI).
- `TableRuntimeRegistry` — registra `SoundObjectInstance` por slot e faz **raycast** a partir de uma `Camera`.
- `SoundObjectInstance` — marca um objeto na mesa com **slot 0–2** e collider; registra-se no registry.
- `OfflineGrabInputDriver` — durante `GrabPhase`, clique esquerdo + raycast → `TrySubmitGrab`.

### O que precisa na cena (mínimo)

1. **Câmera** com tag/layer habituais; referência no `OfflineGrabInputDriver` se não usar `Camera.main`.
2. Um **GameObject** com `TableRuntimeRegistry`.
3. **Três** objetos com collider + `SoundObjectInstance` com `slotIndex` 0, 1 e 2 (ou configuração equivalente).
4. **GameObject** com `LocalMatchSession`.
5. Opcional: `OfflineQuickStart` no mesmo ou em outro objeto, com `session` arrastado.
6. Opcional: `OfflineGrabInputDriver` com referências a `Camera`, `TableRuntimeRegistry`, `LocalMatchSession` (ou deixe o código resolver com `FindAnyObjectByType` onde já existe fallback).

### UI (opcional neste passo)

Para ver o **HUD** de fase/pontos/carta:

- `UIDocument` + `HudView` com o campo **UXML** (`VisualTreeAsset`) apontando para `Assets/UI Toolkit/UXML/HUD/HUD.uxml`.

Se o `VisualTreeAsset` estiver vazio, a tela fica em branco: **sempre atribua o UXML no Inspector**.

## 6. Problemas comuns

| Sintoma | Causa provável | O que fazer |
|---------|----------------|-------------|
| Erros de assembly / tipo não encontrado | Asmdef ou pacote faltando | Abra `Packages/manifest.json`; deixe o Unity recompilar; veja [Manual do Programador](Manual-Programador.md) |
| NGO não faz spawn / RPC não chega | Falta `NetworkObject` no mesmo objeto que `NetMatchSession` | Adicione `NetworkObject`; registre o prefab na lista de prefabs de rede do projeto |
| UI não aparece | `UIDocument` sem `VisualTreeAsset` | Arraste o `.uxml` (importado como Visual Tree Asset) para o campo no componente ou na `HudView` / `MainMenuView` |
| Grab não faz nada | Fase não é `GrabPhase`, ou raycast não acerta no collider | Confirme a fase no `LocalMatchSession`; confira layers, colliders e câmera |
| Dois `TableRuntimeRegistry` | Raycast ou registro inconsistente | Mantenha **um** registry por cena de teste |

## 7. Próximos passos

- **Montar arte/UI/cenas:** [Manual do Designer](Manual-Designer.md)  
- **Regras, pacing, GDD, pedidos de feature:** [Manual do Game Designer](Manual-Game-Designer.md)  
- **Arquitetura, extensão, rede, testes:** [Manual do Programador](Manual-Programador.md)  
- **Criar catálogos e definitions (SO):** [Manual de ScriptableObjects](Manual-ScriptableObjects.md)  

Índice geral: [README desta pasta](README.md).
