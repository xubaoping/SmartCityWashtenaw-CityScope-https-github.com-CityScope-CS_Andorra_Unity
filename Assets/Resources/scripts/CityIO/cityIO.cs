﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System.Linq;


/// <summary> class start </summary>

[System.Serializable]  // have to have this in every JSON class!
public class Grid
{
    public int type;
    public int x;
    public int y;
    public int rot;
}

[System.Serializable] // have to have this in every JSON class!
public class Objects
{
    public float slider1;
    public int toggle1;
    public int toggle2;
    public int toggle3;
    public int toggle4;
    public int dockID;
    public int dockRotation;
    public int IDMax;
    public List<int> density;
    public int pop_young;
    public int pop_mid;
    public int pop_old;
}

[System.Serializable]// have to have this in every JSON class!
public class Table
{
    public List<Grid> grid;
    public Objects objects;
    public string id;
    public long timestamp;

    public static Table CreateFromJSON(string jsonString)
    { // static function that returns Table which holds Class objects 
        return JsonUtility.FromJson<Table>(jsonString);
    }
}

/// <summary> class end </summary>


public class cityIO : MonoBehaviour
{

    //	private string localJson = "file:///Users/noyman/GIT/KendallAgents/Assets/Resources/scripts/citymatrix_volpe.json"; //local file
    private string urlStart = "https://cityio.media.mit.edu/api/table/citymatrix"; // Table data: https://cityio.media.mit.edu/table/cityio_meta
    public string tableNameAddUnderscoreBefore = "";

    private string url;

    public int delayWWW;
    private WWW www;
    private string cityIOtext;
    private string cityIOtext_Old;
    //this one look for changes
    public bool _flag = false;

    public int tableX;
    public int tableY;
    public float cellWorldSize;
    public float cellShrink;
    public float floorHeight;

    private GameObject cityIOGeo;
    public Material _material;

    public Table _table;
    public GameObject gridParent;
    public GameObject textMeshPrefab;
    public static List<GameObject> gridObjects = new List<GameObject>();
    //new list!

    public Color[] colors;


    IEnumerator Start()
    {

        while (true)
        {

            url = urlStart + tableNameAddUnderscoreBefore;
            //WWW www = new WWW (url);
            WWW www = new WWW(url);

            yield return www;
            yield return new WaitForSeconds(delayWWW);
            cityIOtext = www.text; //just a cleaner Var
            if (cityIOtext != cityIOtext_Old)
            {
                cityIOtext_Old = cityIOtext; //new data has arrived from server 
                jsonHandler();


            }
        }
    }


    void jsonHandler()
    {
        _table = Table.CreateFromJSON(cityIOtext); // get parsed JSON into Cells variable --- MUST BE BEFORE CALLING ANYTHING FROM CELLS!!
        drawTable();


        // prints last update time to console 
        System.DateTime epochStart = new System.DateTime(1970, 1, 1, 0, 0, 0, System.DateTimeKind.Utc);
        var lastUpdateTime = epochStart.AddSeconds(System.Math.Round(_table.timestamp / 1000d)).ToLocalTime();
        print("Table was updated." + '\n' + "Following JSON from CityIO server was created at: " + lastUpdateTime + '\n' + cityIOtext);

        _flag = true;
    }


    void drawTable()
    {

        foreach (Transform child in gridParent.transform)
        {
            GameObject.Destroy(child.gameObject); // strat cycle with clean grid
            gridObjects.Clear(); // clean list!!!
        }

        for (int i = 0; i < _table.grid.Count; i++) // loop through list of all cells grid objects 
        {
            // make the geomerty
            cityIOGeo = GameObject.CreatePrimitive(PrimitiveType.Cube); //make cell cube  
            cityIOGeo.GetComponent<Renderer>().material = _material;
            cityIOGeo.transform.parent = gridParent.transform; //put into parent object for later control 
            cityIOGeo.transform.position = new Vector3((_table.grid[i].x * cellWorldSize), 0, (_table.grid[i].y * cellWorldSize)); //compensate for scale shift due to height
            cityIOGeo.transform.localScale = new Vector3(cellWorldSize, 0, cellWorldSize);
            cityIOGeo.name =
           ("Type > " + _table.grid[i].type + " X > " + _table.grid[i].x.ToString() + " Y > " + _table.grid[i].y.ToString());
            cityIOGeo.AddComponent<NavMeshObstacle>();
            cityIOGeo.GetComponent<NavMeshObstacle>().carving = true;

            //ShowBuildingTypeText (i); /// call if you need type text float 

            for (int n = 0; n < _table.objects.density.Count; n++)
            { //go through all 'densities' to match Type to Height. Add +1 so #6 (Road could be in. Fix in JSON Needed) 
              //print(n + " " +_Cells.objects.density[n]);


                if (new int[] { 0, 1, 2, 3, 4, 5 }.Contains(_table.grid[i].type))
                { //if this cell is one of the buildings types
                    cityIOGeo.transform.localScale = new Vector3(cellShrink * cellWorldSize, (_table.objects.density[n] * floorHeight), cellShrink * cellWorldSize);
                    cityIOGeo.transform.localPosition = new Vector3(cityIOGeo.transform.position.x, (_table.objects.density[n] * floorHeight) / 2, cityIOGeo.transform.position.z); //compensate for scale shift and x,y array

                    var _tmpColor = colors[_table.grid[i].type];
                    _tmpColor.a = 0.5f;
                    cityIOGeo.GetComponent<Renderer>().material.color = _tmpColor;
                }
                else if (new int[] { -1, -2, 6, 7, 8 }.Contains(_table.grid[i].type))
                {
                    if (_table.grid[i].type == 6)
                    { //Street
                        //print("Type > " + _table.grid[i].type + " X > " + _table.grid[i].x.ToString() + " Y > " + _table.grid[i].y.ToString());
                        cityIOGeo.transform.localScale = new Vector3(cellShrink * cellWorldSize, 0, cellShrink * cellWorldSize);
                        cityIOGeo.transform.localPosition = new Vector3(cityIOGeo.transform.position.x, 0, cityIOGeo.transform.position.z); //compensate for scale shift and x,y array
                        var _tmpColor = Color.black;
                        _tmpColor.a = 1f;
                        cityIOGeo.GetComponent<Renderer>().material.color = _tmpColor;
                    }
                    else if (_table.grid[i].type == 9) // if parking
                    {
                        cityIOGeo.transform.localScale = new Vector3(cellWorldSize, 0, cellWorldSize);
                        cityIOGeo.transform.localPosition = new Vector3(cityIOGeo.transform.position.x, 0, cityIOGeo.transform.position.z); //compensate for scale shift and x,y array
                        var _tmpColor = Color.gray;
                        _tmpColor.a = 1f;
                        cityIOGeo.GetComponent<Renderer>().material.color = _tmpColor;
                    }
                    else
                    { //if green or other non building type
                        cityIOGeo.transform.localPosition = new Vector3(cityIOGeo.transform.position.x, 0, cityIOGeo.transform.position.z); //hide base plates 
                        cityIOGeo.transform.localScale = new Vector3(cellShrink * cellWorldSize * 0.5f, 0.25f, cellShrink * cellWorldSize * 0.5f);
                        cityIOGeo.GetComponent<Renderer>().material.color = Color.black;
                    }
                }
            }
        }
        gridObjects.Add(cityIOGeo); //add this Gobj to list
    }

    private void ShowBuildingTypeText(int i) //mesh type text metod 
    {
        GameObject textMesh = GameObject.Instantiate(textMeshPrefab, new Vector3((_table.grid[i].x * cellWorldSize),
                                  100, (_table.grid[i].y * cellWorldSize)),
                                  cityIOGeo.transform.rotation, transform) as GameObject; //spwan prefab text

        textMesh.GetComponent<TextMesh>().text = _table.grid[i].type.ToString();
        textMesh.GetComponent<TextMesh>().fontSize = 300;
        textMesh.GetComponent<TextMesh>().color = Color.black;
        textMesh.GetComponent<TextMesh>().characterSize = 0.5f;
    }
}
