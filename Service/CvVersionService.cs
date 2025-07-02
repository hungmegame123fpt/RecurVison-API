using AutoMapper;
using BusinessObject.DTO.CV;
using BusinessObject.DTO.CvVersion;
using BusinessObject.Entities;
using Repository;
using Repository.Interface;
using Service.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public class CvVersionService : ICvVersionService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CvVersionService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IEnumerable<CvVersionDTO>> GetAllAsync()
        {
            var version =  await _unitOfWork.CvVersionRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<CvVersionDTO>>(version);
        }

        public async Task<CvVersionDTO?> GetByIdAsync(int id)
        {
            var version =  await _unitOfWork.CvVersionRepository.GetByIdAsync(id);
            return _mapper.Map<CvVersionDTO>(version);
        }

        public async Task<CvVersionDTO> CreateAsync(CvVersionDTO versionDto)
        {
            var version = _mapper.Map<CvVersion>(versionDto);
            await _unitOfWork.CvVersionRepository.CreateAsync(version);
            await _unitOfWork.SaveChanges();
            return _mapper.Map<CvVersionDTO>(version);
        }

        public async Task<CvVersionDTO> UpdateAsync(CvVersionDTO versionDto)
        {
            var version = _mapper.Map<CvVersion>(versionDto);
            await _unitOfWork.CvVersionRepository.UpdateAsync(version);
            await _unitOfWork.SaveChanges();
            return _mapper.Map<CvVersionDTO>(version);
        }

        public async Task DeleteAsync(int id)
        {
            var version = await _unitOfWork.CvVersionRepository.GetByIdAsync(id);
            if (version != null)
            {
                await _unitOfWork.CvVersionRepository.DeleteAsync(version);
                await _unitOfWork.SaveChanges();
            }
        }

        public async Task<CvVersionDTO> UpdatePlainTextAsync(int id, string plainText)
        {
            var version = await _unitOfWork.CvVersionRepository.GetByIdAsync(id);
            if (version == null) throw new KeyNotFoundException("CV version not found");

            version.PlainText = plainText;

            await _unitOfWork.CvVersionRepository.UpdateAsync(version);
            await _unitOfWork.SaveChanges();
            return _mapper.Map<CvVersionDTO>(version);
        }
    }
}