using System;

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
}

public struct HallContent
{
    public int onum;
    public int cnum;
    public string title;
    public string image_url;
    public int pos_x;
    public int pos_z;
    public string combined_pos;
    public int type;
    public string image_desc;
}