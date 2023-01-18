namespace Admin.Utility
{
    /// <summary>
    /// Структура данных, описывающая зал Музея, совпадает с структурой соответствующей таблицы в базе данных.
    /// </summary>
    public struct Hall
    {
        public int hnum;
        public string name;
        public int sizex;
        public int sizez;
        public bool is_date_b;
        public bool is_date_e;
        public string date_begin;
        public string date_end;
        public bool is_maintained;
        public bool is_hidden;
        public string time_added;
        public string author;
        public int wall;
        public int floor;
        public int roof;
    }
}