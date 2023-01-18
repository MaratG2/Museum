using System;
using System.Collections;
using System.Collections.Generic;
using Admin.Utility;
using UnityEngine;

namespace Admin.PHP
{
    /// <summary>
    /// Отвечает за ООП запросы к базе данных, относящиеся к залам музея.
    /// </summary>
    /// <remarks>
    /// Имеет 3 публичных контракта-корутины – GetHallByHnum (получить зал по его ID), GetAllHalls (получить все залы), GetAllContentsByHnum (получить все наполнение зала по его ID).
    /// При успешном выполнении этих методов, вызываются обратные вызовы OnHallGet, OnAllHallsGet, OnAllHallContentsGet, которые передают объекты или списки объектов.
    /// Для использования класса нужно сконструировать его C# объект, обращаться к методам через объект.
    /// </remarks>
    public class HallQueries
    {
        public Action<Hall> OnHallGet;
        public Action<List<Hall>> OnAllHallsGet;
        public Action<List<HallContent>> OnAllHallContentsGet;
        private QueriesToPHP _queriesToPhp = new (isDebugOn: true);
        private Action<string> _responseCallback;
        private string _responseText;

        public HallQueries()
        {
            _responseCallback += response => _responseText = response;
        }
        ~HallQueries()
        {
            _responseCallback -= response => _responseText = response;
        }
        public IEnumerator GetHallByHnum(int hnum)
        {
            yield return QueryGetHallByHnum(hnum);
            if (string.IsNullOrWhiteSpace(_responseText))
            {
                OnHallGet?.Invoke(new Hall());
                yield break;
            }
            OnHallGet?.Invoke(ParseRawHall(_responseText));
        }
        private IEnumerator QueryGetHallByHnum(int hnum)
        {
            _responseText = "";
            string phpFileName = "get_hall_by_hnum.php";
            WWWForm data = new WWWForm();
            data.AddField("hnum", hnum);
            yield return _queriesToPhp.PostRequest(phpFileName, data, _responseCallback);
        }
        
        public IEnumerator GetAllHalls()
        {
            yield return QueryGetAllHalls();
            if (string.IsNullOrWhiteSpace(_responseText))
            {
                OnAllHallsGet?.Invoke(new List<Hall>());
                yield break;
            }
            var rawHalls = _responseText.Split(";");
            List<Hall> newHalls = new List<Hall>();
            foreach (var rawHall in rawHalls)
            {
                if (string.IsNullOrWhiteSpace(rawHall))
                    continue;
                
                newHalls.Add(ParseRawHall(rawHall));
            }
            OnAllHallsGet?.Invoke(newHalls);
        }
        
        private Hall ParseRawHall(string rawHall)
        {
            if (string.IsNullOrEmpty(rawHall) || _responseText.Split(" ")[0] == "<br")
                return new Hall();

            Hall newHall = new Hall();
            var hallData = rawHall.Split("|");
            newHall.hnum = Int32.Parse(hallData[0]);
            newHall.name = hallData[1];
            newHall.sizex = Int32.Parse(hallData[2]);
            newHall.sizez = Int32.Parse(hallData[3]);
            newHall.is_date_b = Int32.Parse(hallData[4]) == 1;
            newHall.is_date_e = Int32.Parse(hallData[5]) == 1;
            newHall.date_begin = hallData[6];
            newHall.date_end = hallData[7];
            newHall.is_maintained = Int32.Parse(hallData[8]) == 1;
            newHall.is_hidden = Int32.Parse(hallData[9]) == 1;
            newHall.time_added = hallData[10];
            newHall.author = hallData[11];
            newHall.wall = Int32.Parse(hallData[12]);
            newHall.floor = Int32.Parse(hallData[13]);
            newHall.roof = Int32.Parse(hallData[14]);
            return newHall;
        }
        
        private IEnumerator QueryGetAllHalls()
        {
            _responseText = "";
            string phpFileName = "get_all_halls.php";
            yield return _queriesToPhp.GetRequest(phpFileName, _responseCallback);
        }
        
        public IEnumerator GetAllContentsByHnum(int hnum)
        {
            yield return QueryGetAllContentsByHnum(hnum);
            if (string.IsNullOrWhiteSpace(_responseText))
            {
                OnAllHallContentsGet?.Invoke(new List<HallContent>());
                yield break;
            }

            var rawHallContents = _responseText.Split(';');
            List<HallContent> newHallContents = new List<HallContent>();
            foreach (var rawHallContent in rawHallContents)
            {
                if (string.IsNullOrWhiteSpace(rawHallContent))
                    continue;
                
                newHallContents.Add(ParseRawHallContent(rawHallContent, hnum));
            }
            OnAllHallContentsGet?.Invoke(newHallContents);
        }
        
        private HallContent ParseRawHallContent(string rawHallContent, int hnum)
        {
            if (string.IsNullOrEmpty(rawHallContent) || _responseText.Split(" ")[0] == "<br")
                return new HallContent();
            
            var rawContent = rawHallContent.Split('|');
            HallContent newHallContent = new HallContent();
            newHallContent.hnum = hnum;
            newHallContent.cnum = Int32.Parse(rawContent[0]);
            newHallContent.title = rawContent[1];
            newHallContent.image_url = rawContent[2];
            newHallContent.image_desc = rawContent[3];
            newHallContent.combined_pos = rawContent[4];
            newHallContent.type = Int32.Parse(rawContent[5]);
            newHallContent.date_added = rawContent[6];
            newHallContent.operation = rawContent[7];
            newHallContent.pos_x = Int32.Parse(newHallContent.combined_pos.Split('_')[0]);
            newHallContent.pos_z = Int32.Parse(newHallContent.combined_pos.Split('_')[1]);
            return newHallContent;
        }
        private IEnumerator QueryGetAllContentsByHnum(int hnum)
        {
            string phpFileName = "get_contents_by_hnum.php";
            WWWForm data = new WWWForm();
            data.AddField("hnum", hnum);
            yield return _queriesToPhp.PostRequest(phpFileName, data, _responseCallback);
        }
    }
}