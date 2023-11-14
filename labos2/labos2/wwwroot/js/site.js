async function getData() {
    const response = await fetch('/api/home');
    const data = await response.json();
    console.log(data);

    var tbody = document.querySelector('#data-table tbody');

    data.forEach(function (item) {

        var row = tbody.insertRow();

        for (var key in item) {
            var cell = row.insertCell();
            cell.appendChild(document.createTextNode(item[key]));
        }
    })
};

window.onload = getData;

const downloadButton = document.getElementById('csv');
downloadButton.addEventListener('click', function () {

    const filteredRows = dataTable.querySelectorAll("tbody tr");

    let csvContent = "data:text/csv;charset=utf-8,";

    filteredRows.forEach(function (row) {
        const rowData = Array.from(row.cells).map(cell => cell.innerText);
        if (row.style.display === '') {
            csvContent += rowData.join(",") + "\n";
        }
    });

    const encodedUri = encodeURI(csvContent);
    const link = document.createElement("a");
    link.setAttribute("href", encodedUri);
    link.setAttribute("download", "filter.csv");
    document.body.appendChild(link);

    link.click();
});

const downloadJsonButton = document.getElementById('json');
downloadJsonButton.addEventListener('click', function () {

    const filteredRows = dataTable.querySelectorAll('tbody tr');

    const jsonData = [];

    filteredRows.forEach(function (row) {
        const rowData = {};
        Array.from(row.cells).forEach((cell, index) => {
            const headerText = dataTable.querySelector('thead tr').cells[index].innerText;
            if (row.style.display === '') {
                rowData[headerText] = cell.innerText;
            }
        });
        if (row.style.display === '') {
            jsonData.push(rowData);
        }
    });

    const jsonString = JSON.stringify(jsonData, null, 2);

    const encodedUri = encodeURI(`data:text/json;charset=utf-8,${jsonString}`);
    const link = document.createElement("a");
    link.setAttribute("href", encodedUri);
    link.setAttribute("download", "filter.json");
    document.body.appendChild(link);

    link.click();
});



const categoryDropdown = document.getElementById('category');
const searchInput = document.getElementById('searchInput');
const dataTable = document.getElementById('data-table');

function filterTable() {
    const query = searchInput.value.toLowerCase();
    const category = categoryDropdown.value;
    const rows = dataTable.querySelectorAll('tbody tr');

    if (category == "Sva polja") {
        rows.forEach(row => {
            const textContent = row.textContent.toLowerCase();

            if (textContent.includes(query)) {
                row.style.display = '';
            } else {
                row.style.display = 'none';
            }
        });
    } else {
        rows.forEach(row => {
            const cellContent = row.querySelector(`td:nth-child(${getColumnIndex(category)})`).textContent.toLowerCase();
            if (cellContent.includes(query)) {
                row.style.display = '';
            } else {
                row.style.display = 'none';
            }
        });
    }
}

function getColumnIndex(category) {
    const headerRow = dataTable.querySelector('thead tr');
    const headers = Array.from(headerRow.children);
    return headers.findIndex(header => header.textContent.toLowerCase() === category.toLowerCase()) + 1;
}

categoryDropdown.addEventListener('change', filterTable);
searchInput.addEventListener('input', filterTable);

function changeRoute() {

    window.location.href = "datatable.html";
}