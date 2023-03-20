using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectionManager : MonoBehaviour
{
    float maxDistanceRay = 100;
    float borderSelectionSize = 6;
    GameObject objetoSelecionado;
    GameObject previousSelected;

    GameObject[] allEnemies;
    GameObject closestEnemy;
    GameObject closestHere;
    string tagToDetect = "Enemies";


    private void Start()
    {
        allEnemies = GameObject.FindGameObjectsWithTag(tagToDetect);
    }

    void ClickSelect()
    {
        if (Input.GetMouseButtonDown(0))
        {
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, maxDistanceRay, LayerMask.GetMask("Enemies")))
            {
                if (objetoSelecionado == null)
                {
                    objetoSelecionado = hit.collider.gameObject;
                    objetoSelecionado.GetComponent<Outline>().OutlineWidth = borderSelectionSize;
                }
                else if (objetoSelecionado != null)
                {
                    previousSelected = objetoSelecionado;
                    previousSelected.GetComponent<Outline>().OutlineWidth = 0;
                    objetoSelecionado = objetoSelecionado = hit.collider.gameObject;
                    objetoSelecionado.GetComponent<Outline>().OutlineWidth = borderSelectionSize;
                }
            }
        }
        if (objetoSelecionado != null && Input.GetKeyDown(KeyCode.Escape))
        {
            if (previousSelected != null)
            {
                previousSelected.GetComponent<Outline>().OutlineWidth = 0;
            }
            objetoSelecionado.GetComponent<Outline>().OutlineWidth = 0;
            previousSelected = null;
            objetoSelecionado = null;
        }
    }

    void ClosestEnemy()
    {
        if (Input.GetKeyDown(KeyCode.Z))
        {
            float leastDistance = 10f;

            foreach (var enemy in allEnemies)
            {
                float distanceHere = Vector3.Distance(transform.position, enemy.transform.position);

                if (distanceHere < leastDistance)
                {
                    leastDistance = distanceHere;
                    closestHere = enemy;
                }
                print(closestHere);
            }
            if (objetoSelecionado == null)
            {
                objetoSelecionado = closestHere;
                objetoSelecionado.GetComponent<Outline>().OutlineWidth = borderSelectionSize;
            }
            else if (objetoSelecionado != null)
            {
                previousSelected = objetoSelecionado;
                previousSelected.GetComponent<Outline>().OutlineWidth = 0;
                objetoSelecionado = objetoSelecionado = closestHere;
                objetoSelecionado.GetComponent<Outline>().OutlineWidth = borderSelectionSize;
            }

        }
    }
    void Update()
    {
        ClickSelect();
        ClosestEnemy();
    }
}
