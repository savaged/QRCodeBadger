# QRCodeBadger

Generates a printable PDF of volunteer name + QR-code labels from an .xlsx
or .csv file. The input format is picked automatically from the file
extension.

## Project structure

```
QRCodeBadger.sln
QRCodeBadger.Core/          <- class library: Models, Options, Services
QRCodeBadger.ConsoleApp/    <- thin console host, references Core
```

`QRCodeBadger.Core` has no UI dependencies, so it can be referenced
directly from a future WPF (or any other) front end alongside the console
app - just add a `QRCodeBadger.Wpf` project and add a `ProjectReference`
to `QRCodeBadger.Core.csproj`, the same way `ConsoleApp` does.

## Spreadsheet / CSV format

First worksheet (or the CSV itself), header row in row 1, Name in column A
and UUID in column B:

| Name       | UUID                                 |
|------------|--------------------------------------|
| Jane Smith | 3fa85f64-5717-4562-b3fc-2c963f66afa6 |
| John Doe   | 9c858901-8a57-4791-81fe-4c455b099bc9 |

A CSV version of the same would be:

```csv
Name,UUID
Jane Smith,3fa85f64-5717-4562-b3fc-2c963f66afa6
John Doe,9c858901-8a57-4791-81fe-4c455b099bc9
```

By default columns are read positionally (1st = Name, 2nd = UUID). If your
CSV has differently-ordered or named columns, pass header names explicitly:

```csharp
new CsvSpreadsheetReader(nameHeader: "FullName", uuidHeader: "BadgeId")
```

## Build & run

```bash
dotnet restore
dotnet run --project QRCodeBadger.ConsoleApp -- volunteers.xlsx labels.pdf
dotnet run --project QRCodeBadger.ConsoleApp -- volunteers.csv labels.pdf
```

If no output path is given it defaults to `labels.pdf` in the current directory.

## Adjusting the label layout

Edit the `LabelSheetOptions` block in `Program.cs` (or promote it to a config
file/command-line options if you want that flexibility) to match your label
stock: columns/rows per sheet, label width/height, margins, and QR code size,
all in millimetres. If you're using a specific label product, search
"[product code] dimensions mm" to get exact figures.

## Notes

- QuestPDF is used under its free Community license (see
  https://www.questpdf.com/license/ - it's free for individuals and companies
  under a revenue threshold; check it applies to your situation for the
  convention/charity context).
- QR error correction level is set to Medium (15% recoverable) - fine for
  printed labels scanned at close range. Bump to `ECCLevel.H` in
  `QrCodeGenerator.cs` if labels will get scuffed/handled a lot.
