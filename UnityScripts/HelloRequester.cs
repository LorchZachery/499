using AsyncIO;
using NetMQ;
using NetMQ.Sockets;
using UnityEngine;
using System.Runtime.InteropServices;
using System;

/// <summary>
///     Example of requester who only sends Hello. Very nice guy.
///     You can copy this class and modify Run() to suits your needs.
///     To use this class, you just instantiate, call Start() when you want to start and Stop() when you want to stop.
/// </summary>
public class HelloRequester : RunAbleThread
{
    /// <summary>
    ///     Request Hello message to server and receive message back. Do it 10 times.
    ///     Stop requesting when Running=false.
    /// </summary>
    
    
    protected override void Run()
    {
        ForceDotNet.Force(); // this line is needed to prevent unity freeze after one use, not sure why yet
        
        using (RequestSocket client = new RequestSocket())
        {
            
            client.Connect("tcp://localhost:5555");
			
			    string output = null;
			    bool moveOn = false;
                //inilize connection
                client.SendFrame("hello");
                while (Running){
                    
                    moveOn = client.TryReceiveFrameString(out output);
			    	if (moveOn) break;
			    }
			    if (moveOn) Debug.Log("Received " + output);
			    moveOn = false;
			    //first sent height and width to the socket
			    byte[] Hbytes = BitConverter.GetBytes(Height);
                    if (BitConverter.IsLittleEndian)
                        Array.Reverse(Hbytes);
                byte[] Wbytes = BitConverter.GetBytes(Width);
                    if (BitConverter.IsLittleEndian)
                        Array.Reverse(Wbytes);
                //height
                client.SendFrame(Hbytes);
                while (Running){
                   
                    moveOn = client.TryReceiveFrameString(out output);
			    	if (moveOn) break;
			    }
			    if (moveOn) Debug.Log("Received " + output);
			    moveOn = false;
                //width 
                client.SendFrame(Wbytes);
                while (Running){
                    
			    	moveOn = client.TryReceiveFrameString(out output);
			    	if (moveOn) break;
			    }
			    if (moveOn) Debug.Log("Received " + output);
			    moveOn = false;
			
            while(Running)
            {
                
                Debug.Log("Sending Frame");
                client.SendFrame(Color32ArrayToByteArray(Data));
                

               
                // ReceiveFrameString() blocks the thread until you receive the string, but TryReceiveFrameString()
                // do not block the thread, you can try commenting one and see what the other does, try to reason why
                // unity freezes when you use ReceiveFrameString() and play and stop the scene without running the server
//                string message = client.ReceiveFrameString();
//                Debug.Log("Received: " + message);
                string message = null;
                bool gotMessage = false;
                while (Running)
                {
                    gotMessage = client.TryReceiveFrameString(out message); // this returns true if it's successful
                    if (gotMessage) break;
                }
				
				//int converted =  Convert.ToInt32(message);
                if (gotMessage) Debug.Log("Received output " + message);
                Coor = message; 
            }
        }

        NetMQConfig.Cleanup(); // this line is needed to prevent unity freeze after one use, not sure why yet
    }
    private static byte[] Color32ArrayToByteArray(Color32[] colors)
    {
        if (colors == null || colors.Length == 0)
            return null;

        int lengthOfColor32 = Marshal.SizeOf(typeof(Color32));
        int length = lengthOfColor32 * colors.Length;
        byte[] bytes = new byte[length];

        GCHandle handle = default(GCHandle);
        try
        {
            handle = GCHandle.Alloc(colors, GCHandleType.Pinned);
            IntPtr ptr = handle.AddrOfPinnedObject();
            Marshal.Copy(ptr, bytes, 0, length);
        }
        finally
        {
            if (handle != default(GCHandle))
                handle.Free();
        }

        return bytes;
    }
}