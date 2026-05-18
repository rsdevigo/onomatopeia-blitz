---
name: Fluxo cenas blueprint B
overview: "Evoluir o fluxo de cenas (Fase 1) para o blueprint: Gameplay_Core + minijogo aditivo, MinigameDescriptor/Catalog, MinigameServices injetado em OnRegister, MinigameLoader/orquestrador e substituição de OfflineQuickStart."
todos:
  - id: split-scenes
    content: Dividir 30_Gameplay_Offline em 30_Gameplay_Core + 31_Minigame_Blitz + 32_Minigame_Fantasma; atualizar Build Settings
    status: completed
  - id: minigame-so
    content: Criar MinigameDescriptor + MinigameCatalog SO e assets em ScriptableObjects/Minigames
    status: completed
  - id: session-events
    content: Expor CardPrepared/RoundResolved em IMatchSession/LocalMatchSession
    status: completed
  - id: minigame-services
    content: Definir interfaces do saco de serviços, MinigameServicesHost na core e injeção em OnRegister (substituir Empty)
    status: completed
  - id: minigame-loader
    content: Implementar MinigameLoader (Load/Unload additive) + OfflineMinigameOrchestrator com ciclo IMinigame
    status: completed
  - id: scene-flow-v2
    content: Evoluir SceneFlow/SceneNames/GameSessionPrefs (SelectedMinigameId); remover OfflineQuickStart duplicado
    status: completed
  - id: menu-minigame-pick
    content: "MainMenu: seleção de minijogo + gravar prefs; continuar → LoadOfflineGame"
    status: completed
  - id: input-fantasma
    content: Documentar/ajustar grab Blitz vs Fantasma (driver vs TrySubmitWorldGrab)
    status: completed
  - id: docs-blueprint-flow
    content: Atualizar Manual-Programador com fluxo core+aditivo e extensão de minijogo
    status: completed
isProject: false
---

# Fluxo de cenas + IMinigame (blueprint, abordagem B)

## Contexto

**Fase 1 (concluída)** — plano original [`fluxo_cenas_mainmenu_b9aec075.plan.md`](.cursor/plans/fluxo_cenas_mainmenu_b9aec075.plan.md):

- Cenas UI: `10_MainMenu`, `40_Results`, `50_Leaderboard`
- Jogo monolítico: [`30_Gameplay_Offline.unity`](Assets/Scenes/30_Gameplay_Offline.unity) (cópia de SampleScene)
- [`SceneFlow`](Assets/Scripts/Gameplay/Navigation/SceneFlow.cs) / [`SceneNames`](Assets/Scripts/Gameplay/Navigation/SceneNames.cs) em `Blitz.Gameplay.Navigation`
- [`GameSessionPrefs`](Assets/Scripts/Core/GameSessionPrefs.cs) (nome, dificuldade, score)
- [`OfflineQuickStart`](Assets/Scripts/App/OfflineQuickStart.cs): `StartMatch` + `LoadResults` no `MatchEnd`
- Views ligadas a `SceneFlow` (menu, resultados, ranking)

**Gap vs** [`blitz_onomatopoeico_architecture_d4df416d.plan.md`](.cursor/plans/blitz_onomatopoeico_architecture_d4df416d.plan.md) §2 e §11:

| Blueprint | Hoje |
|-----------|------|
| `30_Gameplay_Core` + `31_Minigame_*` **aditivas** | Tudo numa cena `30_Gameplay_Offline` |
| `IMinigameLoader` / `MinigameRegistry` (SO) | `IMinigame` existe; **ninguém** chama o ciclo de vida |
| `MinigameDescriptor` | Não existe |
| `SceneFlow` só navega; match via minijogo | `OfflineQuickStart` ignora `IMinigame` |
| `MinigameServices` com `IAudioDirector`, `IInputRouter`, etc. | [`MinigameServices.Empty`](Assets/Scripts/Gameplay/Minigames/IMinigame.cs) fixo; `OnRegister` vazio nos minijogos |

Este plano descreve a **Fase 2 (abordagem B)**: core + cena aditiva + loader/orquestrador + **saco de serviços** injetado em `OnRegister`.

```mermaid
flowchart TB
  subgraph ui [UI Single scenes]
    MainMenu[10_MainMenu]
    Results[40_Results]
    Leaderboard[50_Leaderboard]
  end
  subgraph match [Match stack]
    Core[30_Gameplay_Core Single]
    Additive[31/32_Minigame Additive]
    Orchestrator[OfflineMinigameOrchestrator]
    ServicesHost[MinigameServicesHost]
    Session[LocalMatchSession]
    Minigame[IMinigame impl]
  end
  MainMenu -->|"SceneFlow.LoadOfflineMatch(minigameId)"| Core
  Core --> Orchestrator
  Core --> ServicesHost
  Orchestrator -->|"Build MinigameServices"| ServicesHost
  Orchestrator -->|"LoadSceneAsync Additive"| Additive
  Orchestrator -->|"OnRegister services"| Minigame
  Orchestrator --> Session
  Minigame -->|"OnMatchBegin"| Session
  Orchestrator -->|"MatchEnd"| Results
  Results --> Leaderboard
  Results --> MainMenu
  Leaderboard --> MainMenu
```

---

## 1. Reorganizar cenas (Editor)

**Objetivo:** separar sessão/HUD/input (core) de mesa/props/ambiente (minijogo).

| Cena | Conteúdo (mover desde `30_Gameplay_Offline`) |
|------|-----------------------------------------------|
| **`30_Gameplay_Core.unity`** (novo; pode renomear/refatorar `30_Gameplay_Offline`) | `LocalMatchSession`, `HUD` + `HudView`, **`MinigameServicesHost`**, **`OfflineMinigameOrchestrator`**, `OfflineGrabInputDriver` (registado no saco como `IInputRouter`), câmara/luz base, opcional `NetworkBootstrap` futuro |
| **`31_Minigame_Blitz.unity`** (aditiva) | `TableRuntimeRegistry`, 3× `SoundObjectInstance`, layout Blitz, `BlitzOnomatopoeicoMinigame` |
| **`32_Minigame_Fantasma.unity`** (aditiva) | Variante de mesa + `FantasmaLadraoMinigame` |

- Manter `SampleScene.unity` só para dev rápido (opcional) ou apontar para core+blitz.
- **Build Settings** (ordem sugerida, índice 0 = menu):
  0. `10_MainMenu`
  1. `30_Gameplay_Core`
  2. `31_Minigame_Blitz`
  3. `32_Minigame_Fantasma`
  4. `40_Results`
  5. `50_Leaderboard`
  6. `SampleScene` (opcional)

**Opcional (blueprint):** `00_Bootstrap.unity` com serviços `DontDestroyOnLoad` (leaderboard JSON futuro) → carrega menu na 1ª frame; não bloqueia esta fase.

---

## 2. Dados de sessão e seleção no menu

Estender [`GameSessionPrefs`](Assets/Scripts/Core/GameSessionPrefs.cs):

- `SelectedMinigameId` (string, ex. `"blitz_ono"`, `"fantasma"`)

No menu ([`MainMenuView`](Assets/Scripts/UI/Views/MainMenuView.cs) / UXML):

- Dropdown ou botões para minijogo (fase mínima: 2 entradas fixas).
- `OnContinue`: gravar `PlayerName`, `DifficultyIndex`, **`SelectedMinigameId`**.

Regras/dificuldade → `MatchRules` + seed: ler `DifficultyIndex` no orquestrador (valores default aceitáveis na primeira entrega; tuning fino pode ficar para plano de bots/dificuldade).

---

## 3. `MinigameDescriptor` + catálogo (Core/Gameplay)

Conforme blueprint §3 / §10:

- **`MinigameDescriptor`** (`ScriptableObject`, `Assets/ScriptableObjects/Minigames/`):
  - `MinigameId` (string estável)
  - `AdditiveSceneName` (ex. `31_Minigame_Blitz`)
  - Metadados opcionais: display name, thumbnail
- **`MinigameCatalog`** (SO lista de descriptors) ou array num host — resolve `MinigameId` → descriptor.

Sem Addressables nesta fase; cenas devem estar na Build Settings.

---

## 4. `MinigameServices` (blueprint §11)

Hoje [`MinigameServices`](Assets/Scripts/Gameplay/Minigames/IMinigame.cs) é uma classe vazia com `Empty`. O blueprint define um **saco read-only** montado na **Gameplay_Core** e passado em `OnRegister` **antes** de `OnSceneLoaded`, para os minijogos cachearem dependências sem `FindObjectOfType` disperso.

### Contratos (interfaces)

Colocar em `Assets/Scripts/Gameplay/Minigames/` (ou `Assets/Scripts/Core/` só se forem puras sem `UnityEngine` — na prática ficam em Gameplay):

| Interface | Responsabilidade nesta fase | Implementação mínima |
|-----------|----------------------------|----------------------|
| **`IAudioDirector`** | Tocar cue da carta / SFX de mesa | Wrapper em torno do `AudioSource` de [`LocalMatchSession`](Assets/Scripts/Gameplay/Match/LocalMatchSession.cs) ou `AudioSource` dedicado na core |
| **`IInputRouter`** | Submeter grab na sessão | Adaptador sobre [`OfflineGrabInputDriver`](Assets/Scripts/Gameplay/Input/OfflineGrabInputDriver.cs) + `IMatchSession.TrySubmitGrab`; Fantasma pode expor ramo alternativo via minijogo |
| **`IPrefabSpawner`** | Spawn local de props/VFX do minijogo | `Instantiate` simples com parent opcional (pool fica para depois) |
| **`IPlayerVisualRegistry`** | Placeholder para avatares/cores por seat | **`NullPlayerVisualRegistry`** no-op até lobby/MP |

### Evoluir `MinigameServices`

Substituir o tipo vazio por propriedades read-only (construtor interno ou factory):

```csharp
public sealed class MinigameServices
{
    public IAudioDirector Audio { get; }
    public IInputRouter Input { get; }
    public IPrefabSpawner Spawner { get; }
    public IPlayerVisualRegistry Players { get; }

    public static MinigameServices Empty { get; } // manter só para testes EditMode
    internal static MinigameServices Create(...) => ...
}
```

### `MinigameServicesHost` (Gameplay_Core)

- `MonoBehaviour` na cena **`30_Gameplay_Core`** (mesmo GO que orquestrador ou filho).
- Referências serializadas: `LocalMatchSession`, `OfflineGrabInputDriver`, opcional prefab root para spawn.
- Método `Build()` (chamado pelo orquestrador após core `Awake`): instancia adaptadores concretos e devolve `MinigameServices`.
- **Não** usar `DontDestroyOnLoad` nesta fase — o saco morre com a cena core.

### Ligação ao orquestrador e minijogos

Ordem no [`OfflineMinigameOrchestrator`](#4-minigameloader--offlineminigameorchestrator) (secção seguinte):

1. Core carregada → `services = servicesHost.Build()`
2. Cena aditiva carregada → resolver `IMinigame`
3. **`minigame.OnRegister(services)`** (nunca `Empty` em runtime offline)
4. `OnSceneLoaded()` → minijogo usa serviços cacheados (ex.: `Input` para ligar raycast, `Audio` para cues locais)
5. … resto do ciclo …
6. `OnUnregister()` → minijogos libertam referências; host descartado ao unload da core

Atualizar implementações:

- [`BlitzOnomatopoeicoMinigame`](Assets/Scripts/Gameplay/Minigames/BlitzOnomatopoeicoMinigame.cs) — em `OnRegister`, guardar `MinigameServices`; opcionalmente delegar áudio de carta ao `IAudioDirector` em `OnRoundBegin` (reduz duplicação com sessão num passo posterior).
- [`FantasmaLadraoMinigame`](Assets/Scripts/Gameplay/Minigames/FantasmaLadraoMinigame.cs) — cachear `Input` / `Audio`; `TrySubmitWorldGrab` pode continuar na API pública do minijogo mas usar `services.Input` por baixo.

### Testes

- EditMode: `MinigameServices.Create` com mocks/stubs das quatro interfaces; garantir que `OnRegister` não rebenta com nulls.
- PlayMode smoke: orquestrador chama `OnRegister` com saco não vazio.

---

## 5. `MinigameLoader` + `OfflineMinigameOrchestrator`

**Local:** `Assets/Scripts/Gameplay/Minigames/` (ou `Navigation/` se preferir agrupar com `SceneFlow`).

### `IMinigameLoader` (interface fina)

- `LoadAsync(MinigameDescriptor descriptor)` → `Scene` aditiva carregada
- `UnloadAsync()` → `SceneManager.UnloadSceneAsync`

### `OfflineMinigameOrchestrator` (`MonoBehaviour` na **Gameplay_Core**)

Responsabilidades (único dono do fluxo de partida offline):

1. **Entrada:** `Start()` ou chamado após core carregar — lê `GameSessionPrefs.SelectedMinigameId`, resolve descriptor, chama loader aditivo.
2. **`IMinigame`:** `GetComponentInChildren` / referência no descriptor para o `MonoBehaviour` que implementa [`IMinigame`](Assets/Scripts/Gameplay/Minigames/IMinigame.cs) na cena aditiva (ou prefab root na cena aditiva).
3. **Ciclo de vida** (ordem do blueprint §11):
   - `OnRegister(servicesHost.Build())` → `OnSceneLoaded()` (ver [§4 MinigameServices](#4-minigameservices-blueprint-§11))
   - `OnMatchBegin(MatchConfig)` — minijogo chama `LocalMatchSession.StartMatch` (como [`BlitzOnomatopoeicoMinigame`](Assets/Scripts/Gameplay/Minigames/BlitzOnomatopoeicoMinigame.cs) já faz)
   - `OnRoundBegin` / `OnRoundEnd` — ver §6
   - `OnMatchEnd()` → `SceneFlow.LoadResults(session.Score)` → `OnUnregister()` + unload aditiva
4. **Substituir** [`OfflineQuickStart`](Assets/Scripts/App/OfflineQuickStart.cs): remover da cena core ou reduzir a wrapper que só delega no orquestrador (evitar dois `StartMatch`).

`SceneFlow` **não** implementa lógica de minijogo; apenas carrega o stack de match:

```csharp
// SceneFlow (evolução)
public static void LoadOfflineMatch(string minigameId) {
    PlayerPrefs.SetString(GameSessionPrefs.SelectedMinigameId, minigameId);
    SceneManager.LoadSceneAsync(SceneNames.GameplayCore, LoadSceneMode.Single);
    // Orchestrator on core Start loads additive scene from prefs/catalog
}
```

Alternativa equivalente: `LoadOfflineGame()` lê prefs já gravados pelo menu (comportamento atual + orquestrador).

---

## 6. Ponte `LocalMatchSession` → `IMinigame`

Hoje [`LocalMatchSession`](Assets/Scripts/Gameplay/Match/LocalMatchSession.cs) só expõe `StateChanged`; `CardPrepared` e `RoundResolved` estão no `RoundController` privado.

**Alteração mínima:**

- Expor em `IMatchSession` (ou interface `IMatchSessionEvents`):
  - `event Action<GeneratedCard> CardPrepared`
  - `event Action<RoundOutcome> RoundResolved`
- `LocalMatchSession` reencaminha os eventos do `_round`.

O orquestrador subscreve:

- `CardPrepared` + `ActiveSet` → `minigame.OnRoundBegin(card, set)`
- `RoundResolved` → `minigame.OnRoundEnd(outcome)`
- `StateChanged` quando `Phase == MatchEnd` → sequência fim (se ainda não tratado em `RoundResolved` da última rodada)

Garantir **uma** transição para resultados (flag `_matchEndHandled` como em `OfflineQuickStart`).

---

## 7. Input e Fantasma

- **Blitz:** [`OfflineGrabInputDriver`](Assets/Scripts/Gameplay/Input/OfflineGrabInputDriver.cs) na **core**, registado no saco como **`IInputRouter`**; o driver continua na cena core mas o minijogo não o referencia diretamente.
- **Fantasma:** grab via [`FantasmaLadraoMinigame.TrySubmitWorldGrab`](Assets/Scripts/Gameplay/Minigames/FantasmaLadraoMinigame.cs) usando `services.Input` (adaptador que aplica `ISolutionSpaceAdapter`); desativar o driver genérico na variante Fantasma ou fazer `IInputRouter` no-op para raycast Blitz (documentar no manual).

---

## 8. `SceneNames` e documentação

Atualizar [`SceneNames.cs`](Assets/Scripts/Gameplay/Navigation/SceneNames.cs):

- `GameplayCore = "30_Gameplay_Core"`
- `MinigameBlitz = "31_Minigame_Blitz"`
- `MinigameFantasma = "32_Minigame_Fantasma"`
- Deprecar/remover referência principal a `30_Gameplay_Offline` após migração.

Atualizar secção **Fluxo de cenas** em [`Docs/Manual-Programador.md`](Docs/Manual-Programador.md): diagrama core+aditiva, ordem Build Settings, prefs, orquestrador, extensão “novo minijogo” (novo SO + cena + entrada no catálogo).

---

## 9. Critérios de pronto (Fase 2)

- Menu → core → cena aditiva correta conforme minijogo escolhido.
- `OnRegister` recebe **`MinigameServices`** populado (não `Empty` em play mode).
- Minijogos cacheiam o saco e usam pelo menos **`IInputRouter`** ou **`IAudioDirector`** num caminho real (ex.: grab ou cue).
- Partida inicia **só** via `IMinigame.OnMatchBegin` (sem `OfflineQuickStart` duplicado).
- Rodadas disparam `OnRoundBegin` / `OnRoundEnd` quando aplicável.
- Fim de partida: `OnMatchEnd` → resultados com score em prefs; unload da cena aditiva.
- Voltar ao menu desde resultados/leaderboard sem cena aditiva órfã na hierarquia.
- Trocar Blitz ↔ Fantasma no menu altera a cena aditiva na próxima entrada (sem editar core).

---

## 10. Fora de âmbito desta fase

- NGO / `NetMatchSession` com minijogos aditivos (saco pode ser reutilizado no host mais tarde)
- Pooling avançado em `IPrefabSpawner`, mixers de áudio, Addressables
- `IPlayerVisualRegistry` com avatares reais (só null-object nesta fase)
- Lobby `20_Lobby.unity`
- Leaderboard JSON (plano separado)

---

## Ficheiros principais a criar/alterar

| Ação | Ficheiro |
|------|----------|
| Criar | `MinigameDescriptor.cs`, `MinigameCatalog.cs` |
| Criar | `IAudioDirector`, `IInputRouter`, `IPrefabSpawner`, `IPlayerVisualRegistry` + implementações mínimas |
| Criar | `MinigameServicesHost.cs`; evoluir `MinigameServices` em `IMinigame.cs` |
| Criar | `MinigameLoader.cs`, `OfflineMinigameOrchestrator.cs` |
| Alterar | `BlitzOnomatopoeicoMinigame.cs`, `FantasmaLadraoMinigame.cs` (`OnRegister` cacheia serviços) |
| Alterar | `SceneFlow.cs`, `SceneNames.cs`, `GameSessionPrefs.cs` |
| Alterar | `LocalMatchSession.cs`, `IMatchSession.cs` |
| Alterar | `MainMenuView.cs` (+ UXML se necessário) |
| Remover/desativar | `OfflineQuickStart` na cena core |
| Cenas | Split `30_Gameplay_Offline` → core + `31_*` / `32_*` |
| Config | `EditorBuildSettings.asset` |
