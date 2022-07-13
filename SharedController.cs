namespace Booking.Site.Controllers
{
    using Booking.Data.Data;
    using Booking.Data.DB.Heron28;
    using Booking.Library.Classes;
    using Booking.Site.Models;
    using Kendo.Mvc.Extensions;
    using Kendo.Mvc.UI;
    using LinqKit;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Microsoft.CodeAnalysis;
    using Microsoft.Extensions.Caching.Memory;
    using Microsoft.Extensions.Logging;
    using Newtonsoft.Json.Linq;
    using System;
    using System.Collections.Generic;
    using System.Data.SqlClient;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Telerik.Web.Captcha;

    /// <summary>
    /// Defines the <see cref="SharedController" />.
    /// </summary>
    public class SharedController : BaseController2
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SharedController"/> class.
        /// </summary>
        /// <param name="loEnv">The loEnv<see cref="IWebHostEnvironment"/>.</param>
        /// <param name="loSecurityManager">The loSecurityManager<see cref="UserManager{Booking.Data.DB.Extensions.IdentityExtend.User}"/>.</param>
        /// <param name="loLoginManager">The loLoginManager<see cref="SignInManager{Booking.Data.DB.Extensions.IdentityExtend.User}"/>.</param>
        /// <param name="loRoleManager">The loRoleManager<see cref="RoleManager{Booking.Data.DB.Extensions.IdentityExtend.Role}"/>.</param>
        /// <param name="loDbContext">The loDbContext<see cref="Booking.Data.DB.Extensions.IdentityExtend.DbContext"/>.</param>
        /// <param name="loHeron28Context">The loHeron28Context<see cref="Booking.Data.DB.Heron28.Heron28Context"/>.</param>
        /// <param name="loRecaptcha">The loRecaptcha<see cref="IRecaptchaService"/>.</param>
        /// <param name="loIMemoryCache">The loIMemoryCache<see cref="IMemoryCache"/>.</param>
        /// <param name="loLogger">The loLogger<see cref="ILogger{SetupController}"/>.</param>
        /// <param name="loIHostApplicationLifetime">The loIHostApplicationLifetime<see cref="Microsoft.Extensions.Hosting.IHostApplicationLifetime"/>.</param>
        public SharedController(IWebHostEnvironment loEnv,
            UserManager<Booking.Data.DB.Extensions.IdentityExtend.User> loSecurityManager,
            SignInManager<Booking.Data.DB.Extensions.IdentityExtend.User> loLoginManager,
            RoleManager<Booking.Data.DB.Extensions.IdentityExtend.Role> loRoleManager,
            Booking.Data.DB.Extensions.IdentityExtend.DbContext loDbContext,
            Booking.Data.DB.Heron28.Heron28Context loHeron28Context, 
            IMemoryCache loIMemoryCache, ILogger<SetupController> loLogger,
            Microsoft.Extensions.Hosting.IHostApplicationLifetime loIHostApplicationLifetime) : base(loEnv,
            loSecurityManager, loLoginManager, loRoleManager, loDbContext, loHeron28Context, 
            loIMemoryCache, loLogger, loIHostApplicationLifetime)
        {
        }

        // Removed for brevity

        /// <summary>
        /// The _ReportViewer.
        /// </summary>
        /// <param name="loReportData">The loReportData<see cref="Booking.Site.Models.Shared.ReportData"/>.</param>
        /// <param name="lnReport">The lnReport<see cref="int"/>.</param>
        /// <param name="lsData">The lsData<see cref="string"/>.</param>
        /// <returns>The <see cref="PartialViewResult"/>.</returns>
        public PartialViewResult _ReportViewer(Booking.Site.Models.Shared.ReportData loReportData, int lnReport, string lsData)
        {
            Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction loIDbContextTransaction = this._oHeron28Context.Database.BeginTransaction();
            try
            {
                object loData2 = null;
                if (this._oIMemoryCache.TryGetValue("Buku_sData2" + Booking.Data.Classes.Configuration.SessionId(this.HttpContext.Session.GetString("Buku_sCompanyId"), Booking.Site.Classes.Helper.GetUserName(this.HttpContext), Booking.Site.Classes.Helper.GetIPAddress(this.HttpContext), this.HttpContext.Session.Id), out loData2))
                {
                    string lsData2 = (string)loData2;
                    Booking.Data.DB.Extensions.IdentityExtend.User loUser = this.SelectedUser(lsData2);
                    if (loUser != null)
                    {
                        Guid lgCompanyId = Guid.Parse(Booking.Library.Classes.Utility.DecodeSerialisedData(lsData2, "gCompanyId"));
                        Booking.Data.DB.Heron28.Entity loEntity = this._oHeron28Context.Entities.Where(E => E.EGId == lgCompanyId).FirstOrDefault();
                        if (loEntity != null)
                        {
                            loReportData._nReport = lnReport;
                            loReportData._sData = lsData;
                            object loImage = null;
                            Booking.Data.DB.Heron28.Entity loImagesEntity = this._oHeron28Context.Entities.AsEnumerable().Where(E => E.ECompanyEGId == lgCompanyId && E.ENType == (short)Booking.Library.Classes.Enums.Entities.Images && !E.Deleted).FirstOrDefault();
                            if (loImagesEntity != null)
                            {
                                if (!loEntity.GetDataSetDataset.Company[0].IsCO_nBrandingId1Null() && loEntity.GetDataSetDataset.Company[0].CO_nBrandingId1 != 0 && loImagesEntity.GetDataSetDataset.Images.FindByIMG_nId(loEntity.GetDataSetDataset.Company[0].CO_nBrandingId1) != null)
                                {
                                    loImage = loImagesEntity.GetDataSetDataset.Images.FindByIMG_nId(loEntity.GetDataSetDataset.Company[0].CO_nBrandingId1).IMG_oImage;
                                }
                                else
                                {
                                    loImage = Booking.Library.Classes.Utility.StreamToBytes(System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream("Booking.Site.Resources.Blank.jpg"));
                                }
                            }
                            else
                            {
                                loImage = Booking.Library.Classes.Utility.StreamToBytes(System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream("Booking.Site.Resources.Blank.jpg"));
                            }
                            if (loImage != null) // We should always have an image because if none has been set the blank is used.
                            {
                                switch ((Booking.Library.Classes.Enums.Reports)lnReport)
                                {
                                    case Booking.Library.Classes.Enums.Reports.AdHocSLInvoice1:
                                    case Booking.Library.Classes.Enums.Reports.AdHocSLInvoice2:
                                    case Booking.Library.Classes.Enums.Reports.AdHocSLCreditNote1:
                                    case Booking.Library.Classes.Enums.Reports.AdHocSLCreditNote2:
                                    case Booking.Library.Classes.Enums.Reports.AdHocPLInvoice1:
                                    case Booking.Library.Classes.Enums.Reports.AdHocPLInvoice2:
                                    case Booking.Library.Classes.Enums.Reports.AdHocPLCreditNote1:
                                    case Booking.Library.Classes.Enums.Reports.AdHocPLCreditNote2:
                                        {
                                            loReportData._sReport = Booking.Library.Classes.Utility.StringValueOf((Booking.Library.Classes.Enums.Reports)lnReport);
                                            loReportData._gId =
                                                Guid.Parse(Booking.Library.Classes.Utility.DecodeSerialisedData(lsData, "gTransactionId"));
                                        }
                                        break;
                                    case Booking.Library.Classes.Enums.Reports.AdHocBatchesInProgress:
                                        {
                                            loReportData._gInstanceId = Guid.Parse(Booking.Library.Classes.Utility.DecodeSerialisedData(lsData, "gInstanceId"));
                                            loReportData._sReport = Booking.Library.Classes.Utility.DecodeSerialisedData(lsData, "sReport");
                                            loReportData._gId =
                                                Guid.Parse(Booking.Library.Classes.Utility.DecodeSerialisedData(lsData, "gBatchId"));
                                        }
                                        break;
                                    case Booking.Library.Classes.Enums.Reports.TimeBatches:
                                        {
                                            loReportData._gInstanceId = Guid.Parse(Booking.Library.Classes.Utility.DecodeSerialisedData(lsData, "gInstanceId"));
                                            loReportData._sReport = Booking.Library.Classes.Utility.DecodeSerialisedData(lsData, "sReport");
                                            loReportData._bAll =
                                                bool.Parse(Booking.Library.Classes.Utility.DecodeSerialisedData(lsData, "bAll"));
                                            if (!loReportData._bAll)
                                            {
                                                loReportData._dStart =
                                                    DateTime.Parse(Booking.Library.Classes.Utility.DecodeSerialisedData(lsData, "dStart"));
                                                loReportData._dEnd =
                                                    DateTime.Parse(Booking.Library.Classes.Utility.DecodeSerialisedData(lsData, "dEnd"));
                                                loReportData._dEnd = loReportData._dEnd.Date.AddDays(1).AddSeconds(-1);
                                            }
                                            else
                                            {
                                                loReportData._dStart = DateTime.MinValue;
                                                loReportData._dEnd = DateTime.MaxValue;
                                            }
                                            loReportData._bPosted =
                                                bool.Parse(Booking.Library.Classes.Utility.DecodeSerialisedData(lsData, "bPosted"));
                                        }
                                        break;
                                    case Booking.Library.Classes.Enums.Reports.DisbursementBatch:
                                        {
                                            loReportData._gInstanceId = Guid.Parse(Booking.Library.Classes.Utility.DecodeSerialisedData(lsData, "gInstanceId"));
                                            loReportData._sReport = Booking.Library.Classes.Utility.DecodeSerialisedData(lsData, "sReport");
                                            loReportData._bAll =
                                                bool.Parse(Booking.Library.Classes.Utility.DecodeSerialisedData(lsData, "bAll"));
                                            if (!loReportData._bAll)
                                            {
                                                loReportData._dStart =
                                                    DateTime.Parse(Booking.Library.Classes.Utility.DecodeSerialisedData(lsData, "dStart"));
                                                loReportData._dEnd =
                                                    DateTime.Parse(Booking.Library.Classes.Utility.DecodeSerialisedData(lsData, "dEnd"));
                                                loReportData._dEnd = loReportData._dEnd.Date.AddDays(1).AddSeconds(-1);
                                            }
                                            else
                                            {
                                                loReportData._dStart = DateTime.MinValue;
                                                loReportData._dEnd = DateTime.MaxValue;
                                            }
                                            loReportData._bPosted =
                                                bool.Parse(Booking.Library.Classes.Utility.DecodeSerialisedData(lsData, "bPosted"));
                                        }
                                        break;
                                    case Booking.Library.Classes.Enums.Reports.TrialBalance1:
                                        {
                                            loReportData._gInstanceId = Guid.Parse(Booking.Library.Classes.Utility.DecodeSerialisedData(lsData, "gInstanceId"));
                                            loReportData._sReport = Booking.Library.Classes.Utility.DecodeSerialisedData(lsData, "sReport");
                                            loReportData._dAt = DateTime.Parse(Booking.Library.Classes.Utility.DecodeSerialisedData(lsData, "dAt"));
                                            loReportData._sExcludeBatches = Booking.Library.Classes.Utility.DecodeSerialisedData(lsData, "sExcludeBatches");
                                            loReportData._bContras = false;
                                            if (loReportData._gInstanceId == Guid.Empty)
                                            {
                                                loReportData._gInstanceId = Guid.NewGuid();
                                                DateTime ldRun = DateTime.Now;
                                                Booking.Data.Classes.Configuration.GLTrialBalance[] loGLTrialBalances = Booking.Data.Classes.Helper.GetGLTrialBalanceData(this._oHeron28Context, loReportData._gInstanceId, lgCompanyId.ToString(), loReportData._dAt, loReportData._sExcludeBatches, loReportData._bContras).ToArray();
                                                foreach (Booking.Data.Classes.Configuration.GLTrialBalance loGLTrialBalance in loGLTrialBalances)
                                                {
                                                    Booking.Data.DB.Heron28.GeneralLedgerTrialBalanceReportingDatum loGeneralLedgerTrialBalanceReportingDatum = new GeneralLedgerTrialBalanceReportingDatum()
                                                    {
                                                        GltbGReportInstanceId = loReportData._gInstanceId,
                                                        GltbGAccountCode = loGLTrialBalance._gAccountCode,
                                                        GltbNReport = (short)loReportData._nReport,
                                                        GltbDRun = ldRun,
                                                        GltbGUserId = Guid.Parse(loUser.Id),
                                                        GltbDAtDate = loReportData._dAt,
                                                        GltbSCompanyName = loGLTrialBalance._sCompanyName,
                                                        GltbSCode = loGLTrialBalance._sCode,
                                                        GltbSDescription = loGLTrialBalance._sDescription,
                                                        GltbNDr = loGLTrialBalance._nDr,
                                                        GltbNCr = loGLTrialBalance._nCr,
                                                        GltbOBranding = (byte[])loImage
                                                    };
                                                    this._oHeron28Context.Add(loGeneralLedgerTrialBalanceReportingDatum);
                                                }
                                                List<Booking.Data.Classes.Configuration.GLTransaction> loGLTransactionTemp = Booking.Data.Classes.Helper.GetAllTransactions(this._oHeron28Context, lgCompanyId, loReportData._sExcludeBatches, String.Empty, String.Empty, null, loReportData._dAt, loReportData._bContras, false);
                                                foreach (Booking.Data.Classes.Configuration.GLTransaction loGLTransaction in loGLTransactionTemp)
                                                {
                                                    Booking.Data.DB.Heron28.GeneralLedgerTransactionsReportingDatum loGeneralLedgerTransactionReportingDatum = new GeneralLedgerTransactionsReportingDatum()
                                                    {
                                                        GltGReportInstanceId = loReportData._gInstanceId,
                                                        GltNReport = (short)loReportData._nReport,
                                                        GltDRun = ldRun,
                                                        GltDContraDate = loGLTransaction._dContraDate,
                                                        GltDTransactionDate = loGLTransaction._dTransactionDate,
                                                        GltGAccountCode = loGLTransaction._gAccountCode,
                                                        GltGId = loGLTransaction._gId,
                                                        GltGUserId = Guid.Parse(loUser.Id),
                                                        GltNCr = loGLTransaction._nCr,
                                                        GltNDr = loGLTransaction._nDr,
                                                        GltSAccountDescription = loGLTransaction._sAccountDescription,
                                                        GltSBatchCode = loGLTransaction._sBatchCode,
                                                        GltSCompanyName = loGLTransaction._sCompanyName,
                                                        GltSOtherSide = loGLTransaction._sOtherSide,
                                                        GltSRef = loGLTransaction._sRef,
                                                        GltSTransactionDescription = loGLTransaction._sTransactionDescription,
                                                        GltSType = loGLTransaction._sType,
                                                        GltSAccountCode = loGLTransaction._sAccountCode,
                                                        GltOBranding = (byte[])loImage
                                                    };
                                                    this._oHeron28Context.Add(loGeneralLedgerTransactionReportingDatum);
                                                }
                                                this._oHeron28Context.SaveChanges();
                                                loIDbContextTransaction.Commit();
                                                AllDataSetDataSet loAllDataSetDataSet = new AllDataSetDataSet();
                                                loAllDataSetDataSet.ReportRun.AddReportRunRow(lnReport, lsData, Guid.Parse(loUser.Id));
                                                Booking.Data.DB.Heron28.Audit.CreateAuditEvent(Enums.AuditOperation.RunReport, null, loReportData._gInstanceId, loUser.Id, null, loAllDataSetDataSet, lgCompanyId);
                                            }
                                        }
                                        break;
                                    case Booking.Library.Classes.Enums.Reports.TrialBalance2:
                                        {
                                            loReportData._gInstanceId = Guid.Parse(Booking.Library.Classes.Utility.DecodeSerialisedData(lsData, "gInstanceId"));
                                            loReportData._sReport = Booking.Library.Classes.Utility.DecodeSerialisedData(lsData, "sReport");
                                            loReportData._bContras = false;
                                            if (loReportData._gInstanceId == Guid.Empty)
                                            {
                                                loReportData._gInstanceId = Guid.NewGuid();
                                                loReportData._dAt = DateTime.Parse(Booking.Library.Classes.Utility.DecodeSerialisedData(lsData, "dAt"));
                                                loReportData._sExcludeBatches = Booking.Library.Classes.Utility.DecodeSerialisedData(lsData, "sExcludeBatches");
                                                DateTime ldRun = DateTime.Now;
                                                List<Booking.Data.Classes.Configuration.GLTransaction> loGLTransactionTemp = Booking.Data.Classes.Helper.GetAllTransactions(this._oHeron28Context, lgCompanyId, loReportData._sExcludeBatches, String.Empty, String.Empty, null, loReportData._dAt, loReportData._bContras, false);
                                                foreach (Booking.Data.Classes.Configuration.GLTransaction loGLTransaction in loGLTransactionTemp)
                                                {
                                                    Booking.Data.DB.Heron28.GeneralLedgerTransactionsReportingDatum loGeneralLedgerTransactionReportingDatum = new GeneralLedgerTransactionsReportingDatum()
                                                    {
                                                        GltGReportInstanceId = loReportData._gInstanceId,
                                                        GltNReport = (short)loReportData._nReport,
                                                        GltDRun = ldRun,
                                                        GltDContraDate = loGLTransaction._dContraDate,
                                                        GltDTransactionDate = loGLTransaction._dTransactionDate,
                                                        GltGAccountCode = loGLTransaction._gAccountCode,
                                                        GltGId = loGLTransaction._gId,
                                                        GltGUserId = Guid.Parse(loUser.Id),
                                                        GltNCr = loGLTransaction._nCr,
                                                        GltNDr = loGLTransaction._nDr,
                                                        GltSAccountDescription = loGLTransaction._sAccountDescription,
                                                        GltSBatchCode = loGLTransaction._sBatchCode,
                                                        GltSCompanyName = loGLTransaction._sCompanyName,
                                                        GltSOtherSide = loGLTransaction._sOtherSide,
                                                        GltSRef = loGLTransaction._sRef,
                                                        GltSTransactionDescription = loGLTransaction._sTransactionDescription,
                                                        GltSType = loGLTransaction._sType,
                                                        GltSAccountCode = loGLTransaction._sAccountCode,
                                                        GltOBranding = (byte[])loImage
                                                    };
                                                    this._oHeron28Context.Add(loGeneralLedgerTransactionReportingDatum);
                                                }
                                                this._oHeron28Context.SaveChanges();
                                                loIDbContextTransaction.Commit();
                                                AllDataSetDataSet loAllDataSetDataSet = new AllDataSetDataSet();
                                                loAllDataSetDataSet.ReportRun.AddReportRunRow(lnReport, lsData, Guid.Parse(loUser.Id));
                                                Booking.Data.DB.Heron28.Audit.CreateAuditEvent(Enums.AuditOperation.RunReport, null, loReportData._gInstanceId, loUser.Id, null, loAllDataSetDataSet, lgCompanyId);
                                            }
                                            else
                                            {
                                                Booking.Data.DB.Heron28.Audit loAudit = this._oHeron28Context.Audits.Where(A => A.AuditCompanyEGId == lgCompanyId && A.AuditNOperation == (short)Booking.Library.Classes.Enums.AuditOperation.RunReport && A.AuditGRecordId == loReportData._gInstanceId).AsEnumerable().Where(A => A.GetDataSetDataset.ReportRun.FirstOrDefault().RR_nReport == lnReport).FirstOrDefault();
                                                if (loAudit != null)
                                                {
                                                    lsData = loAudit.GetDataSetDataset.ReportRun.First().RR_sData;
                                                    loReportData._dAt = DateTime.Parse(Booking.Library.Classes.Utility.DecodeSerialisedData(lsData, "dAt"));
                                                    loReportData._sExcludeBatches = Booking.Library.Classes.Utility.DecodeSerialisedData(lsData, "sExcludeBatches");
                                                }
                                            }
                                        }
                                        break;
                                    case Booking.Library.Classes.Enums.Reports.TransactionReport1:
                                        {
                                            loReportData._gInstanceId = Guid.Parse(Booking.Library.Classes.Utility.DecodeSerialisedData(lsData, "gInstanceId"));
                                            loReportData._sReport = Booking.Library.Classes.Utility.DecodeSerialisedData(lsData, "sReport");
                                            if (loReportData._gInstanceId == Guid.Empty)
                                            {
                                                loReportData._gInstanceId = Guid.NewGuid();
                                                loReportData._bAll =
                                                    bool.Parse(Booking.Library.Classes.Utility.DecodeSerialisedData(lsData, "bAll"));
                                                loReportData._bContras =
                                                    bool.Parse(Booking.Library.Classes.Utility.DecodeSerialisedData(lsData, "bContras"));
                                                loReportData._sExcludeBatches = Booking.Library.Classes.Utility.DecodeSerialisedData(lsData, "sExcludeBatches");
                                                loReportData._sAccounts = String.Empty;
                                                if (!loReportData._bAll)
                                                {
                                                    loReportData._sAccounts = Booking.Library.Classes.Utility.DecodeSerialisedData(lsData, "sAccounts");
                                                }
                                                loReportData._sReport = Booking.Library.Classes.Utility.DecodeSerialisedData(lsData, "sReport");
                                                loReportData._dStart = DateTime.Parse(Booking.Library.Classes.Utility.DecodeSerialisedData(lsData, "dFrom"));
                                                loReportData._nStart = loReportData._dStart.ToOADate();
                                                loReportData._dEnd = DateTime.Parse(Booking.Library.Classes.Utility.DecodeSerialisedData(lsData, "dTo"));
                                                loReportData._nEnd = loReportData._dEnd.Date.AddDays(1).AddSeconds(-1).ToOADate();
                                                DateTime ldRun = DateTime.Now;
                                                List<Booking.Data.Classes.Configuration.GLTransaction> loGLTransactionTemp = Booking.Data.Classes.Helper.GetAllTransactions(this._oHeron28Context, lgCompanyId, loReportData._sExcludeBatches, string.Empty, string.Empty, loReportData._dStart, loReportData._dEnd, loReportData._bContras, false);
                                                foreach (Booking.Data.Classes.Configuration.GLTransaction loGLTransaction in loGLTransactionTemp)
                                                {
                                                    Booking.Data.DB.Heron28.GeneralLedgerTransactionsReportingDatum loGeneralLedgerTransactionReportingDatum = new GeneralLedgerTransactionsReportingDatum()
                                                    {
                                                        GltGReportInstanceId = loReportData._gInstanceId,
                                                        GltNReport = (short)loReportData._nReport,
                                                        GltDRun = ldRun,
                                                        GltDContraDate = loGLTransaction._dContraDate,
                                                        GltDTransactionDate = loGLTransaction._dTransactionDate,
                                                        GltGAccountCode = loGLTransaction._gAccountCode,
                                                        GltGId = loGLTransaction._gId,
                                                        GltGUserId = Guid.Parse(loUser.Id),
                                                        GltNCr = loGLTransaction._nCr,
                                                        GltNDr = loGLTransaction._nDr,
                                                        GltSAccountDescription = loGLTransaction._sAccountDescription,
                                                        GltSBatchCode = loGLTransaction._sBatchCode,
                                                        GltSCompanyName = loGLTransaction._sCompanyName,
                                                        GltSOtherSide = loGLTransaction._sOtherSide,
                                                        GltSRef = loGLTransaction._sRef,
                                                        GltSTransactionDescription = loGLTransaction._sTransactionDescription,
                                                        GltSType = loGLTransaction._sType,
                                                        GltSAccountCode = loGLTransaction._sAccountCode,
                                                        GltOBranding = (byte[])loImage
                                                    };
                                                    this._oHeron28Context.Add(loGeneralLedgerTransactionReportingDatum);
                                                }
                                                this._oHeron28Context.SaveChanges();
                                                loIDbContextTransaction.Commit();
                                                AllDataSetDataSet loAllDataSetDataSet = new AllDataSetDataSet();
                                                loAllDataSetDataSet.ReportRun.AddReportRunRow(lnReport, lsData, Guid.Parse(loUser.Id));
                                                Booking.Data.DB.Heron28.Audit.CreateAuditEvent(Enums.AuditOperation.RunReport, null, loReportData._gInstanceId, loUser.Id, null, loAllDataSetDataSet, lgCompanyId);
                                            }
                                            else
                                            {
                                                Booking.Data.DB.Heron28.Audit loAudit = this._oHeron28Context.Audits.Where(A => A.AuditCompanyEGId == lgCompanyId && A.AuditNOperation == (short)Booking.Library.Classes.Enums.AuditOperation.RunReport && A.AuditGRecordId == loReportData._gInstanceId).AsEnumerable().Where(A => A.GetDataSetDataset.ReportRun.FirstOrDefault().RR_nReport == lnReport).FirstOrDefault();
                                                if (loAudit != null)
                                                {
                                                    lsData = loAudit.GetDataSetDataset.ReportRun.First().RR_sData;
                                                    loReportData._bAll =
                                                        bool.Parse(Booking.Library.Classes.Utility.DecodeSerialisedData(lsData, "bAll"));
                                                    loReportData._bContras =
                                                        bool.Parse(Booking.Library.Classes.Utility.DecodeSerialisedData(lsData, "bContras"));
                                                    loReportData._sExcludeBatches = Booking.Library.Classes.Utility.DecodeSerialisedData(lsData, "sExcludeBatches");
                                                    loReportData._sAccounts = String.Empty;
                                                    if (!loReportData._bAll)
                                                    {
                                                        loReportData._sAccounts = Booking.Library.Classes.Utility.DecodeSerialisedData(lsData, "sAccounts");
                                                    }
                                                    loReportData._dStart = DateTime.Parse(Booking.Library.Classes.Utility.DecodeSerialisedData(lsData, "dFrom"));
                                                    loReportData._nStart = loReportData._dStart.ToOADate();
                                                    loReportData._dEnd = DateTime.Parse(Booking.Library.Classes.Utility.DecodeSerialisedData(lsData, "dTo"));
                                                    loReportData._nEnd = loReportData._dEnd.Date.AddDays(1).AddSeconds(-1).ToOADate();
                                                }
                                            }
                                        }
                                        break;
                                    case Booking.Library.Classes.Enums.Reports.AgedDebtorsReport1:
                                    case Booking.Library.Classes.Enums.Reports.AgedDebtorsReport2:
                                        {
                                            loReportData._gInstanceId = Guid.Parse(Booking.Library.Classes.Utility.DecodeSerialisedData(lsData, "gInstanceId"));
                                            loReportData._sReport = Booking.Library.Classes.Utility.DecodeSerialisedData(lsData, "sReport");
                                            if (loReportData._gInstanceId == Guid.Empty)
                                            {
                                                loReportData._gInstanceId = Guid.NewGuid();
                                                DateTime ldRun = DateTime.Now;
                                                loReportData._bAll =
                                                    bool.Parse(Booking.Library.Classes.Utility.DecodeSerialisedData(lsData, "bAll"));
                                                loReportData._sExcludeBatches = Booking.Library.Classes.Utility.DecodeSerialisedData(lsData, "sExcludeBatches");
                                                loReportData._bShowZeroBalances = bool.Parse(Booking.Library.Classes.Utility.DecodeSerialisedData(lsData, "bShowZeroBalances"));
                                                bool lbShowAllocated = bool.Parse(Booking.Library.Classes.Utility.DecodeSerialisedData(lsData, "bShowAllocated"));
                                                loReportData._sCustomers = string.Empty;
                                                if (!loReportData._bAll)
                                                {
                                                    loReportData._sCustomers = Booking.Library.Classes.Utility.DecodeSerialisedData(lsData, "sCustomers");
                                                }
                                                loReportData._dAt = DateTime.Parse(Booking.Library.Classes.Utility.DecodeSerialisedData(lsData, "dAt"));
                                                Guid lgDebtorsId = Guid.Empty;
                                                if (loEntity.GetDataSetDataset.SpecialAccounts.Where(SA => SA.SA_nType == (short)Booking.Library.Classes.Enums.SpecialAccountTypes.Debtors).FirstOrDefault() != null)
                                                {
                                                    lgDebtorsId = loEntity.GetDataSetDataset.SpecialAccounts.Where(SA => SA.SA_nType == (short)Booking.Library.Classes.Enums.SpecialAccountTypes.Debtors).FirstOrDefault().SA_gId;
                                                }
                                                List<Booking.Data.Classes.Configuration.GLTransaction> loGLTransactionTemp = Booking.Data.Classes.Helper.GetAllTransactions(this._oHeron28Context, lgCompanyId, loReportData._sExcludeBatches, lgDebtorsId.ToString(), loReportData._sCustomers, null, loReportData._dAt, !lbShowAllocated, false, false);
                                                foreach (Booking.Data.Classes.Configuration.GLTransaction loGLTransaction in loGLTransactionTemp)
                                                {
                                                    Booking.Data.DB.Heron28.SlagedDebtorsReportingDatum loSlagedDebtorsReportingDatum = new SlagedDebtorsReportingDatum()
                                                    {
                                                        SladDAtDate = loReportData._dAt,
                                                        SladNReport = (short)loReportData._nReport,
                                                        SladDRun = ldRun,
                                                        SladGReportInstanceId = loReportData._gInstanceId,
                                                        SladDTransactionDate = loGLTransaction._dTransactionDate,
                                                        SladGAccountCode = loGLTransaction._gAccountCode,
                                                        SladGCustomerId = (loGLTransaction._gNameId != null) ? (Guid)loGLTransaction._gNameId : Guid.Empty,
                                                        SladGUserId = Guid.Parse(loUser.Id),
                                                        SladNCr = loGLTransaction._nDr,
                                                        SladNDr = loGLTransaction._nCr,
                                                        SladSAccountCode = loGLTransaction._sAccountCode,
                                                        SladSAccountDescription = loGLTransaction._sAccountDescription,
                                                        SladSBatchCode = loGLTransaction._sBatchCode,
                                                        SladSCode = loGLTransaction._sNameCode,
                                                        SladSCompany = loGLTransaction._sCompanyName,
                                                        SladSDescription = loGLTransaction._sNameDescription,
                                                        SladSRef = loGLTransaction._sRef,
                                                        SladSTransactionDescription = loGLTransaction._sTransactionDescription,
                                                        SladSType = loGLTransaction._sType,
                                                        SladTrnrGId = loGLTransaction._gId,
                                                        SladOBranding = (byte[])loImage,
                                                        SladNAllocated = (loGLTransaction._nAllocated == null) ? 0 : (decimal)loGLTransaction._nAllocated
                                                    };
                                                    this._oHeron28Context.Add(loSlagedDebtorsReportingDatum);
                                                }
                                                this._oHeron28Context.SaveChanges();
                                                loIDbContextTransaction.Commit();
                                                AllDataSetDataSet loAllDataSetDataSet = new AllDataSetDataSet();
                                                loAllDataSetDataSet.ReportRun.AddReportRunRow(lnReport, lsData, Guid.Parse(loUser.Id));
                                                Booking.Data.DB.Heron28.Audit.CreateAuditEvent(Enums.AuditOperation.RunReport, null, loReportData._gInstanceId, loUser.Id, null, loAllDataSetDataSet, lgCompanyId);
                                            }
                                            else
                                            {
                                                Booking.Data.DB.Heron28.Audit loAudit = this._oHeron28Context.Audits.Where(A => A.AuditCompanyEGId == lgCompanyId && A.AuditNOperation == (short)Booking.Library.Classes.Enums.AuditOperation.RunReport && A.AuditGRecordId == loReportData._gInstanceId).AsEnumerable().Where(A => A.GetDataSetDataset.ReportRun.FirstOrDefault().RR_nReport == lnReport).FirstOrDefault();
                                                if (loAudit != null)
                                                {
                                                    lsData = loAudit.GetDataSetDataset.ReportRun.First().RR_sData;
                                                    loReportData._bAll =
                                                        bool.Parse(Booking.Library.Classes.Utility.DecodeSerialisedData(lsData, "bAll"));
                                                    loReportData._sExcludeBatches = Booking.Library.Classes.Utility.DecodeSerialisedData(lsData, "sExcludeBatches");
                                                    loReportData._bShowZeroBalances = bool.Parse(Booking.Library.Classes.Utility.DecodeSerialisedData(lsData, "bShowZeroBalances"));
                                                    loReportData._sCustomers = String.Empty;
                                                    if (!loReportData._bAll)
                                                    {
                                                        loReportData._sCustomers = Booking.Library.Classes.Utility.DecodeSerialisedData(lsData, "sCustomers");
                                                    }
                                                    loReportData._dAt = DateTime.Parse(Booking.Library.Classes.Utility.DecodeSerialisedData(lsData, "dAt"));
                                                }
                                            }
                                        }
                                        break;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception loException)
            {
                Booking.Library.Classes.Utility.MakeExceptionMessage(true, loException, "<br />", "_BatchReportViewer");
                loIDbContextTransaction.Rollback();
            }
            finally
            {
                loIDbContextTransaction.Dispose();
            }
            return this.PartialView(loReportData);
        }

        // Removed for brevity

    } // SharedController
}