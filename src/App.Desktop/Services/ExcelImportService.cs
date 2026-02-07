using ClosedXML.Excel;

namespace App.Desktop.Services;

public sealed class ExcelImportService
{
    public IReadOnlyList<string> GetSheetNames(string filePath)
    {
        using var workbook = new XLWorkbook(filePath);
        return workbook.Worksheets.Select(sheet => sheet.Name).ToList();
    }

    public IReadOnlyList<string> GetHeaders(string filePath, string sheetName)
    {
        using var workbook = new XLWorkbook(filePath);
        var worksheet = workbook.Worksheet(sheetName);
        var headerRow = worksheet.FirstRowUsed();

        if (headerRow is null)
        {
            return Array.Empty<string>();
        }

        return headerRow.CellsUsed().Select(cell => cell.GetString()).ToList();
    }

    public IReadOnlyList<IReadOnlyList<string>> GetPreviewRows(string filePath, string sheetName, int count)
    {
        using var workbook = new XLWorkbook(filePath);
        var worksheet = workbook.Worksheet(sheetName);
        var firstDataRow = worksheet.FirstRowUsed()?.RowBelow();

        if (firstDataRow is null)
        {
            return Array.Empty<IReadOnlyList<string>>();
        }

        var rows = new List<IReadOnlyList<string>>();
        var currentRow = firstDataRow;

        while (!currentRow.IsEmpty() && rows.Count < count)
        {
            rows.Add(currentRow.CellsUsed().Select(cell => cell.GetString()).ToList());
            currentRow = currentRow.RowBelow();
        }

        return rows;
    }
}
