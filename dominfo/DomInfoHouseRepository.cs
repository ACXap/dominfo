using AngleSharp;
using AngleSharp.Dom;
using dominfo.Helpers;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace dominfo
{
    public class DomInfoHouseRepository
    {
        public DomInfoHouseRepository(string userAgent)
        {
            _repository = new DomInfoRepository(userAgent);
        }

        #region PrivateField
        const string URL_MORE_HOUSE = "https://dominfo.ru/ajax/managements/moreHouses";
        private readonly DomInfoRepository _repository;
        #endregion PrivateField

        #region PrivateMethod
        private async Task<List<House>> GetHouseAsync(string content)
        {
            List<House> listHouse = new List<House>();

            var config = Configuration.Default;
            var context = BrowsingContext.New(config);
            var document = await context.OpenAsync(req => req.Content(content));

            var divs = document.QuerySelectorAll("div.manageCompany__housesTableTr.manageCompany__housesTableTr_body");

            foreach (var item in divs)
            {
                House h = new House();

                h.Address = item.QuerySelector("a.a.a_blue").TextContent;
                h.Url = item.QuerySelector("a.a.a_blue").GetAttribute("href");
                h.Date = item.QuerySelector("div>div:nth-child(3)").TextContent;

                listHouse.Add(h);
            }

            return listHouse;
        }
        private async Task<House> GetAllInfoHouseAsync(string content, House house)
        {
            House h = house;

            var config = Configuration.Default;
            var context = BrowsingContext.New(config);
            var document = await context.OpenAsync(req => req.Content(content));

            var div = document.QuerySelector("div.house__table");

            h.TypeHouse = div.QuerySelector("div>div:nth-child(1)>div:nth-child(2)").TextContent;
            h.Date = div.QuerySelector("div>div:nth-child(2)>div:nth-child(2)").TextContent;
            h.Area = div.QuerySelector("div>div:nth-child(3)>div:nth-child(2)").TextContent;
            h.Floors = div.QuerySelector("div>div:nth-child(4)>div:nth-child(2)").TextContent;
            h.FlatFloor = div.QuerySelector("div>div:nth-child(5)>div:nth-child(2)").TextContent;
            h.Status = div.QuerySelector("div>div:nth-child(6)>div:nth-child(2)").TextContent;
            h.Flats = div.QuerySelector("div>div:nth-child(7)>div:nth-child(2)").TextContent;
            h.Parking = div.QuerySelector("div>div:nth-child(8)>div:nth-child(2)").TextContent;
            h.Company = div.QuerySelector("div>div:nth-child(9)>div:nth-child(2)").TextContent;

            h.Porch = GetPorch(document);

            return h;
        }

        private string GetPorch(IDocument document)
        {
            IHtmlCollection<IElement> htmlCollections = document.QuerySelectorAll("div.house__charact.house__contentDom>div>div>div>div.house__tableTd");

            for (int i = 0; i < htmlCollections.Length; i++)
            {
                if (htmlCollections[i].TextContent.Contains("Количество подъездов:"))
                {
                    return htmlCollections[i + 1].TextContent;
                }
            }

            return null;
        }
        #endregion PrivateMethod

        #region PublicMethod
        public List<House> GetHouseManagementCompany(ManagementCompany company)
        {
            string url = DomInfoRepository.URL_SITE + company.Url;
            string content = _repository.LoadPage(url);

            if (company.CountHouse > 15)
            {
                Task<HouseListInfo> task = _repository.GetListInfoAsync<HouseListInfo>(content, "div.js__manageCompanyHousesTable");
                task.Wait();
                HouseListInfo houseListInfo = task.Result;

                _repository.GetToken(url);

                DataResponse dr = _repository.LoadData(houseListInfo.GetLoadAll(), url, URL_MORE_HOUSE);

                content = Base.Base64Decode(dr.DataHtml.Content);
            }

            Task<List<House>> taskListHouse = GetHouseAsync(content);
            taskListHouse.Wait();

            return taskListHouse.Result;
        }

        public House GetInfoHouse(House house)
        {
            string content = _repository.LoadPage(DomInfoRepository.URL_SITE + house.Url);

            var task = GetAllInfoHouseAsync(content, house);
            task.Wait();

            return task.Result;
        }
        #endregion PublicMethod
    }
}