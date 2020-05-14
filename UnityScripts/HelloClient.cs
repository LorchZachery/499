using System;
using UnityEditor.Build;
using UnityEngine;
using System.Linq;

public class HelloClient : MonoBehaviour
{
    
    private HelloRequester _helloRequester;
    WebCamTexture webcamTexture;
    Color32[] data;
    private void Start()
    {
        webcamTexture = new WebCamTexture();
        webcamTexture.Play();
        data = new Color32[webcamTexture.width * webcamTexture.height];
        Debug.Log("height " + webcamTexture.width + "width" + webcamTexture.height);
        _helloRequester = new HelloRequester();
        webcamTexture.GetPixels32(data);
        _helloRequester.Start(data, webcamTexture.width, webcamTexture.height);
    }
    private void Update()
    {
        webcamTexture.GetPixels32(data);
       string message =  _helloRequester.Update(data);
       Debug.Log("Message in main " + message);
       // first number is how many soda cans are in the frame, so data can be parsed easier after
      
       double[] temp = new double[10 * 5];
       
       temp = Array.ConvertAll(message.Split(' '), double.Parse);
        int instances = (int)temp[0];
        double[] cooridantes = new double[instances * 5];
        cooridantes = temp.Where((v, i) => i != 0).ToArray();

        Debug.Log("array: " + String.Join(",", cooridantes));

    }


    private void OnDestroy()
    {
        _helloRequester.Stop();
    }
}