# Sprawozdanie końcowe — Tower Defense

| | |
|---|---|
| **Przedmiot** | Teoria Gier — Semestr 8 |
| **Typ gry** | Tower Defense (2D) |
| **Silnik** | Unity (Universal Render Pipeline 2D) |
| **Język** | C# |
| **Data** | 12.05.2026 |

---

## Spis treści

- [Sprawozdanie końcowe — Tower Defense](#sprawozdanie-końcowe--tower-defense)
  - [Spis treści](#spis-treści)
  - [1. Opis projektu](#1-opis-projektu)
  - [2. Cel gry i mechaniki rozgrywki](#2-cel-gry-i-mechaniki-rozgrywki)
  - [3. Architektura projektu](#3-architektura-projektu)
    - [3.1 Struktura folderów](#31-struktura-folderów)
    - [3.2 Schemat zależności (high-level)](#32-schemat-zależności-high-level)
  - [4. Implementacja — przeciwnicy](#4-implementacja--przeciwnicy)
    - [4.1 EnemyData (ScriptableObject)](#41-enemydata-scriptableobject)
    - [4.2 Tabela 1. Rodzaje przeciwników](#42-tabela-1-rodzaje-przeciwników)
    - [4.3 Logika klasy `Enemy.cs`](#43-logika-klasy-enemycs)
  - [5. Implementacja — system fal](#5-implementacja--system-fal)
    - [5.1 WaveData (ScriptableObject)](#51-wavedata-scriptableobject)
    - [5.2 Tabela 2. Konfiguracja fal bazowych](#52-tabela-2-konfiguracja-fal-bazowych)
    - [5.3 Spawner — kluczowe elementy logiki](#53-spawner--kluczowe-elementy-logiki)
  - [6. Implementacja — ścieżka i pula obiektów](#6-implementacja--ścieżka-i-pula-obiektów)
    - [6.1 Path.cs](#61-pathcs)
    - [6.2 ObjectPooler.cs](#62-objectpoolercs)
  - [7. Implementacja — wieże i pociski](#7-implementacja--wieże-i-pociski)
    - [7.1 TowerData (ScriptableObject)](#71-towerdata-scriptableobject)
    - [7.2 Tabela 3. Rodzaje wież](#72-tabela-3-rodzaje-wież)
    - [7.3 Tower.cs — detekcja celów](#73-towercs--detekcja-celów)
    - [7.4 Projectile.cs](#74-projectilecs)
  - [8. Implementacja — platformy i stawianie wież](#8-implementacja--platformy-i-stawianie-wież)
    - [8.1 Platform.cs](#81-platformcs)
    - [8.2 TowerCard.cs](#82-towercardcs)
    - [8.3 Wpływ na rozgrywkę](#83-wpływ-na-rozgrywkę)
  - [9. GameManager i ekonomia gracza](#9-gamemanager-i-ekonomia-gracza)
  - [10. System poziomów (LevelData / LevelManagment)](#10-system-poziomów-leveldata--levelmanagment)
    - [10.1 LevelData (ScriptableObject)](#101-leveldata-scriptableobject)
    - [10.2 Tabela 4. Dostępne poziomy](#102-tabela-4-dostępne-poziomy)
    - [10.3 LevelManagment](#103-levelmanagment)
  - [11. Interfejs użytkownika](#11-interfejs-użytkownika)
    - [11.1 Stale wyświetlane informacje (HUD)](#111-stale-wyświetlane-informacje-hud)
    - [11.2 Panele](#112-panele)
    - [11.3 Komunikaty kontekstowe](#113-komunikaty-kontekstowe)
    - [11.4 Karty wież](#114-karty-wież)
  - [12. Sterowanie czasem i pauza](#12-sterowanie-czasem-i-pauza)
  - [13. Stany gry: porażka, sukces, tryb endless](#13-stany-gry-porażka-sukces-tryb-endless)
    - [13.1 Game Over](#131-game-over)
    - [13.2 Mission Complete](#132-mission-complete)
    - [13.3 Endless Mode](#133-endless-mode)
  - [14. Zastosowane wzorce projektowe](#14-zastosowane-wzorce-projektowe)
  - [15. Plan dalszego rozwoju](#15-plan-dalszego-rozwoju)
  - [16. Inspiracje i odnośniki](#16-inspiracje-i-odnośniki)

---

## 1. Opis projektu

Projekt jest grą z gatunku **Tower Defense** w widoku 2D z góry. Gracz zarządza obroną bazy, stawiając wieże na wyznaczonych platformach wzdłuż ścieżki, którą podążają wrogowie. Celem każdego poziomu jest przetrwanie określonej liczby fal — gdy wszystkie fale zostaną odparte, gracz otrzymuje ekran zwycięstwa z możliwością przejścia do kolejnego poziomu lub kontynuowania w trybie nieskończonym. Gdy życie gracza spadnie do zera, pojawia się ekran *Game Over*.

Gra zaprojektowana została z myślą o łatwej rozszerzalności — wszystkie kluczowe dane (statystyki wrogów, fal, wież oraz poziomów) opakowane są w **ScriptableObjects**, co pozwala dodawać nowe warianty bez modyfikacji kodu.

---

## 2. Cel gry i mechaniki rozgrywki

**Cel gracza:** Obronić bazę przed wszystkimi falami wrogów, którzy próbują przejść z punktu startowego do końca ścieżki.

**Główne mechaniki:**

- Wrogowie poruszają się po z góry ustalonych waypointach.
- Gracz zaczyna z określoną liczbą zasobów (zależną od poziomu) i może je wydawać na zakup wież.
- Wieże automatycznie atakują wrogów, którzy znajdują się w ich zasięgu.
- Za każdego zabitego wroga gracz otrzymuje nagrodę w zasobach.
- Każdy wróg, który dotrze do końca ścieżki, odbiera graczowi życie (wartość zależna od typu wroga).
- Trudność rośnie wraz z numerem fali — HP wrogów jest skalowane.
- Gracz może regulować prędkość gry (×0.5, ×1, ×2) lub ją zapauzować.

---

## 3. Architektura projektu

### 3.1 Struktura folderów

```
Assets/
├── Art/                                 — grafiki, sprite sheety, tła
├── Prefabs/                             — prefaby wrogów, wież, pocisków, UI
│   ├── BurstTower.prefab, FastTower.prefab, StrongTower.prefab
│   ├── Fly.prefab, Goblin.prefab, HogRider.prefab
│   ├── Projectile.prefab
│   └── Waypoint.prefab
├── Scenes/
│   ├── MainMenu.unity
│   ├── LevelOne.unity
│   └── LevelTwo.unity
├── ScriptableObject/
│   ├── Enemy/        (EnemyData.cs + 3 assety)
│   ├── Level/        (LevelData.cs + 2 assety)
│   ├── Tower/        (TowerData.cs + 3 assety)
│   └── Waves/        (WaveData.cs + 3 assety)
├── Scripts/
│   ├── Enemy.cs
│   ├── GameManager.cs
│   ├── LevelManagment.cs
│   ├── MainMenuController.cs
│   ├── ObjectPooler.cs
│   ├── Path.cs
│   ├── Spawner.cs
│   ├── UIController.cs
│   ├── Tower/
│   │   ├── Platform.cs
│   │   ├── Projectile.cs
│   │   ├── Tower.cs
│   │   └── TowerCard.cs
│   └── Utils/
│       └── EnemyType.cs
└── TextMesh Pro/                        — zasoby UI
```

### 3.2 Schemat zależności (high-level)

```
                       ┌────────────────┐
                       │   MainMenu     │
                       └───────┬────────┘
                               ▼
                   ┌──────────────────────┐
                   │  LevelManagment      │  (singleton, DontDestroyOnLoad)
                   │  - levels[]          │
                   │  - currentLevelData  │
                   └──────────┬───────────┘
                              ▼ LoadScene
            ┌─────────────────┴────────────────────────┐
            │              Scene (LevelOne/Two)         │
            │                                           │
            │   ┌──────────────┐    ┌──────────────┐    │
            │   │ GameManager  │◄───┤   Enemy      │    │
            │   │ (singleton)  │    │  - events    │    │
            │   └─────┬────────┘    └──────┬───────┘    │
            │         │                    ▲            │
            │         │ events             │ damage     │
            │         ▼                    │            │
            │   ┌──────────────┐    ┌──────┴───────┐    │
            │   │ UIController │    │  Projectile  │    │
            │   └─────┬────────┘    └──────▲───────┘    │
            │         │                    │            │
            │         ▼ click               │ Shoot     │
            │   ┌──────────────┐    ┌──────┴───────┐    │
            │   │  Platform    │───►│   Tower      │    │
            │   └──────────────┘    └──────────────┘    │
            │                                           │
            │   ┌──────────────┐    ┌──────────────┐    │
            │   │   Spawner    │───►│ ObjectPooler │    │
            │   └──────────────┘    └──────────────┘    │
            └───────────────────────────────────────────┘
```

---

## 4. Implementacja — przeciwnicy

### 4.1 EnemyData (ScriptableObject)

```csharp
[CreateAssetMenu(fileName = "EnemyData", menuName = "Scriptable Objects/EnemyData")]
public class EnemyData : ScriptableObject
{
    public float lives;
    public float speed;
    public int damage;
    public float reward;
}
```

### 4.2 Tabela 1. Rodzaje przeciwników

| Wróg | Życie | Prędkość | Obrażenia | Nagroda |
|------|-------|----------|-----------|---------|
| **Goblin** | 3 | 2 | 1 | 1 |
| **Mucha (Fly)** | 2 | 4 | 1 | 2 |
| **Rider** | 10 | 4 | 2 | 3 |

- **Goblin** — wróg podstawowy, średnie statystyki, łatwy do pokonania.
- **Mucha** — najszybszy, ale najsłabszy; trudny do trafienia przez wolniejsze pociski.
- **Rider** — „czołg”, najwytrzymalszy i zadający największe obrażenia gdy dotrze do bazy.

### 4.3 Logika klasy `Enemy.cs`

Każdy przeciwnik posiada:

- **referencję do `EnemyData`** (statystyki),
- **dynamiczny pasek HP** jako dziecko prefabu, którego `localScale.x` jest aktualizowana proporcjonalnie do procentowego stanu zdrowia,
- **statyczne eventy** umożliwiające reakcję innych systemów:
  - `OnEnemyReachedEnd(EnemyData)` — wróg dotarł do końca ścieżki,
  - `OnEnemyRemoved(Enemy)` — wróg został zabity.

**Metoda otrzymywania obrażeń:**

```csharp
public void TakeDamage(float damage)
{
    _lives -= damage;
    _lives = Mathf.Max(0, _lives);
    UpdateHealthBar();
    if (_lives <= 0)
    {
        OnEnemyRemoved?.Invoke(this);
        gameObject.SetActive(false);
    }
}
```

**Inicjalizacja z mnożnikiem HP** (do skalowania trudności):

```csharp
public void Initilize(float healthMultiplier)
{
    _maxlives = data.lives * healthMultiplier;
    _lives = _maxlives;
    UpdateHealthBar();
}
```

---

## 5. Implementacja — system fal

### 5.1 WaveData (ScriptableObject)

```csharp
[CreateAssetMenu(fileName = "WaveData", menuName = "Scriptable Objects/WaveData")]
public class WaveData : ScriptableObject
{
    public EnemyType enemyType;
    public float spawnInterval;
    public int enemyPerWave;
}
```

### 5.2 Tabela 2. Konfiguracja fal bazowych

| Fala | Typ wroga | Liczba | Interwał spawnu |
|------|-----------|--------|-----------------|
| 1 | Goblin | 3 | 1.0 s |
| 2 | Mucha | 6 | 0.5 s |
| 3 | Rider | 4 | 2.0 s |

### 5.3 Spawner — kluczowe elementy logiki

- Spawner używa **słownika** `Dictionary<EnemyType, ObjectPooler>`, dzięki czemu dodanie nowego typu wroga sprowadza się do dopisania jednego wpisu.
- Po wyspawnowaniu wszystkich wrogów w fali oraz po ich zlikwidowaniu (przez śmierć lub dojście do końca ścieżki) uruchamiana jest **2-sekundowa przerwa** pomiędzy falami.
- Każda kolejna fala dodaje **+40% HP** do każdego przeciwnika (mnożnik `1f + waveCounter * 0.4f`).
- Po wszystkich falach (`totalWaves` z `LevelData`) spawner emituje event `OnMissionComplete`, który UI obsługuje pokazując panel zwycięstwa.
- Tryb **endless** wyłącza warunek końca misji — fale lecą w nieskończoność.

```csharp
if (_waveCounter + 1 >= LevelManagment.Instance.currentLevelData.totalWaves && !_isEndlessMode)
{
    OnMissionComplete?.Invoke();
    return;
}
```

---

## 6. Implementacja — ścieżka i pula obiektów

### 6.1 Path.cs

Ścieżka to tablica obiektów `GameObject` (waypointów) jako dzieci komponentu `Path`. Wrogowie podążają od waypointu do waypointu metodą `Vector3.MoveTowards()` (niezależnie od liczby klatek dzięki `Time.deltaTime`).

W edytorze ścieżka wizualizowana jest za pomocą `Gizmos` — szare linie łączą kolejne waypointy.

### 6.2 ObjectPooler.cs

Object pooling minimalizuje narzut na instancjonowanie obiektów w runtime — startowo tworzone jest 5 instancji każdego prefabu w stanie nieaktywnym. Pula jest wykorzystywana zarówno dla **wrogów**, jak i dla **pocisków**. Gdy pula się wyczerpie, kolejne obiekty są tworzone dynamicznie.

```csharp
public GameObject GetPooledObject()
{
    foreach (var obj in _pool)
        if (!obj.activeInHierarchy) return obj;
    return CreateNewObject();
}
```

---

## 7. Implementacja — wieże i pociski

### 7.1 TowerData (ScriptableObject)

```csharp
[CreateAssetMenu(fileName = "TowerData", menuName = "Scriptable Objects/TowerData")]
public class TowerData : ScriptableObject
{
    public float range;
    public float damage;
    public float fireRate;
    public float projectileSpeed;
    public float projectileDuration;
    public int cost;
    public Sprite sprite;
    public GameObject prefab;
}
```

### 7.2 Tabela 3. Rodzaje wież

| Wieża | Zasięg | Obrażenia | Fire Rate | Pręd. pocisku | Czas życia pocisku | Koszt |
|-------|--------|-----------|-----------|----------------|---------------------|-------|
| **FastTower** | 3.5 | 1 | 0.7 s | 10 | 2 s | **50** |
| **StrongTower** | 3 | 2 | 1 s | 4 | 3 s | **100** |
| **BurstTower** | 4 | 0.2 | 0.03 s | 8 | 0.5 s | **75** |

- **FastTower** — uniwersalna, dobry balans cena/jakość.
- **StrongTower** — wysoka obrażenia jednostkowe, świetna na Riderów.
- **BurstTower** — bardzo wysoka kadencja, niskie obrażenia jednostkowe — najlepsza na szybko poruszające się Muchy.

### 7.3 Tower.cs — detekcja celów

Wieża używa `CircleCollider2D` jako triggera o promieniu `range`. Kolizje z obiektami otagowanymi `"Enemy"` dodają je do listy `_enemiesInRange`. Zdarzenie `Enemy.OnEnemyRemoved` automatycznie usuwa zabite cele.

```csharp
private void OnTriggerEnter2D(Collider2D collision)
{
    if (collision.CompareTag("Enemy"))
        _enemiesInRange.Add(collision.GetComponent<Enemy>());
}

private void Shoot()
{
    if (_enemiesInRange.Count > 0)
    {
        GameObject projectileObj = _projectilePooler.GetPooledObject();
        projectileObj.transform.position = transform.position;
        projectileObj.SetActive(true);
        Vector2 dir = (_enemiesInRange[0].transform.position - transform.position).normalized;
        projectileObj.GetComponent<Projectile>().Shoot(data, dir);
    }
}
```

### 7.4 Projectile.cs

Pocisk leci po linii prostej z prędkością `projectileSpeed`, znika po upływie `projectileDuration` (zwracany do puli). W kolizji z wrogiem wywołuje `enemy.TakeDamage(damage)` i się dezaktywuje.

---

## 8. Implementacja — platformy i stawianie wież

### 8.1 Platform.cs

Platforma to konkretny punkt na mapie, na którym można postawić wieżę. Wykorzystuje raycast 2D na kliknięcie myszą:

```csharp
if (Mouse.current.leftButton.wasPressedThisFrame)
{
    Vector2 worldPoint = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
    RaycastHit2D hit = Physics2D.Raycast(worldPoint, Vector2.zero, Mathf.Infinity, platformLayerMask);
    if (hit.collider != null)
        OnPlatformClicked?.Invoke(hit.collider.GetComponent<Platform>());
}
```

Po kliknięciu platformy emitowane jest zdarzenie `OnPlatformClicked`, które UI łapie i pokazuje panel wyboru wieży.

### 8.2 TowerCard.cs

Każda karta wieży w panelu wyświetla **sprite**, **koszt** oraz po kliknięciu emituje event `OnTowerCardSelected(TowerData)`, który UI obsługuje:

1. Sprawdza, czy platforma jest pusta — jeśli nie, pokazuje komunikat „This platform is already occupied!”
2. Sprawdza, czy gracz stać na wieżę — jeśli nie, pokazuje „Not enough resources!”
3. Jeśli OK — odejmuje zasoby przez `GameManager.SpendResources()` i instancjonuje wieżę na platformie.

### 8.3 Wpływ na rozgrywkę

Otwarcie panelu wyboru wieży **zatrzymuje czas gry** (`Time.timeScale = 0`), żeby gracz mógł spokojnie wybrać. Po zamknięciu panelu czas wraca do poprzedniej prędkości.

---

## 9. GameManager i ekonomia gracza

`GameManager` jest **singletonem** z `DontDestroyOnLoad` — istnieje przez całą sesję rozgrywki, niezależnie od zmiany sceny.

Przechowuje:

- `_lifes` — życia gracza,
- `_resources` — zasoby (walutę),
- `_gameSpeed` — aktualną prędkość gry.

Reaguje na eventy z `Enemy`:

```csharp
private void HandleEnemyReachedEnd(EnemyData data)
{
    _lifes = Mathf.Max(0, _lifes - data.damage);
    OnLifesChanged?.Invoke(_lifes);
}

private void HandleEnemyRemoved(Enemy enemy)
{
    AddResources(Mathf.RoundToInt(enemy.Data.reward));
}
```

Po załadowaniu nowej sceny `GameManager` resetuje stan gracza zgodnie z `LevelData`:

```csharp
public void ResetGame()
{
    _lifes = LevelManagment.Instance.currentLevelData.StartingHealth;
    OnLifesChanged?.Invoke(_lifes);
    _resources = LevelManagment.Instance.currentLevelData.startingResources;
    OnResourcesChanged?.Invoke(_resources);
    SetGameSpeed(1f);
}
```

---

## 10. System poziomów (LevelData / LevelManagment)

### 10.1 LevelData (ScriptableObject)

```csharp
[CreateAssetMenu(fileName = "LevelData", menuName = "Scriptable Objects/LevelData")]
public class LevelData : ScriptableObject
{
    public string levelName;
    public int totalWaves;
    public int startingResources;
    public int StartingHealth;
}
```

### 10.2 Tabela 4. Dostępne poziomy

| Poziom | Nazwa sceny | Liczba fal | Startowe zasoby | Startowe życie |
|--------|-------------|-----------|------------------|-----------------|
| **Level One** | `LevelOne` | 3 | 200 | 10 |
| **Level Two** | `LevelTwo` | 6 | 150 | 5 |

Poziom 2 jest wyraźnie trudniejszy — mniej zasobów na start, mniej żyć i dwukrotnie więcej fal.

### 10.3 LevelManagment

Singleton odpowiedzialny za:

- przechowywanie listy wszystkich poziomów (`LevelData[] levels`),
- ładowanie poziomu (zarówno przy starcie z menu głównego, jak i przy `Next Level`).

```csharp
public void LoadLevel(LevelData levelData)
{
    currentLevelData = levelData;
    SceneManager.LoadScene(levelData.levelName);
}
```

---

## 11. Interfejs użytkownika

`UIController` jest singletonem zarządzającym całą warstwą prezentacji. Subskrybuje eventy ze `Spawner`, `GameManager`, `Platform` i `TowerCard`.

### 11.1 Stale wyświetlane informacje (HUD)

- **Wave: N** — numer aktualnej fali,
- **Life: N** — życia gracza,
- **Resources: N** — zasoby gracza.

### 11.2 Panele

| Panel | Wywoływany przez | Funkcja |
|-------|------------------|---------|
| **Tower Panel** | klik na platformę | wybór wieży do postawienia |
| **Pause Panel** | przycisk pauzy / `Esc` | wznów / restart / menu główne |
| **Game Over Panel** | `Life == 0` | restart / menu główne |
| **Mission Complete Panel** | event `OnMissionComplete` | next level / endless / menu |

### 11.3 Komunikaty kontekstowe

- **Objective text** — na początku każdego poziomu wyświetla się przez 4 s tekst: *„Survive N waves of enemies!”* (gdzie N pochodzi z `LevelData.totalWaves`).
- **Warning text** — komunikaty ostrzegawcze (3 s):
  - *„This platform is already occupied!”*
  - *„Not enough resources!”*

### 11.4 Karty wież

Karty są generowane dynamicznie z tablicy `TowerData[]` przy każdym otwarciu panelu — to pozwala łatwo zmieniać dostępne wieże w runtime (np. odblokowania, eventy).

---

## 12. Sterowanie czasem i pauza

Gracz może zmienić tempo gry przyciskami w UI:

| Przycisk | Mnożnik czasu |
|----------|----------------|
| **×0.5** | spowolnienie |
| **×1** | normalne tempo |
| **×2** | przyspieszenie |

Aktywny przycisk podświetlany jest na zielono. Pauza (`Esc` lub przycisk) ustawia `Time.timeScale = 0`. Również **otwarcie panelu wieży** automatycznie zatrzymuje grę, by gracz mógł na spokojnie podjąć decyzję.

---

## 13. Stany gry: porażka, sukces, tryb endless

### 13.1 Game Over

Gdy `OnLifesChanged` przekaże wartość ≤ 0:

```csharp
private void UpdateLifeText(int currentLifes)
{
    LifeText.text = $"Life: {currentLifes}";
    if (currentLifes <= 0)
        ShowGameOver();
}
```

Pokazuje się panel Game Over, czas zatrzymany, dostępne akcje: **Restart**, **Main Menu**.

### 13.2 Mission Complete

Po pokonaniu `totalWaves` fal Spawner emituje `OnMissionComplete`. UI pokazuje panel z opcjami:

- **Next Level** — przejście do kolejnego poziomu (przycisk wyłączony, jeśli to ostatni poziom),
- **Endless Mode** — kontynuacja z nieskończonymi falami (skalowanie HP rośnie dalej),
- **Main Menu** — powrót do menu.

### 13.3 Endless Mode

Wywołanie `Spawner.EnableEndlessMode()` ustawia flagę `_isEndlessMode = true`, co wyłącza warunek końca misji. Fale są generowane cyklicznie (`_currentWaveIndex = (_currentWaveIndex + 1) % wavesData.Length`), a HP wrogów rośnie z każdą falą.

---

## 14. Zastosowane wzorce projektowe

| Wzorzec | Zastosowanie |
|---------|--------------|
| **Singleton** | `GameManager`, `LevelManagment`, `UIController`, `Spawner` |
| **ScriptableObject** | `EnemyData`, `WaveData`, `TowerData`, `LevelData` — separacja danych od logiki |
| **Object Pool** | `ObjectPooler` dla wrogów oraz pocisków — eliminacja narzutu `Instantiate` |
| **Observer (Events)** | Statyczne `Action`/`event` w `Enemy`, `Spawner`, `GameManager`, `Platform`, `TowerCard` — luźne powiązania między systemami |

---

## 15. Plan dalszego rozwoju

Choć projekt jest funkcjonalnie kompletny, możliwe rozszerzenia:

- [ ] **Ulepszanie wież** — kolejne poziomy z lepszymi statystykami.
- [ ] **Sprzedaż wież** — odzyskiwanie części zasobów.
- [ ] **Więcej typów wrogów** — np. boss co X fal, wróg lecący ponad ścieżką.
- [ ] **Różne strategie celowania** — najsilniejszy, najsłabszy, najbliższy końca, pierwszy w zasięgu.
- [ ] **Efekty wizualne i dźwiękowe** — wybuchy, dźwięk strzału, muzyka tła.
- [ ] **System osiągnięć** — np. „przejdź poziom bez utraty życia”.
- [ ] **Zapis postępów** — odblokowywanie kolejnych poziomów.
- [ ] **Animacje wrogów i wież** — sprite animator dla ruchu i ataków.

---

## 16. Inspiracje i odnośniki

- Bloons Tower Defense — https://www.crazygames.com/game/bloons-tower-defense
- Kingdom Rush — https://www.kingdomrush.com/

---
