using Application.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebAPI;

namespace Application.Service.Interfaces
{
    public interface IMenuServices
    {
        Task<PaginatedOutputServices<MenuModel>> GetMenuList(string sKey = "", int page = 1);
        Task<ReturnGenericData<MenuModel>> GetMenu(int? id);
        Task<ReturnGenericDictionary> SaveMenu(MenuModel data);
        Task<ReturnGenericStatus> DeleteMenu(int id);
        Task<ReturnGenericData<ReturnDownloadPDF>> PrintMenu(string sKey = "");
        Task<ReturnGenericDropdown> GetControllerList();
        Task<ReturnGenericDropdown> GetSubmenuList();
        Task<ReturnGenericDropdown> GetActionMethodList();
        Task<ReturnGenericDropdown> GetActionMethodParamList();
    }
}
