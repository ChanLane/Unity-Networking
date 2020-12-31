using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class SetLocalplayer : NetworkBehaviour
{

    [SerializeField] private Text playerNamePrefab;
    [SerializeField] private Text playerLabel;
    [SerializeField] private Transform namePostion;
    private string textBoxName = "";
    private string colorBoxName = "";

     [SyncVar (hook = "OnChangeColor")] [SerializeField] private string pColor = "#ffffff";
     [SyncVar (hook = "OnChangeName")] [SerializeField] private string pName = "Player";
    // Start is called before the first frame update


    void OnChangeColor(string color)
    {
        pColor = color;
        Renderer[] rends = GetComponentsInChildren<Renderer>();

        foreach ( var r in rends)
        {
            if (r.gameObject.name == "BODY")
                r.material.SetColor("_Color", ColorFromHex(color)); 
        }
    }

    [Command]
    public void CmdChangeColor(string NewColor)
    {
        pColor = NewColor;
        Renderer[] rends = GetComponentsInChildren<Renderer>();

        foreach ( var r in rends)
        {
            if (r.gameObject.name == "BODY")
                r.material.SetColor("_Color", ColorFromHex(pColor)); 
        }
    }

    Color ColorFromHex(string hex)
    {
        hex = hex.Replace("0x", "");
        hex = hex.Replace("#", "");
        byte a = 255;
        byte r = byte.Parse(hex.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
        byte g = byte.Parse(hex.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
        byte b = byte.Parse(hex.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);
        
        if (hex.Length == 8)
        { 
            a = byte.Parse(hex.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);    
        }

        return new Color32(r, g, b, a);
    }
    
    void OnChangeName(string name)
    {
        pName = name;
        playerLabel.text = pName;
    }
    
    [Command]
    public void CmdChangeName(string newName)
    {
        pName = newName;
        playerLabel.text = pName;
    }
    
    private void OnGUI()
    {
        if (isLocalPlayer)
        {
            textBoxName = GUI.TextField(new Rect(25, 15, 100, 25), textBoxName);
            if (GUI.Button(new Rect(130, 15, 35, 25), "Set"))
                CmdChangeName(textBoxName);

            colorBoxName = GUI.TextField(new Rect(170, 15, 100, 25), colorBoxName);
            if (GUI.Button(new Rect(275,15,35,25), "Set"))
                 CmdChangeColor(colorBoxName);
                
            
        }
    }

    void Start()
    {
        if (isLocalPlayer)
        {
            GetComponent<PlayerController>().enabled = true;
            CameraFollow360.player = this.gameObject.transform;
        }
        else
        {
            GetComponent<PlayerController>().enabled = false;
        }

        GameObject canvas = GameObject.FindWithTag("MainCanvas");
        playerLabel = Instantiate(playerNamePrefab, Vector3.zero, Quaternion.identity);
        playerLabel.transform.SetParent(canvas.transform);
    }

    // Update is called once per frame
    void Update()
    {
        //determine if the object is inside the camera's viewing volume
        Vector3 screenPoint = Camera.main.WorldToViewportPoint(this.transform.position);
        bool onScreen = screenPoint.z > 0 && screenPoint.x > 0 && 
                        screenPoint.x < 1 && screenPoint.y > 0 && screenPoint.y < 1;
        //if it is on screen draw its label attached to is name position
        if(onScreen)
        {
            Vector3 nameLabelPos = Camera.main.WorldToScreenPoint(namePostion.position);
            playerLabel.transform.position = nameLabelPos;
        }
        else //otherwise draw it WAY off the screen 
            playerLabel.transform.position = new Vector3(-1000,-1000,0);
    }
}
