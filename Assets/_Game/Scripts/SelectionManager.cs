using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectionManager : MonoBehaviour
{
    float maxDistanceRay = 100;
    float borderSelectionSize = 6;
    GameObject objetoSelecionado;
    GameObject previousSelected;

    void Update()
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
}
