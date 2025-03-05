using NUnit.Framework;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class dataBaseV3 : MonoBehaviour
{


    public List<Item> itemsList = new();

    private void Start()
    {   
        for(int i = 0; i < itemsList.Count; i++) {
            itemsList[i].id = i;

        }

    }

}
