using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using Gozba_na_klik.DTOs.Invoice;
using Gozba_na_klik.DTOs.Request;

namespace Gozba_na_klik.Services
{
    public class PdfService : IPdfService
    {
        private readonly ILogger<PdfService> _logger;

        public PdfService(ILogger<PdfService> logger)
        {
            _logger = logger;

            QuestPDF.Settings.License = LicenseType.Community;
        }

        public async Task<byte[]> GenerateInvoicePdfAsync(InvoiceDto invoice)
        {
            _logger.LogInformation("Generating PDF for invoice {InvoiceId}", invoice.InvoiceId);

            try
            {
                var document = Document.Create(container =>
                {
                    container.Page(page =>
                    {
                        page.Size(PageSizes.A4);
                        page.Margin(2, Unit.Centimetre);
                        page.DefaultTextStyle(x => x.FontSize(10).FontFamily("Arial"));

                        page.Header().Column(column =>
                        {
                            ComposeHeader(column);
                        });

                        page.Content().Column(content =>
                        {
                            ComposeContent(content, invoice);
                        });

                        page.Footer().Column(footer =>
                        {
                            ComposeFooter(footer, invoice);
                        });
                    });
                });

                var pdfBytes = document.GeneratePdf();
                _logger.LogInformation("PDF successfully generated for invoice {InvoiceId}, size: {Size} bytes",
                    invoice.InvoiceId, pdfBytes.Length);

                return pdfBytes;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating PDF for invoice {InvoiceId}", invoice.InvoiceId);
                throw;
            }
        }

        public bool CanGeneratePdf(InvoiceDto invoice)
        {
            return !string.IsNullOrEmpty(invoice.InvoiceId) &&
                   invoice.Items.Any() &&
                   invoice.Customer != null &&
                   invoice.Restaurant != null;
        }

        private void ComposeHeader(ColumnDescriptor column)
        {
            column.Item().Row(row =>
            {
                row.RelativeItem().Column(leftColumn =>
                {
                    leftColumn.Item().Text("GOZBA NA KLIK")
                        .FontSize(20)
                        .Bold()
                        .FontColor("#2563eb");

                    leftColumn.Item().Text("Food Delivery Service")
                        .FontSize(12)
                        .Italic()
                        .FontColor("#6b7280");
                });
                row.RelativeItem().Column(rightColumn =>
                {
                    rightColumn.Item().AlignRight().Text($"Datum kreiranja:")
                        .FontSize(10)
                        .FontColor("#6b7280");

                    rightColumn.Item().AlignRight().Text($"{DateTime.Now:dd.MM.yyyy HH:mm}")
                        .FontSize(10)
                        .Bold();
                });
            });

            column.Item().PaddingTop(10).BorderBottom(1).BorderColor("#e5e7eb").Text("");
        }
        private void ComposeContent(ColumnDescriptor column, InvoiceDto invoice)
        {
            column.Item().PaddingVertical(20).Column(contentColumn =>
            {
                contentColumn.Item().Column(infoColumn =>
                {
                    ComposeInvoiceInfo(infoColumn, invoice);
                });

                contentColumn.Item().PaddingVertical(10).Text("");

                contentColumn.Item().Column(partyColumn =>
                {
                    ComposePartyInfo(partyColumn, invoice);
                });

                contentColumn.Item().PaddingVertical(15).Text("");

                contentColumn.Item().Column(itemsColumn =>
                {
                    ComposeItemsTable(itemsColumn, invoice);
                });

                contentColumn.Item().PaddingVertical(15).Text("");

                contentColumn.Item().Column(summaryColumn =>
                {
                    ComposeSummary(summaryColumn, invoice);
                });

                if (!string.IsNullOrEmpty(invoice.CustomerNote))
                {
                    contentColumn.Item().PaddingTop(15).Column(notesColumn =>
                    {
                        ComposeNotes(notesColumn, invoice);
                    });
                }
            });
        }

        private void ComposeInvoiceInfo(ColumnDescriptor column, InvoiceDto invoice)
        {
            column.Item().Background("#f8fafc").Padding(15).Column(infoColumn =>
            {
                infoColumn.Item().Row(row =>
                {
                    row.RelativeItem().Text("RAČUN")
                        .FontSize(16)
                        .Bold();

                    row.RelativeItem().AlignRight().Text($"#{invoice.InvoiceId}")
                        .FontSize(16)
                        .Bold()
                        .FontColor("#059669");
                });

                infoColumn.Item().PaddingTop(5).Row(row =>
                {
                    row.RelativeItem().Text($"Porudžbina: #{invoice.OrderId}")
                        .FontSize(10)
                        .FontColor("#6b7280");

                    row.RelativeItem().AlignRight().Text($"Datum porudžbine: {invoice.OrderDate:dd.MM.yyyy HH:mm}")
                        .FontSize(10)
                        .FontColor("#6b7280");
                });
            });
        }

        private void ComposePartyInfo(ColumnDescriptor column, InvoiceDto invoice)
        {
            column.Item().Row(row =>
            {
                row.RelativeItem().Column(customerColumn =>
                {
                    customerColumn.Item().Text("KUPAC")
                        .FontSize(12)
                        .Bold()
                        .FontColor("#374151");

                    customerColumn.Item().PaddingTop(5).Column(customerDataColumn =>
                    {
                        customerDataColumn.Item().Text($"{invoice.Customer.Username}")
                            .FontSize(10)
                            .Bold();

                        customerDataColumn.Item().Text($"{invoice.Customer.Email}")
                            .FontSize(9)
                            .FontColor("#6b7280");

                        if (invoice.DeliveryAddress != null && !string.IsNullOrEmpty(invoice.DeliveryAddress.Street))
                        {
                            customerDataColumn.Item().PaddingTop(3).Text("ADRESA DOSTAVE:")
                                .FontSize(9)
                                .Bold()
                                .FontColor("#6b7280");

                            customerDataColumn.Item().Text($"{invoice.DeliveryAddress.Street}")
                                .FontSize(9);

                            customerDataColumn.Item().Text($"{invoice.DeliveryAddress.City}, {invoice.DeliveryAddress.PostalCode}")
                                .FontSize(9);
                        }
                    });
                });

                row.RelativeItem().PaddingLeft(20).Column(restaurantColumn =>
                {
                    restaurantColumn.Item().Text("RESTORAN")
                        .FontSize(12)
                        .Bold()
                        .FontColor("#374151");

                    restaurantColumn.Item().PaddingTop(5).Column(restaurantDataColumn =>
                    {
                        restaurantDataColumn.Item().Text($"{invoice.Restaurant.Name}")
                            .FontSize(10)
                            .Bold();

                        if (!string.IsNullOrEmpty(invoice.Restaurant.Address))
                        {
                            restaurantDataColumn.Item().Text($"{invoice.Restaurant.Address}")
                                .FontSize(9)
                                .FontColor("#6b7280");
                        }

                        if (!string.IsNullOrEmpty(invoice.Restaurant.Phone))
                        {
                            restaurantDataColumn.Item().Text($"Tel: {invoice.Restaurant.Phone}")
                                .FontSize(9)
                                .FontColor("#6b7280");
                        }
                    });
                });
            });
        }

        private void ComposeItemsTable(ColumnDescriptor column, InvoiceDto invoice)
        {
            column.Item().Column(tableWrapperColumn =>
            {
                tableWrapperColumn.Item().Text("STAVKE PORUDŽBINE")
                    .FontSize(12)
                    .Bold()
                    .FontColor("#374151");

                tableWrapperColumn.Item().PaddingTop(10).Table(table =>
                {
                    table.ColumnsDefinition(columns =>
                    {
                        columns.RelativeColumn(3);
                        columns.RelativeColumn(1);
                        columns.RelativeColumn(1.5f);
                        columns.RelativeColumn(1.5f);
                    });


                    table.Header(header =>
                    {
                        header.Cell().Background("#e5e7eb").Padding(8).Text("JELO").FontSize(9).Bold();
                        header.Cell().Background("#e5e7eb").Padding(8).Text("KOL.").FontSize(9).Bold();
                        header.Cell().Background("#e5e7eb").Padding(8).Text("CENA").FontSize(9).Bold();
                        header.Cell().Background("#e5e7eb").Padding(8).Text("UKUPNO").FontSize(9).Bold();
                    });

                    foreach (var item in invoice.Items)
                    {
                        table.Cell().BorderBottom(1).BorderColor("#f3f4f6").Padding(8).Column(itemColumn =>
                        {
                            itemColumn.Item().Text($"{item.MealName}")
                                .FontSize(9)
                                .Bold();

                            if (item.SelectedAddons.Any())
                            {
                                itemColumn.Item().Text($"Dodaci: {string.Join(", ", item.SelectedAddons.Select(a => a.Name))}")
                                    .FontSize(8)
                                    .Italic()
                                    .FontColor("#6b7280");
                            }
                        });

                        table.Cell().BorderBottom(1).BorderColor("#f3f4f6").Padding(8).Text($"{item.Quantity}")
                            .FontSize(9);

                        table.Cell().BorderBottom(1).BorderColor("#f3f4f6").Padding(8).Text($"{item.UnitPrice:F0} RSD")
                            .FontSize(9);

                        table.Cell().BorderBottom(1).BorderColor("#f3f4f6").Padding(8).Text($"{item.TotalPrice:F0} RSD")
                            .FontSize(9)
                            .Bold();
                    }
                });
            });
        }
        private void ComposeSummary(ColumnDescriptor column, InvoiceDto invoice)
        {
            column.Item().AlignRight().Column(summaryWrapperColumn =>
            {
                summaryWrapperColumn.Item().Width(200).Column(summaryColumn =>
                {
                    summaryColumn.Item().Row(row =>
                    {
                        row.RelativeItem().Text("Cena jela:")
                            .FontSize(10);
                        row.RelativeItem().AlignRight().Text($"{invoice.Summary.SubtotalPrice:F0} RSD")
                            .FontSize(10);
                    });
                    summaryColumn.Item().PaddingTop(3).Row(row =>
                    {
                        row.RelativeItem().Text("Dostava:")
                            .FontSize(10);
                        row.RelativeItem().AlignRight().Text($"{invoice.Summary.DeliveryFee:F0} RSD")
                            .FontSize(10);
                    });

                    summaryColumn.Item().PaddingVertical(8).LineHorizontal(1).LineColor("#e5e7eb");

                    summaryColumn.Item().Row(row =>
                    {
                        row.RelativeItem().Text("UKUPNO:")
                            .FontSize(12)
                            .Bold()
                            .FontColor("#059669");
                        row.RelativeItem().AlignRight().Text($"{invoice.Summary.TotalPrice:F0} RSD")
                            .FontSize(12)
                            .Bold()
                            .FontColor("#059669");
                    });

                    summaryColumn.Item().PaddingTop(8).Text($"Način plaćanja: {invoice.Payment.Method}")
                        .FontSize(9)
                        .FontColor("#6b7280");

                    summaryColumn.Item().Text($"Status: {invoice.Payment.Status}")
                        .FontSize(9)
                        .FontColor("#059669");
                });
            });
        }

        private void ComposeNotes(ColumnDescriptor column, InvoiceDto invoice)
        {
            column.Item().Background("#fef3c7").Padding(10).Column(notesColumn =>
            {
                notesColumn.Item().Text("NAPOMENA KUPCA:")
                    .FontSize(9)
                    .Bold()
                    .FontColor("#92400e");

                notesColumn.Item().PaddingTop(3).Text(invoice.CustomerNote!)
                    .FontSize(9)
                    .FontColor("#92400e");
            });
        }

        private void ComposeFooter(ColumnDescriptor column, InvoiceDto invoice)
        {
            column.Item().AlignCenter().Column(footerColumn =>
            {
                footerColumn.Item().LineHorizontal(1).LineColor("#e5e7eb");

                footerColumn.Item().PaddingTop(10).Text($"Račun generisan automatski dana {invoice.Timestamp:dd.MM.yyyy u HH:mm}")
                    .FontSize(8)
                    .FontColor("#9ca3af");

                footerColumn.Item().Text("Hvala što koristite Gozba na Klik!")
                    .FontSize(8)
                    .Italic()
                    .FontColor("#9ca3af");

                if (invoice.HasAllergenWarning)
                {
                    footerColumn.Item().PaddingTop(5).Text("⚠️ Ovaj order je sadržao potencijalne alergene")
                        .FontSize(8)
                        .FontColor("#dc2626");
                }
            });
        }
    }
}