let ids = [];
const checkboxes = Array.from(document.querySelectorAll("input[type='checkbox']:not(#selectAllCheckbox)"));
const deleteButton = document.getElementById("deleteButton");
const selectAllCheckbox = document.getElementById("selectAllCheckbox");

function onChangeSelectAllHandler(checked) {
    if (checked) {
        checkboxes.forEach((item) => {
            item.checked = true;
            onChangeHandler(item);
        });

        deleteButton.disabled = false;
    }
    else {
        checkboxes.forEach((item) => {
            item.checked = false;
            onChangeHandler(item);
        });

        deleteButton.disabled = true;
    }
}

function onChangeHandler(element) {
    if (element.checked) {
        ids.push(element.id);
        deleteButton.disabled = false;
    }
    else {
        const index = ids.indexOf(element.id);
        ids.splice(index, 1);

        if (!anyCheckboxesChecked()) {
            selectAllCheckbox.checked = false;
            deleteButton.disabled = true;
        }
    }
}

function anyCheckboxesChecked() {
    return checkboxes.filter((item) => item.checked).length > 0;
}