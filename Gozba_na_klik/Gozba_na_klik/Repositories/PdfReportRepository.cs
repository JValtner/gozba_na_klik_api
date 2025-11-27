using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Gozba_na_klik.DTOs.Request;
using Gozba_na_klik.Models;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Gozba_na_klik.Services.Snapshots
{
    public class PdfReportRepository : IPdfReportRepository
    {
        private readonly IMongoCollection<PdfMonthlyReportDocument> _collection;

        public PdfReportRepository(IMongoDatabase db)
        {
            _collection = db.GetCollection<PdfMonthlyReportDocument>("monthly_reports");
        }

        public Task<PdfMonthlyReportDocument> GetByIdAsync(string id) =>
            _collection.Find(x => x.Id == id).FirstOrDefaultAsync();

        public Task<PdfMonthlyReportDocument> GetByRestaurantMonthAsync(int restaurantId, int year, int month) =>
            _collection.Find(x => x.RestaurantId == restaurantId && x.Year == year && x.Month == month)
                       .FirstOrDefaultAsync();

        public Task<List<PdfMonthlyReportDocument>> ListAsync(int restaurantId, int? year = null, int? month = null)
        {
            var filter = Builders<PdfMonthlyReportDocument>.Filter.Eq(x => x.RestaurantId, restaurantId);
            if (year.HasValue) filter &= Builders<PdfMonthlyReportDocument>.Filter.Eq(x => x.Year, year.Value);
            if (month.HasValue) filter &= Builders<PdfMonthlyReportDocument>.Filter.Eq(x => x.Month, month.Value);
            return _collection.Find(filter)
                              .SortByDescending(x => x.CreatedUtc)
                              .ToListAsync();
        }

        public async Task<string> InsertAsync(PdfMonthlyReportDocument doc)
        {
            doc.Id = ObjectId.GenerateNewId().ToString();
            doc.CreatedUtc = DateTime.UtcNow;
            await _collection.InsertOneAsync(doc);
            return doc.Id;
        }

        public async Task<bool> ExistsAsync(int restaurantId, int year, int month)
        {
            var count = await _collection.CountDocumentsAsync(x => x.RestaurantId == restaurantId && x.Year == year && x.Month == month);
            return count > 0;
        }
    }
}
