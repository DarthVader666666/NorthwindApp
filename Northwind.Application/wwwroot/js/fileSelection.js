const fileInput = document.getElementById('fileInput')
const image = document.getElementById('image')

function onImageClickHandler() {
    fileInput.click()
}

function previewFile(input) {
    var file = fileInput.files[0];

    if (file) {
        var reader = new FileReader();

        reader.onload = function () {
            image.setAttribute("src", reader.result);
        }

        reader.readAsDataURL(file);
    }
}