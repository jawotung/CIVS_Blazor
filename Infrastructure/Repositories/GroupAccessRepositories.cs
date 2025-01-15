using Application.Interfaces;
using Application.Models;
using Infrastructure.Common;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using WebAPI;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Infrastructure.Repositories
{
    public class GroupAccessRepositories : IGroupAccessRepositories
    {
        private readonly AppDbContext _dBContext;
        private readonly ADClass _adClass;
        private readonly CommonClass _commonClass;
        public GroupAccessRepositories(AppDbContext dBContext, ADClass addClass, CommonClass commonClass)
        {
            _dBContext = dBContext;
            _adClass = addClass;
            _commonClass = commonClass;
        }

        public async Task<ReturnGenericData<GroupAccessModel>> GetGroupAccess(int id)
        {
            var obj = new ReturnGenericData<GroupAccessModel>();
            var status = new ReturnGenericStatus();
            obj.StatusCode = "01";
            try
            {
                var groupAccessModel = await _dBContext.GroupAccessModels.FindAsync(id);
                if (groupAccessModel == null)
                {
                    obj.StatusMessage = "Not found";
                }
                else
                {
                    groupAccessModel.GroupDesc = _commonClass.GetGroupDesc(groupAccessModel.GroupId);
                    groupAccessModel.MenuIDsDesc = _commonClass.GetMenuIDsDesc(groupAccessModel.MenuIds);
                    groupAccessModel.SelectedMenuIDs = _commonClass.GetGroupDesc(groupAccessModel.MenuIds).Split(",");
                    obj.StatusCode = "00";
                    obj.StatusMessage = "SUCCESS";
                    obj.Data = groupAccessModel;
                }

            }
            catch (Exception ex)
            {
                status = _commonClass.MsgError(ex.Message);
                obj.StatusMessage = status.StatusMessage;
                obj.StatusCode = status.StatusCode;
            }
            return obj;
        }

        public async Task<ReturnGenericStatus> GroupAccessCreate(GroupAccessModel groupAccessModel)
        {
            var obj = new ReturnGenericStatus();
            obj.StatusCode = "01";
            try
            {
                if (!GroupAccessModelExists(groupAccessModel.GroupId))
                {
                    groupAccessModel.MenuIds = string.Join(",", groupAccessModel.SelectedMenuIDs);
                    _dBContext.Add(groupAccessModel);
                    await _dBContext.SaveChangesAsync();
                    obj.StatusCode = "00";
                    obj.StatusMessage = "SUCCESS";
                }
                else
                {
                    obj.StatusMessage = "Group Code Already Exist. Please Change.";
                }
            }
            catch (Exception ex)
            {
                obj.StatusMessage = ex.Message;
            }
            return obj;
        }

        public async Task<ReturnGenericList<SelectListItem>> GetGroupAccessMenuList()
        {
            var obj = new ReturnGenericList<SelectListItem>();
            obj.StatusCode = "01";
            try
            {
                var menuList = await _dBContext.MenuModels
                                .Where(e => e.RootMenu == true)
                                .OrderBy(e => e.MenuDesc)
                                .Select(e => new SelectListItem
                                {
                                    Value = e.MenuCode,
                                    Text = e.MenuDesc,
                                    IsChecked = false
                                })
                                .ToListAsync();
                obj.StatusCode = "00";
                obj.StatusMessage = "SUCCESS";
                obj.Data = menuList;
            }
            catch (Exception ex)
            {
                obj.StatusMessage = ex.Message;
            }
            return obj;
        }

        public async Task<ReturnGenericStatus> GroupAccessDeleteConfirmed(int id)
        {
            var obj = new ReturnGenericStatus();
            obj.StatusCode = "01";
            try
            {
                var groupAccessModel = await _dBContext.GroupAccessModels.FindAsync(id);
                if (groupAccessModel.Isdeleted == false)
                    groupAccessModel.Isdeleted = true;

                //_context.GroupAccessModel.Remove(groupAccessModel);
                _dBContext.Update(groupAccessModel);
                await _dBContext.SaveChangesAsync();
                obj.StatusCode = "00";
                obj.StatusMessage = "SUCCESS";
            }
            catch (Exception ex)
            {
                obj.StatusMessage = ex.Message;
            }

            return obj;
        }

        public async Task<ReturnGenericData<GroupAccessModel>> GroupAccessDetails(int id)
        {
            var obj = new ReturnGenericData<GroupAccessModel>();
            var status = new ReturnGenericStatus();
            obj.StatusCode = "01";
            try
            {

                var groupAccessModel = await _dBContext.GroupAccessModels.Where(m => m.Id == id).FirstOrDefaultAsync();
                if (groupAccessModel == null)
                {
                    obj.StatusMessage = "Not found";
                }
                else
                {
                    obj.StatusCode = "00";
                    obj.StatusMessage = "SUCCESS";
                    groupAccessModel.GroupDesc = _commonClass.GetGroupDesc(groupAccessModel.GroupId);
                    groupAccessModel.MenuIDsDesc = _commonClass.GetMenuIDsDesc(groupAccessModel.MenuIds);
                    obj.Data = groupAccessModel;
                }
            }
            catch (Exception ex)
            {
                status = _commonClass.MsgError(ex.Message);
                obj.StatusMessage = status.StatusMessage;
                obj.StatusCode = status.StatusCode;
            }
            return obj;
        }

        public async Task<ReturnGenericStatus> GroupAccessEdit(GroupAccessModel groupAccessModel)
        {
            var obj = new ReturnGenericStatus();
            obj.StatusCode = "01";
            try
            {
                groupAccessModel.MenuIds = string.Join(",", groupAccessModel.SelectedMenuIDs);
                _dBContext.Update(groupAccessModel);
                await _dBContext.SaveChangesAsync();
                obj.StatusCode = "00";
                obj.StatusMessage = "SUCCESS";
            }
            catch (DbUpdateConcurrencyException ex)
            {
                if (!GroupAccessModelExists(groupAccessModel.Id))
                {
                    obj.StatusCode = "Not Found";
                }
                else
                {
                    obj.StatusCode = ex.Message;
                }
            }
            return obj;
        }

        public async Task<PaginatedOutput<GroupAccessModel>> GetGroupAccessList(int? page)
        {
            try
            {
                IQueryable<GroupAccessModel> groupAccessModel = _dBContext.GroupAccessModels;
              
                PaginatedList<GroupAccessModel> Paginate = await PaginatedList<GroupAccessModel>.CreateAsync(groupAccessModel, page ?? 1);
                
                var data = new PaginatedOutput<GroupAccessModel>(Paginate);
                foreach (var model in data.Data)
                {
                    model.GroupDesc = _commonClass.GetGroupDesc(model.GroupId);
                    model.MenuIDsDesc = _commonClass.GetMenuIDsDesc(model.MenuIds);
                }

                return data;
            }
            catch(Exception ex)
            {
                return null;
            }
           
        }
        private bool GroupAccessModelExists(int id)
        {
            return _dBContext.GroupAccessModels.Any(e => e.Id == id && e.Isdeleted == false);
        }

        private bool GroupAccessModelExists(string GroupID)
        {
            return _dBContext.GroupAccessModels.Any(e => e.GroupId == GroupID && e.Isdeleted == false);
        }

    }
}
