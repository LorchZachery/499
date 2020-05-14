﻿using System;
using System.Threading;
using UnityEngine;

/// <summary>
///     The superclass that you should derive from. It provides Start() and Stop() method and Running property.
///     It will start the thread to run Run() when you call Start().
/// </summary>
public abstract class RunAbleThread
{
    private readonly Thread _runnerThread;

    protected RunAbleThread()
    {
        // we need to create a thread instead of calling Run() directly because it would block unity
        // from doing other tasks like drawing game scenes
        _runnerThread = new Thread(Run);
    }


    protected string Coor { get; set; }
    protected bool Running { get; private set; }
    protected Color32[] Data { get; private set; }
    protected int Height { get; private set; }
    protected int Width { get; private set; }
    /// <summary>
    /// This method will get called when you call Start(). Programmer must implement this method while making sure that
    /// this method terminates in a finite time. You can use Running property (which will be set to false when Stop() is
    /// called) to determine when you should stop the method.
    /// </summary>
    protected abstract void Run();

    public string Start(Color32[] data, int height, int width)
    {
		Coor = "";
        Running = true;
        Data = data;
        Height = height;
        Width = width;
        _runnerThread.Start();
        return Coor;
    }
    public string Update(Color32[] data)
    {
		
        Data = data;
        return Coor;
    }

    public void Stop()
    {
        Running = false;
        // block main thread, wait for _runnerThread to finish its job first, so we can be sure that 
        // _runnerThread will end before main thread end
        _runnerThread.Join();
    }
}