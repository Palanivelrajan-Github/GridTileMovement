using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class CheckInternetConnectivity : MonoBehaviour
{
    string m_ReachabilityText;
    
    private IEnumerator checkInternetConnection(Action<bool> action){
        WWW www = new WWW("http://google.com");
        yield return www;
        if (www.error != null) {
            action (false);
        } else {
            action (true);
        }
    }

    private void Start(){
        //method 1
        /*StartCoroutine(checkInternetConnection((isConnected)=>{
            // handle connection status here
            Debug.Log(isConnected);
        }));*/
    }


    private void Update()
    {
        
        //method 2
         /*Debug.Log(Application.internetReachability == NetworkReachability.NotReachable
            ? "Error. Check internet connection!"
            : "Internet connection is ok!");*/
         
                 
         //method 3
         //Output the network reachability to the console window
         Debug.Log("Internet : " + m_ReachabilityText);
         
         //Check if the device cannot reach the internet
         if (Application.internetReachability == NetworkReachability.NotReachable)
         {
             //Change the Text
             m_ReachabilityText = "Not Reachable.";
         }
         //Check if the device can reach the internet via a carrier data network
         else if (Application.internetReachability == NetworkReachability.ReachableViaCarrierDataNetwork)
         {
             m_ReachabilityText = "Reachable via carrier data network.";
         }
         //Check if the device can reach the internet via a LAN
         else if (Application.internetReachability == NetworkReachability.ReachableViaLocalAreaNetwork)
         {
             m_ReachabilityText = "Reachable via Local Area Network.";
         }
         
    }
}
