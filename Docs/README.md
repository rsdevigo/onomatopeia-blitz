# Documentação — Blitz Onomatopoeico

Party game educativo em **sessões**: mesa com **três** objetos sonoros, **cartas** com letra + par figura/som, e janela de **grab** + fala (sem ASR). O detalhe de arquitetura está no blueprint do repositório (`.cursor/plans/blitz_onomatopoeico_architecture_d4df416d.plan.md`).

## Públicos e manuais

| Papel | Manual | Foco |
|--------|--------|------|
| Qualquer pessoa nova no repositório | [Primeiros passos](Getting-Started.md) | Requisitos, abrir o projeto, primeiro play offline, mapa de pastas |
| **Designer (conteúdo / UI)** | [Manual do Designer](Manual-Designer.md) | Arte, áudio, prefabs, UI Toolkit (UXML/USS), mesa com 3 slots, lobby na UI |
| **Artista / produção de assets** | [Lista de assets — artistas](Lista-Assets-Artistas.md) | Checklist Props, UI, SFX, VFX, shaders e prioridades P0–P2 |
| **Game Designer** | [Manual do Game Designer](Manual-Game-Designer.md) | Regras, loop de jogo, fases, dificuldade, multijogador na UX, specs para dev, playtest |
| **Programador** | [Manual do Programador](Manual-Programador.md) | Assemblies, Core/Gameplay/UI/Netcode, fluxo de match, NGO, testes, extensão |
| **Conteúdo / configuração de dados** | [Manual de ScriptableObjects](Manual-ScriptableObjects.md) | Criar catálogos de onomatopeia e minijogo, ligar na cena core |

**Importante:** neste índice, “Designer” = produção visual e montagem no Unity. “Game Designer” = sistemas e experiência de jogo (GDD). Não confunda os dois.

## Versão do Unity

Consulte `ProjectSettings/ProjectVersion.txt` no repositório (referência atual: **6000.4.x**).

## Ordem de leitura sugerida

1. [Primeiros passos](Getting-Started.md)  
2. O manual do seu papel (Designer, Game Designer ou Programador)  
3. Blueprint de arquitetura, quando precisar de profundidade técnica ou de decisões antigas
