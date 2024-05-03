using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.Universal;

public class Screenshot : MonoBehaviour {

    private static Screenshot instance;

    [SerializeField]
    private bool dontDestroyOnLoad;

    private void Start() {
        if (!instance && dontDestroyOnLoad) {
            DontDestroyOnLoad(gameObject);
            instance = this;
        } else {
            Destroy(gameObject);
        }
    }

    private void Update() {
        string fileName = Time.frameCount + ".png";
        if (Keyboard.current.pKey.wasPressedThisFrame) {
            Debug.Log("Captured Normal Screenshot: " + fileName);
            ScreenCapture.CaptureScreenshot(fileName);
        } else if (Keyboard.current.oKey.wasPressedThisFrame) {
            Debug.Log("Captured Transparent Screenshot: " + fileName);
            TakeTransparentScreenshot(Camera.main, Screen.width, Screen.height, fileName);
        }
    }

    //https://forum.unity.com/threads/screenshot-with-transparent-background-postprocess.1008244/#post-8593033
    private static void TakeTransparentScreenshot(Camera cam, int width, int height, string savePath) {
        var urpCameraData = cam.GetComponent<UniversalAdditionalCameraData>();

        bool wasPostprocessing = urpCameraData.renderPostProcessing;
        RenderTexture wasTargetTexture = cam.targetTexture;
        CameraClearFlags wasClearFlags = cam.clearFlags;
        RenderTexture wasActive = RenderTexture.active;

        Texture2D transparentTexture = new Texture2D(width, height, TextureFormat.ARGB32, false);
        Texture2D postprocessTexture = new Texture2D(width, height, TextureFormat.ARGB32, false);
        RenderTexture renderTexture = RenderTexture.GetTemporary(width, height, 24, RenderTextureFormat.ARGB32);
        Rect grabArea = new Rect(0, 0, width, height);

        RenderTexture.active = renderTexture;
        cam.targetTexture = renderTexture;
        cam.clearFlags = CameraClearFlags.SolidColor;

        cam.backgroundColor = Color.clear;
        urpCameraData.renderPostProcessing = false;
        cam.Render();
        transparentTexture.ReadPixels(grabArea, 0, 0);
        transparentTexture.Apply();

        urpCameraData.renderPostProcessing = true;
        cam.Render();
        postprocessTexture.ReadPixels(grabArea, 0, 0);
        postprocessTexture.Apply();

        Color[] transparentPixels = transparentTexture.GetPixels();
        Color[] postprocessPixels = transparentTexture.GetPixels();

        for (var i = 0; i < transparentPixels.Length; i++) {
            Color c = postprocessPixels[i];
            c.a = transparentPixels[i].a;
            postprocessPixels[i] = c;
        }

        postprocessTexture.SetPixels(postprocessPixels);

        byte[] pngShot = postprocessTexture.EncodeToPNG();
        File.WriteAllBytes(savePath, pngShot);

        cam.clearFlags = wasClearFlags;
        cam.targetTexture = wasTargetTexture;
        urpCameraData.renderPostProcessing = wasPostprocessing;
        RenderTexture.active = wasActive;
        RenderTexture.ReleaseTemporary(renderTexture);
        Destroy(transparentTexture);
        Destroy(postprocessTexture);
    }

}