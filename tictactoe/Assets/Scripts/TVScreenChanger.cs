using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TVScreenChanger : MonoBehaviour
{

    // Materials 
    Material[] screensavers;
    private List<Material> screensaver_list = new List<Material>();

    public void ChangeTVScreen()
    {

        // Materials 
        GameObject tv_screen = GameObject.FindGameObjectWithTag("tv_screen");
        for (int j = 1; j < 5; j++)
        {
            string s = "tv_screensaver_" + j.ToString();
            //print ("String: " + s); 
            Material screensaver = Resources.Load(s, typeof(Material)) as Material;
            screensaver_list.Add(screensaver);
            //print (screensaver.name); 
        }

        int i = Random.Range(0, 4);
        Material material = screensaver_list[i];

        Material[] temp_screensavers = tv_screen.GetComponent<MeshRenderer>().materials;
        temp_screensavers[3] = material;
        tv_screen.GetComponent<MeshRenderer>().materials = temp_screensavers;
    }

}
