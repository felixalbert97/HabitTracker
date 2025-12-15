# HabitTracker 
## WPF MVVM Demo Application (.NET)

Dieses Repository enthält ein **Demo- / Portfolio-Projekt**, das im Rahmen einer Bewerbung als **Junior Softwareentwickler .NET** erstellt wurde.

Ziel des Projekts ist es, grundlegende Kenntnisse in **C#**, **WPF**, **MVVM** und der **Anbindung einer SQLite-Datenbank** zu demonstrieren – nicht die Umsetzung einer vollständigen Produktiv-Anwendung.

---

## Projektziel

Der Fokus dieses Projekts liegt auf:

- einer **sauberen MVVM-Struktur**
- klarer Trennung von View, ViewModel und Model
- grundlegender Datenpersistenz mit SQLite
- nachvollziehbarer Projektstruktur

Das Projekt dient ausdrücklich **Demonstrationszwecken**.

---

## Technischer Stack

- **.NET:** 10.0  
- **UI:** WPF  
- **Architektur:** MVVM  
- **Datenbank:** SQLite  

---

## Projektumfang

- 3 Views
- MVVM-Ordnerstruktur
- lokale SQLite-Datenbank
- einfache Datenbindung und ViewModel-Logik

Nicht enthalten (bewusst):
- vollständige Geschäftslogik
- Validierung auf Produktivniveau
- automatisierte Tests
- vollständige Fehlerbehandlung

---

## Projektstruktur

HabitTracker  
├── Models // Domänenmodelle & Enums  
├── ViewModels // ViewModels (MVVM)  
├── Views // WPF Views (XAML)  
├── Services // SQLite-Anbindung  
├── Commands // Databinding  
└── Helpers // Hilfsklassen (Wrapper)

## Build & Ausführung

### Voraussetzungen

- Windows
- Visual Studio (empfohlen)
- .NET 10.0 SDK

### Build

Das Projekt ist **lokal buildbar** und kann direkt in Visual Studio geöffnet werden:

1. Repository klonen
2. Solution in Visual Studio öffnen
3. Projekt bauen (`Build Solution`)
4. Anwendung starten

> Hinweis:  
> Das Projekt ist nicht als eigenständiger Installer gedacht und wird derzeit ausschließlich über Visual Studio ausgeführt.

---

## Datenbank

Die Anwendung verwendet eine lokale **SQLite-Datenbank**.

Für dieses Demo-Projekt wird die Datenbank bewusst **projektlokal** im Build-Verzeichnis angelegt:
bin/Debug/net10.0-windows/Data  

Dies ermöglicht:
- ein einfaches Build & Run ohne zusätzliche Konfiguration
- eine schnelle Nachvollziehbarkeit der erzeugten Daten
- eine unkomplizierte Bewertung des Projekts im Rahmen einer Bewerbung

In einer produktiven Anwendung würde der Datenbankpfad stattdessen z. B. unter
`LocalApplicationData` abgelegt werden.

---

## Motivation & Einordnung

Dieses Projekt wurde erstellt, um:

- erste praktische Erfahrungen mit **.NET & WPF** zu sammeln
- MVVM-Konzepte anzuwenden
- Code-Struktur und Lesbarkeit zu priorisieren

Es stellt **keinen Anspruch auf Vollständigkeit**, sondern soll einen Einblick in Arbeitsweise, Strukturverständnis und Lernstand geben.

---

## Hinweis

Feedback und Verbesserungsvorschläge sind willkommen – das Projekt dient auch als Grundlage zur Weiterentwicklung.
