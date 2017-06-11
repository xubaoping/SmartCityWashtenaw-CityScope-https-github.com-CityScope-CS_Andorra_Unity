﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class stateManager : MonoBehaviour
{
    public GameObject _contextHolder;
    public GameObject _heatmapHolder;
    public cityIO _cityIOscript;
    public HeatMaps _heatmapsScript;
    public GameObject _andorraCityScope;
    public GameObject _andorraHeatmap;
    public GameObject _floorsUI;

    public int _sliderState = 0;
    private int _oldState;

    void Awake()
    {
        _sliderState = _cityIOscript._table.objects.toggle1;
        _oldState = _sliderState;
        StateControl(_sliderState);
    }

    void Update()
    {
        if (_sliderState != _oldState)
        {
            StateControl(_sliderState);
            _oldState = _sliderState;
        }
    }

    void StateControl(int _sliderState)
    {
        switch (_sliderState)
        {
            default:
                CleanOldViz(_contextHolder, _heatmapHolder);
                ShowContext(_andorraCityScope);
                print("Default: Basic Sat view and cityIO grid" + '\n');
                _floorsUI.SetActive(false);

                break;
            case 0: //CITYIO
                CleanOldViz(_contextHolder, _heatmapHolder);
                ShowContext(_andorraCityScope);
                print("State 0: Basic Sat view and cityIO grid" + '\n');
                _floorsUI.SetActive(false);
                break;

            case 1:// FLOORS
                CleanOldViz(_contextHolder, _heatmapHolder);
                ShowContext(_andorraHeatmap);
                StartCoroutine(_heatmapsScript.FloorsViz());
                _floorsUI.SetActive(true);
                print("State 1: Floors map" + '\n');
                break;

            case 2: // LANDUSE 
                CleanOldViz(_contextHolder, _heatmapHolder);
                ShowContext(_andorraHeatmap);
                _heatmapsScript.TypesViz();
                print("State 2: Land use map" + '\n');
                _floorsUI.SetActive(false);
                break;

            case 3: // HEATMAP
                CleanOldViz(_contextHolder, _heatmapHolder);
                ShowContext(_andorraHeatmap);
                _heatmapsScript.SearchNeighbors();
                print("State 3: Proximity HeatMap" + '\n');
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
            Destroy(child.gameObject);
        }
    }

    void ShowContext(GameObject t)
    {
        t.transform.gameObject.SetActive(true);
    }
}


