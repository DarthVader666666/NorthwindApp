const checkboxes = Array.from(document.querySelectorAll("input[type='checkbox']:not(#selectAllCheckbox)"));
const deleteButton = document.getElementById("deleteButton");
const createButton = document.getElementById("createButton");
const selectAllCheckbox = document.getElementById("selectAllCheckbox");

function onChangeSelectAllHandler(checked) {
    if (checked) {
        checkboxes.forEach((item) => {
            item.checked = true;
            onChangeHandler(item);
        });

        createButton.hidden = true;
        deleteButton.hidden = false;
    }
    else {
        checkboxes.forEach((item) => {
            item.checked = false;
            onChangeHandler(item);
        });

        createButton.hidden = false;
        deleteButton.hidden = true;
    }
}

function onChangeHandler(element) {
    if (element.checked) {
        createButton.hidden = true;
        deleteButton.hidden = false;
    }
    else {
        if (!anyCheckboxesChecked()) {
            selectAllCheckbox.checked = false;
            createButton.hidden = false;
            deleteButton.hidden = true;
        }
    }
}

function anyCheckboxesChecked() {
    const checkedCheckBoxes = checkboxes.filter((item) => item.checked);
    return checkedCheckBoxes.length > 0;
}