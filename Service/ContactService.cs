using BusinessObject.DTO;
using BusinessObject.Entities;
using Repository.Interface;
using Service.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public class ContactService : IContactService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ContactService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task SubmitMessageAsync(ContactMessageDto dto)
        {
            var message = new ContactMessage
            {
                Name = dto.Name,
                Email = dto.Email,
                Subject = dto.Subject,
                Message = dto.Message
            };

            await _unitOfWork.ContactRepository.CreateAsync(message);
            await _unitOfWork.SaveChanges();
        }
        public async Task<bool> RespondToContactAsync(ContactReplyDto dto)
        {
            var message = await _unitOfWork.ContactRepository.GetByIdAsync(dto.ContactId);
            if (message == null) return false;

            message.AdminResponse = dto.Response;
            message.ResponseDate = DateTime.UtcNow;

            await _unitOfWork.ContactRepository.UpdateAsync(message);
            await _unitOfWork.SaveChanges();
            return true;
        }
        public async Task<List<ContactMessage>> GetAllContactsAsync()
        {
            return await _unitOfWork.ContactRepository.GetAllAsync();
        }
    }
}
