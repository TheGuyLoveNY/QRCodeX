using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public Text statusText;
    public Button urlButton;
    public Image scanningIndicator;
    public RawImage generatedQRCodeImage;
    public QRcodeSystem qrCodeManager;


    private const string scanningText = "Scanning...";
    private const string noCameraPermissionText = "Error: Camera Permission Required!";


    private void Start()
    {
        InitializeUI();
        SubscribeEvents();
    }

    private void OnEnable()
    {
        StartScanning();
    }

    private void InitializeUI()
    {
        if (statusText)
            statusText.gameObject.SetActive(false);

        if (scanningIndicator)
            scanningIndicator.gameObject.SetActive(false);

        if (urlButton)
            urlButton.gameObject.SetActive(false);

        if (generatedQRCodeImage)
            generatedQRCodeImage.gameObject.SetActive(false);
    }

    private void SubscribeEvents()
    {
        qrCodeManager.QRCodeDetected += PostQRCodeDetection;
        urlButton.onClick.AddListener(OpenURL);
    }

    public void StartScanning()
    {

        //User hasn't granted Permission!
        if (!PermissionManager.HaveCameraPermission)
        {
            SetStatusText(noCameraPermissionText, true);
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
            SetStatusText(encodeData);

        //StopScanning();
    }

    private void SetStatusText(string text, bool errorText = false)
    {
        if (!statusText)
        {
            Debug.LogError("[UIManager.SetStatusText()] : StatusText Unassigned!");
            return;
        }

        urlButton.gameObject.SetActive(true);
        statusText.gameObject.SetActive(true);
        generatedQRCodeImage.gameObject.SetActive(true);

        statusText.text = text;
        statusText.color = errorText ? Color.red : Color.white;
    }


    private void OpenURL()
    {
        var encodedData = qrCodeManager.GetEncodedData();
        URLBrowser.OpenURL(encodedData);
    }

}
