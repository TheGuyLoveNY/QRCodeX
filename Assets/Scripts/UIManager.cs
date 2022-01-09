using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public enum UIMode { QRScan, QRGenerate, MainMenu }
    [SerializeField] private UIMode mode = UIMode.QRGenerate;
    [SerializeField] private QRcodeSystem qrCodeManager;

    [Header("MainMenu UI")]
    [SerializeField] private Button scanButton;
    [SerializeField] private Button generateButton;

    [Header("QRGenerator UI")]
    [SerializeField] private InputField urlInputField;
    [SerializeField] private Button generateQRButton;
    [SerializeField] private Button generateSceneBackButton;

    [Header("QRScan UI")]
    [SerializeField] private Text statusText;
    [SerializeField] private Button urlButton;
    [SerializeField] private Image scanningIndicator;
    [SerializeField] private Image processStatusIcon;
    [SerializeField] private Sprite successIcon;
    [SerializeField] private Sprite failIcon;
    [SerializeField] private RawImage generatedQRCodeImage;
    [SerializeField] private Button scanSceneBackButton;


    private const string noCameraPermissionText = "Camera Permission Denied!";


    private void Start()
    {
        Initialize();
        SubscribeEvents();

        StartScanning();
    }

    private void OnApplicationFocus(bool hasFocus)
    {
        StartScanning();
    }

    private void Initialize()
    {
        if (statusText)
            statusText.gameObject.SetActive(false);

        if (scanningIndicator)
            scanningIndicator.gameObject.SetActive(false);

        if (urlButton)
            urlButton.gameObject.SetActive(false);

        if (generatedQRCodeImage)
            generatedQRCodeImage.gameObject.SetActive(false);

        if (processStatusIcon)
            processStatusIcon.gameObject.SetActive(false);


        //Request Camera Permission here for MAIN MENU MODE
    }

    private void SubscribeEvents()
    {
        switch (mode)
        {
            case UIMode.QRScan:
                qrCodeManager.QRCodeDetected += PostQRCodeDetection;
                urlButton.onClick.AddListener(OpenURL);
                scanSceneBackButton.onClick.AddListener(LoadMainMenu);
                break;

            case UIMode.QRGenerate:
                generateQRButton.onClick.AddListener(GenerateQRCode);
                generateSceneBackButton.onClick.AddListener(LoadMainMenu);
                break;

            case UIMode.MainMenu:
                scanButton.onClick.AddListener(CustomSceneManager.LoadScanScene);
                generateButton.onClick.AddListener(CustomSceneManager.LoadGenerateScene);
                break;
        }
    }

    public void StartScanning()
    {
        //Only for Scan Mode!
        if (mode == UIMode.QRScan)
        {
            //User hasn't granted Permission!
            if (!PermissionManager.HaveCameraPermission)
            {
                SetStatus(noCameraPermissionText, true);
                return;
            }

            scanningIndicator.gameObject.SetActive(true);
            qrCodeManager.ProcessQRCode();
        }
    }

    public void StopScanning()
    {
        scanningIndicator.gameObject.SetActive(false);
        qrCodeManager.StopQRScan();
    }

    private void PostQRCodeDetection()
    {

        var encodeData = qrCodeManager.GetEncodedData();

        if (!string.IsNullOrEmpty(encodeData))
            SetStatus(encodeData);
    }

    private void SetStatus(string text, bool statusFailed = false)
    {
        if (!statusText)
        {
            Debug.LogError("[UIManager.SetStatusText()] : StatusText Unassigned!");
            return;
        }

        urlButton.gameObject.SetActive(true);
        statusText.gameObject.SetActive(true);
        processStatusIcon.gameObject.SetActive(true);

        if (!statusFailed)
            generatedQRCodeImage.gameObject.SetActive(true);

        statusText.text = text;
        statusText.color = statusFailed ? Color.red : Color.white;
        processStatusIcon.sprite = statusFailed ? failIcon : successIcon;
    }


    private void OpenURL()
    {
        var encodedData = qrCodeManager.GetEncodedData();
        URLBrowser.OpenURL(encodedData);
    }

    private void GenerateQRCode()
    {
        if (urlInputField == null || string.IsNullOrEmpty(urlInputField.text))
            return;

        //Remove any whitespace in the url.
        string url = urlInputField.text.Trim();
        qrCodeManager.GenerateQRCode(url);
    }

    private void LoadMainMenu()
    {
        CustomSceneManager.LoadMainMenuScene();
    }


}
