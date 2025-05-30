using System;
using UnityEngine;

public class ChangeTexture : MonoBehaviour
{
    [SerializeField] MeshRenderer bookMesh;
    [SerializeField] Material[] materials = new Material[2];
    private int currentIndexOfMaterial = 0;
    private void Start()
    {
        bookMesh.material = materials[currentIndexOfMaterial];
    }
    public void ChangeTextureOnClick()
    {
        currentIndexOfMaterial = currentIndexOfMaterial == 0 ? 1 : 0;
        bookMesh.material = materials[currentIndexOfMaterial];
    }
}
