using BusinessObject.DTO;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Interface
{
    public interface IBlogService
    {
        Task<IEnumerable<BlogPostResponse>> GetAllAsync();
        Task<BlogPostResponse?> GetByIdAsync(int id);
        Task CreateAsync(CreateBlogPostRequest request);
        Task<string> UploadThumbnailAsync(IFormFile file);
    }
}
