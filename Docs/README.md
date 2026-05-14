# Documentação — Blitz Onomatopoeico

Party game educativo por **sessões**: mesa com **três** objetos sonoros, **cartas** com letra + par figura/som, e janela de **grab** + fala (sem ASR). O detalhe de arquitetura vive no blueprint do repositório (`.cursor/plans/blitz_onomatopoeico_architecture_d4df416d.plan.md`).

## Públicos e manuais

| Papel | Manual | Foco |
|--------|--------|------|
| Qualquer pessoa nova no repo | [Getting Started](Getting-Started.md) | Requisitos, abrir o projeto, primeiro play offline, mapa de pastas |
| **Designer (conteúdo / UI)** | [Manual do Designer](Manual-Designer.md) | Art, áudio, prefabs, UI Toolkit (UXML/USS), mesa 3 slots, lobby na UI |
| **Game Designer** | [Manual do Game Designer](Manual-Game-Designer.md) | Regras, loop de jogo, fases, dificuldade, MP em UX, specs para dev, playtest |
| **Programador** | [Manual do Programador](Manual-Programador.md) | Assemblies, Core/Gameplay/UI/Netcode, match flow, NGO, testes, extensão |

**Importante:** “Designer” neste índice = produção visual e montagem em Unity. “Game Designer” = sistemas e experiência de jogo (GDD). Não confundir os dois.

## Versão do Unity

Ver `ProjectSettings/ProjectVersion.txt` no repositório (referência atual: **6000.4.x**).

## Ordem de leitura sugerida

1. [Getting Started](Getting-Started.md)  
2. O manual do teu papel (Designer, Game Designer ou Programador)  
3. Blueprint de arquitetura, quando precisares de profundidade técnica ou de decisões antigas
