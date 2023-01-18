namespace Admin.Utility
{
    /// <summary>
    /// Структура данных, описывающая наполнение зала Музея, совпадает с структурой соответствующей таблицы в базе данных.
    /// </summary>
    public struct HallContent
    {
        public int hnum;
        public int cnum;
        public string title;
        public string image_url;
        public string image_desc;
        public string combined_pos;
        public int type;
        public string date_added;
        public string operation;
        public int pos_x;
        public int pos_z;
    }
}