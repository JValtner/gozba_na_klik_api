using System;
using Microsoft.Extensions.Logging;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using Gozba_na_klik.DTOs.Request;

namespace Gozba_na_klik.Services.Pdf
{
    public class QuestPdfRenderer : IPdfRenderer
    {
        private readonly ILogger<QuestPdfRenderer> _logger;

        public QuestPdfRenderer(ILogger<QuestPdfRenderer> logger)
        {
            _logger = logger;
        }

        public byte[] Render(MonthlyReportDTO report)
        {
            _logger.LogInformation("Rendering PDF for restaurant {RestaurantId} month {Month}/{Year}", report.RestaurantId, report.Month, report.Year);

            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(2, Unit.Centimetre);
                    page.DefaultTextStyle(x => x.FontSize(10).FontFamily("Arial"));

                    // Header
                    page.Header().Text($"Mesečni izveštaj - {report.Restaurant?.Name ?? report.RestaurantId.ToString()}")
                        .FontSize(18).Bold();

                    // Content
                    page.Content().Column(col =>
                    {
                        col.Item().Text($"Mesec: {report.Month}/{report.Year}").Bold();
                        col.Item().Text($"Ukupan broj porudžbina: {report.TotalOrders}");
                        col.Item().Text($"Ukupan prihod: {report.TotalRevenue:N2} RSD");
                        col.Item().Text($"Prosečna vrednost porudžbine: {report.AverageOrderValue:N2} RSD");

                        col.Item().PaddingTop(10).LineHorizontal(1).LineColor(Colors.Grey.Lighten2);

                        // Top 5 Revenue Orders
                        if (report.Top5RevenueOrders?.Items != null && report.Top5RevenueOrders.Items.Count > 0)
                        {
                            col.Item().PaddingTop(10).Text("Top 5 porudžbina po prihodu").Bold();
                            col.Item().Table(table =>
                            {
                                table.ColumnsDefinition(c =>
                                {
                                    c.ConstantColumn(60);
                                    c.RelativeColumn();
                                    c.ConstantColumn(100);
                                });

                                table.Header(h =>
                                {
                                    h.Cell().Text("ID");
                                    h.Cell().Text("Datum");
                                    h.Cell().Text("Prihod");
                                });

                                foreach (var o in report.Top5RevenueOrders.Items)
                                {
                                    table.Cell().Text(o.OrderId.ToString());
                                    table.Cell().Text(o.OrderDate.ToShortDateString());
                                    table.Cell().Text($"{o.TotalPrice:N2} RSD");
                                }
                            });
                        }

                        // Top & Bottom Meals
                        if (report.Top5PopularMeals?.Items != null && report.Top5PopularMeals.Items.Count > 0)
                        {
                            col.Item().PaddingTop(10).Text("Top 5 najpopularnijih jela").Bold();
                            col.Item().Table(table =>
                            {
                                table.ColumnsDefinition(c =>
                                {
                                    c.RelativeColumn();
                                    c.ConstantColumn(60);
                                    c.ConstantColumn(80);
                                });

                                table.Header(h =>
                                {
                                    h.Cell().Text("Jelo");
                                    h.Cell().Text("Prodatih");
                                    h.Cell().Text("Prihod");
                                });

                                foreach (var m in report.Top5PopularMeals.Items)
                                {
                                    table.Cell().Text(m.MealName);
                                    table.Cell().Text(m.UnitsSold.ToString());
                                    table.Cell().Text($"{m.Revenue:N2} RSD");
                                }
                            });

                            col.Item().PaddingTop(10).Text("Bottom 5 jela po prodaji").Bold();
                            col.Item().Table(table =>
                            {
                                table.ColumnsDefinition(c =>
                                {
                                    c.RelativeColumn();
                                    c.ConstantColumn(60);
                                    c.ConstantColumn(80);
                                });

                                table.Header(h =>
                                {
                                    h.Cell().Text("Jelo");
                                    h.Cell().Text("Prodatih");
                                    h.Cell().Text("Prihod");
                                });

                                foreach (var m in report.Bottom5PopularMeals.Items)
                                {
                                    table.Cell().Text(m.MealName);
                                    table.Cell().Text(m.UnitsSold.ToString());
                                    table.Cell().Text($"{m.Revenue:N2} RSD");
                                }
                            });
                        }

                        // Profit Report
                        if (report.ProfitReport?.DailyReports != null && report.ProfitReport.DailyReports.Count > 0)
                        {
                            col.Item().PaddingTop(10).LineHorizontal(1).LineColor(Colors.Grey.Lighten2);
                            col.Item().PaddingTop(10).Text("Profit po danima").Bold();
                            col.Item().Table(table =>
                            {
                                table.ColumnsDefinition(c =>
                                {
                                    c.RelativeColumn();
                                    c.ConstantColumn(80);
                                    c.ConstantColumn(60);
                                });

                                table.Header(h =>
                                {
                                    h.Cell().Text("Datum");
                                    h.Cell().Text("Broj porudžbina");
                                    h.Cell().Text("Prihod");
                                });

                                foreach (var d in report.ProfitReport.DailyReports)
                                {
                                    table.Cell().Text(d.Date.ToShortDateString());
                                    table.Cell().Text(d.TotalDailyOrders.ToString());
                                    table.Cell().Text($"{d.DailyRevenue:N2} RSD");
                                }
                            });

                            col.Item().Text($"Ukupan prihod: {report.ProfitReport.TotalRevenue:N2} RSD").Bold();
                            col.Item().Text($"Ukupan broj porudžbina: {report.ProfitReport.TotalPeriodOrders}").Bold();
                            col.Item().Text($"Prosečan dnevni prihod: {report.ProfitReport.AverageDailyProfit:N2} RSD").Bold();
                        }

                        // Meal Sales Report
                        if (report.MealSalesReport?.DailyReports != null && report.MealSalesReport.DailyReports.Count > 0)
                        {
                            col.Item().PaddingTop(10).LineHorizontal(1).LineColor(Colors.Grey.Lighten2);
                            col.Item().PaddingTop(10).Text("Prodaja jela po danima").Bold();
                            col.Item().Table(table =>
                            {
                                table.ColumnsDefinition(c =>
                                {
                                    c.RelativeColumn();
                                    c.RelativeColumn();
                                    c.ConstantColumn(60);
                                    c.ConstantColumn(80);
                                });

                                table.Header(h =>
                                {
                                    h.Cell().Text("Datum");
                                    h.Cell().Text("Jelo");
                                    h.Cell().Text("Prodatih");
                                    h.Cell().Text("Prihod");
                                });

                                foreach (var d in report.MealSalesReport.DailyReports)
                                {
                                    table.Cell().Text(d.Date.ToShortDateString());
                                    table.Cell().Text(d.MealName);
                                    table.Cell().Text(d.TotalDailyUnitsSold.ToString());
                                    table.Cell().Text($"{d.DailyRevenue:N2} RSD");
                                }
                            });

                            col.Item().Text($"Ukupno prodatih: {report.MealSalesReport.TotalUnitsSold}").Bold();
                            col.Item().Text($"Ukupan prihod: {report.MealSalesReport.TotalRevenue:N2} RSD").Bold();
                            col.Item().Text($"Prosečno dnevno prodatih: {report.MealSalesReport.AverageDailyUnitsSold:N2}").Bold();
                        }

                        // Orders Summary
                        if (report.OrdersReport != null)
                        {
                            col.Item().PaddingTop(10).LineHorizontal(1).LineColor(Colors.Grey.Lighten2);
                            col.Item().PaddingTop(10).Text("Pregled porudžbina").Bold();
                            col.Item().Column(subCol =>
                            {
                                subCol.Item().Text($"Ukupno porudžbina: {report.OrdersReport.TotalOrders}");
                                subCol.Item().Text($"Prihvaćene: {report.OrdersReport.TotalAcceptedOrders}");
                                subCol.Item().Text($"Otkazane: {report.OrdersReport.TotalCancelledOrders}");
                                subCol.Item().Text($"Isporučene: {report.OrdersReport.TotalCompletedOrders}");
                                subCol.Item().Text($"Na čekanju: {report.OrdersReport.TotalPendingOrders}");
                                subCol.Item().Text($"U dostavi: {report.OrdersReport.TotalInDeliveryOrders}");
                                subCol.Item().Text($"Spremne: {report.OrdersReport.TotalReadyOrders}");
                                subCol.Item().Text($"Dostavljene: {report.OrdersReport.TotalDeliveredOrders}");
                            });
                        }
                    });

                    // Footer
                    page.Footer().AlignCenter().Text($"Generisano {DateTime.UtcNow:dd.MM.yyyy HH:mm}");
                });
            });

            return document.GeneratePdf();
        }
    }
}
