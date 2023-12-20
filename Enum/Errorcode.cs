public enum ErrorCode
{
    SUCCESS = 0, // Allow code to continue
    NORMAL_EXIT = 1, // Normal exit code
    INPUT_ERROR = 2, // User entered wrong input
    SOURCE_ERROR = 3, // Source File/Folder is inaccessible to the user
    BUSINESS_SOFT_LAUNCHED = 4 // Error if business software is launched while user starts backups
};

public enum Status
{
    TERMINATED = 0, // Stopped or not started
    RUNNING = 1, // Is running
    PAUSED = 2, // Is paused
    WORKBENCH = 3 // Test
}