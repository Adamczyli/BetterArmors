# BetterArmors

**BetterArmors** to plugin dla [EXILED](https://github.com/Exiled-Team/EXILED) do SCP: Secret Laboratory, który sprawia, że zbroje (armor) dają dodatkowe HP graczowi, zamiast AHP. Po zdjęciu zbroi HP jest odbierane. Jeśli gracz straci HP poniżej wartości dodanej przez zbroję, ta zostaje automatycznie zniszczona.

---

## Funkcje

-  Dodawanie HP po założeniu armoru.
-  Odbieranie HP po zdjęciu armoru.
-  Automatyczne niszczenie armoru, gdy gracz spadnie poniżej „dodatkowego” HP.

---

## ⚙️ Konfiguracja (`.yml`)

```yaml
is_enabled: true
debug: false
light_armor_hp: 25
combat_armor_hp: 50
heavy_armor_hp: 75
