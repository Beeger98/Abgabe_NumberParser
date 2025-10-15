# NumberParser

A small, focused C#/.NET project that parses ASCII-art “digit images” from a text file into actual numbers. It demonstrates a clean, configurable pipeline for:
- Reading and normalizing input lines
- Segmenting multi-line ASCII glyphs into character “boxes”
- Detecting characters using a pluggable detection algorithm
- Emitting parsed number strings

Currently includes a reference detector for digits 1–5 rendered over four lines.

---

## Contents

- [Overview](#overview)
- [How it works](#how-it-works)
  - [Pipeline](#pipeline)
  - [Character detection](#character-detection)
- [Getting started](#getting-started)
  - [Prerequisites](#prerequisites)
  - [Build](#build)
  - [Run](#run)
- [Input format](#input-format)
  - [Supported digits (1–5)](#supported-digits-1-5)
  - [Example](#example)
- [Configuration](#configuration)
- [API usage](#api-usage)
- [Project structure](#project-structure)


---

## Overview

- Language: C# (100%)
- Target framework: .NET 9.0
- Project type: Console application
- Default input file: `numbers.txt` (copied to output on build)

Key files:
- Solution: [NumberParser.sln](https://github.com/Beeger98/NumberParser/blob/master/NumberParser.sln)
- Project: [NumberParser/NumberParser.csproj](https://github.com/Beeger98/NumberParser/blob/master/NumberParser/NumberParser.csproj)
- Entry point: `NumberParser/Program.cs`
- Parser: `NumberParser/FileParser.cs`
- Example detector: `NumberParser/FourLinesCharacters.cs`

---

## How it works

### Pipeline

Given a text file containing fixed-height ASCII-art digits, the parser emits decoded number strings:

1. Read and normalize lines
   - Reads from Command Line Parameter or  `./numbers.txt`
   - Drops tabs and ignores empty lines
2. Group into “image lines”
   - Chunks the normalized lines using a configured `LineHeight` (e.g., 4)
3. Find character boxes
   - Scans columns to find segments that contain non-space characters across the height
   - Each contiguous “data” segment becomes a `(StartPos, Width)` box
4. Extract glyphs and detect characters
   - Slices each box from every line to form a `char[][]` glyph
   - Calls the configured `DetectionAlgorithm` delegate to map glyphs to `char`
   - Falls back to `UnknownDetectedCharacter` when detection fails
5. Emit results
   - Concatenates detected characters per image line and yields them as strings

Main entry point (Program.cs) configures and runs the pipeline:
- `LineHeight = 4`
- `DetectionAlgorithm = FourLinesCharacters.DetectChar`
- `UnknownDetectedCharacter = 'X'`

### Character detection

`FourLinesCharacters` provides a simple dictionary-based matcher for digits 1–5, represented as exact multi-line strings. You can replace this strategy (e.g., with fuzzy or pattern-based matching) by supplying your own `ParserConfiguration.DetectionAlgorithm`.

## Getting started

### Prerequisites

- .NET SDK 9.0 or later

Check your SDK:
```bash
dotnet --version
```

### Build

```bash
dotnet build
```

### Run
```bash
 Numberparser.exe parameter.txt
```
Notes:
- The app reads from `parameter.txt` at runtime.
- `parameter.txt` is configured to be copied to the output folder on build.

---

## Input format

- Fixed height: 4 lines per “row” of digits (configurable via `LineHeight`).
- Each character is an ASCII glyph inside a box of width ≥ 1 columns.
- Separation: ensure at least one completely blank column between digits so the segmenter can split them.
- Trailing spaces matter for exact matching in the reference detector.

### Supported digits (1–5)

Current known glyphs (exactly as matched):

- 1
```
|
|
|
|
```

- 2
```
---
 _|
|  
---
```

- 3
```
---
 / 
 \ 
-- 
```

- 4
```
|   |
|___|
    |
    |
```

- 5
```
-----
|___ 
    |
____|
```

### Example

A minimal `numbers.txt` containing a single “row” of 1–5 (ensure at least one blank column between characters):

```
|  ---  ---  |   |  -----
|   _|   /   |___|  |___ 
|  |      \      |      |
|  ---  --       |  ____|
```

Expected output:
```
12345
```

If a glyph is unknown, it will be emitted as `X` (configurable).

---

## Configuration

`ParserConfiguration` (see `FileParser.cs`):

- `int LineHeight`
  - Number of lines that constitute a single “row” of digits (default usage: 4)
- `CharacterDetection DetectionAlgorithm`
  - Delegate: `char? CharacterDetection(char[][] characterImage)`
  - Receives the glyph as a 2D char array; return `null` if unknown
- `char? UnknownDetectedCharacter`
  - Character used when detection returns `null` (e.g., `'X'`)

Example (same as Program.cs):
```csharp
var config = new ParserConfiguration
{
    LineHeight = 4,
    DetectionAlgorithm = FourLinesCharacters.DetectChar,
    UnknownDetectedCharacter = 'X'
};
```

---

## API usage

Programmatic usage:
```csharp
var config = new ParserConfiguration
{
    LineHeight = 4,
    DetectionAlgorithm = FourLinesCharacters.DetectChar,
    UnknownDetectedCharacter = 'X'
};

var parser = new FileParser(config);

// Note: current implementation reads from "./numbers.txt"
foreach (var line in parser.ParseFile("./numbers.txt"))
{
    Console.WriteLine(line);
}
```

---

## Project structure

```
NumberParser/
├─ NumberParser.sln
├─ NumberParser/
│  ├─ NumberParser.csproj
│  ├─ Program.cs
│  ├─ FileParser.cs
│  ├─ FourLinesCharacters.cs
│  └─ numbers.txt        # Input sample (copied to output on build)
└─ NumberParserTests/    # Reserved for tests
```


