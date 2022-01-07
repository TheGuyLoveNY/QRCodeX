using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public Text statusText;
    public Image scanningIndicator;
    public QRcodeSystem qrCodeManager;


    private const string scanningText = "Scanning...";


    private void Start()
    {
        InitializeUI();
        SubscribeEvents();

        StartScanning();
    }

    private void InitializeUI()
    {
        if (statusText)
            statusText.gameObject.SetActive(false);

        if (scanningIndicator)
            scanningIndicator.gameObject.SetActive(false);
    }

    private void SubscribeEvents()
    {
        qrCodeManager.QRCodeDetected += PostQRCodeDetection;
    }

    public void StartScanning()
    {
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

        StopScanning();
    }

    private void SetStatusText(string text)
    {
        if (!statusText)
        {
            Debug.LogError("[UIManager.SetStatusText()] : StatusText Unassigned!");
            return;
        }

        statusText.gameObject.SetActive(true);
        statusText.text = text;
    }

}
