using System;
using System.Collections;
using System.Collections.Generic;
using Admin.Utility;
using UnityEngine;

namespace Admin.PHP
{
    public class HallContentQueries
    {
        public Action<List<HallContent>> hallContentGotCallback;
        private QueriesToPHP _queriesToPhp = new (isDebugOn: true);
        private Action<string> _responseCallback;
        private string _responseText;

        public HallContentQueries()
        {
            _responseCallback += response => _responseText = response;
        }

        ~HallContentQueries()
        {
            _responseCallback -= response => _responseText = response;
        }
 
        public IEnumerator GetContentsByHnum(int num)
        {
            yield return QueryGetContentsByHnum(num);
            if (string.IsNullOrWhiteSpace(_responseText))
            {
                yield break;
            }
        
            var rawHallContents = _responseText.Split(';');
            List<HallContent> newHallContents = new List<HallContent>();
            foreach (var rawHallContent in rawHallContents)
            {
                if (string.IsNullOrWhiteSpace(rawHallContent))
                    continue;
            
                var rawContent = rawHallContent.Split('|');
                HallContent newHallContent = new HallContent();
                newHallContent.hnum = num;
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

                newHallContents.Add(newHallContent);
            }

            hallContentGotCallback?.Invoke(newHallContents);
        }
        private IEnumerator QueryGetContentsByHnum(int num)
        {
            string phpFileName = "get_contents_by_hnum.php";
            WWWForm data = new WWWForm();
            data.AddField("hnum", num);
            yield return _queriesToPhp.PostRequest(phpFileName, data, _responseCallback);
        }
    }
}