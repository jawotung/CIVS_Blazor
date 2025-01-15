using Application.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebAPI;

namespace Application.Service.Interfaces
{
    public interface IGroupServices
    {
        Task<PaginatedOutputServices<GroupModel>> GetGroup(int? page);
        Task<ReturnGenericData<ReturnGroup>> GetGroupDetails(int id);
        Task<ReturnGenericStatus> GroupCreate(GroupModel groupModel);
        Task<ReturnGenericStatus> GroupEdit(int id, GroupModel groupModel);
        Task<ReturnGenericStatus> GroupDeleteConfirmed(int id);
        Task<ReturnGenericData<ReturnDownloadPDF>> PrintReport();
        Task<ReturnGenericData<ReturnDownloadPDF>> PrintMatrix();
    }
}
