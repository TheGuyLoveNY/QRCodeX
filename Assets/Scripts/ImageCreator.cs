using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class ImageCreator
{
    private const string imageName = "/TempQRCodImage.jpg";


    public static string SaveTextureToJPG(Texture2D texture)
    {
        byte[] imageRawData = texture.EncodeToJPG();

        string savePath = Application.persistentDataPath + imageName;
        File.WriteAllBytes(savePath, imageRawData);

        return savePath;
    }

}
