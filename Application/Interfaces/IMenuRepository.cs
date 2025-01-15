using Application.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebAPI;

namespace Application.Interfaces
{
    public interface IMenuRepository
    {
        Task<PaginatedOutput<MenuModel>> GetMenuList(int page = 1);
        Task<ReturnGenericData<MenuModel>> GetMenu(int? id);
        Task<ReturnGenericStatus> SaveMenu(MenuModel data);
        Task<ReturnGenericStatus> DeleteMenu(int id);
        Task<ReturnGenericData<ReturnDownloadPDF>> PrintMenu();
        Task<ReturnGenericDropdown> GetSubmenuList();
        ReturnGenericDropdown GetActionMethodList();
        ReturnGenericDropdown GetActionMethodParamList();
    }
}
