﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class IsObserved : MonoBehaviour
{
    [SerializeField]
    Transform m_objectObserved;
    [SerializeField]
    bool m_showDebugRay;
    [SerializeField]
    public Transform m_head;
    

    [Tooltip("Precision range in degrees")]
    [SerializeField]
    float m_precisionRange = 45;
    [SerializeField]
    LayerMask m_layerRestriction;


    [SerializeField]
    UnityEvent StartedLooking;
    [SerializeField]
    UnityEvent StoppedLooking;

    [SerializeField]
    float m_lookDuration = 5;

    [SerializeField]
    UnityEvent LookedForDuration;

    bool m_isHitting;
    bool m_hasLookedForDuration;
    float m_currentlookDuration;

    Vector3 objectDirection;
    Quaternion localQuaternionOfObject;
    float angle;

    private void Reset()
    {
        m_objectObserved = transform;
    }

    private void Update()
    {
        objectDirection = m_objectObserved.position - m_head.position;
        localQuaternionOfObject = Quaternion.LookRotation(objectDirection, m_head.up);
        float angle = Quaternion.Angle(m_head.rotation, localQuaternionOfObject);

        if(m_showDebugRay)
            Debug.DrawRay(m_head.position, m_head.forward * 10, Color.red);

        RaycastHit hit;
        Physics.Raycast(m_head.position, objectDirection, out hit, Mathf.Infinity, m_layerRestriction);
        if (angle < m_precisionRange && (!Physics.Raycast(m_head.position, objectDirection, out hit, Mathf.Infinity, m_layerRestriction)|| hit.collider.gameObject == m_objectObserved.gameObject))
        {
            if (!m_isHitting)
            {
                StartedLooking.Invoke();
            }
            m_isHitting = true;


            m_currentlookDuration = m_currentlookDuration += Time.deltaTime;
            if (m_currentlookDuration > m_lookDuration && !m_hasLookedForDuration)
            {
                m_hasLookedForDuration = true;
                LookedForDuration.Invoke();
            }
        }
        else
        {
            if (m_showDebugRay)
                Debug.DrawRay(m_head.position, m_head.forward * 10, Color.white);
            if (m_isHitting)
            {
                StoppedLooking.Invoke();
            }

            m_currentlookDuration = 0;
            m_isHitting = false;
            m_hasLookedForDuration = false;
        }
    }
}
