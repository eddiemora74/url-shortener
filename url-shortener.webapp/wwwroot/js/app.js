window.clipboardCopy = {
    copyText: function(text) {
        navigator.clipboard.writeText(text);
    }
};

function triggerToast(toastId) {
    const clipboardToast = document.getElementById(toastId)
    const toastBootstrap = bootstrap.Toast.getOrCreateInstance(clipboardToast)
    toastBootstrap.show();
}

function showModal(modalId) {
    const modalElement = document.getElementById(modalId);
    const modalInstance = bootstrap.Modal.getOrCreateInstance(modalElement);
    modalInstance.show();
}

function hideModal(modalId) {
    const modalElement = document.getElementById(modalId);
    const modalInstance = bootstrap.Modal.getOrCreateInstance(modalElement);
    modalInstance.hide();
}
