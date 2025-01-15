window.downloadFileTest = function (fileName, content) {
    console.log("Downloading file:", fileName);
};
window.startIdleTimer = function (dotNetHelper) {
    let timeout;

    function resetTimer() {
        clearTimeout(timeout);
        timeout = setTimeout(logout, 10 * 60 * 1000); // 10 minutes
    }

    function logout() {
        dotNetHelper.invokeMethodAsync('IdleLogout');
    }

    window.addEventListener('mousemove', resetTimer);
    window.addEventListener('keydown', resetTimer);
    window.addEventListener('click', resetTimer);

    resetTimer(); 
};