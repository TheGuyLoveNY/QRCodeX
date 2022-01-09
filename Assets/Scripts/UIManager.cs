using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public Text statusText;
    public Button urlButton;
    public Image scanningIndicator;
    public Image processStatusIcon;
    public Sprite successIcon;
    public Sprite failIcon;
    public RawImage generatedQRCodeImage;
    public QRcodeSystem qrCodeManager;

    private const string scanningText = "Scanning...";
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
    }

    private void SubscribeEvents()
    {
        qrCodeManager.QRCodeDetected += PostQRCodeDetection;
        urlButton.onClick.AddListener(OpenURL);
    }

    public void StartScanning()
    {

        //User hasn't granted Permission!
        if (PermissionManager.HaveCameraPermission)
        {
            SetStatus(noCameraPermissionText, true);
            return;
        }

        scanningIndicator.gameObject.SetActive(true);
        qrCodeManager.ProcessQRCode();
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

        //StopScanning();
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

}
