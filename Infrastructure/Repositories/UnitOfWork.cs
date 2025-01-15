using Application.Interfaces;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        public UnitOfWork(
            IAuthRepository auth,
            IApiServiceRepository apiService,
            IBranchRepository branch,
            IBranchModelAuxesRepository branchModelAux,
            IAmountLimitsRepository amountLimits,
            IGroupRepository group,
            IGroupAccessRepositories groupAccess,
            IGroupMemberRepositories groupMember,
            IEmailTemplateRepository emailTemplate,
            IMenuRepository menu,
            IRejectReasonRepository rejectReason,
            IUserAmountLimitRepository userAmountLimit,
            IUserTypeRepository userType,
            IUserRepository user,
            IInwardClearingChequeUploadRepository inwardClearingChequeUpload,
            IInwardClearingChequeRepository inwardClearingCheque,
            IInwardClearingChequeReportRepository inwardClearingChequeReport,
            ISVSRepository sVSRepository,
            IRolePolicyService rolePolicyService
        )
        {
            Auth = auth;
            ApiService = apiService;
            Group = group;
            GroupAccess = groupAccess;
            GroupMember = groupMember;
            Branch = branch;
            BranchModelAux = branchModelAux;
            AmountLimits = amountLimits;
            EmailTemplate = emailTemplate;
            Menu = menu;
            RejectReason = rejectReason;
            UserAmountLimit = userAmountLimit;
            UserType = userType;
            User = user;
            InwardClearingChequeUpload = inwardClearingChequeUpload;
            InwardClearingCheque = inwardClearingCheque;
            InwardClearingChequeReport = inwardClearingChequeReport;
            SVS = sVSRepository;
            RolePolicyService = rolePolicyService;
        }
        public IAuthRepository Auth { get; }
        public IApiServiceRepository ApiService { get; }
        public IBranchRepository Branch { get; }
        public IBranchModelAuxesRepository BranchModelAux { get; }
        public IAmountLimitsRepository AmountLimits { get; }
        public IGroupRepository Group { get; }
        public IGroupAccessRepositories GroupAccess { get; }
        public IGroupMemberRepositories GroupMember { get; }
        public IEmailTemplateRepository EmailTemplate { get; }
        public IMenuRepository Menu { get; }
        public IRejectReasonRepository RejectReason { get; }
        public IUserAmountLimitRepository UserAmountLimit { get; }
        public IUserTypeRepository UserType { get; }
        public IUserRepository User { get; }
        public IInwardClearingChequeUploadRepository InwardClearingChequeUpload { get; }
        public IInwardClearingChequeRepository InwardClearingCheque { get; }
        public IInwardClearingChequeReportRepository InwardClearingChequeReport { get; }
        public ISVSRepository SVS { get; }
        public IRolePolicyService RolePolicyService { get; }
    }
}
