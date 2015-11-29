MemorySharp - Original Author: Jämes Ménétrey aka ZenLulz.
===================

Hello,

I I really liked the MemorySharp library by ZenLulz. Sadly, it did not contain some items I wanted in a memory library as I started getting more and more into it since I started. Instead of simply use a different library, 

I decided to just work on adding the features in myself. No exes, or dlls are included. You must restore the packages and then compile if you want. For now, I do not intend to release binary's until it is more stable. 

One main feature added was support for people running injected code. A basic WndProc hook, curtiosy of jadds blog: http://blog.ntoskr.nl/hooking-threads-without-detours-or-patches/ , unsafe code reads, etc.

**

Some documentation of new features by example.
-----------------------------

**Examples of added features**
--------------------------

Pattern scanning.
----------------

 
```csharp
                // Our memory sharp instance for the pattern scanning examples below.
                var sharp = new MemorySharp(FromProcessName("ProcessName").First());

                // Standard pattern scan from byte/mask pattern and log the scan result 
                // values to all valid log instances. 

                var scanResult1 = sharp.Patterns.Find(new byte[] {4, 4, 00, 0xC}, "XX?X", 4, false);
                LogManager.Instance.LogInfo(scanResult1.Address + " " + scanResult1.Offset + " " +
                                            scanResult1.OriginalAddress);

                ______________

                // Dword text-based pattern scan from and log the scan result values to 
                // all valid log instances.

                var scanResult2 = sharp.Patterns.Find("55 8b ec 51 FF 05 ?? ?? ?? ?? A1", 0xC, false);
                LogManager.Instance.LogInfo(scanResult2.Address + " " + scanResult2.Offset + " " +
                                            scanResult2.OriginalAddress);
                ______________

                // byte[] array based pattern scan from and log the scan result values 
                // to all valid log instances.

                var scanResult3 = sharp.Patterns.Find(new byte[] {4, 4, 00, 0xC}, 0xC, false);
                LogManager.Instance.LogInfo(scanResult3.Address + " " + scanResult3.Offset + " " +
                                            scanResult3.OriginalAddress);

                ______________

                // Now we will use files to add pattern scan results to a Dictionary.
                // Our patterns to create our json file with.

                var pattern = new SerializablePattern
                              {
                                  Description = "ExamplePattern",
                                  TextPattern = "55 8b ec 51 FF 05 ?? ?? ?? ?? A1",
                                  OffsetToAdd = 0,
                                  RebaseResult = false,
                                  Comments = "This comment is useful when the pattern is stored in a xml or json file."
                              };

                var pattern2 = new SerializablePattern
                               {
                                   Description = "ExamplePattern2",
                                   TextPattern = "55 8b ?? a1 ?? ?? ?? ?? 56 57 8d ?? ?? ??",
                                   OffsetToAdd = 0,
                                   RebaseResult = false,
                                   Comments = "This comment is useful when the pattern is stored in a xml or json file."
                               };

                // Save the objects to a json file as an array so 
                // they can be serialized later.

                JsonHelper.ExportToFile(new[] {pattern, pattern2}, "Patterns.json");

                // Now in your app folder, there should be a Patterns.json file. 
                // We can scan our results for this pattern from this file from now on.

                // This is the dictonary instance we will add the results to.

                var patternResults = new Dictionary<string, IntPtr>();
                sharp.Patterns.CollectJsonScanResults("Patterns.json", patternResults);

                // Now print the pointers found from the pattern scan, 
                // using the description of the pattern as the key.
                Console.WriteLine(patternResults["ExamplePattern"].ToString("X"));
                Console.WriteLine(patternResults["ExamplePattern2"].ToString("X"));

                // You can also save all the results to a text file 
                // in a format you can copy and paste into a class for 
                // static pointers instances.

                // The text file produced should contain the following:
                // public static IntPtr ExamplePattern { get; } = (IntPtr)0x000;
                // public static IntPtr ExamplePattern2 { get; } = (IntPtr)0x000
           
```

Injected code support - Hook WndProc and execute your custom engines code in the main thread.
------------------------------------------------------------------------
```csharp
    public static class HookExample
    {
        private static IWindowEngine _windowEngine;
        private static WindowHook WindowHook { get; set; }

        public static void Attach()
        {
            // This is our custom engine we pass to the window hook class as a ref.
            _windowEngine = new HookEngine();
            // 0x500 should be the handle to the window being hooked. 
            // Should often just be the MainWindowHandle of the process.
            WindowHook = new WindowHook(new IntPtr(0x500), "WndProcHook", ref _windowEngine);
            WindowHook.Enable();
            // This should invoke our HookEngine start up method to run, 
            // printing a console message.
            WindowHook.SendUserMessage(UserMessage.StartUp);
        }

        private class HookEngine : IWindowEngine
        {
            public void StartUp() => WriteLine(@"Hi from process: " + GetCurrentProcess().ProcessName);
            public void ShutDown() => WriteLine(@"Bye from process: " + GetCurrentProcess().ProcessName);
        }
    }
```

Basic three dimensional math structures and extension methods.
--------------------------------------------------------------
```csharp
     // Use extenstion methods to get world to get distances
     // And world to screen values.
     public static float DistanceTo(Vector2 vector2One, Vector2 vector2Two)
            {
                return vector2One.DistanceTo(vector2Two);
            }

            public static Vector2 GetWorldToScreen(Matrix viewMatrix, Vector2 vector2, Vector3 vector3)
            {
                return viewMatrix.WorldToScreen(vector2, vector3);
            }
```

Executing functions inside of a process from external code made slightly easier than before.
------------------------------------------------------------------------
```csharp
   var remoteCall = new RemoteCallParams
                                 {
                                     Address = new IntPtr(0x500),
                                     CallingConvention = CallingConventions.Cdecl
                                 };
                var sharp = new MemorySharp(FromProcessName("ProcessName").First());
                // Execute the function at 0x500, as a cdecl function with 1 param,
                // the player id  
                // int __cdecl  ExampleFunction(int playerId);
                // Lets just say the function returns a value, like the 
                // life of a player. 
                // We will jsut use 500 as the player id here.
                var playersLife = sharp.Execute<int>(remoteCall, 500);
                Console.WriteLine(playersLife);
```

Updater classes - Threaded updater.
-----------------------------------
```csharp
        // Using the class below is as simple as:
        var updaterExample = new MyUpdaterExample("Example", 2500);
        updaterExample.OnUpdate += UpdaterExample_OnUpdate;
        updaterExample.Enable();
		// Handle update events.
        private static void UpdaterExample_OnUpdate(object sender, ThreadedUpdater.DeltaEventArgs e)
            {
                WriteLine(@"Detected the update event.");
            }

    // The updater used above.
    public class MyUpdaterExample : ThreadedUpdater
    {
        // Inherits from the threaded updater class.
        public MyUpdaterExample(string name, int updateRateMs) : base(name, updateRateMs)
        {
        }

        public override void OnUpdateEvent(DeltaEventArgs e)
        {
            WriteLine(@"Hi. Doing stuff here.");
            // Raise the OnUpdate event.
            base.OnUpdateEvent(e);
        }

        private void UpdaterExample_OnUpdate(object sender, DeltaEventArgs e)
        {
            WriteLine(@"Detected the update event.");
        }
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


