using DokumanSistem.Core.Models;
using DokumanSistem.Core.Models.DTO;
using DokumanSistem.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Text.Json;



namespace DokumanSistem.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly DocumentDbContext _context;

        public CategoryController(DocumentDbContext context)
        {
            _context = context;
        }
        [Authorize(Roles = "Admin")]
        [HttpPost("Create")]
        public async Task<IActionResult> CreateCategory(CreateCategoryRequest request)
        {
            var parentCategory = request.ParentCategoryId.HasValue
                ? await _context.Categories.FirstOrDefaultAsync(c => c.Id == request.ParentCategoryId)
                : null;

            var category = new Category
            {
                Name = request.Name,
                ParentCategory = parentCategory,
                IsGroup = request.IsGroup,
                Guid = Guid.NewGuid(),
                IndexData = request.IndexData 
            };

            try
            {
                var indexDataList = JsonSerializer.Deserialize<List<string>>(request.IndexData);
                // Eğer geçerli bir JSON değilse, burada hata alırsınız
            }
            catch (JsonException ex)
            {
                return BadRequest("Geçersiz JSON formatı: " + ex.Message);
            }

            _context.Categories.Add(category);
            await _context.SaveChangesAsync();

            return Ok(category);
        }



        [HttpPost("assign")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AssignCategoryToUser([FromBody] AssignCategoryRequest request)
        {
            try
            {
                Console.WriteLine($"Gelen istek: Email={request.Email}, CategoryId={request.CategoryId}");

                var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
                var category = await _context.Categories.FindAsync(request.CategoryId);

                if (user == null || category == null)
                {
                    Console.WriteLine("Kullanıcı veya kategori bulunamadı.");
                    return NotFound("Kullanıcı veya kategori bulunamadı.");
                }

                var userCategoryLink = new UserCategoryLink
                {
                    UserId = user.Id,
                    CategoryId = category.Id
                };

                _context.UserCategoryLinks.Add(userCategoryLink);
                await _context.SaveChangesAsync();

                Console.WriteLine("Kullanıcı başarıyla kategoriye atandı.");
                return Ok(new { message = "Kategori başarıyla kullanıcıya atandı." });

            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ HATA: {ex.Message}");
                return StatusCode(500, "Sunucu hatası: " + ex.Message);
            }
        }




        [HttpGet("list")]
        [Authorize]
        public async Task<IActionResult> GetCategories()
        {
            var categories = await _context.Categories
                .Include(c => c.SubCategories)
                .ToListAsync();

            return Ok(categories);
        }

        [HttpGet("user-categories")]
        [Authorize]
        public IActionResult GetUserCategories()
        {
            var email = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
            if (email == null)
            {
                return Unauthorized("Kullanıcı e-postası bulunamadı.");
            }

            var user = _context.Users.FirstOrDefault(u => u.Email == email);
            if (user == null)
            {
                return NotFound("Kullanıcı bulunamadı.");
            }

            var categoryLinks = _context.UserCategoryLinks
                .Where(link => link.UserId == user.Id)
                .ToList();

            var result = new List<object>();
            foreach (var link in categoryLinks)
            {
                var category = _context.Categories.FirstOrDefault(c => c.Id == link.CategoryId);
                if (category == null)
                    continue;
      

            result.Add(new
            {
                category.Name,
                category.Guid,
               
            });
        }
            return Ok(result);
        }


            



        [Authorize]
        [HttpGet("user-categories-with-documents")]
        public IActionResult GetUserCategoriesWithDocuments()
        {
            var email = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
            if (email == null)
            {
                return Unauthorized("Kullanıcı e-postası bulunamadı.");
            }

            var user = _context.Users.FirstOrDefault(u => u.Email == email);
            if (user == null)
            {
                return NotFound("Kullanıcı bulunamadı.");
            }

            var categoryLinks = _context.UserCategoryLinks
                .Where(link => link.UserId == user.Id)
                .ToList();

            var result = new List<object>();

            foreach (var link in categoryLinks)
            {
                var category = _context.Categories.FirstOrDefault(c => c.Id == link.CategoryId);
                if (category == null)
                    continue;

                var documents = _context.Documents
                    .Where(d => d.CategoryGuid == category.Guid)
                    .Select(d => new
                    {
                        d.Id,
                        d.FileName,
                        d.CreatedDate,
                        d.UpdatedDate,
                        d.CreatedBy,
                        d.UpdatedBy,
                        d.Metadata,
                        DocumentIndexes = _context.DocumentIndexes
                            .Where(idx => idx.DocumentId == d.Id)
                            .Select(idx => new
                            {
                                idx.CompanyName,
                                idx.DocumentDate,
                                idx.AdditionalInfo
                            })
                            .ToList()
                    })
                    .ToList();

                result.Add(new
                {
                    category.Name,
                    category.Guid,
                    Documents = documents
                });
            }

            return Ok(result);
        }



        [HttpDelete("delete/{id}")]
        public IActionResult DeleteCategory(int id)
        {
            var category = _context.Categories.Find(id);
            if (category == null)
            {
                return NotFound("Kategori bulunamadı.");
            }

            _context.Categories.Remove(category);
            _context.SaveChanges();

            return Ok(new { message = "Kategori silindi." });
        }

        [Authorize]
        [HttpPost("upload/document")]
        public async Task<IActionResult> UploadDocument(
      IFormFile file,
      [FromForm] Guid categoryGuid,
      [FromForm] string metadataJson)
        {
            var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
            var userFullName = User.FindFirst(ClaimTypes.GivenName)?.Value;

            if (string.IsNullOrEmpty(userEmail) || string.IsNullOrEmpty(userFullName))
                return Unauthorized(new { message = "Kullanıcı bilgileri doğrulanamadı." });

            if (file == null || file.Length == 0)
                return BadRequest("Dosya yüklenemedi.");

            var category = await _context.Categories.FirstOrDefaultAsync(c => c.Guid == categoryGuid);
            if (category == null)
                return NotFound("Kategori bulunamadı.");

      
            List<string> expectedKeys;
            try
            {
                expectedKeys = JsonSerializer.Deserialize<List<string>>(category.IndexData ?? "[]");
                if (expectedKeys == null)
                    expectedKeys = new List<string>();  
            }
            catch
            {
                return BadRequest("Kategoriye ait IndexData JSON formatı hatalı.");
            }

            Dictionary<string, string> providedMetadata;
            try
            {
                providedMetadata = JsonSerializer.Deserialize<Dictionary<string, string>>(metadataJson);
            }
            catch
            {
                return BadRequest("Gönderilen metadata JSON formatında olmalı.");
            }

            var missingFields = expectedKeys
                .Where(key => !providedMetadata.ContainsKey(key) || string.IsNullOrWhiteSpace(providedMetadata[key]))
                .ToList();

            if (missingFields.Any())
                return BadRequest($"Eksik metadata alanları: {string.Join(", ", missingFields)}");

            // Dosya kaydetme işlemi
            var categoryFolderPath = Path.Combine(Directory.GetCurrentDirectory(), "Documents", category.Name);
            if (!Directory.Exists(categoryFolderPath))
                Directory.CreateDirectory(categoryFolderPath);

            var filePath = Path.Combine(categoryFolderPath, file.FileName);
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            // Dokümanı veritabanına kaydet
            var document = new Document
            {
                Id = Guid.NewGuid(),
                FileName = file.FileName,
                FileData = filePath,
                CategoryGuid = category.Guid,
                CreatedDate = DateTime.UtcNow,
                CreatedBy = userFullName,
                UpdatedDate = DateTime.UtcNow,
                UpdatedBy = userEmail,
                Metadata = JsonSerializer.Serialize(providedMetadata)
            };

            _context.Documents.Add(document);
            await _context.SaveChangesAsync();

            return Ok(new { DocumentId = document.Id, Message = "Doküman başarıyla yüklendi." });
        }


        private IActionResult Unauthorized(object value)
        {
            throw new NotImplementedException();
        }

        [Authorize]
        [HttpGet("{guid}/metadata-fields")]
        public async Task<IActionResult> GetMetadataFields(Guid guid)
        {
            
            var category = await _context.Categories.FirstOrDefaultAsync(c => c.Guid == guid);
            if (category == null)
                return NotFound("Kategori bulunamadı.");

            if (string.IsNullOrWhiteSpace(category.IndexData))
                return Ok(new { fields = new List<string>() });
            try
            {
               
                var jsonDoc = JsonDocument.Parse(category.IndexData);

                
                var fields = jsonDoc.RootElement.EnumerateArray()
                    .Where(e => e.ValueKind == JsonValueKind.String) 
                    .Select(e => e.GetString())
                    .ToList();

                return Ok(new { fields });
            }
            catch (JsonException ex)
            {
                
                return BadRequest($"Kategoriye ait IndexData geçersiz JSON formatında. Hata: {ex.Message}");
            }
        }



        [HttpGet("document/view/{id}")]
        public IActionResult ViewDocument(Guid id)
        {
            var document = _context.Documents.FirstOrDefault(d => d.Id == id);

            if (document == null)
            {
                return NotFound("Dosya bulunamadı.");
            }

            try
            {
                var filePath = document.FileData;

                if (!System.IO.File.Exists(filePath))
                {
                    return NotFound("Dosya sistemde bulunamadı.");
                }

                var fileBytes = System.IO.File.ReadAllBytes(filePath);

                var cd = new System.Net.Mime.ContentDisposition
                {
                    Inline = true,
                    FileName = document.FileName
                };

                Response.Headers.Add("Content-Disposition", cd.ToString());

                return File(fileBytes, "application/pdf");
            }
            catch (Exception ex)
            {
                return BadRequest($"Dosya okuma hatası: {ex.Message}");
            }
        }

        [HttpGet("{guid}/metadata-values")]
        [Authorize]
        public async Task<IActionResult> GetMetadataFilterValues(Guid guid)
        {
            var category = await _context.Categories.FirstOrDefaultAsync(c => c.Guid == guid);
            if (category == null)
                return NotFound("Kategori bulunamadı.");

            if (string.IsNullOrWhiteSpace(category.IndexData))
                return Ok(new { Fields = new Dictionary<string, List<string>>() });

            List<string> indexFieldsList = null;
            Dictionary<string, string> indexFieldsDict = null;

            try
            {
                // Önce JSON dizisi olarak dene
                indexFieldsList = JsonSerializer.Deserialize<List<string>>(category.IndexData);

                if (indexFieldsList == null)
                {
                    // Eğer List<string> deseralize edilemiyorsa, JSON nesnesi olarak dene
                    indexFieldsDict = JsonSerializer.Deserialize<Dictionary<string, string>>(category.IndexData);
                }
            }
            catch (JsonException)
            {
                return BadRequest("IndexData geçersiz JSON formatında.");
            }

            if (indexFieldsList == null && indexFieldsDict == null)
            {
                return BadRequest("IndexData geçersiz JSON formatında.");
            }

            // Kategorinin metadata'sını ve dokümanları al
            var documents = await _context.Documents
                .Where(d => d.CategoryGuid == guid && !string.IsNullOrWhiteSpace(d.Metadata))
                .ToListAsync();

            var metadataValues = new Dictionary<string, HashSet<string>>();

            foreach (var doc in documents)
            {
                try
                {
                    var metadata = JsonSerializer.Deserialize<Dictionary<string, string>>(doc.Metadata);
                    if (metadata != null)
                    {
                        // Eğer IndexData bir listeyse, her bir öğeyi kontrol et
                        if (indexFieldsList != null)
                        {
                            foreach (var field in indexFieldsList)
                            {
                                if (metadata.TryGetValue(field, out var value))
                                {
                                    if (!metadataValues.ContainsKey(field))
                                        metadataValues[field] = new HashSet<string>();

                                    metadataValues[field].Add(value);
                                }
                            }
                        }
                        // Eğer IndexData bir dictionary ise, anahtarları kontrol et
                        if (indexFieldsDict != null)
                        {
                            foreach (var field in indexFieldsDict.Keys)
                            {
                                if (metadata.TryGetValue(field, out var value))
                                {
                                    if (!metadataValues.ContainsKey(field))
                                        metadataValues[field] = new HashSet<string>();

                                    metadataValues[field].Add(value);
                                }
                            }
                        }
                    }
                }
                catch (JsonException)
                {
                    continue; // Eğer metadata geçersizse, o belgeyi atla
                }
            }

            // HashSet'leri List'e dönüştür
            var result = metadataValues.ToDictionary(kv => kv.Key, kv => kv.Value.ToList());

            return Ok(new { Fields = result });
        }

        [HttpPost("filter-documents")]
        [Authorize]
        public async Task<IActionResult> FilterDocuments([FromBody] DocumentFilterRequest request)
        {
            var query = _context.Documents.AsQueryable();

            // Kategori filtresi
            if (!string.IsNullOrEmpty(request.CategoryGuid) && Guid.TryParse(request.CategoryGuid, out Guid categoryId))
            {
                query = query.Where(d => d.CategoryGuid == categoryId);
            }

            // Metadata filtresi
            if (request.Filters != null && request.Filters.Any())
            {
                foreach (var filter in request.Filters)
                {
                    string key = filter.Key;
                    string value = filter.Value;

                    if (!string.IsNullOrWhiteSpace(key) && !string.IsNullOrWhiteSpace(value))
                    {
                        // Metadata içeriği kontrol edilecek
                        var searchTerm = $"\"{key}\":\"{value}\""; // Anahtar ve değer
                        query = query.Where(d => d.Metadata.Contains(searchTerm)); // Metadata içeriği
                    }
                }
            }

            var result = await query.ToListAsync();
            return Ok(result);
        }



    }
}