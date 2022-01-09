using System.Threading;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

using ZXing;
using ZXing.QrCode;

public class QRcodeSystem : MonoBehaviour
{
    public enum SystemMode
    {
        GENERATE,
        SCAN,
        BOTH
    }
    public SystemMode systemMode = SystemMode.BOTH;


    public RawImage cameraRenderTexture;
    public RawImage generatedQRCodeTexture;

    public UnityAction QRCodeDetected;


    //----------- Private Fields --------------
    [SerializeField] 
    private CameraProperties cameraProperties;

    private WebCamTexture cameraTexture;
    private Texture2D tempQRCodeTexture2D;
    private Rect screenRect;
    private Color32[] cameraColor;
    private BarcodeReader qrCodeReader;
    private Thread renderingThread;

    private int camWidth, camHeight;
    private int defaultCameraFPS = 30;
    private const int threadSleepTime = 200;
    
    private bool shouldEncode = false;
    private string encodeData = string.Empty;

    private const string testQRCode = "www.fakesouls.com";


    private void Awake()
    {
        InitializeSystem();
    }


    private void OnEnable()
    {

        //We need camera permission or to render camera for QRCode Generation.
        if (systemMode == SystemMode.GENERATE)
            return;


        if (!PermissionManager.HaveCameraPermission)
        {
            PermissionManager.RequestCameraPermissionAction?.Invoke();
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
        tempQRCodeTexture2D = new Texture2D(256, 256);

        qrCodeReader = new BarcodeReader
        {
            AutoRotate = false,
            Options = new ZXing.Common.DecodingOptions { TryHarder = true } //Decodes aggressively (More computation)
        };


        //Follow Initialization should be only for QRScan
        if (systemMode != SystemMode.GENERATE)
        {
            screenRect = new Rect(0, 0, cameraRenderTexture.rectTransform.rect.width, cameraRenderTexture.rectTransform.rect.height);

            if (cameraProperties.useDefaultSettings)
            {
                //Setting default values.
                cameraProperties.requestedHeight = (int)screenRect.height;
                cameraProperties.requestedWidth = (int)screenRect.width;
                cameraProperties.requestedFPS = defaultCameraFPS;
            }

            cameraTexture = new WebCamTexture
            {
                requestedWidth = cameraProperties.requestedWidth,
                requestedHeight = cameraProperties.requestedHeight,
                requestedFPS = cameraProperties.requestedFPS > 0 ? cameraProperties.requestedFPS : (cameraProperties.requestedFPS = defaultCameraFPS)
            };

            camWidth = cameraTexture.width;
            camHeight = cameraTexture.height;

            OnEnable();
        }
    }

    public void ProcessQRCode()
    {
        //Starts a new Thread that Encodes/Decodes QRCode.
        renderingThread = new Thread(ReadQRCode);
        renderingThread.Start();
    }



    private void Update()
    {
        if(systemMode != SystemMode.GENERATE)
            RenderCamera();

        //Generates QRCode.
        if (shouldEncode && !string.IsNullOrEmpty(encodeData))
            GenerateQRCode(encodeData);

#if UNITY_EDITOR
        //Only for testing in the Editor
        TestSystem();
#endif

    }

    private void RenderCamera()
    {
        if (cameraColor == null && cameraTexture.isPlaying)
            cameraColor = cameraTexture.GetPixels32();

        if (cameraRenderTexture != null && cameraTexture.isPlaying)
            cameraRenderTexture.texture = cameraTexture;
    }



    public void GenerateQRCode(string encodeData)
    {
        if (string.IsNullOrEmpty(encodeData))
            return;

        var color32 = Encode(encodeData, tempQRCodeTexture2D.width , tempQRCodeTexture2D.height);
        tempQRCodeTexture2D.SetPixels32(color32);
        tempQRCodeTexture2D.Apply();

        generatedQRCodeTexture.texture = tempQRCodeTexture2D;
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
        OnDisable();
    }

    public string GetEncodedData()
    {
        return encodeData;
    }


    #region Testing (Editor Only)
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
