using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbrirConfig : MonoBehaviour {

  public GameObject Panel;

    public void  OpenPanelConfig(){
        if (Panel !=null)
        {
            Panel.SetActive(true);
        }
    }

    
}
