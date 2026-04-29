using UnityEngine;
using UnityEngine.VFX;

//머테리얼 컬러를 바꾸는 스크립트
public class ChangeMatColor : MonoBehaviour
{
    private ParticleSystemRenderer textCorn_Door;
    private VisualEffect textHoloEF;

    private void Awake()
    {
        textCorn_Door = GetComponent<ParticleSystemRenderer>();
        textHoloEF = transform.GetChild(0).GetComponent<VisualEffect>();
    }

    public void SetMatColor(Color color)
    {
        textCorn_Door.material.SetColor("_BaseColor", color);
        SetEFColor(color);
    }

    private void SetEFColor(Color color)
    {
        //ProjectionRayGradient용
        GradientColorKey[] gradientColorKeys1 = new GradientColorKey[2];
        gradientColorKeys1[0] = new GradientColorKey(color, 0f);
        gradientColorKeys1[1] = new GradientColorKey(color, 1f);

        GradientAlphaKey[] gradientAlphaKeys1 = new GradientAlphaKey[2];
        gradientAlphaKeys1[0] = new GradientAlphaKey(1f, 0f);
        gradientAlphaKeys1[1] = new GradientAlphaKey(0f, 1f);

        Gradient gradient1 = new Gradient();
        gradient1.SetKeys(gradientColorKeys1, gradientAlphaKeys1);

        //ProjectionDustGradient용
        GradientColorKey[] gradientColorKeys2 = new GradientColorKey[2];
        gradientColorKeys2[0] = new GradientColorKey(color, 0f);
        gradientColorKeys2[1] = new GradientColorKey(color, 1f);

        GradientAlphaKey[] gradientAlphaKeys2 = new GradientAlphaKey[2];
        gradientAlphaKeys2[0] = new GradientAlphaKey(0.49f, 0f);
        gradientAlphaKeys2[1] = new GradientAlphaKey(0f, 1f);

        Gradient gradient2 = new Gradient();
        gradient2.SetKeys(gradientColorKeys2, gradientAlphaKeys2);


        textHoloEF.SetGradient("Projection Ray Gradient", gradient1);
        textHoloEF.SetGradient("Projection Dust Gradient", gradient2);
    }
}
