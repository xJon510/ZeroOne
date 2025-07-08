using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using UnityEngine.UI;

public class BootManager : MonoBehaviour
{
    private bool skipRequested = false;

    [Header("UI References")]
    public CanvasGroup bootScreenUI; // Terminal output screen
    public CanvasGroup titleScreenUI; // Main menu

    public TextMeshProUGUI consoleText;

    public ScrollRect terminalScrollRect;

    string[] normalLines = new string[]
{
    "[INFO] Initializing system components and verifying configurations for optimal performance...",
    "[INFO] Loading kernel modules and establishing secure communication channels with hardware interfaces...",
    "[INFO] Checking filesystem integrity across all mounted volumes to ensure data consistency and reliability...",
    "[INFO] Synchronizing system clock with network time protocol servers to maintain accurate timekeeping...",
    "[INFO] Allocating memory resources for user-space applications and optimizing virtual memory management...",
    "[INFO] Establishing network connections and negotiating protocols for seamless data transmission...",
    "[INFO] Verifying user authentication credentials and applying security policies to enforce access controls...",
    "[INFO] Scanning for connected peripheral devices and loading appropriate drivers for compatibility...",
    "[INFO] Configuring display settings and initializing graphical user interface components for user interaction...",
    "[INFO] Performing system health checks and logging diagnostic information for maintenance purposes...",
    "[INFO] Starting background services and daemons to support system operations and user applications...",
    "[INFO] Monitoring system resource usage and adjusting priorities to maintain optimal performance levels...",
    "[INFO] Checking for software updates and applying patches to address known vulnerabilities and bugs...",
    "[INFO] Cleaning up temporary files and freeing disk space to ensure sufficient storage availability...",
    "[INFO] Encrypting sensitive data and securing communication channels to protect against unauthorized access...",
    "[INFO] Compiling system logs and generating reports for administrative review and analysis...",
    "[INFO] Validating configuration files and applying settings to customize system behavior as per user preferences...",
    "[INFO] Establishing firewall rules and monitoring network traffic to prevent unauthorized intrusions...",
    "[INFO] Performing routine maintenance tasks and scheduling automated jobs for system upkeep...",
    "[INFO] Verifying hardware components and running diagnostics to detect potential issues or failures...",
    "[INFO] Initializing audio subsystems and configuring sound settings for multimedia applications...",
    "[INFO] Setting up user environments and loading personalized settings for a customized experience...",
    "[INFO] Monitoring battery status and optimizing power consumption for energy efficiency...",
    "[INFO] Establishing secure shell connections and authenticating remote access requests...",
    "[INFO] Parsing command-line arguments and executing scripts as per user instructions...",
    "[INFO] Compiling source code and resolving dependencies for software installations...",
    "[INFO] Performing disk defragmentation and optimizing file storage for faster data retrieval...",
    "[INFO] Checking environmental sensors and adjusting system cooling mechanisms as needed...",
    "[INFO] Validating digital signatures and verifying the integrity of downloaded files...",
    "[INFO] Establishing virtual private network connections and encrypting data transmissions...",
    "[INFO] Monitoring system logs for unusual activity and triggering alerts for potential security incidents...",
    "[INFO] Configuring network interfaces and assigning IP addresses for seamless connectivity...",
    "[INFO] Initializing database services and ensuring data consistency across distributed systems...",
    "[INFO] Performing garbage collection and optimizing memory usage for application performance...",
    "[INFO] Checking for hardware firmware updates and applying patches to enhance functionality...",
    "[INFO] Establishing Bluetooth connections and pairing with nearby devices for data exchange...",
    "[INFO] Monitoring system temperature and adjusting fan speeds to prevent overheating...",
    "[INFO] Validating user input and sanitizing data to prevent injection attacks...",
    "[INFO] Configuring email services and synchronizing messages across devices...",
    "[INFO] Performing system backups and verifying the integrity of archived data...",
    "[INFO] Checking disk quotas and enforcing storage limits for user accounts...",
    "[INFO] Initializing virtualization environments and allocating resources for virtual machines...",
    "[INFO] Monitoring network latency and adjusting routes for optimal data transmission...",
    "[INFO] Configuring print services and managing print queues for document processing...",
    "[INFO] Establishing secure connections to cloud services and synchronizing data...",
    "[INFO] Parsing log files and extracting relevant information for troubleshooting...",
    "[INFO] Performing integrity checks on system binaries and libraries to detect tampering...",
    "[INFO] Configuring proxy settings and routing network traffic through secure channels...",
    "[INFO] Monitoring file system changes and triggering events for real-time updates...",
    "[INFO] Validating SSL certificates and establishing trust chains for secure communications...",
    "[INFO] Checking system uptime and logging metrics for performance analysis...",
    "[INFO] Configuring keyboard layouts and input methods for multilingual support...",
    "[INFO] Monitoring disk I/O operations and optimizing read/write speeds for efficiency...",
    "[INFO] Establishing connections to directory services and authenticating user credentials...",
    "[INFO] Parsing configuration templates and generating files for system services...",
    "[INFO] Performing load balancing and distributing network traffic across multiple servers...",
    "[INFO] Checking for orphaned processes and terminating zombie tasks to free resources...",
    "[INFO] Configuring network shares and setting permissions for collaborative access...",
    "[INFO] Monitoring system entropy and replenishing random number pools for cryptographic operations...",
    "[INFO] Validating API endpoints and ensuring compatibility with client applications...",
    "[INFO] Configuring environmental variables and setting paths for executable files...",
    "[INFO] Performing checksum verifications and detecting data corruption in files...",
    "[INFO] Establishing multicast groups and managing subscriptions for data distribution...",
    "[INFO] Monitoring process lifecycles and restarting services upon unexpected termination...",
    "[INFO] Configuring DNS settings and resolving domain names to IP addresses...",
    "[INFO] Parsing XML configurations and applying settings to system components...",
    "[INFO] Performing stress tests on hardware components to assess reliability under load...",
    "[INFO] Monitoring network bandwidth usage and identifying potential bottlenecks...",
    "[INFO] Configuring access control lists and enforcing security policies on resources...",
    "[INFO] Validating JSON data structures and ensuring compliance with schemas...",
    "[INFO] Establishing connections to message brokers and subscribing to event channels...",
    "[INFO] Monitoring system call activity and detecting anomalies in application behavior...",
    "[INFO] Configuring cron jobs and scheduling tasks for automated execution...",
    "[INFO] Performing latency measurements and optimizing response times for services...",
    "[INFO] Checking for open ports and securing unused services to reduce attack surface...",
    "[INFO] Configuring logging levels and directing output to appropriate log files...",
    "[INFO] Monitoring cache utilization and purging stale entries to free memory...",
    "[INFO] Establishing connections to remote repositories and synchronizing codebases...",
    "[INFO] Validating input forms and providing feedback for user data entry...",
    "[INFO] Configuring session timeouts and managing user sessions for security...",
    "[INFO] Monitoring thread activity and detecting deadlocks in multithreaded applications...",
    "[INFO] Performing data deduplication and optimizing storage usage across volumes...",
    "[INFO] Checking for broken symbolic links and repairing file system references...",
    "[INFO] Configuring load order of services and managing dependencies during startup...",
    "[INFO] Monitoring CPU utilization and throttling processes to prevent resource starvation...",
    "[INFO] Establishing connections to authentication servers and validating tokens...",
    "[INFO] Parsing command history and suggesting auto-completions for user convenience...",
    "[INFO] Performing network scans and identifying active hosts on the local subnet...",
    "[INFO] Configuring swap space and managing virtual memory paging operations...",
    "[INFO] Monitoring file descriptors and preventing exhaustion of system resources...",
    "[INFO] Validating configuration syntax and highlighting errors for correction...",
    "[INFO] Establishing connections to time servers and synchronizing system clocks...",
    "[INFO] Parsing environment configurations and applying overrides for development settings...",
    "[INFO] Performing data compression and reducing file sizes for efficient storage...",
    "[INFO] Monitoring system interrupts and handling signals for process control...",
    "[INFO] Configuring user permissions and enforcing least privilege access models...",
    "[INFO] Validating command-line options and providing usage information for utilities...",
    "[INFO] Establishing connections to telemetry services and reporting system metrics...",
    "[INFO] Parsing log rotation policies and archiving old logs for historical reference..."
};

    private string[] oneLiners = 
        { 
        "Grabbing all sensitive files...", 
        "Decrypting user secrets...", 
        "Installing keylogger modules...", 
        "Accessing webcam... SUCCESS", 
        "Spoofing MAC address...", 
        "Backdooring operating system...", 
        "Extracting encryption keys...", 
        "Root privileges granted.", 
        "Initiating remote control protocol...", 
        "Sending data to unknown IP...", 
        "Crypto mining enabled.", 
        "Optimizing quantum bitstream...", 
        "Injecting XOR payload...", 
        "Calibrating L3 cache filters...", 
        "Allocating heapspace... 95%", 
        "Unrolling loops... done", 
        "Normalizing neural subroutines...", 
        "Faking terminal authenticity...", 
        "Flipping Schrödinger’s bit...", 
        "Locating the mainframe...", 
        "Engaging turbo fan RPM...", 
        "Legally distinct software initialized.", 
        "You didn’t read the license, did you?", 
        "Totally not malware.", 
        "For legal reasons, this is a joke.", 
        "We respect your privacy.*", 
        "* Terms and conditions apply.", 
        "Checking system clock... wrong, but okay.", 
        "Boot time within expected limits.", 
        "Now running faster than your GPA.", 
        "You shouldn’t have launched this.", 
        "Are you alone right now?", 
        "He’s watching.", 
        "We’ve been waiting.", 
        "Access level: TERMINAL", 
        ">_>   <_<   >_>", 
        "This is real.", 
        "Exit disabled.", 
 };

    [Header("Timing Settings")]
    public float normalLineDelay = 0.1f;
    public float oneLinerDelay = 0.5f;
    public float bootDuration = 5f;

    [Header("Post Boot")]
    public GameObject Bloom;
    public Texture2D defaultCursorTexture;

    [Header("Audio")]
    public MusicManager musicManager;

    private Vector2 hotspot = Vector2.zero;

    private List<string> bootQueue = new List<string>();

    private void Start()
    {
        // Check if we should skip the boot
        if (PlayerPrefs.GetInt("SkipBoot", 0) == 1)
        {
            // Clear the flag for future clean boots
            PlayerPrefs.SetInt("SkipBoot", 0);
            PlayerPrefs.Save();

            // Instantly go to title screen (no animation)
            SkipToTitleImmediate();
            return;
        }

        // Clear console
        consoleText.text = "";
        // Set borderless windowed at 960x540
        Screen.SetResolution(960, 540, false);
        Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
        // Start the boot sequence
        StartCoroutine(BootSequence());
    }

    private void Update()
    {
        if (Input.anyKeyDown && !skipRequested)
        {
            skipRequested = true;
            StopAllCoroutines();
            StartCoroutine(SkipToTitle());
        }
    }

    IEnumerator BootSequence()
    {
        float elapsed = 0f;
        int normalLineCount = 0;

        GenerateBootQueue();

        foreach (string line in bootQueue)
        {
            // Stop if we've gone too long
            if (elapsed >= bootDuration)
                break;

            consoleText.text += line + "\n";
            ForceScrollToBottom();

            if (line.StartsWith("[SYS]")) // Assuming one-liners start like this
            {
                yield return new WaitForSeconds(oneLinerDelay);
                elapsed += oneLinerDelay;
            }
            else
            {
                yield return new WaitForSeconds(normalLineDelay);
                elapsed += normalLineDelay;
                normalLineCount++;
            }
        }

        yield return new WaitForSeconds(0.5f);

        // Transition to title screen
        bootScreenUI.alpha = 0;
        bootScreenUI.interactable = false;
        bootScreenUI.blocksRaycasts = false;

        titleScreenUI.alpha = 1;
        titleScreenUI.interactable = true;
        titleScreenUI.blocksRaycasts = true;

        // Go fullscreen
        Screen.fullScreen = true;
        Bloom.SetActive(true);
        SetDefaultCursor();

        LogInitializer.InitializeAllLogs();
        Debug.Log("Calling PlayFirstSong()");
        musicManager?.PlayFirstSong();
    }

    void GenerateBootQueue()
    {
        List<string> combined = new List<string>();

        for (int i = 0; i < 50; i++) // Arbitrary large loop; bootDuration will cap it
        {
            // Add normal line
            string normal = normalLines[UnityEngine.Random.Range(0, normalLines.Length)];
            combined.Add(normal);

            // Every 5 lines, add a one-liner
            if ((i + 1) % 5 == 0 && oneLiners.Length > 0)
            {
                string oneLiner = oneLiners[UnityEngine.Random.Range(0, oneLiners.Length)];
                combined.Add("[SYS] " + oneLiner);
            }
        }

        // Shuffle the whole set for randomness
        Shuffle(combined);
        bootQueue = combined;
    }

    void Shuffle(List<string> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int k = UnityEngine.Random.Range(0, i + 1);
            var temp = list[i];
            list[i] = list[k];
            list[k] = temp;
        }
    }

    void ForceScrollToBottom()
    {
        StartCoroutine(DelayedScroll());
    }

    IEnumerator DelayedScroll()
    {
        yield return null;
        Canvas.ForceUpdateCanvases();
        terminalScrollRect.verticalNormalizedPosition = 0f;
        Canvas.ForceUpdateCanvases();
    }

    private void SetDefaultCursor()
    {
        if (defaultCursorTexture != null)
        {
            Cursor.SetCursor(defaultCursorTexture, hotspot, CursorMode.Auto);
        }
        else
        {
            UnityEngine.Debug.LogWarning("Default cursor texture not assigned.");
        }
    }

    IEnumerator SkipToTitle()
    {
        consoleText.text += "\n\n[SYS] Boot sequence manually skipped.\n";

        yield return new WaitForSeconds(1f); // slight delay for clarity

        bootScreenUI.alpha = 0;
        bootScreenUI.interactable = false;
        bootScreenUI.blocksRaycasts = false;

        titleScreenUI.alpha = 1;
        titleScreenUI.interactable = true;
        titleScreenUI.blocksRaycasts = true;

        Screen.fullScreen = true;
        Bloom.SetActive(true);
        SetDefaultCursor();

        LogInitializer.InitializeAllLogs();
        Debug.Log("Calling PlayFirstSong()");
        musicManager?.PlayFirstSong();
    }

    private void SkipToTitleImmediate()
    {
        bootScreenUI.alpha = 0;
        bootScreenUI.interactable = false;
        bootScreenUI.blocksRaycasts = false;

        titleScreenUI.alpha = 1;
        titleScreenUI.interactable = true;
        titleScreenUI.blocksRaycasts = true;

        Screen.fullScreen = true;
        Bloom.SetActive(true);
        SetDefaultCursor();

        LogInitializer.InitializeAllLogs();
    }
}
