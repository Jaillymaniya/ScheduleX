window.downloadFileFromBase64 = (fileName, base64Data) => {
    try {
        const link = document.createElement('a');
        link.download = fileName;
        link.href = "data:application/octet-stream;base64," + base64Data;

        document.body.appendChild(link);
        link.click();
        document.body.removeChild(link);
    } catch (e) {
        console.error("Download error:", e);
        alert("Failed to download file");
    }
};