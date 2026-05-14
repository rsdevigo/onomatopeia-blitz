# Manual do Programador

Arquitetura **Blitz** em assemblies, fluxo de **match/ronda**, **UI** desacoplada, **NGO** e **testes**. Convenção de namespaces: `Blitz.*`.

## 1. Assemblies (`asmdef`) e dependências

| Assembly | Ficheiro | Referências principais | Responsabilidade |
|----------|----------|------------------------|------------------|
| **Blitz.Core** | `Assets/Scripts/Core/Blitz.Core.asmdef` | Nenhuma (sem `UnityEngine`) | DTOs, regras puras, RNG/contracts, interfaces sem MonoBehaviour |
| **Blitz.Gameplay** | `Assets/Scripts/Gameplay/Blitz.Gameplay.asmdef` | `Blitz.Core`, `Unity.InputSystem` | MonoBehaviours, mesa, input grab, sessão offline, minijogos, lobby host |
| **Blitz.UI** | `Assets/Scripts/UI/Blitz.UI.asmdef` | `Blitz.Core`, `Blitz.Gameplay` | UI Toolkit views/presenters/viewmodels — não fazer `Physics.Raycast` daqui |
| **Blitz.Netcode** | `Assets/Scripts/Networking/NGO/Blitz.Netcode.asmdef` | `Blitz.Core`, `Blitz.Gameplay`, `Unity.Netcode.Runtime`, `Unity.Netcode.GameObjects` | `NetworkBehaviour`, RPCs, variáveis replicadas |
| **Blitz.App** | `Assets/Scripts/App/Blitz.App.asmdef` | `Blitz.Core`, `Blitz.Gameplay`, `Blitz.UI`, `Blitz.Netcode` | Composição / bootstrap (ex.: `OfflineQuickStart`) |

**Direção permitida:** `Core` ← ninguém de jogos; `Gameplay` → `Core`; `UI` → `Core` + `Gameplay` (para `IMatchSession`, etc.); `Netcode` → `Core` + `Gameplay` + pacotes NGO; `App` → todos.

**Não fazer:** referenciar `Blitz.UI` ou `Blitz.Netcode` desde `Blitz.Core` (quebra a separação).

## 2. Domínio de cartas e mesa (`Blitz.Core`)

Ficheiros-chave em `Assets/Scripts/Core/`:

- **`Identifiers.cs`** — `LetterId`, `PhonemeId`, `SoundObjectId` (slot 0–2).
- **`CardModels.cs`** — `GeneratedCard`, `CardMode` (`HasTruePair` vs `ExclusionMismatch`), `CardPresentationPair`.
- **`ActiveLetterSoundSet.cs`** — três letras com `T(L)`, permutação de fonemas nos slots.
- **`AnswerResolver.cs`** / **`IAnswerResolver.cs`** — resposta canónica para `(carta, mesa)`.
- **`CardGenerator.cs`** — gera par carta+mesa com retries.
- **`CardUniqueness.cs`** — invariantes de unicidade (usado pelo gerador e testes).
- **`MatchModels.cs`** — `MatchRules`, `MatchPhase`, `RoundOutcome`.
- **Lobby (contrato + stub)** — `ILobbyService`, `LobbyServiceStub`, modelos de seat em `Assets/Scripts/Core/`.

### Testes EditMode

- Assembly: `Assets/Tests/EditMode/Core/Blitz.Core.Tests.asmdef`
- Exemplo: `AnswerResolverTests.cs` — cobre positivo, exclusão e propriedades do gerador em várias seeds.

Correr no Unity: **Window → General → Test Runner → EditMode**.

## 3. Sessão e rondas (`Blitz.Gameplay`)

### Contrato

- **`IMatchSession`** (`Assets/Scripts/Gameplay/Match/IMatchSession.cs`) — fase, pontos, carta ativa, `TrySubmitGrab`, `Tick`, evento `StateChanged`.

### Implementação offline

- **`LocalMatchSession`** — `MonoBehaviour` + `Update` chama `Tick`; delega em `RoundController`.
- **`RoundController`** — máquina de estados por fase (`MatchInit` → … → `MatchEnd`); gera carta via `CardGenerator`; resolve com `AnswerResolver`; dispara `RoundResolved`.

### Mesa e grab

- **`TableRuntimeRegistry`** — registo de `SoundObjectInstance`, `TryRaycastGrab(Camera, screenPos, out SoundObjectId)`.
- **`SoundObjectInstance`** — `slotIndex` 0–2; `OnEnable` regista no registry (procura `TableRuntimeRegistry` na cena/parent).
- **`OfflineGrabInputDriver`** — `GrabPhase` + clique esquerdo (`Mouse.current`) + `TrySubmitGrab`.

### Feedback

- **`GameplayFeedbackBus`** (`Assets/Scripts/Gameplay/Feedback/GameplayFeedbackBus.cs`) — evento simples para VFX/UI futuros.

### Lobby (host MonoBehaviour)

- **`LobbyServiceHost`** — implementa `ILobbyService` delegando em `LobbyServiceStub` (útil para serialização no Inspector das views).

### Minijogos

- **`IMinigame`**, **`MinigameServices`**, **`MatchConfig`** — `Assets/Scripts/Gameplay/Minigames/IMinigame.cs`.
- **`BlitzOnomatopoeicoMinigame`**, **`FantasmaLadraoMinigame`** — exemplos; Fantasma expõe `TrySubmitWorldGrab` com `ISolutionSpaceAdapter`.
- **`ISolutionSpaceAdapter`** / **`IdentitySolutionSpaceAdapter`** — mapear pick do mundo ↔ slot core.

## 4. UI (`Blitz.UI`)

- **Padrão:** View (`MonoBehaviour` + `UIDocument`) → **Presenter** (liga UI a VM/serviços) → **ViewModel** (estado mínimo, `INotify`-style onde aplicável).
- Pastas: `Views/`, `Presenters/`, `ViewModels/`, `Binding/UiBind.cs` (schedule de refresh leve).
- **Regra:** presenters subscrevem `IMatchSession.StateChanged` (ou equivalente), **não** disparam física.

## 5. Rede — NGO (`Blitz.Netcode`)

- **`NetMatchSession`** (`Assets/Scripts/Networking/NGO/NetMatchSession.cs`) — `NetworkBehaviour`:
  - `NetworkVariable<byte>` fase, `NetworkVariable<int>` pontuação.
  - `SubmitGrabRpc` — `[Rpc(SendTo.Server)]`; valida contra carta/servidor e atualiza estado **no servidor**.
  - `ServerBeginStubRound` — helper para gerar carta no servidor (testes / host).

### Checklist NGO

1. Mesmo GameObject: **`NetworkObject`** + `NetMatchSession`.
2. Prefab registado na lista de **Network Prefabs** do projeto (conforme versão NGO / NetworkManager).
3. Não escrever `NetworkVariable` a partir de cliente sem API servidor — mantém autoridade clara.

### Limitações atuais (para roadmap)

- Replicação compacta de **carta inteira** para clientes pode ser incremental (hash + reconstrução, ou `INetworkSerializable` numa camada que não polua `Core`).

## 6. Extensão: novo minijogo

1. Implementar **`IMinigame`** (ciclo de vida: registo, cena carregada, início/fim de match e ronda).
2. Se o espaço de picks no mundo **não** for 1:1 com slots 0–2, fornecer **`ISolutionSpaceAdapter`** (ver Fantasma).
3. Colocar cena aditiva e prefabs sob `Assets/Prefabs/Minigames/...` conforme convenção.
4. (Futuro) `MinigameDescriptor` ScriptableObject + loader — ainda conceptual no blueprint; até lá, documenta o nome da cena no PR.

## 7. Convenções de repo

- **Blueprint** (`.cursor/plans/blitz_onomatopoeico_architecture_d4df416d.plan.md`) — decisão arquitetural; não editar sem acordo da equipa.
- **Documentação de utilizador** — `Docs/` (este conjunto de manuais).
- **PRs:** descreve impacto em `Core` (regras) vs `Gameplay` (comportamento Unity) vs `Netcode` (RPCs).

## 8. Onde aprofundar

- [Getting Started](Getting-Started.md)  
- [Manual do Designer](Manual-Designer.md)  
- [Manual do Game Designer](Manual-Game-Designer.md)  
- [README dos Docs](README.md)
