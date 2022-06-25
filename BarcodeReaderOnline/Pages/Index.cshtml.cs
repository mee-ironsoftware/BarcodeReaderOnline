using IronBarCode;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Globalization;

namespace BarcodeReaderOnline.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;

        public IndexModel(ILogger<IndexModel> logger)
        {
            _logger = logger;
        }

        public IFormFile? FormFile { get; set; }
        public string Result { get; private set; } = String.Empty;

        public void OnGet()
        {
            string dateTime = DateTime.Now.ToString("d", new CultureInfo("en-US"));
            ViewData["TimeStamp"] = dateTime;
        }

        public async Task<IActionResult> OnPostUploadAsync()
        {
            if (!ModelState.IsValid)
            {
                Result = "Please correct the form.";

                return Page();
            }

            if (!ModelState.IsValid)
            {
                Result = "Please correct the form.";

                return Page();
            }

            if (FormFile == null)
            {
                Result = "Please update image or pdf file.";
            }

            try
            {
                using (var memoryStream = new MemoryStream())
                {
                    await FormFile!.CopyToAsync(memoryStream);

                    BarcodeResult[] results;
                    
                    if (FormFile.FileName.EndsWith(".pdf"))
                    {
                        results = BarcodeReader.ReadBarcodesFromPdf(memoryStream);
                    }
                    else
                    {
                        results = BarcodeReader.ReadAllBarcodes(memoryStream);
                    }

                    if (results != null && results.Length > 0)
                    {
                        Result += $"Found {results.Length} Barcode(s)";
                        foreach (var result in results)
                        {
                            Result += $"<br> <b>Barcode type:</b> {result.BarcodeType} / <b>Barcode value:</b> {result.Value}";
                        }
                    }
                    else
                    {
                        Result = "No Barcode found.";
                    }
                }
            }
            catch (Exception ex)
            {
                Result = $"{FormFile!.Name} upload failed. Please contact the Help Desk for support. Error: {ex.Message}";
            }

            return Page();
        }
    }
}