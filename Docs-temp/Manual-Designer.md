# Manual do Designer (conteúdo / UI)

Manual para **produção visual**, **UI Toolkit** e **montagem de cenas/prefabs** no Unity. Não é obrigatório saber C#; basta saber arrastar referências e respeitar convenções de pastas.

## 1. Fluxo de trabalho sugerido

1. **Ideia** (mockup ou referência) →  
2. **Asset** (sprite, mesh, áudio) nas pastas certas →  
3. **Prefab** (variante reutilizável) →  
4. **Cena** (instâncias, iluminação, câmera, UI).

### Pastas (convenção do projeto)

| Tipo | Pasta |
|------|--------|
| Meshes, materiais, sprites de tema | `Assets/Art/` |
| Clips de fonema, SFX, música | `Assets/Audio/` |
| Prefabs core (bootstrap, app) | `Assets/Prefabs/Core/` |
| Mesa, objetos sonoros, âncoras | `Assets/Prefabs/Table/` |
| Variantes por minijogo | `Assets/Prefabs/Minigames/BlitzOnomatopoeico/`, `FantasmaLadraoSons/` |
| UI “host” em prefab (se precisar) | `Assets/Prefabs/UI/` |

Mantenha nomes **estáveis** (sem renomear GUIDs à toa) para não quebrar referências na equipe.

## 2. Mesa de três objetos (slots 0, 1, 2)

### Papel de design

- Existem **exatamente três** alvos físicos na mesa.
- Cada alvo corresponde a um **índice de slot** `0`, `1` ou `2`. O código e o resolver falam sempre nesse vocabulário.

### Componentes que você vai mexer

- **`SoundObjectInstance`** (`Assets/Scripts/Gameplay/Table/SoundObjectInstance.cs`)  
  - Campo **slot index** (0–2): identifica o objeto no mundo.  
  - Exige **Collider** (ou filho com collider) para o raycast do grab.
- **`TableRuntimeRegistry`** (na mesma cena)  
  - Recebe o registro dos `SoundObjectInstance` quando ficam ativos.  
  - Faz o **raycast** a partir da câmera quando o jogador clica (via `OfflineGrabInputDriver`).

### Boas práticas

- Um objeto por slot; **não** duplique o mesmo índice em dois objetos ativos.
- Colliders **não** como trigger, salvo decisão explícita do GD (afeta `Physics.Raycast` no registry).
- Mantenha os três objetos **legíveis** na câmera (silhueta, cor, distância).

## 3. UI Toolkit (UXML e USS)

### Onde estão os arquivos

- **UXML:** `Assets/UI Toolkit/UXML/` (subpastas: `MainMenu/`, `Lobby/`, `HUD/`, `Results/`, `Leaderboard/`, `Common/`).
- **USS:** `Assets/UI Toolkit/USS/` (`Theme.uss`, `Components/`).

### Como ligar à cena

1. Crie um **GameObject** com `UIDocument`.
2. No mesmo objeto (ou filho), adicione o script de **View** correspondente, por exemplo:
   - `MainMenuView` → associe o **Visual Tree Asset** exportado do `MainMenu.uxml`.
   - `HudView` → idem para `HUD.uxml`.
   - `LobbyView` → `Lobby.uxml` **e** o template `LobbySeatRow.uxml` no campo do template de linha.
3. Garanta que exista **Panel Settings** válido (o Unity cria por padrão em muitos templates; se a UI não renderizar, verifique o `Panel Settings` no `UIDocument`).

### Estilo

- Tokens globais em `Theme.uss`; estilos de componente em `USS/Components/`.
- Nos UXML, os `<Style src="..."/>` usam caminhos **relativos** ao arquivo UXML — se mover pastas, atualize os `src`.

## 4. Lobby (oito lugares) — visão UI

- Fluxo de **simulação local** e toggles **Pronto** estão ligados ao **`LobbyServiceHost`** (`Assets/Scripts/Gameplay/Lobby/LobbyServiceHost.cs`): é o objeto que você arrasta para a cena e referencia no `LobbyView`.
- O UXML principal: `Assets/UI Toolkit/UXML/Lobby/Lobby.uxml`.  
- Cada linha vem do template `LobbySeatRow.uxml` (o `LobbyPresenter` instancia 8 linhas em runtime).

Você não precisa configurar rede para testar o shell: o stub preenche lugares após o botão “Simular”.

## 5. Minijogos (visão de arte / cena)

Existem dois exemplos no código (nomes de classe):

- **`BlitzOnomatopoeicoMinigame`** — mesa “blitz” com carta e objetos genéricos.
- **`FantasmaLadraoMinigame`** — variante com **adaptador** de espaço de solução (mesma lógica de resposta, outro tipo de alvo no mundo).

Em termos de **entrega**:

- Cada minijogo deve ter **cena própria** (ou aditiva) com iluminação e props; o programador define o fluxo de carga — você foca em **prefabs** e **âncoras** nomeadas de forma clara (`Slot_0` … `CardMount`, etc., conforme combinado com a equipe).

Detalhe técnico de `IMinigame` e adaptadores: veja [Manual do Programador](Manual-Programador.md).

## 6. Checklist antes de entregar uma cena

- [ ] **Câmera** ativa, clear flags e depth corretos para o seu URP.
- [ ] **Event System** se usar interação UI com mouse/teclado além do grab (depende da cena).
- [ ] **UIDocument** com `VisualTreeAsset` preenchido em **todas** as views usadas.
- [ ] **TableRuntimeRegistry** presente e único.
- [ ] Três **`SoundObjectInstance`** com slots 0–2 e colliders.
- [ ] **OfflineGrabInputDriver** (se teste de grab) com referências válidas ou objetos únicos na cena para `FindAnyObjectByType`.
- [ ] Layers: objetos de mesa numa layer que a câmera consiga “ver” no raycast (o default costuma bastar).
- [ ] Nenhum material/rosa missing — verifique referências quebradas no Inspector antes do PR.

## 7. Onde aprofundar

- [Primeiros passos](Getting-Started.md) — primeiro arranque e smoke test.  
- [Manual de ScriptableObjects](Manual-ScriptableObjects.md) — criar definitions de som/figura e catálogos.  
- [Manual do Game Designer](Manual-Game-Designer.md) — regras e pacing.  
- [Manual do Programador](Manual-Programador.md) — extensão e integração.  
- Blueprint: `.cursor/plans/blitz_onomatopoeico_architecture_d4df416d.plan.md`.
