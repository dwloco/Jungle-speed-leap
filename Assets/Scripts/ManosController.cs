﻿using System;
using System.Collections.Generic;
using UnityEngine;

public class ManosController : MonoBehaviour {

    public int iIndexJug;
    GameObject totem;
    GameObject manoIzq, manoDer;
    TotemBehaviour totemBehaviour;
    Vector3[] posicionInicial = new Vector3[2]; //Posicion inicial de las manitos
    Lerpeador lerpMov, lerpMovBack, lerpMovUp;
    //Lerpeador lerpRot;
    int iEstado; //0 empieza a moverse - 1 en movimiento - 2 movido
    Vector3 corrimiento;
    Boolean bLoAgarre;
    

    // Use this for initialization
    void Start()
    {
        lerpMov = new Lerpeador(1);
        lerpMovBack = new Lerpeador(1);
        lerpMovUp = new Lerpeador(0.5f);
        //lerpRot = new Lerpeador(0.5f);

        manoIzq = transform.GetChild(0).gameObject;
        manoDer = transform.GetChild(1).gameObject;
        totem = TotemManager.instance.totem;
        totemBehaviour = totem.GetComponent<TotemBehaviour>();
        PonerManos();
        iEstado = 0;
        posicionInicial[0] = manoIzq.transform.position;
        posicionInicial[1] = manoDer.transform.position;
        bLoAgarre = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (MesaManager.instance.iIndexJugActual == iIndexJug)
        {
            //TODO: Animacion tocar mazo
        }

        bool bAgarrarPosible = MesaManager.instance.mesa.TieneIgualdadConResto(iIndexJug);
        if (bAgarrarPosible)
        {
            if (!totemBehaviour.estaAgarrado() || totemBehaviour.ObtenerJugador() == iIndexJug)
            {
                IntentarAgarrar();
            } else
            {
                Retroceder();
            }
        } else
        {
            Retroceder();
        }
    }

    int iEstadoRetroceso = 0;
    /// <summary>
    /// Si la mano se queda en el medio del recorrido en un momento que no corresponde o otro agarra el totem antes, vuelve para atras
    /// </summary>
    private void Retroceder()
    {
        if (manoDer.transform.position != posicionInicial[1])
        {
            if (iEstadoRetroceso == 0)
            {
                lerpMovBack.Start(manoDer, posicionInicial[1]);
                iEstadoRetroceso++;
            }
            else if (iEstadoRetroceso == 1)
            {
                if (lerpMovBack.Update())
                {
                    iEstadoRetroceso++;
                }
            }
            else
            {
                iEstadoRetroceso = 0;
                lerpMovBack = new Lerpeador(1f);
            }
        }
    }

    /// <summary>
    /// Realiza los movimientos para robar el totem
    /// </summary>
    private void IntentarAgarrar()
    {

        switch (iEstado)
        {
            case 0:
                lerpMov.Start(manoDer, totem.transform.position + corrimiento);
                iEstado++;
                break;
            case 1:
                if (lerpMov.Update())
                {
                    iEstado++;
                }
                break;
            case 2:
                //Animacion de cerrar mano
                lerpMovUp.Start(manoDer, manoDer.transform.position + Vector3.up * 0.2f);
                totemBehaviour.fijarTotemEnMano(manoDer.transform);
                iEstado++;
                break;
            case 3:
                if (lerpMovUp.Update())
                {
                    iEstado++;
                }
                break;
            case 4:
                lerpMovBack.Start(manoDer, posicionInicial[1]);
                iEstado++;
                break;
            case 5:
                if (lerpMovBack.Update())
                {
                    iEstado++;
                }
                break;
            default:
                //Se reincian todos los lerpeadores y vuelve al estado inicial
                iEstado = 0;
                lerpMov = new Lerpeador(1);
                lerpMovBack = new Lerpeador(1);
                lerpMovUp = new Lerpeador(0.5f);
                break;
        }
    }

    /*Quaternion ObtenerRotacion()
    {
        Quaternion rotacion = Quaternion.Euler(0, 0, 0);
        if (iIndexJug == 1)
        {
            rotacion = Quaternion.Euler(0, 0, 0);
        } else
        {
            rotacion = Quaternion.Euler(0, 90, 0);
        } 
        return rotacion;
    }*/

    void PonerManos()
    {
        float fDistancia = 0.08f; //Espacio contra el totem
        float fCorrimientoMano = 0.05f; //Evitar atravesar el totem
        float fDistY = 0.08f;
        switch (iIndexJug)
        {
            case 1:
                manoIzq.transform.position = totem.transform.position + new Vector3(0.35f, 0.1f, -0.09f);
                manoDer.transform.position = totem.transform.position + new Vector3(0.35f, 0.1f, 0.09f);
                manoIzq.transform.rotation = Quaternion.Euler(0, 0, 90);
                manoDer.transform.rotation = Quaternion.Euler(0, 180, -90);
                corrimiento = new Vector3(fDistancia, fDistY, fCorrimientoMano);
                break;
            case 2:
                manoIzq.transform.position = totem.transform.position + new Vector3(0.09f, 0.1f, 0.35f);
                manoDer.transform.position = totem.transform.position + new Vector3(-0.09f, 0.1f, 0.35f);
                manoIzq.transform.rotation = Quaternion.Euler(0, 270, 90);
                manoDer.transform.rotation = Quaternion.Euler(0, -270, -90);
                corrimiento = new Vector3(-fCorrimientoMano, fDistY, fDistancia);
                break;
            case 3:
                manoIzq.transform.position = totem.transform.position - new Vector3(0.35f, -0.1f, -0.09f);
                manoDer.transform.position = totem.transform.position - new Vector3(0.35f, -0.1f, 0.09f);
                manoIzq.transform.rotation = Quaternion.Euler(0, 180, 90);
                manoDer.transform.rotation = Quaternion.Euler(0, 0, -90);
                corrimiento = new Vector3(-fDistancia, fDistY, -fCorrimientoMano);
                break;
            default:
                Debug.Log("Esto no deberia pasar xdxd");
                break;
        }
    }
}
