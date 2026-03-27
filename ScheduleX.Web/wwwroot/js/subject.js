function showToast(msg, type) {
    alert(msg); // replace later with nice toast
}

function downloadFile(name, bytes) {
    const blob = new Blob([bytes]);
    const link = document.createElement("a");
    link.href = URL.createObjectURL(blob);
    link.download = name;
    link.click();
}