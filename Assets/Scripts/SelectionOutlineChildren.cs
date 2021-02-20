using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class SelectionOutlineChildren : MonoBehaviour
{
    [FormerlySerializedAs("renderers")]
    public Renderer[] Renderers;
    
    float m_Highlighted = 0.0f;
    MaterialPropertyBlock m_Block;
    int m_HighlightActiveID;

    private void Start()
    {
        if(Renderers == null || Renderers.Length == 0)
        {
            Renderers = GetComponentsInChildren<Renderer>();
        }

        m_HighlightActiveID = Shader.PropertyToID("HighlightActive");
        m_Block = new MaterialPropertyBlock();
        m_Block.SetFloat(m_HighlightActiveID, m_Highlighted);
        for (int i = 0; i < Renderers.Length; i++)
        {
            Renderers[i].SetPropertyBlock(m_Block);
        }

    }

    public void Highlight()
    {
        m_Highlighted = 1.0f;

        GetAndSetPropertyBlocks();
    }
    public void RemoveHighlight()
    {
        m_Highlighted = 0.0f;

        GetAndSetPropertyBlocks();
    }

    private void GetAndSetPropertyBlocks()
    {
        for (int i = 0; i < Renderers.Length; i++)
        {
            Renderers[i].GetPropertyBlock(m_Block);
        }
        m_Block.SetFloat(m_HighlightActiveID, m_Highlighted);
        for (int i = 0; i < Renderers.Length; i++)
        {
            Renderers[i].SetPropertyBlock(m_Block);
        }
    }


}
