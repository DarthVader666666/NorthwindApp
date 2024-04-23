const checkboxes = Array.from(document.querySelectorAll("input[type='checkbox']:not(#selectAllCheckbox)"));
const deleteButton = document.getElementById("deleteButton");
const selectAllCheckbox = document.getElementById("selectAllCheckbox");
const selectAllSpan = document.getElementById("selectAllSpan");

function onChangeSelectAllHandler(checked) {
    if (checked) {
        checkboxes.forEach((item) => {
            item.checked = true;
            onChangeHandler(item);
        });

        deleteButton.disabled = false;
        deleteButton.hidden = false;
    }
    else {
        checkboxes.forEach((item) => {
            item.checked = false;
            onChangeHandler(item);
        });

        deleteButton.disabled = true;
        deleteButton.hidden = true;
    }
}

function onChangeHandler(element) {
    if (element.checked) {
        deleteButton.disabled = false;
        deleteButton.hidden = false;
    }
    else {
        if (!anyCheckboxesChecked()) {
            selectAllCheckbox.checked = false;
            deleteButton.disabled = true;
            deleteButton.hidden = true;
        }
    }
}

function anyCheckboxesChecked() {
    return checkboxes.filter((item) => item.checked).length > 0;
}