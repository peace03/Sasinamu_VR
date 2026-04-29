using UnityEngine;

public class ChangeMaterial : MonoBehaviour
{
    private MeshRenderer selfMat;

    private void Awake()
    {
        selfMat = GetComponent<MeshRenderer>();
    }

    public void SetMaterial(Material mat)
    {
        selfMat.material = mat;
    }
}