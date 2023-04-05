using System.Collections.Generic;
using Admin.Edit;
using Admin.GenerationMap;
using Admin.Models;
using Admin.Utility;
using GenerationMap;
using InProject;
using Museum.Scripts.ReadInfo;
using UnityEngine;

namespace Museum.Scripts.GenerationMap
{
    /// <summary>
    /// Отвечает за генерацию экспонатов музея.
    /// </summary>
    public class ExhibitsSpawner : MonoBehaviour
    {
        [SerializeField] public GameObject picture;
        [SerializeField] public GameObject pedestal;
        [SerializeField] public GameObject videoFrame;
        [SerializeField] public GameObject infoBoxPrefab;
        private DecorationsRepository _decorationsRepository;

        public void Start()
        {
            _decorationsRepository = FindObjectOfType<DecorationsRepository>();
        }

        public List<GameObject> SpawnExhibits(Room room)
        {
            var exhibits = new List<GameObject>();
            for (var i = 0; i < room.Exhibits.GetLength(0); i++)
            {
                for (var j = 0; j < room.Exhibits.GetLength(1); j++)
                {
                    
                    var currentExhibit = room.Exhibits[i, j];
                    if (currentExhibit.Id == ExhibitsConstants.Picture.Id)
                        exhibits.Add(SpawnPicture(i, j, currentExhibit, room));
                    
                    if (currentExhibit.Id == ExhibitsConstants.Cup.Id)
                        exhibits.Add(SpawnExhibit(i, j, currentExhibit.HeightSpawn, currentExhibit,
                            room.FloorBlocs, room, pedestal));
                    
                    if (currentExhibit.Id == ExhibitsConstants.Decoration.Id)
                        exhibits.Add(SpawnDecoration(i, j, currentExhibit.HeightSpawn, currentExhibit, room));
                    
                    if (currentExhibit.Id == ExhibitsConstants.Video.Id)
                        exhibits.Add(SpawnVideoExhibit(i, j, currentExhibit, room));

                    if (currentExhibit.Id == ExhibitsConstants.InfoBox.Id)
                        exhibits.Add(SpawnInfoBox(i, j, currentExhibit.HeightSpawn, currentExhibit, room));
                }
            }

            return exhibits;
        }
        
        private GameObject SpawnPicture(int i, int j, ExhibitDto exhibitDto,
            Room room)
        {
            var nearWall = FindNearWallV2(i, j, room);
            var position = nearWall.GetComponent<WallBlock>().SpawnPosition.position;
            var rotate = nearWall.transform.rotation;
            var exhibit = SpawnChunk(picture,
                position, rotate);
            exhibit.GetComponent<ReadPicture>().SetNewPicture(exhibitDto.LinkOnImage);
            exhibit.GetComponent<ReadPicture>().Title = exhibitDto.Description;

            return exhibit;
        }

        private GameObject SpawnExhibit(int i, int j, float height, ExhibitDto exhibitDto,
            GameObject[,] floorBlocks, Room room, GameObject o)
        {
            var nearWall = FindNearWallV2(i, j, room);
            var rotate = nearWall.transform.rotation;
            var exhibit = SpawnChunk(o, floorBlocks[i, j].transform.position + new Vector3(0, height, 0), rotate);
            exhibit.GetComponent<ViewObject>().Title = "Кубок";
            return exhibit;
        }
        
        private GameObject SpawnVideoExhibit(int i, int j, ExhibitDto exhibitDto, Room room)
        {
            var nearWall = FindNearWallV2(i, j, room);
            var position = nearWall.GetComponent<WallBlock>().SpawnPosition.position;
            var rotate = nearWall.transform.rotation;
            var exhibit = SpawnChunk(videoFrame,
                position, rotate);
            exhibit.GetComponent<VideoFrame>().videoUrl = exhibitDto.LinkOnImage;
            exhibit.GetComponent<VideoFrame>().Title = exhibitDto.Description;

            return exhibit;
        }
        
        private GameObject SpawnDecoration(int i, int j, float height, ExhibitDto exhibitDto,Room room)
        {
            var go = _decorationsRepository.GetDecorationGOByName(exhibitDto.Description);
            if (go is null) return null;
            
            var nearWall = FindNearWallV2(i, j, room);
            var rotate = nearWall.transform.rotation;
            var decoration = SpawnChunk(go, room.FloorBlocs[i, j].transform.position + new Vector3(0, height, 0), rotate);
           
            return decoration;
        }
        
        private GameObject SpawnInfoBox(int i, int j, float height, ExhibitDto exhibitDto, Room room)
        {
            var nearWall = FindNearWallV2(i, j, room);
            var rotate = nearWall.transform.rotation;
            var infoBox = SpawnChunk(infoBoxPrefab, room.FloorBlocs[i, j].transform.position + new Vector3(0, height, 0), rotate);
            var readText = infoBox.GetComponent<ReadText>();
            readText.Title = "Информация о текущем зале";
            string descriptionWithNewLines = FormatNewLines(exhibitDto.Description);
            var infos = JsonHelper.FromJson<InfoPart.InfoPartData>(descriptionWithNewLines);
            foreach (var info in infos)
            {
                if (info.url == null) continue;
                readText.AddNewInfo(info.url, info.desc);
            }
            
            return infoBox;
        }

        private string FormatNewLines(string description)
        {
            return description.Replace("\n", "\\n")
                .Replace("\r", "\\r").Replace("\t", "\\t");
        }

        private GameObject FindNearWallV2(int i, int j, Room room)
        {
            var roomWallBlocs = room.WallBlocs;
            if (i == 0)
                return roomWallBlocs[0, j];
            
            if (j == 0)
                return roomWallBlocs[i, 0];
            
            if (i == room.Length - 1)
                return roomWallBlocs[room.Length - 1, j];
            
            if (j == room.Width-1)
                return roomWallBlocs[i, room.Width - 1];
            
            if (i < roomWallBlocs.GetLength(0) / 2)
                return roomWallBlocs[0, j];
            
            return roomWallBlocs[room.Length - 1, j];
        }
        
        private GameObject SpawnChunk(GameObject chunk, Vector3 position, Quaternion rotate)
        {
            return Instantiate(chunk, position, rotate);
        }
    }
}