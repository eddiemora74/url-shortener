window.clipboardCopy = {
    copyText: function(text) {
        navigator.clipboard.writeText(text).then(function () {
            const clipboardToast = document.getElementById('clipboard-copy-toast')
            const toastBootstrap = bootstrap.Toast.getOrCreateInstance(clipboardToast)
            toastBootstrap.show();
        })
            .catch(function (error) {
                alert(error);
            });
    }
};

function showModal(modalId) {
    const modalElement = document.getElementById(modalId);
    const modalInstance = bootstrap.Modal.getOrCreateInstance(modalElement);
    modalInstance.show();
}
