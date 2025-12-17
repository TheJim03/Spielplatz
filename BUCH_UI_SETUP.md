# 📚 Buch UI Setup - Ausführliche Anleitung (2-Seiten Layout)

Diese Anleitung zeigt dir Schritt-für-Schritt wie du die Buch-UI in Unity aufbaust.
**Layout**: Buch mittig mit 2 Seiten (je 2 Snippet-Slots), Inventar unten als horizontale Leiste.

---

## 🎯 Teil 1: Paper Snippet Prefab erstellen (Worldspace)

### 1.1 Neues GameObject erstellen
1. **Hierarchy** → Rechtsklick → **Create Empty**
2. Benenne es: `PaperSnippet`
3. Position: `(0, 1, 0)` (ein Meter über dem Boden)

### 1.2 Visuelle Komponente hinzufügen
**Option A - Mit 3D Quad:**
1. Rechtsklick auf `PaperSnippet` → **3D Object** → **Quad**
2. Rotation des Quad: `(90, 0, 0)` (liegt flach)
3. Scale: `(0.4, 0.4, 0.4)` (kleinere Snippets)

**Option B - Mit 3D Cube (einfacher für Anfang):**
1. Rechtsklick auf `PaperSnippet` → **3D Object** → **Cube**
2. Scale: `(0.25, 0.01, 0.3)` (kleineres flaches Snippet)

### 1.3 Material erstellen
1. **Project** → **Assets/Materials** → Rechtsklick → **Create** → **Material**
2. Benenne es: `SnippetMaterial`
3. Farbe: Weiß oder leicht gelblich `(1, 0.95, 0.8)`
4. Ziehe das Material auf dein PaperSnippet-GameObject

### 1.4 Collider hinzufügen
1. PaperSnippet GameObject auswählen
2. **Inspector** → **Add Component** → `Box Collider`
3. ✅ Haken bei **Is Trigger** setzen!

### 1.5 Script hinzufügen
1. PaperSnippet GameObject auswählen
2. **Add Component** → suche nach `Paper`
3. Konfiguriere im Inspector:
   - **Paper ID**: `1` (für erstes Snippet)
   - **Collect Sound**: (optional, später hinzufügen)
   - **Sound Volume**: `1`
   - **Pickup Duration**: `0.5`
   - **Float Height**: `1`

### 1.6 Als Prefab speichern
1. Ziehe das `PaperSnippet` GameObject aus der Hierarchy in den **Project**-Ordner `Assets/Prefabs/`
2. Du hast jetzt ein Prefab!
3. Erstelle 3 weitere Kopien in der Szene und setze **Paper ID** auf 2, 3, 4

---

## 🎮 Teil 2: PaperManager erstellen

### 2.1 GameObject erstellen
1. **Hierarchy** → Rechtsklick → **Create Empty**
2. Benenne es: `PaperManager`

### 2.2 Script hinzufügen
1. `PaperManager` GameObject auswählen
2. **Add Component** → suche nach `PaperManager`
3. Konfiguriere:
   - **Max Papers**: `4`
   - **Paper Text**: (leer lassen, kommt später)

---

## 🖼️ Teil 3: Canvas & UI Layout erstellen

### 3.1 Canvas erstellen
1. **Hierarchy** → Rechtsklick → **UI** → **Canvas**
2. Canvas auswählen, im Inspector:
   - **Render Mode**: `Screen Space - Overlay`
   - **UI Scale Mode**: `Scale With Screen Size`
   - **Reference Resolution**: `1920 x 1080`

### 3.2 Event System (automatisch erstellt)
- Sollte automatisch erstellt worden sein
- Falls nicht: **Hierarchy** → Rechtsklick → **UI** → **Event System**

### 3.3 Haupt-Panel erstellen (BookPanel)
1. Rechtsklick auf **Canvas** → **UI** → **Panel**
2. Benenne es: `BookPanel`
3. **Inspector** → **RectTransform**:
   - **Anchors**: Stretch/Stretch (ganz oben links Icon)
   - **Left**: `0`, **Top**: `0`, **Right**: `0`, **Bottom**: `0`
   - **Color**: Halbtransparent schwarz `(0, 0, 0, 200)`
4. ❌ **Deaktiviere** das GameObject (Checkbox oben links) - es soll zu Beginn unsichtbar sein!

---

## 📦 Teil 4: Buch-Container erstellen (mittig)

### 4.1 Buch Container Panel
1. Rechtsklick auf `BookPanel` → **UI** → **Panel**
2. Benenne es: `BookContainer`
3. **RectTransform**:
   - **Anchors**: Mittig (center/middle)
   - **Pos X**: `0`, **Pos Y**: `100` (leicht nach oben für Platz für Inventar)
   - **Width**: `900`, **Height**: `600`
   - **Color**: Braun/Beige `(0.8, 0.7, 0.5, 1)` (wie ein Buch)

### 4.2 Buch Header
1. Rechtsklick auf `BookContainer` → **UI** → **Text - TextMeshPro**
   - (Falls Popup: "Import TMP Essentials" → klicke **Import**)
2. Benenne es: `BookHeaderText`
3. **RectTransform**:
   - **Anchors**: Top-Stretch
   - **Pos Y**: `-35`
   - **Height**: `70`
4. **TextMeshPro - Text**:
   - **Text**: `"Das Buch der Weisen"`
   - **Font Size**: `32`
   - **Alignment**: Center/Middle
   - **Color**: Dunkelbraun `(0.2, 0.1, 0.05, 1)`

### 4.3 Linke Buchseite Container
1. Rechtsklick auf `BookContainer` → **UI** → **Panel**
2. Benenne es: `LeftPage`
3. **RectTransform**:
   - **Anchors**: Links-Stretch (left stretch vertical)
   - **Left**: `20`, **Top**: `90`, **Right**: `460`, **Bottom**: `20`
   - **Color**: Pergament `(0.95, 0.92, 0.85, 1)`

### 4.4 Rechte Buchseite Container
1. Rechtsklick auf `BookContainer` → **UI** → **Panel**
2. Benenne es: `RightPage`
3. **RectTransform**:
   - **Anchors**: Rechts-Stretch (right stretch vertical)
   - **Left**: `460`, **Top**: `90`, **Right**: `20`, **Bottom**: `20`
   - **Color**: Pergament `(0.95, 0.92, 0.85, 1)`

### 4.5 Mittellinie (Buchrücken)
1. Rechtsklick auf `BookContainer` → **UI** → **Image**
2. Benenne es: `BookSpine`
---

## 📖 Teil 5: Snippet-Slots erstellen (2 pro Seite = 4 total)

### 5.1 Linke Seite - Slot-Container
1. Rechtsklick auf `LeftPage` → **UI** → **Panel**
2. Benenne es: `LeftSlotsContainer`
3. **RectTransform**:
   - **Anchors**: Stretch/Stretch
   - **Left**: `15`, **Top**: `15`, **Right**: `15`, **Bottom**: `15`
   - **Color**: Transparent `(0, 0, 0, 0)`
4. **Add Component** → `Vertical Layout Group`
5. Konfiguriere:
   - **Spacing**: `20`
   - **Child Alignment**: Middle Center
   - ✅ **Child Force Expand**: Width AN, Height AN

### 5.2 Rechte Seite - Slot-Container
1. Rechtsklick auf `RightPage` → **UI** → **Panel**
2. Benenne es: `RightSlotsContainer`
3. Gleiche Settings wie `LeftSlotsContainer` (siehe 5.1)
4. **Add Component** → `Grid Layout Group`
5. Konfiguriere **Grid Layout Group**:
   - **Cell Size**: `(200, 250)` (Hochformat für Buchseiten)
   - **Spacing**: `(20, 20)`
   - **Start Axis**: Horizontal
   - **Child Alignment**: Middle Center
   - **Constraint**: Fixed Column Count → `2`

---

## 📄 Teil 6: Page Slots erstellen (5 Stück)

### 6.1 Ersten Slot erstellen
1. Rechtsklick auf `SlotsGrid` → **UI** → **Image**
2. Benenne es: `PageSlot1`
3. **Image Component**:
   - **Color**: Halbtransparent weiß `(1, 1, 1, 0.3)`
   - **Source Image**: (kann leer bleiben oder ein Rahmen-Sprite)

---

## 📄 Teil 6: Snippet Slots erstellen (4 Stück - je 2 pro Seite)

### 6.1 Linke Seite - Slot 1 (oben)
1. Rechtsklick auf `LeftSlotsContainer` → **UI** → **Image**
2. Benenne es: `SnippetSlot1`
3. **Image Component**:
   - **Color**: Halbtransparent grau `(0.8, 0.8, 0.8, 0.3)`
   - **Source Image**: (kann leer bleiben)

### 6.2 Placeholder Text
1. Rechtsklick auf `SnippetSlot1` → **UI** → **Text - TextMeshPro**
2. Benenne es: `PlaceholderText`
3. **RectTransform**: Stretch/Stretch
4. **Text**:
   - **Text**: `"Snippet 1"`
   - **Font Size**: `24`
   - **Alignment**: Center/Middle
   - **Color**: Grau `(0.4, 0.4, 0.4, 1)`

### 6.3 Script hinzufügen
1. `SnippetSlot1` auswählen
2. **Add Component** → `BookPageSlot`
3. Konfiguriere:
   - **Required Paper ID**: `1`
   - **Slot Image**: Ziehe die `Image` Component von SnippetSlot1 hier rein
   - **Empty Color**: `(0.8, 0.8, 0.8, 0.3)`
   - **Filled Color**: `(1, 1, 1, 1)`
   - **Placeholder Text**: Ziehe das `PlaceholderText` GameObject hier rein

### 6.4 Linke Seite - Slot 2 (unten)
1. Dupliziere `SnippetSlot1` (Strg+D) im `LeftSlotsContainer`
2. Benenne es: `SnippetSlot2`
3. **BookPageSlot Script** → **Required Paper ID**: `2`
4. **PlaceholderText** → Text: `"Snippet 2"`

### 6.5 Rechte Seite - Slot 3 (oben)
---

## 📦 Teil 7: Inventar-Leiste erstellen (unten)

### 7.1 Inventar Panel
1. Rechtsklick auf `BookPanel` → **UI** → **Panel**
2. Benenne es: `InventoryPanel`
3. **RectTransform**:
   - **Anchors**: Unten-Stretch (bottom stretch)
   - **Pos Y**: `80`
   - **Height**: `140`
   - **Left**: `100`, **Right**: `100`
   - **Color**: Dunkelgrau `(0.2, 0.2, 0.2, 0.9)`

### 7.2 Header Text
1. Rechtsklick auf `InventoryPanel` → **UI** → **Text - TextMeshPro**
2. Benenne es: `HeaderText`
3. **RectTransform**:
   - **Anchors**: Top-Stretch
   - **Pos Y**: `-15`
   - **Height**: `30`
4. **Text**:
   - **Text**: `"Gesammelte Snippets"`
   - **Font Size**: `18`
   - **Alignment**: Center/Middle
   - **Color**: Weiß

### 7.3 Grid Container (horizontal)
1. Rechtsklick auf `InventoryPanel` → **UI** → **Panel**
2. Benenne es: `InventoryGrid`
3. **RectTransform**:
   - **Anchors**: Stretch/Stretch
   - **Left**: `20`, **Top**: `50`, **Right**: `20`, **Bottom**: `10`
   - **Color**: Transparent `(0, 0, 0, 0)`
4. **Add Component** → `Horizontal Layout Group`
5. Konfiguriere:
   - **Spacing**: `15`
   - **Child Alignment**: Middle Center
   - **Child Force Expand**: Width AUS, Height AN

---
---

## 🔧 Teil 9: BookUI Manager einrichten

### 9.1 GameObject erstellen
1. **Hierarchy** → Rechtsklick → **Create Empty**
2. Benenne es: `BookUIManager`

### 9.2 Script hinzufügen & konfigurieren
1. `BookUIManager` auswählen
2. **Add Component** → `BookUI`
3. Konfiguriere im Inspector:
   - **Book Panel**: Ziehe `BookPanel` hier rein
   - **Inventory Container**: Ziehe `InventoryGrid` (unter InventoryPanel) hier rein
   - **Book Page Container**: Ziehe `BookContainer` hier rein (nicht mehr benötigt, aber leer lassen ist ok)
   - **Paper Item Prefab**: Ziehe das `SnippetItem` Prefab hier rein
   - **Toggle Key**: `B`
   - **Font Size**: `32`
---

## ✅ Teil 10: Player Tag setzen0.3, 0.2, 0.1, 1)`

### 8.3 Components hinzufügen
1. `SnippetItem` auswählen
2. **Add Component** → `Canvas Group`
3. **Add Component** → `DraggablePaper`
4. Konfiguriere **DraggablePaper**:
   - **Paper ID**: `1` (wird später dynamisch gesetzt)

### 8.4 Als Prefab speichern
1. Ziehe `SnippetItem` aus Hierarchy in `Assets/Prefabs/`
2. ✅ Prefab erstellt!
3. Lösche das `SnippetItem` aus der Hierarchy
3. Lösche das `PaperItem` aus der Hierarchy (wir brauchen es nur als Prefab)

---
---

## 🎮 Teil 12: Testen!l - Sounds hinzufügen
### 8.1 GameObject erstellen
1. **Hierarchy** → Rechtsklick → **Create Empty**
2. Benenne es: `BookUIManager`

### 12.2 Play Mode
1. Klicke **Play** (oben Mitte)
2. Laufe zu einem Snippet → es wird eingesammelt
3. Drücke **B** → UI öffnet sich!
4. **Layout**: Buch ist mittig mit 2 Seiten (je 2 Slots), Inventar unten
5. Ziehe Snippets aus dem Inventar unten in die richtigen Slots im Buch
6. Wenn alle 4 platziert → Console zeigt "BUCH VOLLSTÄNDIG!"n
   - **Book Page Container**: Ziehe `SlotsGrid` hier rein
   - **Paper Item Prefab**: Ziehe das `PaperItem` Prefab hier rein
   - **Toggle Key**: `B`

---

## ✅ Teil 9: Player Tag setzen

### 9.1 Player GameObject finden
1. Finde in der Hierarchy deinen **Ball** oder **Block** (was der Spieler steuert)
2. Wähle ihn aus

### 9.2 Tag setzen
1. **Inspector** → ganz oben **Tag** Dropdown
2. Falls "Player" nicht existiert:
   - Klicke **Add Tag...**
   - **+** klicken
   - Name: `Player`
   - Speichern
3. Zurück zum Player GameObject
**Snippets werden nicht eingesammelt:**
- Prüfe ob Player den Tag "Player" hat
- Prüfe ob Snippet einen Collider mit "Is Trigger" hat

**Drag & Drop funktioniert nicht:**
- Prüfe ob `SnippetItem` Prefab `CanvasGroup` hat
- Prüfe ob `DraggablePaper` Script drauf ist
- Prüfe ob Canvas ein `Event System` hat

**Snippets akzeptieren nicht:**
- Prüfe ob Snippet IDs stimmen (1-4)
- Prüfe ob Slot IDs mit Snippet IDs übereinstimmen

**Inventar erscheint leer:**
- Prüfe ob `BookUI` das richtige `SnippetItem` Prefab hat
- Prüfe ob `InventoryGrid` korrekt zugewiesen ist
2. **Paper Script** → **Collect Sound**: Ziehe `paper_collect.wav` hier rein

### 10.3 Slots konfigurieren
1. Wähle `PageSlot1` (und alle anderen)
2. **BookPageSlot Script**:
   - **Correct Place Sound**: `slot_correct.wav`
   - **Wrong Place Sound**: `slot_wrong.wav`

---

## 🎮 Teil 11: Testen!

### 11.1 Szene speichern
1. **File** → **Save Scene** (Strg+S)

### 11.2 Play Mode
1. Klicke **Play** (oben Mitte)
2. Laufe zu einem Paper → es wird eingesammelt
3. Drücke **B** → UI öffnet sich!
4. Ziehe Papers in die richtigen Slots
5. Wenn alle 5 platziert → Console zeigt "BUCH VOLLSTÄNDIG!"

---

## 🐛 Troubleshooting

**UI öffnet sich nicht:**
- Prüfe ob `BookPanel` das Script `BookUI` hat
- Prüfe ob `BookPanel` zu Beginn deaktiviert ist

**Papers werden nicht eingesammelt:**
- Prüfe ob Player den Tag "Player" hat
- Prüfe ob Paper einen Collider mit "Is Trigger" hat

**Drag & Drop funktioniert nicht:**
- Prüfe ob `PaperItem` Prefab `CanvasGroup` hat
- Prüfe ob `DraggablePaper` Script drauf ist
- Prüfe ob Canvas ein `Event System` hat

**Papers akzeptieren nicht:**
- Prüfe ob Paper IDs stimmen (1-5)
- Prüfe ob Slot IDs mit Paper IDs übereinstimmen

---

## 🎨 Nächste Schritte (Optional)

- **Bilder statt Farben**: Ersetze die Image Colors mit echten Sprites
- **Animations**: Slot-Glow bei Hover, Confetti bei Completion
- **Sound-Effekte**: Mehr Audio-Feedback
- **Level Complete**: Wenn Buch fertig → nächstes Level laden

Viel Erfolg! 🚀
