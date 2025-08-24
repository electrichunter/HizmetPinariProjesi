// HizmetPinari/Controllers/ServiceRequestController.cs

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HizmetPinari.Api.Data; // DbContext'inizin olduğu namespace
using HizmetPinari.Dtos;
using HizmetPinari.Models;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System;

namespace HizmetPinari.Controllers
{
    /// <summary>
    /// Hizmet talepleri (Service Requests) ile ilgili CRUD operasyonlarını yöneten API controller.
    /// </summary>
    [Authorize] // Bu controller'daki tüm endpoint'ler için kimlik doğrulaması (authentication) zorunludur.
    [ApiController]
    [Route("api/servicerequests")]
    public class ServiceRequestController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ServiceRequestController(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Yeni bir hizmet talebi oluşturur.
        /// </summary>
        /// <param name="createRequestDto">Talep oluşturma verileri.</param>
        /// <returns>Oluşturulan talebin detayları.</returns>
        [HttpPost]
        [ProducesResponseType(typeof(RequestDetailsDto), 201)] // Başarılı olursa dönecek tipi belirtir
        [ProducesResponseType(typeof(ValidationProblemDetails), 400)] // Validasyon hatası olursa
        [ProducesResponseType(401)] // Yetkisiz erişim olursa
        [ProducesResponseType(404)] // Kategori bulunamazsa
        public async Task<IActionResult> CreateServiceRequest([FromBody] CreateRequestDto createRequestDto)
        {
            // Geliştirme: Gönderilen kategori ID'sinin veritabanında var olup olmadığını kontrol et.
            var categoryExists = await _context.ServiceCategories.AnyAsync(c => c.CategoryID == createRequestDto.CategoryID);
            if (!categoryExists)
            {
                // ModelState'e özel bir hata ekleyerek client'a anlamlı bir mesaj dön.
                ModelState.AddModelError(nameof(createRequestDto.CategoryID), "Belirtilen kategori bulunamadı.");
                return NotFound(ModelState);
            }

            var userId = GetCurrentUserId();
            if (userId == null) return Unauthorized();

            var serviceRequest = new ServiceRequest
            {
                CustomerID = userId.Value,
                CategoryID = createRequestDto.CategoryID,
                Title = createRequestDto.Title,
                Description = createRequestDto.Description,
                Location = createRequestDto.Location,
                // Status ve CreatedAt model içinde varsayılan değer alıyor.
            };

            await _context.ServiceRequests.AddAsync(serviceRequest);
            await _context.SaveChangesAsync();

            // Oluşturulan talebi detaylarıyla birlikte geri dönmek için veritabanından tekrar çekiyoruz.
            var createdRequestDetails = await GetRequestDetailsDtoById(serviceRequest.RequestID);

            return CreatedAtAction(nameof(GetRequestById), new { id = serviceRequest.RequestID }, createdRequestDetails);
        }

        /// <summary>
        /// Giriş yapmış kullanıcının kendi oluşturduğu hizmet taleplerini listeler.
        /// </summary>
        /// <returns>Kullanıcının hizmet taleplerinin listesi.</returns>
        [HttpGet("my-requests")]
        [ProducesResponseType(typeof(IEnumerable<RequestDetailsDto>), 200)]
        [ProducesResponseType(401)]
        public async Task<ActionResult<IEnumerable<RequestDetailsDto>>> GetMyRequests()
        {
            var userId = GetCurrentUserId();
            if (userId == null) return Unauthorized();

            var requests = await _context.ServiceRequests
                .Where(sr => sr.CustomerID == userId.Value)
                .Include(sr => sr.Category) // Kategori bilgilerini join et
                .Include(sr => sr.Offers)   // Teklif sayısını alabilmek için teklifleri join et
                .OrderByDescending(sr => sr.CreatedAt)
                .Select(sr => new RequestDetailsDto // Geliştirme: Projeksiyon ile sadece gerekli verileri çek
                {
                    RequestID = sr.RequestID,
                    CustomerID = sr.CustomerID,
                    CustomerFullName = sr.Customer.FirstName + " " + sr.Customer.LastName,
                    CategoryID = sr.CategoryID,
                    CategoryName = sr.Category.CategoryName,
                    Title = sr.Title,
                    Description = sr.Description,
                    Location = sr.Location,
                    Status = sr.Status,
                    OfferCount = sr.Offers.Count, // Teklif sayısını hesapla
                    CreatedAt = sr.CreatedAt,
                    UpdatedAt = sr.UpdatedAt
                })
                .ToListAsync();

            return Ok(requests);
        }

        /// <summary>
        /// ID'si verilen tek bir hizmet talebinin detaylarını getirir.
        /// </summary>
        /// <param name="id">Talep ID'si</param>
        /// <returns>Talep detayları.</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(RequestDetailsDto), 200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<RequestDetailsDto>> GetRequestById(long id)
        {
            var requestDto = await GetRequestDetailsDtoById(id);
            if (requestDto == null)
            {
                return NotFound("Talep bulunamadı.");
            }
            return Ok(requestDto);
        }

        /// <summary>
        /// Bir hizmet talebini iptal eder. (Veritabanından silmez, statüsünü günceller).
        /// </summary>
        /// <param name="id">İptal edilecek talep ID'si.</param>
        /// <returns>İşlem başarılıysa 204 No Content.</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)] // Forbidden - Kullanıcının bu işlemi yapma yetkisi yok
        [ProducesResponseType(404)]
        public async Task<IActionResult> CancelRequest(long id)
        {
            var userId = GetCurrentUserId();
            if (userId == null) return Unauthorized();

            var serviceRequest = await _context.ServiceRequests.FindAsync(id);

            if (serviceRequest == null)
            {
                return NotFound("İptal edilecek talep bulunamadı.");
            }

            // Geliştirme: Sadece talebin sahibi talebi iptal edebilir.
            if (serviceRequest.CustomerID != userId.Value)
            {
                return Forbid("Bu talebi iptal etme yetkiniz bulunmamaktadır.");
            }

            serviceRequest.Status = "CANCELLED";
            serviceRequest.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return NoContent(); // Başarılı silme/iptal işlemlerinde standart yanıt
        }


        // --- Helper Methods ---

        private long? GetCurrentUserId()
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return long.TryParse(userIdString, out long userId) ? userId : (long?)null;
        }

        private async Task<RequestDetailsDto> GetRequestDetailsDtoById(long requestId)
        {
            return await _context.ServiceRequests
                .Where(sr => sr.RequestID == requestId)
                .Include(sr => sr.Customer)
                .Include(sr => sr.Category)
                .Include(sr => sr.Offers)
                .Select(sr => new RequestDetailsDto
                {
                    RequestID = sr.RequestID,
                    CustomerID = sr.CustomerID,
                    CustomerFullName = sr.Customer.FirstName + " " + sr.Customer.LastName,
                    CategoryID = sr.CategoryID,
                    CategoryName = sr.Category.CategoryName,
                    Title = sr.Title,
                    Description = sr.Description,
                    Location = sr.Location,
                    Status = sr.Status,
                    OfferCount = sr.Offers.Count,
                    CreatedAt = sr.CreatedAt,
                    UpdatedAt = sr.UpdatedAt
                })
                .FirstOrDefaultAsync();
        }
    }
}
