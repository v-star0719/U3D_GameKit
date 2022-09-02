using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//将gameObject按网格排列在3D空间中
[ExecuteInEditMode]
public class Grid3D : MonoBehaviour
{
    public Vector3 horzOffset;
    public Vector3 vertOffset;

    public int itemPerLine;
    public bool reposition;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (reposition)
        {
            //reposition = false;
            Repositon();
        }
    }

    private void Repositon()
    {
        int itemLimit = itemPerLine;
        if (itemLimit <= 0)
        {
            itemLimit = int.MaxValue;
        }

        Vector3 lineStart = Vector3.zero;
        Vector3 curPos = lineStart;
        int count = transform.childCount;
        int col = 0;
        for (int i = 0; i < count; i++)
        {
            var trans = transform.GetChild(i);
            trans.localPosition = curPos;

            col++;
            if (col == itemLimit)
            {
                col = 0;
                lineStart = lineStart + vertOffset;
                curPos = lineStart;
            }
            else
            {
                curPos = curPos + horzOffset;
            }
        }


    }
}
