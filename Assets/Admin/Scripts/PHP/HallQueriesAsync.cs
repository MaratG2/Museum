using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Admin.Utility;
using UnityEngine;

namespace Admin.PHP
{
    public class HallQueriesAsync
    {
        private ClientPhpAsync _phpClient = new(isDebugOn: true);

        
        public async Task<Hall> GetHallByHnumAsync(int hnum)
        {
            var getHall = await QueryGetHallByHnumAsync(hnum).ConfigureAwait(false);
            if (string.IsNullOrWhiteSpace(getHall))
            {
                Debug.Log($"Get empty data for request 'getHallByHnum' Hnum = {hnum}");
                return new Hall();
            }

            return ParseRawHall(getHall);
        }
        private async Task<string> QueryGetHallByHnumAsync(int hnum)
        {
            var phpFileName = "get_hall_by_hnum.php";
            var data = new WWWForm();
            data.AddField("hnum", hnum);
            return await _phpClient.PostRequestAsync(phpFileName, data);
        }
        
        public async Task<List<Hall>> GetAllHalls()
        {
            var getAllHalls =  await QueryGetAllHalls().ConfigureAwait(false);
            if (string.IsNullOrEmpty(getAllHalls) || getAllHalls.Split(" ")[0] == "<br")
            {
                Debug.Log("Get empty data for request 'getAllHalls'");
                return new List<Hall>();
            }
            
            var rawHalls = getAllHalls.Split(";");
            var newHalls = new List<Hall>();
            foreach (var rawHall in rawHalls)
            {
                if (string.IsNullOrWhiteSpace(rawHall))
                    continue;
                newHalls.Add(ParseRawHall(rawHall));
            }

            return newHalls;
        }
        
        private Hall ParseRawHall(string rawHall)
        {
            if (string.IsNullOrEmpty(rawHall))
                return new Hall();

            var newHall = new Hall();
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

        private async Task<string> QueryGetAllHalls()
        {
            var phpFileName = "get_all_halls.php";
            return await _phpClient.GetRequestAsync(phpFileName).ConfigureAwait(false);
        }

        public async Task<List<HallContent>> GetAllContentsByHnumAsync(int hnum, WWWForm form)
        {
            var allContentsByHnum = await QueryGetAllContentsByHnumAsync(hnum,form).ConfigureAwait(false);

            if (string.IsNullOrEmpty(allContentsByHnum) || allContentsByHnum.Split(" ")[0] == "<br")
            {
                Debug.Log($"Get empty data for request 'getAllContentsByHnum' Hnum = {hnum}");
                return new List<HallContent>();
            }

            var rawHallContents = allContentsByHnum.Split(';');
            
            var newHallContents = new List<HallContent>();
            
            foreach (var rawHallContent in rawHallContents)
            {
                if (string.IsNullOrWhiteSpace(rawHallContent))
                    continue;
                
                newHallContents.Add(ParseRawHallContent(rawHallContent, hnum));
            }

            return newHallContents;
        }
        private HallContent ParseRawHallContent(string rawHallContent, int hnum)
        {
            if (string.IsNullOrEmpty(rawHallContent))
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

        private async Task<string> QueryGetAllContentsByHnumAsync(int hnum,WWWForm form)
        {
            var phpFileName = "get_contents_by_hnum.php";
            Debug.Log("start");
            Debug.Log("finish");
            form.AddField("hnum", hnum);
            return await _phpClient.PostRequestAsync(phpFileName, form).ConfigureAwait(false);
        }
    }
}