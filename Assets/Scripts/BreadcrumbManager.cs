using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreadcrumbManager : MonoBehaviour
{
    public static BreadcrumbManager singleton;
    public GameObject crumbPre;

    private List<List<GameObject>> crumbs = new List<List<GameObject>>();
    private List<float> crumbNums = new List<float>(); //Needs to be float to prevent rounding.
    private int lastBot = 0;

    void Awake()
    {
        singleton = this;
    }

    public void Init(int numBots)
    {
        for (int i = 0; i < numBots; ++i)
        {
            crumbs.Add(new List<GameObject>());
            crumbNums.Add(0);
        }
    }

    public void PlaceCrumb(Vector3 pos, int bot)
    {
        if (crumbs.Count < bot || crumbNums[bot] > 255)
            return;
        if (lastBot != bot)
            ResetCrumbs();
        lastBot = bot;
        crumbs[bot].Add(Instantiate(crumbPre, pos, new Quaternion(), transform));
        crumbNums[bot] = crumbNums[bot] + 1;
        for (int i = 0; i < crumbs[bot].Count; ++i)
            crumbs[bot][i].GetComponent<SpriteRenderer>().color = Color.HSVToRGB(i / (crumbNums[bot] + 1), 1f, 0.7f);
    }

    public void ResetCrumbs()
    {
        foreach (List<GameObject> GOL in crumbs)
        {
            foreach (GameObject c in GOL)
                Destroy(c);
            GOL.Clear();
        }

        for (int i = 0; i < crumbNums.Count; ++i)
            crumbNums[i] = 0;
    }
}
