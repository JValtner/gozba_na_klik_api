using MongoDB.Driver;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Gozba_na_klik.DTOs.Invoice;
using Gozba_na_klik.Models;
using System.Text.Json;

namespace Gozba_na_klik.Repositories
{
    public class InvoiceRepository : IInvoiceRepository
    {
        private readonly IMongoCollection<InvoiceDocument> _invoices;
        private readonly ILogger<InvoiceRepository> _logger;

        public InvoiceRepository(IMongoDatabase database, ILogger<InvoiceRepository> logger)
        {
            _invoices = database.GetCollection<InvoiceDocument>("invoices");
            _logger = logger;

            CreateIndexes();
        }

        public async Task<InvoiceDto> SaveInvoiceAsync(InvoiceDto invoice)
        {
            try
            {
                var document = new InvoiceDocument
                {
                    Id = ObjectId.GenerateNewId(),
                    InvoiceId = invoice.InvoiceId,
                    OrderId = invoice.OrderId,
                    Timestamp = invoice.Timestamp,
                    InvoiceJson = JsonSerializer.Serialize(invoice, new JsonSerializerOptions
                    {
                        WriteIndented = true
                    })
                };

                await _invoices.InsertOneAsync(document);
                _logger.LogInformation("Invoice {InvoiceId} saved to MongoDB", invoice.InvoiceId);

                return invoice;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving invoice {InvoiceId} to MongoDB", invoice.InvoiceId);
                throw;
            }
        }

        public async Task<InvoiceDto?> GetInvoiceByOrderIdAsync(int orderId)
        {
            try
            {
                var filter = Builders<InvoiceDocument>.Filter.Eq(x => x.OrderId, orderId);
                var document = await _invoices.Find(filter).FirstOrDefaultAsync();

                if (document == null)
                {
                    _logger.LogInformation("No invoice found for order ID {OrderId}", orderId);
                    return null;
                }

                var invoiceDto = JsonSerializer.Deserialize<InvoiceDto>(document.InvoiceJson);
                _logger.LogInformation("Retrieved invoice {InvoiceId} for order {OrderId}", document.InvoiceId, orderId);

                return invoiceDto;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving invoice for order ID {OrderId}", orderId);
                throw;
            }
        }

        public async Task<InvoiceDto?> GetInvoiceByIdAsync(string invoiceId)
        {
            try
            {
                var filter = Builders<InvoiceDocument>.Filter.Eq(x => x.InvoiceId, invoiceId);
                var document = await _invoices.Find(filter).FirstOrDefaultAsync();

                if (document == null)
                {
                    _logger.LogInformation("No invoice found with ID {InvoiceId}", invoiceId);
                    return null;
                }

                var invoiceDto = JsonSerializer.Deserialize<InvoiceDto>(document.InvoiceJson);
                _logger.LogInformation("Retrieved invoice {InvoiceId}", invoiceId);

                return invoiceDto;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving invoice {InvoiceId}", invoiceId);
                throw;
            }
        }

        public async Task<bool> InvoiceExistsForOrderAsync(int orderId)
        {
            try
            {
                var filter = Builders<InvoiceDocument>.Filter.Eq(x => x.OrderId, orderId);
                var count = await _invoices.CountDocumentsAsync(filter);

                _logger.LogInformation("Invoice existence check for order {OrderId}: {Exists}", orderId, count > 0);
                return count > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking invoice existence for order ID {OrderId}", orderId);
                throw;
            }
        }

        public async Task<bool> DeleteInvoiceAsync(string invoiceId)
        {
            try
            {
                var filter = Builders<InvoiceDocument>.Filter.Eq(x => x.InvoiceId, invoiceId);
                var result = await _invoices.DeleteOneAsync(filter);

                var deleted = result.DeletedCount > 0;
                _logger.LogInformation("Invoice {InvoiceId} deletion: {Success}", invoiceId, deleted ? "Success" : "Not found");

                return deleted;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting invoice {InvoiceId}", invoiceId);
                throw;
            }
        }

        private void CreateIndexes()
        {
            try
            {
                var invoiceIdIndex = new CreateIndexModel<InvoiceDocument>(
                    Builders<InvoiceDocument>.IndexKeys.Ascending(x => x.InvoiceId),
                    new CreateIndexOptions { Unique = true });

                var orderIdIndex = new CreateIndexModel<InvoiceDocument>(
                    Builders<InvoiceDocument>.IndexKeys.Ascending(x => x.OrderId));

                var timestampIndex = new CreateIndexModel<InvoiceDocument>(
                    Builders<InvoiceDocument>.IndexKeys.Descending(x => x.Timestamp));

                _invoices.Indexes.CreateMany(new[] { invoiceIdIndex, orderIdIndex, timestampIndex });
                _logger.LogInformation("MongoDB indexes created for invoices collection");
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Error creating MongoDB indexes for invoices collection");
            }
        }
    }

    public class InvoiceDocument
    {
        [BsonId]
        public ObjectId Id { get; set; }

        [BsonElement("invoiceId")]
        public string InvoiceId { get; set; } = string.Empty;

        [BsonElement("orderId")]
        public int OrderId { get; set; }

        [BsonElement("timestamp")]
        public DateTime Timestamp { get; set; }

        [BsonElement("invoiceJson")]
        public string InvoiceJson { get; set; } = string.Empty;
    }
}