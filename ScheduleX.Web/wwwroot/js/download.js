function downloadFileFromBase64(fileName, base64Data) {
    if (!base64Data) {
        alert("File data is empty!");
        return;
    }

    const link = document.createElement('a');

    link.href = "data:application/vnd.openxmlformats-officedocument.spreadsheetml.sheet;base64," + base64Data;
    link.download = fileName;

    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);
}