# MemorySharp
## MemorySharp's author and information ##
This project is a modifed(by lolp1) version of ZenLulz the original MemorySharp library, to whom should get all credits for his nice work. Although currently not actively being updated by him, you can find his information here:
http://binarysharp.com/
https://github.com/ZenLulz

**This version of MemorySharp information**
The reason for making modifications and changes to the original library are mainly for me to learn as a beginner, and also to provide some useful functionality that can range from syntax sugar and minor convenience to full implementation of useful new features.

Basic Features Added
------------------------------------------------------------------------
 **Support for 'in-process' Memory operations, aka injected - see MemoryPlus.cs for most details**  
 

 - GreyMagics Detour Manager added.
 - GreyMagics Patch Manager added.
 - `UnsafeMarshalType<T>` - Internal Marshaling.
 - Generic WndProc hook - just supply the `IWindowEngine` Interface and window handle.
 - IntPtr/Int/Uint/long extension methods for quick and simple internal  reading/writing memory.
 - Static class for basic Internal memory operations.
 - Misc. class objects such as `ProcessFunction<T>` and `VirtualClass`
 
**Misc features for the library added**
 
   
 - Pattern scanning - supporting various formats including patter scanning from xml files.
 - Some x64 bit Peb data support.
 - 3D math structures and utilities
 - Generic useful extension methods and helper classes.
 - Basic ILog Interface and default implementation for easy and effective logging.
 - Generic plugin interface and default implementation.

A few examples of features added being used
--------------

**Pattern scanning from an xml file**
```csharp
    var sharp = new MemorySharp("ProcessName");
    Dictionary<string, IntPtr> myPatterns = new Dictionary<string, IntPtr>(); 
    // Fill the dictionary instance with scanned pattern results.
    sharp.Patterns.CollectXmlPatternScanResults("Patterns.xml", myPatterns)
    foreach (var patternScanResult in myPatterns)
		LogManager.LogInfo(patternScanResult.Address + " " patternScanResult.Offset);
```
**Traditonal Pattern scans**
```csharp
        var sharp = new MemorySharp("ProcessName");
        byte[] myPatternBytes = {1, 2, 3,};
        string myPatternMask = "x?x";
        int offsetLoctedAt = 5;
        bool isOffsetMode = false;
        bool rebaseResult = false;
        ScanResult result = sharp.Patterns.Find(myPatternBytes, myPatternMask, offsetLoctedAt, isOffsetMode,
            rebaseResult);
        Console.WriteLine(result.Address + " " +result.Offset + " " result.OriginalAddress);
```
**From text format:**
```csharp
	    var sharp = new MemorySharp("ProcessName");
        string myPattern = "55 8B EC";
        int offset = 5;
        bool isOffsetResult = false;
        bool rebase = false;
        ScanResult result = sharp.Patterns.Find(myPattern, offset, isOffsetResult, rebase);
        Console.WriteLine(result.Address + " " +result.Offset + " " + result.OriginalAddress);
```
**WndProc hook**
 ```csharp

    public class Example : IWindowEngine
        {           
            public void StartUp()
            {
                MessageBox.Show(@"Hi from " + Process.GetCurrentProcess().ProcessName);
            }
    
            public void ShutDown()
            {
                MessageBox.Show(@"Shutting down.");
            }
        }
	        private static WindowHook _wndProcHook;
            private static IntPtr _myGameWindowHandle;
            private static IWindowEngine _windowEngine;
            public static void Test()
            {
                _myGameWindowHandle = new IntPtr(500);
                _windowEngine = new Example();
                _wndProcHook = new WindowHook("HookName", _myGameWindowHandle, _windowEngine);
                _wndProcHook.Enable();
                _wndProcHook.InvokeUserMessage(UserMessage.StartUp);
                _wndProcHook.InvokeUserMessage(UserMessage.ShutDown);
                _wndProcHook.Disable();
            }
```
**Reading values with 3DMath structures and helpers**
```csharp
			    // Get the location of an in-game object and comparing a distance
                var sharp = new MemorySharp("ProcessName");
                IntPtr objectOneXyz = new IntPtr(500);
                IntPtr objectTwoXyz = new IntPtr(500);
    
                float[] xyz = sharp.ReadArray<float>(objectOneXyz, 3);
                float[] otherXyz = sharp.ReadArray<float>(objectTwoXyz, 3);
    
                Vector3 vector3 = new Vector3(xyz);
                Vector3 vector3Two = new Vector3(otherXyz);
               
                Console.WriteLine(vector3.DistanceTo(vector3Two));
```
World To Screen with extension methods:
  
```csharp
		        int matrixRows = 1;
                int matrixCocolumns = 1;
                Matrix viewMatrix = new Matrix(matrixRows, matrixCocolumns);
                Vector2 screenSize = new Vector2(500,500);
                Vector3 pointToConvert = new Vector3(1,1,1);
                Vector2 clientCordinates = viewMatrix.WorldToScreen(screenSize, pointToConvert);
                Console.WriteLine(clientCordinates.X + " " + clientCordinates.Y);
```
**Anyways, that should be enough to get you started peaking around the library if you like the original MemorySharp like a beginner like myself does :).

**Credits**
-------
I can't possible remember all of them - but I fully support giving all credits to the proper people for any code used. I put together this lib mostly from writing out other peoples code by hand and then implementing them in my application to learn how stuff works, so I missed a lot I am sure. Simply message me if you want to be added if I stole your code ^_^.

**People**
----------

 - ZenLulz for MemorySharp
 - Jadd @ ownedcore.
 - Zat @ unknowncheats for his ExternalUtilsCSharp - where the math/updated classes mostly come from.
 - aganonki @ unknowncheats for spoon feeding all my noob questions and cool ideas like the VirtualClass.
 - Apoc for his Detour/Patch/MarshalCache 
 - jeffora for his cool extension methods for internal reads, and his Marshaling examples.
 - Torpedos @ ownedcore for giving me really solid advice in general.
 - More..

**

**Sites**
-----
www.blizzhackers.cc
www.ownedcore.com
www.unknowncheats.me
https://github.com/jeffora/extemory/tree/master/src/Extemory
https://github.com/BigMo/ExternalUtilsCSharp
https://github.com/aganonki/HackTools
https://github.com/miceiken/IceFlake (Apocs GreyMagic is here as well)
https://github.com/Dramacydal/DirtyDeeds/tree/master/DirtyDeeds - cool hack and great learning material.
http://blog.ntoskr.nl/hooking-threads-without-detours-or-patches/ - WndProc hook blog by Jadd. 
