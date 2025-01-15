using Application.Interfaces;
using Application.Models;
using Infrastructure.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Reflection;
using System.Threading.Tasks;
using WebAPI;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace BOCCIVS.server.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [Authorize]
    public class MenuController : ControllerBase
    {

        private readonly IUnitOfWork _unitOfWork;
        public MenuController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet("GetMenuList")]
        public async Task<PaginatedOutput<MenuModel>> GetMenuList(int page = 1)
        {
            try
            {
                PaginatedOutput<MenuModel> Paginate = await _unitOfWork.Menu.GetMenuList(page);

                return Paginate;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        [HttpGet("GetMenu")]
        public async Task<ReturnGenericData<MenuModel>> GetMenu(int? id)
        {
            ReturnGenericData<MenuModel> r = await _unitOfWork.Menu.GetMenu(id);
            return r;
        }
        [HttpPost("SaveMenu")]
        public async Task<ReturnGenericStatus> SaveMenu(MenuModel data)
        {
            ReturnGenericStatus status = new();
            if (ModelState.IsValid)
            {
                status = await _unitOfWork.Menu.SaveMenu(data);
            }
            return status;
        }
        [HttpPost, ActionName("Delete")]
        public async Task<ReturnGenericStatus> DeleteMenu(int id)
        {
            ReturnGenericStatus status = new();
            status = await _unitOfWork.Menu.DeleteMenu(id);
            return status;
        }
        [HttpGet("PrintMenu")]
        public async Task<ReturnGenericData<ReturnDownloadPDF>> PrintMenu()
        {
            return await _unitOfWork.Menu.PrintMenu();
            //return File(FileData, "application/pdf", string.Format("MenuListReport{0}.pdf", DateTime.Now.ToString("MMddyyyy")));
        }
        [HttpGet("GetControllerList")]
        public ReturnGenericDropdown GetControllerList()
        {
            ReturnGenericStatus status = new ReturnGenericStatus();
            ReturnGenericDropdown x = new ReturnGenericDropdown();
            try
            {

                List<Application.Models.SelectListItem> result = Assembly.GetExecutingAssembly()
                                      .GetTypes()
                                      //.Where(type => typeof(Controller).IsAssignableFrom(type))
                                      .SelectMany(type => type.GetMethods(BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.Public))
                                      .Where(m => !m.GetCustomAttributes(typeof(System.Runtime.CompilerServices.CompilerGeneratedAttribute), true).Any())
                                      .GroupBy(x => x.DeclaringType.Name)
                                      .Select(x => new Application.Models.SelectListItem
                                      {
                                          Text = x.Key,
                                          Value = x.Key,
                                          IsChecked = false,
                                      })
                .ToList();


                status.StatusMessage = "";
                status.StatusCode = "00";
                x.Data = result;
            }
            catch (Exception err)
            {
                status.StatusMessage = err.Message;
                status.StatusCode = "01";
            }
            x.StatusMessage = status.StatusMessage;
            x.StatusCode = status.StatusCode;
            return x;
        }
        [HttpGet("GetSubmenuList")]
        public async Task<ReturnGenericDropdown> GetSubmenuList()
        {
            return await _unitOfWork.Menu.GetSubmenuList();
        }
        [HttpGet("GetActionMethodList")]
        public ReturnGenericDropdown GetActionMethodList()
        {
            return _unitOfWork.Menu.GetActionMethodList();
        }
        [HttpGet("GetActionMethodParamList")]
        public ReturnGenericDropdown GetActionMethodParamList()
        {
            return _unitOfWork.Menu.GetActionMethodParamList();
        }
    }
}
