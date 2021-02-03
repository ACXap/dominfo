using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace dominfo
{
    class Program
    {

        private static List<ManagementCompany> listManagementCompany;
        private static List<House> listHouse;
        private static List<House> listHouseFullInfo;
        private static UserAgentRepository userAgents;

        private static List<string> listSity;

        static void Main(string[] args)
        {
            listSity = GetListSity();

            
            foreach(var urlRegion in listSity)
            {
                string nameRegion = urlRegion.Split('/').Last();
                string fileNameCompany = nameRegion + "_Company.csv";
                string fileNameTempHouse = nameRegion + "_TempHouses.csv";
                string fileHouse = nameRegion + "_Houses.csv";

                listManagementCompany = new List<ManagementCompany>();
                listHouse = new List<House>();
                listHouseFullInfo = new List<House>();


                Console.WriteLine("Получаем все управляющие компании");
                listManagementCompany = new DomInfoCompanyRepository(userAgents.GetRandomUserAgent()).GetManagementCompanies(urlRegion);
                File.AppendAllLines(fileNameCompany, listManagementCompany.Select(m => m.ToString()), Encoding.Default);
                Console.WriteLine("Всего управляющих компаний: " + listManagementCompany.Count);


                Console.WriteLine("Получаем все ссылки на дома");
                Parallel.ForEach(listManagementCompany, c =>
                {
                    listHouse.AddRange(new DomInfoHouseRepository(userAgents.GetRandomUserAgent()).GetHouseManagementCompany(c));
                });
                File.AppendAllLines(fileNameTempHouse, listHouse.Select(h => h.ToString()), Encoding.Default);
                Console.WriteLine("Всего домов: " + listHouse.Count);


                Console.WriteLine("Получаем всю информацию по домам");
                Parallel.ForEach(listHouse, h =>
                {
                    listHouseFullInfo.Add(new DomInfoHouseRepository(userAgents.GetRandomUserAgent()).GetInfoHouse(h));
                });
                File.WriteAllLines(fileHouse, listHouseFullInfo.Select(h => h.GetAllInfo()), Encoding.Default);
            }

            Console.WriteLine("OK");
            Console.ReadLine();


            // string urlRegion = "https://dominfo.ru/uk/region/respublika-tatarstan/kazan";
            //string urlRegion = "https://dominfo.ru/uk/region/respublika-tatarstan/naberejnye-chelny";
            //string urlRegion = "https://dominfo.ru/uk/region/samarskaya-oblast/samara";
            //string urlRegion = "https://dominfo.ru/uk/region/orenburgskaya-oblast/orenburg";

            //string nameRegion = urlRegion.Split('/').Last();
            //string fileNameCompany = nameRegion + "_Company.csv";
            //string fileNameTempHouse = nameRegion + "_TempHouses.csv";
            //string fileHouse = nameRegion + "_Houses.csv";


            //Console.WriteLine("Получаем все управляющие компании");
            //listManagementCompany = new DomInfoCompanyRepository(userAgents.GetRandomUserAgent()).GetManagementCompanies(urlRegion);
            //File.AppendAllLines(fileNameCompany, listManagementCompany.Select(m => m.ToString()), Encoding.Default);
            //Console.WriteLine("Всего управляющих компаний: " + listManagementCompany.Count);


            //Console.WriteLine("Получаем все ссылки на дома");
            //Parallel.ForEach(listManagementCompany, c =>
            //{
            //    listHouse.AddRange(new DomInfoHouseRepository(userAgents.GetRandomUserAgent()).GetHouseManagementCompany(c));
            //});
            //File.AppendAllLines(fileNameTempHouse, listHouse.Select(h => h.ToString()), Encoding.Default);
            //Console.WriteLine("Всего домов: " + listHouse.Count);


            //Console.WriteLine("Получаем всю информацию по домам");
            //Parallel.ForEach(listHouse, h =>
            //{
            //    listHouseFullInfo.Add(new DomInfoHouseRepository(userAgents.GetRandomUserAgent()).GetInfoHouse(h));
            //});
            //File.WriteAllLines(fileHouse, listHouseFullInfo.Select(h => h.GetAllInfo()), Encoding.Default);


            //Console.WriteLine("OK");
            //Console.ReadLine();
        }

        private static List<string> GetListSity()
        {
           return File.ReadAllLines("sity.txt").ToList();
        }
    }
}