# ChronoGit

GUI pro `git rebase --interactive`. Obvyklé použití je úklid
větve před mergnutím do mastera.

## Funkcionalita

Samozřejmě podpora funkcí interaktivního rebasu jako takového,
tedy `pick`, `reword`, `edit`, `squash`, `fixup`, `exec`, `break`,
`drop`, `label`, `reset` a `merge`.

Dále přesouvání příkazů i skupin příkazů, undo/redo.
Možnost obarvování commitů - podle Autora / podle commit hlášky.
Globální operace nad commity - `exec` po každém (změněném) commitu,
`break` po každém commitu.

## Alternativy

Jednou z alternativ je přímo editovat přímo textový soubor s příkazy.
Lepší by bylo použít tento [nástroj](https://github.com/MitMaro/git-interactive-rebase-tool).
Oproti té GUI verze hlavně bude příkazy zobrazovat přehledněji,
navíc nabídne pohodlnější globální operace.

## Technologie

- GUI: Avalonia + ReactiveUI
- Git: LibGit2Sharp

## Features pokročilého C#

Jednotlivé akce dává smysl reprezentovat pomocí seznamu příkazů.
Nicméně na různých příkazech dává smysl provádět úplně různé věci
na stejnou klávesovou zkratku. Např.:
- U `pick` chceme změnit název commitu, u `exec` změnit spouštěný příkaz.
- `pick` chceme změnit na `drop`, zatímco `break` chceme úplně smazat.
