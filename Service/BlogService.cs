using BusinessObject.DTO;
using BusinessObject.Entities;
using Microsoft.AspNetCore.Http;
using Repository.Interface;
using Service.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public class BlogService : IBlogService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IFileStorageService _storageService;

        public BlogService(IUnitOfWork unitOfWork, IFileStorageService storageService)
        {
            _unitOfWork = unitOfWork;
            _storageService = storageService;
        }
        public async Task<IEnumerable<BlogPostResponse>> GetAllAsync()
        {
            var posts = await _unitOfWork.BlogPostRepository.GetAllWithCategoriesAsync();
            return posts.Select(p => new BlogPostResponse
            {
                BlogPostId = p.BlogPostId,
                Title = p.Title,
                Summary = p.Summary,
                PublishedDate = p.PublishedDate,
                ThumbnailImageUrl = p.ThumbnailImageUrl,
                AuthorName = p.Author.FullName,
                Position = p.Author.Position,
                Avatar = p.Author.ProfileImageUrl,
                Categories = p.Categories.Select(c => c.Name).ToList()
            });
        }

        public async Task<BlogPostResponse?> GetByIdAsync(int id)
        {
            var post = await _unitOfWork.BlogPostRepository.GetByIdWithCategoriesAsync(id);
            if (post == null) return null;

            return new BlogPostResponse
            {
                BlogPostId = post.BlogPostId,
                Title = post.Title,
                Summary = post.Summary,
                Content = post.Content,
                PublishedDate = post.PublishedDate,
                ThumbnailImageUrl = post.ThumbnailImageUrl,
                AuthorName = post.Author.FullName,
                Position = post.Author.Position,
                Avatar = post.Author.ProfileImageUrl,
                Categories = post.Categories.Select(c => c.Name).ToList()
            };
        }
        public async Task CreateAsync(CreateBlogPostRequest request)
        {
            var categories = new List<BlogCategory>();
            foreach (var cat in request.Categories)
            {
                var existing = await _unitOfWork.BlogCategoryRepository.GetByNameAsync(cat);
                if (existing != null)
                {
                    categories.Add(existing);
                }
                else
                {
                    var newCategory = new BlogCategory { Name = cat };
                    categories.Add(newCategory);
                }
            }
            string? thumbnail = null;
            string? avatar = null;
            if (request.ThumbnailImageUrl != null) {
                 thumbnail = await _storageService.SaveImageFileWithOriginalNameAsync(request.ThumbnailImageUrl, request.ThumbnailImageUrl.FileName);
            }
            if (request.Avatar != null) {
                 avatar = await _storageService.SaveImageFileWithOriginalNameAsync(request.Avatar, request.Avatar.FileName);
            }
            if (request.AuthorId != null)
            {
                var blogPost = new BlogPost
                {
                    Title = request.Title ?? "N/A",
                    Summary = request.Summary ?? "N/A",
                    Content = request.Content ?? "N/A",
                    PublishedDate = DateTime.UtcNow,
                    ThumbnailImageUrl = thumbnail ?? string.Empty,
                    AuthorId = request.AuthorId ?? 0,
                    Categories = categories
                };

                await _unitOfWork.BlogPostRepository.CreateAsync(blogPost);
                await _unitOfWork.SaveChanges();

            }
            else
            {
                var author = new Author
                {
                    FullName = request.AuthorName,
                    Position = request.Position,
                    ProfileImageUrl = avatar ?? string.Empty
                };
                await _unitOfWork.AuthorRepository.CreateAsync(author);
                await _unitOfWork.SaveChanges();
                var blogPost = new BlogPost
                {
                    Title = request.Title ?? "N/A",
                    Summary = request.Summary ?? "N/A",
                    Content = request.Content ?? "N/A",
                    PublishedDate = DateTime.UtcNow,
                    ThumbnailImageUrl = thumbnail ?? string.Empty,
                    AuthorId = author.AuthorId,
                    Categories = categories
                };
                await _unitOfWork.BlogPostRepository.CreateAsync(blogPost);
                await _unitOfWork.SaveChanges();
            }
        }
        public async Task<string> UploadThumbnailAsync(IFormFile file)
        {
            return await _storageService.SaveImageFileWithOriginalNameAsync(file, file.FileName);
        }

    }
}
