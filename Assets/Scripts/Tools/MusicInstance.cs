using System;
using System.Runtime.InteropServices;
using UnityEngine;
using FMOD.Studio;

public class MusicInstance
{
    public class TimelineInfo
    {
        public int CurrentMusicBar = 0;
        public FMOD.StringWrapper LastMarker = new();
    }

    public TimelineInfo timelineInfo;
    GCHandle timelineHandle;
    
    EventInstance evt;

    public MusicInstance(string _evt)
    {
        evt = FMODUnity.RuntimeManager.CreateInstance(_evt);
        
        timelineInfo = new TimelineInfo();
        evt.setUserData(GCHandle.ToIntPtr(timelineHandle));
        timelineHandle = GCHandle.Alloc(timelineInfo);
        
        evt.setCallback(BeatEventCallback, EVENT_CALLBACK_TYPE.TIMELINE_BEAT | EVENT_CALLBACK_TYPE.TIMELINE_MARKER);
        evt.start();
    }

    public void start()
    {
        evt.start();
    }

    public void stop(STOP_MODE mode)
    {
        evt.stop(mode);
    }

    public void Destroy()
    {
        evt.setUserData(IntPtr.Zero);
        evt.stop(STOP_MODE.IMMEDIATE);
        evt.release();
        timelineHandle.Free();
    }

    [AOT.MonoPInvokeCallback(typeof(EVENT_CALLBACK))]
    static FMOD.RESULT BeatEventCallback(EVENT_CALLBACK_TYPE type, IntPtr instancePtr, IntPtr parameterPtr)
    {
        EventInstance instance = new EventInstance(instancePtr);

        // Retrieve the user data
        IntPtr timelineInfoPtr;
        FMOD.RESULT result = instance.getUserData(out timelineInfoPtr);
        if (result != FMOD.RESULT.OK)
        {
            Debug.LogError("Timeline Callback error: " + result);
        }
        else if (timelineInfoPtr != IntPtr.Zero)
        {
            // Get the object to store beat and marker details
            GCHandle timelineHandle = GCHandle.FromIntPtr(timelineInfoPtr);
            TimelineInfo timelineInfo = (TimelineInfo)timelineHandle.Target;

            switch (type)
            {
                case EVENT_CALLBACK_TYPE.TIMELINE_BEAT:
                    {
                        var parameter = (TIMELINE_BEAT_PROPERTIES)Marshal.PtrToStructure(parameterPtr, typeof(TIMELINE_BEAT_PROPERTIES));
                        timelineInfo.CurrentMusicBar = parameter.bar;
                    }
                    break;
                case EVENT_CALLBACK_TYPE.TIMELINE_MARKER:
                    {
                        var parameter = (TIMELINE_MARKER_PROPERTIES)Marshal.PtrToStructure(parameterPtr, typeof(TIMELINE_MARKER_PROPERTIES));
                        timelineInfo.LastMarker = parameter.name;
                    }
                    break;
            }
        }
        return FMOD.RESULT.OK;
    }
}