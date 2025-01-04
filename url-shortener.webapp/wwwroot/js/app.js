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