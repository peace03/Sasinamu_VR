using UnityEngine;

//하위 Light 오브젝트들을 컨트롤하는 스크립트
public class LightObjsHandler : MonoBehaviour
{
    private ChangeMaterial[] lights;

    private void Awake()
    {
        lights = new ChangeMaterial[3];
        for (int i = 0; i < 3; i++)
        {
            lights[i] = transform.GetChild(i).GetComponent<ChangeMaterial>();
        }
    }

    public void SetMaterials(Material mat)
    {
        foreach (var obj in lights)
        {
            obj.SetMaterial(mat);
        }
    }
}
