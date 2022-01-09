using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

using ZXing;
using ZXing.QrCode;

public class QRcodeSystem : MonoBehaviour
{
    //TODO: CLEAN UP!!

    public float readTime = 3f;
    public RawImage camTexture;
    public RawImage qrCodeRawImage;

    public string testQRCode = "www.google.com";

    public UnityAction QRCodeDetected;

    private WebCamTexture cameraTexture;
    private Texture2D tempQRCodeTexture2D;
    private Rect screenRect;
    private Color32[] cameraColor;
    private BarcodeReader qrCodeReader;
    private Thread renderingThread;

    private int camWidth, camHeight;
    private const int threadSleepTime = 200;

    private bool shouldEncode = false;
    private string encodeData = string.Empty;

    //Camera Properties
    [SerializeField] private CameraProperties cameraProperties;
    private int defaultCameraFPS = 30;

    private void Awake()
    {
        InitializeSystem();
    }


    private void OnEnable()
    {
        if (!PermissionManager.HaveCameraPermission)
        {
            PermissionManager.RequestCameraPermissionAction.Invoke();
            return;
        }

        if (cameraTexture != null && !cameraTexture.isPlaying)
        {
            cameraTexture.Play();
            camWidth = cameraTexture.width;
            camHeight = cameraTexture.height;
        }
    }

    private void OnDisable()
    {
        if (cameraTexture != null && cameraTexture.isPlaying)
            cameraTexture.Stop();

        if (renderingThread != null)
            renderingThread.Abort();
    }

    private void InitializeSystem()
    {
        //screenRect = new Rect(0, 0, Screen.width, Screen.height);
        screenRect = new Rect(0, 0, camTexture.rectTransform.rect.width, camTexture.rectTransform.rect.height);

        if (cameraProperties.useDefaultSettings)
        {
            //Setting default values.
            cameraProperties.requestedHeight = (int) screenRect.height;
            cameraProperties.requestedWidth = (int)screenRect.width;
            cameraProperties.requestedFPS = defaultCameraFPS;
        }

        cameraTexture = new WebCamTexture
        {
            requestedWidth = cameraProperties.requestedWidth,
            requestedHeight = cameraProperties.requestedHeight,
            requestedFPS = cameraProperties.requestedFPS > 0 ? cameraProperties.requestedFPS : (cameraProperties.requestedFPS = defaultCameraFPS)
        };

        //print($"Width : {camTexture.rectTransform.rect.width} | Heigth : {camTexture.rectTransform.rect.height}");

        //cameraTexture = new WebCamTexture
        //{
        //    requestedWidth = (int) camTexture.rectTransform.rect.width,
        //    requestedHeight = (int) camTexture.rectTransform.rect.height,
        //    requestedFPS = cameraProperties.requestedFPS > 0 ? cameraProperties.requestedFPS : (cameraProperties.requestedFPS = defaultCameraFPS)
        //};

        camWidth = cameraTexture.width;
        camHeight = cameraTexture.height;

        tempQRCodeTexture2D = new Texture2D(256, 256);

        qrCodeReader = new BarcodeReader
        {
            AutoRotate = false,
            Options = new ZXing.Common.DecodingOptions { TryHarder = true }
        };

        OnEnable();
    }

    public void ProcessQRCode()
    {
        //VIsual effect here for the user and better UX

        renderingThread = new Thread(ReadQRCode);
        renderingThread.Start();
    }



    private void Update()
    {
        if (cameraColor == null && cameraTexture.isPlaying)
            cameraColor = cameraTexture.GetPixels32();

        if (camTexture != null && cameraTexture.isPlaying)
            camTexture.texture = cameraTexture;

        if (shouldEncode && !string.IsNullOrEmpty(encodeData))
            GenerateQRCode(encodeData);

        //TODO: Remove (only for testing)
        TestSystem();
    }

    public void GenerateQRCode(string encodeData)
    {
        if (string.IsNullOrEmpty(encodeData) || !shouldEncode)
            return;

        var color32 = Encode(encodeData, tempQRCodeTexture2D.width , tempQRCodeTexture2D.height);
        tempQRCodeTexture2D.SetPixels32(color32);
        tempQRCodeTexture2D.Apply();

        qrCodeRawImage.texture = tempQRCodeTexture2D;
        QRCodeDetected?.Invoke();
        shouldEncode = false;
    }


    private void ReadQRCode()
    {
        if (cameraTexture == null || qrCodeReader == null)
        {
            Debug.LogError($"[ReadQRCode()]: CameraTexture or QRCodeReader is null ");
            return;
        }

        while (true)
        {
            try
            {
                //Decode the curent frame
                var result = qrCodeReader.Decode(cameraColor, camWidth, camHeight);
                if (result != null && result.Text != encodeData)
                {
                    encodeData = result.Text;
                    shouldEncode = true;
                }

                //Sleep and then reset the captured frame.
                Thread.Sleep(threadSleepTime);
                cameraColor = null;
            }
            catch
            {  }
        }
    }

    private static Color32[] Encode(string encodingData, int width, int height)
    {
        var qrCodeWriter = new BarcodeWriter
        {
            Format = BarcodeFormat.QR_CODE,
            Options = new QrCodeEncodingOptions
            {
                Height = height,
                Width = width
            }
        };

        return qrCodeWriter.Write(encodingData);
    }



    public void StopQRScan()
    {
        if (renderingThread != null)
            renderingThread.Abort();
    }

    public string GetEncodedData()
    {
        return encodeData;
    }


    #region Testing

    private void TestSystem()
    {
        if (Input.GetKeyDown(KeyCode.E))
            PrintCameraProperties();

        if (Input.GetKeyDown(KeyCode.T))
            PrintScreenRect();

        if (Input.GetKeyDown(KeyCode.M))
        {
            shouldEncode = true;
            encodeData = testQRCode;
        }

    }


    private void PrintCameraProperties()
    {
        print("************ Camera Properties *********** \n" +
            $"Height : {cameraProperties.requestedHeight} " +
            $"Width : {cameraProperties.requestedWidth}" +
            $"FPS: {cameraProperties.requestedFPS}");
    }

    private void PrintScreenRect()
    {
        print("********** ScreenRect *************** \n" +
            $"Position X: {screenRect.x} | Y: {screenRect.y}" +
            $"Height : {screenRect.height}" +
            $"Width : {screenRect.width}");
    }




    #endregion

}
