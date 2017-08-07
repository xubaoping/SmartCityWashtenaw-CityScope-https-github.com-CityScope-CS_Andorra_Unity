﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class stateManager : MonoBehaviour
{
    public GameObject _contextHolder;
    public GameObject _cellTowers;
    public GameObject _heatmapHolder;
    public cityIO _cityIOscript;
    public Visualizations _heatmapsScript;
    public GameObject _andorraCityScope;
    public GameObject _andorraHeatmap;
    public GameObject _floorsUI;
    private int _sliderState = 3;
    private int _oldState;
    public bool _demoModeBool = true;
    public int _changeModeEverySeconds = 60;

	private const int NUM_STATES = 6;
	private enum HeatmapState { CITYIO = 0, LANDUSE = 1, FLOORS = 2, RES_PROXIMITY = 3, OFFICE_PROXIMITY = 4, PARK_PROXIMITY = 5, CELL = 6 };

    void Awake()
    {
        if (_demoModeBool != true)
        {
			_sliderState = (int)_cityIOscript._table.objects.slider1; //gets the slider 
            _oldState = _sliderState;
            StateControl(_sliderState);
        }
        else
        {
            StartCoroutine(DemoMode());
        }
    }
    void Update()
    {
		_sliderState = (int)_cityIOscript._table.objects.slider1; //gets the slider 
        if (_sliderState != _oldState)
        {
            StateControl(_sliderState);
            _oldState = _sliderState;
        }
    }
    IEnumerator DemoMode()
    {
        while (true)
        {
			for (int i = 0; i < NUM_STATES; i++)
            {
                yield return new WaitForEndOfFrame();
				StateControl(i);
                yield return new WaitForSeconds(_changeModeEverySeconds);

            }
        }
    }
    void StateControl(int _sliderState)
    {
		CleanOldViz(_contextHolder, _heatmapHolder);
        switch (_sliderState)
        {
            default:
                ShowContext(_andorraCityScope);
                print("Default: Basic Sat view and cityIO grid" + '\n');
                _floorsUI.SetActive(false);
                break;
			case (int) HeatmapState.CITYIO:
                ShowContext(_andorraCityScope);
                print("State 0: Basic Sat view and cityIO grid" + '\n');
                _floorsUI.SetActive(false);
                break;
			case (int)HeatmapState.LANDUSE: // LANDUSE 
                ShowContext(_andorraHeatmap);
                _heatmapsScript.TypesViz();
                print("State 2: Land use map" + '\n');
                _floorsUI.SetActive(false);
                break;
			case (int)HeatmapState.FLOORS: // FLOORS
                ShowContext(_andorraHeatmap);
                _heatmapsScript.FloorsViz();
                _floorsUI.SetActive(true);
                print("State 1: Floors map" + '\n');
                break;
			case (int)HeatmapState.RES_PROXIMITY: // HEATMAP
                ShowContext(_andorraHeatmap);
				_heatmapsScript.HeatmapViz(Visualizations.HeatmapType.RES);
                print("State 3: Proximity to Res HeatMap" + '\n');
                _floorsUI.SetActive(false);
                break;
			case (int)HeatmapState.OFFICE_PROXIMITY: // HEATMAP
				ShowContext(_andorraHeatmap);
				_heatmapsScript.HeatmapViz(Visualizations.HeatmapType.OFFICE);
				print("State 4: Proximity to Offices HeatMap" + '\n');
				_floorsUI.SetActive(false);
				break;
			case (int)HeatmapState.PARK_PROXIMITY: // HEATMAP
				ShowContext(_andorraHeatmap);
				_heatmapsScript.HeatmapViz(Visualizations.HeatmapType.PARK);
				print("State 5: Proximity to Parks HeatMap" + '\n');
				_floorsUI.SetActive(false);
				break;
			case (int)HeatmapState.CELL: // Cell towers
                ShowContext(_andorraHeatmap);
                ShowContext(_cellTowers);
                print("State 6: Celltowers heatmap" + '\n');
                _floorsUI.SetActive(false);
                break;
        }
    }

    void CleanOldViz(GameObject _contextHolder, GameObject _heatmapHolder)
    {
        foreach (Transform child in _contextHolder.transform)
        {
            child.gameObject.SetActive(false);
        }

        foreach (Transform child in _heatmapHolder.transform)
        {
			child.gameObject.SetActive(false);
        }
    }
    void ShowContext(GameObject t)
    {
        t.transform.gameObject.SetActive(true);
    }
}


