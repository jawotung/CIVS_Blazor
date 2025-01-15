using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IUnitOfWork
    {
        IAuthRepository Auth { get; }
        IApiServiceRepository ApiService { get; }
        IBranchRepository Branch { get; }
        IBranchModelAuxesRepository BranchModelAux { get; }
        IAmountLimitsRepository AmountLimits { get; }
        IGroupRepository Group { get; }
        IGroupAccessRepositories GroupAccess { get; }
        IGroupMemberRepositories GroupMember { get; }
        IEmailTemplateRepository EmailTemplate { get; }
        IMenuRepository Menu { get; }
        IRejectReasonRepository RejectReason { get; }
        IUserAmountLimitRepository UserAmountLimit {  get; }
        IUserTypeRepository UserType { get; }
        IUserRepository User { get; }
        IInwardClearingChequeRepository InwardClearingCheque { get; }
        IInwardClearingChequeUploadRepository InwardClearingChequeUpload { get; }
        IInwardClearingChequeReportRepository InwardClearingChequeReport { get; }
        ISVSRepository SVS { get; }
        IRolePolicyService RolePolicyService { get; }
    }
}
