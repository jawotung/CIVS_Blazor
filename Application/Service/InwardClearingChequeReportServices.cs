using Application.Interfaces;
using Application.Models;
using Application.Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using WebAPI;
using System.Net.Http.Json;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Application.Service
{
    public class InwardClearingChequeReportServices : IInwardClearingChequeReportServices
    {
        private readonly HttpClient httpClient;

        public InwardClearingChequeReportServices(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        public async Task<PaginatedOutputServices<InwardClearingReportModel>> GetReportList(string Key, int page = 1, string? dateFromFilter = null, string? dateToFilter = null, string? BSRTNFilter = null)
        {
            try
            {
                var response = await httpClient.GetAsync("/InwardClearingChequeReport/GetReportList?Key=" + (Key ?? "") + "&page=" + page + "&dateFromFilter=" + (dateFromFilter ?? "") + "&dateToFilter=" + (dateToFilter ?? "") + "&BSRTNFilter=" + (BSRTNFilter ?? ""));
                var result = await response.Content.ReadFromJsonAsync<PaginatedOutputServices<InwardClearingReportModel>>();
                return result;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public async Task<ReturnGenericDropdown> GetBuddyBranches()
        {
            try
            {
                var response = await httpClient.GetAsync("/InwardClearingChequeReport/GetBuddyBranches");
                var result = await response.Content.ReadFromJsonAsync<ReturnGenericDropdown>();
                return result;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public async Task<ReturnGenericData<MCCheckDetailModel>> GetMCDetails(string acctAmt)
        {
            try
            {
                var response = await httpClient.GetAsync("/InwardClearingChequeReport/GetMCDetails?acctAmt=" + acctAmt);
                var result = await response.Content.ReadFromJsonAsync<ReturnGenericData<MCCheckDetailModel>>();
                return result;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public async Task<ReturnGenericData<ReturnDownloadPDF>> PrintReport(string Key, string? dateFromFilter = null, string? dateToFilter = null, string? BSRTNFilter = null)
        {
            var response = await httpClient.GetAsync("/InwardClearingChequeReport/PrintReport?Key=" + (Key ?? "") + "&dateFromFilter=" + (dateFromFilter ?? "") + "&dateToFilter=" + (dateToFilter ?? "") + "&BSRTNFilter=" + (BSRTNFilter ?? ""));
            var result = await response.Content.ReadFromJsonAsync<ReturnGenericData<ReturnDownloadPDF>>();
            return result;
        }
        public async Task<ReturnGenericData<ReturnDownloadPDF>> PrintReportWithImages(string Key, string? dateFromFilter = null, string? dateToFilter = null, string? BSRTNFilter = null)
        {
            var response = await httpClient.GetAsync("/InwardClearingChequeReport/PrintReportWithImages?Key=" + (Key ?? "") + "&dateFromFilter=" + (dateFromFilter ?? "") + "&dateToFilter=" + (dateToFilter ?? "") + "&BSRTNFilter=" + (BSRTNFilter ?? ""));
            var result = await response.Content.ReadFromJsonAsync<ReturnGenericData<ReturnDownloadPDF>>();
            return result;
        }
    }
}
