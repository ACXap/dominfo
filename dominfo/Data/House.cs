namespace dominfo
{
    public class House
    {
        public string Address { get; set; }
        public string Url { get; set; }


        public string TypeHouse { get; set; }
        public string Date { get; set; }
        public string Area { get; set; }
        public string Floors { get; set; }
        public string FlatFloor { get; set; }
        public string Status { get; set; }
        public string Flats { get; set; }
        public string Parking { get; set; }

        public string Company { get; set; }


        public string GetHeaderAllInfo()
        {
            return "Адрес;Ссылка;Тип дома;Год постройки;Площадь дома;Этажей;Квартир на этаже;Статус;Кол-во квартир;Парковка;Управляющая компания";
        }

        public string GetAllInfo()
        {
            return $"{Address};{Url};{TypeHouse};{Date};{Area};{Floors};{FlatFloor};{Status};{Flats};{Parking};{Company};";
        }

        public override string ToString()
        {
            return $"{Address};{Url};";
        }
    }
}