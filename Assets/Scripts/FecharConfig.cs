using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FecharConfig : MonoBehaviour{
    
    public GameObject Panel;

    public void  ClosePanelConfig(){
        if (Panel !=null)
        {
            Panel.SetActive(false);
        }
    }
}
