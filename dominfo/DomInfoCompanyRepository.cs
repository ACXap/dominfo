using AngleSharp;
using dominfo.Helpers;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace dominfo
{
    public class DomInfoCompanyRepository 
    {
        public DomInfoCompanyRepository(string userAgent)
        {
            _repository = new DomInfoRepository(userAgent);
        }

        #region PrivateField
        const string URL_MORE_COMPANIES = "https://dominfo.ru/ajax/managements/moreCompanies";
        private readonly DomInfoRepository _repository;
        #endregion PrivateField

        #region PrivateMethod
        private async Task<List<ManagementCompany>> GetCompaniesAsync(string content)
        {
            List<ManagementCompany> list = new List<ManagementCompany>();

            var config = Configuration.Default;
            var context = BrowsingContext.New(config);
            var document = await context.OpenAsync(req => req.Content(content));

            var divs = document.QuerySelectorAll("div.manageListComp__tableTr.manageListComp__tableTr_body");

            foreach (var item in divs)
            {
                ManagementCompany mc = new ManagementCompany();

                mc.Name = item.QuerySelector("a.a.a_blue").TextContent;
                mc.Url = item.QuerySelector("a.a.a_blue").GetAttribute("href");
                mc.Inn = item.QuerySelector("div.manageListComp__tableTd.manageListComp__tableTd_inn").TextContent;

                try
                {
                    mc.CountHouse = int.Parse(item.QuerySelector("div.manageListComp__tableTd.manageListComp__tableTd_count").TextContent.Replace(" ", ""));
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message + " " + mc.Url);
                    mc.CountHouse = 999;
                }


                list.Add(mc);
            }

            return list;
        }
        #endregion PrivateMethod

        #region PublicMethod
        public List<ManagementCompany> GetManagementCompanies(string urlRegion)
        {
            string content = _repository.LoadPage(urlRegion);

            Task<CompanyListInfo> task = _repository.GetListInfoAsync<CompanyListInfo>(content, "div.js__manageListCompTable");
            task.Wait();
            CompanyListInfo companyListInfo = task.Result;

            _repository.GetToken(urlRegion);

            DataResponse dr = _repository.LoadData(companyListInfo.GetLoadAll(), urlRegion, URL_MORE_COMPANIES);

            Task<List<ManagementCompany>> taskListCompany = GetCompaniesAsync(Base.Base64Decode(dr.DataHtml.Content));
            taskListCompany.Wait();

            return taskListCompany.Result;
        }
        #endregion PublicMethod
    }
}