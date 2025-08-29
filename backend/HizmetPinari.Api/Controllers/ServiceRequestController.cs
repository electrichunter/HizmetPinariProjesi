// HizmetPinari/Controllers/ServiceRequestController.cs
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HizmetPinari.Api.Data;
using HizmetPinari.Dtos;
using HizmetPinari.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace HizmetPinari.Api.Controllers // Namespace diğer controller'lar ile uyumlu hale getirildi.
{
    /// <summary>
    /// Hizmet talepleri (Service Requests) ile ilgili CRUD operasyonlarını yöneten API controller.
    /// </summary>
    [Authorize] // Bu controller'daki tüm endpoint'ler için kimlik doğrulaması zorunludur.
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
        [HttpPost]
        [ProducesResponseType(typeof(RequestDetailsDto), 201)]
        public async Task<IActionResult> CreateServiceRequest([FromBody] CreateRequestDto createRequestDto)
        {
            try
            {
                var categoryExists = await _context.ServiceCategories.AnyAsync(c => c.CategoryID == createRequestDto.CategoryID && c.IsActive);
                if (!categoryExists)
                {
                    ModelState.AddModelError(nameof(createRequestDto.CategoryID), "Belirtilen kategori bulunamadı veya aktif değil.");
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
                    Location = createRequestDto.Location
                };

                await _context.ServiceRequests.AddAsync(serviceRequest);
                await _context.SaveChangesAsync();

                var createdRequestDetails = await GetRequestDetailsDtoById(serviceRequest.RequestID);
                if (createdRequestDetails == null)
                {
                     return StatusCode(500, "Talep oluşturuldu ancak detayları getirilemedi.");
                }

                return CreatedAtAction(nameof(GetRequestById), new { id = serviceRequest.RequestID }, createdRequestDetails);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[HATA] CreateServiceRequest: {ex.Message}");
                return StatusCode(500, new { Message = "Talep oluşturulurken bir hata oluştu.", Details = ex.Message });
            }
        }

        /// <summary>
        /// Giriş yapmış kullanıcının kendi oluşturduğu hizmet taleplerini listeler.
        /// </summary>
        [HttpGet("my-requests")]
        public async Task<ActionResult<IEnumerable<RequestDetailsDto>>> GetMyRequests()
        {
            try
            {
                var userId = GetCurrentUserId();
                if (userId == null) return Unauthorized();

                var requests = await _context.ServiceRequests
                    .Where(sr => sr.CustomerID == userId.Value)
                    .OrderByDescending(sr => sr.CreatedAt)
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
                    .ToListAsync();

                return Ok(requests);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[HATA] GetMyRequests: {ex.Message}");
                return StatusCode(500, new { Message = "Talepleriniz getirilirken bir hata oluştu.", Details = ex.Message });
            }
        }

        /// <summary>
        /// ID'si verilen tek bir hizmet talebinin detaylarını getirir.
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<RequestDetailsDto>> GetRequestById(long id)
        {
            try
            {
                var requestDto = await GetRequestDetailsDtoById(id);
                if (requestDto == null)
                {
                    return NotFound("Talep bulunamadı.");
                }
                return Ok(requestDto);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[HATA] GetRequestById: {ex.Message}");
                return StatusCode(500, new { Message = "Talep detayı getirilirken bir hata oluştu.", Details = ex.Message });
            }
        }

        /// <summary>
        /// Bir hizmet talebini iptal eder.
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> CancelRequest(long id)
        {
            try
            {
                var userId = GetCurrentUserId();
                if (userId == null) return Unauthorized();

                var serviceRequest = await _context.ServiceRequests.FindAsync(id);

                if (serviceRequest == null)
                {
                    return NotFound("İptal edilecek talep bulunamadı.");
                }

                if (serviceRequest.CustomerID != userId.Value)
                {
                    return Forbid("Bu talebi iptal etme yetkiniz bulunmamaktadır.");
                }

                serviceRequest.Status = "CANCELLED";
                serviceRequest.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[HATA] CancelRequest: {ex.Message}");
                return StatusCode(500, new { Message = "Talep iptal edilirken bir hata oluştu.", Details = ex.Message });
            }
        }

        // --- YARDIMCI METOTLAR ---

        private long? GetCurrentUserId()
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return long.TryParse(userIdString, out long userId) ? userId : null;
        }

        private async Task<RequestDetailsDto?> GetRequestDetailsDtoById(long requestId)
        {
            return await _context.ServiceRequests
                .Where(sr => sr.RequestID == requestId)
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