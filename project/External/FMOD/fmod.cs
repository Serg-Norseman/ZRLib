/* ========================================================================================== */
/*                                                                                            */
/* FMOD Ex - C# Wrapper . Copyright (c), Firelight Technologies Pty, Ltd. 2004-2014.          */
/*                                                                                            */
/* ========================================================================================== */

using System;
using System.Text;
using System.Runtime.InteropServices;

// ReSharper disable EnumUnderlyingTypeIsInt

namespace FMOD
{
    /*
        FMOD version number.  Check this against FMOD.System.getVersion / System_GetVersion
        0xaaaabbcc -> aaaa = major version number.  bb = minor version number.  cc = development version number.
    */
    public class VERSION
    {
        public const int    number = 0x00044434;
#if WIN64
        public const string dll    = "fmodex64";
#else
        public const string dll    = "fmodex";
#endif
    }

    /*
        FMOD types 
    */
    /*
    [STRUCTURE] 
    [
        [DESCRIPTION]   
        Structure describing a point in 3D space.

        [REMARKS]
        FMOD uses a left handed co-ordinate system by default.
        To use a right handed co-ordinate system specify FMOD_INIT_3D_RIGHTHANDED from FMOD_INITFLAGS in System.init.

        [PLATFORMS]
        Win32, Win64, Linux, Linux64, Macintosh, Xbox360, PlayStation Portable, PlayStation 3, Wii

        [SEE_ALSO]      
        System.set3DListenerAttributes
        System.get3DListenerAttributes
        Channel.set3DAttributes
        Channel.get3DAttributes
        Geometry.addPolygon
        Geometry.setPolygonVertex
        Geometry.getPolygonVertex
        Geometry.setRotation
        Geometry.getRotation
        Geometry.setPosition
        Geometry.getPosition
        Geometry.setScale
        Geometry.getScale
        FMOD_INITFLAGS
    ]
    */
    [StructLayout(LayoutKind.Sequential)]
    public struct VECTOR
    {
        public float x;        /* X co-ordinate in 3D space. */
        public float y;        /* Y co-ordinate in 3D space. */
        public float z;        /* Z co-ordinate in 3D space. */
    }


    /*
    [STRUCTURE] 
    [
        [DESCRIPTION]   
        Structure describing a globally unique identifier.

        [REMARKS]

        [PLATFORMS]
        Win32, Win64, Linux, Linux64, Macintosh, Xbox360, PlayStation Portable, PlayStation 3, Wii

        [SEE_ALSO]      
        System.getDriverInfo
    ]
    */
    [StructLayout(LayoutKind.Sequential)]
    public struct GUID
    {
        public uint   Data1;       /* Specifies the first 8 hexadecimal digits of the GUID */
        public ushort Data2;       /* Specifies the first group of 4 hexadecimal digits.   */
        public ushort Data3;       /* Specifies the second group of 4 hexadecimal digits.  */
        [MarshalAs(UnmanagedType.ByValArray,SizeConst=8)]
        public byte[] Data4;       /* Array of 8 bytes. The first 2 bytes contain the third group of 4 hexadecimal digits. The remaining 6 bytes contain the final 12 hexadecimal digits. */
    }

    /*
    [STRUCTURE] 
    [
        [DESCRIPTION]   

        [REMARKS]

        [PLATFORMS]
        Win32, Win64, Linux, Linux64, Macintosh, Xbox360, PlayStation Portable, PlayStation 3, Wii, iPhone

        [SEE_ALSO]      
    ]
    */
    [StructLayout(LayoutKind.Sequential)]
    public struct ASYNCREADINFO
    {
        public IntPtr   handle;         /* [r] The file handle that was filled out in the open callback. */
        public uint     offset;         /* [r] Seek position, make sure you read from this file offset. */
        public uint     sizebytes;      /* [r] how many bytes requested for read. */
        public int      priority;       /* [r] 0 = low importance.  100 = extremely important (ie 'must read now or stuttering may occur') */

        public IntPtr   buffer;         /* [w] Buffer to read file data into. */
        public uint     bytesread;      /* [w] Fill this in before setting result code to tell FMOD how many bytes were read. */
        public RESULT   result;         /* [r/w] Result code, FMOD_OK tells the system it is ready to consume the data.  Set this last!  Default value = FMOD_ERR_NOTREADY. */

        public IntPtr   userdata;       /* [r] User data pointer. */
    }

    /*
    [ENUM]
    [
        [DESCRIPTION]   
        error codes.  Returned from every function.

        [REMARKS]

        [PLATFORMS]
        Win32, Win64, Linux, Linux64, Macintosh, Xbox360, PlayStation Portable, PlayStation 3, Wii

        [SEE_ALSO]      
    ]
    */
    public enum RESULT :int
    {
        OK,                        /* No errors. */
        ERR_ALREADYLOCKED,         /* Tried to call lock a second time before unlock was called. */
        ERR_BADCOMMAND,            /* Tried to call a function on a data type that does not allow this type of functionality (ie calling Sound.lock on a streaming sound). */
        ERR_CDDA_DRIVERS,          /* Neither NTSCSI nor ASPI could be initialised. */
        ERR_CDDA_INIT,             /* An error occurred while initialising the CDDA subsystem. */
        ERR_CDDA_INVALID_DEVICE,   /* Couldn't find the specified device. */
        ERR_CDDA_NOAUDIO,          /* No audio tracks on the specified disc. */
        ERR_CDDA_NODEVICES,        /* No CD/DVD devices were found. */ 
        ERR_CDDA_NODISC,           /* No disc present in the specified drive. */
        ERR_CDDA_READ,             /* A CDDA read error occurred. */
        ERR_CHANNEL_ALLOC,         /* Error trying to allocate a channel. */
        ERR_CHANNEL_STOLEN,        /* The specified channel has been reused to play another sound. */
        ERR_COM,                   /* A Win32 COM related error occured. COM failed to initialize or a QueryInterface failed meaning a Windows codec or driver was not installed properly. */
        ERR_DMA,                   /* DMA Failure.  See debug output for more information. */
        ERR_DSP_CONNECTION,        /* DSP connection error.  Connection possibly caused a cyclic dependancy. */
        ERR_DSP_FORMAT,            /* DSP Format error.  A DSP unit may have attempted to connect to this network with the wrong format. */
        ERR_DSP_NOTFOUND,          /* DSP connection error.  Couldn't find the DSP unit specified. */
        ERR_DSP_RUNNING,           /* DSP error.  Cannot perform this operation while the network is in the middle of running.  this will most likely happen if a connection or disconnection is attempted in a DSP callback. */
        ERR_DSP_TOOMANYCONNECTIONS,/* DSP connection error.  The unit being connected to or disconnected should only have 1 input or output. */
        ERR_FILE_BAD,              /* Error loading file. */
        ERR_FILE_COULDNOTSEEK,     /* Couldn't perform seek operation.  this is a limitation of the medium (ie netstreams) or the file format. */
        ERR_FILE_DISKEJECTED,      /* Media was ejected while reading. */
        ERR_FILE_EOF,              /* End of file unexpectedly reached while trying to read essential data (truncated data?). */
        ERR_FILE_NOTFOUND,         /* File not found. */
        ERR_FILE_UNWANTED,         /* Unwanted file access occured. */
        ERR_FORMAT,                /* Unsupported file or audio format. */
        ERR_HTTP,                  /* A HTTP error occurred. this is a catch-all for HTTP errors not listed elsewhere. */
        ERR_HTTP_ACCESS,           /* The specified resource requires authentication or is forbidden. */
        ERR_HTTP_PROXY_AUTH,       /* Proxy authentication is required to access the specified resource. */
        ERR_HTTP_SERVER_ERROR,     /* A HTTP server error occurred. */
        ERR_HTTP_TIMEOUT,          /* The HTTP request timed out. */
        ERR_INITIALIZATION,        /* FMOD was not initialized correctly to support this function. */
        ERR_INITIALIZED,           /* Cannot call this command after System.init. */
        ERR_INTERNAL,              /* An error occured that wasn't supposed to.  Contact support. */
        ERR_INVALID_ADDRESS,       /* On Xbox 360, this memory address passed to FMOD must be physical, (ie allocated with XPhysicalAlloc.) */
        ERR_INVALID_FLOAT,         /* Value passed in was a NaN, Inf or denormalized float. */
        ERR_INVALID_HANDLE,        /* An invalid object handle was used. */
        ERR_INVALID_PARAM,         /* An invalid parameter was passed to this function. */
        ERR_INVALID_POSITION,      /* An invalid seek position was passed to this function. */
        ERR_INVALID_SPEAKER,       /* An invalid speaker was passed to this function based on the current speaker mode. */
        ERR_INVALID_SYNCPOINT,     /* The syncpoint did not come from this sound handle. */
        ERR_INVALID_VECTOR,        /* The vectors passed in are not unit length, or perpendicular. */
        ERR_MAXAUDIBLE,            /* Reached maximum audible playback count for this sound's soundgroup. */
        ERR_MEMORY,                /* Not enough memory or resources. */
        ERR_MEMORY_CANTPOINT,      /* Can't use FMOD_OPENMEMORY_POINT on non PCM source data, or non mp3/xma/adpcm data if CREATECOMPRESSEDSAMPLE was used. */
        ERR_MEMORY_SRAM,           /* Not enough memory or resources on console sound ram. */
        ERR_NEEDS2D,               /* Tried to call a command on a 3d sound when the command was meant for 2d sound. */
        ERR_NEEDS3D,               /* Tried to call a command on a 2d sound when the command was meant for 3d sound. */
        ERR_NEEDSHARDWARE,         /* Tried to use a feature that requires hardware support.  (ie trying to play a GCADPCM compressed sound in software on Wii). */
        ERR_NEEDSSOFTWARE,         /* Tried to use a feature that requires the software engine.  Software engine has either been turned off, or command was executed on a hardware channel which does not support this feature. */
        ERR_NET_CONNECT,           /* Couldn't connect to the specified host. */
        ERR_NET_SOCKET_ERROR,      /* A socket error occurred.  this is a catch-all for socket-related errors not listed elsewhere. */
        ERR_NET_URL,               /* The specified URL couldn't be resolved. */
        ERR_NET_WOULD_BLOCK,       /* Operation on a non-blocking socket could not complete immediately. */
        ERR_NOTREADY,              /* Operation could not be performed because specified sound is not ready. */
        ERR_OUTPUT_ALLOCATED,      /* Error initializing output device, but more specifically, the output device is already in use and cannot be reused. */
        ERR_OUTPUT_CREATEBUFFER,   /* Error creating hardware sound buffer. */
        ERR_OUTPUT_DRIVERCALL,     /* A call to a standard soundcard driver failed, which could possibly mean a bug in the driver or resources were missing or exhausted. */
        ERR_OUTPUT_ENUMERATION,    /* Error enumerating the available driver list. List may be inconsistent due to a recent device addition or removal. */
        ERR_OUTPUT_FORMAT,         /* Soundcard does not support the minimum features needed for this soundsystem (16bit stereo output). */
        ERR_OUTPUT_INIT,           /* Error initializing output device. */
        ERR_OUTPUT_NOHARDWARE,     /* FMOD_HARDWARE was specified but the sound card does not have the resources nescessary to play it. */
        ERR_OUTPUT_NOSOFTWARE,     /* Attempted to create a software sound but no software channels were specified in System.init. */
        ERR_PAN,                   /* Panning only works with mono or stereo sound sources. */
        ERR_PLUGIN,                /* An unspecified error has been returned from a 3rd party plugin. */
        ERR_PLUGIN_INSTANCES,      /* The number of allowed instances of a plugin has been exceeded */
        ERR_PLUGIN_MISSING,        /* A requested output, dsp unit type or codec was not available. */
        ERR_PLUGIN_RESOURCE,       /* A resource that the plugin requires cannot be found. (ie the DLS file for MIDI playback) */
        ERR_PRELOADED,             /* The specified sound is still in use by the event system, call EventSystem.unloadFSB before trying to release it. */
        ERR_PROGRAMMERSOUND,       /* The specified sound is still in use by the event system, wait for the event which is using it finish with it. */
        ERR_RECORD,                /* An error occured trying to initialize the recording device. */
        ERR_REVERB_INSTANCE,       /* Specified Instance in REVERB_PROPERTIES couldn't be set. Most likely because another application has locked the EAX4 FX slot. */
        ERR_SUBSOUND_ALLOCATED,    /* this subsound is already being used by another sound, you cannot have more than one parent to a sound.  Null out the other parent's entry first. */
        ERR_SUBSOUND_CANTMOVE,     /* Shared subsounds cannot be replaced or moved from their parent stream, such as when the parent stream is an FSB file. */
        ERR_SUBSOUND_MODE,         /* The subsound's mode bits do not match with the parent sound's mode bits.  See documentation for function that it was called with. */
        ERR_SUBSOUNDS,             /* The error occured because the sound referenced contains subsounds.  (ie you cannot play the parent sound as a static sample, only its subsounds.) */
        ERR_TAGNOTFOUND,           /* The specified tag could not be found or there are no tags. */
        ERR_TOOMANYCHANNELS,       /* The sound created exceeds the allowable input channel count.  this can be increased using the maxinputchannels parameter in System.setSoftwareFormat. */
        ERR_UNIMPLEMENTED,         /* Something in FMOD hasn't been implemented when it should be! contact support! */
        ERR_UNINITIALIZED,         /* this command failed because System.init or System.setDriver was not called. */
        ERR_UNSUPPORTED,           /* A command issued was not supported by this object.  Possibly a plugin without certain callbacks specified. */
        ERR_UPDATE,                /* An error caused by System.update occured. */
        ERR_VERSION,               /* The version number of this file format is not supported. */

        ERR_EVENT_FAILED,          /* An Event failed to be retrieved, most likely due to 'just fail' being specified as the max playbacks behavior. */
        ERR_EVENT_INFOONLY,        /* Can't execute this command on an EVENT_INFOONLY event. */
        ERR_EVENT_INTERNAL,        /* An error occured that wasn't supposed to.  See debug log for reason. */
        ERR_EVENT_MAXSTREAMS,      /* Event failed because 'Max streams' was hit when FMOD_INIT_FAIL_ON_MAXSTREAMS was specified. */
        ERR_EVENT_MISMATCH,        /* FSB mis-matches the FEV it was compiled with. */
        ERR_EVENT_NAMECONFLICT,    /* A category with the same name already exists. */
        ERR_EVENT_NOTFOUND,        /* The requested event, event group, event category or event property could not be found. */
        ERR_EVENT_NEEDSSIMPLE,     /* Tried to call a function on a complex event that's only supported by simple events. */
        ERR_EVENT_GUIDCONFLICT,    /* An event with the same GUID already exists. */
        ERR_EVENT_ALREADY_LOADED,  /* The specified project has already been loaded. Having multiple copies of the same project loaded simultaneously is forbidden. */

        ERR_MUSIC_UNINITIALIZED,   /* Music system is not initialized probably because no music data is loaded. */
        ERR_MUSIC_NOTFOUND,        /* The requested music entity could not be found. */
        ERR_MUSIC_NOCALLBACK,      /* The music callback is required, but it has not been set. */
    }



    /*
    [ENUM]
    [
        [DESCRIPTION]   
        These output types are used with System.setOutput/System.getOutput, to choose which output method to use.
  
        [REMARKS]
        To drive the output synchronously, and to disable FMOD's timing thread, use the FMOD_INIT_NONREALTIME flag.
        
        To pass information to the driver when initializing fmod use the extradriverdata parameter for the following reasons.
        <li>FMOD_OUTPUTTYPE_WAVWRITER - extradriverdata is a pointer to a char * filename that the wav writer will output to.
        <li>FMOD_OUTPUTTYPE_WAVWRITER_NRT - extradriverdata is a pointer to a char * filename that the wav writer will output to.
        <li>FMOD_OUTPUTTYPE_DSOUND - extradriverdata is a pointer to a HWND so that FMOD can set the focus on the audio for a particular window.
        <li>FMOD_OUTPUTTYPE_GC - extradriverdata is a pointer to a FMOD_ARAMBLOCK_INFO struct. this can be found in fmodgc.h.
        Currently these are the only FMOD drivers that take extra information.  Other unknown plugins may have different requirements.

        [PLATFORMS]
        Win32, Win64, Linux, Linux64, Macintosh, Xbox360, PlayStation Portable, PlayStation 3, Wii

        [SEE_ALSO]      
        System.setOutput
        System.getOutput
        System.setSoftwareFormat
        System.getSoftwareFormat
        System.init
    ]
    */
    public enum OUTPUTTYPE :int
    {
        AUTODETECT,      /* Picks the best output mode for the platform.  this is the default. */

        UNKNOWN,         /* All             - 3rd party plugin, unknown.  this is for use with System.getOutput only. */
        NOSOUND,         /* All             - All calls in this mode succeed but make no sound. */
        WAVWRITER,       /* All             - Writes output to fmodoutput.wav by default.  Use the 'extradriverdata' parameter in System.init, by simply passing the filename as a string, to set the wav filename. */
        NOSOUND_NRT,     /* All             - Non-realtime version of _NOSOUND.  User can drive mixer with System.update at whatever rate they want. */
        WAVWRITER_NRT,   /* All             - Non-realtime version of _WAVWRITER.  User can drive mixer with System.update at whatever rate they want. */

        DSOUND,          /* Win32/Win64     - DirectSound output.                       (Default on Windows XP and below) */
        WINMM,           /* Win32/Win64     - Windows Multimedia output. */
        WASAPI,          /* Win32           - Windows Audio Session API.                (Default on Windows Vista and above) */
        ASIO,            /* Win32           - Low latency ASIO 2.0 driver. */
        OSS,             /* Linux/Linux64   - Open Sound System output.                 (Default on Linux, third preference) */
        ALSA,            /* Linux/Linux64   - Advanced Linux Sound Architecture output. (Default on Linux, second preference if available) */
        ESD,             /* Linux/Linux64   - Enlightment Sound Daemon output. */
        PULSEAUDIO,      /* Linux/Linux64   - PulseAudio output.                        (Default on Linux, first preference if available) */
        COREAUDIO,       /* Mac             - Macintosh CoreAudio output.               (Default on Mac) */
        XBOX360,         /* Xbox 360        - Native Xbox360 output.                    (Default on Xbox 360) */
        PSP,             /* PSP             - Native PSP output.                        (Default on PSP) */
        PS3,             /* PS3             - Native PS3 output.                        (Default on PS3) */
        NGP,             /* NGP             - Native NGP output.                        (Default on NGP) */
        WII,			 /* Wii			    - Native Wii output.                        (Default on Wii) */
        _3DS,            /* 3DS             - Native 3DS output                         (Default on 3DS) */
        AUDIOTRACK,      /* Android         - Java Audio Track output.                  (Default on Android 2.2 and below) */
        OPENSL,          /* Android         - OpenSL ES output.                         (Default on Android 2.3 and above) */
        NACL,            /* Native Client   - Native Client output.                     (Default on Native Client) */
        WIIU,            /* Wii U           - Native Wii U output.                      (Default on Wii U) */

        MAX            /* Maximum number of output types supported. */
    }


    /*
    [ENUM] 
    [
        [DESCRIPTION]   

        [REMARKS]

        [PLATFORMS]
        Win32, Win64, Linux, Linux64, Macintosh, Xbox360, PlayStation Portable, PlayStation 3, Wii

        [SEE_ALSO]
    ]
    */
    public enum CAPS
    {
        NONE                   = 0x00000000,    /* Device has no special capabilities. */
        HARDWARE               = 0x00000001,    /* Device supports hardware mixing. */
        HARDWARE_EMULATED      = 0x00000002,    /* User has device set to 'Hardware acceleration = off' in control panel, and now extra 200ms latency is incurred. */
        OUTPUT_MULTICHANNEL    = 0x00000004,    /* Device can do multichannel output, ie greater than 2 channels. */
        OUTPUT_FORMAT_PCM8     = 0x00000008,    /* Device can output to 8bit integer PCM. */
        OUTPUT_FORMAT_PCM16    = 0x00000010,    /* Device can output to 16bit integer PCM. */
        OUTPUT_FORMAT_PCM24    = 0x00000020,    /* Device can output to 24bit integer PCM. */
        OUTPUT_FORMAT_PCM32    = 0x00000040,    /* Device can output to 32bit integer PCM. */
        OUTPUT_FORMAT_PCMFLOAT = 0x00000080,    /* Device can output to 32bit floating point PCM. */
        REVERB_LIMITED         = 0x00002000     /* Device supports some form of limited hardware reverb, maybe parameterless and only selectable by environment. */
    }

    /*
    [DEFINE] 
    [
        [NAME]
        FMOD_DEBUGLEVEL

        [DESCRIPTION]   
        Bit fields to use with FMOD.Debug_SetLevel / FMOD.Debug_GetLevel to control the level of tty debug output with logging versions of FMOD (fmodL).

        [REMARKS]

        [PLATFORMS]
        Win32, Win64, Linux, Linux64, Macintosh, Xbox360, PlayStation Portable, PlayStation 3, Wii

        [SEE_ALSO]
        Debug_SetLevel 
        Debug_GetLevel
    ]
    */
    public enum DEBUGLEVEL
    {
        LEVEL_NONE           = 0x00000000,
        LEVEL_LOG            = 0x00000001,
        LEVEL_ERROR          = 0x00000002,
        LEVEL_WARNING        = 0x00000004,
        LEVEL_HINT           = 0x00000008,
        LEVEL_ALL            = 0x000000FF,   
        TYPE_MEMORY          = 0x00000100,
        TYPE_THREAD          = 0x00000200,
        TYPE_FILE            = 0x00000400,
        TYPE_NET             = 0x00000800,
        TYPE_EVENT           = 0x00001000,
        TYPE_ALL             = 0x0000FFFF,                     
        DISPLAY_TIMESTAMPS   = 0x01000000,
        DISPLAY_LINENUMBERS  = 0x02000000,
        DISPLAY_COMPRESS     = 0x04000000,
        DISPLAY_THREAD       = 0x08000000,
        DISPLAY_ALL          = 0x0F000000,   
        ALL                  = unchecked((int)0xffffffff)
    }


    /*
    [DEFINE] 
    [
        [NAME]
        FMOD_MEMORY_TYPE

        [DESCRIPTION]   
        Bit fields for memory allocation type being passed into FMOD memory callbacks.

        [REMARKS]

        [PLATFORMS]
        Win32, Win64, Linux, Linux64, Macintosh, Xbox360, PlayStation Portable, PlayStation 3, Wii

        [SEE_ALSO]
        FMOD_MEMORY_ALLOCCALLBACK
        FMOD_MEMORY_REALLOCCALLBACK
        FMOD_MEMORY_FREECALLBACK
        Memory_Initialize
    
    ]
    */
    public enum MEMORY_TYPE
    {
        NORMAL           = 0x00000000,       /* Standard memory. */
        STREAM_FILE      = 0x00000001,       /* Stream file buffer, size controllable with System.setStreamBufferSize. */
        STREAM_DECODE    = 0x00000002,       /* Stream decode buffer, size controllable with FMOD_CREATESOUNDEXINFO.decodebuffersize. */
        SAMPLEDATA       = 0x00000004,       /* Sample data buffer.  Raw audio data, usually PCM/MPEG/ADPCM/XMA data. */
        DSP_OUTPUTBUFFER = 0x00000008,       /* DSP memory block allocated when more than 1 output exists on a DSP node. */
        XBOX360_PHYSICAL = 0x00100000,       /* Requires XPhysicalAlloc / XPhysicalFree. */
        PERSISTENT       = 0x00200000,       /* Persistent memory. Memory will be freed when System.release is called. */
        SECONDARY        = 0x00400000        /* Secondary memory. Allocation should be in secondary memory. For example RSX on the PS3. */
    }


    /*
    [ENUM]
    [
        [DESCRIPTION]   
        These are speaker types defined for use with the System.setSpeakerMode or System.getSpeakerMode command.

        [REMARKS]
        These are important notes on speaker modes in regards to sounds created with FMOD_SOFTWARE.<br>
        Note below the phrase 'sound channels' is used.  These are the subchannels inside a sound, they are not related and 
        have nothing to do with the FMOD class "Channel".<br>
        For example a mono sound has 1 sound channel, a stereo sound has 2 sound channels, and an AC3 or 6 channel wav file have 6 "sound channels".<br>
        <br>
        FMOD_SPEAKERMODE_RAW<br>
        ---------------------<br>
        this mode is for output devices that are not specifically mono/stereo/quad/surround/5.1 or 7.1, but are multichannel.<br>
        Sound channels map to speakers sequentially, so a mono sound maps to output speaker 0, stereo sound maps to output speaker 0 & 1.<br>
        The user assumes knowledge of the speaker order.  FMOD_SPEAKER enumerations may not apply, so raw channel indicies should be used.<br>
        Multichannel sounds map input channels to output channels 1:1. <br>
        Channel.setPan and Channel.setSpeakerMix do not work.<br>
        Speaker levels must be manually set with Channel.setSpeakerLevels.<br>
        <br>
        FMOD_SPEAKERMODE_MONO<br>
        ---------------------<br>
        this mode is for a 1 speaker arrangement.<br>
        Panning does not work in this speaker mode.<br>
        Mono, stereo and multichannel sounds have each sound channel played on the one speaker unity.<br>
        Mix behaviour for multichannel sounds can be set with Channel.setSpeakerLevels.<br>
        Channel.setSpeakerMix does not work.<br>
        <br>
        FMOD_SPEAKERMODE_STEREO<br>
        -----------------------<br>
        this mode is for 2 speaker arrangements that have a left and right speaker.<br>
        <li>Mono sounds default to an even distribution between left and right.  They can be panned with Channel.setPan.<br>
        <li>Stereo sounds default to the middle, or full left in the left speaker and full right in the right speaker.  
        <li>They can be cross faded with Channel.setPan.<br>
        <li>Multichannel sounds have each sound channel played on each speaker at unity.<br>
        <li>Mix behaviour for multichannel sounds can be set with Channel.setSpeakerLevels.<br>
        <li>Channel.setSpeakerMix works but only front left and right parameters are used, the rest are ignored.<br>
        <br>
        FMOD_SPEAKERMODE_QUAD<br>
        ------------------------<br>
        this mode is for 4 speaker arrangements that have a front left, front right, rear left and a rear right speaker.<br>
        <li>Mono sounds default to an even distribution between front left and front right.  They can be panned with Channel.setPan.<br>
        <li>Stereo sounds default to the left sound channel played on the front left, and the right sound channel played on the front right.<br>
        <li>They can be cross faded with Channel.setPan.<br>
        <li>Multichannel sounds default to all of their sound channels being played on each speaker in order of input.<br>
        <li>Mix behaviour for multichannel sounds can be set with Channel.setSpeakerLevels.<br>
        <li>Channel.setSpeakerMix works but side left, side right, center and lfe are ignored.<br>
        <br>
        FMOD_SPEAKERMODE_SURROUND<br>
        ------------------------<br>
        this mode is for 4 speaker arrangements that have a front left, front right, front center and a rear center.<br>
        <li>Mono sounds default to the center speaker.  They can be panned with Channel.setPan.<br>
        <li>Stereo sounds default to the left sound channel played on the front left, and the right sound channel played on the front right.  
        <li>They can be cross faded with Channel.setPan.<br>
        <li>Multichannel sounds default to all of their sound channels being played on each speaker in order of input.<br>
        <li>Mix behaviour for multichannel sounds can be set with Channel.setSpeakerLevels.<br>
        <li>Channel.setSpeakerMix works but side left, side right and lfe are ignored, and rear left / rear right are averaged into the rear speaker.<br>
        <br>
        FMOD_SPEAKERMODE_5POINT1<br>
        ------------------------<br>
        this mode is for 5.1 speaker arrangements that have a left/right/center/rear left/rear right and a subwoofer speaker.<br>
        <li>Mono sounds default to the center speaker.  They can be panned with Channel.setPan.<br>
        <li>Stereo sounds default to the left sound channel played on the front left, and the right sound channel played on the front right.  
        <li>They can be cross faded with Channel.setPan.<br>
        <li>Multichannel sounds default to all of their sound channels being played on each speaker in order of input.  
        <li>Mix behaviour for multichannel sounds can be set with Channel.setSpeakerLevels.<br>
        <li>Channel.setSpeakerMix works but side left / side right are ignored.<br>
        <br>
        FMOD_SPEAKERMODE_7POINT1<br>
        ------------------------<br>
        this mode is for 7.1 speaker arrangements that have a left/right/center/rear left/rear right/side left/side right 
        and a subwoofer speaker.<br>
        <li>Mono sounds default to the center speaker.  They can be panned with Channel.setPan.<br>
        <li>Stereo sounds default to the left sound channel played on the front left, and the right sound channel played on the front right.  
        <li>They can be cross faded with Channel.setPan.<br>
        <li>Multichannel sounds default to all of their sound channels being played on each speaker in order of input.  
        <li>Mix behaviour for multichannel sounds can be set with Channel.setSpeakerLevels.<br>
        <li>Channel.setSpeakerMix works and every parameter is used to set the balance of a sound in any speaker.<br>
        <br>
        FMOD_SPEAKERMODE_SRS5_1_MATRIX<br>
        ------------------------------------------------------<br>
		this mode is for mono, stereo, 5.1 and 7.1 speaker arrangements, as it is backwards and forwards compatible with 
		stereo, but to get a surround effect a SRS 5.1, Prologic or Prologic 2 hardware decoder / amplifier is needed.<br>
		Pan behavior is the same as FMOD_SPEAKERMODE_5POINT1.<br>
		<br>
		If this function is called the numoutputchannels setting in System.setSoftwareFormat is overwritten.<br>
		<br>
		Output rate must be 44100, 48000 or 96000 for this to work otherwise FMOD_ERR_OUTPUT_INIT will be returned.<br>
    
        FMOD_SPEAKERMODE_MYEARS<br>
        ------------------------------------------------------<br>
        this mode is for headphones.  this will attempt to load a MyEars profile (see myears.net.au) and use it to generate
        surround sound on headphones using a personalized HRTF algorithm, for realistic 3d sound.<br>
        Pan behavior is the same as FMOD_SPEAKERMODE_7POINT1.<br>
        MyEars speaker mode will automatically be set if the speakermode is FMOD_SPEAKERMODE_STEREO and the MyEars profile exists.<br>
        If this mode is set explicitly, FMOD_INIT_DISABLE_MYEARS_AUTODETECT has no effect.<br>
        If this mode is set explicitly and the MyEars profile does not exist, FMOD_ERR_OUTPUT_DRIVERCALL will be returned.

        [PLATFORMS]
        Win32, Win64, Linux, Linux64, Macintosh, Xbox360, PlayStation Portable, PlayStation 3, Wii

        [SEE_ALSO]
        System.setSpeakerMode
        System.getSpeakerMode
        System.getDriverCaps
        Channel.setSpeakerLevels
    ]
    */
    public enum SPEAKERMODE :int
    {
        RAW,              /* There is no specific speakermode.  Sound channels are mapped in order of input to output.  See remarks for more information. */
        MONO,             /* The speakers are monaural. */
        STEREO,           /* The speakers are stereo (DEFAULT). */
        QUAD,             /* 4 speaker setup.  this includes front left, front right, rear left, rear right.  */
        SURROUND,         /* 4 speaker setup.  this includes front left, front right, center, rear center (rear left/rear right are averaged). */
        _5POINT1,         /* 5.1 speaker setup.  this includes front left, front right, center, rear left, rear right and a subwoofer. */
        _7POINT1,         /* 7.1 speaker setup.  this includes front left, front right, center, rear left, rear right, side left, side right and a subwoofer. */

        SRS5_1_MATRIX,    /* Stereo compatible output, embedded with surround information. SRS 5.1/Prologic/Prologic2 decoders will split the signal into a 5.1 speaker set-up or SRS virtual surround will decode into a 2-speaker/headphone setup.  See remarks about limitations. */
        MYEARS,           /* Stereo output, but data is encoded using personalized HRTF algorithms.  See myears.net.au */

        MAX,              /* Maximum number of speaker modes supported. */
    }


    /*
    [ENUM]
    [
        [DESCRIPTION]   
        These are speaker types defined for use with the Channel.setSpeakerLevels command.
        It can also be used for speaker placement in the System.setSpeakerPosition command.

        [REMARKS]
        If you are using FMOD_SPEAKERMODE_RAW and speaker assignments are meaningless, just cast a raw integer value to this type.<br>
        For example (FMOD_SPEAKER)7 would use the 7th speaker (also the same as FMOD_SPEAKER_SIDE_RIGHT).<br>
        Values higher than this can be used if an output system has more than 8 speaker types / output channels.  15 is the current maximum.

        [PLATFORMS]
        Win32, Win64, Linux, Linux64, Macintosh, Xbox360, PlayStation Portable, PlayStation 3, Wii

        [SEE_ALSO]
        FMOD_SPEAKERMODE
        Channel.setSpeakerLevels
        Channel.getSpeakerLevels
        System.setSpeakerPosition
        System.getSpeakerPosition
    ]
    */
    public enum SPEAKER :int
    {
        FRONT_LEFT,
        FRONT_RIGHT,
        FRONT_CENTER,
        LOW_FREQUENCY,
        BACK_LEFT,
        BACK_RIGHT,
        SIDE_LEFT,
        SIDE_RIGHT,
    
        MAX,                               /* Maximum number of speaker types supported. */
        MONO        = FRONT_LEFT,    /* For use with FMOD_SPEAKERMODE_MONO and Channel.SetSpeakerLevels.  Mapped to same value as FMOD_SPEAKER_FRONT_LEFT. */
        NULL        = MAX,           /* A non speaker.  Use this to send. */
        SBL         = SIDE_LEFT,     /* For use with FMOD_SPEAKERMODE_7POINT1 on PS3 where the extra speakers are surround back inside of side speakers. */
        SBR         = SIDE_RIGHT,    /* For use with FMOD_SPEAKERMODE_7POINT1 on PS3 where the extra speakers are surround back inside of side speakers. */
    }


    /*
    [ENUM]
    [
        [DESCRIPTION]   
        These are plugin types defined for use with the System.getNumPlugins / System_GetNumPlugins, 
        System.getPluginInfo / System_GetPluginInfo and System.unloadPlugin / System_UnloadPlugin functions.

        [REMARKS]

        [PLATFORMS]
        Win32, Win64, Linux, Linux64, Macintosh, Xbox360, PlayStation Portable, PlayStation 3, Wii

        [SEE_ALSO]
        System.getNumPlugins
        System.getPluginInfo
        System.unloadPlugin
    ]
    */
    public enum PLUGINTYPE :int
    {
        OUTPUT,     /* The plugin type is an output module.  FMOD mixed audio will play through one of these devices */
        CODEC,      /* The plugin type is a file format codec.  FMOD will use these codecs to load file formats for playback. */
        DSP         /* The plugin type is a DSP unit.  FMOD will use these plugins as part of its DSP network to apply effects to output or generate sound in realtime. */
    }


    /*
    [ENUM] 
    [
        [DESCRIPTION]   
        Initialization flags.  Use them with System.init in the flags parameter to change various behaviour.  

        [REMARKS]

        [PLATFORMS]
        Win32, Win64, Linux, Linux64, Macintosh, Xbox360, PlayStation Portable, PlayStation 3, Wii

        [SEE_ALSO]
        System.init
    ]
    */
    public enum INITFLAGS :int
    {
        NORMAL                    = 0x00000000,   /* All platforms - Initialize normally */
        STREAM_FROM_UPDATE        = 0x00000001,   /* All platforms - No stream thread is created internally.  Streams are driven from System.update.  Mainly used with non-realtime outputs. */
        _3D_RIGHTHANDED           = 0x00000002,   /* All platforms - FMOD will treat +X as left, +Y as up and +Z as forwards. */
        SOFTWARE_DISABLE          = 0x00000004,   /* All platforms - Disable software mixer to save memory.  Anything created with FMOD_SOFTWARE will fail and DSP will not work. */
        OCCLUSION_LOWPASS         = 0x00000008,   /* All platforms - All FMOD_SOFTWARE (and FMOD_HARDWARE on 3DS and NGP) with FMOD_3D based voices will add a software lowpass filter effect into the DSP chain which is automatically used when Channel.set3DOcclusion is used or the geometry API. */
        HRTF_LOWPASS              = 0x00000010,   /* All platforms - All FMOD_SOFTWARE (and FMOD_HARDWARE on 3DS and NGP) with FMOD_3D based voices will add a software lowpass filter effect into the DSP chain which causes sounds to sound duller when the sound goes behind the listener.  Use System.setAdvancedSettings to adjust cutoff frequency. */
        DISTANCE_FILTERING        = 0x00000200,   /* All platforms - All FMOD_SOFTWARE with FMOD_3D based voices will add a software lowpass and highpass filter effect into the DSP chain which will act as a distance-automated bandpass filter. Use System.setAdvancedSettings to adjust the center frequency. */
        SOFTWARE_REVERB_LOWMEM    = 0x00000040,   /* All platforms - SFX reverb is run using 22/24khz delay buffers, halving the memory required. */
        ENABLE_PROFILE            = 0x00000020,   /* All platforms - Enable TCP/IP based host which allows "DSPNet Listener.exe" to connect to it, and view the DSP dataflow network graph in real-time. */
        VOL0_BECOMES_VIRTUAL      = 0x00000080,   /* All platforms - Any sounds that are 0 volume will go virtual and not be processed except for having their positions updated virtually.  Use System.setAdvancedSettings to adjust what volume besides zero to switch to virtual at. */
        WASAPI_EXCLUSIVE          = 0x00000100,   /* Win32 Vista only - for WASAPI output - Enable exclusive access to hardware, lower latency at the expense of excluding other applications from accessing the audio hardware. */
        DISABLEDOLBY              = 0x00100000,   /* Wii / 3DS - Disable Dolby Pro Logic surround. Speakermode will be set to STEREO even if user has selected surround in the system settings. */
        WII_DISABLEDOLBY          = 0x00100000,   /* Wii only - Disable Dolby Pro Logic surround. Speakermode will be set to STEREO even if user has selected surround in the Wii system settings. */
        _360_MUSICMUTENOTPAUSE    = 0x00200000,   /* Xbox 360 only - The "music" channelgroup which by default pauses when custom 360 dashboard music is played, can be changed to mute (therefore continues playing) instead of pausing, by using this flag. */
        SYNCMIXERWITHUPDATE       = 0x00400000,   /* Win32/Wii/PS3/Xbox 360 - FMOD Mixer thread is woken up to do a mix when System.update is called rather than waking periodically on its own timer. */
        DTS_NEURALSURROUND        = 0x02000000,   /* Win32/Mac/Linux - Use DTS Neural surround downmixing from 7.1 if speakermode set to FMOD_SPEAKERMODE_STEREO or FMOD_SPEAKERMODE_5POINT1.  Internal DSP structure will be set to 7.1. */
        GEOMETRY_USECLOSEST       = 0x04000000,   /* All platforms - With the geometry engine, only process the closest polygon rather than accumulating all polygons the sound to listener line intersects. */
        DISABLE_MYEARS_AUTODETECT = 0x08000000    /* Win32 - Disables automatic setting of FMOD_SPEAKERMODE_STEREO to FMOD_SPEAKERMODE_MYEARS if the MyEars profile exists on the PC.  MyEars is HRTF 7.1 downmixing through headphones. */
    }


    /*
    [ENUM]
    [
        [DESCRIPTION]   
        These definitions describe the type of song being played.

        [REMARKS]

        [PLATFORMS]
        Win32, Win64, Linux, Linux64, Macintosh, Xbox360, PlayStation Portable, PlayStation 3, Wii

        [SEE_ALSO]      
        Sound.getFormat
    ]
    */
    public enum SOUND_TYPE
    {
        UNKNOWN,         /* 3rd party / unknown plugin format. */
        AIFF,            /* AIFF. */
        ASF,             /* Microsoft Advanced Systems Format (ie WMA/ASF/WMV). */
        AT3,             /* Sony ATRAC 3 format */
        CDDA,            /* Digital CD audio. */
        DLS,             /* Sound font / downloadable sound bank. */
        FLAC,            /* FLAC lossless codec. */
        FSB,             /* FMOD Sample Bank. */
        GCADPCM,         /* GameCube ADPCM */
        IT,              /* Impulse Tracker. */
        MIDI,            /* MIDI. */
        MOD,             /* Protracker / Fasttracker MOD. */
        MPEG,            /* MP2/MP3 MPEG. */
        OGGVORBIS,       /* Ogg vorbis. */
        PLAYLIST,        /* Information only from ASX/PLS/M3U/WAX playlists */
        RAW,             /* Raw PCM data. */
        S3M,             /* ScreamTracker 3. */
        SF2,             /* Sound font 2 format. */
        USER,            /* User created sound. */
        WAV,             /* Microsoft WAV. */
        XM,              /* FastTracker 2 XM. */
        XMA,             /* Xbox360 XMA */
        VAG,             /* PlayStation Portable adpcm VAG format. */        
        AUDIOQUEUE,      /* iPhone hardware decoder, supports AAC, ALAC and MP3. */
        XWMA,            /* Xbox360 XWMA */
        BCWAV,           /* 3DS BCWAV container format for DSP ADPCM and PCM */
        AT9,             /* NGP ATRAC 9 format */
        VORBIS,          /* Raw vorbis */
    }


    /*
    [ENUM]
    [
        [DESCRIPTION]   
        These definitions describe the native format of the hardware or software buffer that will be used.

        [REMARKS]
        this is the format the native hardware or software buffer will be or is created in.

        [PLATFORMS]
        Win32, Win64, Linux, Linux64, Macintosh, Xbox360, PlayStation Portable, PlayStation 3, Wii

        [SEE_ALSO]
        System.createSoundEx
        Sound.getFormat
    ]
    */
    public enum SOUND_FORMAT :int
    {
        NONE,     /* Unitialized / unknown */
        PCM8,     /* 8bit integer PCM data */
        PCM16,    /* 16bit integer PCM data  */
        PCM24,    /* 24bit integer PCM data  */
        PCM32,    /* 32bit integer PCM data  */
        PCMFLOAT, /* 32bit floating point PCM data  */
        GCADPCM,  /* Compressed GameCube DSP data */
        IMAADPCM, /* Compressed XBox ADPCM data */
        VAG,      /* Compressed PlayStation 2 ADPCM data */
        HEVAG,    /* Compressed NGP ADPCM data. */
        XMA,      /* Compressed Xbox360 data. */
        MPEG,     /* Compressed MPEG layer 2 or 3 data. */
        MAX,      /* Maximum number of sound formats supported. */ 
        CELT,     /* Compressed CELT data. */
        AT9,      /* Compressed ATRAC9 data. */
        XWMA,     /* Compressed Xbox360 xWMA data. */
        VORBIS,   /* Compressed Vorbis data. */
    }


    /*
    [DEFINE]
    [
        [NAME] 
        FMOD_MODE

        [DESCRIPTION]   
        Sound description bitfields, bitwise OR them together for loading and describing sounds.

        [REMARKS]
        By default a sound will open as a static sound that is decompressed fully into memory.<br>
        To have a sound stream instead, use FMOD_CREATESTREAM.<br>
        Some opening modes (ie FMOD_OPENUSER, FMOD_OPENMEMORY, FMOD_OPENRAW) will need extra information.<br>
        this can be provided using the FMOD_CREATESOUNDEXINFO structure.

        [PLATFORMS]
        Win32, Win64, Linux, Linux64, Macintosh, Xbox360, PlayStation Portable, PlayStation 3, Wii

        [SEE_ALSO]
        System.createSound
        System.createStream
        Sound.setMode
        Sound.getMode
        Channel.setMode
        Channel.getMode
        Sound.set3DCustomRolloff
        Channel.set3DCustomRolloff
    ]
    */

    [Flags]
    public enum MODE :uint
    {
        DEFAULT                = 0x00000000,  /* FMOD_DEFAULT is a default sound type.  Equivalent to all the defaults listed below.  FMOD_LOOP_OFF, FMOD_2D, FMOD_HARDWARE. */
        LOOP_OFF               = 0x00000001,  /* For non looping sounds. (default).  Overrides FMOD_LOOP_NORMAL / FMOD_LOOP_BIDI. */
        LOOP_NORMAL            = 0x00000002,  /* For forward looping sounds. */
        LOOP_BIDI              = 0x00000004,  /* For bidirectional looping sounds. (only works on software mixed static sounds). */
        _2D                    = 0x00000008,  /* Ignores any 3d processing. (default). */
        _3D                    = 0x00000010,  /* Makes the sound positionable in 3D.  Overrides FMOD_2D. */
        HARDWARE               = 0x00000020,  /* Attempts to make sounds use hardware acceleration. (default). */
        SOFTWARE               = 0x00000040,  /* Makes sound reside in software.  Overrides FMOD_HARDWARE.  Use this for FFT, DSP, 2D multi speaker support and other software related features. */
        CREATESTREAM           = 0x00000080,  /* Decompress at runtime, streaming from the source provided (standard stream).  Overrides FMOD_CREATESAMPLE. */
        CREATESAMPLE           = 0x00000100,  /* Decompress at loadtime, decompressing or decoding whole file into memory as the target sample format. (standard sample). */
        CREATECOMPRESSEDSAMPLE = 0x00000200,  /* Load MP2, MP3, IMAADPCM or XMA into memory and leave it compressed.  During playback the FMOD software mixer will decode it in realtime as a 'compressed sample'.  Can only be used in combination with FMOD_SOFTWARE. */
        OPENUSER               = 0x00000400,  /* Opens a user created static sample or stream. Use FMOD_CREATESOUNDEXINFO to specify format and/or read callbacks.  If a user created 'sample' is created with no read callback, the sample will be empty.  Use FMOD_Sound_Lock and FMOD_Sound_Unlock to place sound data into the sound if this is the case. */
        OPENMEMORY             = 0x00000800,  /* "name_or_data" will be interpreted as a pointer to memory instead of filename for creating sounds. */
        OPENMEMORY_POINT       = 0x10000000,  /* "name_or_data" will be interpreted as a pointer to memory instead of filename for creating sounds.  Use FMOD_CREATESOUNDEXINFO to specify length.  this differs to FMOD_OPENMEMORY in that it uses the memory as is, without duplicating the memory into its own buffers.  FMOD_SOFTWARE only.  Doesn't work with FMOD_HARDWARE, as sound hardware cannot access main ram on a lot of platforms.  Cannot be freed after open, only after Sound.release.   Will not work if the data is compressed and FMOD_CREATECOMPRESSEDSAMPLE is not used. */
        OPENRAW                = 0x00001000,  /* Will ignore file format and treat as raw pcm.  User may need to declare if data is FMOD_SIGNED or FMOD_UNSIGNED */
        OPENONLY               = 0x00002000,  /* Just open the file, dont prebuffer or read.  Good for fast opens for info, or when sound.readData is to be used. */
        ACCURATETIME           = 0x00004000,  /* For FMOD_CreateSound - for accurate FMOD_Sound_GetLength / FMOD_Channel_SetPosition on VBR MP3, AAC and MOD/S3M/XM/IT/MIDI files.  Scans file first, so takes longer to open. FMOD_OPENONLY does not affect this. */
        MPEGSEARCH             = 0x00008000,  /* For corrupted / bad MP3 files.  this will search all the way through the file until it hits a valid MPEG header.  Normally only searches for 4k. */
        NONBLOCKING            = 0x00010000,  /* For opening sounds and getting streamed subsounds (seeking) asyncronously.  Use Sound.getOpenState to poll the state of the sound as it opens or retrieves the subsound in the background. */
        UNIQUE                 = 0x00020000,  /* Unique sound, can only be played one at a time */
        _3D_HEADRELATIVE       = 0x00040000,  /* Make the sound's position, velocity and orientation relative to the listener. */
        _3D_WORLDRELATIVE      = 0x00080000,  /* Make the sound's position, velocity and orientation absolute (relative to the world). (DEFAULT) */
        _3D_INVERSEROLLOFF     = 0x00100000,  /* this sound will follow the inverse rolloff model where mindistance = full volume, maxdistance = where sound stops attenuating, and rolloff is fixed according to the global rolloff factor.  (DEFAULT) */
        _3D_LINEARSQUAREROLLOFF= 0x00400000,  /* this sound will follow a linear-square rolloff model where mindistance = full volume, maxdistance = silence.  Rolloffscale is ignored. */
        _3D_LOGROLLOFF         = 0x00100000,  /* this sound will follow the standard logarithmic rolloff model where mindistance = full volume, maxdistance = where sound stops attenuating, and rolloff is fixed according to the global rolloff factor.  (default) */
        _3D_LINEARROLLOFF      = 0x00200000,  /* this sound will follow a linear rolloff model where mindistance = full volume, maxdistance = silence.  */
        _3D_CUSTOMROLLOFF      = 0x04000000,  /* this sound will follow a rolloff model defined by Sound.set3DCustomRolloff / Channel.set3DCustomRolloff.  */
        _3D_IGNOREGEOMETRY     = 0x40000000,  /* Is not affect by geometry occlusion.  If not specified in Sound.setMode, or Channel.setMode, the flag is cleared and it is affected by geometry again. */
        CDDA_FORCEASPI         = 0x00400000,  /* For CDDA sounds only - use ASPI instead of NTSCSI to access the specified CD/DVD device. */
        CDDA_JITTERCORRECT     = 0x00800000,  /* For CDDA sounds only - perform jitter correction. Jitter correction helps produce a more accurate CDDA stream at the cost of more CPU time. */
        UNICODE                = 0x01000000,  /* Filename is double-byte unicode. */
        IGNORETAGS             = 0x02000000,  /* Skips id3v2/asf/etc tag checks when opening a sound, to reduce seek/read overhead when opening files (helps with CD performance). */
        LOWMEM                 = 0x08000000,  /* Removes some features from samples to give a lower memory overhead, like Sound.getName. */
        LOADSECONDARYRAM       = 0x20000000,  /* Load sound into the secondary RAM of supported platform.  On PS3, sounds will be loaded into RSX/VRAM. */
        VIRTUAL_PLAYFROMSTART  = 0x80000000   /* For sounds that start virtual (due to being quiet or low importance), instead of swapping back to audible, and playing at the correct offset according to time, this flag makes the sound play from the start. */
    }


    /*
    [ENUM]
    [
        [DESCRIPTION]   
        These values describe what state a sound is in after NONBLOCKING has been used to open it.

        [REMARKS]    

        [PLATFORMS]
        Win32, Win64, Linux, Linux64, Macintosh, Xbox360, PlayStation Portable, PlayStation 3, Wii

        [SEE_ALSO]
        Sound.getOpenState
        MODE
    ]
    */
    public enum OPENSTATE :int
    {
        READY = 0,       /* Opened and ready to play */
        LOADING,         /* Initial load in progress */
        ERROR,           /* Failed to open - file not found, out of memory etc.  See return value of Sound.getOpenState for what happened. */
        CONNECTING,      /* Connecting to remote host (internet sounds only) */
        BUFFERING,       /* Buffering data */
        SEEKING,         /* Seeking to subsound and re-flushing stream buffer. */
        PLAYING,         /* Ready and playing, but not possible to release at this time without stalling the main thread. */
        SETPOSITION,     /* Seeking within a stream to a different position. */
    }


    /*
    [ENUM]
    [
        [DESCRIPTION]   
        These flags are used with SoundGroup.setMaxAudibleBehavior to determine what happens when more sounds 
        are played than are specified with SoundGroup.setMaxAudible.

        [REMARKS]
        When using FMOD_SOUNDGROUP_BEHAVIOR_MUTE, SoundGroup.setMuteFadeSpeed can be used to stop a sudden transition.  
        Instead, the time specified will be used to cross fade between the sounds that go silent and the ones that become audible.

        [PLATFORMS]
        Win32, Win64, Linux, Linux64, Macintosh, Xbox360, PlayStation Portable, PlayStation 3, Wii

        [SEE_ALSO]      
        SoundGroup.setMaxAudibleBehavior
        SoundGroup.getMaxAudibleBehavior
        SoundGroup.setMaxAudible
        SoundGroup.getMaxAudible
        SoundGroup.setMuteFadeSpeed
        SoundGroup.getMuteFadeSpeed
    ]
    */
    public enum SOUNDGROUP_BEHAVIOR :int
    {
        BEHAVIOR_FAIL,              /* Any sound played that puts the sound count over the SoundGroup.setMaxAudible setting, will simply fail during System.playSound. */
        BEHAVIOR_MUTE,              /* Any sound played that puts the sound count over the SoundGroup.setMaxAudible setting, will be silent, then if another sound in the group stops the sound that was silent before becomes audible again. */
        BEHAVIOR_STEALLOWEST        /* Any sound played that puts the sound count over the SoundGroup.setMaxAudible setting, will steal the quietest / least important sound playing in the group. */
    }

    /*
    [ENUM]
    [
        [DESCRIPTION]   
        These callback types are used with System.setCallback.

        [REMARKS]
        Each callback has commanddata parameters passed as int unique to the type of callback.<br>
        See reference to FMOD_SYSTEM_CALLBACK to determine what they might mean for each type of callback.<br>
        <br>
        <b>Note!</b>  Currently the user must call System.update for these callbacks to trigger!

        [PLATFORMS]
        Win32, Win64, Linux, Linux64, Macintosh, Xbox360, PlayStation Portable, PlayStation 3, Wii

        [SEE_ALSO]      
        System.setCallback
        FMOD_SYSTEM_CALLBACK
        System.update
    ]
    */
    public enum SYSTEM_CALLBACKTYPE :int
    {
        DEVICELISTCHANGED,      /* Called when the enumerated list of devices has changed. */
        DEVICELOST,             /* Called from System.update when an output device has been lost due to control panel parameter changes and FMOD cannot automatically recover. */
        MEMORYALLOCATIONFAILED, /* Called directly when a memory allocation fails somewhere in FMOD. */
        THREADCREATED,          /* Called directly when a thread is created. */
        BADDSPCONNECTION,       /* Called when a bad connection was made with DSP.addInput. Usually called from mixer thread because that is where the connections are made.  */
        BADDSPLEVEL,            /* Called when too many effects were added exceeding the maximum tree depth of 128.  this is most likely caused by accidentally adding too many DSP effects. Usually called from mixer thread because that is where the connections are made.  */

        MAX                     /* Maximum number of callback types supported. */
    }

    /*
    [ENUM]
    [
        [DESCRIPTION]   
        These callback types are used with Channel.setCallback.

        [REMARKS]
        Each callback has commanddata parameters passed int unique to the type of callback.
        See reference to FMOD_CHANNEL_CALLBACK to determine what they might mean for each type of callback.

        [PLATFORMS]
        Win32, Win64, Linux, Linux64, Macintosh, Xbox360, PlayStation Portable, PlayStation 3, Wii

        [SEE_ALSO]      
        Channel.setCallback
        FMOD_CHANNEL_CALLBACK
    ]
    */
    public enum CHANNEL_CALLBACKTYPE :int
    {
        END,                  /* Called when a sound ends. */
        VIRTUALVOICE,         /* Called when a voice is swapped out or swapped in. */
        SYNCPOINT,            /* Called when a syncpoint is encountered.  Can be from wav file markers. */
        OCCLUSION,            /* Called when the channel has its geometry occlusion value calculated.  Can be used to clamp or change the value. */

        MAX
    }


    /*
    [ENUM]
    [
        [DESCRIPTION]   
        List of windowing methods used in spectrum analysis to reduce leakage / transient signals intefering with the analysis.
        this is a problem with analysis of continuous signals that only have a small portion of the signal sample (the fft window size).
        Windowing the signal with a curve or triangle tapers the sides of the fft window to help alleviate this problem.

        [REMARKS]
        Cyclic signals such as a sine wave that repeat their cycle in a multiple of the window size do not need windowing.
        I.e. If the sine wave repeats every 1024, 512, 256 etc samples and the FMOD fft window is 1024, then the signal would not need windowing.
        Not windowing is the same as FMOD_DSP_FFT_WINDOW_RECT, which is the default.
        If the cycle of the signal (ie the sine wave) is not a multiple of the window size, it will cause frequency abnormalities, so a different windowing method is needed.
        <exclude>
        
        FMOD_DSP_FFT_WINDOW_RECT.
        <img src = "rectangle.gif"></img>
        
        FMOD_DSP_FFT_WINDOW_TRIANGLE.
        <img src = "triangle.gif"></img>
        
        FMOD_DSP_FFT_WINDOW_HAMMING.
        <img src = "hamming.gif"></img>
        
        FMOD_DSP_FFT_WINDOW_HANNING.
        <img src = "hanning.gif"></img>
        
        FMOD_DSP_FFT_WINDOW_BLACKMAN.
        <img src = "blackman.gif"></img>
        
        FMOD_DSP_FFT_WINDOW_BLACKMANHARRIS.
        <img src = "blackmanharris.gif"></img>
        </exclude>

        [PLATFORMS]
        Win32, Win64, Linux, Linux64, Macintosh, Xbox360, PlayStation Portable, PlayStation 3, Wii

        [SEE_ALSO]      
        System.getSpectrum
        Channel.getSpectrum
    ]
    */
    public enum DSP_FFT_WINDOW :int
    {
        RECT,           /* w[n] = 1.0                                                                                            */
        TRIANGLE,       /* w[n] = TRI(2n/N)                                                                                      */
        HAMMING,        /* w[n] = 0.54 - (0.46 * COS(n/N) )                                                                      */
        HANNING,        /* w[n] = 0.5 *  (1.0  - COS(n/N) )                                                                      */
        BLACKMAN,       /* w[n] = 0.42 - (0.5  * COS(n/N) ) + (0.08 * COS(2.0 * n/N) )                                           */
        BLACKMANHARRIS, /* w[n] = 0.35875 - (0.48829 * COS(1.0 * n/N)) + (0.14128 * COS(2.0 * n/N)) - (0.01168 * COS(3.0 * n/N)) */

        MAX
    }


    /*
    [ENUM]
    [
        [DESCRIPTION]   
        List of interpolation types that the FMOD Ex software mixer supports.  

        [REMARKS]
        The default resampler type is FMOD_DSP_RESAMPLER_LINEAR.<br>
        Use System.setSoftwareFormat to tell FMOD the resampling quality you require for FMOD_SOFTWARE based sounds.

        [PLATFORMS]
        Win32, Win64, Linux, Linux64, Macintosh, Xbox360, PlayStation Portable, PlayStation 3, Wii

        [SEE_ALSO]      
        System.setSoftwareFormat
        System.getSoftwareFormat
    ]
    */
    public enum DSP_RESAMPLER :int
    {
        NOINTERP,        /* No interpolation.  High frequency aliasing hiss will be audible depending on the sample rate of the sound. */
        LINEAR,          /* Linear interpolation (default method).  Fast and good quality, causes very slight lowpass effect on low frequency sounds. */
        CUBIC,           /* Cubic interpolation.  Slower than linear interpolation but better quality. */
        SPLINE,          /* 5 point spline interpolation.  Slowest resampling method but best quality. */

        MAX,             /* Maximum number of resample methods supported. */
    }


    /*
    [ENUM]
    [
        [DESCRIPTION]   
        List of tag types that could be stored within a sound.  These include id3 tags, metadata from netstreams and vorbis/asf data.

        [REMARKS]

        [PLATFORMS]
        Win32, Win64, Linux, Linux64, Macintosh, Xbox360, PlayStation Portable, PlayStation 3, Wii

        [SEE_ALSO]      
        Sound.getTag
    ]
    */
    public enum TAGTYPE :int
    {
        UNKNOWN = 0,
        ID3V1,
        ID3V2,
        VORBISCOMMENT,
        SHOUTCAST,
        ICECAST,
        ASF,
        MIDI,
        PLAYLIST,
        FMOD,
        USER
    }


    /*
    [ENUM]
    [
        [DESCRIPTION]   
        List of data types that can be returned by Sound.getTag

        [REMARKS]

        [PLATFORMS]
        Win32, Win64, Linux, Linux64, Macintosh, Xbox360, PlayStation Portable, PlayStation 3, Wii

        [SEE_ALSO]      
        Sound.getTag
    ]
    */
    public enum TAGDATATYPE :int
    {
        BINARY = 0,
        INT,
        FLOAT,
        STRING,
        STRING_UTF16,
        STRING_UTF16BE,
        STRING_UTF8,
        CDTOC
    }

    /*
    [ENUM]
    [
        [DESCRIPTION]   
        Types of delay that can be used with Channel.setDelay / Channel.getDelay.

        [REMARKS]
        If you haven't called Channel.setDelay yet, if you call Channel.getDelay with FMOD_DELAYTYPE_DSPCLOCK_START it will return the 
        equivalent global DSP clock value to determine when a channel started, so that you can use it for other channels to sync against.<br>
        <br>
        Use System.getDSPClock to also get the current dspclock time, a base for future calls to Channel.setDelay.<br>
        <br>
        Use FMOD_64BIT_ADD or FMOD_64BIT_SUB to add a hi/lo combination together and cope with wraparound.
        <br>
        If FMOD_DELAYTYPE_END_MS is specified, the value is not treated as a 64 bit number, just the delayhi value is used and it is treated as milliseconds.

        [PLATFORMS]
        Win32, Win64, Linux, Linux64, Macintosh, Xbox360, PlayStation Portable, PlayStation 3, Wii

        [SEE_ALSO]      
        Channel.setDelay
        Channel.getDelay
        System.getDSPClock
    ]
    */
    public enum DELAYTYPE :int
    {
        END_MS,              /* Delay at the end of the sound in milliseconds.  Use delayhi only.   Channel.isPlaying will remain true until this delay has passed even though the sound itself has stopped playing.*/
        DSPCLOCK_START,      /* Time the sound started if Channel.getDelay is used, or if Channel.setDelay is used, the sound will delay playing until this exact tick. */
        DSPCLOCK_END,        /* Time the sound should end. If this is non-zero, the channel will go silent at this exact tick. */
        DSPCLOCK_PAUSE,      /* Time the sound should pause. If this is non-zero, the channel will pause at this exact tick. */

        MAX                  /* Maximum number of tag datatypes supported. */
    }

    public class DELAYTYPE_UTILITY
    {
        void FMOD_64BIT_ADD(ref uint hi1, ref uint lo1, uint hi2, uint lo2)
        {
            hi1 += (uint)((hi2) + ((((lo1) + (lo2)) < (lo1)) ? 1 : 0));
            lo1 += (lo2);
        }

        void FMOD_64BIT_SUB(ref uint hi1, ref uint lo1, uint hi2, uint lo2)
        {
            hi1 -= (uint)((hi2) + ((((lo1) - (lo2)) > (lo1)) ? 1 : 0));
            lo1 -= (lo2);
        }
    }


    /*
    [STRUCTURE] 
    [
        [DESCRIPTION]   
        Structure describing a piece of tag data.

        [REMARKS]
        Members marked with [in] mean the user sets the value before passing it to the function.
        Members marked with [out] mean FMOD sets the value to be used after the function exits.

        [PLATFORMS]
        Win32, Win64, Linux, Linux64, Macintosh, Xbox360, PlayStation Portable, PlayStation 3, Wii

        [SEE_ALSO]      
        Sound.getTag
        TAGTYPE
        TAGDATATYPE
    ]
    */
    [StructLayout(LayoutKind.Sequential)]
    public struct TAG
    {
        public TAGTYPE           type;         /* [out] The type of this tag. */
        public TAGDATATYPE       datatype;     /* [out] The type of data that this tag contains */
        public IntPtr            namePtr;      /* [out] The name of this tag i.e. "TITLE", "ARTIST" etc. */
        public IntPtr            data;         /* [out] Pointer to the tag data - its format is determined by the datatype member */
        public uint              datalen;      /* [out] Length of the data contained in this tag */
        public bool              updated;      /* [out] True if this tag has been updated since last being accessed with Sound.getTag */

        public string name { get { return Marshal.PtrToStringAnsi(namePtr); } }
    }


    /*
    [STRUCTURE] 
    [
        [DESCRIPTION]   
        Structure describing a CD/DVD table of contents

        [REMARKS]
        Members marked with [in] mean the user sets the value before passing it to the function.
        Members marked with [out] mean FMOD sets the value to be used after the function exits.

        [PLATFORMS]
        Win32, Win64, Linux, Linux64, Macintosh, Xbox360, PlayStation Portable, PlayStation 3, Wii

        [SEE_ALSO]      
        Sound.getTag
    ]
    */
    [StructLayout(LayoutKind.Sequential)]
    public struct CDTOC
    {
        public int numtracks;                  /* [out] The number of tracks on the CD */
        [MarshalAs(UnmanagedType.ByValArray,SizeConst=100)]
        public int[] min;                   /* [out] The start offset of each track in minutes */
        [MarshalAs(UnmanagedType.ByValArray,SizeConst=100)]
        public int[] sec;                   /* [out] The start offset of each track in seconds */
        [MarshalAs(UnmanagedType.ByValArray,SizeConst=100)]
        public int[] frame;                 /* [out] The start offset of each track in frames */
    }


    /*
    [ENUM]
    [
        [DESCRIPTION]   
        List of time types that can be returned by Sound.getLength and used with Channel.setPosition or Channel.getPosition.

        [REMARKS]

        [PLATFORMS]
        Win32, Win64, Linux, Linux64, Macintosh, Xbox360, PlayStation Portable, PlayStation 3, Wii

        [SEE_ALSO]      
        Sound.getLength
        Channel.setPosition
        Channel.getPosition
    ]
    */
    public enum TIMEUNIT
    {
        MS                = 0x00000001,  /* Milliseconds. */
        PCM               = 0x00000002,  /* PCM Samples, related to milliseconds * samplerate / 1000. */
        PCMBYTES          = 0x00000004,  /* Bytes, related to PCM samples * channels * datawidth (ie 16bit = 2 bytes). */
        RAWBYTES          = 0x00000008,  /* Raw file bytes of (compressed) sound data (does not include headers).  Only used by Sound.getLength and Channel.getPosition. */        
        PCMFRACTION       = 0x00000010,  /* Fractions of 1 PCM sample.  Unsigned int range 0 to 0xFFFFFFFF.  Used for sub-sample granularity for DSP purposes. */
        MODORDER          = 0x00000100,  /* MOD/S3M/XM/IT.  Order in a sequenced module format.  Use Sound.getFormat to determine the format. */
        MODROW            = 0x00000200,  /* MOD/S3M/XM/IT.  Current row in a sequenced module format.  Sound.getLength will return the number if rows in the currently playing or seeked to pattern. */
        MODPATTERN        = 0x00000400,  /* MOD/S3M/XM/IT.  Current pattern in a sequenced module format.  Sound.getLength will return the number of patterns in the song and Channel.getPosition will return the currently playing pattern. */
        SENTENCE_MS       = 0x00010000,  /* Currently playing subsound in a sentence time in milliseconds. */
        SENTENCE_PCM      = 0x00020000,  /* Currently playing subsound in a sentence time in PCM Samples, related to milliseconds * samplerate / 1000. */
        SENTENCE_PCMBYTES = 0x00040000,  /* Currently playing subsound in a sentence time in bytes, related to PCM samples * channels * datawidth (ie 16bit = 2 bytes). */
        SENTENCE          = 0x00080000,  /* Currently playing sentence index according to the channel. */
        SENTENCE_SUBSOUND = 0x00100000,  /* Currently playing subsound index in a sentence. */
        BUFFERED          = 0x10000000,  /* Time value as seen by buffered stream.  this is always ahead of audible time, and is only used for processing. */
    }


    /*
    [ENUM]
    [
        [DESCRIPTION]
        When creating a multichannel sound, FMOD will pan them to their default speaker locations, for example a 6 channel sound will default to one channel per 5.1 output speaker.<br>
        Another example is a stereo sound.  It will default to left = front left, right = front right.<br>
        <br>
        this is for sounds that are not 'default'.  For example you might have a sound that is 6 channels but actually made up of 3 stereo pairs, that should all be located in front left, front right only.

        [REMARKS]
        For full flexibility of speaker assignments, use Channel.setSpeakerLevels.  this functionality is cheaper, uses less memory and easier to use.

        [PLATFORMS]
        Win32, Win64, Linux, Linux64, Macintosh, Xbox360, PlayStation Portable, PlayStation 3, Wii

        [SEE_ALSO]
        FMOD_CREATESOUNDEXINFO
        Channel.setSpeakerLevels
    ]
    */
    public enum SPEAKERMAPTYPE
    {
        DEFAULT,     /* this is the default, and just means FMOD decides which speakers it puts the source channels. */
        ALLMONO,     /* this means the sound is made up of all mono sounds.  All voices will be panned to the front center by default in this case.  */
        ALLSTEREO,   /* this means the sound is made up of all stereo sounds.  All voices will be panned to front left and front right alternating every second channel.  */
        _51_PROTOOLS /* Map a 5.1 sound to use protools L C R Ls Rs LFE mapping.  Will return an error if not a 6 channel sound. */
    }


    /*
    [STRUCTURE] 
    [
        [DESCRIPTION]
        Structure defining a reverb environment.<br>
        <br>
        For more indepth descriptions of the reverb properties under win32, please see the EAX2 and EAX3
        documentation at http://developer.creative.com/ under the 'downloads' section.<br>
        If they do not have the EAX3 documentation, then most information can be attained from
        the EAX2 documentation, as EAX3 only adds some more parameters and functionality on top of 
        EAX2.

        [REMARKS]
        Note the default reverb properties are the same as the FMOD_PRESET_GENERIC preset.<br>
        Note that integer values that typically range from -10,000 to 1000 are represented in 
        decibels, and are of a logarithmic scale, not linear, wheras float values are always linear.<br>
        <br>
        The numerical values listed below are the maximum, minimum and default values for each variable respectively.<br>
        <br>
        <b>SUPPORTED</b> next to each parameter means the platform the parameter can be set on.  Some platforms support all parameters and some don't.<br>
        EAX   means hardware reverb on FMOD_OUTPUTTYPE_DSOUND on windows only (must use FMOD_HARDWARE), on soundcards that support EAX 1 to 4.<br>
        EAX4  means hardware reverb on FMOD_OUTPUTTYPE_DSOUND on windows only (must use FMOD_HARDWARE), on soundcards that support EAX 4.<br>
        I3DL2 means hardware reverb on FMOD_OUTPUTTYPE_DSOUND on windows only (must use FMOD_HARDWARE), on soundcards that support I3DL2 non EAX native reverb.<br>
        GC    means Nintendo Gamecube hardware reverb (must use FMOD_HARDWARE).<br>
        WII   means Nintendo Wii hardware reverb (must use FMOD_HARDWARE).<br>
        PS2   means Playstation 2 hardware reverb (must use FMOD_HARDWARE).<br>
        SFX   means FMOD SFX software reverb.  this works on any platform that uses FMOD_SOFTWARE for loading sounds.<br>
        <br>
        Members marked with [in] mean the user sets the value before passing it to the function.<br>
        Members marked with [out] mean FMOD sets the value to be used after the function exits.<br>

        [PLATFORMS]
        Win32, Win64, Linux, Linux64, Macintosh, Xbox360, PlayStation Portable, PlayStation 3, Wii

        [SEE_ALSO]
        System.setReverbProperties
        System.getReverbProperties
        FMOD_REVERB_PRESETS
        FMOD_REVERB_FLAGS
    ]
    */
    [StructLayout(LayoutKind.Sequential)]
    public struct REVERB_PROPERTIES
    {                                   /*          MIN     MAX    DEFAULT   DESCRIPTION */
        public int   Instance;          /* [in]     0     , 3     , 0      , EAX4 only. Environment Instance. 3 seperate reverbs simultaneously are possible. this specifies which one to set. (win32 only) */
        public int   Environment;       /* [in/out] -1    , 25    , -1     , sets all listener properties (win32/ps2) */
        public float EnvDiffusion;      /* [in/out] 0.0   , 1.0   , 1.0    , environment diffusion (win32/xbox) */
        public int   Room;              /* [in/out] -10000, 0     , -1000  , room effect level (at mid frequencies) (win32/xbox) */
        public int   RoomHF;            /* [in/out] -10000, 0     , -100   , relative room effect level at high frequencies (win32/xbox) */
        public int   RoomLF;            /* [in/out] -10000, 0     , 0      , relative room effect level at low frequencies (win32 only) */
        public float DecayTime;         /* [in/out] 0.1   , 20.0  , 1.49   , reverberation decay time at mid frequencies (win32/xbox) */
        public float DecayHFRatio;      /* [in/out] 0.1   , 2.0   , 0.83   , high-frequency to mid-frequency decay time ratio (win32/xbox) */
        public float DecayLFRatio;      /* [in/out] 0.1   , 2.0   , 1.0    , low-frequency to mid-frequency decay time ratio (win32 only) */
        public int   Reflections;       /* [in/out] -10000, 1000  , -2602  , early reflections level relative to room effect (win32/xbox) */
        public float ReflectionsDelay;  /* [in/out] 0.0   , 0.3   , 0.007  , initial reflection delay time (win32/xbox) */
        public int   Reverb;            /* [in/out] -10000, 2000  , 200    , late reverberation level relative to room effect (win32/xbox) */
        public float ReverbDelay;       /* [in/out] 0.0   , 0.1   , 0.011  , late reverberation delay time relative to initial reflection (win32/xbox) */
        public float ModulationTime;    /* [in/out] 0.04  , 4.0   , 0.25   , modulation time (win32 only) */
        public float ModulationDepth;   /* [in/out] 0.0   , 1.0   , 0.0    , modulation depth (win32 only) */
        public float HFReference;       /* [in/out] 1000.0, 20000 , 5000.0 , reference high frequency (hz) (win32/xbox) */
        public float LFReference;       /* [in/out] 20.0  , 1000.0, 250.0  , reference low frequency (hz) (win32 only) */
        public float Diffusion;         /* [in/out] 0.0   , 100.0 , 100.0  , Value that controls the echo density in the late reverberation decay. (xbox only) */
        public float Density;           /* [in/out] 0.0   , 100.0 , 100.0  , Value that controls the modal density in the late reverberation decay (xbox only) */
        public uint  Flags;             /* [in/out] REVERB_FLAGS - modifies the behavior of above properties (win32/ps2) */

        #region wrapperinternal
        public REVERB_PROPERTIES(int instance, int environment, float envDiffusion, int room, int roomHF, int roomLF,
            float decayTime, float decayHFRatio, float decayLFRatio, int reflections, float reflectionsDelay,
            int reverb, float reverbDelay, float modulationTime, float modulationDepth, float hfReference,
            float lfReference, float diffusion, float density, uint flags)
        {
            Instance            = instance;
            Environment         = environment;
            EnvDiffusion        = envDiffusion;
            Room                = room;
            RoomHF              = roomHF;
            RoomLF              = roomLF;
            DecayTime           = decayTime;
            DecayHFRatio        = decayHFRatio;
            DecayLFRatio        = decayLFRatio;
            Reflections         = reflections;
            ReflectionsDelay    = reflectionsDelay;
            Reverb              = reverb;
            ReverbDelay          = reverbDelay;
            ModulationTime      = modulationTime;
            ModulationDepth     = modulationDepth;
            HFReference         = hfReference;
            LFReference         = lfReference;
            Diffusion           = diffusion;
            Density             = density;
            Flags               = flags;
        }
        #endregion
    }


    /*
    [DEFINE] 
    [
        [NAME] 
        REVERB_FLAGS

        [DESCRIPTION]
        Values for the Flags member of the REVERB_PROPERTIES structure.

        [PLATFORMS]
        Win32, Win64, Linux, Linux64, Macintosh, Xbox360, PlayStation Portable, PlayStation 3, Wii

        [SEE_ALSO]
        REVERB_PROPERTIES
    ]
    */
    [StructLayout(LayoutKind.Sequential)]
    public struct REVERB_FLAGS
    {
        public const uint HIGHQUALITYREVERB     = 0x00000400; /* Wii. Use high quality reverb */
        public const uint HIGHQUALITYDPL2REVERB = 0x00000800; /* Wii. Use high quality DPL2 reverb */
        public const uint DEFAULT               = 0x00000000;
    }


    /*
    [DEFINE] 
    [
    [NAME] 
    FMOD_REVERB_PRESETS

    [DESCRIPTION]   
    A set of predefined environment PARAMETERS, created by Creative Labs
    These are used to initialize an FMOD_REVERB_PROPERTIES structure statically.
    ie 
    FMOD_REVERB_PROPERTIES prop = FMOD_PRESET_GENERIC;

    [PLATFORMS]
    Win32, Win64, Linux, Linux64, Macintosh, Xbox360, PlayStation Portable, PlayStation 3, Wii

    [SEE_ALSO]
    System.setReverbProperties
    ]
    */
    public class PRESET
    {
        /*                                                                           Instance  Env   Diffus  Room   RoomHF  RmLF DecTm   DecHF  DecLF   Refl  RefDel   Revb  RevDel  ModTm  ModDp   HFRef    LFRef   Diffus  Densty  FLAGS */
        public REVERB_PROPERTIES OFF()                 { return new REVERB_PROPERTIES(0,      -1,    1.00f, -10000, -10000, 0,   1.00f,  1.00f, 1.0f,  -2602, 0.007f,   200, 0.011f, 0.25f, 0.000f, 5000.0f, 250.0f, 0.0f,   0.0f,  0x33f );}
        public REVERB_PROPERTIES GENERIC()             { return new REVERB_PROPERTIES(0,       0,    1.00f, -1000,  -100,   0,   1.49f,  0.83f, 1.0f,  -2602, 0.007f,   200, 0.011f, 0.25f, 0.000f, 5000.0f, 250.0f, 100.0f, 100.0f, 0x3f );}
        public REVERB_PROPERTIES PADDEDCELL()          { return new REVERB_PROPERTIES(0,       1,    1.00f, -1000,  -6000,  0,   0.17f,  0.10f, 1.0f,  -1204, 0.001f,   207, 0.002f, 0.25f, 0.000f, 5000.0f, 250.0f, 100.0f, 100.0f, 0x3f );}
        public REVERB_PROPERTIES ROOM()                { return new REVERB_PROPERTIES(0,       2,    1.00f, -1000,  -454,   0,   0.40f,  0.83f, 1.0f,  -1646, 0.002f,    53, 0.003f, 0.25f, 0.000f, 5000.0f, 250.0f, 100.0f, 100.0f, 0x3f );}
        public REVERB_PROPERTIES BATHROOM()            { return new REVERB_PROPERTIES(0,       3,    1.00f, -1000,  -1200,  0,   1.49f,  0.54f, 1.0f,   -370, 0.007f,  1030, 0.011f, 0.25f, 0.000f, 5000.0f, 250.0f, 100.0f,  60.0f, 0x3f );}
        public REVERB_PROPERTIES LIVINGROOM()          { return new REVERB_PROPERTIES(0,       4,    1.00f, -1000,  -6000,  0,   0.50f,  0.10f, 1.0f,  -1376, 0.003f, -1104, 0.004f, 0.25f, 0.000f, 5000.0f, 250.0f, 100.0f, 100.0f, 0x3f );}
        public REVERB_PROPERTIES STONEROOM()           { return new REVERB_PROPERTIES(0,       5,    1.00f, -1000,  -300,   0,   2.31f,  0.64f, 1.0f,   -711, 0.012f,    83, 0.017f, 0.25f, 0.000f, 5000.0f, 250.0f, 100.0f, 100.0f, 0x3f );}
        public REVERB_PROPERTIES AUDITORIUM()          { return new REVERB_PROPERTIES(0,       6,    1.00f, -1000,  -476,   0,   4.32f,  0.59f, 1.0f,   -789, 0.020f,  -289, 0.030f, 0.25f, 0.000f, 5000.0f, 250.0f, 100.0f, 100.0f, 0x3f );}
        public REVERB_PROPERTIES CONCERTHALL()         { return new REVERB_PROPERTIES(0,       7,    1.00f, -1000,  -500,   0,   3.92f,  0.70f, 1.0f,  -1230, 0.020f,    -2, 0.029f, 0.25f, 0.000f, 5000.0f, 250.0f, 100.0f, 100.0f, 0x3f );}
        public REVERB_PROPERTIES CAVE()                { return new REVERB_PROPERTIES(0,       8,    1.00f, -1000,  0,      0,   2.91f,  1.30f, 1.0f,   -602, 0.015f,  -302, 0.022f, 0.25f, 0.000f, 5000.0f, 250.0f, 100.0f, 100.0f, 0x1f );}
        public REVERB_PROPERTIES ARENA()               { return new REVERB_PROPERTIES(0,       9,    1.00f, -1000,  -698,   0,   7.24f,  0.33f, 1.0f,  -1166, 0.020f,    16, 0.030f, 0.25f, 0.000f, 5000.0f, 250.0f, 100.0f, 100.0f, 0x3f );}
        public REVERB_PROPERTIES HANGAR()              { return new REVERB_PROPERTIES(0,       10,   1.00f, -1000,  -1000,  0,   10.05f, 0.23f, 1.0f,   -602, 0.020f,   198, 0.030f, 0.25f, 0.000f, 5000.0f, 250.0f, 100.0f, 100.0f, 0x3f );}
        public REVERB_PROPERTIES CARPETTEDHALLWAY()    { return new REVERB_PROPERTIES(0,       11,   1.00f, -1000,  -4000,  0,   0.30f,  0.10f, 1.0f,  -1831, 0.002f, -1630, 0.030f, 0.25f, 0.000f, 5000.0f, 250.0f, 100.0f, 100.0f, 0x3f );}
        public REVERB_PROPERTIES HALLWAY()             { return new REVERB_PROPERTIES(0,       12,   1.00f, -1000,  -300,   0,   1.49f,  0.59f, 1.0f,  -1219, 0.007f,   441, 0.011f, 0.25f, 0.000f, 5000.0f, 250.0f, 100.0f, 100.0f, 0x3f );}
        public REVERB_PROPERTIES STONECORRIDOR()       { return new REVERB_PROPERTIES(0,       13,   1.00f, -1000,  -237,   0,   2.70f,  0.79f, 1.0f,  -1214, 0.013f,   395, 0.020f, 0.25f, 0.000f, 5000.0f, 250.0f, 100.0f, 100.0f, 0x3f );}
        public REVERB_PROPERTIES ALLEY()               { return new REVERB_PROPERTIES(0,       14,   0.30f, -1000,  -270,   0,   1.49f,  0.86f, 1.0f,  -1204, 0.007f,    -4, 0.011f, 0.25f, 0.000f, 5000.0f, 250.0f, 100.0f, 100.0f, 0x3f );}
        public REVERB_PROPERTIES FOREST()              { return new REVERB_PROPERTIES(0,       15,   0.30f, -1000,  -3300,  0,   1.49f,  0.54f, 1.0f,  -2560, 0.162f,  -229, 0.088f, 0.25f, 0.000f, 5000.0f, 250.0f,  79.0f, 100.0f, 0x3f );}
        public REVERB_PROPERTIES CITY()                { return new REVERB_PROPERTIES(0,       16,   0.50f, -1000,  -800,   0,   1.49f,  0.67f, 1.0f,  -2273, 0.007f, -1691, 0.011f, 0.25f, 0.000f, 5000.0f, 250.0f,  50.0f, 100.0f, 0x3f );}
        public REVERB_PROPERTIES MOUNTAINS()           { return new REVERB_PROPERTIES(0,       17,   0.27f, -1000,  -2500,  0,   1.49f,  0.21f, 1.0f,  -2780, 0.300f, -1434, 0.100f, 0.25f, 0.000f, 5000.0f, 250.0f,  27.0f, 100.0f, 0x1f );}
        public REVERB_PROPERTIES QUARRY()              { return new REVERB_PROPERTIES(0,       18,   1.00f, -1000,  -1000,  0,   1.49f,  0.83f, 1.0f, -10000, 0.061f,   500, 0.025f, 0.25f, 0.000f, 5000.0f, 250.0f, 100.0f, 100.0f, 0x3f );}
        public REVERB_PROPERTIES PLAIN()               { return new REVERB_PROPERTIES(0,       19,   0.21f, -1000,  -2000,  0,   1.49f,  0.50f, 1.0f,  -2466, 0.179f, -1926, 0.100f, 0.25f, 0.000f, 5000.0f, 250.0f,  21.0f, 100.0f, 0x3f );}
        public REVERB_PROPERTIES PARKINGLOT()          { return new REVERB_PROPERTIES(0,       20,   1.00f, -1000,  0,      0,   1.65f,  1.50f, 1.0f,  -1363, 0.008f, -1153, 0.012f, 0.25f, 0.000f, 5000.0f, 250.0f, 100.0f, 100.0f, 0x1f );}
        public REVERB_PROPERTIES SEWERPIPE()           { return new REVERB_PROPERTIES(0,       21,   0.80f, -1000,  -1000,  0,   2.81f,  0.14f, 1.0f,    429, 0.014f,  1023, 0.021f, 0.25f, 0.000f, 5000.0f, 250.0f,  80.0f,  60.0f, 0x3f );}
        public REVERB_PROPERTIES UNDERWATER()          { return new REVERB_PROPERTIES(0,       22,   1.00f, -1000,  -4000,  0,   1.49f,  0.10f, 1.0f,   -449, 0.007f,  1700, 0.011f, 1.18f, 0.348f, 5000.0f, 250.0f, 100.0f, 100.0f, 0x3f );}
    }

    /*
    [STRUCTURE] 
    [
        [DESCRIPTION]
        Structure defining the properties for a reverb source, related to a FMOD channel.

        For more indepth descriptions of the reverb properties under win32, please see the EAX3
        documentation at http://developer.creative.com/ under the 'downloads' section.
        If they do not have the EAX3 documentation, then most information can be attained from
        the EAX2 documentation, as EAX3 only adds some more parameters and functionality on top of 
        EAX2.

        Note the default reverb properties are the same as the PRESET_GENERIC preset.
        Note that integer values that typically range from -10,000 to 1000 are represented in 
        decibels, and are of a logarithmic scale, not linear, wheras FLOAT values are typically linear.
        PORTABILITY: Each member has the platform it supports in braces ie (win32/xbox).  
        Some reverb parameters are only supported in win32 and some only on xbox. If all parameters are set then
        the reverb should product a similar effect on either platform.
        Linux and FMODCE do not support the reverb api.

        The numerical values listed below are the maximum, minimum and default values for each variable respectively.

        [REMARKS]
        For EAX4 support with multiple reverb environments, set FMOD_REVERB_CHANNELFLAGS_ENVIRONMENT0,
        FMOD_REVERB_CHANNELFLAGS_ENVIRONMENT1 or/and FMOD_REVERB_CHANNELFLAGS_ENVIRONMENT2 in the flags member 
        of FMOD_REVERB_CHANNELPROPERTIES to specify which environment instance(s) to target. 
        Only up to 2 environments to target can be specified at once. Specifying three will result in an error.
        If the sound card does not support EAX4, the environment flag is ignored.

        [PLATFORMS]
        Win32, Win64, Linux, Linux64, Macintosh, Xbox360, PlayStation Portable, PlayStation 3, Wii

        [SEE_ALSO]
        Channel.setReverbProperties
        Channel.getReverbProperties
        REVERB_CHANNELFLAGS
    ]
    */
    [StructLayout(LayoutKind.Sequential)]
    public struct REVERB_CHANNELPROPERTIES  
    {                                          /*          MIN     MAX    DEFAULT  DESCRIPTION */
        public int       Direct;               /* [in/out] -10000, 1000,  0,       direct path level (at low and mid frequencies) (win32/xbox) */
        public int       Room;                 /* [in/out] -10000, 1000,  0,       room effect level (at low and mid frequencies) (win32/xbox) */
        public uint      Flags;                /* [in/out] REVERB_CHANNELFLAGS - modifies the behavior of properties (win32) */
        public IntPtr    ConnectionPoint;      /* [in/out] See remarks.            DSP network location to connect reverb for this channel.    (SUPPORTED:SFX only).*/
    }


    /*
    [DEFINE] 
    [
        [NAME] 
        REVERB_CHANNELFLAGS

        [DESCRIPTION]
        Values for the Flags member of the REVERB_CHANNELPROPERTIES structure.

        [PLATFORMS]
        Win32, Win64, Linux, Linux64, Macintosh, Xbox360, PlayStation Portable, PlayStation 3, Wii

        [SEE_ALSO]
        REVERB_CHANNELPROPERTIES
    ]
    */
    [StructLayout(LayoutKind.Sequential)]
    public struct REVERB_CHANNELFLAGS
    {
        public const uint INSTANCE0     = 0x00000010; /* SFX/Wii. Specify channel to target reverb instance 0.  Default target. */
        public const uint INSTANCE1     = 0x00000020; /* SFX/Wii. Specify channel to target reverb instance 1. */
        public const uint INSTANCE2     = 0x00000040; /* SFX/Wii. Specify channel to target reverb instance 2. */
        public const uint INSTANCE3     = 0x00000080; /* SFX. Specify channel to target reverb instance 3. */
        public const uint DEFAULT       = INSTANCE0;
    }


    /*
    [STRUCTURE] 
    [
        [DESCRIPTION]
        Settings for advanced features like configuring memory and cpu usage for the FMOD_CREATECOMPRESSEDSAMPLE feature.
   
        [REMARKS]
        maxMPEGcodecs / maxADPCMcodecs / maxXMAcodecs will determine the maximum cpu usage of playing realtime samples.  Use this to lower potential excess cpu usage and also control memory usage.<br>
   
        [PLATFORMS]
        Win32, Win64, Linux, Linux64, Macintosh, Xbox360, PlayStation Portable, PlayStation 3
   
        [SEE_ALSO]
        System.setAdvancedSettings
        System.getAdvancedSettings
    ]
    */
    [StructLayout(LayoutKind.Sequential)]
    public struct ADVANCEDSETTINGS
    {                       
        public int     cbsize;                      /* Size of structure.  Use sizeof(FMOD_ADVANCEDSETTINGS) */
        public int     maxMPEGcodecs;               /* For use with FMOD_CREATECOMPRESSEDSAMPLE only.  Mpeg  codecs consume 48,696 per instance and this number will determine how many mpeg channels can be played simultaneously.  Default = 16. */
        public int     maxADPCMcodecs;              /* For use with FMOD_CREATECOMPRESSEDSAMPLE only.  ADPCM codecs consume 1k per instance and this number will determine how many ADPCM channels can be played simultaneously.  Default = 32. */
        public int     maxXMAcodecs;                /* For use with FMOD_CREATECOMPRESSEDSAMPLE only.  XMA   codecs consume 8k per instance and this number will determine how many XMA channels can be played simultaneously.  Default = 32.  */
        public int     maxPCMcodecs;                /* [in/out] Optional. Specify 0 to ignore. For use with PS3 only.                          PCM   codecs consume 12,672 bytes per instance and this number will determine how many streams and PCM voices can be played simultaneously. Default = 16 */
        public int     maxCELTcodecs;               /* [in/out] Optional. Specify 0 to ignore. For use with FMOD_CREATECOMPRESSEDSAMPLE only.  CELT  codecs consume 11,500 bytes per instance and this number will determine how many CELT channels can be played simultaneously. Default = 16 */    
        public int     maxVORBIScodecs;             /* [in/out] Optional. Specify 0 to ignore. For use with FMOD_CREATECOMPRESSEDSAMPLE only.  Vorbis codecs consume 12,000 bytes per instance and this number will determine how many Vorbis channels can be played simultaneously. Default = 32. */    
        public int     ASIONumChannels;             /* [in/out] */
        public IntPtr  ASIOChannelList;             /* [in/out] */
        public IntPtr  ASIOSpeakerList;             /* [in/out] Optional. Specify 0 to ignore. Pointer to a list of speakers that the ASIO channels map to.  this can be called after System.init to remap ASIO output. */
        public int     max3DReverbDSPs;             /* [in/out] The max number of 3d reverb DSP's in the system. */
        public float   HRTFMinAngle;                /* [in/out] For use with FMOD_INIT_HRTF_LOWPASS.  The angle (0-360) of a 3D sound from the listener's forward vector at which the HRTF function begins to have an effect.  Default = 180.0. */
        public float   HRTFMaxAngle;                /* [in/out] For use with FMOD_INIT_HRTF_LOWPASS.  The angle (0-360) of a 3D sound from the listener's forward vector at which the HRTF function begins to have maximum effect.  Default = 360.0.  */
        public float   HRTFFreq;                    /* [in/out] For use with FMOD_INIT_HRTF_LOWPASS.  The cutoff frequency of the HRTF's lowpass filter function when at maximum effect. (i.e. at HRTFMaxAngle).  Default = 4000.0. */
        public float   vol0virtualvol;              /* [in/out] For use with FMOD_INIT_VOL0_BECOMES_VIRTUAL.  If this flag is used, and the volume is 0.0, then the sound will become virtual.  Use this value to raise the threshold to a different point where a sound goes virtual. */
        public int     eventqueuesize;              /* [in/out] Optional. Specify 0 to ignore. For use with FMOD Event system only.  Specifies the number of slots available for simultaneous non blocking loads.  Default = 32. */
        public uint    defaultDecodeBufferSize;     /* [in/out] Optional. Specify 0 to ignore. For streams. this determines the default size of the double buffer (in milliseconds) that a stream uses.  Default = 400ms */
        public string  debugLogFilename;            /* [in/out] Optional. Specify 0 to ignore. Gives fmod's logging system a path/filename.  Normally the log is placed in the same directory as the executable and called fmod.log. When using System.getAdvancedSettings, provide at least 256 bytes of memory to copy into. */
        public ushort  profileport;                 /* [in/out] Optional. Specify 0 to ignore. For use with FMOD_INIT_ENABLE_PROFILE.  Specify the port to listen on for connections by the profiler application. */
        public uint    geometryMaxFadeTime;         /* [in/out] Optional. Specify 0 to ignore. The maximum time in miliseconds it takes for a channel to fade to the new level when its occlusion changes. */
        public uint    maxSpectrumWaveDataBuffers;  /* [in/out] Optional. Specify 0 to ignore. The maximum number of buffers for use with getWaveData/getSpectrum. */
        public uint    musicSystemCacheDelay;       /* [in/out] Optional. Specify 0 to ignore. The delay the music system should allow for loading a sample from disk (in milliseconds). Default = 400 ms. */
        public float   distanceFilterCenterFreq;    /* [in/out] Optional. Specify 0 to ignore. For use with FMOD_INIT_DISTANCE_FILTERING.  The default center frequency in Hz for the distance filtering effect. Default = 1500.0. */
        public uint    stackSizeStream;             /* [in/out] Optional. Specify 0 to ignore. Specify the stack size for the FMOD Stream thread in bytes.  Useful for custom codecs that use excess stack.  Default 49,152 (48kb) */
        public uint    stackSizeNonBlocking;        /* [in/out] Optional. Specify 0 to ignore. Specify the stack size for the FMOD_NONBLOCKING loading thread.  Useful for custom codecs that use excess stack.  Default 65,536 (64kb) */
        public uint    stackSizeMixer;              /* [in/out] Optional. Specify 0 to ignore. Specify the stack size for the FMOD mixer thread.  Useful for custom dsps that use excess stack.  Default 49,152 (48kb) */
    }


    /*
    [ENUM] 
    [
        [NAME] 
        FMOD_MISC_VALUES

        [DESCRIPTION]
        Miscellaneous values for FMOD functions.

        [PLATFORMS]
        Win32, Win64, Linux, Linux64, Macintosh, Xbox360, PlayStation Portable, PlayStation 3, Wii

        [SEE_ALSO]
        System.playSound
        System.playDSP
        System.getChannel
    ]
    */
    public enum CHANNELINDEX
    {
        FREE   = -1,     /* For a channel index, FMOD chooses a free voice using the priority system. */
        REUSE  = -2      /* For a channel index, re-use the channel handle that was passed in. */
    }


    public class Factory
    {
        public static RESULT System_Create(ref System system)
        {
#if WIN64
            if (IntPtr.Size != 8)
            {
                /* Attempting to use 64-bit FMOD dll with 32-bit application.*/
            
                return RESULT.ERR_FILE_BAD;
            }
#else
            if (IntPtr.Size != 4)
            {
                /* Attempting to use 32-bit FMOD dll with 64-bit application. A likely cause of this error 
                 * is targetting platform 'Any CPU'. You cannot link to unmanaged dll with 'Any CPU'
                 * target. 
                 * 
                 * For 32-bit applications: set the platform to 'x86'.
                 * 
                 * For 64-bit applications:
                 * 1. set the platform to x64
                 * 2. add the conditional complication symbol WIN64
                 * 3. download the win64 fmod release
                 * 4. copy the fmodex64.dll to the location of the .exe file for your application */

                return RESULT.ERR_FILE_BAD;
            }
#endif

            RESULT result           = RESULT.OK;
            IntPtr      systemraw   = new IntPtr();
            System      systemnew   = null;

            result = FMOD_System_Create(ref systemraw);
            if (result != RESULT.OK)
            {
                return result;
            }

            systemnew = new System();
            systemnew.setRaw(systemraw);
            system = systemnew;

            return result;
        }


        #region importfunctions
  
        [DllImport (VERSION.dll)]
        private static extern RESULT FMOD_System_Create                      (ref IntPtr system);

        #endregion
    }

    
    public class System
    {
        public RESULT release                ()
        {
            return FMOD_System_Release(systemraw);
        }

        public RESULT setOutput              (OUTPUTTYPE output)
        {
            return FMOD_System_SetOutput(systemraw, output);
        }

        public RESULT setDriver              (int driver)
        {
            return FMOD_System_SetDriver(systemraw, driver);
        }
        
        public RESULT init                   (int maxchannels, INITFLAGS flags, IntPtr extradriverdata)
        {
            return FMOD_System_Init(systemraw, maxchannels, flags, extradriverdata);
        }

        public RESULT close                  ()
        {
            return FMOD_System_Close(systemraw);
        }

        public RESULT getVersion             (ref uint version)
        {
            return FMOD_System_GetVersion(systemraw, ref version);
        }

        public RESULT createStream            (string name_or_data, MODE mode, ref Sound sound)
        {
            RESULT result           = RESULT.OK;
            IntPtr      soundraw    = new IntPtr();
            Sound       soundnew    = null;

            mode = mode | FMOD.MODE.UNICODE;

            try
            {
                result = FMOD_System_CreateStream(systemraw, name_or_data, mode, 0, ref soundraw);
            }
            catch
            {
                result = RESULT.ERR_INVALID_PARAM;
            }
            if (result != RESULT.OK)
            {
                return result;
            }

            if (sound == null)
            {
                soundnew = new Sound();
                soundnew.setRaw(soundraw);
                sound = soundnew;
            }
            else
            {
                sound.setRaw(soundraw);
            }

            return result;
        }

        public RESULT playSound              (CHANNELINDEX channelid, Sound sound, bool paused, ref Channel channel)
        {
            RESULT result      = RESULT.OK;
            IntPtr      channelraw;
            Channel     channelnew  = null;

            if (channel != null)
            {
                channelraw = channel.getRaw();
            }
            else
            {
                channelraw  = new IntPtr();
            }

            try
            {
                result = FMOD_System_PlaySound(systemraw, channelid, sound.getRaw(), (paused ? 1 : 0), ref channelraw);
            }
            catch
            {
                result = RESULT.ERR_INVALID_PARAM;
            }
            if (result != RESULT.OK)
            {
                return result;
            }

            if (channel == null)
            {
                channelnew = new Channel();
                channelnew.setRaw(channelraw);
                channel = channelnew;
            }
            else
            {
                channel.setRaw(channelraw);
            }
                             
            return result;                                                                    
        }
     
        public RESULT setReverbProperties    (ref REVERB_PROPERTIES prop)
        {
            return FMOD_System_SetReverbProperties(systemraw, ref prop);
        }

        public RESULT setReverbAmbientProperties (ref REVERB_PROPERTIES prop)
        {
            return FMOD_System_SetReverbAmbientProperties(systemraw, ref prop);
        }

        public RESULT set3DListenerAttributes(int listener, ref VECTOR pos, ref VECTOR vel, ref VECTOR forward, ref VECTOR up)
        {
            return FMOD_System_Set3DListenerAttributes(systemraw, listener, ref pos, ref vel, ref forward, ref up);
        }

        #region importfunctions

        [DllImport (VERSION.dll)]
        private static extern RESULT FMOD_System_Release                (IntPtr system);
        [DllImport (VERSION.dll)]
        private static extern RESULT FMOD_System_SetOutput              (IntPtr system, OUTPUTTYPE output);
        [DllImport (VERSION.dll)]
        private static extern RESULT FMOD_System_SetDriver              (IntPtr system, int driver);
        [DllImport (VERSION.dll)]
        private static extern RESULT FMOD_System_Init                   (IntPtr system, int maxchannels, INITFLAGS flags, IntPtr extradriverdata);
        [DllImport (VERSION.dll)]
        private static extern RESULT FMOD_System_Close                  (IntPtr system);
        [DllImport (VERSION.dll)]
        private static extern RESULT FMOD_System_Update                 (IntPtr system);
        [DllImport (VERSION.dll)]                       
        private static extern RESULT FMOD_System_Set3DSettings          (IntPtr system, float dopplerscale, float distancefactor, float rolloffscale);
        [DllImport (VERSION.dll)]
        private static extern RESULT FMOD_System_Set3DListenerAttributes(IntPtr system, int listener, ref VECTOR pos, ref VECTOR vel, ref VECTOR forward, ref VECTOR up);
        [DllImport (VERSION.dll)]
        private static extern RESULT FMOD_System_GetVersion             (IntPtr system, ref uint version);
        [DllImport(VERSION.dll, CharSet = CharSet.Unicode)]
        private static extern RESULT FMOD_System_CreateStream           (IntPtr system, string name_or_data, MODE mode, int exinfo, ref IntPtr sound);   
        [DllImport (VERSION.dll)]
        private static extern RESULT FMOD_System_PlaySound              (IntPtr system, CHANNELINDEX channelid, IntPtr sound, int paused, ref IntPtr channel);
        [DllImport (VERSION.dll)]
        private static extern RESULT FMOD_System_SetReverbProperties    (IntPtr system, ref REVERB_PROPERTIES prop);
        [DllImport (VERSION.dll)]
        private static extern RESULT FMOD_System_SetReverbAmbientProperties(IntPtr system, ref REVERB_PROPERTIES prop);

        #endregion

        #region wrapperinternal
        
        private IntPtr systemraw;

        public void setRaw(IntPtr system)
        {
            systemraw = new IntPtr();

            systemraw = system;
        }

        public IntPtr getRaw()
        {
            return systemraw;
        }

        #endregion
    }
    

    public class Sound
    {
        public RESULT release                 ()
        {
            return FMOD_Sound_Release(soundraw);
        }

        #region importfunctions

        [DllImport (VERSION.dll)]
        private static extern RESULT FMOD_Sound_Release                 (IntPtr sound);
        
        #endregion

        #region wrapperinternal

        private IntPtr soundraw;

        public void setRaw(IntPtr sound)
        {
            soundraw = new IntPtr();
            soundraw = sound;
        }

        public IntPtr getRaw()
        {
            return soundraw;
        }

        #endregion
    }


    public class Channel
    {
        public RESULT stop                  ()
        {
            return FMOD_Channel_Stop(channelraw);
        }
        
        public RESULT setPaused             (bool paused)
        {
            return FMOD_Channel_SetPaused(channelraw, (paused ? 1 : 0));
        }

        public RESULT setVolume             (float volume)
        {
            return FMOD_Channel_SetVolume(channelraw, volume);
        }

        public RESULT isPlaying             (ref bool isplaying)
        {
            RESULT result;
            int p = 0;

            result = FMOD_Channel_IsPlaying(channelraw, ref p);

            isplaying = (p != 0);

            return result;
        }

        public RESULT setReverbProperties   (ref REVERB_CHANNELPROPERTIES prop)
        {
            return FMOD_Channel_SetReverbProperties(channelraw, ref prop);
        }

        public RESULT set3DMinMaxDistance   (float mindistance, float maxdistance)
        {
            return FMOD_Channel_Set3DMinMaxDistance(channelraw, mindistance, maxdistance);
        }

        public RESULT set3DAttributes       (ref VECTOR pos, ref VECTOR vel)
        {
            return FMOD_Channel_Set3DAttributes(channelraw, ref pos, ref vel);
        }

        #region importfunctions

        [DllImport (VERSION.dll)]
        private static extern RESULT FMOD_Channel_Stop                  (IntPtr channel);

        [DllImport (VERSION.dll)]
        private static extern RESULT FMOD_Channel_SetPaused             (IntPtr channel, int paused);

        [DllImport (VERSION.dll)]
        private static extern RESULT FMOD_Channel_SetVolume             (IntPtr channel, float volume);

        [DllImport (VERSION.dll)]
        private static extern RESULT FMOD_Channel_IsPlaying             (IntPtr channel, ref int isplaying);

        [DllImport (VERSION.dll)]
        private static extern RESULT FMOD_Channel_SetReverbProperties   (IntPtr channel, ref REVERB_CHANNELPROPERTIES prop);

        [DllImport (VERSION.dll)]
        private static extern RESULT FMOD_Channel_Set3DMinMaxDistance   (IntPtr channel, float mindistance, float maxdistance);

        [DllImport (VERSION.dll)]
        private static extern RESULT FMOD_Channel_Set3DAttributes       (IntPtr channel, ref VECTOR pos, ref VECTOR vel);

        #endregion
        
        #region wrapperinternal

        private IntPtr channelraw;

        public void setRaw(IntPtr channel)
        {
            channelraw = new IntPtr();

            channelraw = channel;
        }

        public IntPtr getRaw()
        {
            return channelraw;
        }

        #endregion
    }
}
