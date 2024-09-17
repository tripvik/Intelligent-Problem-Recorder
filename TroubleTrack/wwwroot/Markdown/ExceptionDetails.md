# Exception Dumps Using ProcDump

ProcDump does not require installation. But one needs to be specific about the PID to which it is attaching. That PID needs to be determined prior to starting ProcDump. This may be tricky when the respective process is crashing and restarting frequently, with a different PID; such as when Asp.Net apps are causing their w3wp.exe to crash and restart. If the w3wp.exe is crashing very fast, then it is advisable to use the DebugDiag method.

1. **Download** the tool and copy it on a disk folder, for example ***D:\Temp-Dumps\\***
   *<https://docs.microsoft.com/en-us/sysinternals/downloads/procdump>*

2. Open an **administrative console** from where to run commands.
   Navigate to the disk folder above (***D:\Temp-Dumps\\***).

3. **Find the process ID**, the **PID**, of the IIS w3wp.exe worker process executing your application.
   Use the AppCmd IIS tool to list processes for application pools:
   ```
   D:\Temp-Dumps> C:\Windows\System32\InetSrv\appcmd.exe list wp
   ```

4. **Execute** the following command to collect dump(s):
   ```
   D:\Temp-Dumps> procdump.exe -accepteula -ma -e 1 -f "*ExceptionNameOrCodeOrMessageExcerpt*" [PID]
   ```
   You may want to redirect the console output of ProcDump to a file, to persist the recording of the encountered exceptions:
   ```
   D:\Temp-Dumps> procdump.exe -accepteula -ma -e 1 -f "*ExceptionNameOrCodeOrMessageExcerpt*" [PID] > Monitoring-log.txt
   ```
   Replace [PID] with the actual Process ID integer number identified at step 2.
   
   Please make sure that there is enough disk space on the drive where dumps are collected. Each process dump will take space in the disk approximately the same size the process uses in memory (column Commit Size in Task Manager). For example, if the process' memory usage is ~1 GB, then the size of a dump file will be around 1 GB.

5. **Start reproducing** the problem: issue a request from the client (browser) that you know it would trigger the exception.
   **Or simply wait** or make requests to the IIS/Asp.Net app until the exception occurs.
   You should end up with a memory dump file (.DMP) in the location where ProcDump.exe was saved (example: ***D:\Temp-Dumps\\***).

6. **Compress** the dump file(s) - .DMP - before uploading the dumps to share for analysis.