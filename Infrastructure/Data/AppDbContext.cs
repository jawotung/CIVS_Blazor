using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace WebAPI;

public partial class AppDbContext : DbContext
{
    public AppDbContext()
    {
    }

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<AmountLimitsModel> AmountLimitsModels { get; set; }

    public virtual DbSet<AuditLog> AuditLogs { get; set; }

    public virtual DbSet<BranchModel> BranchModels { get; set; }

    public virtual DbSet<BranchModelAux> BranchModelAuxes { get; set; }

    public virtual DbSet<BranchModelOrg> BranchModelOrgs { get; set; }

    public virtual DbSet<ChequeAccountDetail> ChequeAccountDetails { get; set; }

    public virtual DbSet<EmailTemplateModel> EmailTemplateModels { get; set; }

    public virtual DbSet<GroupAccessModel> GroupAccessModels { get; set; }

    public virtual DbSet<GroupMemberModel> GroupMemberModels { get; set; }

    public virtual DbSet<GroupModel> GroupModels { get; set; }

    public virtual DbSet<InwardClearingChequeDetailsModel> InwardClearingChequeDetailsModels { get; set; }

    public virtual DbSet<InwardClearingChequeHistoryModel> InwardClearingChequeHistoryModels { get; set; }

    public virtual DbSet<InwardClearingChequeImageModel> InwardClearingChequeImageModels { get; set; }

    public virtual DbSet<MenuModel> MenuModels { get; set; }

    public virtual DbSet<RejectReasonModel> RejectReasonModels { get; set; }

    public virtual DbSet<UserAmountLimitModel> UserAmountLimitModels { get; set; }

    public virtual DbSet<UserAuthenticationModel> UserAuthenticationModels { get; set; }

    public virtual DbSet<UserModel> UserModels { get; set; }

    public virtual DbSet<UserTypeModel> UserTypeModels { get; set; }

//    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
//#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
//        => optionsBuilder.UseSqlServer("Server=DEV-ADD\\\\\\\\MSSQL2019DEV,1433;Database=BOCCIVSDEV;User Id=BOCCIVSUser;Password=B0CC1VSP@promptw0rd;Encrypt=true;trustServerCertificate=true;Integrated Security=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AmountLimitsModel>(entity =>
        {
            entity.ToTable("AmountLimitsModel");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.AllowedAction).HasMaxLength(50);
            entity.Property(e => e.AmountLimitsCode).HasMaxLength(10);
            entity.Property(e => e.AmountLimitsDesc).HasMaxLength(200);
            entity.Property(e => e.Isdeleted).HasColumnName("ISDeleted");
        });

        modelBuilder.Entity<AuditLog>(entity =>
        {
            entity.ToTable("AuditLog");

            entity.HasIndex(e => e.LogTime, "IX_AuditLog_LogTime");

            entity.Property(e => e.Id).HasColumnName("ID");
        });

        modelBuilder.Entity<BranchModel>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_BranchModel_2");

            entity.ToTable("BranchModel");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.BranchBrstn)
                .HasMaxLength(10)
                .HasDefaultValue("")
                .HasColumnName("BranchBRSTN");
            entity.Property(e => e.BranchCode).HasMaxLength(10);
            entity.Property(e => e.BranchDesc).HasMaxLength(50);
            entity.Property(e => e.Isdeleted).HasColumnName("ISDeleted");
        });

        modelBuilder.Entity<BranchModelAux>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_BranchBuddyModel");

            entity.ToTable("BranchModelAux");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.BranchBcp).HasColumnName("BranchBCP");
            entity.Property(e => e.BranchBuddyCode).HasMaxLength(10);
            entity.Property(e => e.BranchCode).HasMaxLength(10);
        });

        modelBuilder.Entity<BranchModelOrg>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("BranchModel_org");

            entity.Property(e => e.BranchBrstn)
                .HasMaxLength(10)
                .HasColumnName("BranchBRSTN");
            entity.Property(e => e.BranchCode).HasMaxLength(10);
            entity.Property(e => e.BranchDesc).HasMaxLength(50);
            entity.Property(e => e.Id)
                .ValueGeneratedOnAdd()
                .HasColumnName("ID");
            entity.Property(e => e.Isdeleted).HasColumnName("ISDeleted");
        });

        modelBuilder.Entity<ChequeAccountDetail>(entity =>
        {
            entity.HasIndex(e => e.ChequeImageLinkedKey, "NCIDX_ChequeAccountDetails_ChkImgLnkKey");

            entity.HasIndex(e => e.EffectivityDate, "NCIDX_ChequeAccountDetails_EffectivityDate");

            entity.HasIndex(e => e.StatusAsOfDate, "NCIDX_ChequeAccountDetails_StatusAsOfDate");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.AccountName)
                .HasMaxLength(1000)
                .IsUnicode(false);
            entity.Property(e => e.AccountNumber)
                .HasMaxLength(12)
                .IsUnicode(false);
            entity.Property(e => e.AccountStatus)
                .HasMaxLength(1000)
                .IsUnicode(false);
            entity.Property(e => e.ChequeImageLinkedKey)
                .HasMaxLength(60)
                .IsUnicode(false);
            entity.Property(e => e.EffectivityDate).HasColumnType("datetime");
            entity.Property(e => e.StatusAsOfDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<EmailTemplateModel>(entity =>
        {
            entity.ToTable("EmailTemplateModel");

            entity.Property(e => e.Id).HasColumnName("ID");
        });

        modelBuilder.Entity<GroupAccessModel>(entity =>
        {
            entity.ToTable("GroupAccessModel");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.GroupId)
                .HasMaxLength(10)
                .HasColumnName("GroupID");
            entity.Property(e => e.Isdeleted).HasColumnName("ISDeleted");
            entity.Property(e => e.MenuIds).HasColumnName("MenuIDs");
        });

        modelBuilder.Entity<GroupMemberModel>(entity =>
        {
            entity.ToTable("GroupMemberModel");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.GroupId)
                .HasMaxLength(10)
                .HasColumnName("GroupID");
            entity.Property(e => e.Isdeleted).HasColumnName("ISDeleted");
        });

        modelBuilder.Entity<GroupModel>(entity =>
        {
            entity.ToTable("GroupModel");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.GroupCode).HasMaxLength(10);
            entity.Property(e => e.GroupDesc).HasMaxLength(50);
            entity.Property(e => e.Isdeleted).HasColumnName("ISDeleted");
        });

        modelBuilder.Entity<InwardClearingChequeDetailsModel>(entity =>
        {
            entity
                .ToTable("InwardClearingChequeDetailsModel");

            entity.HasKey(e => e.Id);//.HasName("PK_InwardClearingChequeDetailsModel_2");

            entity.HasIndex(e => new { e.AccountNumber, e.Brstn, e.CheckStatus, e.ApprovedDateTime }, "IX_Details_AcctNo_BRSTN_ChkStat_AppveDate");

            entity.HasIndex(e => new { e.AccountNumber, e.Brstn, e.CheckStatus, e.EffectivityDate }, "IX_Details_AcctNo_BRSTN_ChkStat_EffDate");

            entity.HasIndex(e => new { e.AccountNumber, e.Brstn, e.CheckStatus, e.VerifiedDateTime, e.BranchCode }, "IX_Details_AcctNo_BRSTN_ChkStat_VrfyDate_BrCode");

            entity.HasIndex(e => new { e.AccountNumber, e.Brstn, e.CheckStatus, e.VerifiedDateTime, e.ClearingOfficer }, "IX_Details_AcctNo_BRSTN_ChkStat_VrfyDate_ClrOfcr");

            entity.HasIndex(e => new { e.AccountNumber, e.Brstn, e.EffectivityDate }, "IX_Details_AcctNo_BRSTN_EffDate");

            entity.HasIndex(e => new { e.AccountNumber, e.CheckNumber, e.CheckAmount, e.CheckStatus, e.EffectivityDate }, "IX_Details_AcctNo_ChkNo_ChkAmt_ChkStat_EffDate");

            entity.HasIndex(e => new { e.AccountNumber, e.CheckNumber, e.CheckAmount, e.EffectivityDate }, "IX_Details_AcctNo_ChkNo_ChkAmt_EffDate");

            entity.HasIndex(e => new { e.BranchCode, e.CheckStatus }, "IX_Details_BrCode_ChkStat");

            entity.HasIndex(e => e.CheckAmount, "IX_Details_ChkAmt");

            entity.HasIndex(e => e.ChequeImageLinkedKey, "IX_Details_ChkImgLnkKey");

            entity.HasIndex(e => new { e.EffectivityDate, e.AccountNumber, e.CheckNumber, e.CheckAmount }, "IX_Details_EffDate_AcctNo_ChkNo_ChkAmt");

            entity.HasIndex(e => e.Id, "IX_Details_Id");

            entity.Property(e => e.AccountNumber).HasMaxLength(12);
            entity.Property(e => e.ApprovedBy).HasMaxLength(50);
            entity.Property(e => e.BranchCode).HasMaxLength(10);
            entity.Property(e => e.Brstn)
                .HasMaxLength(10)
                .IsFixedLength()
                .HasColumnName("BRSTN");
            entity.Property(e => e.CheckNumber).HasMaxLength(10);

            entity.Property(e => e.CheckStatus).HasMaxLength(25);
            entity.Property(e => e.ChequeImageLinkedKey).HasMaxLength(60);
            entity.Property(e => e.ClearingOfficer).HasMaxLength(50);
            entity.Property(e => e.Id)
                .ValueGeneratedOnAdd()
                .HasColumnName("ID");
            entity.Property(e => e.Reason).HasMaxLength(10);
            entity.Property(e => e.VerifiedBy).HasMaxLength(50);
        });

        modelBuilder.Entity<InwardClearingChequeHistoryModel>(entity =>
        {
            entity.ToTable("InwardClearingChequeHistoryModel");

            entity.HasIndex(e => new { e.ChequeImageLinkedKey, e.CheckStatusTo }, "IX_History_ChkImgLnkKey");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.BranchCode).HasMaxLength(10);
            entity.Property(e => e.CheckStatusFrom).HasMaxLength(25);
            entity.Property(e => e.CheckStatusTo).HasMaxLength(25);
            entity.Property(e => e.ChequeImageLinkedKey).HasMaxLength(60);
            entity.Property(e => e.ClearingOfficer).HasMaxLength(50);
            entity.Property(e => e.Reason).HasMaxLength(10);
        });

        modelBuilder.Entity<InwardClearingChequeImageModel>(entity =>
        {
            entity.ToTable("InwardClearingChequeImageModel");

            entity.HasIndex(e => e.ChequeImageFileName, "IX_Image_ChkImgFleName");

            entity.HasIndex(e => e.ChequeImageLinkedKey, "IX_Image_ChkImgLnkKey");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.ChequeImageFileContentType).HasMaxLength(255);
            entity.Property(e => e.ChequeImageFileName).HasMaxLength(255);
            entity.Property(e => e.ChequeImageLinkedKey).HasMaxLength(60);
        });

        modelBuilder.Entity<MenuModel>(entity =>
        {
            entity.ToTable("MenuModel");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Isdeleted).HasColumnName("ISDeleted");
            entity.Property(e => e.MenuCode).HasMaxLength(10);
            entity.Property(e => e.MenuDesc).HasMaxLength(100);
        });

        modelBuilder.Entity<RejectReasonModel>(entity =>
        {
            entity.ToTable("RejectReasonModel");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Isdeleted).HasColumnName("ISDeleted");
            entity.Property(e => e.RejectReasonCode).HasMaxLength(10);
            entity.Property(e => e.RejectReasonDesc).HasMaxLength(100);
        });

        modelBuilder.Entity<UserAmountLimitModel>(entity =>
        {
            entity.ToTable("UserAmountLimitModel");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.AmountLimitId).HasMaxLength(10);
            entity.Property(e => e.Isdeleted).HasColumnName("ISDeleted");
            entity.Property(e => e.UserId).HasMaxLength(50);
        });

        modelBuilder.Entity<UserAuthenticationModel>(entity =>
        {
            entity.ToTable("UserAuthenticationModel");

            entity.Property(e => e.AccessToken).IsUnicode(false);
            entity.Property(e => e.AccessTokenExpiry).HasColumnType("datetime");
            entity.Property(e => e.AuthType)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.CreatedBy)
                .HasMaxLength(150)
                .IsUnicode(false)
                .HasDefaultValueSql("(suser_sname())");
            entity.Property(e => e.IpAddress)
                .HasMaxLength(150)
                .IsUnicode(false);
            entity.Property(e => e.IsValid).HasDefaultValue(true);
            entity.Property(e => e.RefreshToken)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.RefreshTokenExpiry).HasColumnType("datetime");
            entity.Property(e => e.UpdatedAt).HasColumnType("datetime");
            entity.Property(e => e.UpdatedBy)
                .HasMaxLength(150)
                .IsUnicode(false);
            entity.Property(e => e.UserAgent)
                .HasMaxLength(150)
                .IsUnicode(false);
            entity.Property(e => e.UserId)
                .HasMaxLength(50)
                .HasColumnName("UserID");
        });

        modelBuilder.Entity<UserModel>(entity =>
        {
            entity.ToTable("UserModel");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.EmployeeNumber)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.Isdeleted).HasColumnName("ISDeleted");
            entity.Property(e => e.Isdisabled).HasColumnName("ISDisabled");
            entity.Property(e => e.LastLoginSession)
                .IsUnicode(false)
                .HasDefaultValue("");
            entity.Property(e => e.UserId)
                .HasMaxLength(50)
                .HasColumnName("UserID");
            entity.Property(e => e.UserType).HasMaxLength(10);
        });

        modelBuilder.Entity<UserTypeModel>(entity =>
        {
            entity.ToTable("UserTypeModel");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Isdeleted).HasColumnName("ISDeleted");
            entity.Property(e => e.UserTypeCode).HasMaxLength(10);
            entity.Property(e => e.UserTypeDesc).HasMaxLength(50);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
