MemorySharp - Original Author: Jämes Ménétrey aka ZenLulz.
===================

Hello,

I I really liked the MemorySharp library by ZenLulz. Sadly, it did not contain some items I wanted in a memory library as I started getting more and more into it since I started. Instead of simply use a different library, 

I decided to just work on adding the features in myself. No exes, or dlls are included. You must restore the packages and then compile if you want. For now, I do not intend to release binary's until it is more stable. 

One main feature added was support for people running injected code. A basic WndProc hook, curtiosy of jadds blog: http://blog.ntoskr.nl/hooking-threads-without-detours-or-patches/ , unsafe code reads, etc.

**

Some documentation of new features by example.
-----------------------------

* Executing internal game functions using assembly injection and CreateRemoteThread with ease with MemorySharps assembly wrappers and the ``` RemoteCall``` class.
```csharp
          public static class ExampleCalls
          {
            // This will assemble the asm code needed, alloc/inject it in the remote process, then execute it using                  
            // CreateRemoteThread.
            // This is a simple no parameter function called.
            public static IntPtr LocalPlayerPointer => MemorySharp.Execute<IntPtr>(GetLocalPlayerPointer);
            
            // Calling functions with parameters are also possible/easy thanks to ZenLulz wrappers.
            public static void TryMoveToLocation(Vector2 vector2Location)
            {
                MemorySharp.Execute(MoveToLocation, vector2Location.X, vector2Location.Y);
            }                         

            // Private values.
            private static MemorySharp MemorySharp { get; } = new MemorySharp(FromProcessName("GameProcessName").First());

            // A function we will pretends is a cdecl function with no params and returns an IntPtr to the local player object.
            private static RemoteCallParams GetLocalPlayerPointer { get; } = new RemoteCallParams
            {
                Address = (IntPtr)0x500,
                CallingConvention = CallingConventions.Cdecl
            };

            // A function we will pretends is a cdecl function with some params to take to move the char some where. 
            private static RemoteCallParams MoveToLocation{ get; } = new RemoteCallParams
            {
                Address = (IntPtr)0x500,
                CallingConvention = CallingConventions.Cdecl
            };
        }

```
* Pattern scanning examples.

**Pattern Scan xml/json file examples:**

Json:
```Json
     {
        "Description": "GameState",
        "TextPattern": "80 3d ?? ?? ?? ?? ?? 74 ?? 50 b9 ?? ?? ?? ?? e8 ?? ?? ?? ?? 85 c0 74 ?? 8b 40 08 83 f8 02 74 ?? 83 f8 01 75 ?? b0 01 c3 32 c0 c3",
        "OffsetToAdd": 2,
        "IsOffsetMode": false,
        "RebaseAddress": false,
        "Comments": null
      },
      {
        "Description": "LocalPlayer",
        "TextPattern": "80 3d ?? ?? ?? ?? ?? 74 ?? 50 b9 ?? ?? ?? ?? e8 ?? ?? ?? ?? 85 c0 74 ?? 8b 40 08 83 f8 02 74 ?? 83 f8 01 75 ?? b0 01 c3 32 c0 c3",
        "OffsetToAdd": 4,
        "IsOffsetMode": false,
        "RebaseAddress": false,
        "Comments": null
      }
```
Xml:
```XML
    ?xml version="1.0" encoding="utf-16"?>
    <ArrayOfSerializablePattern xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema">
      <SerializablePattern>
        <Description>GameState</Description>
        <TextPattern>80 3d ?? ?? ?? ?? ?? 74 ?? 50 b9 ?? ?? ?? ?? e8 ?? ?? ?? ?? 85 c0 74 ?? 8b 40 08 83 f8 02 74 ?? 83 f8 01 75 ?? b0 01 c3 32 c0 c3</TextPattern>
        <OffsetToAdd>2</OffsetToAdd>
        <IsOffsetMode>false</IsOffsetMode>
        <RebaseAddress>false</RebaseAddress>
      </SerializablePattern>
      <SerializablePattern>
        <Description>LocalPlayer</Description>
        <TextPattern>80 3d ?? ?? ?? ?? ?? 74 ?? 50 b9 ?? ?? ?? ?? e8 ?? ?? ?? ?? 85 c0 74 ?? 8b 40 08 83 f8 02 74 ?? 83 f8 01 75 ?? b0 01 c3 32 c0 c3</TextPattern>
        <OffsetToAdd>2</OffsetToAdd>
        <IsOffsetMode>false</IsOffsetMode>
        <RebaseAddress>false</RebaseAddress>
      </SerializablePattern>
    </ArrayOfSerializablePattern>
```

**Using the patterns and pattern files:**
  
```csharp
      public static class PatternScanExamples
        {
            // Collect an entire pattern file json or xml to any dictionary using the IDictionary interface.
            public static Dictionary<string, IntPtr> GetPointerDictionary()
            {
                var sharp = new MemorySharp(ApplicationFinder.FromProcessName("GameProcessNames").First());
                var results = new Dictionary<string, IntPtr>();
                sharp.Patterns.CollectJsonScanResults("Patterns.json", results);
                return results;
            }

            // More standard byte-mask pattern.
            public static void PrintByteMaskScanResult()
            {
                var sharp = new MemorySharp(ApplicationFinder.FromProcessName("GameProcessNames").First());
                var result = sharp.Patterns.Find(new byte[] { 0X1C, 0X2C, 0X3C, 00, 00, 00, 0X7C, 0x8C, 0x9C }, "xxx???xxx", 0xC, false, false);
                Console.WriteLine(result.Address.ToString("X"));
                Console.WriteLine(result.OriginalAddress.ToString("X"));
            }
            // Print a text based pattern scan result.
            public static void PrintTextPatternScanResult()
            {
                var sharp = new MemorySharp(ApplicationFinder.FromProcessName("GameProcessNames").First());
                var textBasedPattern =
                    "80 3d ?? ?? ?? ?? ?? 74 ?? 50 b9 ?? ?? ?? ?? e8 ?? ?? ?? ?? 85 c0 74 ?? 8b 40 08 83 f8 02 74 ?? 83 f8 01 75 ?? b0 01 c3 32 c0 c3";
                // Add 0xC to the found pattern offset, the result is a full address not an offset style pattern, and the address should not be rebased for the scan result.
                var scanResult = sharp.Patterns.Find(textBasedPattern, 0xC, false, false);
                Console.WriteLine(scanResult.Address.ToString("X"));
                Console.WriteLine(scanResult.OriginalAddress.ToString("X"));
            }

            // Create a file log of scanned patterns address results in neat formats.
            public static void GeneratePointerFile()
            {
                var sharp = new MemorySharp(ApplicationFinder.FromProcessName("GameProcessNames").First());
                // Log formatted pointers ready for C# code use to a text file using the data in the xml pattern file.
                sharp.Patterns.LogScanResultsToFile("Patterns.xml", PatternFileType.Xml);
                // This does the same as above, but for json.
                sharp.Patterns.LogScanResultsToFile("Patterns.json", PatternFileType.Json);
                // Now the applications folder should contain a log with all the addresses found from scans. Format:
                // public IntPtr [Description] {get;} = (IntPtr)0x00;
            }
        }

```
Injected Support added.
-----------------------

Hook WndProc and pass your own call back engine easily:
```csharp
     public class HookEngine : IWindowEngine
        {
            public void StartUp() => WriteLine(@"Hi from process: " + GetCurrentProcess().ProcessName);
            public void ShutDown() => WriteLine(@"Bye from process: " + GetCurrentProcess().ProcessName);
        }
    
        public static class HookExample
        {
            private static IWindowEngine _windowEngine;
            private static WindowHook WindowHook { get; set; }
            public static void Attach()
            {
                // This is our custom engine we pass to the window hook class as a ref.
                _windowEngine = new HookEngine();
                // 0x500 should be the handle to the window being hooked. Should often just be the MainWindowHandle of the process.
                WindowHook = new WindowHook(new IntPtr(0x500),"WndProcHook",ref _windowEngine);
                WindowHook.Enable();
                // This should invoke our HookEngine start up method to run, printing a console message.
                WindowHook.SendUserMessage(UserMessage.StartUp);
            }
```
Using IntPtr Extensions for internal reading.
---------------------------------------------
```csharp
    public static class InjectedDelegatesReadWriteExamples
        {
            private static GetObjectLocation InternalGetObjectLocation { get; set; }

            // Read/Write memory with IntPtr extensions while injected.
            public static void PrintAndWriteAddresS()
            {
                var pointer = new IntPtr(0x500).Read<IntPtr>();
                Console.WriteLine(pointer.ToString("X"));
                // Write to it.
                pointer.WriteBytes(new byte[] {1, 2, 3});
                pointer.WriteString("Hi", Encoding.UTF8);
                pointer.Write("Use a type of object here, not a string. Use write string for that.");
            }

            // Use delegates with extensions easier. This is the delegate.
            [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
            private delegate void GetObjectLocation(IntPtr objectPointer, out Vector3 location3D);
            // Register it.
            public static void RegisterGetObjectLocationDelegate() => InternalGetObjectLocation = new IntPtr(0x500).ToDelegate<GetObjectLocation>();
            // Using it.
            public static Vector3 MyLocation
            {
                get
                {
                    Vector3 myVector3;
                    InternalGetObjectLocation(new IntPtr(0x500), out myVector3);
                    return myVector3;
                }
            }
        }
```
Tools, logging, updaters, misc features added examples.
---------------------------------------------

* Updaters (thanks to Zatt @ unknowncheats and aganonki as well from there).


```csharp
    // Credits: Zatt @ unknowncheats.me
    public class MyUpdaterExample
    {
        // 2500 Interval updater.
        private static ThreadedUpdater Updater { get; } = new ThreadedUpdater("ExampleConsoleWriter", 2500);
        
       public static void StartUpdating()
            {
                Updater.Enable();
                // Console should get lines every 2500 ms now.
                Thread.Sleep(5000);
                // Turn it off now.
                Updater.Disable();
            }

            private void Updater_OnUpdate(object sender, ThreadedUpdater.DeltaEventArgs e)
            {
               Console.WriteLine("Hi, we were called.");
            }
        }
```
3D Math (Credits: Zatt).
------------------------
* WorldToScreen
 
```csharp
        public static Vector2 GetWorldToScreen(Matrix viewMatrix, Vector2 vector2, Vector3 vector3)
            {
               return viewMatrix.WorldToScreen(vector2, vector3);
            }
```   
      
 
 * Distance to.            
```csharp
public static float DistanceTo(Vector2 vector2One, Vector2 vector2Two)
 {
  return vector2One.DistanceTo(vector2Two);
 }
```


Credits for MemorySharp - Note: All credits for the base of this library go to Jämes Ménétrey aka ZenLulz. 
------------------------------------------------------------------------

Credits - please note, I support all original authors work fully. Most of the additons here come from disecting others code and making it fit here. Pst me if I missed you and you would enjoy some credits.


**--- Direct code useage credits, there is likely far more. Did the major ones I can remember for now, message me as suggested above if you're missing and want credits. ---**
Zatt @ unknowncheats.
GitHub: https://github.com/BigMo
Major code used: Updater classes, 3D math releated classes.
Project suggestion of his to check out: https://github.com/BigMo/ExternalUtilsCSharp


Jadd @ ownedcore.
GitHub: Unknown.
Major code used: WndProc hook example - http://blog.ntoskr.nl/hooking-threads-without-detours-or-patches/
Project suggestions of his to check out: Monitor the blog? :).

aganonki @ unknowncheats.
GitHub: https://github.com/aganonki
Major code used: TimedUpdater class.
Project suggestions of his to check out: https://github.com/aganonki/HackTools
Note: He helped me a LOT with non-direct code help, just helping me understand basic oop concepts.

aevitas @ unknowncheats?
GitHub: https://github.com/aevitas
Major code used: A lot of references to his external memory classes, and more. https://github.com/aevitas/bluerain/blob/master/src/BlueRain/ExternalProcessMemory.cs
Project suggestions of his to check out: https://github.com/aevitas/orion

Apoc @ ownedcore
GitHub: Unknown.
Major code used: Pretty much 1:1 rip of his memory library's 'GreyMagic' patch and detour classes.
Project suggestion of his to check out: Google if you want :) too many.


miceiken @ ownedcore
GitHub: https://github.com/miceiken
Major code used: His IceFlake's project Pulse engines and action queue systems.
Project suggestions of his to check out: Great start on injected code - https://github.com/miceiken/IceFlake

jeffora @ ownedcore?
GitHub: https://github.com/jeffora
Major code used: https://github.com/jeffora/extemory extension methods ripped 1:1 almost in some cases.
Project suggestions of his to check out: https://github.com/jeffora/extemory - cool memory library that is not known well.
--- End ---


**--- Helpful people credits ---**

Torpedos @ ownedcore.
Replied to lots of noob questions, gave me very solid tips in terms of going about learning and using code in relation to game hacking.


Corthezz @ ownedcore. 
Good blog for starters who are stuck in external land or have not messed with assembly calling of functions or what ever much yet.,

LordTerror @ Unknown.
A friend who helped me understand hacking concepts a LOT. I do not think he uses forums these days at all, but might have some old good reads if you google around.

--- End --- 


**--- Cool Projects to check out ---** 

1. Cool Diablo II hack using hardware breakpoints and remote threads to make the 'external' hack perform in some cool ways - https://github.com/Dramacydal/DirtyDeeds/tree/master/DirtyDeeds
2. Really solid base example of an externally based Object Manager for WoW - https://github.com/Dramacydal/WowMoPObjMgrTest , it also provides some good insight on how it works internally.
3. Not updated anymore - but a cool example of getting started with C# code on modern patches of WoW - https://github.com/unknowndev/CoolFish.


