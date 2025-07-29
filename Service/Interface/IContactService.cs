using BusinessObject.DTO;
using BusinessObject.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Interface
{
    public interface IContactService
    {
        Task SubmitMessageAsync(ContactMessageDto dto);
        Task<bool> RespondToContactAsync(ContactReplyDto dto);
        Task<List<ContactMessage>> GetAllContactsAsync();
    }
}
