# Capture Crash Dump by procdump
1. Download [procdump](https://learn.microsoft.com/en-us/sysinternals/downloads/procdump)

1. Run **command prompt** as Administrator and navigate to procdump.

    ![](../images/ManualDump1.png)

1. Run the cmdlet and wait issue happen again.
    ```bash
    procdump -accepteula -e -ma -w w3wp.exe
    ```
    ![](../images/crashdump1.png)