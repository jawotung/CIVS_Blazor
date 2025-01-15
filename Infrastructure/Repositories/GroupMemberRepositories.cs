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
    public class GroupMemberRepositories : IGroupMemberRepositories
    {
        private readonly AppDbContext _dBContext;
        private readonly ADClass _adClass;
        private readonly CommonClass _commonClass;
        public GroupMemberRepositories(AppDbContext dBContext, ADClass addClass, CommonClass commonClass)
        {
            _dBContext = dBContext;
            _adClass = addClass;
            _commonClass = commonClass;
        }

        public async Task<ReturnGenericData<GroupMemberModel>> GetGroupMember(int id)
        {
            var obj = new ReturnGenericData<GroupMemberModel>();
            obj.StatusCode = "01";

            try
            {
                var groupMemberModel = await _dBContext.GroupMemberModels.FindAsync(id);
                if (groupMemberModel == null)
                {
                    obj.StatusMessage = "Not found";
                }
                else
                {
                    groupMemberModel.GroupDesc = _commonClass.GetGroupDesc(groupMemberModel.GroupId);
                    groupMemberModel.UserTypesDesc = _commonClass.GetMenuIDsDesc(groupMemberModel.UserTypes);
                    groupMemberModel.SelectedMemberIDs = groupMemberModel.UserTypes.Split(","); 
                    obj.StatusCode = "00";
                    obj.StatusMessage = "SUCCESS";
                    obj.Data = groupMemberModel;
                }
            }
            catch(Exception ex)
            {
                obj.StatusMessage = ex.Message;
            }
            return obj;
        }

        public async Task<PaginatedOutput<GroupMemberModel>> GetGroupMemberList(int? page)
        {
            try
            {
                IQueryable<GroupMemberModel> groupMemberModel = _dBContext.GroupMemberModels;

                PaginatedList<GroupMemberModel> Paginate = await PaginatedList<GroupMemberModel>.CreateAsync(groupMemberModel, page ?? 1);

                var data = new PaginatedOutput<GroupMemberModel>(Paginate);
                foreach (var model in data.Data)
                {
                    model.GroupDesc = _commonClass.GetGroupDesc(model.GroupId);
                    model.UserTypesDesc = _commonClass.GetUserTypeIDsDesc(model.UserTypes);
                }

                return data;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<ReturnGenericList<SelectListItem>> GetUserTypeList()
        {
            var obj = new ReturnGenericList<SelectListItem>();
            obj.StatusCode = "01";
            try
            {
                var menuList = await _dBContext.UserTypeModels
                                .Select(e => new SelectListItem
                                {
                                    Value = e.UserTypeCode,
                                    Text = e.UserTypeDesc,
                                    IsChecked = false
                                })
                                .ToListAsync();
                obj.StatusMessage = "00";
                obj.StatusMessage = "SUCCESS";
                obj.Data = menuList;
            }
            catch (Exception ex)
            {
                obj.StatusMessage = ex.Message;
            }
            return obj;
        }

        public Task<ReturnGenericStatus> GroupMemberCreate(GroupMemberModel groupMemberModel)
        {

            throw new NotImplementedException();
        }

        public async Task<ReturnGenericStatus> GroupMemberDeleteConfirmed(int id)
        {
            var obj = new ReturnGenericStatus();
            obj.StatusCode = "01";
            try
            {
                var groupMemberModel = await _dBContext.GroupMemberModels.FindAsync(id);
                if (groupMemberModel.Isdeleted == false)
                    groupMemberModel.Isdeleted = true;

                //_context.GroupMemberModel.Remove(groupMemberModel);
                _dBContext.Update(groupMemberModel);
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

        public async Task<ReturnGenericData<GroupMemberModel>> GroupMemberDetails(int id)
        {
            var obj = new ReturnGenericData<GroupMemberModel>();
            obj.StatusCode = "01";
            try
            {
                if (id == null)
                {
                    obj.StatusMessage = "Not found";
                }
                else
                {
                    var groupMemberModel = await _dBContext.GroupMemberModels
                                        .FirstOrDefaultAsync(m => m.Id == id);
                    if (groupMemberModel == null)
                    {
                        obj.StatusMessage = "Not found";
                    }
                    else
                    {
                        obj.StatusCode = "00";
                        obj.StatusMessage = "SUCCESS";
                        groupMemberModel.GroupDesc = _commonClass.GetGroupDesc(groupMemberModel.GroupId);
                        groupMemberModel.UserTypesDesc = _commonClass.GetUserTypeIDsDesc(groupMemberModel.UserTypes);
                        obj.Data = groupMemberModel;
                    }
                }

                
            }
            catch(Exception ex)
            {
                obj.StatusMessage = ex.Message;
            }

            return obj;
        }

        public async Task<ReturnGenericStatus> GroupMemberEdit(GroupMemberModel groupMemberModel)
        {
            var obj = new ReturnGenericStatus();
            obj.StatusCode = "01";
            try
            {
                if (groupMemberModel.SelectedMemberIDs.IsNull())
                {
                    obj.StatusMessage = "Please Select Member.";
                }
                else
                {
                    if (!MemberIDsExist(groupMemberModel.GroupId, groupMemberModel.SelectedMemberIDs, out string memberID))
                    {
                        groupMemberModel.UserTypes = string.Join(",", groupMemberModel.SelectedMemberIDs);
                        _dBContext.Update(groupMemberModel);
                        await _dBContext.SaveChangesAsync();
                        obj.StatusCode = "00";
                        obj.StatusMessage = "SUCCESS";
                    }
                    else
                    {
                        obj.StatusMessage = "Member " + _commonClass.GetUserTypeIDsDesc(memberID) + " Already Exist in Another Group. Please Remove.";
                    }
                }
            }
            catch (DbUpdateConcurrencyException ex)
            {
                if (!GroupMemberModelExists(groupMemberModel.Id))
                {
                    obj.StatusMessage = "Not found";
                }
                else
                {
                    obj.StatusMessage = ex.Message;
                }
            }

            return obj;
        }

        private bool GroupMemberModelExists(int id)
        {
            return _dBContext.GroupMemberModels.Any(e => e.Id == id);
        }

        private bool GroupMemberModelExists(string GroupID)
        {
            return _dBContext.GroupMemberModels.Any(e => e.GroupId == GroupID && e.Isdeleted == false);
        }

        private bool MemberIDsExist(string GroupID, IList<string> SelectedMemberIDs, out string memberID)
        {
            foreach (string memberIDS in SelectedMemberIDs)
            {
                if (_dBContext.GroupMemberModels.Any(e => e.UserTypes.Contains(memberIDS) && e.GroupId != GroupID))
                {
                    memberID = memberIDS;
                    return true;
                }
            }
            memberID = "";
            return false;
        }
    }
}
