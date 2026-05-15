# Manual do Programador

Arquitetura **Blitz** em assemblies, fluxo de **match/rodada**, **UI** desacoplada, **NGO** e **testes**. Convenção de namespaces: `Blitz.*`.

## 1. Assemblies (`asmdef`) e dependências

| Assembly | Arquivo | Referências principais | Responsabilidade |
|----------|---------|------------------------|------------------|
| **Blitz.Core** | `Assets/Scripts/Core/Blitz.Core.asmdef` | Nenhuma (sem `UnityEngine`) | DTOs, regras puras, RNG/contratos, interfaces sem MonoBehaviour |
| **Blitz.Gameplay** | `Assets/Scripts/Gameplay/Blitz.Gameplay.asmdef` | `Blitz.Core`, `Unity.InputSystem` | MonoBehaviours, mesa, input de grab, sessão offline, minijogos, host do lobby |
| **Blitz.UI** | `Assets/Scripts/UI/Blitz.UI.asmdef` | `Blitz.Core`, `Blitz.Gameplay` | UI Toolkit views/presenters/viewmodels — não chame `Physics.Raycast` daqui |
| **Blitz.Netcode** | `Assets/Scripts/Networking/NGO/Blitz.Netcode.asmdef` | `Blitz.Core`, `Blitz.Gameplay`, `Unity.Netcode.Runtime`, `Unity.Netcode.GameObjects` | `NetworkBehaviour`, RPCs, variáveis replicadas |
| **Blitz.App** | `Assets/Scripts/App/Blitz.App.asmdef` | `Blitz.Core`, `Blitz.Gameplay`, `Blitz.UI`, `Blitz.Netcode` | Composição / bootstrap (ex.: `OfflineQuickStart`) |

**Direção permitida:** `Core` ← ninguém de “jogo”; `Gameplay` → `Core`; `UI` → `Core` + `Gameplay` (para `IMatchSession`, etc.); `Netcode` → `Core` + `Gameplay` + pacotes NGO; `App` → todos.

**Evite:** referenciar `Blitz.UI` ou `Blitz.Netcode` a partir de `Blitz.Core` (quebra a separação).

## 2. Domínio de cartas e mesa (`Blitz.Core`)

Arquivos-chave em `Assets/Scripts/Core/`:

- **`Identifiers.cs`** — `LetterId`, `PhonemeId`, `SoundObjectId` (slot 0–2).
- **`CardModels.cs`** — `GeneratedCard`, `CardMode` (`HasTruePair` vs `ExclusionMismatch`), `CardPresentationPair`.
- **`ActiveLetterSoundSet.cs`** — três letras com `T(L)`, permutação de fonemas nos slots.
- **`AnswerResolver.cs`** / **`IAnswerResolver.cs`** — resposta canônica para `(carta, mesa)`.
- **`CardGenerator.cs`** — gera par carta+mesa com retries.
- **`CardUniqueness.cs`** — invariantes de unicidade (usado pelo gerador e pelos testes).
- **`MatchModels.cs`** — `MatchRules`, `MatchPhase`, `RoundOutcome`.
- **Lobby (contrato + stub)** — `ILobbyService`, `LobbyServiceStub`, modelos de seat em `Assets/Scripts/Core/`.

### Testes EditMode

- Assembly: `Assets/Tests/EditMode/Core/Blitz.Core.Tests.asmdef`
- Exemplo: `AnswerResolverTests.cs` — cobre positivo, exclusão e propriedades do gerador em várias seeds.

Rodar no Unity: **Window → General → Test Runner → EditMode**.

## 3. Sessão e rodadas (`Blitz.Gameplay`)

### Contrato

- **`IMatchSession`** (`Assets/Scripts/Gameplay/Match/IMatchSession.cs`) — fase, pontos, carta ativa, `TrySubmitGrab`, `Tick`, evento `StateChanged`.

### Implementação offline

- **`LocalMatchSession`** — `MonoBehaviour` + `Update` chama `Tick`; delega em `RoundController`.
- **`RoundController`** — máquina de estados por fase (`MatchInit` → … → `MatchEnd`); gera carta via `CardGenerator`; resolve com `AnswerResolver`; dispara `RoundResolved`.

### Mesa e grab

- **`TableRuntimeRegistry`** — registro de `SoundObjectInstance`, `TryRaycastGrab(Camera, screenPos, out SoundObjectId)`.
- **`SoundObjectInstance`** — `slotIndex` 0–2; `OnEnable` registra no registry (procura `TableRuntimeRegistry` na cena/parent).
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

- **Padrão:** View (`MonoBehaviour` + `UIDocument`) → **Presenter** (liga UI a VM/serviços) → **ViewModel** (estado mínimo, estilo `INotify` onde couber).
- Pastas: `Views/`, `Presenters/`, `ViewModels/`, `Binding/UiBind.cs` (agendamento de refresh leve).
- **Regra:** presenters assinam `IMatchSession.StateChanged` (ou equivalente), **não** disparam física.

## 5. Rede — NGO (`Blitz.Netcode`)

- **`NetMatchSession`** (`Assets/Scripts/Networking/NGO/NetMatchSession.cs`) — `NetworkBehaviour`:
  - `NetworkVariable<byte>` fase, `NetworkVariable<int>` pontuação.
  - `SubmitGrabRpc` — `[Rpc(SendTo.Server)]`; valida contra carta/servidor e atualiza estado **no servidor**.
  - `ServerBeginStubRound` — helper para gerar carta no servidor (testes / host).

### Checklist NGO

1. No mesmo GameObject: **`NetworkObject`** + `NetMatchSession`.
2. Prefab registrado na lista de **Network Prefabs** do projeto (conforme versão NGO / NetworkManager).
3. Não escreva `NetworkVariable` a partir do cliente sem API de servidor — mantenha autoridade clara.

### Limitações atuais (para roadmap)

- Replicação compacta da **carta inteira** para clientes pode ser incremental (hash + reconstrução, ou `INetworkSerializable` numa camada que não polua o `Core`).

## 6. Extensão: novo minijogo

1. Implemente **`IMinigame`** (ciclo de vida: registro, cena carregada, início/fim de match e rodada).
2. Se o espaço de picks no mundo **não** for 1:1 com slots 0–2, forneça **`ISolutionSpaceAdapter`** (ver Fantasma).
3. Coloque cena aditiva e prefabs em `Assets/Prefabs/Minigames/...` conforme convenção.
4. (Futuro) `MinigameDescriptor` ScriptableObject + loader — ainda conceitual no blueprint; até lá, documente o nome da cena no PR.

## 7. Convenções do repositório

- **Blueprint** (`.cursor/plans/blitz_onomatopoeico_architecture_d4df416d.plan.md`) — decisão arquitetural; não edite sem alinhamento da equipe.
- **Documentação de usuário** — `Docs/` (este conjunto de manuais).
- **PRs:** descreva impacto em `Core` (regras) vs `Gameplay` (comportamento Unity) vs `Netcode` (RPCs).

## 8. Onde aprofundar

- [Primeiros passos](Getting-Started.md)  
- [Manual do Designer](Manual-Designer.md)  
- [Manual do Game Designer](Manual-Game-Designer.md)  
- [README dos Docs](README.md)
