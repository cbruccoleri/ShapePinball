using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraPostProcess : MonoBehaviour
{
    /// <summary>
    /// Material used to attach the post-processing shader
    /// </summary>
    [SerializeField]
    private Material _material;

    private Camera _camera;

    private void Awake()
    {
        _camera = gameObject.GetComponent<Camera>();
    }

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        // set parameters to the shader
        Vector3 scrPt = _camera.WorldToScreenPoint(Vector3.zero);
        Vector4 param = new Vector4(scrPt.x, scrPt.y, scrPt.z, 0.0f);
        _material.SetVector("_Loc", param);
        //Debug.Log($"Screen Pt: {param}");

        // Render to the back buffer, as long as the camera render texture is set to null
        Graphics.Blit(source, destination, _material);
    }
}
