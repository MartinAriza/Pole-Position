﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] public GameObject m_Focus;

    [SerializeField] public Vector3 m_offset = new Vector3(10, 10, 10);

    [SerializeField] public CircuitController m_Circuit;
    [SerializeField] private float m_Distance = 8; 
    [SerializeField] private float m_Elevation = 4;
    [Range(0, 1)] [SerializeField] private float m_Following = 1f; 

    private Vector3 m_Direction = Vector3.zero;

    private Camera mainCamera;

    void Start()
    {
        mainCamera = this.GetComponent<Camera>();
    }

    //Ajusta la cámara a la posición del jugador por el circuito cada frame de forma suave
    void Update()
    {
        if (m_Focus != null)
        {
            if (this.m_Circuit != null)
            {
                if (this.m_Direction.magnitude == 0)
                {
                    this.m_Direction = new Vector3(0f, -1f, 0f);
                }

                int segIdx;
                float carDist;
                Vector3 carProj;

                m_Circuit.ComputeClosestPointArcLength(m_Focus.transform.position, out segIdx, out carProj,
                    out carDist);

                Vector3 pathDir = -m_Circuit.GetSegment(segIdx);
                pathDir = new Vector3(pathDir.x, 0f, pathDir.z);
                pathDir.Normalize();

                this.m_Direction = Vector3.Lerp(this.m_Direction, pathDir, this.m_Following * Time.deltaTime * 2);
                Vector3 offset = this.m_Direction * this.m_Distance;
                offset = new Vector3(offset.x, m_Elevation, offset.z);

                mainCamera.transform.position = m_Focus.transform.position + offset;
                mainCamera.transform.LookAt(m_Focus.transform.position);
            }
            else
            {
                mainCamera.transform.position = m_Focus.transform.position + m_offset;
                mainCamera.transform.LookAt(m_Focus.transform.position);
            }
        }
    }
}