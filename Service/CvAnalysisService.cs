﻿using BusinessObject.DTO.CV;
using BusinessObject.Entities;
using Microsoft.EntityFrameworkCore;
using Repository.Interface;
using Service.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public class CvAnalysisService : ICvAnalysisResultService
    {
        private readonly IUnitOfWork _unitOfWork;

        public CvAnalysisService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<CvAnalysisResultDto?> GetLatestAnalysisByCvIdAsync(int cvId)
        {
            var cv = await _unitOfWork.CVRepository.GetByIdAsync(cvId);
            var result = await _unitOfWork.CvAnalysisResult.GetLatestAnalysisForCvAsync(cvId);
            return new CvAnalysisResultDto
            {
                Id = result.Id,
                Name = result.Name,
                Email = result.Email,
                Phone = result.Phone,
                Summary = result.Summary,
                JobDescriptionId = result.JobDescriptionId,
                MatchScore = result.MatchScore,
                CvId = result.CvId,
                CreatedAt = result.CreatedAt,
                JdAlignment = result.JdAlignment,
                CvUrl = cv.FilePath,
                JobDescription = result.JobDescription,
                Skills = result.Skills,
                Education = result.Education,
                Projects = result.Projects,
                Certifications = result.Certifications
            };
        }
        public async Task<List<CvAnalysisSummaryDto>> GetAnalysisSummariesAsync(int userId)
        {
            var results = await _unitOfWork.CvAnalysisResult
                .GetQueryable()
                .Where(r => r.Cv.UserId == userId)
                .Include(r => r.Cv)
                .Include(r => r.JobDescription)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();

            return results.Select(r => new CvAnalysisSummaryDto
            {
                Id = r.Id,
                CreatedAt = r.CreatedAt,
                Score = r.MatchScore,
                Cv = new CvSimpleDto
                {
                    Id = r.Cv.CvId,
                    Name = r.Cv.Title,
                    Url = r.Cv.FilePath
                },
                Jd = new JobDescriptionSimpleDto
                {
                    Id = r.JobDescription?.Id ?? 0,
                    Name = r.JobDescription?.FileName ?? "N/A",
                    Url = r.JobDescription?.FileUrl ?? ""
                }
            }).ToList();
        }
    }
}
